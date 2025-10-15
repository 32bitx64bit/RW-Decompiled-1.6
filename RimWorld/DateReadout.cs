using System;
using System.Collections.Generic;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class DateReadout
{
	private static string dateString;

	private static int dateStringDay;

	private static Season dateStringSeason;

	private static Quadrum dateStringQuadrum;

	private static int dateStringYear;

	private static readonly List<string> fastHourStrings24h;

	private static readonly List<string> fastHourStrings12h;

	private static readonly List<string> seasonsCached;

	private const float DateRightPadding = 7f;

	public static float Height => 48 + (SeasonLabelVisible ? 26 : 0);

	private static bool SeasonLabelVisible
	{
		get
		{
			if (!WorldRendererUtility.WorldSelected)
			{
				return Find.CurrentMap != null;
			}
			return false;
		}
	}

	static DateReadout()
	{
		dateStringDay = -1;
		dateStringSeason = Season.Undefined;
		dateStringQuadrum = Quadrum.Undefined;
		dateStringYear = -1;
		fastHourStrings24h = new List<string>();
		fastHourStrings12h = new List<string>();
		seasonsCached = new List<string>();
		Reset();
	}

	public static void Reset()
	{
		dateString = null;
		dateStringDay = -1;
		dateStringSeason = Season.Undefined;
		dateStringQuadrum = Quadrum.Undefined;
		dateStringYear = -1;
		fastHourStrings24h.Clear();
		for (int i = 0; i < 24; i++)
		{
			fastHourStrings24h.Add(i.ToString() + "LetterHour".Translate());
		}
		fastHourStrings12h.Clear();
		for (int j = 0; j < 24; j++)
		{
			TaggedString taggedString = ((j >= 12) ? "PM".Translate() : "AM".Translate());
			string item = ((j == 0) ? $"12 {taggedString}" : ((j > 12) ? $"{j - 12} {taggedString}" : $"{j} {taggedString}"));
			fastHourStrings12h.Add(item);
		}
		seasonsCached.Clear();
		int length = Enum.GetValues(typeof(Season)).Length;
		for (int k = 0; k < length; k++)
		{
			Season season = (Season)k;
			seasonsCached.Add((season == Season.Undefined) ? "" : season.LabelCap());
		}
	}

	public static void DateOnGUI(Rect dateRect)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		if (WorldRendererUtility.WorldSelected && Find.WorldSelector.SelectedTile.Valid)
		{
			val = Find.WorldGrid.LongLatOf(Find.WorldSelector.SelectedTile);
		}
		else if (WorldRendererUtility.WorldSelected && Find.WorldSelector.NumSelectedObjects > 0)
		{
			val = Find.WorldGrid.LongLatOf(Find.WorldSelector.FirstSelectedObject.Tile);
		}
		else
		{
			if (Find.CurrentMap == null)
			{
				return;
			}
			val = Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile);
		}
		int index = GenDate.HourInteger(Find.TickManager.TicksAbs, val.x);
		int num = GenDate.DayOfTwelfth(Find.TickManager.TicksAbs, val.x);
		Season season = GenDate.Season(Find.TickManager.TicksAbs, val);
		Quadrum quadrum = GenDate.Quadrum(Find.TickManager.TicksAbs, val.x);
		int num2 = GenDate.Year(Find.TickManager.TicksAbs, val.x);
		string text = (SeasonLabelVisible ? seasonsCached[(int)season] : "");
		if (num != dateStringDay || season != dateStringSeason || quadrum != dateStringQuadrum || num2 != dateStringYear)
		{
			dateString = GenDate.DateReadoutStringAt(Find.TickManager.TicksAbs, val);
			dateStringDay = num;
			dateStringSeason = season;
			dateStringQuadrum = quadrum;
			dateStringYear = num2;
		}
		Text.Font = GameFont.Small;
		float num3 = Mathf.Max(Mathf.Max(Text.CalcSize(Prefs.TwelveHourClockMode ? fastHourStrings12h[index] : fastHourStrings24h[index]).x, Text.CalcSize(dateString).x), Text.CalcSize(text).x) + 7f;
		((Rect)(ref dateRect)).xMin = ((Rect)(ref dateRect)).xMax - num3;
		if (Mouse.IsOver(dateRect))
		{
			Widgets.DrawHighlight(dateRect);
		}
		Widgets.BeginGroup(dateRect);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)2;
		Rect rect = dateRect.AtZero();
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 7f;
		if (Prefs.TwelveHourClockMode)
		{
			Widgets.Label(rect, fastHourStrings12h[index]);
		}
		else
		{
			Widgets.Label(rect, fastHourStrings24h[index]);
		}
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 26f;
		Widgets.Label(rect, dateString);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 26f;
		if (!text.NullOrEmpty())
		{
			Widgets.Label(rect, text);
		}
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
		if (Mouse.IsOver(dateRect))
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				Quadrum quadrum2 = (Quadrum)i;
				stringBuilder.AppendLine(quadrum2.Label() + " - " + quadrum2.GetSeason(val.y).LabelCap());
			}
			TaggedString taggedString = "DateReadoutTip".Translate(GenDate.DaysPassed, 15, season.LabelCap(), 15, GenDate.Quadrum(GenTicks.TicksAbs, val.x).Label(), stringBuilder.ToString());
			TooltipHandler.TipRegion(dateRect, new TipSignal(taggedString, 86423));
		}
	}
}
