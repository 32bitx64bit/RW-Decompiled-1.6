using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public static class StatsReportUtility
{
	private static StatDrawEntry selectedEntry;

	private static StatDrawEntry mousedOverEntry;

	private static Vector2 scrollPosition;

	private static ScrollPositioner scrollPositioner = new ScrollPositioner();

	private static Vector2 scrollPositionRightPanel;

	private static QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private static float listHeight;

	private static float rightPanelHeight;

	private static List<StatDrawEntry> cachedDrawEntries = new List<StatDrawEntry>();

	private static List<string> cachedEntryValues = new List<string>();

	public static int SelectedStatIndex
	{
		get
		{
			if (cachedDrawEntries.NullOrEmpty() || selectedEntry == null)
			{
				return -1;
			}
			return cachedDrawEntries.IndexOf(selectedEntry);
		}
	}

	public static QuickSearchWidget QuickSearchWidget => quickSearchWidget;

	public static void Reset()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		scrollPosition = default(Vector2);
		scrollPositionRightPanel = default(Vector2);
		selectedEntry = null;
		scrollPositioner.Arm(armed: false);
		mousedOverEntry = null;
		cachedDrawEntries.Clear();
		cachedEntryValues.Clear();
		quickSearchWidget.Reset();
		PermitsCardUtility.selectedPermit = null;
		PermitsCardUtility.selectedFaction = ((ModLister.RoyaltyInstalled && Current.ProgramState == ProgramState.Playing) ? Faction.OfEmpire : null);
	}

	public static void DrawStatsReport(Rect rect, Def def, ThingDef stuff)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			StatRequest req = ((def is BuildableDef def2) ? StatRequest.For(def2, stuff) : StatRequest.ForEmpty());
			cachedDrawEntries.AddRange(def.SpecialDisplayStats(req));
			cachedDrawEntries.AddRange(from r in StatsToDraw(def, stuff)
				where r.ShouldDisplay()
				select r);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, null);
	}

	public static void DrawStatsReport(Rect rect, AbilityDef def)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			StatRequest req = StatRequest.ForEmpty();
			cachedDrawEntries.AddRange(def.SpecialDisplayStats(req));
			cachedDrawEntries.AddRange(from r in StatsToDraw(def)
				where r.ShouldDisplay()
				select r);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, null);
	}

	public static void DrawStatsReport(Rect rect, Thing thing)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			cachedDrawEntries.AddRange(thing.def.SpecialDisplayStats(StatRequest.For(thing)));
			cachedDrawEntries.AddRange(StatsToDraw(thing));
			cachedDrawEntries.RemoveAll((StatDrawEntry de) => (de.stat != null && !de.stat.showNonAbstract) || !de.ShouldDisplay(thing));
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, thing, null);
	}

	public static void DrawStatsReport(Rect rect, Hediff hediff)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			cachedDrawEntries.AddRange(hediff.SpecialDisplayStats(StatRequest.ForEmpty()));
			cachedDrawEntries.AddRange(from r in StatsToDraw(hediff.def, null)
				where r.ShouldDisplay()
				select r);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, null);
	}

	public static void DrawStatsReport(Rect rect, WorldObject worldObject)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			cachedDrawEntries.AddRange(worldObject.def.SpecialDisplayStats(StatRequest.ForEmpty()));
			cachedDrawEntries.AddRange(from r in StatsToDraw(worldObject)
				where r.ShouldDisplay()
				select r);
			cachedDrawEntries.RemoveAll((StatDrawEntry de) => de.stat != null && !de.stat.showNonAbstract);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, worldObject);
	}

	public static void DrawStatsReport(Rect rect, RoyalTitleDef title, Faction faction, Pawn pawn = null)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			cachedDrawEntries.AddRange(title.SpecialDisplayStats(StatRequest.For(title, faction, pawn)));
			cachedDrawEntries.AddRange(from r in StatsToDraw(title, faction)
				where r.ShouldDisplay(pawn)
				select r);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, null);
	}

	public static void DrawStatsReport(Rect rect, Faction faction)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (cachedDrawEntries.NullOrEmpty())
		{
			StatRequest req = StatRequest.ForEmpty();
			cachedDrawEntries.AddRange(faction.def.SpecialDisplayStats(req));
			cachedDrawEntries.AddRange(from r in StatsToDraw(faction)
				where r.ShouldDisplay()
				select r);
			FinalizeCachedDrawEntries(cachedDrawEntries);
		}
		DrawStatsWorker(rect, null, null);
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(Def def, ThingDef stuff)
	{
		yield return DescriptionEntry(def);
		if (def is BuildableDef eDef)
		{
			StatRequest statRequest = StatRequest.For(eDef, stuff);
			foreach (StatDef item in DefDatabase<StatDef>.AllDefs.Where((StatDef st) => st.Worker.ShouldShowFor(statRequest)))
			{
				yield return new StatDrawEntry(item.category, item, eDef.GetStatValueAbstract(item, stuff), StatRequest.For(eDef, stuff));
			}
		}
		if (!(def is ThingDef { IsStuff: not false } thingDef))
		{
			yield break;
		}
		foreach (StatDrawEntry item2 in StuffStats(thingDef))
		{
			yield return item2;
		}
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(RoyalTitleDef title, Faction faction)
	{
		yield return DescriptionEntry(title, faction);
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(Faction faction)
	{
		yield return DescriptionEntry(faction);
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(AbilityDef def)
	{
		yield return DescriptionEntry(def);
		StatRequest statRequest = StatRequest.For(def);
		foreach (StatDef item in DefDatabase<StatDef>.AllDefs.Where((StatDef st) => st.Worker.ShouldShowFor(statRequest)))
		{
			yield return new StatDrawEntry(item.category, item, def.GetStatValueAbstract(item), StatRequest.For(def));
		}
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(Thing thing)
	{
		yield return DescriptionEntry(thing);
		StatDrawEntry statDrawEntry = QualityEntry(thing);
		if (statDrawEntry != null)
		{
			yield return statDrawEntry;
		}
		foreach (StatDef item in DefDatabase<StatDef>.AllDefs.Where((StatDef st) => st.Worker.ShouldShowFor(StatRequest.For(thing))))
		{
			if (!item.Worker.IsDisabledFor(thing))
			{
				float statValue = thing.GetStatValue(item);
				if (item.showOnDefaultValue || statValue != item.defaultBaseValue)
				{
					yield return new StatDrawEntry(item.category, item, statValue, StatRequest.For(thing));
				}
			}
			else
			{
				yield return new StatDrawEntry(item.category, item);
			}
		}
		if (thing.def.useHitPoints)
		{
			yield return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "HitPointsBasic".Translate().CapitalizeFirst(), thing.HitPoints + " / " + thing.MaxHitPoints, "Stat_HitPoints_Desc".Translate(), 99998);
		}
		foreach (StatDrawEntry item2 in thing.SpecialDisplayStats())
		{
			yield return item2;
		}
		if (!thing.def.IsStuff)
		{
			yield break;
		}
		foreach (StatDrawEntry item3 in StuffStats(thing.def))
		{
			yield return item3;
		}
	}

	private static IEnumerable<StatDrawEntry> StatsToDraw(WorldObject worldObject)
	{
		yield return DescriptionEntry(worldObject);
		foreach (StatDrawEntry specialDisplayStat in worldObject.SpecialDisplayStats)
		{
			yield return specialDisplayStat;
		}
	}

	private static IEnumerable<StatDrawEntry> StuffStats(ThingDef stuffDef)
	{
		if (!stuffDef.stuffProps.statFactors.NullOrEmpty())
		{
			for (int i = 0; i < stuffDef.stuffProps.statFactors.Count; i++)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.StuffStatFactors, stuffDef.stuffProps.statFactors[i].stat, stuffDef.stuffProps.statFactors[i].value, StatRequest.ForEmpty(), ToStringNumberSense.Factor);
			}
		}
		if (!stuffDef.stuffProps.statFactorsQuality.NullOrEmpty())
		{
			foreach (StatModifierQuality item in stuffDef.stuffProps.statFactorsQuality)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.StuffStatFactors, item.stat, item.ToStringAsFactorRange);
			}
		}
		if (!stuffDef.stuffProps.statOffsets.NullOrEmpty())
		{
			for (int i = 0; i < stuffDef.stuffProps.statOffsets.Count; i++)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.StuffStatOffsets, stuffDef.stuffProps.statOffsets[i].stat, stuffDef.stuffProps.statOffsets[i].value, StatRequest.ForEmpty(), ToStringNumberSense.Offset);
			}
		}
		if (stuffDef.stuffProps.statOffsetsQuality.NullOrEmpty())
		{
			yield break;
		}
		foreach (StatModifierQuality item2 in stuffDef.stuffProps.statOffsetsQuality)
		{
			yield return new StatDrawEntry(StatCategoryDefOf.StuffStatOffsets, item2.stat, item2.ToStringAsOffsetRange);
		}
	}

	private static void FinalizeCachedDrawEntries(IEnumerable<StatDrawEntry> original)
	{
		cachedDrawEntries = (from sd in original
			orderby sd.category.displayOrder, sd.DisplayPriorityWithinCategory descending, sd.LabelCap
			select sd).ToList();
		quickSearchWidget.noResultsMatched = !cachedDrawEntries.Any();
		foreach (StatDrawEntry cachedDrawEntry in cachedDrawEntries)
		{
			cachedEntryValues.Add(cachedDrawEntry.ValueString);
		}
		if (selectedEntry != null)
		{
			selectedEntry = cachedDrawEntries.FirstOrDefault((StatDrawEntry e) => e.Same(selectedEntry));
		}
		if (!quickSearchWidget.filter.Active)
		{
			return;
		}
		foreach (StatDrawEntry cachedDrawEntry2 in cachedDrawEntries)
		{
			if (Matches(cachedDrawEntry2))
			{
				selectedEntry = cachedDrawEntry2;
				scrollPositioner.Arm();
				break;
			}
		}
	}

	private static bool Matches(StatDrawEntry sd)
	{
		return quickSearchWidget.filter.Matches(sd.LabelCap);
	}

	private static StatDrawEntry DescriptionEntry(Def def)
	{
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", def.description, 99999, null, Dialog_InfoCard.DefsToHyperlinks(def.descriptionHyperlinks));
	}

	private static StatDrawEntry DescriptionEntry(Faction faction)
	{
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", faction.GetReportText, 99999, null, Dialog_InfoCard.DefsToHyperlinks(faction.def.descriptionHyperlinks));
	}

	private static StatDrawEntry DescriptionEntry(RoyalTitleDef title, Faction faction)
	{
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", title.GetReportText(faction), 99999, null, Dialog_InfoCard.TitleDefsToHyperlinks(title.GetHyperlinks(faction)));
	}

	private static StatDrawEntry DescriptionEntry(Thing thing)
	{
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", thing.DescriptionFlavor, 99999, null, Dialog_InfoCard.DefsToHyperlinks(thing.DescriptionHyperlinks), forceUnfinalizedMode: false, overridesHideStats: true);
	}

	private static StatDrawEntry DescriptionEntry(WorldObject worldObject)
	{
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Description".Translate(), "", worldObject.GetDescription(), 99999);
	}

	private static StatDrawEntry QualityEntry(Thing t)
	{
		if (!t.TryGetQuality(out var qc))
		{
			return null;
		}
		return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, "Quality".Translate(), qc.GetLabel().CapitalizeFirst(), "QualityDescription".Translate(), 99999);
	}

	public static void SelectEntry(int index)
	{
		if (index >= 0 && index <= cachedDrawEntries.Count)
		{
			SelectEntry(cachedDrawEntries[index]);
		}
	}

	public static void SelectEntry(StatDef stat, bool playSound = false)
	{
		foreach (StatDrawEntry cachedDrawEntry in cachedDrawEntries)
		{
			if (cachedDrawEntry.stat == stat)
			{
				SelectEntry(cachedDrawEntry, playSound);
				return;
			}
		}
		Messages.Message("MessageCannotSelectInvisibleStat".Translate(stat), MessageTypeDefOf.RejectInput, historical: false);
	}

	private static void SelectEntry(StatDrawEntry rec, bool playSound = true)
	{
		selectedEntry = rec;
		if (playSound)
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
	}

	private static void DrawStatsWorker(Rect rect, Thing optionalThing, WorldObject optionalWorldObject)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(rect);
		((Rect)(ref val)).width = ((Rect)(ref val)).width * 0.5f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		((Rect)(ref rect2)).x = ((Rect)(ref val)).xMax;
		((Rect)(ref rect2)).width = ((Rect)(ref rect)).xMax - ((Rect)(ref rect2)).x;
		scrollPositioner.ClearInterestRects();
		Text.Font = GameFont.Small;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, listHeight);
		Widgets.BeginScrollView(val, ref scrollPosition, viewRect);
		float curY = 0f;
		string text = null;
		mousedOverEntry = null;
		Rect rect3 = default(Rect);
		for (int i = 0; i < cachedDrawEntries.Count; i++)
		{
			StatDrawEntry ent = cachedDrawEntries[i];
			if (ent.category.LabelCap != text)
			{
				Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, ent.category.LabelCap);
				text = ent.category.LabelCap;
			}
			bool highlightLabel = false;
			bool lowlightLabel = false;
			bool flag = selectedEntry == ent;
			bool flag2 = false;
			GUI.color = Color.white;
			if (quickSearchWidget.filter.Active)
			{
				if (Matches(ent))
				{
					highlightLabel = true;
					flag2 = true;
				}
				else
				{
					lowlightLabel = true;
				}
			}
			((Rect)(ref rect3))._002Ector(8f, curY, ((Rect)(ref viewRect)).width - 8f, 30f);
			curY = (((Rect)(ref rect3)).yMax = curY + ent.Draw(((Rect)(ref rect3)).x, ((Rect)(ref rect3)).y, ((Rect)(ref rect3)).width, flag, highlightLabel, lowlightLabel, delegate
			{
				SelectEntry(ent);
			}, delegate
			{
				mousedOverEntry = ent;
			}, scrollPosition, val, cachedEntryValues[i]));
			if (flag || flag2)
			{
				scrollPositioner.RegisterInterestRect(rect3);
			}
		}
		listHeight = curY + 100f;
		Widgets.EndScrollView();
		scrollPositioner.ScrollVertically(ref scrollPosition, ((Rect)(ref val)).size);
		Rect outRect = rect2.ContractedBy(10f);
		StatDrawEntry statDrawEntry = selectedEntry ?? mousedOverEntry ?? cachedDrawEntries.FirstOrDefault();
		if (statDrawEntry == null)
		{
			return;
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, rightPanelHeight);
		StatRequest statRequest = (statDrawEntry.hasOptionalReq ? statDrawEntry.optionalReq : ((optionalThing == null) ? StatRequest.ForEmpty() : StatRequest.For(optionalThing)));
		string explanationText = statDrawEntry.GetExplanationText(statRequest);
		float num2 = 0f;
		Widgets.BeginScrollView(outRect, ref scrollPositionRightPanel, val2);
		Rect rect4 = val2;
		((Rect)(ref rect4)).width = ((Rect)(ref rect4)).width - 4f;
		Widgets.Label(rect4, explanationText);
		float num3 = Text.CalcHeight(explanationText, ((Rect)(ref rect4)).width) + 10f;
		num2 += num3;
		IEnumerable<Dialog_InfoCard.Hyperlink> hyperlinks = statDrawEntry.GetHyperlinks(statRequest);
		if (hyperlinks != null)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(((Rect)(ref rect4)).x, ((Rect)(ref rect4)).y + num3, ((Rect)(ref rect4)).width, ((Rect)(ref rect4)).height - num3);
			Color color = GUI.color;
			GUI.color = Widgets.NormalOptionColor;
			foreach (Dialog_InfoCard.Hyperlink item in hyperlinks)
			{
				float num4 = Text.CalcHeight(item.Label, ((Rect)(ref val3)).width);
				Widgets.HyperlinkWithIcon(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, num4), text: item.HasGeneOwnerThing ? item.Label : "ViewHyperlink".Translate(item.Label).ToString(), hyperlink: item);
				((Rect)(ref val3)).y = ((Rect)(ref val3)).y + num4;
				((Rect)(ref val3)).height = ((Rect)(ref val3)).height - num4;
				num2 += num4;
			}
			GUI.color = color;
		}
		rightPanelHeight = num2;
		Widgets.EndScrollView();
	}

	public static void Notify_QuickSearchChanged()
	{
		cachedDrawEntries.Clear();
		cachedEntryValues.Clear();
	}
}
