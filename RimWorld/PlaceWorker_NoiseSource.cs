using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_NoiseSource : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		CompProperties_NoiseSource compProperties = def.GetCompProperties<CompProperties_NoiseSource>();
		if (compProperties != null)
		{
			GenDraw.DrawRadiusRing(center, compProperties.radius, Color.white);
		}
	}
}
