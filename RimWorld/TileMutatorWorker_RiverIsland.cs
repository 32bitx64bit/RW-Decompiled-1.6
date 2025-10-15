using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class TileMutatorWorker_RiverIsland : TileMutatorWorker_River
{
	private const float MinWidth = 1.5f;

	private const float RiverCurveAmplitude = 20f;

	private static readonly FloatRange RiverIslandAmplitudeRange = new FloatRange(1f, 2f);

	private static readonly FloatRange RiverIslandCentralityRange = new FloatRange(0f, 0.3f);

	private static readonly FloatRange RiverWidthFracRange = new FloatRange(0.2f, 0.8f);

	private RiverNode nodeA;

	private RiverNode nodeB;

	private float widthFrac;

	private float riverIslandAmplitude;

	private float riverIslandCentrality;

	protected override float GetCurveAmplitude => 20f;

	public override string GetLabel(PlanetTile tile)
	{
		return def.label;
	}

	public TileMutatorWorker_RiverIsland(TileMutatorDef def)
		: base(def)
	{
	}

	public override void Init(Map map)
	{
		base.Init(map);
		riverIslandAmplitude = RiverIslandAmplitudeRange.RandomInRange;
		riverIslandCentrality = RiverIslandCentralityRange.RandomInRange;
		widthFrac = RiverWidthFracRange.RandomInRange;
	}

	protected override void GenerateRiverGraph(Map map)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (ModsConfig.OdysseyActive && !map.TileInfo.Isnt<SurfaceTile>(out var casted))
		{
			List<SurfaceTile.RiverLink> source = casted.Rivers.OrderBy((SurfaceTile.RiverLink rl) => -((SurfaceTile)rl.neighbor.Tile).riverDist).ToList();
			float headingFromTo = Find.WorldGrid.GetHeadingFromTo(source.First().neighbor.Tile.tile, source.Last().neighbor.Tile.tile);
			var (val, val2) = GetMapEdgeNodes(map, headingFromTo);
			if (IsFlowingAToB(val, val2, headingFromTo))
			{
				nodeA = new RiverNode
				{
					start = val,
					end = val2,
					width = source.First().river.widthOnMap
				};
				nodeB = new RiverNode
				{
					start = val,
					end = val2,
					width = source.First().river.widthOnMap
				};
			}
			else
			{
				nodeA = new RiverNode
				{
					start = val2,
					end = val,
					width = source.First().river.widthOnMap
				};
				nodeB = new RiverNode
				{
					start = val2,
					end = val,
					width = source.First().river.widthOnMap
				};
			}
			map.waterInfo.riverGraph.Add(nodeA);
			map.waterInfo.riverGraph.Add(nodeB);
		}
	}

	protected override Vector2 GetDisplacedPoint(RiverNode riverNode, float t)
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
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = riverNode.end - riverNode.start;
		Vector3 val2 = riverNode.start + t * val;
		float num = -4f * Mathf.Pow(t, 2f) + 4f * t;
		float num2 = t * Vector3.Distance(riverNode.start, riverNode.end);
		Vector3 val3 = new Vector3(0f - val.z, 0f, val.x);
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		float num3 = Mathf.InverseLerp(riverIslandCentrality, 1f - riverIslandCentrality, t);
		float num4 = (float)riverBendNoise.GetValue(num2 * GetWidthNoiseFactor(riverNode), 0.0, riverNode.seed);
		float num5 = -4f * Mathf.Pow(num3, 2f) + 4f * num3;
		num4 = ((riverNode != nodeA) ? (num4 - riverIslandAmplitude * num5) : (num4 + riverIslandAmplitude * num5));
		Vector3 val4 = val2 + num4 * GetCurveAmplitude * normalized * num;
		return new Vector2(val4.x, val4.z);
	}

	protected override float GetRiverWidthAt(RiverNode riverNode, Vector2 cell)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		float tValue = GetTValue(riverNode, cell);
		float num = ((riverNode == nodeA) ? widthFrac : (1f - widthFrac));
		float num2 = Mathf.InverseLerp(riverIslandCentrality, 1f - riverIslandCentrality, tValue);
		float num3 = -4f * Mathf.Pow(num2, 2f) + 4f * num2;
		float num4 = 1f - num * num3;
		return Mathf.Max(riverNode.width / 2f * num4 * (1f + riverWidthNoise.GetValue(Vector2.op_Implicit(cell)) * 0.15f), 1.5f);
	}
}
