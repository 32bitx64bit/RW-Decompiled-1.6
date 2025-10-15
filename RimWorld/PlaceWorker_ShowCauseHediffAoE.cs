using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_ShowCauseHediffAoE : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		CompProperties_CauseHediff_AoE compProperties = def.GetCompProperties<CompProperties_CauseHediff_AoE>();
		if (compProperties != null)
		{
			GenDraw.DrawRadiusRing(center, compProperties.range, Color.white);
		}
	}
}
