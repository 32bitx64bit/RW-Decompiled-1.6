using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Verse;

public class PathGridDoorsBlockedJob : IJob
{
	private const int MinDistToCheckBlocked = 32;

	public Map map;

	public Pawn pawn;

	public IntVec3 start;

	public IntVec3 dest;

	public TraverseParms traverseParams;

	public PathFinderCostTuning tuning;

	public IReadOnlyList<Thing> doors;

	public IReadOnlyList<IPathFindCostProvider> providers;

	public IReadOnlyList<Pawn> pawns;

	public NativeArray<ushort> providerCost;

	public NativeArray<bool> blocked;

	private bool passAllDestroyableThings;

	private bool passWater;

	public void Execute()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		providerCost.Clear<ushort>();
		blocked.Clear<bool>();
		foreach (Thing door in doors)
		{
			if (!(door is Building_Door building_Door))
			{
				continue;
			}
			foreach (IntVec3 item in building_Door.OccupiedRect())
			{
				int num = map.cellIndices.CellToIndex(item);
				providerCost[num] = PathUtility.GetDoorCost(building_Door, traverseParams, pawn, tuning);
			}
		}
		if (pawn != null)
		{
			foreach (IPathFindCostProvider provider in providers)
			{
				foreach (IntVec3 item2 in provider.GetOccupiedRect())
				{
					int num2 = map.cellIndices.CellToIndex(item2);
					ref NativeArray<ushort> reference = ref providerCost;
					int num3 = num2;
					reference[num3] += provider.PathFindCostFor(pawn);
				}
			}
		}
		if (!PawnUtility.ShouldCollideWithPawns(pawn))
		{
			return;
		}
		foreach (Pawn pawn in pawns)
		{
			if (pawn != this.pawn && CanBlockEver(pawn))
			{
				bool collideWithNonHostile = this.pawn.CurJob != null && (this.pawn.CurJob.collideWithPawns || this.pawn.CurJob.def.collideWithPawns || this.pawn.jobs.curDriver.collideWithPawns);
				if (PawnUtility.PawnBlockedBy(this.pawn, pawn, collideOnlyWithStandingPawns: false, collideWithNonHostile, forPathFinder: true))
				{
					blocked[map.cellIndices.CellToIndex(pawn.Position)] = true;
				}
			}
		}
	}

	private bool CanBlockEver(Pawn other)
	{
		if (other.Destroyed || other.Discarded || !other.Spawned)
		{
			return false;
		}
		if (blocked[map.cellIndices.CellToIndex(other.Position)])
		{
			return false;
		}
		if (!pawn.pather.BestPathHadPawnsInTheWayRecently())
		{
			int lengthManhattan = (other.Position - start).LengthManhattan;
			int lengthManhattan2 = (other.Position - dest).LengthManhattan;
			if (Mathf.Min(lengthManhattan, lengthManhattan2) > 32)
			{
				return false;
			}
		}
		return PawnUtility.ShouldCollideWithPawns(other);
	}
}
