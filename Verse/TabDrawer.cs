using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public static class TabDrawer
{
	private const float MaxTabWidth = 200f;

	public const float TabHeight = 32f;

	public const float TabHoriztonalOverlap = 10f;

	private static readonly List<TabRecord> tmpTabs = new List<TabRecord>();

	public static TabRecord DrawTabs<T>(Rect baseRect, List<T> tabs, int rows, float? maxTabWidth) where T : TabRecord
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (rows <= 1)
		{
			return DrawTabs(baseRect, tabs);
		}
		int num = Mathf.FloorToInt((float)tabs.Count / (float)rows);
		int num2 = 0;
		TabRecord result = null;
		Rect val = baseRect;
		((Rect)(ref baseRect)).yMin = ((Rect)(ref baseRect)).yMin - (float)(rows - 1) * 31f;
		Rect rect = baseRect;
		((Rect)(ref rect)).yMax = ((Rect)(ref val)).y;
		Widgets.DrawMenuSection(rect);
		for (int i = 0; i < rows; i++)
		{
			int num3 = ((i == 0) ? (tabs.Count - (rows - 1) * num) : num);
			tmpTabs.Clear();
			for (int j = num2; j < num2 + num3; j++)
			{
				tmpTabs.Add(tabs[j]);
			}
			TabRecord tabRecord = DrawTabs(baseRect, tmpTabs, maxTabWidth ?? ((Rect)(ref baseRect)).width);
			if (tabRecord != null)
			{
				result = tabRecord;
			}
			((Rect)(ref baseRect)).yMin = ((Rect)(ref baseRect)).yMin + 31f;
			num2 += num3;
		}
		tmpTabs.Clear();
		return result;
	}

	public static float GetOverflowTabHeight<T>(Rect baseRect, List<T> tabs, float minTabWidth, float maxTabWidth) where T : TabRecord
	{
		int num = Mathf.CeilToInt((float)tabs.Count * minTabWidth / ((Rect)(ref baseRect)).width);
		if (num <= 1)
		{
			return 32f;
		}
		return 32f * (float)num - (float)num;
	}

	public static TabRecord DrawTabsOverflow<T>(Rect baseRect, List<T> tabs, float minTabWidth, float maxTabWidth) where T : TabRecord
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.CeilToInt((float)tabs.Count * minTabWidth / ((Rect)(ref baseRect)).width);
		if (num <= 1)
		{
			((Rect)(ref baseRect)).y = ((Rect)(ref baseRect)).y + 32f;
			T result = DrawTabs(baseRect, tabs, maxTabWidth);
			((Rect)(ref baseRect)).yMax = ((Rect)(ref baseRect)).y;
			return result;
		}
		((Rect)(ref baseRect)).height = 64f;
		int num2 = Mathf.FloorToInt((float)tabs.Count / (float)num);
		int num3 = 0;
		TabRecord result2 = null;
		for (int i = 0; i < num; i++)
		{
			int num4 = Mathf.Min(tabs.Count - num3, num2);
			if (tabs.Count - num3 - num4 == 1)
			{
				((Rect)(ref baseRect)).xMax = ((Rect)(ref baseRect)).xMax + ((Rect)(ref baseRect)).width / (float)num2;
				num4++;
			}
			int num5 = num3;
			((Rect)(ref baseRect)).y = ((Rect)(ref baseRect)).y + 31f;
			tmpTabs.Clear();
			for (int j = num3; j < num5 + num4; j++)
			{
				tmpTabs.Add(tabs[j]);
				num3++;
			}
			TabRecord tabRecord = DrawTabs(baseRect, tmpTabs, ((Rect)(ref baseRect)).width);
			if (tabRecord != null)
			{
				result2 = tabRecord;
			}
		}
		tmpTabs.Clear();
		return result2;
	}

	public static TTabRecord DrawTabs<TTabRecord>(Rect baseRect, List<TTabRecord> tabs, float maxTabWidth = 200f) where TTabRecord : TabRecord
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		TTabRecord val = null;
		TTabRecord val2 = tabs.Find((TTabRecord t) => t.Selected);
		float num = ((Rect)(ref baseRect)).width + (float)(tabs.Count - 1) * 10f;
		float tabWidth = num / (float)tabs.Count;
		if (tabWidth > maxTabWidth)
		{
			tabWidth = maxTabWidth;
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(baseRect);
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 32f;
		((Rect)(ref rect)).height = 9999f;
		Widgets.BeginGroup(rect);
		Text.Anchor = (TextAnchor)4;
		Text.Font = GameFont.Small;
		Func<TTabRecord, Rect> func = (TTabRecord tab) => new Rect((float)tabs.IndexOf(tab) * (tabWidth - 10f), 1f, tabWidth, 32f);
		List<TTabRecord> list = tabs.ListFullCopy();
		if (val2 != null)
		{
			list.Remove(val2);
			list.Add(val2);
		}
		TabRecord tabRecord = null;
		List<TTabRecord> list2 = list.ListFullCopy();
		list2.Reverse();
		for (int i = 0; i < list2.Count; i++)
		{
			TTabRecord val3 = list2[i];
			Rect val4 = func(val3);
			if (tabRecord == null && Mouse.IsOver(val4))
			{
				tabRecord = val3;
			}
			MouseoverSounds.DoRegion(val4, SoundDefOf.Mouseover_Tab);
			if (Mouse.IsOver(val4) && !val3.GetTip().NullOrEmpty())
			{
				TooltipHandler.TipRegion(val4, val3.GetTip());
			}
			if (Widgets.ButtonInvisible(val4))
			{
				val = val3;
			}
		}
		foreach (TTabRecord item in list)
		{
			Rect rect2 = func(item);
			item.Draw(rect2);
		}
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
		if (val != null && val != val2)
		{
			SoundDefOf.RowTabSelect.PlayOneShotOnCamera();
			if (val.clickedAction != null)
			{
				val.clickedAction();
			}
		}
		return val;
	}
}
