using UnityEngine;
using Verse;

namespace RimWorld;

public class CompRitualEffect_IntervalSpawnCircle : CompRitualEffect_IntervalSpawnBurst
{
	protected new CompProperties_RitualEffectIntervalSpawnCircle Props => (CompProperties_RitualEffectIntervalSpawnCircle)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		return CellRect.CenteredOn(ritual.selectedTarget.Cell, Props.area.x / 2, Props.area.z / 2).ClipInsideMap(ritual.Map).Cells.RandomElementByWeight(delegate(IntVec3 c)
		{
			float num = Mathf.Max(Mathf.Abs((c - ritual.selectedTarget.Cell).LengthHorizontal - Props.radius), 1f);
			return 1f / Mathf.Pow(num, Props.concentration);
		}).ToVector3Shifted() + Rand.UnitVector3 * 0.5f;
	}
}
