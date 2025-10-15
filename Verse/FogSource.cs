using System.Collections.Generic;
using LudeonTK;
using Unity.Collections;

namespace Verse;

public class FogSource : SimpleBoolPathFinderDataSource
{
	public FogSource(Map map)
		: base(map)
	{
	}

	public override void ComputeAll(IEnumerable<PathRequest> _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		data.CopyFrom(map.fogGrid.FogGrid_Unsafe);
	}

	public override bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		CellIndices cellIndices = map.cellIndices;
		NativeBitArray fogGrid_Unsafe = map.fogGrid.FogGrid_Unsafe;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			((NativeBitArray)(ref data)).Set(num, ((NativeBitArray)(ref fogGrid_Unsafe)).IsSet(num));
		}
		return false;
	}
}
