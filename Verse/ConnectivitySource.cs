using System.Collections.Generic;
using Unity.Collections;

namespace Verse;

public class ConnectivitySource : SimplePathFinderDataSource<CellConnection>
{
	private static readonly HashSet<IntVec3> checkedCells = new HashSet<IntVec3>(160000);

	public ConnectivitySource(Map map)
		: base(map)
	{
	}

	public override void ComputeAll(IEnumerable<PathRequest> _)
	{
		CellIndices cellIndices = map.cellIndices;
		for (int i = 0; i < cellCount; i++)
		{
			data[i] = CellConnection.AllNeighbours;
		}
		int x = map.Size.x;
		int z = map.Size.z;
		ref NativeArray<CellConnection> reference;
		int num;
		for (int j = 0; j < z; j++)
		{
			IntVec3 c = new IntVec3(0, 0, j);
			IntVec3 c2 = new IntVec3(x - 1, 0, j);
			reference = ref data;
			num = cellIndices.CellToIndex(c);
			reference[num] ^= CellConnection.WestNeighbours;
			reference = ref data;
			num = cellIndices.CellToIndex(c2);
			reference[num] ^= CellConnection.EastNeighbours;
		}
		for (int k = 0; k < x; k++)
		{
			IntVec3 c3 = new IntVec3(k, 0, 0);
			IntVec3 c4 = new IntVec3(k, 0, z - 1);
			reference = ref data;
			num = cellIndices.CellToIndex(c3);
			reference[num] ^= CellConnection.SouthNeighbours;
			reference = ref data;
			num = cellIndices.CellToIndex(c4);
			reference[num] ^= CellConnection.NorthNeighbours;
		}
		IntVec3 c5 = new IntVec3(0, 0, 0);
		IntVec3 c6 = new IntVec3(x - 1, 0, 0);
		IntVec3 c7 = new IntVec3(0, 0, z - 1);
		IntVec3 c8 = new IntVec3(x - 1, 0, z - 1);
		reference = ref data;
		num = cellIndices.CellToIndex(c5);
		reference[num] ^= CellConnection.SouthWest;
		reference = ref data;
		num = cellIndices.CellToIndex(c6);
		reference[num] ^= CellConnection.SouthEast;
		reference = ref data;
		num = cellIndices.CellToIndex(c7);
		reference[num] ^= CellConnection.NorthWest;
		reference = ref data;
		num = cellIndices.CellToIndex(c8);
		reference[num] ^= CellConnection.NorthEast;
	}

	public override bool UpdateIncrementally(IEnumerable<PathRequest> _, List<IntVec3> cellDeltas)
	{
		CellIndices cellIndices = map.cellIndices;
		foreach (IntVec3 cellDelta in cellDeltas)
		{
			checkedCells.Clear();
			foreach (IntVec3 cell in CellRect.FromCell(cellDelta).ExpandedBy(1).ClipInsideMap(map)
				.Cells)
			{
				if (checkedCells.Add(cell))
				{
					data[cellIndices.CellToIndex(cell)] = ComputeCellConnectivity(cell);
				}
			}
		}
		return false;
	}

	private CellConnection ComputeCellConnectivity(IntVec3 cell)
	{
		CellConnection cellConnection = CellConnection.Self;
		foreach (CellConnection item in CellConnection.AllNeighbours)
		{
			IntVec3 c = cell + item.Offset();
			if (!c.InBounds(map))
			{
				continue;
			}
			Building building = map.edificeGrid.InnerArray[map.cellIndices.CellToIndex(c)];
			if (building != null && building.def.passability == Traversability.Impassable)
			{
				if (!building.def.destroyable)
				{
					continue;
				}
			}
			else if (!c.WalkableByAny(map))
			{
				continue;
			}
			cellConnection |= item;
		}
		return cellConnection;
	}
}
