using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class FishShadowComponent : MapComponent
{
	public List<IntVec3> fishWaterCells = new List<IntVec3>();

	private const int SpawnFishFleckIntervalTicks = 450;

	private const int CellsPerFish = 550;

	private const int SpawnFishFleckMaxAttempts = 5;

	private const float FishFleckRotationSpeed = 75f;

	private const int MinCellCount = 12;

	public FishShadowComponent(Map map)
		: base(map)
	{
	}

	public override void MapComponentTick()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!ModsConfig.OdysseyActive)
		{
			return;
		}
		foreach (WaterBody body in map.waterBodyTracker.Bodies)
		{
			if (body.HasFish && body.CellCount >= 12 && GenTicks.IsTickInterval(body.GetHashCode(), Mathf.CeilToInt(450f / Mathf.Max(1f, (float)body.CellCount / 550f))))
			{
				IntVec3? intVec = FindFishFleckLocation(body);
				if (intVec.HasValue)
				{
					SpawnFishFleck(map, intVec.Value.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteLow));
				}
			}
		}
	}

	private static IntVec3? FindFishFleckLocation(WaterBody body)
	{
		for (int i = 0; i < 5; i++)
		{
			IntVec3 intVec = body.cells.RandomElement();
			List<IntVec3> list = GenAdjFast.AdjacentCells8Way(intVec);
			bool flag = true;
			foreach (IntVec3 item in list)
			{
				if (!body.cells.Contains(item) || !item.GetTerrain(body.map).IsWater)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return intVec;
			}
		}
		return null;
	}

	private static void SpawnFishFleck(Map map, Vector3 loc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			bool @bool = Rand.Bool;
			FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc, map, @bool ? FleckDefOf.FishShadowReverse : FleckDefOf.FishShadow, 3f * Rand.Range(0.4f, 1.1f));
			dataStatic.rotation = Rand.Range(0f, 180f);
			dataStatic.rotationRate = (@bool ? (-1f) : 1f) * 75f * Rand.Range(0.75f, 1.25f);
			map.flecks.CreateFleck(dataStatic);
		}
	}
}
