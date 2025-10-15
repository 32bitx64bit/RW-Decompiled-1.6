using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Verb_ShootBeam : Verb
{
	private List<Vector3> path = new List<Vector3>();

	private List<Vector3> tmpPath = new List<Vector3>();

	private int ticksToNextPathStep;

	private Vector3 initialTargetPosition;

	private MoteDualAttached mote;

	private Effecter endEffecter;

	private Sustainer sustainer;

	private HashSet<IntVec3> pathCells = new HashSet<IntVec3>();

	private HashSet<IntVec3> tmpPathCells = new HashSet<IntVec3>();

	private HashSet<IntVec3> tmpHighlightCells = new HashSet<IntVec3>();

	private HashSet<IntVec3> tmpSecondaryHighlightCells = new HashSet<IntVec3>();

	private HashSet<IntVec3> hitCells = new HashSet<IntVec3>();

	private const int NumSubdivisionsPerUnitLength = 1;

	protected override int ShotsPerBurst => base.BurstShotCount;

	public float ShotProgress => (float)ticksToNextPathStep / (float)base.TicksBetweenBurstShots;

	public Vector3 InterpolatedPosition
	{
		get
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = base.CurrentTarget.CenterVector3 - initialTargetPosition;
			return Vector3.Lerp(path[burstShotsLeft], path[Mathf.Min(burstShotsLeft + 1, path.Count - 1)], ShotProgress) + val;
		}
	}

	public override float? AimAngleOverride
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (state != VerbState.Bursting)
			{
				return null;
			}
			return (InterpolatedPosition - caster.DrawPos).AngleFlat();
		}
	}

	public override void DrawHighlight(LocalTargetInfo target)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		base.DrawHighlight(target);
		CalculatePath(target.CenterVector3, tmpPath, tmpPathCells, addRandomOffset: false);
		foreach (IntVec3 tmpPathCell in tmpPathCells)
		{
			ShootLine resultingLine;
			bool flag = TryFindShootLineFromTo(caster.Position, target, out resultingLine);
			if ((verbProps.stopBurstWithoutLos && !flag) || !TryGetHitCell(resultingLine.Source, tmpPathCell, out var hitCell))
			{
				continue;
			}
			tmpHighlightCells.Add(hitCell);
			if (!verbProps.beamHitsNeighborCells)
			{
				continue;
			}
			foreach (IntVec3 beamHitNeighbourCell in GetBeamHitNeighbourCells(resultingLine.Source, hitCell))
			{
				if (!tmpHighlightCells.Contains(beamHitNeighbourCell))
				{
					tmpSecondaryHighlightCells.Add(beamHitNeighbourCell);
				}
			}
		}
		tmpSecondaryHighlightCells.RemoveWhere((IntVec3 x) => tmpHighlightCells.Contains(x));
		if (tmpHighlightCells.Any())
		{
			GenDraw.DrawFieldEdges(tmpHighlightCells.ToList(), (Color)(((_003F?)verbProps.highlightColor) ?? Color.white));
		}
		if (tmpSecondaryHighlightCells.Any())
		{
			GenDraw.DrawFieldEdges(tmpSecondaryHighlightCells.ToList(), (Color)(((_003F?)verbProps.secondaryHighlightColor) ?? Color.white));
		}
		tmpHighlightCells.Clear();
		tmpSecondaryHighlightCells.Clear();
	}

	protected override bool TryCastShot()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
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
		if (base.EquipmentSource != null)
		{
			base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
			base.EquipmentSource.GetComp<CompApparelReloadable>()?.UsedOnce();
		}
		lastShotTick = Find.TickManager.TicksGame;
		ticksToNextPathStep = base.TicksBetweenBurstShots;
		IntVec3 targetCell = InterpolatedPosition.Yto0().ToIntVec3();
		if (!TryGetHitCell(resultingLine.Source, targetCell, out var hitCell))
		{
			return true;
		}
		HitCell(hitCell, resultingLine.Source);
		if (verbProps.beamHitsNeighborCells)
		{
			hitCells.Add(hitCell);
			foreach (IntVec3 beamHitNeighbourCell in GetBeamHitNeighbourCells(resultingLine.Source, hitCell))
			{
				if (!hitCells.Contains(beamHitNeighbourCell))
				{
					float damageFactor = (pathCells.Contains(beamHitNeighbourCell) ? 1f : 0.5f);
					HitCell(beamHitNeighbourCell, resultingLine.Source, damageFactor);
					hitCells.Add(beamHitNeighbourCell);
				}
			}
		}
		return true;
	}

	protected bool TryGetHitCell(IntVec3 source, IntVec3 targetCell, out IntVec3 hitCell)
	{
		IntVec3 intVec = GenSight.LastPointOnLineOfSight(source, targetCell, (IntVec3 c) => c.InBounds(caster.Map) && c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (verbProps.beamCantHitWithinMinRange && intVec.DistanceTo(source) < verbProps.minRange)
		{
			hitCell = default(IntVec3);
			return false;
		}
		hitCell = (intVec.IsValid ? intVec : targetCell);
		return intVec.IsValid;
	}

	protected IntVec3 GetHitCell(IntVec3 source, IntVec3 targetCell)
	{
		TryGetHitCell(source, targetCell, out var hitCell);
		return hitCell;
	}

	protected IEnumerable<IntVec3> GetBeamHitNeighbourCells(IntVec3 source, IntVec3 pos)
	{
		if (!verbProps.beamHitsNeighborCells)
		{
			yield break;
		}
		for (int i = 0; i < 4; i++)
		{
			IntVec3 intVec = pos + GenAdj.CardinalDirections[i];
			if (intVec.InBounds(Caster.Map) && (!verbProps.beamHitsNeighborCellsRequiresLOS || GenSight.LineOfSight(source, intVec, caster.Map)))
			{
				yield return intVec;
			}
		}
	}

	public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
	{
		return base.TryStartCastOn(verbProps.beamTargetsGround ? ((LocalTargetInfo)castTarg.Cell) : castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast);
	}

	public override void BurstingTick()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		ticksToNextPathStep--;
		Vector3 val = InterpolatedPosition;
		IntVec3 intVec = val.ToIntVec3();
		Vector3 val2 = InterpolatedPosition - caster.Position.ToVector3Shifted();
		float num = val2.MagnitudeHorizontal();
		Vector3 val3 = val2.Yto0();
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(caster.Position, intVec, (IntVec3 c) => c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (intVec2.IsValid)
		{
			num -= (intVec - intVec2).LengthHorizontal;
			val = caster.Position.ToVector3Shifted() + normalized * num;
			intVec = val.ToIntVec3();
		}
		Vector3 offsetA = normalized * verbProps.beamStartOffset;
		Vector3 val4 = val - intVec.ToVector3Shifted();
		if (mote != null)
		{
			mote.UpdateTargets(new TargetInfo(caster.Position, caster.Map), new TargetInfo(intVec, caster.Map), offsetA, val4);
			mote.Maintain();
		}
		if (verbProps.beamGroundFleckDef != null && Rand.Chance(verbProps.beamFleckChancePerTick))
		{
			FleckMaker.Static(val, caster.Map, verbProps.beamGroundFleckDef);
		}
		if (endEffecter == null && verbProps.beamEndEffecterDef != null)
		{
			endEffecter = verbProps.beamEndEffecterDef.Spawn(intVec, caster.Map, val4);
		}
		if (endEffecter != null)
		{
			endEffecter.offset = val4;
			endEffecter.EffectTick(new TargetInfo(intVec, caster.Map), TargetInfo.Invalid);
			endEffecter.ticksLeft--;
		}
		if (verbProps.beamLineFleckDef != null)
		{
			float num2 = 1f * num;
			for (int i = 0; (float)i < num2; i++)
			{
				if (Rand.Chance(verbProps.beamLineFleckChanceCurve.Evaluate((float)i / num2)))
				{
					Vector3 val5 = (float)i * normalized - normalized * Rand.Value + normalized / 2f;
					FleckMaker.Static(caster.Position.ToVector3Shifted() + val5, caster.Map, verbProps.beamLineFleckDef);
				}
			}
		}
		sustainer?.Maintain();
	}

	public override void WarmupComplete()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		burstShotsLeft = ShotsPerBurst;
		state = VerbState.Bursting;
		initialTargetPosition = currentTarget.CenterVector3;
		CalculatePath(currentTarget.CenterVector3, path, pathCells);
		hitCells.Clear();
		if (verbProps.beamMoteDef != null)
		{
			mote = MoteMaker.MakeInteractionOverlay(verbProps.beamMoteDef, caster, new TargetInfo(path[0].ToIntVec3(), caster.Map));
		}
		TryCastNextBurstShot();
		ticksToNextPathStep = base.TicksBetweenBurstShots;
		endEffecter?.Cleanup();
		if (verbProps.soundCastBeam != null)
		{
			sustainer = verbProps.soundCastBeam.TrySpawnSustainer(SoundInfo.InMap(caster, MaintenanceType.PerTick));
		}
	}

	private void CalculatePath(Vector3 target, List<Vector3> pathList, HashSet<IntVec3> pathCellsList, bool addRandomOffset = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		pathList.Clear();
		Vector3 val = (target - caster.Position.ToVector3Shifted()).Yto0();
		float magnitude = ((Vector3)(ref val)).magnitude;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = normalized.RotatedBy(-90f);
		float num = ((verbProps.beamFullWidthRange > 0f) ? Mathf.Min(magnitude / verbProps.beamFullWidthRange, 1f) : 1f);
		float num2 = (verbProps.beamWidth + 1f) * num / (float)ShotsPerBurst;
		Vector3 val3 = target.Yto0() - val2 * verbProps.beamWidth / 2f * num;
		pathList.Add(val3);
		for (int i = 0; i < ShotsPerBurst; i++)
		{
			Vector3 val4 = normalized * (Rand.Value * verbProps.beamMaxDeviation) - normalized / 2f;
			Vector3 val5 = Mathf.Sin(((float)i / (float)ShotsPerBurst + 0.5f) * MathF.PI * 57.29578f) * verbProps.beamCurvature * -normalized - normalized * verbProps.beamMaxDeviation / 2f;
			if (addRandomOffset)
			{
				pathList.Add(val3 + (val4 + val5) * num);
			}
			else
			{
				pathList.Add(val3 + val5 * num);
			}
			val3 += val2 * num2;
		}
		pathCellsList.Clear();
		foreach (Vector3 path in pathList)
		{
			pathCellsList.Add(path.ToIntVec3());
		}
	}

	private bool CanHit(Thing thing)
	{
		if (!thing.Spawned)
		{
			return false;
		}
		return !CoverUtility.ThingCovered(thing, caster.Map);
	}

	private void HitCell(IntVec3 cell, IntVec3 sourceCell, float damageFactor = 1f)
	{
		if (cell.InBounds(caster.Map))
		{
			ApplyDamage(VerbUtility.ThingsToHit(cell, caster.Map, CanHit).RandomElementWithFallback(), sourceCell, damageFactor);
			if (verbProps.beamSetsGroundOnFire && Rand.Chance(verbProps.beamChanceToStartFire))
			{
				FireUtility.TryStartFireIn(cell, caster.Map, 1f, caster);
			}
		}
	}

	private void ApplyDamage(Thing thing, IntVec3 sourceCell, float damageFactor = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 intVec = InterpolatedPosition.Yto0().ToIntVec3();
		IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(sourceCell, intVec, (IntVec3 c) => c.InBounds(caster.Map) && c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
		if (intVec2.IsValid)
		{
			intVec = intVec2;
		}
		Map map = caster.Map;
		if (thing == null || verbProps.beamDamageDef == null)
		{
			return;
		}
		float angleFlat = (currentTarget.Cell - caster.Position).AngleFlat;
		BattleLogEntry_RangedImpact log = new BattleLogEntry_RangedImpact(caster, thing, currentTarget.Thing, base.EquipmentSource.def, null, null);
		DamageInfo dinfo;
		if (verbProps.beamTotalDamage > 0f)
		{
			float num = verbProps.beamTotalDamage / (float)pathCells.Count;
			num *= damageFactor;
			dinfo = new DamageInfo(verbProps.beamDamageDef, num, verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, caster, null, base.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, currentTarget.Thing);
		}
		else
		{
			float amount = (float)verbProps.beamDamageDef.defaultDamage * damageFactor;
			dinfo = new DamageInfo(verbProps.beamDamageDef, amount, verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, caster, null, base.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, currentTarget.Thing);
		}
		thing.TakeDamage(dinfo).AssociateWithLog(log);
		if (thing.CanEverAttachFire())
		{
			float chance = ((verbProps.flammabilityAttachFireChanceCurve == null) ? verbProps.beamChanceToAttachFire : verbProps.flammabilityAttachFireChanceCurve.Evaluate(thing.GetStatValue(StatDefOf.Flammability)));
			if (Rand.Chance(chance))
			{
				thing.TryAttachFire(verbProps.beamFireSizeRange.RandomInRange, caster);
			}
		}
		else if (Rand.Chance(verbProps.beamChanceToStartFire))
		{
			FireUtility.TryStartFireIn(intVec, map, verbProps.beamFireSizeRange.RandomInRange, caster, verbProps.flammabilityAttachFireChanceCurve);
		}
	}

	public override void ExposeData()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		Scribe_Collections.Look(ref path, "path", LookMode.Value);
		Scribe_Values.Look(ref ticksToNextPathStep, "ticksToNextPathStep", 0);
		Scribe_Values.Look(ref initialTargetPosition, "initialTargetPosition");
		if (Scribe.mode == LoadSaveMode.PostLoadInit && path == null)
		{
			path = new List<Vector3>();
		}
	}
}
