using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_ShowProjectileInterceptorRadius : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		CompProperties_ProjectileInterceptor compProperties = def.GetCompProperties<CompProperties_ProjectileInterceptor>();
		if (compProperties != null)
		{
			GenDraw.DrawCircleOutline(center.ToVector3Shifted(), compProperties.radius);
		}
	}
}
