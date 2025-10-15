using System;
using Unity.Collections;
using Verse;

namespace RimWorld;

public class UsedRectPathGridCustomizer : PathRequest.IPathGridCustomizer, IDisposable
{
	private NativeArray<ushort> grid;

	public UsedRectPathGridCustomizer(Map map)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		grid = new NativeArray<ushort>(map.cellIndices.NumGridCells, (Allocator)4, (NativeArrayOptions)1);
		foreach (CellRect usedRect in MapGenerator.UsedRects)
		{
			foreach (IntVec3 cell in usedRect.ExpandedBy(2).Cells)
			{
				if (cell.InBounds(map))
				{
					grid[map.cellIndices.CellToIndex(cell)] = 10000;
				}
			}
		}
	}

	public NativeArray<ushort> GetOffsetGrid()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return grid;
	}

	public void Dispose()
	{
		grid.Dispose();
	}
}
