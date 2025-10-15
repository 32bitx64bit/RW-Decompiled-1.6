using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class TradeUI
{
	public const float CountColumnWidth = 75f;

	public const float PriceColumnWidth = 100f;

	public const float AdjustColumnWidth = 240f;

	public const float TotalNumbersColumnsWidths = 590f;

	public static readonly Color NoTradeColor = new Color(0.5f, 0.5f, 0.5f);

	public static void DrawTradeableRow(Rect rect, Tradeable trad, int index)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		Text.Font = GameFont.Small;
		Widgets.BeginGroup(rect);
		float width = ((Rect)(ref rect)).width;
		int num = trad.CountHeldBy(Transactor.Trader);
		if (num != 0 && trad.IsThing)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(width - 75f, 0f, 75f, ((Rect)(ref rect)).height);
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
			}
			Text.Anchor = (TextAnchor)5;
			Rect rect2 = val;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 5f;
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 5f;
			Widgets.Label(rect2, num.ToStringCached());
			TooltipHandler.TipRegionByKey(val, "TraderCount");
			Rect rect3 = new Rect(((Rect)(ref val)).x - 100f, 0f, 100f, ((Rect)(ref rect)).height);
			Text.Anchor = (TextAnchor)5;
			DrawPrice(rect3, trad, TradeAction.PlayerBuys);
		}
		width -= 175f;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(width - 240f, 0f, 240f, ((Rect)(ref rect)).height);
		if (!trad.TraderWillTrade)
		{
			DrawWillNotTradeText(rect4, "TraderWillNotTrade".Translate());
		}
		else if (ModsConfig.IdeologyActive && TransferableUIUtility.TradeIsPlayerSellingToSlavery(trad, TradeSession.trader.Faction) && !new HistoryEvent(HistoryEventDefOf.SoldSlave, TradeSession.playerNegotiator.Named(HistoryEventArgsNames.Doer)).DoerWillingToDo())
		{
			DrawWillNotTradeText(rect4, "NegotiatorWillNotTradeSlaves".Translate(TradeSession.playerNegotiator));
			if (Mouse.IsOver(rect4))
			{
				Widgets.DrawHighlight(rect4);
				TooltipHandler.TipRegion(rect4, "NegotiatorWillNotTradeSlavesTip".Translate(TradeSession.playerNegotiator, TradeSession.playerNegotiator.Ideo.name));
			}
		}
		else
		{
			bool flash = Time.time - Dialog_Trade.lastCurrencyFlashTime < 1f && trad.IsCurrency;
			TransferableUIUtility.DoCountAdjustInterface(rect4, trad, index, trad.GetMinimumToTransfer(), trad.GetMaximumToTransfer(), flash);
		}
		width -= 240f;
		int num2 = trad.CountHeldBy(Transactor.Colony);
		if (num2 != 0 || trad.IsCurrency)
		{
			Rect rect5 = default(Rect);
			((Rect)(ref rect5))._002Ector(width - 100f, 0f, 100f, ((Rect)(ref rect)).height);
			Text.Anchor = (TextAnchor)3;
			DrawPrice(rect5, trad, TradeAction.PlayerSells);
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref rect5)).x - 75f, 0f, 75f, ((Rect)(ref rect)).height);
			if (Mouse.IsOver(val2))
			{
				Widgets.DrawHighlight(val2);
			}
			Text.Anchor = (TextAnchor)3;
			Rect rect6 = val2;
			((Rect)(ref rect6)).xMin = ((Rect)(ref rect6)).xMin + 5f;
			((Rect)(ref rect6)).xMax = ((Rect)(ref rect6)).xMax - 5f;
			Widgets.Label(rect6, num2.ToStringCached());
			TooltipHandler.TipRegionByKey(val2, "ColonyCount");
		}
		width -= 175f;
		TransferableUIUtility.DoExtraIcons(trad, rect, ref width);
		if (ModsConfig.IdeologyActive)
		{
			TransferableUIUtility.DrawCaptiveTradeInfo(trad, TradeSession.trader, rect, ref width);
		}
		Rect idRect = default(Rect);
		((Rect)(ref idRect))._002Ector(0f, 0f, width, ((Rect)(ref rect)).height);
		TransferableUIUtility.DrawTransferableInfo(trad, idRect, trad.TraderWillTrade ? Color.white : NoTradeColor);
		GenUI.ResetLabelAlign();
		Widgets.EndGroup();
	}

	private static void DrawPrice(Rect rect, Tradeable trad, TradeAction action)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Invalid comparison between Unknown and I4
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Invalid comparison between Unknown and I4
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		if (trad.IsCurrency || !trad.TraderWillTrade)
		{
			return;
		}
		rect = rect.Rounded();
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, new TipSignal(() => trad.GetPriceTooltip(action), trad.GetHashCode() * 297));
		}
		if (action == TradeAction.PlayerBuys)
		{
			switch (trad.PriceTypeFor(action))
			{
			case PriceType.VeryCheap:
				GUI.color = new Color(0f, 1f, 0f);
				break;
			case PriceType.Cheap:
				GUI.color = new Color(0.5f, 1f, 0.5f);
				break;
			case PriceType.Normal:
				GUI.color = Color.white;
				break;
			case PriceType.Expensive:
				GUI.color = new Color(1f, 0.5f, 0.5f);
				break;
			case PriceType.Exorbitant:
				GUI.color = new Color(1f, 0f, 0f);
				break;
			}
		}
		else
		{
			switch (trad.PriceTypeFor(action))
			{
			case PriceType.VeryCheap:
				GUI.color = new Color(1f, 0f, 0f);
				break;
			case PriceType.Cheap:
				GUI.color = new Color(1f, 0.5f, 0.5f);
				break;
			case PriceType.Normal:
				GUI.color = Color.white;
				break;
			case PriceType.Expensive:
				GUI.color = new Color(0.5f, 1f, 0.5f);
				break;
			case PriceType.Exorbitant:
				GUI.color = new Color(0f, 1f, 0f);
				break;
			}
		}
		float priceFor = trad.GetPriceFor(action);
		string label = ((TradeSession.TradeCurrency == TradeCurrency.Silver) ? priceFor.ToStringMoney() : priceFor.ToString());
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 5f;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 5f;
		if ((int)Text.Anchor == 3)
		{
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax + 300f;
		}
		if ((int)Text.Anchor == 5)
		{
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin - 300f;
		}
		Widgets.Label(rect2, label);
		GUI.color = Color.white;
	}

	private static void DrawWillNotTradeText(Rect rect, string text)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).height = ((Rect)(ref rect)).height + 4f;
		rect = rect.Rounded();
		GUI.color = NoTradeColor;
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, text);
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
	}
}
