using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Verse;

[BurstCompile(CompileSynchronously = true)]
public struct PathGridJob : IJobParallelFor
{
	public const int Cost_OutsideAllowedArea = 600;

	public const int Cost_AvoidFog = 600;

	[ReadOnly]
	public PathFinderCostTuning tuning;

	[ReadOnly]
	public PathFinder.UnmanagedGridTraverseParams traverseParams;

	[ReadOnly]
	public CellIndices indicies;

	[ReadOnly]
	public ReadOnly<int> pathGridDirect;

	[ReadOnly]
	public ReadOnly allowedGrid;

	[ReadOnly]
	public ReadOnly building;

	[ReadOnly]
	public ReadOnly buildingDestroyable;

	[ReadOnly]
	public ReadOnly fence;

	[ReadOnly]
	public ReadOnly lordGrid;

	[ReadOnly]
	public ReadOnly water;

	[ReadOnly]
	public ReadOnly darknessDanger;

	[ReadOnly]
	public ReadOnly persistentDanger;

	[ReadOnly]
	public ReadOnly fogged;

	[ReadOnly]
	public ReadOnly player;

	[ReadOnly]
	public ReadOnly<byte> avoidGrid;

	[ReadOnly]
	public ReadOnly<ushort> factionCosts;

	[ReadOnly]
	public ReadOnly<ushort> buildingHitPoints;

	[ReadOnly]
	public ReadOnly<ushort> perceptualCost;

	[ReadOnly]
	public ReadOnly<ushort> custom;

	public NativeArray<int> grid;

	private bool passAllDestroyableThings;

	private bool passWater;

	[BurstCompile]
	public void Execute(int index)
	{
		passAllDestroyableThings = traverseParams.mode == TraverseMode.PassAllDestroyableThings || traverseParams.mode == TraverseMode.PassAllDestroyableThingsNotWater;
		passWater = traverseParams.mode != TraverseMode.NoPassClosedDoorsOrWater && traverseParams.mode != TraverseMode.PassAllDestroyableThingsNotWater;
		if (!CellIsPassable(index))
		{
			grid[index] = 10000;
		}
		else
		{
			grid[index] = CostForCell(index);
		}
	}

	private bool DestroyableBuilding(int index)
	{
		if (pathGridDirect[index] < 10000)
		{
			return false;
		}
		if (traverseParams.fenceBlocked && traverseParams.canBashFences && ((ReadOnly)(ref fence)).IsSet(index))
		{
			return true;
		}
		if (!((ReadOnly)(ref building)).IsSet(index))
		{
			return false;
		}
		if (!((ReadOnly)(ref buildingDestroyable)).IsSet(index))
		{
			return false;
		}
		if (traverseParams.mode == TraverseMode.PassAllDestroyablePlayerOwnedThings && ((ReadOnly)(ref player)).IsSet(index))
		{
			return true;
		}
		return passAllDestroyableThings;
	}

	private bool CellIsPassable(int index)
	{
		if (pathGridDirect[index] >= 10000 && !DestroyableBuilding(index))
		{
			return false;
		}
		if (custom.Length > 0 && custom[index] >= 10000)
		{
			return false;
		}
		if (!passWater && ((ReadOnly)(ref water)).IsSet(index))
		{
			return false;
		}
		if (traverseParams.fenceBlocked && !traverseParams.canBashFences && ((ReadOnly)(ref fence)).IsSet(index))
		{
			return false;
		}
		return true;
	}

	private int CostForCell(int index)
	{
		int num = (DestroyableBuilding(index) ? (tuning.costBlockedWallBase + Mathf.RoundToInt((float)(int)buildingHitPoints[index] * tuning.costBlockedWallExtraPerHitPoint)) : ((tuning.costWater < 0 || !((ReadOnly)(ref water)).IsSet(index)) ? pathGridDirect[index] : tuning.costWater));
		if (tuning.costWater <= 0 || !((ReadOnly)(ref water)).IsSet(index))
		{
			num += perceptualCost[index];
		}
		if (avoidGrid.Length > 0)
		{
			num += avoidGrid[index] * 8;
		}
		if (((ReadOnly)(ref allowedGrid)).Length > 0 && !((ReadOnly)(ref allowedGrid)).IsSet(index))
		{
			num += 600;
		}
		if (traverseParams.fenceBlocked && ((ReadOnly)(ref fence)).IsSet(index) && passAllDestroyableThings)
		{
			num += tuning.costBlockedDoor + Mathf.RoundToInt((float)(int)buildingHitPoints[index] * tuning.costBlockedDoorPerHitPoint);
		}
		IntVec3 c = indicies[index];
		if (traverseParams.targetBuildable.Area == 0 || !traverseParams.targetBuildable.Contains(c))
		{
			num += factionCosts[index];
		}
		if (traverseParams.avoidPersistentDanger && ((ReadOnly)(ref persistentDanger)).IsSet(index))
		{
			num += tuning.costDanger;
		}
		else if (traverseParams.avoidDarknessDanger && ((ReadOnly)(ref darknessDanger)).IsSet(index))
		{
			num += tuning.costDanger;
		}
		if (((ReadOnly)(ref lordGrid)).Length > 0 && !((ReadOnly)(ref lordGrid)).IsSet(index))
		{
			num += tuning.costOffLordWalkGrid;
		}
		if (traverseParams.avoidFog && ((ReadOnly)(ref fogged)).IsSet(index))
		{
			num += 600;
		}
		if (custom.Length > 0)
		{
			num += custom[index];
		}
		return num;
	}
}
