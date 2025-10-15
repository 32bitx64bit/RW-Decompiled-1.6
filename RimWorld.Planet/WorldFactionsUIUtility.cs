using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

public static class WorldFactionsUIUtility
{
	private static Vector2 scrollPosition;

	private static float listingHeight;

	private static float warningHeight;

	private const float RowHeight = 24f;

	private const float AddButtonHeight = 28f;

	private const int MaxVisibleFactions = 12;

	private const int MaxVisibleFactionsRecommended = 11;

	private const float RowMarginX = 6f;

	public static void DoWindowContents(Rect rect, List<FactionDef> factions, bool isDefaultFactionCounts)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, Text.LineHeight);
		Widgets.Label(rect2, "Factions".Translate());
		TooltipHandler.TipRegion(rect2, () => "FactionSelectionDesc".Translate(12), 4534123);
		float num = Text.LineHeight + 4f;
		float num2 = ((Rect)(ref rect)).width * 0.050000012f;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x + num2, ((Rect)(ref rect)).y + num, ((Rect)(ref rect)).width * 0.9f, ((Rect)(ref rect)).height - num - Text.LineHeight - 28f - warningHeight);
		Rect outRect = rect3.ContractedBy(4f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width, listingHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		listingHeight = 0f;
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.ColumnWidth = ((Rect)(ref val)).width;
		listing_Standard.Begin(val);
		for (int i = 0; i < factions.Count; i++)
		{
			if (factions[i].displayInFactionSelection)
			{
				listing_Standard.Gap(4f);
				if (DoRow(listing_Standard.GetRect(24f), factions[i], factions, i))
				{
					i--;
				}
				listing_Standard.Gap(4f);
				listingHeight += 32f;
			}
		}
		listing_Standard.End();
		Widgets.EndScrollView();
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref outRect)).x, Mathf.Min(((Rect)(ref rect3)).yMax, ((Rect)(ref outRect)).y + listingHeight + 4f), ((Rect)(ref outRect)).width, 28f);
		if (Widgets.ButtonText(rect4, "Add".Translate().CapitalizeFirst() + "...") && TutorSystem.AllowAction("ConfiguringWorldFactions"))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (FactionDef configurableFaction in FactionGenerator.ConfigurableFactions)
			{
				if (!configurableFaction.displayInFactionSelection)
				{
					continue;
				}
				FactionDef localDef = configurableFaction;
				string text = localDef.LabelCap;
				Action action = delegate
				{
					factions.Add(localDef);
				};
				AcceptanceReport acceptanceReport = CanAddFaction(localDef);
				if (!acceptanceReport)
				{
					action = null;
					if (!acceptanceReport.Reason.NullOrEmpty())
					{
						text = text + " (" + acceptanceReport.Reason + ")";
					}
				}
				else
				{
					int num3 = factions.Count((FactionDef x) => x == localDef);
					if (num3 > 0)
					{
						text = text + " (" + num3 + ")";
					}
				}
				FloatMenuOption floatMenuOption = new FloatMenuOption(text, action, localDef.FactionIcon, localDef.DefaultColor, MenuOptionPriority.Default, null, null, 24f, (Rect r) => Widgets.InfoCardButton(((Rect)(ref r)).x, ((Rect)(ref r)).y + 3f, localDef), null, playSelectionSound: true, 0, HorizontalJustification.Left, extraPartRightJustified: true);
				floatMenuOption.tooltip = text.AsTipTitle() + "\n" + localDef.Description;
				list.Add(floatMenuOption);
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		float yMax = ((Rect)(ref rect4)).yMax;
		int num4 = factions.Count((FactionDef x) => !x.hidden);
		StringBuilder stringBuilder = new StringBuilder();
		if (num4 == 0)
		{
			stringBuilder.AppendLine("FactionsDisabledWarning".Translate());
		}
		else
		{
			if (ModsConfig.RoyaltyActive && !factions.Contains(FactionDefOf.Empire))
			{
				stringBuilder.AppendLine("Warning".Translate() + ": " + "FactionDisabledContentWarning".Translate(FactionDefOf.Empire.label));
			}
			if (!factions.Contains(FactionDefOf.Mechanoid))
			{
				stringBuilder.AppendLine("Warning".Translate() + ": " + "MechanoidsDisabledContentWarning".Translate(FactionDefOf.Mechanoid.label));
			}
			if (!factions.Contains(FactionDefOf.Insect))
			{
				stringBuilder.AppendLine("Warning".Translate() + ": " + "InsectsDisabledContentWarning".Translate(FactionDefOf.Insect.label));
			}
			if (ModsConfig.OdysseyActive)
			{
				if (!factions.Contains(FactionDefOf.Mechanoid))
				{
					stringBuilder.AppendLine("Warning".Translate() + ": " + "OdysseyFactionDisabledContentWarning".Translate(FactionDefOf.Mechanoid.label));
				}
				if (!factions.Contains(FactionDefOf.Insect))
				{
					stringBuilder.AppendLine("Warning".Translate() + ": " + "OdysseyFactionDisabledContentWarning".Translate(FactionDefOf.Insect.label));
				}
			}
		}
		warningHeight = 0f;
		if (stringBuilder.Length > 0)
		{
			bool wordWrap = Text.WordWrap;
			string text2 = stringBuilder.ToString().TrimEndNewlines();
			Rect rect5 = default(Rect);
			((Rect)(ref rect5))._002Ector(((Rect)(ref rect)).x, yMax, ((Rect)(ref rect)).width, ((Rect)(ref rect)).yMax - yMax);
			GUI.color = Color.yellow;
			Text.Font = GameFont.Tiny;
			warningHeight = Text.CalcHeight(text2, ((Rect)(ref rect5)).width);
			Text.WordWrap = true;
			Widgets.Label(rect5, text2);
			Text.WordWrap = wordWrap;
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
		}
		AcceptanceReport CanAddFaction(FactionDef f)
		{
			if (!f.hidden && factions.Count((FactionDef x) => !x.hidden) >= 12)
			{
				return "TotalFactionsAllowed".Translate(12).ToString().UncapitalizeFirst();
			}
			if (factions.Count((FactionDef x) => x == f) >= f.maxConfigurableAtWorldCreation)
			{
				return "MaxFactionsForType".Translate(f.maxConfigurableAtWorldCreation).ToString().UncapitalizeFirst();
			}
			return true;
		}
	}

	public static bool DoRow(Rect rect, FactionDef factionDef, List<FactionDef> factions, int index)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		bool flag = true;
		foreach (ScenPart part in Current.Game.Scenario.parts)
		{
			if (part.def.preventRemovalOfFaction == factionDef)
			{
				flag = false;
				break;
			}
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y - 4f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height + 8f);
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect2);
		}
		Widgets.BeginGroup(rect);
		WidgetRow widgetRow = new WidgetRow(6f, 0f);
		GUI.color = factionDef.DefaultColor;
		widgetRow.Icon((Texture)(object)factionDef.FactionIcon);
		GUI.color = Color.white;
		widgetRow.Gap(4f);
		Text.Anchor = (TextAnchor)4;
		widgetRow.Label(factionDef.LabelCap);
		Text.Anchor = (TextAnchor)0;
		if (flag && Widgets.ButtonImage(new Rect(((Rect)(ref rect)).width - 24f - 6f, 0f, 24f, 24f), TexButton.Delete) && TutorSystem.AllowAction("ConfiguringWorldFactions"))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			factions.RemoveAt(index);
			result = true;
		}
		Widgets.EndGroup();
		if (Mouse.IsOver(rect2))
		{
			string text = factionDef.LabelCap.AsTipTitle() + "\n" + factionDef.Description;
			if (!flag)
			{
				text = text + "\n\n" + "FactionRemovalDisabled".Translate().Colorize(ColorLibrary.RedReadable);
			}
			TooltipHandler.TipRegion(rect2, text);
			Widgets.DrawHighlight(rect2);
		}
		return result;
	}
}
