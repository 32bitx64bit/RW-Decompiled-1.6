using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompAbilityEffect_Burner : CompAbilityEffect
{
	public new CompProperties_AbilityBurner Props => (CompProperties_AbilityBurner)props;

	public override IEnumerable<PreCastAction> GetPreCastActions()
	{
		yield return new PreCastAction
		{
			action = delegate(LocalTargetInfo a, LocalTargetInfo _)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				Vector3 drawPos = parent.pawn.DrawPos;
				IntVec3 intVec = drawPos.Yto0().ToIntVec3();
				Map map = parent.pawn.Map;
				IncineratorSpray incineratorSpray = GenSpawn.Spawn(ThingDefOf.IncineratorSpray, intVec, map) as IncineratorSpray;
				int numStreams = Props.numStreams;
				Vector3 val = a.CenterVector3 - drawPos;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				for (int i = 0; i < numStreams; i++)
				{
					float angle = Rand.Range(0f - Props.coneSizeDegrees, Props.coneSizeDegrees);
					Vector3 val2 = normalized.RotatedBy(angle);
					Vector3 vect = drawPos + val2 * (Props.range + Rand.Value * Props.rangeNoise);
					IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(intVec, vect.ToIntVec3(), (IntVec3 c) => c.CanBeSeenOverFast(map), skipFirstCell: true);
					if (!intVec2.IsValid)
					{
						intVec2 = vect.ToIntVec3();
					}
					float num = Vector3.Distance(intVec2.ToVector3(), drawPos);
					float num2 = Mathf.Clamp01(num / Props.sizeReductionDistanceThreshold);
					val = intVec2.ToVector3() - drawPos;
					if (!(Vector3.Dot(((Vector3)(ref val)).normalized, val2) <= 0.5f))
					{
						MoteDualAttached mote = MoteMaker.MakeInteractionOverlay(ThingDefOf.Mote_IncineratorBurst, new TargetInfo(intVec, map), new TargetInfo(intVec2, map));
						incineratorSpray.Add(new IncineratorProjectileMotion
						{
							mote = mote,
							targetDest = a.Cell,
							worldSource = drawPos + val2 * Props.barrelOffsetDistance,
							worldTarget = intVec2.ToVector3(),
							moveVector = val2,
							startScale = Rand.Range(0.8f, 1.2f) * num2,
							endScale = (1f + Rand.Range(0.1f, 0.4f)) * num2,
							lifespanTicks = Mathf.FloorToInt(num * 5f) + Rand.Range(-Props.lifespanNoise, Props.lifespanNoise)
						});
						map.effecterMaintainer.AddEffecterToMaintain(Props.effecterDef.Spawn(intVec2, map), intVec2, 100);
					}
				}
			},
			ticksAwayFromCast = 5
		};
	}
}
