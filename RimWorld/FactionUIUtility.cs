using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class FactionUIUtility
{
	private static bool showAll;

	private static List<Faction> visibleFactions = new List<Faction>();

	private const float FactionIconRectSize = 42f;

	private const float FactionIconRectGapX = 24f;

	private const float RowHeight = 80f;

	private const float LabelRowHeight = 50f;

	private const float FactionIconSpacing = 5f;

	private const float IdeoIconSpacing = 5f;

	private const float BasicsColumnWidth = 300f;

	private const float InfoColumnWidth = 40f;

	private const float IdeosColumnWidth = 60f;

	private const float RelationsColumnWidth = 70f;

	private const float NaturalGoodwillColumnWidth = 54f;

	private static List<int> tmpTicks = new List<int>();

	private static List<int> tmpCustomGoodwill = new List<int>();

	public static void DoWindowContents(Rect fillRect, ref Vector2 scrollPosition, ref float scrollViewHeight, Faction scrollToFaction = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Invalid comparison between Unknown and I4
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, ((Rect)(ref fillRect)).width, ((Rect)(ref fillRect)).height);
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		if (Prefs.DevMode)
		{
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref rect)).width - 120f, 0f, 120f, 24f), "DEV: Show all", ref showAll);
		}
		else
		{
			showAll = false;
		}
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 50f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - 50f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollViewHeight);
		visibleFactions.Clear();
		foreach (Faction item in Find.FactionManager.AllFactionsInViewOrder)
		{
			if ((!item.IsPlayer && !item.Hidden) || showAll)
			{
				visibleFactions.Add(item);
			}
		}
		if (visibleFactions.Count > 0)
		{
			Widgets.Label(new Rect(614f, 50f, 200f, 100f), "EnemyOf".Translate());
			((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + Text.LineHeight;
			Widgets.BeginScrollView(outRect, ref scrollPosition, val);
			float num = 0f;
			int num2 = 0;
			foreach (Faction visibleFaction in visibleFactions)
			{
				if ((!visibleFaction.IsPlayer && !visibleFaction.Hidden) || showAll)
				{
					if (visibleFaction == scrollToFaction)
					{
						scrollPosition.y = num;
					}
					if (num2 % 2 == 1)
					{
						Widgets.DrawLightHighlight(new Rect(((Rect)(ref val)).x, num, ((Rect)(ref val)).width, 80f));
					}
					num += DrawFactionRow(visibleFaction, num, val);
					num2++;
				}
			}
			if ((int)Event.current.type == 8)
			{
				scrollViewHeight = num;
			}
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(rect, "NoFactions".Translate());
			Text.Anchor = (TextAnchor)0;
		}
		Widgets.EndGroup();
	}

	private static float DrawFactionRow(Faction faction, float rowY, Rect fillRect)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_087b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref fillRect)).width - 300f - 40f - 70f - 54f - 16f - 120f;
		Faction[] array = Find.FactionManager.AllFactionsInViewOrder.Where((Faction f) => f != faction && f.HostileTo(faction) && ((!f.IsPlayer && !f.Hidden) || showAll)).ToArray();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(90f, rowY, 300f, 80f);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Rect val = new Rect(24f, rowY + (((Rect)(ref rect)).height - 42f) / 2f, 42f, 42f);
		GUI.color = faction.Color;
		GUI.DrawTexture(val, (Texture)(object)faction.def.FactionIcon);
		GUI.color = Color.white;
		string label = faction.Name.CapitalizeFirst() + "\n" + faction.def.LabelCap + "\n" + ((faction.leader != null) ? (faction.LeaderTitle.CapitalizeFirst() + ": " + faction.leader.Name.ToStringFull) : "");
		Widgets.Label(rect, label);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, rowY, ((Rect)(ref rect)).xMax, 80f);
		if (Mouse.IsOver(val2))
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => faction.Name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + faction.def.LabelCap.Resolve() + "\n\n" + faction.def.Description, faction.loadID ^ 0x738AC053), rect: val2);
			Widgets.DrawHighlight(val2);
		}
		if (Widgets.ButtonInvisible(val2, doMouseoverSound: false))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(faction));
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).xMax, rowY, 40f, 80f);
		Widgets.InfoCardButtonCentered(rect2, faction);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref rect2)).xMax, rowY, 60f, 80f);
		if (ModsConfig.IdeologyActive && !Find.IdeoManager.classicMode)
		{
			if (faction.ideos != null)
			{
				float num2 = ((Rect)(ref val3)).x;
				float num3 = ((Rect)(ref val3)).y;
				if (faction.ideos.PrimaryIdeo != null)
				{
					if (num2 + 40f > ((Rect)(ref val3)).xMax)
					{
						num2 = ((Rect)(ref val3)).x;
						num3 += 45f;
					}
					Rect rect3 = default(Rect);
					((Rect)(ref rect3))._002Ector(num2, num3 + (((Rect)(ref val3)).height - 40f) / 2f, 40f, 40f);
					IdeoUIUtility.DoIdeoIcon(rect3, faction.ideos.PrimaryIdeo, doTooltip: true, delegate
					{
						IdeoUIUtility.OpenIdeoInfo(faction.ideos.PrimaryIdeo);
					});
					num2 += ((Rect)(ref rect3)).width + 5f;
					num2 = ((Rect)(ref val3)).x;
					num3 += 45f;
				}
				List<Ideo> minor = faction.ideos.IdeosMinorListForReading;
				int i;
				Rect rect4 = default(Rect);
				for (i = 0; i < minor.Count; i++)
				{
					if (num2 + 22f > ((Rect)(ref val3)).xMax)
					{
						num2 = ((Rect)(ref val3)).x;
						num3 += 27f;
					}
					if (num3 + 22f > ((Rect)(ref val3)).yMax)
					{
						break;
					}
					((Rect)(ref rect4))._002Ector(num2, num3 + (((Rect)(ref val3)).height - 22f) / 2f, 22f, 22f);
					IdeoUIUtility.DoIdeoIcon(rect4, minor[i], doTooltip: true, delegate
					{
						IdeoUIUtility.OpenIdeoInfo(minor[i]);
					});
					num2 += ((Rect)(ref rect4)).width + 5f;
				}
			}
		}
		else
		{
			((Rect)(ref val3)).width = 0f;
		}
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(((Rect)(ref val3)).xMax, rowY, 70f, 80f);
		if (!faction.IsPlayer)
		{
			string text = faction.PlayerRelationKind.GetLabelCap();
			if (faction.defeated)
			{
				text = text.Colorize(ColorLibrary.Grey);
			}
			GUI.color = faction.PlayerRelationKind.GetColor();
			Text.Anchor = (TextAnchor)4;
			if (faction.HasGoodwill && !faction.def.permanentEnemy)
			{
				Widgets.Label(new Rect(((Rect)(ref rect5)).x, ((Rect)(ref rect5)).y - 10f, ((Rect)(ref rect5)).width, ((Rect)(ref rect5)).height), text);
				Text.Font = GameFont.Medium;
				Widgets.Label(new Rect(((Rect)(ref rect5)).x, ((Rect)(ref rect5)).y + 10f, ((Rect)(ref rect5)).width, ((Rect)(ref rect5)).height), faction.PlayerGoodwill.ToStringWithSign());
				Text.Font = GameFont.Small;
			}
			else
			{
				Widgets.Label(rect5, text);
			}
			GenUI.ResetLabelAlign();
			GUI.color = Color.white;
			if (Mouse.IsOver(rect5))
			{
				TaggedString taggedString = "";
				if (faction.def.permanentEnemy)
				{
					taggedString = "CurrentGoodwillTip_PermanentEnemy".Translate();
				}
				else if (faction.HasGoodwill)
				{
					taggedString = "Goodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + (faction.PlayerGoodwill.ToStringWithSign() + ", " + faction.PlayerRelationKind.GetLabel()).Colorize(faction.PlayerRelationKind.GetColor());
					TaggedString ongoingEvents = GetOngoingEvents(faction);
					if (!ongoingEvents.NullOrEmpty())
					{
						taggedString += "\n\n" + "OngoingEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + ongoingEvents;
					}
					TaggedString recentEvents = GetRecentEvents(faction);
					if (!recentEvents.NullOrEmpty())
					{
						taggedString += "\n\n" + "RecentEvents".Translate().Colorize(ColoredText.TipSectionTitleColor) + ":\n" + recentEvents;
					}
					string s = "";
					switch (faction.PlayerRelationKind)
					{
					case FactionRelationKind.Ally:
						s = "CurrentGoodwillTip_Ally".Translate(0.ToString("F0"));
						break;
					case FactionRelationKind.Neutral:
						s = "CurrentGoodwillTip_Neutral".Translate((-75).ToString("F0"), 75.ToString("F0"));
						break;
					case FactionRelationKind.Hostile:
						s = "CurrentGoodwillTip_Hostile".Translate(0.ToString("F0"));
						break;
					}
					taggedString += "\n\n" + s.Colorize(ColoredText.SubtleGrayColor);
				}
				if (taggedString != "")
				{
					TooltipHandler.TipRegion(rect5, taggedString);
				}
				Widgets.DrawHighlight(rect5);
			}
		}
		Rect rect6 = default(Rect);
		((Rect)(ref rect6))._002Ector(((Rect)(ref rect5)).xMax, rowY, 54f, 80f);
		if (!faction.IsPlayer && faction.HasGoodwill && !faction.def.permanentEnemy)
		{
			FactionRelationKind relationKindForGoodwill = GetRelationKindForGoodwill(faction.NaturalGoodwill);
			GUI.color = relationKindForGoodwill.GetColor();
			Rect val4 = rect6.ContractedBy(7f);
			((Rect)(ref val4)).y = rowY + 30f;
			((Rect)(ref val4)).height = 20f;
			Text.Anchor = (TextAnchor)1;
			Widgets.DrawRectFast(val4, Color.black);
			Widgets.Label(val4, faction.NaturalGoodwill.ToStringWithSign());
			GenUI.ResetLabelAlign();
			GUI.color = Color.white;
			if (Mouse.IsOver(rect6))
			{
				TaggedString taggedString2 = "NaturalGoodwill".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + faction.NaturalGoodwill.ToStringWithSign().Colorize(relationKindForGoodwill.GetColor());
				int goodwill = Mathf.Clamp(faction.NaturalGoodwill - 50, -100, 100);
				int goodwill2 = Mathf.Clamp(faction.NaturalGoodwill + 50, -100, 100);
				taggedString2 += "\n" + "NaturalGoodwillRange".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + goodwill.ToString().Colorize(GetRelationKindForGoodwill(goodwill).GetColor()) + " " + "RangeTo".Translate() + " " + goodwill2.ToString().Colorize(GetRelationKindForGoodwill(goodwill2).GetColor());
				TaggedString naturalGoodwillExplanation = GetNaturalGoodwillExplanation(faction);
				if (!naturalGoodwillExplanation.NullOrEmpty())
				{
					taggedString2 += "\n\n" + "AffectedBy".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n" + naturalGoodwillExplanation;
				}
				taggedString2 += "\n\n" + "NaturalGoodwillDescription".Translate(1.25f.ToStringPercent()).Colorize(ColoredText.SubtleGrayColor);
				TooltipHandler.TipRegion(rect6, taggedString2);
				Widgets.DrawHighlight(rect6);
			}
		}
		float num4 = ((Rect)(ref rect6)).xMax + 17f;
		for (int j = 0; j < array.Length; j++)
		{
			if (num4 >= ((Rect)(ref rect6)).xMax + num)
			{
				num4 = ((Rect)(ref rect6)).xMax;
				rowY += 27f;
			}
			DrawFactionIconWithTooltip(new Rect(num4, rowY + 29f, 22f, 22f), array[j]);
			num4 += 27f;
		}
		Text.Anchor = (TextAnchor)0;
		return 80f;
	}

	public static void DrawFactionIconWithTooltip(Rect r, Faction faction)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = faction.Color;
		GUI.DrawTexture(r, (Texture)(object)faction.def.FactionIcon);
		GUI.color = Color.white;
		if (Mouse.IsOver(r))
		{
			TipSignal tip = new TipSignal(() => faction.Name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + faction.def.LabelCap.Resolve() + "\n\n" + faction.def.Description, faction.loadID ^ 0x738AC053);
			TooltipHandler.TipRegion(r, tip);
			Widgets.DrawHighlight(r);
		}
		if (Widgets.ButtonInvisible(r, doMouseoverSound: false))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(faction));
		}
	}

	public static void DrawRelatedFactionInfo(Rect rect, Faction faction, ref float curY)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = (TextAnchor)8;
		curY += 10f;
		FactionRelationKind playerRelationKind = faction.PlayerRelationKind;
		string text = faction.Name.CapitalizeFirst() + "\n" + "goodwill".Translate().CapitalizeFirst() + ": " + faction.PlayerGoodwill.ToStringWithSign();
		GUI.color = Color.gray;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, ((Rect)(ref rect)).width, Text.CalcHeight(text, ((Rect)(ref rect)).width));
		Widgets.Label(rect2, text);
		curY += ((Rect)(ref rect2)).height;
		GUI.color = playerRelationKind.GetColor();
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).x, curY - 7f, ((Rect)(ref rect2)).width, 25f);
		Widgets.Label(rect3, playerRelationKind.GetLabelCap());
		curY += ((Rect)(ref rect3)).height;
		GUI.color = Color.white;
		GenUI.ResetLabelAlign();
	}

	private static TaggedString GetRecentEvents(Faction other)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		List<HistoryEventDef> allDefsListForReading = DefDatabase<HistoryEventDef>.AllDefsListForReading;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			int recentCountWithinTicks = Find.HistoryEventsManager.GetRecentCountWithinTicks(allDefsListForReading[i], 3600000, other);
			if (recentCountWithinTicks <= 0)
			{
				continue;
			}
			Find.HistoryEventsManager.GetRecent(allDefsListForReading[i], 3600000, tmpTicks, tmpCustomGoodwill, other);
			int num = 0;
			for (int j = 0; j < tmpTicks.Count; j++)
			{
				num += tmpCustomGoodwill[j];
			}
			if (num != 0)
			{
				string text = "- " + allDefsListForReading[i].LabelCap;
				if (recentCountWithinTicks != 1)
				{
					text = text + " x" + recentCountWithinTicks;
				}
				text = text + ": " + num.ToStringWithSign().Colorize((num >= 0) ? FactionRelationKind.Ally.GetColor() : FactionRelationKind.Hostile.GetColor());
				stringBuilder.AppendInNewLine(text);
			}
		}
		return stringBuilder.ToString();
	}

	private static FactionRelationKind GetRelationKindForGoodwill(int goodwill)
	{
		if (goodwill <= -75)
		{
			return FactionRelationKind.Hostile;
		}
		if (goodwill >= 75)
		{
			return FactionRelationKind.Ally;
		}
		return FactionRelationKind.Neutral;
	}

	private static TaggedString GetOngoingEvents(Faction other)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		List<GoodwillSituationManager.CachedSituation> situations = Find.GoodwillSituationManager.GetSituations(other);
		for (int i = 0; i < situations.Count; i++)
		{
			if (situations[i].maxGoodwill < 100)
			{
				string text = "- " + situations[i].def.Worker.GetPostProcessedLabelCap(other);
				text = text + ": " + (situations[i].maxGoodwill.ToStringWithSign() + " " + "max".Translate()).Colorize(FactionRelationKind.Hostile.GetColor());
				stringBuilder.AppendInNewLine(text);
			}
		}
		return stringBuilder.ToString();
	}

	private static TaggedString GetNaturalGoodwillExplanation(Faction other)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		List<GoodwillSituationManager.CachedSituation> situations = Find.GoodwillSituationManager.GetSituations(other);
		for (int i = 0; i < situations.Count; i++)
		{
			if (situations[i].naturalGoodwillOffset != 0)
			{
				string text = "- " + situations[i].def.Worker.GetPostProcessedLabelCap(other);
				text = text + ": " + situations[i].naturalGoodwillOffset.ToStringWithSign().Colorize((situations[i].naturalGoodwillOffset >= 0) ? FactionRelationKind.Ally.GetColor() : FactionRelationKind.Hostile.GetColor());
				stringBuilder.AppendInNewLine(text);
			}
		}
		return stringBuilder.ToString();
	}
}
