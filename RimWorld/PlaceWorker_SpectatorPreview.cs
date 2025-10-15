using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class PlaceWorker_SpectatorPreview : PlaceWorker
{
	private static Graphic spectatingPawnsGraphic;

	private static Graphic SpectatingPawnsGraphic
	{
		get
		{
			if (spectatingPawnsGraphic == null)
			{
				spectatingPawnsGraphic = GraphicDatabase.Get<Graphic_Multi>("UI/Overlays/PawnsSpectating", ShaderDatabase.Transparent);
			}
			return spectatingPawnsGraphic;
		}
	}

	public static void DrawSpectatorPreview(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, bool useArrow, out CellRect rect, Thing thing = null)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		rect = GenAdj.OccupiedRect(center, rot, def.size);
		RitualFocusProperties ritualFocus = def.ritualFocus;
		if (ritualFocus == null && def.entityDefToBuild is ThingDef thingDef)
		{
			ritualFocus = thingDef.ritualFocus;
		}
		if (ritualFocus == null)
		{
			return;
		}
		foreach (SpectateRectSide allSelectedItem in ritualFocus.allowedSpectateSides.GetAllSelectedItems<SpectateRectSide>())
		{
			if (allSelectedItem.ValidSingleSide())
			{
				Rot4 rot2 = allSelectedItem.Rotated(rot).AsRot4();
				if (useArrow)
				{
					GenDraw.DrawArrowRotated(allSelectedItem.GraphicOffsetForRect(center, rect, rot, Vector2.zero), rot2.AsAngle, ghost: true);
				}
				Vector2 val = (rot2.IsHorizontal ? new Vector2(2f, 4f) : new Vector2(4f, 2f));
				bool flag = (allSelectedItem & SpectateRectSide.Horizontal) != SpectateRectSide.Horizontal;
				Vector2 val2 = val - new Vector2(0.5f, 0.5f);
				GenDraw.DrawMeshNowOrLater(SpectatingPawnsGraphic.MeshAt(rot2.Opposite), Matrix4x4.TRS(allSelectedItem.GraphicOffsetForRect(center, rect, rot, flag ? val2 : (-val2)) + new Vector3(0f, 8f, 0f), Quaternion.identity, new Vector3(val.x, 1f, val.y)), SpectatingPawnsGraphic.MatAt(rot2.Opposite), drawNow: false);
			}
		}
	}

	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DrawSpectatorPreview(def, center, rot, ghostCol, useArrow: true, out var _, thing);
	}
}
