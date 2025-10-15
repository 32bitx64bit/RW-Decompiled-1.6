using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public abstract class Verb_Spray : Verb
{
	protected List<IntVec3> path = new List<IntVec3>();

	protected Vector3 initialTargetPosition;

	protected override int ShotsPerBurst => base.BurstShotCount;

	public override float? AimAngleOverride
	{
		get
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (state == VerbState.Bursting && Available())
			{
				return (path[ShotsPerBurst - burstShotsLeft].ToVector3Shifted() - caster.DrawPos).AngleFlat();
			}
			return null;
		}
	}

	protected override bool TryCastShot()
	{
		if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
		{
			return false;
		}
		ShootLine resultingLine;
		bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out resultingLine);
		if (verbProps.stopBurstWithoutLos && !flag)
		{
			return false;
		}
		if (base.EquipmentSource != null && burstShotsLeft <= 1)
		{
			base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
			base.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
		}
		HitCell(path[ShotsPerBurst - burstShotsLeft]);
		lastShotTick = Find.TickManager.TicksGame;
		return true;
	}

	public override bool Available()
	{
		return ShotsPerBurst - burstShotsLeft >= 0;
	}

	public override void WarmupComplete()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		burstShotsLeft = ShotsPerBurst;
		state = VerbState.Bursting;
		initialTargetPosition = currentTarget.CenterVector3;
		PreparePath();
		TryCastNextBurstShot();
	}

	protected abstract void PreparePath();

	protected virtual void HitCell(IntVec3 cell)
	{
		verbProps.sprayEffecterDef?.Spawn(caster.Position, cell, caster.Map);
	}

	public override void ExposeData()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		Scribe_Collections.Look(ref path, "path", LookMode.Value);
		Scribe_Values.Look(ref initialTargetPosition, "initialTargetPosition");
		if (Scribe.mode == LoadSaveMode.PostLoadInit && path == null)
		{
			path = new List<IntVec3>();
		}
	}
}
