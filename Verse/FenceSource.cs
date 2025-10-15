using System.Collections.Generic;
using Unity.Collections;

namespace Verse;

public class FenceSource : SimpleBoolPathFinderDataSource
{
	public FenceSource(Map map)
		: base(map)
	{
	}

	public override void ComputeAll(IEnumerable<PathRequest> _)
	{
		((NativeBitArray)(ref data)).Clear();
		CellIndices cellIndices = map.cellIndices;
		foreach (Thing item in (IEnumerable<Thing>)map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial))
		{
			if (item.def.IsFence)
			{
				int num = cellIndices.CellToIndex(item.Position);
				((NativeBitArray)(ref data)).Set(num, true);
			}
		}
	}

	public override bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		CellIndices cellIndices = map.cellIndices;
		Building[] innerArray = map.edificeGrid.InnerArray;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			int num = cellIndices.CellToIndex(cellDelta);
			((NativeBitArray)(ref data)).Set(num, innerArray[num]?.def.IsFence ?? false);
		}
		return false;
	}
}
