using System;
using LudeonTK;
using Unity.Collections;

namespace Verse;

public class MapGenFloatGrid : IDisposable
{
	private readonly Map map;

	private NativeArray<float> grid;

	public float this[IntVec3 c]
	{
		get
		{
			return grid[map.cellIndices.CellToIndex(c)];
		}
		set
		{
			grid[map.cellIndices.CellToIndex(c)] = value;
		}
	}

	internal ref NativeArray<float> Grid_Unsafe => ref grid;

	public MapGenFloatGrid(Map map)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		grid = new NativeArray<float>(map.cellIndices.NumGridCells, (Allocator)4, (NativeArrayOptions)1);
	}

	public void Clear()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NativeArrayUtility.MemClear<float>(grid);
	}

	public void Dispose()
	{
		grid.Dispose();
	}
}
