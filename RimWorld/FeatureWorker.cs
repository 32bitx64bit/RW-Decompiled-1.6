using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class FeatureWorker
{
	public FeatureDef def;

	protected static bool[] visited;

	protected static int[] groupSize;

	protected static int[] groupID;

	private static readonly List<PlanetTile> tmpNeighbors = new List<PlanetTile>();

	private static readonly HashSet<PlanetTile> tmpTilesForTextDrawPosCalculationSet = new HashSet<PlanetTile>();

	private static readonly List<PlanetTile> tmpEdgeTiles = new List<PlanetTile>();

	private static readonly List<(PlanetTile tile, int traversalDistance)> tmpTraversedTiles = new List<(PlanetTile, int)>();

	public abstract void GenerateWhereAppropriate(PlanetLayer layer);

	protected void AddFeature(PlanetLayer layer, List<PlanetTile> members, List<PlanetTile> tilesForTextDrawPosCalculation)
	{
		WorldFeature worldFeature = new WorldFeature(def, layer);
		worldFeature.name = NameGenerator.GenerateName(def.nameMaker, Find.WorldFeatures.features.Select((WorldFeature x) => x.name), appendNumberIfNameUsed: false, "r_name");
		WorldGrid worldGrid = Find.WorldGrid;
		for (int i = 0; i < members.Count; i++)
		{
			worldGrid[members[i]].feature = worldFeature;
		}
		AssignBestDrawPos(layer, worldFeature, tilesForTextDrawPosCalculation);
		Find.WorldFeatures.features.Add(worldFeature);
	}

	private void AssignBestDrawPos(PlanetLayer layer, WorldFeature newFeature, List<PlanetTile> tilesForTextDrawPosCalculation)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		WorldGrid worldGrid = Find.WorldGrid;
		tmpEdgeTiles.Clear();
		tmpTilesForTextDrawPosCalculationSet.Clear();
		tmpTilesForTextDrawPosCalculationSet.AddRange(tilesForTextDrawPosCalculation);
		Vector3 val = Vector3.zero;
		for (int i = 0; i < tilesForTextDrawPosCalculation.Count; i++)
		{
			PlanetTile planetTile = tilesForTextDrawPosCalculation[i];
			val += worldGrid.GetTileCenter(planetTile);
			bool flag = worldGrid.IsOnEdge(planetTile);
			if (!flag)
			{
				worldGrid.GetTileNeighbors(planetTile, tmpNeighbors);
				for (int j = 0; j < tmpNeighbors.Count; j++)
				{
					if (!tmpTilesForTextDrawPosCalculationSet.Contains(tmpNeighbors[j]))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				tmpEdgeTiles.Add(planetTile);
			}
		}
		val /= (float)tilesForTextDrawPosCalculation.Count;
		if (!tmpEdgeTiles.Any())
		{
			tmpEdgeTiles.Add(tilesForTextDrawPosCalculation.RandomElement());
		}
		int bestTileDist = 0;
		tmpTraversedTiles.Clear();
		layer.Filler.FloodFill(PlanetTile.Invalid, (Predicate<PlanetTile>)((PlanetTile tile) => tmpTilesForTextDrawPosCalculationSet.Contains(tile)), (Predicate<PlanetTile, int>)Process, int.MaxValue, (IEnumerable<PlanetTile>)tmpEdgeTiles);
		PlanetTile tile2 = PlanetTile.Invalid;
		float num = -1f;
		for (int k = 0; k < tmpTraversedTiles.Count; k++)
		{
			if (tmpTraversedTiles[k].traversalDistance == bestTileDist)
			{
				Vector3 val2 = worldGrid.GetTileCenter(tmpTraversedTiles[k].tile) - val;
				float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
				if (!tile2.Valid || sqrMagnitude < num)
				{
					tile2 = tmpTraversedTiles[k].tile;
					num = sqrMagnitude;
				}
			}
		}
		float maxDrawSizeInTiles = (float)bestTileDist * 2f * 1.2f;
		newFeature.drawCenter = worldGrid.GetTileCenter(tile2);
		newFeature.maxDrawSizeInTiles = maxDrawSizeInTiles;
		bool Process(PlanetTile tile, int traversalDist)
		{
			tmpTraversedTiles.Add((tile, traversalDist));
			bestTileDist = traversalDist;
			return false;
		}
	}

	protected static void ClearVisited(PlanetLayer layer)
	{
		ClearOrCreate(layer, ref visited);
	}

	protected static void ClearGroupSizes(PlanetLayer layer)
	{
		ClearOrCreate(layer, ref groupSize);
	}

	protected static void ClearGroupIDs(PlanetLayer layer)
	{
		ClearOrCreate(layer, ref groupID);
	}

	private static void ClearOrCreate<T>(PlanetLayer layer, ref T[] array)
	{
		int tilesCount = layer.TilesCount;
		if (array == null || array.Length != tilesCount)
		{
			array = new T[tilesCount];
		}
		else
		{
			Array.Clear(array, 0, array.Length);
		}
	}
}
