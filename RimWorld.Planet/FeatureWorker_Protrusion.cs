using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet;

public abstract class FeatureWorker_Protrusion : FeatureWorker
{
	private readonly List<PlanetTile> roots = new List<PlanetTile>();

	private readonly HashSet<PlanetTile> rootsSet = new HashSet<PlanetTile>();

	private readonly List<PlanetTile> rootsWithoutSmallPassages = new List<PlanetTile>();

	private readonly HashSet<PlanetTile> rootsWithoutSmallPassagesSet = new HashSet<PlanetTile>();

	private readonly List<PlanetTile> currentGroup = new List<PlanetTile>();

	private readonly List<PlanetTile> currentGroupMembers = new List<PlanetTile>();

	private static readonly List<int> tmpGroup = new List<int>();

	protected virtual int MinSize => def.minSize;

	protected virtual int MaxSize => def.maxSize;

	protected virtual int MaxPassageWidth => def.maxPassageWidth;

	protected virtual float MaxPctOfWholeArea => def.maxPctOfWholeArea;

	protected abstract bool IsRoot(PlanetTile tile);

	protected virtual bool IsMember(PlanetTile tile)
	{
		return Find.WorldGrid[tile].feature == null;
	}

	public override void GenerateWhereAppropriate(PlanetLayer layer)
	{
		CalculateRoots(layer);
		CalculateRootsWithoutSmallPassages(layer);
		CalculateContiguousGroups(layer);
	}

	private void CalculateRoots(PlanetLayer layer)
	{
		roots.Clear();
		int tilesCount = layer.TilesCount;
		for (int i = 0; i < tilesCount; i++)
		{
			PlanetTile planetTile = new PlanetTile(i, layer);
			if (IsRoot(planetTile))
			{
				roots.Add(planetTile);
			}
		}
		rootsSet.Clear();
		rootsSet.AddRange(roots);
	}

	private void CalculateRootsWithoutSmallPassages(PlanetLayer layer)
	{
		rootsWithoutSmallPassages.Clear();
		rootsWithoutSmallPassages.AddRange(roots);
		GenPlanetMorphology.Open(layer, rootsWithoutSmallPassages, MaxPassageWidth);
		rootsWithoutSmallPassagesSet.Clear();
		rootsWithoutSmallPassagesSet.AddRange(rootsWithoutSmallPassages);
	}

	private void CalculateContiguousGroups(PlanetLayer layer)
	{
		int minSize = MinSize;
		int maxSize = MaxSize;
		float maxPctOfWholeArea = MaxPctOfWholeArea;
		int maxPassageWidth = MaxPassageWidth;
		FeatureWorker.ClearVisited(layer);
		FeatureWorker.ClearGroupSizes(layer);
		for (int i = 0; i < roots.Count; i++)
		{
			PlanetTile rootTile = roots[i];
			if (!FeatureWorker.visited[rootTile.tileId])
			{
				tmpGroup.Clear();
				layer.Filler.FloodFill(rootTile, (PlanetTile x) => rootsSet.Contains(x), delegate(PlanetTile x)
				{
					FeatureWorker.visited[x.tileId] = true;
					tmpGroup.Add(x.tileId);
				});
				for (int j = 0; j < tmpGroup.Count; j++)
				{
					FeatureWorker.groupSize[tmpGroup[j]] = tmpGroup.Count;
				}
				tmpGroup.Clear();
			}
		}
		FeatureWorker.ClearVisited(layer);
		for (int k = 0; k < rootsWithoutSmallPassages.Count; k++)
		{
			PlanetTile rootTile2 = rootsWithoutSmallPassages[k];
			if (FeatureWorker.visited[rootTile2.tileId])
			{
				continue;
			}
			currentGroup.Clear();
			layer.Filler.FloodFill(rootTile2, (PlanetTile x) => rootsWithoutSmallPassagesSet.Contains(x), delegate(PlanetTile x)
			{
				FeatureWorker.visited[x.tileId] = true;
				currentGroup.Add(x);
			});
			if (currentGroup.Count < minSize)
			{
				continue;
			}
			GenPlanetMorphology.Dilate(layer, currentGroup, maxPassageWidth * 2, (PlanetTile x) => rootsSet.Contains(x));
			if (currentGroup.Count > maxSize || (float)currentGroup.Count / (float)FeatureWorker.groupSize[rootTile2.tileId] > maxPctOfWholeArea || (!def.canTouchWorldEdge && currentGroup.Any((PlanetTile x) => Find.WorldGrid.IsOnEdge(x))))
			{
				continue;
			}
			currentGroupMembers.Clear();
			for (int l = 0; l < currentGroup.Count; l++)
			{
				if (IsMember(currentGroup[l]))
				{
					currentGroupMembers.Add(currentGroup[l]);
				}
			}
			if (currentGroupMembers.Count < minSize)
			{
				continue;
			}
			if (currentGroup.Any((PlanetTile x) => layer[x].feature == null))
			{
				currentGroup.RemoveAll((PlanetTile x) => layer[x].feature != null);
			}
			AddFeature(layer, currentGroupMembers, currentGroup);
		}
	}
}
