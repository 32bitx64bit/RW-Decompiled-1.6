using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public static class TooltipHandler
{
	private static Dictionary<int, ActiveTip> activeTips = new Dictionary<int, ActiveTip>();

	private static int frame = 0;

	private static List<int> dyingTips = new List<int>(32);

	private const float SpaceBetweenTooltips = 2f;

	private static List<ActiveTip> drawingTips = new List<ActiveTip>();

	private static Func<ActiveTip, ActiveTip, int> compareTooltipsByPriorityCached = CompareTooltipsByPriority;

	public static void ClearTooltipsFrom(Rect rect)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7 || !Mouse.IsOver(rect))
		{
			return;
		}
		dyingTips.Clear();
		foreach (KeyValuePair<int, ActiveTip> activeTip in activeTips)
		{
			if (activeTip.Value.lastTriggerFrame == frame)
			{
				dyingTips.Add(activeTip.Key);
			}
		}
		for (int i = 0; i < dyingTips.Count; i++)
		{
			activeTips.Remove(dyingTips[i]);
		}
	}

	public static void TipRegion(Rect rect, Func<string> textGetter, int uniqueId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		TipRegion(rect, new TipSignal(textGetter, uniqueId));
	}

	public static void TipRegionByKey(Rect rect, string key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect) || DebugViewSettings.drawTooltipEdges)
		{
			TipRegion(rect, key.Translate());
		}
	}

	public static void TipRegionByKey(Rect rect, string key, NamedArgument arg1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect) || DebugViewSettings.drawTooltipEdges)
		{
			TipRegion(rect, key.Translate(arg1));
		}
	}

	public static void TipRegionByKey(Rect rect, string key, NamedArgument arg1, NamedArgument arg2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect) || DebugViewSettings.drawTooltipEdges)
		{
			TipRegion(rect, key.Translate(arg1, arg2));
		}
	}

	public static void TipRegionByKey(Rect rect, string key, NamedArgument arg1, NamedArgument arg2, NamedArgument arg3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect) || DebugViewSettings.drawTooltipEdges)
		{
			TipRegion(rect, key.Translate(arg1, arg2, arg3));
		}
	}

	public static void TipRegion(Rect rect, TipSignal tip)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 7 && (Mouse.IsOver(rect) || DebugViewSettings.drawTooltipEdges) && (tip.textGetter != null || !tip.text.NullOrEmpty()) && !SteamDeck.KeyboardShowing)
		{
			if (DebugViewSettings.drawTooltipEdges)
			{
				Widgets.DrawBox(rect);
			}
			if (!activeTips.ContainsKey(tip.uniqueId))
			{
				ActiveTip value = new ActiveTip(tip);
				activeTips.Add(tip.uniqueId, value);
				activeTips[tip.uniqueId].firstTriggerTime = Time.realtimeSinceStartup;
			}
			activeTips[tip.uniqueId].lastTriggerFrame = frame;
			activeTips[tip.uniqueId].signal.text = tip.text;
			activeTips[tip.uniqueId].signal.textGetter = tip.textGetter;
		}
	}

	public static void DoTooltipGUI()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if (!CellInspectorDrawer.active)
		{
			DrawActiveTips();
			if ((int)Event.current.type == 7)
			{
				CleanActiveTooltips();
				frame++;
			}
		}
	}

	private static void DrawActiveTips()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (activeTips.Count == 0)
		{
			return;
		}
		drawingTips.Clear();
		foreach (ActiveTip value in activeTips.Values)
		{
			if ((double)Time.realtimeSinceStartup > value.firstTriggerTime + (double)value.signal.delay)
			{
				drawingTips.Add(value);
			}
		}
		if (drawingTips.Any())
		{
			drawingTips.SortStable(compareTooltipsByPriorityCached);
			Vector2 pos = CalculateInitialTipPosition(drawingTips);
			for (int i = 0; i < drawingTips.Count; i++)
			{
				pos.y += drawingTips[i].DrawTooltip(pos);
				pos.y += 2f;
			}
			drawingTips.Clear();
		}
	}

	private static void CleanActiveTooltips()
	{
		dyingTips.Clear();
		foreach (KeyValuePair<int, ActiveTip> activeTip in activeTips)
		{
			if (activeTip.Value.lastTriggerFrame != frame)
			{
				dyingTips.Add(activeTip.Key);
			}
		}
		for (int i = 0; i < dyingTips.Count; i++)
		{
			activeTips.Remove(dyingTips[i]);
		}
	}

	private static Vector2 CalculateInitialTipPosition(List<ActiveTip> drawingTips)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < drawingTips.Count; i++)
		{
			Rect tipRect = drawingTips[i].TipRect;
			num += ((Rect)(ref tipRect)).height;
			num2 = Mathf.Max(num2, ((Rect)(ref tipRect)).width);
			if (i != drawingTips.Count - 1)
			{
				num += 2f;
			}
		}
		return GenUI.GetMouseAttachedWindowPos(num2, num);
	}

	private static int CompareTooltipsByPriority(ActiveTip A, ActiveTip B)
	{
		int num = 0 - A.signal.priority;
		int value = 0 - B.signal.priority;
		return num.CompareTo(value);
	}
}
