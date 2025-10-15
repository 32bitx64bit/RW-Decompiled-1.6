using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class OrbitLayer : PlanetLayer
{
	public OrbitLayer()
	{
	}

	public OrbitLayer(int layerId, PlanetLayerDef def, float radius, Vector3 origin, Vector3 viewCenter, float viewAngle, int subdivisions, float extraCameraAltitude, float backgroundWorldCameraOffset, float backgroundWorldCameraParallaxDistancePer100Cells)
		: base(layerId, def, radius, origin, viewCenter, viewAngle, subdivisions, extraCameraAltitude, backgroundWorldCameraOffset, backgroundWorldCameraParallaxDistancePer100Cells)
	{
	}//IL_0004: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public override AcceptanceReport CanSelectLayer()
	{
		AcceptanceReport acceptanceReport = base.CanSelectLayer();
		if (!acceptanceReport)
		{
			return acceptanceReport;
		}
		if (!Find.WorldObjects.AnyWorldObjectOnLayer(this))
		{
			return "CannotSelectOrbitReason".Translate();
		}
		return true;
	}
}
