using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_FireOverlay : CompProperties
{
	public float fireSize = 1f;

	public float finalFireSize = 1f;

	public float fireGrowthDurationTicks = -1f;

	public Vector3 offset;

	public Vector3? offsetNorth;

	public Vector3? offsetSouth;

	public Vector3? offsetWest;

	public Vector3? offsetEast;

	public CompProperties_FireOverlay()
	{
		compClass = typeof(CompFireOverlay);
	}

	public Vector3 DrawOffsetForRot(Rot4 rot)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		return (Vector3)(rot.AsInt switch
		{
			0 => ((_003F?)offsetNorth) ?? offset, 
			1 => ((_003F?)offsetEast) ?? offset, 
			2 => ((_003F?)offsetSouth) ?? offset, 
			3 => ((_003F?)offsetWest) ?? offset, 
			_ => offset, 
		});
	}

	public override void DrawGhost(IntVec3 center, Rot4 rot, ThingDef thingDef, Color ghostCol, AltitudeLayer drawAltitude, Thing thing = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Graphic graphic = GhostUtility.GhostGraphicFor(CompFireOverlay.FireGraphic, thingDef, ghostCol);
		Vector3 loc = center.ToVector3ShiftedWithAltitude(drawAltitude) + thingDef.graphicData.DrawOffsetForRot(rot) + DrawOffsetForRot(rot);
		graphic.DrawFromDef(loc, rot, thingDef);
	}
}
