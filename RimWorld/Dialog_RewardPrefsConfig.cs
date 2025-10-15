using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_RewardPrefsConfig : Window
{
	private Vector2 scrollPosition;

	private float viewRectHeight;

	private const float TitleHeight = 40f;

	private const float RowHeight = 45f;

	private const float IconSize = 35f;

	private const float GoodwillWidth = 100f;

	private const float CheckboxOffset = 150f;

	private const float FactionNameWidth = 250f;

	public override Vector2 InitialSize => new Vector2(700f, 440f);

	public Dialog_RewardPrefsConfig()
	{
		forcePause = true;
		doCloseX = true;
		doCloseButton = true;
		absorbInputAroundWindow = true;
		closeOnClickedOutside = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Invalid comparison between Unknown and I4
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, InitialSize.x / 2f, 40f), "ChooseRewards".Translate());
		Text.Font = GameFont.Small;
		string text = "ChooseRewardsDesc".Translate();
		float num = Text.CalcHeight(text, ((Rect)(ref inRect)).width);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 40f, ((Rect)(ref inRect)).width, num);
		Widgets.Label(rect, text);
		IEnumerable<Faction> allFactionsVisibleInViewOrder = Find.FactionManager.AllFactionsVisibleInViewOrder;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(inRect);
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - Window.CloseButSize.y;
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + (44f + ((Rect)(ref rect)).height + 4f);
		float curY = 0f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref outRect)).width - 16f, viewRectHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		int index = 0;
		Rect rect2 = default(Rect);
		Rect rect3 = default(Rect);
		foreach (Faction item in allFactionsVisibleInViewOrder)
		{
			if (item.IsPlayer)
			{
				continue;
			}
			float curX = 0f;
			if (item.def.HasRoyalTitles)
			{
				DoFactionInfo(val, item, ref curX, ref curY, ref index);
				TaggedString label = "AcceptRoyalFavor".Translate(item.Named("FACTION")).CapitalizeFirst();
				((Rect)(ref rect2))._002Ector(curX, curY, label.GetWidthCached(), 45f);
				Text.Anchor = (TextAnchor)3;
				Widgets.Label(rect2, label);
				Text.Anchor = (TextAnchor)0;
				if (Mouse.IsOver(rect2))
				{
					TooltipHandler.TipRegion(rect2, "AcceptRoyalFavorDesc".Translate(item.Named("FACTION")));
					Widgets.DrawHighlight(rect2);
				}
				Widgets.Checkbox(((Rect)(ref val)).width - 150f, curY + 12f, ref item.allowRoyalFavorRewards, 24f, disabled: false, paintable: true);
				curY += 45f;
			}
			if (item.CanEverGiveGoodwillRewards)
			{
				curX = 0f;
				DoFactionInfo(val, item, ref curX, ref curY, ref index);
				TaggedString label2 = "AcceptGoodwill".Translate().CapitalizeFirst();
				((Rect)(ref rect3))._002Ector(curX, curY, label2.GetWidthCached(), 45f);
				Text.Anchor = (TextAnchor)3;
				Widgets.Label(rect3, label2);
				Text.Anchor = (TextAnchor)0;
				if (Mouse.IsOver(rect3))
				{
					TooltipHandler.TipRegion(rect3, "AcceptGoodwillDesc".Translate(item.Named("FACTION")));
					Widgets.DrawHighlight(rect3);
				}
				Widgets.Checkbox(((Rect)(ref val)).width - 150f, curY + 12f, ref item.allowGoodwillRewards, 24f, disabled: false, paintable: true);
				Widgets.Label(new Rect(((Rect)(ref val)).width - 100f, curY, 100f, 35f), (item.PlayerGoodwill.ToStringWithSign() + "\n" + item.PlayerRelationKind.GetLabelCap()).Colorize(item.PlayerRelationKind.GetColor()));
				curY += 45f;
			}
		}
		if ((int)Event.current.type == 8)
		{
			viewRectHeight = curY;
		}
		Widgets.EndScrollView();
	}

	private void DoFactionInfo(Rect rect, Faction faction, ref float curX, ref float curY, ref int index)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(new Rect(curX, curY, ((Rect)(ref rect)).width, 45f));
		}
		FactionUIUtility.DrawFactionIconWithTooltip(new Rect(curX, curY + 5f, 35f, 35f), faction);
		curX += 45f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(curX, curY, 250f, 45f);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, faction.Name);
		Text.Anchor = (TextAnchor)0;
		curX += 250f;
		if (Mouse.IsOver(rect2))
		{
			TooltipHandler.TipRegion(tip: new TipSignal(() => faction.Name + "\n\n" + faction.def.Description + "\n\n" + faction.PlayerRelationKind.GetLabelCap().Colorize(faction.PlayerRelationKind.GetColor()), faction.loadID ^ 0x4468077), rect: rect2);
			Widgets.DrawHighlight(rect2);
		}
		index++;
	}
}
