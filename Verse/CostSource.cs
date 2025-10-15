using System.Collections.Generic;
using Unity.Collections;
using Verse.AI;

namespace Verse;

public class CostSource : SimplePathFinderDataSource<int>
{
	protected readonly PathingContext context;

	public CostSource(Map map, PathingContext context)
		: base(map)
	{
		this.context = context;
	}

	public override void ComputeAll(IEnumerable<PathRequest> _)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		data.CopyFrom(context.pathGrid.Grid_Unsafe);
	}

	public override bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		CellIndices cellIndices = map.cellIndices;
		NativeArray<int> grid_Unsafe = context.pathGrid.Grid_Unsafe;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			data[num] = grid_Unsafe[num];
		}
		return false;
	}
}
