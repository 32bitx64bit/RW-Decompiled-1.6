using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class TileMutatorWorker_RiverConfluence : TileMutatorWorker_River
{
	public TileMutatorWorker_RiverConfluence(TileMutatorDef def)
		: base(def)
	{
	}

	public override string GetLabel(PlanetTile tile)
	{
		return def.label;
	}

	protected override void GenerateRiverGraph(Map map)
	{
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		if (!ModsConfig.OdysseyActive || map.TileInfo.Isnt<SurfaceTile>(out var casted))
		{
			return;
		}
		List<SurfaceTile.RiverLink> list = casted.Rivers.ToList();
		SurfaceTile.RiverLink item = list.OrderBy((SurfaceTile.RiverLink rl) => ((SurfaceTile)rl.neighbor.Tile).riverDist).First();
		list.Remove(item);
		SurfaceTile.RiverLink item2 = GenCollection.MaxBy(list.Where((SurfaceTile.RiverLink rl) => !rl.neighbor.Tile.WaterCovered), (SurfaceTile.RiverLink rl) => rl.river.widthOnMap);
		list.Remove(item2);
		List<SurfaceTile.RiverLink> list2 = list.Where((SurfaceTile.RiverLink rl) => !rl.neighbor.Tile.WaterCovered).ToList();
		List<SurfaceTile.RiverLink> list3 = list.Where((SurfaceTile.RiverLink rl) => rl.neighbor.Tile.WaterCovered).ToList();
		float angle = Find.WorldGrid.GetHeadingFromTo(item2.neighbor.Tile.tile, item.neighbor.Tile.tile);
		Rot4 rot = Find.World.CoastDirectionAt(map.Tile);
		if (rot != Rot4.Invalid)
		{
			angle = rot.AsAngle;
		}
		(Vector3, Vector3) mapEdgeNodes = GetMapEdgeNodes(map, angle);
		Vector3 item3 = mapEdgeNodes.Item1;
		Vector3 item4 = mapEdgeNodes.Item2;
		RiverNode riverNode = ((!IsFlowingAToB(item3, item4, angle)) ? new RiverNode
		{
			start = item4,
			end = item3,
			width = item2.river.widthOnMap
		} : new RiverNode
		{
			start = item3,
			end = item4,
			width = item2.river.widthOnMap
		});
		map.waterInfo.riverGraph.Add(riverNode);
		float tValue = GetTValue(riverNode, new Vector2((float)riverCenter.x, (float)riverCenter.z));
		Vector3 val = GetDisplacedPoint(riverNode, tValue).ToVector3();
		foreach (SurfaceTile.RiverLink item9 in list2)
		{
			float headingFromTo = Find.WorldGrid.GetHeadingFromTo(item9.neighbor.Tile.tile, casted.tile);
			var (val2, val3) = GetMapEdgeNodes(map, headingFromTo);
			if (IsFlowingAToB(val2, val3, headingFromTo))
			{
				RiverNode item5 = new RiverNode
				{
					start = val2,
					end = val,
					width = item9.river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item5);
			}
			else
			{
				RiverNode item6 = new RiverNode
				{
					start = val3,
					end = val,
					width = item9.river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item6);
			}
		}
		foreach (SurfaceTile.RiverLink item10 in list3)
		{
			float headingFromTo2 = Find.WorldGrid.GetHeadingFromTo(casted.tile, item10.neighbor.Tile.tile);
			var (val4, val5) = GetMapEdgeNodes(map, headingFromTo2);
			if (IsFlowingAToB(val4, val5, headingFromTo2))
			{
				RiverNode item7 = new RiverNode
				{
					start = val,
					end = val5,
					width = item10.river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item7);
			}
			else
			{
				RiverNode item8 = new RiverNode
				{
					start = val,
					end = val4,
					width = item10.river.widthOnMap
				};
				map.waterInfo.riverGraph.Add(item8);
			}
		}
	}
}
