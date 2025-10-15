using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld;

public class TileMutatorWorker_River : TileMutatorWorker
{
	private const float RiverCurveFrequency = 0.007f;

	private const float RiverCurveLacunarity = 2f;

	private const float RiverCurvePersistence = 1f;

	private const int RiverCurveOctaves = 2;

	private const float RiverCurveAmplitude = 40f;

	private const float RiverWidthFrequency = 0.02f;

	private const float RiverWidthLacunarity = 2f;

	private const float RiverWidthPersistence = 1f;

	private const int RiverWidthOctaves = 3;

	protected const float RiverWidthNoiseAmplitude = 0.15f;

	private const float RemoveRoofWidthThreshold = 20f;

	private const float RiverCaveBankFactor = 0.3f;

	private const float ShallowFactor = 0.2f;

	private const int Oversample = 25;

	protected ModuleBase riverBendNoise;

	protected ModuleBase riverWidthNoise;

	protected ModuleBase shallowizer;

	private ModuleBase riverbankNoise;

	protected IntVec3 riverCenter;

	private Dictionary<RiverNode, float[]> nodeDepthMaps = new Dictionary<RiverNode, float[]>();

	protected virtual float GetCurveFrequency => 0.007f;

	protected virtual float GetCurveAmplitude => 40f;

	public TileMutatorWorker_River(TileMutatorDef def)
		: base(def)
	{
	}

	public override string GetLabel(PlanetTile tile)
	{
		if (tile.Tile is SurfaceTile surfaceTile)
		{
			return surfaceTile.Rivers[0].river.label;
		}
		throw new Exception("Attempted to get river label on a tile which is not a SurfaceTile");
	}

	public override void Init(Map map)
	{
		if (map.waterInfo.lakeCenter.IsValid)
		{
			riverCenter = map.waterInfo.lakeCenter;
		}
		else
		{
			riverCenter = GetRiverCenter(map);
		}
		riverBendNoise = new Perlin(GetCurveFrequency, 2.0, 1.0, 2, Rand.Int, QualityMode.Medium);
		shallowizer = new Perlin(0.029999999329447746, 2.0, 0.5, 3, Rand.Int, QualityMode.Medium);
		shallowizer = new Abs(shallowizer);
		riverbankNoise = new Perlin(0.029999999329447746, 2.0, 2.0, 2, Rand.Int, QualityMode.Medium);
		riverWidthNoise = new Perlin(0.019999999552965164, 2.0, 1.0, 3, Rand.Int, QualityMode.Medium);
	}

	public override void GeneratePostTerrain(Map map)
	{
		GenerateRiverGraph(map);
		if (map.waterInfo.riverGraph.NullOrEmpty())
		{
			return;
		}
		GenerateDepthMaps(map);
		List<IntVec3> list = new List<IntVec3>();
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		foreach (IntVec3 allCell in map.AllCells)
		{
			Building edifice = allCell.GetEdifice(map);
			TerrainDef terrainDef = RiverTerrainAt(allCell, map);
			TerrainDef terrainDef2 = RiverBankTerrainAt(allCell, map);
			float riverWidth;
			float depth = GetDepth(allCell, map, out riverWidth);
			float value = riverbankNoise.GetValue(allCell);
			float num = (float)map.Biome.riverbankSizeRange.Lerped(value) * 0.3f;
			bool flag = riverWidth > 20f;
			if (depth > 0f - num)
			{
				edifice?.Destroy();
				if (flag)
				{
					map.roofGrid.SetRoof(allCell, null);
				}
				else if (edifice != null)
				{
					list.Add(edifice.Position);
				}
			}
			if (terrainDef != null)
			{
				map.terrainGrid.SetTerrain(allCell, terrainDef);
				elevation[allCell] = -1f;
			}
			else if (terrainDef2 != null && edifice == null)
			{
				map.terrainGrid.SetTerrain(allCell, terrainDef2);
			}
		}
		RoofCollapseCellsFinder.RemoveBulkCollapsingRoofs(list, map);
	}

	public override void GeneratePostFog(Map map)
	{
		if (!map.waterInfo.riverGraph.NullOrEmpty())
		{
			GenerateRiverLookupTexture(map);
		}
	}

	protected virtual void GenerateRiverGraph(Map map)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (!map.TileInfo.Isnt<SurfaceTile>(out var casted))
		{
			List<SurfaceTile.RiverLink> source = casted.Rivers.OrderBy((SurfaceTile.RiverLink rl) => -((SurfaceTile)rl.neighbor.Tile).riverDist).ToList();
			float headingFromTo = Find.WorldGrid.GetHeadingFromTo(source.First().neighbor.Tile.tile, source.Last().neighbor.Tile.tile);
			var (val, val2) = GetMapEdgeNodes(map, headingFromTo);
			if (IsFlowingAToB(val, val2, headingFromTo))
			{
				RiverNode item = new RiverNode
				{
					start = val,
					end = val2,
					width = source.First().river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item);
			}
			else
			{
				RiverNode item2 = new RiverNode
				{
					start = val2,
					end = val,
					width = source.First().river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item2);
			}
		}
	}

	protected virtual IntVec3 GetRiverCenter(Map map)
	{
		return new IntVec3((int)(Rand.Range(0.3f, 0.7f) * (float)map.Size.x), 0, (int)(Rand.Range(0.3f, 0.7f) * (float)map.Size.z));
	}

	protected (Vector3, Vector3) GetMapEdgeNodes(Map map, float angle)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		float slope = Mathf.Tan((450f - angle) % 360f * (MathF.PI / 180f));
		List<Vector2> intersections = new List<Vector2>();
		GenGeo.LineRectIntersection(new Vector2((float)riverCenter.x, (float)riverCenter.z), slope, new Vector2(-25f, -25f), new Vector2((float)(map.Size.x + 25), (float)(map.Size.z + 25)), ref intersections);
		return (new Vector3(intersections[0].x, 0f, intersections[0].y), new Vector3(intersections[1].x, 0f, intersections[1].y));
	}

	protected bool IsFlowingAToB(Vector3 a, Vector3 b, float angle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.RoundToInt((b - a).AngleFlat() - angle) % 360 == 0;
	}

	private void GenerateDepthMaps(Map map)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		Vector2 val3 = default(Vector2);
		foreach (RiverNode item in map.waterInfo.riverGraph)
		{
			int num = map.Size.x + 50;
			int num2 = map.Size.z + 50;
			float[] array = new float[num * num2];
			nodeDepthMaps.Add(item, array);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					float num3 = float.MinValue;
					((Vector2)(ref val))._002Ector((float)(i - 25), (float)(j - 25));
					float tValue = GetTValue(item, val);
					if (!(tValue < 0f) && !(tValue > 1f))
					{
						Vector2 displacedPoint = GetDisplacedPoint(item, tValue);
						float riverWidthAt = GetRiverWidthAt(item, val);
						Vector2 val2 = GetDisplacedPoint(item, tValue - 0.001f) - displacedPoint;
						Vector2 normalized = ((Vector2)(ref val2)).normalized;
						((Vector2)(ref val3))._002Ector(0f - normalized.y, normalized.x);
						float num4 = Mathf.Abs(Vector2.Dot(val - displacedPoint, val3));
						num3 = riverWidthAt - num4;
					}
					array[i + j * num] = num3;
				}
			}
		}
	}

	protected virtual float GetRiverWidthAt(RiverNode riverNode, Vector2 cell)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return riverNode.width / 2f * (1f + riverWidthNoise.GetValue(Vector2.op_Implicit(cell)) * 0.15f * GetWidthNoiseFactor(riverNode));
	}

	private float GetDepth(IntVec3 cell, Map map, out float riverWidth)
	{
		float num = float.MinValue;
		riverWidth = 0f;
		foreach (RiverNode item in map.waterInfo.riverGraph)
		{
			if (GetSegmentDepth(cell, map, item) > num)
			{
				num = Mathf.Max(num, GetSegmentDepth(cell, map, item));
				riverWidth = item.width;
			}
		}
		return num;
	}

	private float GetSegmentDepth(IntVec3 cell, Map map, RiverNode riverNode)
	{
		int num = cell.x + 25 + (cell.z + 25) * (map.Size.x + 50);
		return nodeDepthMaps[riverNode][num];
	}

	protected float GetTValue(RiverNode riverNode, Vector2 point)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(riverNode.start.x, riverNode.start.z);
		Vector2 val2 = new Vector2(riverNode.end.x, riverNode.end.z) - val;
		Vector2 val3 = point - val;
		float num = Vector2.Dot(val2, val2);
		return Vector2.Dot(val3, val2) / num;
	}

	protected virtual Vector2 GetDisplacedPoint(RiverNode riverNode, float t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = riverNode.end - riverNode.start;
		Vector3 val2 = riverNode.start + t * val;
		float num = -4f * Mathf.Pow(t, 2f) + 4f * t;
		float num2 = t * Vector3.Distance(riverNode.start, riverNode.end);
		Vector3 val3 = new Vector3(0f - val.z, 0f, val.x);
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		float num3 = (float)riverBendNoise.GetValue(num2 * GetWidthNoiseFactor(riverNode), 0.0, riverNode.seed);
		Vector3 val4 = val2 + num3 * GetCurveAmplitude * normalized * num;
		return new Vector2(val4.x, val4.z);
	}

	protected virtual float GetWidthNoiseFactor(RiverNode riverNode)
	{
		return 6f / riverNode.width;
	}

	private TerrainDef RiverTerrainAt(IntVec3 cell, Map map)
	{
		TerrainDef terrainDef = map.terrainGrid.TerrainAt(cell);
		if (terrainDef.IsWater && !terrainDef.IsRiver && terrainDef != TerrainDefOf.Marsh)
		{
			return null;
		}
		float riverWidth;
		float depth = GetDepth(cell, map, out riverWidth);
		if (depth > 4f && shallowizer.GetValue(cell) > 0.2f)
		{
			return MapGenUtility.DeepMovingWaterTerrainAt(cell, map);
		}
		if (depth > 0f)
		{
			return MapGenUtility.ShallowMovingWaterTerrainAt(cell, map);
		}
		return null;
	}

	private TerrainDef RiverBankTerrainAt(IntVec3 cell, Map map)
	{
		TerrainDef existing = map.terrainGrid.TerrainAt(cell);
		if (existing.IsWater && existing != TerrainDefOf.Marsh)
		{
			return null;
		}
		if (!map.Biome.terrainsByFertility.Any((TerrainThreshold tt) => tt.terrain == existing))
		{
			return null;
		}
		float riverWidth;
		float depth = GetDepth(cell, map, out riverWidth);
		float value = riverbankNoise.GetValue(cell);
		int num = map.Biome.riverbankSizeRange.Lerped(value);
		if (depth > (float)(-num))
		{
			return MapGenUtility.RiverbankTerrainAt(cell, map);
		}
		return null;
	}

	private void GenerateRiverLookupTexture(Map map)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		List<IntVec3> list = new List<IntVec3>();
		List<float> list2 = new List<float>();
		List<Vector2> list3 = new List<Vector2>();
		int num = 2;
		foreach (RiverNode item2 in map.waterInfo.riverGraph)
		{
			Vector3 val = item2.end - item2.start;
			float magnitude = ((Vector3)(ref val)).magnitude;
			val = item2.end - item2.start;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Vector2 val2 = (item2.start - normalized).ToVector2();
			for (int i = 0; (float)i < magnitude; i += num)
			{
				Vector2 displacedPoint = GetDisplacedPoint(item2, (float)i / magnitude);
				IntVec3 item = displacedPoint.ToVector3().ToIntVec3();
				list.Add(item);
				list2.Add(item2.width);
				list3.Add(displacedPoint - val2);
				val2 = displacedPoint;
			}
		}
		map.waterInfo.riverFlowMap = new List<float>();
		for (int j = 0; j < map.Size.x * map.Size.z * 2; j++)
		{
			map.waterInfo.riverFlowMap.Add(0f);
		}
		for (int k = 0; k < map.Size.x; k++)
		{
			for (int l = 0; l < map.Size.z; l++)
			{
				IntVec3 intVec = new IntVec3(k, 0, l);
				if (!intVec.GetTerrain(map).IsRiver)
				{
					continue;
				}
				int num2 = intVec.x * map.Size.z + intVec.z;
				int num3 = 0;
				Vector2 val3 = Vector2.zero;
				for (int m = 0; m < list.Count; m++)
				{
					if (!((list[m] - intVec).Magnitude > list2[m] * 1.5f / 2f + 1f))
					{
						num3++;
						val3 += list3[m];
					}
				}
				val3 /= (float)num3;
				map.waterInfo.riverFlowMap[num2 * 2] = val3.x;
				map.waterInfo.riverFlowMap[num2 * 2 + 1] = val3.y;
			}
		}
	}
}
