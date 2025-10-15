using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public static class ThingFilterUI
{
	public class UIState
	{
		public Vector2 scrollPosition;

		public QuickSearchWidget quickSearch = new QuickSearchWidget();
	}

	private static float viewHeight;

	private const float ExtraViewHeight = 90f;

	private const float RangeLabelTab = 10f;

	private const float RangeLabelHeight = 19f;

	private const float SliderHeight = 32f;

	private const float SliderTab = 20f;

	public static void DoThingFilterConfigWindow(Rect rect, UIState state, ThingFilter filter, ThingFilter parentFilter = null, int openMask = 1, IEnumerable<ThingDef> forceHiddenDefs = null, IEnumerable<SpecialThingFilterDef> forceHiddenFilters = null, bool forceHideHitPointsConfig = false, bool forceHideQualityConfig = false, bool showMentalBreakChanceRange = false, List<ThingDef> suppressSmallVolumeTags = null, Map map = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Invalid comparison between Unknown and I4
		Widgets.DrawMenuSection(rect);
		float num = ((Rect)(ref rect)).width - 2f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + 3f, ((Rect)(ref rect)).y + 3f, num / 2f - 3f - 1.5f, 24f);
		if (Widgets.ButtonText(rect2, "ClearAll".Translate()))
		{
			filter.SetDisallowAll(forceHiddenDefs, forceHiddenFilters);
			SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect2)).xMax + 3f, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width, 24f), "AllowAll".Translate()))
		{
			filter.SetAllowAll(parentFilter);
			SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
		}
		((Rect)(ref rect)).yMin = ((Rect)(ref rect2)).yMax;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x + 3f, ((Rect)(ref rect)).y + 3f, ((Rect)(ref rect)).width - 16f - 6f, 24f);
		state.quickSearch.OnGUI(rect3);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect3)).yMax + 3f;
		TreeNode_ThingCategory node = filter.RootNode;
		bool flag = true;
		bool flag2 = true;
		if (parentFilter != null)
		{
			node = parentFilter.DisplayRootCategory;
			flag = parentFilter.allowedHitPointsConfigurable;
			flag2 = parentFilter.allowedQualitiesConfigurable;
		}
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 4f;
		((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 6f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, viewHeight);
		Rect visibleRect = default(Rect);
		((Rect)(ref visibleRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
		((Rect)(ref visibleRect)).position = ((Rect)(ref visibleRect)).position + state.scrollPosition;
		Widgets.BeginScrollView(rect, ref state.scrollPosition, viewRect);
		float y = 2f;
		if (flag && !forceHideHitPointsConfig)
		{
			DrawHitPointsFilterConfig(ref y, ((Rect)(ref viewRect)).width, filter);
		}
		if (flag2 && !forceHideQualityConfig)
		{
			DrawQualityFilterConfig(ref y, ((Rect)(ref viewRect)).width, filter);
		}
		if (ModsConfig.AnomalyActive && showMentalBreakChanceRange)
		{
			DrawMentalBreakFilterConfig(ref y, ((Rect)(ref viewRect)).width, filter);
		}
		float num2 = y;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 9999f);
		((Rect)(ref visibleRect)).position = ((Rect)(ref visibleRect)).position - ((Rect)(ref rect4)).position;
		Listing_TreeThingFilter listing_TreeThingFilter = new Listing_TreeThingFilter(filter, parentFilter, forceHiddenDefs, forceHiddenFilters, suppressSmallVolumeTags, state.quickSearch.filter);
		listing_TreeThingFilter.Begin(rect4);
		listing_TreeThingFilter.ListCategoryChildren(node, openMask, map, visibleRect);
		listing_TreeThingFilter.End();
		state.quickSearch.noResultsMatched = listing_TreeThingFilter.matchCount == 0;
		if ((int)Event.current.type == 8)
		{
			viewHeight = num2 + listing_TreeThingFilter.CurHeight + 90f;
		}
		Widgets.EndScrollView();
	}

	private static void DrawHitPointsFilterConfig(ref float y, float width, ThingFilter filter)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(20f, y, width - 20f, 32f);
		FloatRange range = filter.AllowedHitPointsPercents;
		Widgets.FloatRange(rect, 1, ref range, 0f, 1f, "HitPoints", ToStringStyle.PercentZero, 0f, GameFont.Small, null, 0.01f);
		filter.AllowedHitPointsPercents = range;
		y += 32f;
		y += 5f;
		Text.Font = GameFont.Small;
	}

	private static void DrawQualityFilterConfig(ref float y, float width, ThingFilter filter)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(20f, y, width - 20f, 32f);
		QualityRange range = filter.AllowedQualityLevels;
		Widgets.QualityRange(rect, 876813230, ref range);
		filter.AllowedQualityLevels = range;
		y += 32f;
		y += 5f;
		Text.Font = GameFont.Small;
	}

	private static void DrawMentalBreakFilterConfig(ref float y, float width, ThingFilter filter)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(20f, y, width - 20f, 32f);
		FloatRange range = filter.AllowedMentalBreakChance;
		Widgets.FloatRange(rect, 968573221, ref range, 0f, 1f, "MaxMentalBreakChance", ToStringStyle.PercentZero);
		filter.AllowedMentalBreakChance = range;
		y += 32f;
		y += 5f;
		Text.Font = GameFont.Small;
	}
}
