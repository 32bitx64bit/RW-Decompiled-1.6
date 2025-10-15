using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ColonistBarDrawLocsFinder
{
	private List<int> entriesInGroup = new List<int>();

	private List<int> horizontalSlotsPerGroup = new List<int>();

	private const float MarginTop = 21f;

	private ColonistBar ColonistBar => Find.ColonistBar;

	private static float MaxColonistBarWidth => (float)UI.screenWidth - 520f;

	public void CalculateDrawLocs(List<Vector2> outDrawLocs, out float scale, int groupsCount)
	{
		if (ColonistBar.Entries.Count == 0)
		{
			outDrawLocs.Clear();
			scale = 1f;
		}
		else
		{
			CalculateColonistsInGroup(groupsCount);
			scale = FindBestScale(out var onlyOneRow, out var maxPerGlobalRow, groupsCount);
			CalculateDrawLocs(outDrawLocs, scale, onlyOneRow, maxPerGlobalRow, groupsCount);
		}
	}

	private void CalculateColonistsInGroup(int groupsCount)
	{
		entriesInGroup.Clear();
		List<ColonistBar.Entry> entries = ColonistBar.Entries;
		for (int i = 0; i < groupsCount; i++)
		{
			entriesInGroup.Add(0);
		}
		for (int j = 0; j < entries.Count; j++)
		{
			entriesInGroup[entries[j].group]++;
		}
	}

	private int CalculateGroupsCount()
	{
		List<ColonistBar.Entry> entries = ColonistBar.Entries;
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			if (num != entries[i].group)
			{
				num2++;
				num = entries[i].group;
			}
		}
		return num2;
	}

	private float FindBestScale(out bool onlyOneRow, out int maxPerGlobalRow, int groupsCount)
	{
		float num = 1f;
		List<ColonistBar.Entry> entries = ColonistBar.Entries;
		while (true)
		{
			float num2 = (ColonistBar.BaseSize.x + 24f) * num;
			float num3 = MaxColonistBarWidth - (float)(groupsCount - 1) * 25f * num;
			maxPerGlobalRow = Mathf.FloorToInt(num3 / num2);
			onlyOneRow = true;
			if (TryDistributeHorizontalSlotsBetweenGroups(maxPerGlobalRow, groupsCount))
			{
				int allowedRowsCountForScale = GetAllowedRowsCountForScale(num);
				bool flag = true;
				int num4 = -1;
				for (int i = 0; i < entries.Count; i++)
				{
					if (num4 != entries[i].group)
					{
						num4 = entries[i].group;
						int num5 = Mathf.CeilToInt((float)entriesInGroup[entries[i].group] / (float)horizontalSlotsPerGroup[entries[i].group]);
						if (num5 > 1)
						{
							onlyOneRow = false;
						}
						if (num5 > allowedRowsCountForScale)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			num *= 0.95f;
		}
		return num;
	}

	private bool TryDistributeHorizontalSlotsBetweenGroups(int maxPerGlobalRow, int groupsCount)
	{
		horizontalSlotsPerGroup.Clear();
		for (int j = 0; j < groupsCount; j++)
		{
			horizontalSlotsPerGroup.Add(0);
		}
		GenMath.DHondtDistribution(horizontalSlotsPerGroup, (int i) => entriesInGroup[i], maxPerGlobalRow);
		for (int k = 0; k < horizontalSlotsPerGroup.Count; k++)
		{
			if (horizontalSlotsPerGroup[k] == 0)
			{
				int num = horizontalSlotsPerGroup.Max();
				if (num <= 1)
				{
					return false;
				}
				int index = horizontalSlotsPerGroup.IndexOf(num);
				horizontalSlotsPerGroup[index]--;
				horizontalSlotsPerGroup[k]++;
			}
		}
		return true;
	}

	private static int GetAllowedRowsCountForScale(float scale)
	{
		if (scale > 0.58f)
		{
			return 1;
		}
		if (scale > 0.42f)
		{
			return 2;
		}
		return 3;
	}

	private void CalculateDrawLocs(List<Vector2> outDrawLocs, float scale, bool onlyOneRow, int maxPerGlobalRow, int groupsCount)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		outDrawLocs.Clear();
		int num = maxPerGlobalRow;
		if (onlyOneRow)
		{
			for (int i = 0; i < horizontalSlotsPerGroup.Count; i++)
			{
				horizontalSlotsPerGroup[i] = Mathf.Min(horizontalSlotsPerGroup[i], entriesInGroup[i]);
			}
			num = ColonistBar.Entries.Count;
		}
		float num2 = (ColonistBar.BaseSize.x + 24f) * scale;
		float num3 = (float)num * num2 + (float)(groupsCount - 1) * 25f * scale;
		List<ColonistBar.Entry> entries = ColonistBar.Entries;
		int num4 = -1;
		int num5 = -1;
		float num6 = ((float)UI.screenWidth - num3) / 2f;
		for (int j = 0; j < entries.Count; j++)
		{
			if (num4 != entries[j].group)
			{
				if (num4 >= 0)
				{
					num6 += 25f * scale;
					num6 += (float)horizontalSlotsPerGroup[num4] * scale * (ColonistBar.BaseSize.x + 24f);
				}
				num5 = 0;
				num4 = entries[j].group;
			}
			else
			{
				num5++;
			}
			Vector2 drawLoc = GetDrawLoc(num6, 21f, entries[j].group, num5, scale);
			outDrawLocs.Add(drawLoc);
		}
	}

	private Vector2 GetDrawLoc(float groupStartX, float groupStartY, int group, int numInGroup, float scale)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		float num = groupStartX + (float)(numInGroup % horizontalSlotsPerGroup[group]) * scale * (ColonistBar.BaseSize.x + 24f);
		float num2 = groupStartY + (float)(numInGroup / horizontalSlotsPerGroup[group]) * scale * (ColonistBar.BaseSize.y + 32f);
		if (numInGroup >= entriesInGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group])
		{
			int num3 = horizontalSlotsPerGroup[group] - entriesInGroup[group] % horizontalSlotsPerGroup[group];
			num += (float)num3 * scale * (ColonistBar.BaseSize.x + 24f) * 0.5f;
		}
		return new Vector2(num, num2);
	}
}
