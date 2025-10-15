using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet;

public class WorldGenStep_Lakes : WorldGenStep
{
	private const int LakeMaxSize = 15;

	public override int SeedPart => 401463656;

	public override void GenerateFresh(string seed, PlanetLayer layer)
	{
		GenerateLakes(layer);
	}

	private void GenerateLakes(PlanetLayer layer)
	{
		bool[] touched = new bool[layer.TilesCount];
		List<int> oceanChunk = new List<int>();
		foreach (Tile tile3 in layer.Tiles)
		{
			PlanetTile tile2 = tile3.tile;
			if (touched[tile2.tileId] || layer[tile2.tileId].PrimaryBiome != BiomeDefOf.Ocean)
			{
				continue;
			}
			layer.Filler.FloodFill(tile2, (PlanetTile tid) => layer[tid].PrimaryBiome == BiomeDefOf.Ocean, delegate(PlanetTile tile)
			{
				oceanChunk.Add(tile.tileId);
				touched[tile.tileId] = true;
			});
			if (oceanChunk.Count <= 15)
			{
				for (int i = 0; i < oceanChunk.Count; i++)
				{
					layer[oceanChunk[i]].PrimaryBiome = BiomeDefOf.Lake;
				}
			}
			oceanChunk.Clear();
		}
	}
}
