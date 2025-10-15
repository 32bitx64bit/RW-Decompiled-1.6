using LudeonTK;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Verb_ArcSprayIncinerator : Verb_ShootBeam
{
	[TweakValue("Incinerator", 0f, 10f)]
	public static float DistanceToLifetimeScalar = 5f;

	[TweakValue("Incinerator", -2f, 7f)]
	public static float BarrelOffset = 5f;

	private IncineratorSpray sprayer;

	public override void WarmupComplete()
	{
		sprayer = GenSpawn.Spawn(ThingDefOf.IncineratorSpray, caster.Position, caster.Map) as IncineratorSpray;
		base.WarmupComplete();
		Find.BattleLog.Add(new BattleLogEntry_RangedFire(caster, currentTarget.HasThing ? currentTarget.Thing : null, base.EquipmentSource?.def, null, burst: false));
	}

	protected override bool TryCastShot()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		bool result = base.TryCastShot();
		Vector3 val = base.InterpolatedPosition.Yto0();
		IntVec3 intVec = val.ToIntVec3();
		Vector3 drawPos = caster.DrawPos;
		Vector3 val2 = val - drawPos;
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		drawPos += normalized * BarrelOffset;
		MoteDualAttached moteDualAttached = MoteMaker.MakeInteractionOverlay(A: new TargetInfo(caster.Position, caster.Map), moteDef: ThingDefOf.Mote_IncineratorBurst, B: new TargetInfo(intVec, caster.Map));
		float num = Vector3.Distance(val, drawPos);
		float num2 = ((num < BarrelOffset) ? 0.5f : 1f);
		IncineratorSpray incineratorSpray = sprayer;
		if (incineratorSpray != null)
		{
			IncineratorProjectileMotion proj = new IncineratorProjectileMotion
			{
				mote = moteDualAttached,
				targetDest = intVec,
				worldSource = drawPos,
				worldTarget = val
			};
			val2 = val - drawPos;
			proj.moveVector = ((Vector3)(ref val2)).normalized;
			proj.startScale = 1f * num2;
			proj.endScale = (1f + Rand.Range(0.1f, 0.4f)) * num2;
			proj.lifespanTicks = Mathf.FloorToInt(num * DistanceToLifetimeScalar);
			incineratorSpray.Add(proj);
			return result;
		}
		return result;
	}
}
