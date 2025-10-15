using UnityEngine;
using Verse;

namespace RimWorld;

public class CompRitualEffect_IntervalSpawnArea : CompRitualEffect_IntervalSpawnBurst
{
	protected new CompProperties_RitualEffectIntervalSpawnArea Props => (CompProperties_RitualEffectIntervalSpawnArea)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		CellRect cellRect = CellRect.CenteredOn(ritual.selectedTarget.Cell, Props.area.x / 2, Props.area.z / 2).ClipInsideMap(ritual.Map);
		if (Props.smoothEdges)
		{
			return cellRect.Cells.RandomElementByWeight((IntVec3 c) => 1f / Mathf.Max((c - ritual.selectedTarget.Cell).LengthHorizontal, 1f)).ToVector3Shifted() + Rand.UnitVector3 * 0.5f;
		}
		return cellRect.RandomVector3;
	}
}
