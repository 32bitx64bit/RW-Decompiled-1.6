using LudeonTK;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Verse;

[BurstCompile(CompileSynchronously = true)]
public struct PathFinderJob : IJob
{
	public struct CalcNode
	{
		public enum Status : byte
		{
			None,
			Open,
			Closed
		}

		public int gCost;

		public float hCost;

		public float fCost;

		public int parentIndex;

		public Status status;
	}

	public struct ResultData
	{
		public int pathCost;
	}

	private const int Cost_Blocked = 175;

	public CellIndices indices;

	public float heuristicStrength;

	public int moveTicksCardinal;

	public int moveTicksDiagonal;

	public IntVec3 start;

	public IntVec3 destCell;

	public CellRect destRect;

	public PathFinder.UnmanagedGridTraverseParams traverseParams;

	public NativeArray<CalcNode> calcGrid;

	public NativePriorityQueue<int, float, FloatMinComparer> frontier;

	public NativeList<IntVec3> path;

	public NativeReference<ResultData> result;

	[ReadOnly]
	public ReadOnly<int> grid;

	[ReadOnly]
	public ReadOnly<ushort> providerCost;

	[ReadOnly]
	public ReadOnly<bool> blocked;

	[ReadOnly]
	public NativeList<int> excludedRectIndices;

	[ReadOnly]
	public ReadOnly<CellConnection> connectivity;

	[ReadOnly]
	public ReadOnly fences;

	[ReadOnly]
	public ReadOnly buildings;

	private bool passAllDestroyableThings;

	private bool passWater;

	[BurstCompile]
	public void Execute()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		calcGrid.Clear<CalcNode>();
		frontier.Clear();
		path.Clear();
		result.Value = default(ResultData);
		int num = indices.CellToIndex(destCell);
		bool flag = destRect.Area == 1;
		int num2 = 0;
		InitalizeSearch();
		while (frontier.Count > 0)
		{
			frontier.Dequeue(out var element, out var priority);
			if (!Mathf.Approximately(priority, calcGrid[element].fCost) || calcGrid[element].status == CalcNode.Status.Closed)
			{
				continue;
			}
			IntVec3 intVec = indices.IndexToCell(element);
			if (flag)
			{
				if (element == num)
				{
					FinalizePath(element);
					break;
				}
			}
			else if (destRect.Contains(intVec) && !NativeListExtensions.Contains<int, int>(excludedRectIndices, element))
			{
				FinalizePath(element);
				break;
			}
			if (num2 >= indices.NumGridCells)
			{
				break;
			}
			CellConnection connections = connectivity[element];
			int num3 = connections.BitLoopEndIndex();
			for (int i = 0; i < num3; i++)
			{
				CellConnection cellConnection = CellConnectionExtensions.FlagFromBitIndex[i];
				if (!connections.HasBit(cellConnection))
				{
					continue;
				}
				IntVec3 intVec2 = intVec + CellConnectionExtensions.OffsetFromBitIndex[i];
				int num4 = indices.CellToIndex(intVec2);
				if (calcGrid[num4].status == CalcNode.Status.Closed)
				{
					continue;
				}
				int num5 = IndexCost(num4);
				if (num5 >= 10000)
				{
					continue;
				}
				if (cellConnection.Diagonal())
				{
					int num6 = indices.CellToIndex(new IntVec3(intVec2.x, 0, intVec.z));
					int num7 = indices.CellToIndex(new IntVec3(intVec.x, 0, intVec2.z));
					if (IndexCost(num6) >= 10000 || IndexCost(num7) >= 10000 || ((ReadOnly)(ref buildings)).IsSet(num6) || (!traverseParams.canBashFences && ((ReadOnly)(ref fences)).IsSet(num6)) || ((ReadOnly)(ref buildings)).IsSet(num7) || (!traverseParams.canBashFences && ((ReadOnly)(ref fences)).IsSet(num7)))
					{
						continue;
					}
				}
				num5 = ((!cellConnection.Diagonal()) ? (num5 + moveTicksCardinal) : (num5 + moveTicksDiagonal));
				int num8 = num5 + calcGrid[element].gCost;
				CalcNode.Status status = calcGrid[num4].status;
				if (status != 0)
				{
					int num9 = 0;
					if (status == CalcNode.Status.Closed)
					{
						num9 = Mathf.CeilToInt((float)moveTicksCardinal * 0.8f);
					}
					if (calcGrid[num4].gCost <= num8 + num9)
					{
						continue;
					}
				}
				if (status == CalcNode.Status.None)
				{
					IntVec3 intVec3 = destCell - intVec2;
					if (intVec3.x != 0 || intVec3.z != 0)
					{
						int num10 = math.abs(intVec3.x);
						int num11 = math.abs(intVec3.z);
						float num12 = 1f * (float)(num10 + num11) + -0.58579004f * (float)math.min(num10, num11);
						CalcNode calcNode = calcGrid[num4];
						calcNode.hCost = num12 * 13f * heuristicStrength;
						calcGrid[num4] = calcNode;
					}
				}
				float num13 = (float)num8 + calcGrid[num4].hCost;
				if (num13 < 0f)
				{
					num13 = 0f;
				}
				CalcNode calcNode2 = calcGrid[num4];
				calcNode2.gCost = num8;
				calcNode2.fCost = num13;
				calcNode2.parentIndex = element;
				calcNode2.status = CalcNode.Status.Open;
				calcGrid[num4] = calcNode2;
				frontier.Enqueue(num4, num13);
			}
			num2++;
			CalcNode calcNode3 = calcGrid[element];
			calcNode3.status = CalcNode.Status.Closed;
			calcGrid[element] = calcNode3;
		}
	}

	private int IndexCost(int index)
	{
		if (providerCost[index] == ushort.MaxValue)
		{
			return 10000;
		}
		return math.max(grid[index], (int)providerCost[index]) + (blocked[index] ? 175 : 0);
	}

	private void InitalizeSearch()
	{
		int num = indices.CellToIndex(start);
		CalcNode calcNode = calcGrid[num];
		calcNode.parentIndex = num;
		calcNode.status = CalcNode.Status.Open;
		calcGrid[num] = calcNode;
		frontier.Enqueue(num, 0f);
	}

	private void FinalizePath(int finalIndex)
	{
		int num = finalIndex;
		while (true)
		{
			ref NativeList<IntVec3> reference = ref path;
			IntVec3 intVec = indices.IndexToCell(num);
			reference.Add(ref intVec);
			int parentIndex = calcGrid[num].parentIndex;
			if (num == parentIndex)
			{
				break;
			}
			num = parentIndex;
		}
		ResultData value = default(ResultData);
		value.pathCost = calcGrid[finalIndex].gCost;
		result.Value = value;
	}
}
