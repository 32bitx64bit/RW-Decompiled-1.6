using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_SellableItems : Window
{
	private ThingCategoryDef currentCategory;

	private bool pawnsTabOpen;

	private List<ThingDef> sellableItems = new List<ThingDef>();

	private List<TabRecord> tabs = new List<TabRecord>();

	private Vector2 scrollPosition;

	private ITrader trader;

	private List<ThingDef> cachedSellablePawns;

	private Dictionary<ThingCategoryDef, List<ThingDef>> cachedSellableItemsByCategory = new Dictionary<ThingCategoryDef, List<ThingDef>>();

	private const float RowHeight = 24f;

	private const float TitleRectHeight = 40f;

	private const float RestockTextHeight = 20f;

	private const float BottomAreaHeight = 55f;

	private readonly Vector2 BottomButtonSize = new Vector2(160f, 40f);

	public override Vector2 InitialSize => new Vector2(650f, (float)Mathf.Min(UI.screenHeight, 1000));

	protected override float Margin => 0f;

	public Dialog_SellableItems(ITrader trader)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		forcePause = true;
		absorbInputAroundWindow = true;
		this.trader = trader;
		CalculateSellableItems(trader.TraderKind);
		CalculateTabs();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		float num = 40f;
		Rect rect = new Rect(0f, 0f, ((Rect)(ref inRect)).width, 40f);
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, "SellableItemsTitle".Translate().CapitalizeFirst());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		if (trader is ITraderRestockingInfoProvider { NextRestockTick: var nextRestockTick } traderRestockingInfoProvider)
		{
			if (nextRestockTick != -1)
			{
				float num2 = (nextRestockTick - Find.TickManager.TicksGame).TicksToDays();
				Widgets.Label(new Rect(0f, num, ((Rect)(ref inRect)).width, 20f), "NextTraderRestock".Translate(num2.ToString("0.0")));
				num += 20f;
			}
			else if (!traderRestockingInfoProvider.EverVisited)
			{
				Widgets.Label(new Rect(0f, num, ((Rect)(ref inRect)).width, 20f), "TraderNotVisitedYet".Translate());
				num += 20f;
			}
			else if (traderRestockingInfoProvider.RestockedSinceLastVisit)
			{
				Widgets.Label(new Rect(0f, num, ((Rect)(ref inRect)).width, 20f), "TraderRestockedSinceLastVisit".Translate());
				num += 20f;
			}
		}
		Text.Anchor = (TextAnchor)0;
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + (64f + num);
		Widgets.DrawMenuSection(inRect);
		TabDrawer.DrawTabs(inRect, tabs);
		inRect = inRect.ContractedBy(17f);
		Widgets.BeginGroup(inRect);
		Rect val = inRect.AtZero();
		DoBottomButtons(val);
		Rect outRect = val;
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - 65f;
		List<ThingDef> sellableItemsInCategory = GetSellableItemsInCategory(currentCategory, pawnsTabOpen);
		if (sellableItemsInCategory.Any())
		{
			float num3 = (float)sellableItemsInCategory.Count * 24f;
			num = 0f;
			Rect viewRect = default(Rect);
			((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, num3);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float num4 = scrollPosition.y - 24f;
			float num5 = scrollPosition.y + ((Rect)(ref outRect)).height;
			for (int i = 0; i < sellableItemsInCategory.Count; i++)
			{
				if (num > num4 && num < num5)
				{
					Widgets.DefLabelWithIcon(new Rect(0f, num, ((Rect)(ref viewRect)).width, 24f), sellableItemsInCategory[i]);
				}
				num += 24f;
			}
			Widgets.EndScrollView();
		}
		else
		{
			Widgets.NoneLabel(0f, ((Rect)(ref outRect)).width);
		}
		Widgets.EndGroup();
	}

	private void DoBottomButtons(Rect rect)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f - BottomButtonSize.x / 2f, ((Rect)(ref rect)).height - 55f, BottomButtonSize.x, BottomButtonSize.y), "CloseButton".Translate()))
		{
			Close();
		}
	}

	private void CalculateSellableItems(TraderKindDef trader)
	{
		sellableItems.Clear();
		cachedSellableItemsByCategory.Clear();
		cachedSellablePawns = null;
		List<ThingDef> allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			if (allDefsListForReading[i].PlayerAcquirable && !allDefsListForReading[i].IsCorpse && !typeof(MinifiedThing).IsAssignableFrom(allDefsListForReading[i].thingClass) && trader.WillTrade(allDefsListForReading[i]) && TradeUtility.EverPlayerSellable(allDefsListForReading[i]))
			{
				sellableItems.Add(allDefsListForReading[i]);
			}
		}
		sellableItems.SortBy((ThingDef x) => x.label);
	}

	private void CalculateTabs()
	{
		tabs.Clear();
		List<ThingCategoryDef> allDefsListForReading = DefDatabase<ThingCategoryDef>.AllDefsListForReading;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			ThingCategoryDef category = allDefsListForReading[i];
			if (category.parent == ThingCategoryDefOf.Root && category != ThingCategoryDefOf.Animals && GetSellableItemsInCategory(category, pawns: false).Count != 0)
			{
				if (currentCategory == null)
				{
					currentCategory = category;
				}
				tabs.Add(new TabRecord(category.LabelCap, delegate
				{
					currentCategory = category;
					pawnsTabOpen = false;
				}, () => currentCategory == category));
			}
		}
		tabs.Add(new TabRecord("PawnsTabShort".Translate(), delegate
		{
			currentCategory = null;
			pawnsTabOpen = true;
		}, () => pawnsTabOpen));
	}

	private List<ThingDef> GetSellableItemsInCategory(ThingCategoryDef category, bool pawns)
	{
		if (pawns)
		{
			if (cachedSellablePawns == null)
			{
				cachedSellablePawns = new List<ThingDef>();
				for (int i = 0; i < sellableItems.Count; i++)
				{
					if (sellableItems[i].category == ThingCategory.Pawn && (!ModsConfig.AnomalyActive || sellableItems[i] != ThingDefOf.CreepJoiner))
					{
						cachedSellablePawns.Add(sellableItems[i]);
					}
				}
			}
			return cachedSellablePawns;
		}
		if (cachedSellableItemsByCategory.TryGetValue(category, out var value))
		{
			return value;
		}
		value = new List<ThingDef>();
		for (int j = 0; j < sellableItems.Count; j++)
		{
			if (sellableItems[j].IsWithinCategory(category))
			{
				value.Add(sellableItems[j]);
			}
		}
		cachedSellableItemsByCategory.Add(category, value);
		return value;
	}
}
