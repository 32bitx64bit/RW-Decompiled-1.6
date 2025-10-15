using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class TileMutatorWorker_RiverDelta : TileMutatorWorker_River
{
	private const float MinWidth = 1.5f;

	private const float DeltaCurveFrequency = 0.02f;

	private const float DeltaCurveAmplitude = 10f;

	private const float DeltaMergeDist = 25f;

	private const float DeltaSplitChance = 0.9f;

	private const float MinSegmentLength = 30f;

	private const float MaxSegmentLength = 70f;

	private const float MinSplitAngle = 15f;

	private const float MaxSplitAngle = 30f;

	private const float MinInitialLength = 0.2f;

	private const float MaxInitialLength = 0.5f;

	private const float ExtraWidthAngle = 5f;

	private static FloatRange SegmentLengthRange => new FloatRange(30f, 70f);

	private static FloatRange SplitAngleRange => new FloatRange(15f, 30f);

	private static FloatRange InitialLengthRange => new FloatRange(0.2f, 0.5f);

	protected override float GetCurveFrequency => 0.02f;

	protected override float GetCurveAmplitude => 10f;

	protected override float GetWidthNoiseFactor(RiverNode riverNode)
	{
		return 1f;
	}

	public TileMutatorWorker_RiverDelta(TileMutatorDef def)
		: base(def)
	{
	}

	public override string GetLabel(PlanetTile tile)
	{
		return def.label;
	}

	protected override void GenerateRiverGraph(Map map)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (ModsConfig.OdysseyActive && !map.TileInfo.Isnt<SurfaceTile>(out var casted))
		{
			SurfaceTile.RiverLink riverLink = GenCollection.MaxBy(casted.Rivers.Where((SurfaceTile.RiverLink rl) => !rl.neighbor.Tile.WaterCovered), (SurfaceTile.RiverLink rl) => rl.river.widthOnMap);
			float num = ((Find.World.CoastAngleAt(map.Tile, BiomeDefOf.Ocean) ?? Find.World.CoastAngleAt(map.Tile, BiomeDefOf.Lake)).Value + 180f) % 360f;
			float angle = (450f - num) % 360f;
			(Vector3, Vector3) mapEdgeNodes = GetMapEdgeNodes(map, angle);
			Vector3 item = mapEdgeNodes.Item1;
			Vector3 item2 = mapEdgeNodes.Item2;
			RiverNode riverNode = ((!IsFlowingAToB(item, item2, angle)) ? new RiverNode
			{
				start = item2,
				end = item2 + (item - item2) * InitialLengthRange.RandomInRange,
				width = riverLink.river.widthOnMap
			} : new RiverNode
			{
				start = item,
				end = item + (item2 - item) * InitialLengthRange.RandomInRange,
				width = riverLink.river.widthOnMap
			});
			map.waterInfo.riverGraph.Add(riverNode);
			SplitRiver(map, riverNode, num);
			MergeNearbyNodes(map, riverNode);
		}
	}

	private void SplitRiver(Map map, RiverNode prevSegment, float baseAngle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		if (prevSegment.end.InBounds(map) && !map.terrainGrid.TerrainAt(prevSegment.end.ToIntVec3()).IsOcean)
		{
			Vector3 val = prevSegment.end - prevSegment.start;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(0f - normalized.z, 0f, normalized.x);
			float num = 5f * (prevSegment.width / 10f);
			if (!Rand.Chance(0.9f))
			{
				float num2 = (baseAngle + SplitAngleRange.RandomInRange - SplitAngleRange.Average) * (MathF.PI / 180f);
				float randomInRange = SegmentLengthRange.RandomInRange;
				float width = prevSegment.width;
				Vector3 start = prevSegment.end - normalized * width / 4f;
				Vector3 end = default(Vector3);
				((Vector3)(ref end))._002Ector(prevSegment.end.x + randomInRange * Mathf.Cos(num2), 0f, prevSegment.end.z + randomInRange * Mathf.Sin(num2));
				RiverNode riverNode = new RiverNode
				{
					start = start,
					end = end,
					width = width
				};
				map.waterInfo.riverGraph.Add(riverNode);
				prevSegment.childNodes.Add(riverNode);
				SplitRiver(map, riverNode, baseAngle);
				return;
			}
			float num3 = (baseAngle - SplitAngleRange.RandomInRange - num) * (MathF.PI / 180f);
			float randomInRange2 = SegmentLengthRange.RandomInRange;
			float num4 = Mathf.Max(prevSegment.width * Rand.Range(0.33f, 0.67f), 1.5f);
			Vector3 start2 = prevSegment.end - normalized * num4 / 4f - val2 * (prevSegment.width / 2f - num4 / 2f);
			Vector3 end2 = default(Vector3);
			((Vector3)(ref end2))._002Ector(prevSegment.end.x + randomInRange2 * Mathf.Cos(num3), 0f, prevSegment.end.z + randomInRange2 * Mathf.Sin(num3));
			RiverNode riverNode2 = new RiverNode
			{
				start = start2,
				end = end2,
				width = num4 * 1.1f
			};
			map.waterInfo.riverGraph.Add(riverNode2);
			prevSegment.childNodes.Add(riverNode2);
			SplitRiver(map, riverNode2, baseAngle);
			num3 = (baseAngle + SplitAngleRange.RandomInRange + num) * (MathF.PI / 180f);
			randomInRange2 = SegmentLengthRange.RandomInRange;
			num4 = Mathf.Max(prevSegment.width - num4, 1.5f) + 1f;
			Vector3 start3 = prevSegment.end - normalized * num4 / 4f + val2 * (prevSegment.width / 2f - num4 / 2f);
			Vector3 end3 = default(Vector3);
			((Vector3)(ref end3))._002Ector(prevSegment.end.x + randomInRange2 * Mathf.Cos(num3), 0f, prevSegment.end.z + randomInRange2 * Mathf.Sin(num3));
			RiverNode riverNode3 = new RiverNode
			{
				start = start3,
				end = end3,
				width = num4 * 1.1f
			};
			map.waterInfo.riverGraph.Add(riverNode3);
			prevSegment.childNodes.Add(riverNode3);
			SplitRiver(map, riverNode3, baseAngle);
		}
	}

	private void MergeNearbyNodes(Map map, RiverNode root)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		HashSet<RiverNode> markedForRemoval = new HashSet<RiverNode>();
		List<Vector3> list = new List<Vector3>();
		Queue<RiverNode> queue = new Queue<RiverNode>();
		queue.Enqueue(root);
		while (!queue.Empty())
		{
			RiverNode riverNode = queue.Dequeue();
			bool flag = false;
			foreach (Vector3 item in list)
			{
				Vector3 val = riverNode.end - item;
				if (!(((Vector3)(ref val)).magnitude < 25f))
				{
					continue;
				}
				riverNode.end = item;
				foreach (RiverNode childNode in riverNode.childNodes)
				{
					RecursivelyMarkForRemoval(childNode);
				}
				flag = true;
			}
			if (flag)
			{
				continue;
			}
			list.Add(riverNode.end);
			foreach (RiverNode item2 in riverNode.childNodes.InRandomOrder())
			{
				queue.Enqueue(item2);
			}
		}
		foreach (RiverNode item3 in markedForRemoval)
		{
			map.waterInfo.riverGraph.Remove(item3);
		}
		void RecursivelyMarkForRemoval(RiverNode node)
		{
			markedForRemoval.Add(node);
			foreach (RiverNode childNode2 in node.childNodes)
			{
				RecursivelyMarkForRemoval(childNode2);
			}
		}
	}
}
