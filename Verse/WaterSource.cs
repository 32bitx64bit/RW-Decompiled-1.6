using System.Collections.Generic;
using Unity.Collections;

namespace Verse;

public class WaterSource : SimpleBoolPathFinderDataSource
{
	public WaterSource(Map map)
		: base(map)
	{
	}

	public override void ComputeAll(IEnumerable<PathRequest> _)
	{
		((NativeBitArray)(ref data)).Clear();
		TerrainGrid terrainGrid = map.terrainGrid;
		for (int i = 0; i < cellCount; i++)
		{
			TerrainDef terrainDef = terrainGrid.TerrainAt(i);
			if (terrainDef != null && terrainDef.IsWater)
			{
				((NativeBitArray)(ref data)).Set(i, true);
			}
		}
	}

	public override bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		CellIndices cellIndices = map.cellIndices;
		TerrainGrid terrainGrid = map.terrainGrid;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			((NativeBitArray)(ref data)).Set(num, terrainGrid.TerrainAt(num)?.IsWater ?? false);
		}
		return false;
	}
}
