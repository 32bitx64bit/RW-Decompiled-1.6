using System;
using System.Collections.Generic;
using LudeonTK;
using Unity.Collections;

namespace Verse;

public class BuildingSource : IPathFinderDataSource, IDisposable
{
	private readonly Map map;

	private readonly int cellCount;

	private NativeBitArray buildings;

	private NativeBitArray destroyable;

	private NativeBitArray player;

	private NativeArray<ushort> hitpoints;

	public ReadOnly Buildings => ((NativeBitArray)(ref buildings)).AsReadOnly();

	public ReadOnly Destroyable => ((NativeBitArray)(ref destroyable)).AsReadOnly();

	public ReadOnly Player => ((NativeBitArray)(ref player)).AsReadOnly();

	public ReadOnly<ushort> Hitpoints => hitpoints.AsReadOnly();

	public BuildingSource(Map map)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		cellCount = map.cellIndices.NumGridCells;
		buildings = new NativeBitArray(cellCount, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		destroyable = new NativeBitArray(cellCount, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		player = new NativeBitArray(cellCount, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		hitpoints = new NativeArray<ushort>(cellCount, (Allocator)4, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		((NativeBitArray)(ref buildings)).Dispose();
		((NativeBitArray)(ref destroyable)).Dispose();
		((NativeBitArray)(ref player)).Dispose();
		hitpoints.Dispose();
	}

	public void ComputeAll(IEnumerable<PathRequest> _)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((NativeBitArray)(ref buildings)).Clear();
		((NativeBitArray)(ref destroyable)).Clear();
		((NativeBitArray)(ref player)).Clear();
		hitpoints.Clear<ushort>();
		Building[] innerArray = map.edificeGrid.InnerArray;
		for (int i = 0; i < cellCount; i++)
		{
			if (innerArray[i] != null)
			{
				SetBuildingData(i);
			}
		}
	}

	public bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		CellIndices cellIndices = map.cellIndices;
		Building[] innerArray = map.edificeGrid.InnerArray;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			if (innerArray[num] != null)
			{
				SetBuildingData(num);
				continue;
			}
			((NativeBitArray)(ref buildings)).Set(num, false);
			((NativeBitArray)(ref destroyable)).Set(num, false);
			((NativeBitArray)(ref player)).Set(num, false);
			hitpoints[num] = 0;
		}
		return false;
	}

	private void SetBuildingData(int index)
	{
		Building building = map.edificeGrid[index];
		if (building != null)
		{
			if (building.def.Fillage == FillCategory.Full)
			{
				((NativeBitArray)(ref buildings)).Set(index, true);
				((NativeBitArray)(ref destroyable)).Set(index, building.def.destroyable);
				hitpoints[index] = (ushort)Math.Clamp(building.HitPoints, 0, 65535);
			}
			else
			{
				((NativeBitArray)(ref buildings)).Set(index, false);
				((NativeBitArray)(ref destroyable)).Set(index, false);
				hitpoints[index] = 0;
			}
			((NativeBitArray)(ref player)).Set(index, building.Faction?.IsPlayer ?? false);
		}
	}
}
