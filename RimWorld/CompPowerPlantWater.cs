using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompPowerPlantWater : CompPowerPlant
{
	private float spinPosition;

	private bool cacheDirty = true;

	private bool waterUsable;

	private bool waterDoubleUsed;

	private float spinRate = 1f;

	private const float PowerFactorIfWaterDoubleUsed = 0.3f;

	private const float SpinRateFactor = 1f / 150f;

	private const float BladeOffset = 2.36f;

	private const int BladeCount = 9;

	private const int RebuildCacheIntervalTicks = 1200;

	public static readonly Material BladesMat = MaterialPool.MatFrom("Things/Building/Power/WatermillGenerator/WatermillGeneratorBlades");

	protected override float DesiredPowerOutput
	{
		get
		{
			if (cacheDirty)
			{
				RebuildCache();
			}
			if (!waterUsable)
			{
				return 0f;
			}
			if (waterDoubleUsed)
			{
				return base.DesiredPowerOutput * 0.3f;
			}
			return base.DesiredPowerOutput;
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		base.PostSpawnSetup(respawningAfterLoad);
		spinPosition = Rand.Range(0f, 15f);
		RebuildCache();
		ForceOthersToRebuildCache(parent.Map);
	}

	public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
	{
		base.PostDeSpawn(map, mode);
		ForceOthersToRebuildCache(map);
	}

	private void ClearCache()
	{
		cacheDirty = true;
	}

	private void RebuildCache()
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		waterUsable = true;
		foreach (IntVec3 item in WaterCells())
		{
			if (item.InBounds(parent.Map) && !item.GetAffordances(parent.Map).Contains(TerrainAffordanceDefOf.MovingFluid))
			{
				waterUsable = false;
				break;
			}
		}
		waterDoubleUsed = false;
		List<Building> list = parent.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.WatermillGenerator);
		foreach (IntVec3 item2 in WaterUseCells())
		{
			if (!item2.InBounds(parent.Map))
			{
				continue;
			}
			foreach (Building item3 in list)
			{
				if (item3 != parent && item3.GetComp<CompPowerPlantWater>().WaterUseRect().Contains(item2))
				{
					waterDoubleUsed = true;
					break;
				}
			}
		}
		if (!waterUsable)
		{
			spinRate = 0f;
			return;
		}
		Vector3 val = Vector3.zero;
		foreach (IntVec3 item4 in WaterCells())
		{
			val += parent.Map.waterInfo.GetWaterMovement(item4.ToVector3Shifted());
		}
		spinRate = Mathf.Sign(Vector3.Dot(val, parent.Rotation.Rotated(RotationDirection.Clockwise).FacingCell.ToVector3()));
		spinRate *= Rand.RangeSeeded(0.9f, 1.1f, parent.thingIDNumber * 60509 + 33151);
		if (waterDoubleUsed)
		{
			spinRate *= 0.5f;
		}
		cacheDirty = false;
	}

	private void ForceOthersToRebuildCache(Map map)
	{
		foreach (Building item in map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.WatermillGenerator))
		{
			item.GetComp<CompPowerPlantWater>().ClearCache();
		}
	}

	public override void CompTick()
	{
		base.CompTick();
		if (base.PowerOutput > 0.01f)
		{
			spinPosition = (spinPosition + 1f / 150f * spinRate + MathF.PI * 2f) % (MathF.PI * 2f);
		}
		if (parent.IsHashIntervalTick(1200))
		{
			RebuildCache();
		}
	}

	public IEnumerable<IntVec3> WaterCells()
	{
		return WaterCells(parent.Position, parent.Rotation);
	}

	public static IEnumerable<IntVec3> WaterCells(IntVec3 loc, Rot4 rot)
	{
		IntVec3 perpOffset = rot.Rotated(RotationDirection.Counterclockwise).FacingCell;
		yield return loc + rot.FacingCell * 3;
		yield return loc + rot.FacingCell * 3 - perpOffset;
		yield return loc + rot.FacingCell * 3 - perpOffset * 2;
		yield return loc + rot.FacingCell * 3 + perpOffset;
		yield return loc + rot.FacingCell * 3 + perpOffset * 2;
	}

	public CellRect WaterUseRect()
	{
		return WaterUseRect(parent.Position, parent.Rotation);
	}

	public static CellRect WaterUseRect(IntVec3 loc, Rot4 rot)
	{
		int width = (rot.IsHorizontal ? 7 : 13);
		int height = (rot.IsHorizontal ? 13 : 7);
		return CellRect.CenteredOn(loc + rot.FacingCell * 4, width, height);
	}

	public IEnumerable<IntVec3> WaterUseCells()
	{
		return WaterUseCells(parent.Position, parent.Rotation);
	}

	public static IEnumerable<IntVec3> WaterUseCells(IntVec3 loc, Rot4 rot)
	{
		foreach (IntVec3 item in WaterUseRect(loc, rot))
		{
			yield return item;
		}
	}

	public IEnumerable<IntVec3> GroundCells()
	{
		return GroundCells(parent.Position, parent.Rotation);
	}

	public static IEnumerable<IntVec3> GroundCells(IntVec3 loc, Rot4 rot)
	{
		IntVec3 perpOffset = rot.Rotated(RotationDirection.Counterclockwise).FacingCell;
		yield return loc - rot.FacingCell;
		yield return loc - rot.FacingCell - perpOffset;
		yield return loc - rot.FacingCell + perpOffset;
		yield return loc;
		yield return loc - perpOffset;
		yield return loc + perpOffset;
	}

	public override void PostDraw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		Vector3 val = parent.TrueCenter();
		val += parent.Rotation.FacingCell.ToVector3() * 2.36f;
		Vector2 val2 = default(Vector2);
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < 9; i++)
		{
			float num = spinPosition + MathF.PI * 2f * (float)i / 9f;
			float num2 = Mathf.Abs(4f * Mathf.Sin(num));
			bool num3 = num % (MathF.PI * 2f) < MathF.PI;
			((Vector2)(ref val2))._002Ector(num2, 1f);
			((Vector3)(ref val3))._002Ector(val2.x, 1f, val2.y);
			Matrix4x4 val4 = default(Matrix4x4);
			((Matrix4x4)(ref val4)).SetTRS(val + Vector3.up * 0.03658537f * Mathf.Cos(num), parent.Rotation.AsQuat, val3);
			Graphics.DrawMesh(num3 ? MeshPool.plane10 : MeshPool.plane10Flip, val4, BladesMat, 0);
		}
	}

	public override string CompInspectStringExtra()
	{
		string text = base.CompInspectStringExtra();
		if (waterUsable && waterDoubleUsed)
		{
			text += "\n" + "Watermill_WaterUsedTwice".Translate();
		}
		return text;
	}
}
