using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Dialog_Trade : Window
{
	private bool giftsOnly;

	private Vector2 scrollPosition = Vector2.zero;

	private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	public static float lastCurrencyFlashTime = -100f;

	private List<Tradeable> cachedTradeables;

	private Tradeable cachedCurrencyTradeable;

	private TransferableSorterDef sorter1;

	private TransferableSorterDef sorter2;

	private bool playerIsCaravan;

	private bool isShuttleCaravan;

	private List<Thing> playerCaravanAllPawnsAndItems;

	private bool massUsageDirty = true;

	private float cachedMassUsage;

	private bool massCapacityDirty = true;

	private float cachedMassCapacity;

	private string cachedMassCapacityExplanation;

	private bool tilesPerDayDirty = true;

	private float cachedTilesPerDay;

	private string cachedTilesPerDayExplanation;

	private bool daysWorthOfFoodDirty = true;

	private (float days, float tillRot) cachedDaysWorthOfFood;

	private bool foragedFoodPerDayDirty = true;

	private (ThingDef food, float perDay) cachedForagedFoodPerDay;

	private string cachedForagedFoodPerDayExplanation;

	private bool visibilityDirty = true;

	private float cachedVisibility;

	private string cachedVisibilityExplanation;

	private const float TitleAreaHeight = 45f;

	private const float TopAreaHeight = 58f;

	private const float ColumnWidth = 120f;

	private const float FirstCommodityY = 6f;

	private const float RowInterval = 30f;

	private const float SpaceBetweenTraderNameAndTraderKind = 27f;

	private const float ShowSellableItemsIconSize = 32f;

	private const float GiftModeIconSize = 32f;

	private const float TradeModeIconSize = 32f;

	protected static readonly Vector2 AcceptButtonSize = new Vector2(160f, 40f);

	protected static readonly Vector2 OtherBottomButtonSize = new Vector2(160f, 40f);

	private static readonly Texture2D ShowSellableItemsIcon = ContentFinder<Texture2D>.Get("UI/Commands/SellableItems");

	private static readonly Texture2D GiftModeIcon = ContentFinder<Texture2D>.Get("UI/Buttons/GiftMode");

	private static readonly Texture2D TradeModeIcon = ContentFinder<Texture2D>.Get("UI/Buttons/TradeMode");

	public override Vector2 InitialSize => new Vector2(1024f, (float)UI.screenHeight);

	private PlanetTile Tile => TradeSession.playerNegotiator.Tile;

	private BiomeDef Biome => Find.WorldGrid[Tile].PrimaryBiome;

	private float MassUsage
	{
		get
		{
			if (massUsageDirty)
			{
				massUsageDirty = false;
				TradeSession.deal.UpdateCurrencyCount();
				if (cachedCurrencyTradeable != null)
				{
					cachedTradeables.Add(cachedCurrencyTradeable);
				}
				Building_PassengerShuttle shuttle = TradeSession.playerNegotiator.GetCaravan().Shuttle;
				if (shuttle != null)
				{
					cachedMassUsage = CollectionsMassCalculator.MassUsageLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, IgnorePawnsInventoryMode.Ignore, includePawnsMass: true);
					cachedMassUsage -= shuttle.GetStatValue(StatDefOf.Mass);
				}
				else
				{
					cachedMassUsage = CollectionsMassCalculator.MassUsageLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, IgnorePawnsInventoryMode.Ignore);
				}
				if (cachedCurrencyTradeable != null)
				{
					cachedTradeables.RemoveLast();
				}
			}
			return cachedMassUsage;
		}
	}

	private float MassCapacity
	{
		get
		{
			if (massCapacityDirty)
			{
				massCapacityDirty = false;
				Building_PassengerShuttle shuttle = TradeSession.playerNegotiator.GetCaravan().Shuttle;
				if (shuttle != null)
				{
					cachedMassCapacity = shuttle.TransporterComp.MassCapacity;
					return cachedMassCapacity;
				}
				TradeSession.deal.UpdateCurrencyCount();
				if (cachedCurrencyTradeable != null)
				{
					cachedTradeables.Add(cachedCurrencyTradeable);
				}
				StringBuilder stringBuilder = new StringBuilder();
				cachedMassCapacity = CollectionsMassCalculator.CapacityLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, stringBuilder);
				cachedMassCapacityExplanation = stringBuilder.ToString();
				if (cachedCurrencyTradeable != null)
				{
					cachedTradeables.RemoveLast();
				}
			}
			return cachedMassCapacity;
		}
	}

	private float TilesPerDay
	{
		get
		{
			if (tilesPerDayDirty)
			{
				tilesPerDayDirty = false;
				TradeSession.deal.UpdateCurrencyCount();
				Caravan caravan = TradeSession.playerNegotiator.GetCaravan();
				if (caravan.Shuttle != null)
				{
					cachedTilesPerDayExplanation = "CaravanMovementSpeedShuttle".Translate();
					return 0f;
				}
				StringBuilder stringBuilder = new StringBuilder();
				cachedTilesPerDay = TilesPerDayCalculator.ApproxTilesPerDayLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, MassUsage, MassCapacity, Tile, caravan.pather.Moving ? caravan.pather.nextTile : PlanetTile.Invalid, TradeSession.playerNegotiator.GetCaravan().Shuttle != null, stringBuilder);
				cachedTilesPerDayExplanation = stringBuilder.ToString();
			}
			return cachedTilesPerDay;
		}
	}

	private (float days, float tillRot) DaysWorthOfFood
	{
		get
		{
			if (daysWorthOfFoodDirty)
			{
				daysWorthOfFoodDirty = false;
				TradeSession.deal.UpdateCurrencyCount();
				float item = DaysWorthOfFoodCalculator.ApproxDaysWorthOfFoodLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, Tile, IgnorePawnsInventoryMode.Ignore, Faction.OfPlayer);
				cachedDaysWorthOfFood = (days: item, tillRot: DaysUntilRotCalculator.ApproxDaysUntilRotLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, Tile, IgnorePawnsInventoryMode.Ignore));
			}
			return cachedDaysWorthOfFood;
		}
	}

	private (ThingDef food, float perDay) ForagedFoodPerDay
	{
		get
		{
			if (foragedFoodPerDayDirty)
			{
				foragedFoodPerDayDirty = false;
				TradeSession.deal.UpdateCurrencyCount();
				StringBuilder stringBuilder = new StringBuilder();
				cachedForagedFoodPerDay = ForagedFoodPerDayCalculator.ForagedFoodPerDayLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, Biome, Faction.OfPlayer, stringBuilder);
				cachedForagedFoodPerDayExplanation = stringBuilder.ToString();
			}
			return cachedForagedFoodPerDay;
		}
	}

	private float Visibility
	{
		get
		{
			if (visibilityDirty)
			{
				visibilityDirty = false;
				TradeSession.deal.UpdateCurrencyCount();
				StringBuilder stringBuilder = new StringBuilder();
				cachedVisibility = CaravanVisibilityCalculator.VisibilityLeftAfterTradeableTransfer(playerCaravanAllPawnsAndItems, cachedTradeables, stringBuilder);
				cachedVisibilityExplanation = stringBuilder.ToString();
			}
			return cachedVisibility;
		}
	}

	public override QuickSearchWidget CommonSearchWidget => quickSearchWidget;

	public Dialog_Trade(Pawn playerNegotiator, ITrader trader, bool giftsOnly = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this.giftsOnly = giftsOnly;
		TradeSession.SetupWith(trader, playerNegotiator, giftsOnly);
		SetupPlayerCaravanVariables();
		forcePause = true;
		absorbInputAroundWindow = true;
		soundAppear = SoundDefOf.CommsWindow_Open;
		soundClose = SoundDefOf.CommsWindow_Close;
		if (trader is PassingShip)
		{
			soundAmbient = SoundDefOf.RadioComms_Ambience;
		}
		commonSearchWidgetOffset.x += 18f;
		commonSearchWidgetOffset.y -= 18f;
		sorter1 = TransferableSorterDefOf.Category;
		sorter2 = TransferableSorterDefOf.MarketValue;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		quickSearchWidget.Reset();
	}

	public override void PostOpen()
	{
		base.PostOpen();
		if (!giftsOnly && !playerIsCaravan)
		{
			Pawn playerNegotiator = TradeSession.playerNegotiator;
			float level = playerNegotiator.health.capacities.GetLevel(PawnCapacityDefOf.Talking);
			float level2 = playerNegotiator.health.capacities.GetLevel(PawnCapacityDefOf.Hearing);
			if (level < 0.95f || level2 < 0.95f)
			{
				TaggedString text = ((!(level < 0.95f)) ? "NegotiatorHearingImpaired".Translate(playerNegotiator.LabelShort, playerNegotiator) : "NegotiatorTalkingImpaired".Translate(playerNegotiator.LabelShort, playerNegotiator));
				text += "\n\n" + "NegotiatorCapacityImpaired".Translate();
				Find.WindowStack.Add(new Dialog_MessageBox(text));
			}
		}
		CacheTradeables();
	}

	private void CacheTradeables()
	{
		cachedCurrencyTradeable = TradeSession.deal.AllTradeables.FirstOrDefault((Tradeable x) => x.IsCurrency && (TradeSession.TradeCurrency != TradeCurrency.Favor || x.IsFavor));
		cachedTradeables = (from tr in TradeSession.deal.AllTradeables
			where !tr.IsCurrency && (tr.TraderWillTrade || !TradeSession.trader.TraderKind.hideThingsNotWillingToTrade)
			where quickSearchWidget.filter.Matches(tr.Label)
			orderby (!tr.TraderWillTrade) ? (-1) : 0 descending
			select tr).ThenBy((Tradeable tr) => tr, sorter1.Comparer).ThenBy((Tradeable tr) => tr, sorter2.Comparer).ThenBy((Tradeable tr) => TransferableUIUtility.DefaultListOrderPriority(tr))
			.ThenBy((Tradeable tr) => tr.ThingDef.label)
			.ThenBy((Tradeable tr) => tr.AnyThing.TryGetQuality(out var qc) ? ((int)qc) : (-1))
			.ThenBy((Tradeable tr) => tr.AnyThing.HitPoints)
			.ToList();
		quickSearchWidget.noResultsMatched = !cachedTradeables.Any();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		if (playerIsCaravan)
		{
			CaravanUIUtility.DrawCaravanInfo(new CaravanUIUtility.CaravanInfo(MassUsage, MassCapacity, cachedMassCapacityExplanation, TilesPerDay, cachedTilesPerDayExplanation, DaysWorthOfFood, ForagedFoodPerDay, cachedForagedFoodPerDayExplanation, Visibility, cachedVisibilityExplanation), null, Tile, null, -9999f, new Rect(12f, 0f, ((Rect)(ref inRect)).width - 24f, 40f));
			((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 52f;
		}
		TradeSession.deal.UpdateCurrencyCount();
		Widgets.BeginGroup(inRect);
		inRect = inRect.AtZero();
		TransferableUIUtility.DoTransferableSorters(sorter1, sorter2, delegate(TransferableSorterDef x)
		{
			sorter1 = x;
			CacheTradeables();
		}, delegate(TransferableSorterDef x)
		{
			sorter2 = x;
			CacheTradeables();
		});
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(new Rect(0f, 27f, ((Rect)(ref inRect)).width / 2f, ((Rect)(ref inRect)).height / 2f), "NegotiatorTradeDialogInfo".Translate(TradeSession.playerNegotiator.Name.ToStringFull, TradeSession.playerNegotiator.GetStatValue(StatDefOf.TradePriceImprovement).ToStringPercent()));
		float num = ((Rect)(ref inRect)).width - 590f;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(num, 0f, ((Rect)(ref inRect)).width - num, 58f);
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Medium;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 0f, ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).height);
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(rect2, Faction.OfPlayer.Name.Truncate(((Rect)(ref rect2)).width));
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).width / 2f, 0f, ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).height);
		Text.Anchor = (TextAnchor)2;
		string text = TradeSession.trader.TraderName;
		if (Text.CalcSize(text).x > ((Rect)(ref rect3)).width)
		{
			Text.Font = GameFont.Small;
			text = text.Truncate(((Rect)(ref rect3)).width);
		}
		Widgets.Label(rect3, text);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)2;
		Widgets.Label(new Rect(((Rect)(ref rect)).width / 2f, 27f, ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).height / 2f), TradeSession.trader.TraderKind.LabelCap);
		Text.Anchor = (TextAnchor)0;
		if (!TradeSession.giftMode)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.6f);
			Text.Font = GameFont.Tiny;
			Rect rect4 = new Rect(((Rect)(ref rect)).width / 2f - 100f - 30f, 0f, 200f, ((Rect)(ref rect)).height);
			Text.Anchor = (TextAnchor)7;
			Widgets.Label(rect4, "PositiveBuysNegativeSells".Translate());
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
		float num2 = 0f;
		if (cachedCurrencyTradeable != null)
		{
			float num3 = ((Rect)(ref inRect)).width - 16f;
			Rect rect5 = new Rect(0f, 58f, num3, 30f);
			int countToTransfer = cachedCurrencyTradeable.CountToTransfer;
			TradeUI.DrawTradeableRow(rect5, cachedCurrencyTradeable, 1);
			if (countToTransfer != cachedCurrencyTradeable.CountToTransfer)
			{
				CountToTransferChanged();
			}
			GUI.color = Color.gray;
			Widgets.DrawLineHorizontal(0f, 87f, num3);
			GUI.color = Color.white;
			num2 = 30f;
		}
		Rect mainRect = default(Rect);
		((Rect)(ref mainRect))._002Ector(0f, 58f + num2, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - 58f - 38f - num2 - 20f);
		FillMainRect(mainRect);
		Text.Font = GameFont.Small;
		Rect rect6 = default(Rect);
		((Rect)(ref rect6))._002Ector(((Rect)(ref inRect)).width / 2f - AcceptButtonSize.x / 2f, ((Rect)(ref inRect)).height - 55f, AcceptButtonSize.x, AcceptButtonSize.y);
		if (Widgets.ButtonText(rect6, TradeSession.giftMode ? ("OfferGifts".Translate() + " (" + FactionGiftUtility.GetGoodwillChange(TradeSession.deal.AllTradeables, TradeSession.trader.Faction).ToStringWithSign() + ")") : "AcceptButton".Translate()))
		{
			Action action = delegate
			{
				if (TradeSession.deal.TryExecute(out var actuallyTraded))
				{
					if (actuallyTraded)
					{
						SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
						TradeSession.playerNegotiator.GetCaravan()?.RecacheInventory();
						Close(doCloseSound: false);
					}
					else
					{
						Close();
					}
				}
			};
			if (TradeSession.deal.DoesTraderHaveEnoughSilver())
			{
				action();
			}
			else
			{
				FlashSilver();
				SoundDefOf.ClickReject.PlayOneShotOnCamera();
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmTraderShortFunds".Translate(), action));
			}
			Event.current.Use();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect6)).x - 10f - OtherBottomButtonSize.x, ((Rect)(ref rect6)).y, OtherBottomButtonSize.x, OtherBottomButtonSize.y), "ResetButton".Translate()))
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			TradeSession.deal.Reset();
			CacheTradeables();
			CountToTransferChanged();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect6)).xMax + 10f, ((Rect)(ref rect6)).y, OtherBottomButtonSize.x, OtherBottomButtonSize.y), "CancelButton".Translate()))
		{
			Close();
			Event.current.Use();
		}
		float y = OtherBottomButtonSize.y;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref inRect)).width - y, ((Rect)(ref rect6)).y, y, y);
		if (Widgets.ButtonImageWithBG(val, ShowSellableItemsIcon, (Vector2?)new Vector2(32f, 32f)))
		{
			Find.WindowStack.Add(new Dialog_SellableItems(TradeSession.trader));
		}
		TooltipHandler.TipRegionByKey(val, "CommandShowSellableItemsDesc");
		Faction faction = TradeSession.trader.Faction;
		if (faction != null && !giftsOnly && !faction.def.permanentEnemy && TradeSession.trader.TradeCurrency != TradeCurrency.Favor)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref val)).x - y - 4f, ((Rect)(ref rect6)).y, y, y);
			if (TradeSession.giftMode)
			{
				if (Widgets.ButtonImageWithBG(val2, TradeModeIcon, (Vector2?)new Vector2(32f, 32f)))
				{
					TradeSession.giftMode = false;
					TradeSession.deal.Reset();
					CacheTradeables();
					CountToTransferChanged();
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				TooltipHandler.TipRegionByKey(val2, "TradeModeTip");
			}
			else
			{
				if (Widgets.ButtonImageWithBG(val2, GiftModeIcon, (Vector2?)new Vector2(32f, 32f)))
				{
					TradeSession.giftMode = true;
					TradeSession.deal.Reset();
					CacheTradeables();
					CountToTransferChanged();
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				TooltipHandler.TipRegionByKey(val2, "GiftModeTip", faction.Name);
			}
		}
		Widgets.EndGroup();
	}

	public override void Close(bool doCloseSound = true)
	{
		DragSliderManager.ForceStop();
		base.Close(doCloseSound);
		if (TradeSession.trader is Pawn pawn && pawn.mindState.hasQuest)
		{
			TradeUtility.ReceiveQuestFromTrader(pawn, TradeSession.playerNegotiator);
		}
	}

	private void FillMainRect(Rect mainRect)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		float num = 6f + (float)cachedTradeables.Count * 30f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref mainRect)).width - 16f, num);
		Widgets.BeginScrollView(mainRect, ref scrollPosition, viewRect);
		float num2 = 6f;
		float num3 = scrollPosition.y - 30f;
		float num4 = scrollPosition.y + ((Rect)(ref mainRect)).height;
		int num5 = 0;
		for (int i = 0; i < cachedTradeables.Count; i++)
		{
			if (num2 > num3 && num2 < num4)
			{
				Rect rect = new Rect(0f, num2, ((Rect)(ref viewRect)).width, 30f);
				int countToTransfer = cachedTradeables[i].CountToTransfer;
				TradeUI.DrawTradeableRow(rect, cachedTradeables[i], num5);
				if (countToTransfer != cachedTradeables[i].CountToTransfer)
				{
					CountToTransferChanged();
				}
			}
			num2 += 30f;
			num5++;
		}
		Widgets.EndScrollView();
	}

	public void FlashSilver()
	{
		lastCurrencyFlashTime = Time.time;
	}

	public override bool CausesMessageBackground()
	{
		return true;
	}

	private void SetupPlayerCaravanVariables()
	{
		Caravan caravan = TradeSession.playerNegotiator.GetCaravan();
		if (caravan != null)
		{
			playerIsCaravan = true;
			playerCaravanAllPawnsAndItems = new List<Thing>();
			List<Pawn> pawnsListForReading = caravan.PawnsListForReading;
			for (int i = 0; i < pawnsListForReading.Count; i++)
			{
				playerCaravanAllPawnsAndItems.Add(pawnsListForReading[i]);
			}
			playerCaravanAllPawnsAndItems.AddRange(CaravanInventoryUtility.AllInventoryItems(caravan));
			caravan.Notify_StartedTrading();
		}
		else
		{
			playerIsCaravan = false;
		}
	}

	private void CountToTransferChanged()
	{
		massUsageDirty = true;
		massCapacityDirty = true;
		tilesPerDayDirty = true;
		daysWorthOfFoodDirty = true;
		foragedFoodPerDayDirty = true;
		visibilityDirty = true;
	}

	public override void Notify_CommonSearchChanged()
	{
		CacheTradeables();
	}
}
