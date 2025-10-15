using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_CocoonTriggerRadius : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		CompProperties_WakeUpDormant compProperties = def.GetCompProperties<CompProperties_WakeUpDormant>();
		if (compProperties != null)
		{
			GenDraw.DrawRadiusRing(center, compProperties.wakeUpCheckRadius, Color.white);
		}
	}
}
