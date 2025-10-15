using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class PlaceWorker_FuelingPort : PlaceWorker
{
	private static readonly Material FuelingPortCellMaterial = MaterialPool.MatFrom("UI/Overlays/FuelingPort", ShaderDatabase.Transparent);

	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		Map currentMap = Find.CurrentMap;
		if (def.building != null && def.building.hasFuelingPort && FuelingPortUtility.GetFuelingPortCell(center, rot).Standable(currentMap))
		{
			DrawFuelingPortCell(center, rot);
		}
	}

	public static void DrawFuelingPortCell(IntVec3 center, Rot4 rot)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = FuelingPortUtility.GetFuelingPortCell(center, rot).ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
		Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, FuelingPortCellMaterial, 0);
	}
}
