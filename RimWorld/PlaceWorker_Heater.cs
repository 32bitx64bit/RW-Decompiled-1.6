using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_Heater : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Map currentMap = Find.CurrentMap;
		Room room = center.GetRoom(currentMap);
		if (room != null && !room.UsesOutdoorTemperature)
		{
			GenDraw.DrawFieldEdges(room.Cells.ToList(), GenTemperature.ColorRoomHot);
		}
	}
}
