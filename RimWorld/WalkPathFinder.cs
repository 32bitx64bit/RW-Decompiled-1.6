using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class WalkPathFinder
{
	private const int NumPathNodes = 8;

	private const float StepDistMin = 2f;

	private const float StepDistMax = 14f;

	private static readonly int StartRadialIndex = GenRadial.NumCellsInRadius(14f);

	private static readonly int EndRadialIndex = GenRadial.NumCellsInRadius(2f);

	private const int RadialIndexStride = 3;

	public static bool TryFindWalkPath(Pawn pawn, IntVec3 root, out List<IntVec3> result)
	{
		List<IntVec3> list = new List<IntVec3> { root };
		IntVec3 intVec4 = root;
		for (int i = 0; i < 8; i++)
		{
			IntVec3 intVec5 = IntVec3.Invalid;
			float num = -1f;
			for (int num2 = StartRadialIndex; num2 > EndRadialIndex; num2 -= 3)
			{
				IntVec3 intVec6 = intVec4 + GenRadial.RadialPattern[num2];
				if (ValidCell(intVec6, intVec4))
				{
					float num3 = 10000f;
					for (int j = 0; j < list.Count; j++)
					{
						num3 += (float)(list[j] - intVec6).LengthManhattan;
					}
					float num4 = (intVec6 - root).LengthManhattan;
					if (num4 > 40f)
					{
						num3 *= Mathf.InverseLerp(70f, 40f, num4);
					}
					if (list.Count >= 2)
					{
						float angleFlat = (list[list.Count - 1] - list[list.Count - 2]).AngleFlat;
						float angleFlat2 = (intVec6 - intVec4).AngleFlat;
						float num5;
						if (angleFlat2 > angleFlat)
						{
							num5 = angleFlat2 - angleFlat;
						}
						else
						{
							angleFlat -= 360f;
							num5 = angleFlat2 - angleFlat;
						}
						if (num5 > 110f)
						{
							num3 *= 0.01f;
						}
					}
					if (list.Count >= 4 && (intVec4 - root).LengthManhattan < (intVec6 - root).LengthManhattan)
					{
						num3 *= 1E-05f;
					}
					if (num3 > num)
					{
						intVec5 = intVec6;
						num = num3;
					}
				}
			}
			if (num < 0f)
			{
				result = null;
				return false;
			}
			list.Add(intVec5);
			intVec4 = intVec5;
		}
		list.Add(root);
		result = list;
		return true;
		bool ValidCell(IntVec3 c, IntVec3 intVec3)
		{
			if (c.InBounds(pawn.Map) && !c.Fogged(pawn.Map) && !c.GetTerrain(pawn.Map).avoidWander && !c.Roofed(pawn.Map) && c.Standable(pawn.Map) && !c.IsForbidden(pawn) && GenSight.LineOfSight(intVec3, c, pawn.Map))
			{
				return !PawnUtility.KnownDangerAt(c, pawn.Map, pawn);
			}
			return false;
		}
	}

	public static void DebugFlashWalkPath(IntVec3 root, int numEntries = 8)
	{
		Map currentMap = Find.CurrentMap;
		if (!TryFindWalkPath(currentMap.mapPawns.FreeColonistsSpawned.First(), root, out var result))
		{
			currentMap.debugDrawer.FlashCell(root, 0.2f, "NOPATH");
			return;
		}
		for (int i = 0; i < result.Count; i++)
		{
			currentMap.debugDrawer.FlashCell(result[i], (float)i / (float)numEntries, i.ToString());
			if (i > 0)
			{
				currentMap.debugDrawer.FlashLine(result[i], result[i - 1]);
			}
		}
	}
}
