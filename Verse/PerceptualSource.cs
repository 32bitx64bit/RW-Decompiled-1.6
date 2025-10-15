using System;
using System.Collections.Generic;
using LudeonTK;
using Unity.Collections;

namespace Verse;

public class PerceptualSource : IPathFinderDataSource, IDisposable
{
	private readonly Map map;

	private NativeArray<ushort> costDrafted;

	private NativeArray<ushort> costUndrafted;

	public ReadOnly<ushort> CostDrafted => costDrafted.AsReadOnly();

	public ReadOnly<ushort> CostUndrafted => costUndrafted.AsReadOnly();

	public PerceptualSource(Map map)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		int numGridCells = map.cellIndices.NumGridCells;
		costDrafted = new NativeArray<ushort>(numGridCells, (Allocator)4, (NativeArrayOptions)1);
		costUndrafted = new NativeArray<ushort>(numGridCells, (Allocator)4, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		costDrafted.Dispose();
		costUndrafted.Dispose();
	}

	public void ComputeAll(IEnumerable<PathRequest> _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		costDrafted.Clear<ushort>();
		costUndrafted.Clear<ushort>();
		TerrainGrid terrainGrid = map.terrainGrid;
		for (int i = 0; i < map.cellIndices.NumGridCells; i++)
		{
			TerrainDef terrainDef = terrainGrid.TerrainAt(i);
			if (terrainDef != null)
			{
				costUndrafted[i] = (ushort)Math.Clamp(terrainDef.extraNonDraftedPerceivedPathCost, 0, 65535);
				costDrafted[i] = (ushort)Math.Clamp(terrainDef.extraDraftedPerceivedPathCost, 0, 65535);
			}
		}
	}

	public bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		CellIndices cellIndices = map.cellIndices;
		TerrainGrid terrainGrid = map.terrainGrid;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			TerrainDef terrainDef = terrainGrid.TerrainAt(num);
			if (terrainDef != null)
			{
				costUndrafted[num] = (ushort)Math.Clamp(terrainDef.extraNonDraftedPerceivedPathCost, 0, 65535);
				costDrafted[num] = (ushort)Math.Clamp(terrainDef.extraDraftedPerceivedPathCost, 0, 65535);
			}
			else
			{
				costUndrafted[num] = 0;
				costDrafted[num] = 0;
			}
		}
		return false;
	}
}
