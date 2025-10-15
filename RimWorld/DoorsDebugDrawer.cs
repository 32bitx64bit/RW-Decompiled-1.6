using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class DoorsDebugDrawer
{
	public static void DrawDebug()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!DebugViewSettings.drawDoorsDebug)
		{
			return;
		}
		CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
		List<Thing> list = Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
		Color col = default(Color);
		for (int i = 0; i < list.Count; i++)
		{
			if (!currentViewRect.Contains(list[i].Position) || !(list[i] is Building_Door building_Door))
			{
				continue;
			}
			if (building_Door.FreePassage)
			{
				((Color)(ref col))._002Ector(0f, 1f, 0f, 0.5f);
			}
			else
			{
				((Color)(ref col))._002Ector(1f, 0f, 0f, 0.5f);
			}
			foreach (IntVec3 item in building_Door.OccupiedRect())
			{
				CellRenderer.RenderCell(item, SolidColorMaterials.SimpleSolidColorMaterial(col));
			}
		}
	}
}
