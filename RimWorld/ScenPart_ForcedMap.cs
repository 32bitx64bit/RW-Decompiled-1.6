using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ScenPart_ForcedMap : ScenPart
{
	public MapGeneratorDef mapGenerator;

	public PlanetLayerDef layerDef;

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Defs.Look(ref mapGenerator, "mapGenerator");
		Scribe_Defs.Look(ref layerDef, "layerDef");
	}

	public override void DoEditInterface(Listing_ScenEdit listing)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
		((Rect)(ref scenPartRect)).height = ScenPart.RowHeight;
		Text.Anchor = (TextAnchor)2;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref scenPartRect)).x - 200f, ((Rect)(ref scenPartRect)).y + ScenPart.RowHeight, 200f, ScenPart.RowHeight);
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 4f;
		Widgets.Label(rect, "ScenPart_ForcedMapPlanetLayer".Translate());
		Text.Anchor = (TextAnchor)0;
		if (Widgets.ButtonText(scenPartRect, mapGenerator.LabelCap))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (MapGeneratorDef item in DefDatabase<MapGeneratorDef>.AllDefs.Where((MapGeneratorDef d) => d.validScenarioMap))
			{
				MapGeneratorDef localFd2 = item;
				list.Add(new FloatMenuOption(localFd2.LabelCap, delegate
				{
					mapGenerator = localFd2;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		((Rect)(ref scenPartRect)).y = ((Rect)(ref scenPartRect)).y + ScenPart.RowHeight;
		if (!Widgets.ButtonText(scenPartRect, layerDef.LabelCap))
		{
			return;
		}
		List<FloatMenuOption> list2 = new List<FloatMenuOption>();
		foreach (PlanetLayerDef allDef in DefDatabase<PlanetLayerDef>.AllDefs)
		{
			PlanetLayerDef localFd = allDef;
			list2.Add(new FloatMenuOption(localFd.LabelCap, delegate
			{
				layerDef = localFd;
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list2));
	}

	public override void PostWorldGenerate()
	{
		PlanetTile planetTile = TileFinder.RandomStartingTile();
		PlanetLayer planetLayer = Find.WorldGrid.FirstLayerOfDef(layerDef);
		if (layerDef != PlanetLayerDefOf.Surface && planetLayer != null)
		{
			planetTile = planetLayer.GetClosestTile_NewTemp(planetTile, validSettlement: true);
		}
		Find.GameInitData.startingTile = planetTile;
		Find.GameInitData.mapGeneratorDef = mapGenerator;
	}
}
