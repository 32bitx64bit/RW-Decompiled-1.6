using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld.Planet;

public class WorldGenStep_AncientRoads : WorldGenStep
{
	public float maximumSiteCurve;

	public float maximumSegmentCurviness;

	public override int SeedPart => 773428712;

	public override void GenerateFresh(string seed, PlanetLayer layer)
	{
		GenerateAncientRoads(layer);
	}

	private void GenerateAncientRoads(PlanetLayer layer)
	{
		Find.WorldPathGrid.RecalculateLayerPerceivedPathCosts(layer, 0);
		List<List<PlanetTile>> list = GenerateProspectiveRoads(layer);
		list.Sort((List<PlanetTile> lhs, List<PlanetTile> rhs) => -lhs.Count.CompareTo(rhs.Count));
		HashSet<PlanetTile> used = new HashSet<PlanetTile>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Any((PlanetTile elem) => used.Contains(elem)))
			{
				continue;
			}
			if (list[i].Count < 4)
			{
				break;
			}
			foreach (PlanetTile item in list[i])
			{
				used.Add(item);
			}
			for (int j = 0; j < list[i].Count - 1; j++)
			{
				float num = Find.WorldGrid.ApproxDistanceInTiles(list[i][j], list[i][j + 1]) * maximumSegmentCurviness;
				float costCutoff = num * 12000f;
				using WorldPath worldPath = layer.Pather.FindPath(list[i][j], list[i][j + 1], null, (float cost) => cost > costCutoff);
				if (worldPath == null || worldPath == WorldPath.NotFound)
				{
					continue;
				}
				List<PlanetTile> nodesReversed = worldPath.NodesReversed;
				if ((float)nodesReversed.Count > Find.WorldGrid.ApproxDistanceInTiles(list[i][j], list[i][j + 1]) * maximumSegmentCurviness)
				{
					continue;
				}
				for (int k = 0; k < nodesReversed.Count - 1; k++)
				{
					if (Find.WorldGrid.GetRoadDef(nodesReversed[k], nodesReversed[k + 1], visibleOnly: false) != null)
					{
						Find.WorldGrid.OverlayRoad(nodesReversed[k], nodesReversed[k + 1], RoadDefOf.AncientAsphaltHighway);
					}
					else
					{
						Find.WorldGrid.OverlayRoad(nodesReversed[k], nodesReversed[k + 1], RoadDefOf.AncientAsphaltRoad);
					}
				}
			}
		}
	}

	private List<List<PlanetTile>> GenerateProspectiveRoads(PlanetLayer layer)
	{
		List<PlanetTile> list = Find.World.genData.ancientSites[layer];
		List<List<PlanetTile>> list2 = new List<List<PlanetTile>>();
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list.Count; j++)
			{
				List<PlanetTile> list3 = new List<PlanetTile>();
				list3.Add(list[i]);
				List<PlanetTile> list4 = list;
				float ang = Find.World.grid.GetHeadingFromTo(list[i], list[j]);
				PlanetTile current = list[i];
				while (true)
				{
					list4 = list4.Where((PlanetTile idx) => idx != current && Math.Abs(Find.World.grid.GetHeadingFromTo(current, idx) - ang) < maximumSiteCurve).ToList();
					if (list4.Count == 0)
					{
						break;
					}
					PlanetTile planetTile = GenCollection.MinBy(list4, (PlanetTile idx) => Find.World.grid.ApproxDistanceInTiles(current, idx));
					ang = Find.World.grid.GetHeadingFromTo(current, planetTile);
					current = planetTile;
					list3.Add(current);
				}
				list2.Add(list3);
			}
		}
		return list2;
	}
}
