using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_DarklightOverlay : CompProperties_FireOverlay
{
	public CompProperties_DarklightOverlay()
	{
		compClass = typeof(CompDarklightOverlay);
	}

	public override void DrawGhost(IntVec3 center, Rot4 rot, ThingDef thingDef, Color ghostCol, AltitudeLayer drawAltitude, Thing thing = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		GhostUtility.GhostGraphicFor(CompDarklightOverlay.DarklightGraphic, thingDef, ghostCol).DrawFromDef(center.ToVector3ShiftedWithAltitude(drawAltitude), rot, thingDef);
	}
}
