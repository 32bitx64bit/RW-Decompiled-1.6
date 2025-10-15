using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PlaceWorker_RitualSeat : PlaceWorker
{
	public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		ThingDef thingDef = ((def.entityDefToBuild != null) ? ((ThingDef)def.entityDefToBuild) : def);
		List<Thing> list = Find.CurrentMap.listerThings.ThingsOfDef(ThingDefOf.Lectern);
		for (int i = 0; i < list.Count; i++)
		{
			Thing thing2 = list[i];
			if (!GatheringsUtility.InGatheringArea(center, thing2.Position, Find.CurrentMap) || !SpectatorCellFinder.IsCorrectlyRotatedChair(center, rot, thingDef, thing2.OccupiedRect()))
			{
				continue;
			}
			GenDraw.DrawLineBetween(GenThing.TrueCenter(center, rot, thingDef.size, thingDef.Altitude), thing2.TrueCenter(), SimpleColor.Yellow);
			foreach (Thing item in PlaceWorker_RitualPosition.GetRitualFocusInRange(thing2.Position, thing2))
			{
				if (GatheringsUtility.InGatheringArea(center, item.Position, Find.CurrentMap) && SpectatorCellFinder.IsCorrectlyRotatedChair(center, rot, thingDef, item.OccupiedRect()))
				{
					GenDraw.DrawLineBetween(thing2.TrueCenter(), item.TrueCenter(), SimpleColor.Green);
				}
			}
		}
	}
}
