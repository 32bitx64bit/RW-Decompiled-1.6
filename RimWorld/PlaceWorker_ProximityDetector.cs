using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_ProximityDetector : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		GenDraw.DrawRadiusRing(center, 19.9f, Color.white);
	}
}
