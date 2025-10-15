using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_Label : PawnColumnWorker
{
	private const int LeftMargin = 3;

	private const float PortraitCameraZoom = 1.2f;

	private static Dictionary<string, TaggedString> labelCache = new Dictionary<string, TaggedString>();

	private static float labelCacheForWidth = -1f;

	protected virtual TextAnchor LabelAlignment => (TextAnchor)3;

	protected override TextAnchor DefaultHeaderAlignment => (TextAnchor)6;

	protected override float GetHeaderOffsetX(Rect rect)
	{
		return 33f;
	}

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, Mathf.Min(((Rect)(ref rect)).height, def.groupable ? ((Rect)(ref rect)).height : ((float)GetMinCellHeight(pawn))));
		Rect rect2 = val;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 3f;
		if (def.showIcon)
		{
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + ((Rect)(ref val)).height;
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).height, ((Rect)(ref val)).height);
			if (Find.Selector.IsSelected(pawn))
			{
				SelectionDrawerUtility.DrawSelectionOverlayWholeGUI(rect3.ContractedBy(2f));
			}
			Widgets.ThingIcon(rect3, pawn);
		}
		if (pawn.health.summaryHealth.SummaryHealthPercent < 0.99f)
		{
			Rect rect4 = default(Rect);
			((Rect)(ref rect4))._002Ector(((Rect)(ref rect2)).x - 3f, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width + 3f, ((Rect)(ref rect2)).height);
			((Rect)(ref rect4)).yMin = ((Rect)(ref rect4)).yMin + 4f;
			((Rect)(ref rect4)).yMax = ((Rect)(ref rect4)).yMax - 6f;
			Widgets.FillableBar(rect4, pawn.health.summaryHealth.SummaryHealthPercent, GenMapUI.OverlayHealthTex, BaseContent.ClearTex, doBorder: false);
		}
		if (Mouse.IsOver(val))
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		TaggedString taggedString = GetLabel(pawn);
		if (((Rect)(ref rect2)).width != labelCacheForWidth)
		{
			labelCacheForWidth = ((Rect)(ref rect2)).width;
			labelCache.Clear();
		}
		if (Text.CalcSize(taggedString).x > ((Rect)(ref rect2)).width)
		{
			taggedString = taggedString.Truncate(((Rect)(ref rect2)).width, labelCache);
		}
		if (pawn.IsSlave || pawn.IsColonyMech)
		{
			taggedString = taggedString.Colorize(PawnNameColorUtility.PawnNameColorOf(pawn));
		}
		Text.Font = GameFont.Small;
		Text.Anchor = LabelAlignment;
		Text.WordWrap = false;
		Widgets.Label(rect2, taggedString);
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		if (Widgets.ButtonInvisible(val))
		{
			CameraJumper.TryJumpAndSelect(pawn);
			if (Current.ProgramState == ProgramState.Playing && Event.current.button == 0)
			{
				Find.MainTabsRoot.EscapeCurrentTab(playSound: false);
			}
		}
		else if (Mouse.IsOver(val))
		{
			TipSignal tooltip = pawn.GetTooltip();
			tooltip.text = "ClickToJumpTo".Translate() + "\n\n" + tooltip.text;
			TooltipHandler.TipRegion(val, tooltip);
		}
	}

	private TaggedString GetLabel(Pawn pawn)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!pawn.RaceProps.Humanlike && !pawn.RaceProps.Animal && pawn.Name != null && !pawn.Name.Numerical)
		{
			return pawn.Name.ToStringShort.CapitalizeFirst() + ", " + pawn.KindLabel.Colorize(ColoredText.SubtleGrayColor);
		}
		if (def.useLabelShort)
		{
			return pawn.LabelShortCap;
		}
		return pawn.LabelNoCount.CapitalizeFirst();
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), 80);
	}

	public override int GetOptimalWidth(PawnTable table)
	{
		return Mathf.Clamp(165, GetMinWidth(table), GetMaxWidth(table));
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return string.Compare(GetValueToCompare(a), GetValueToCompare(b), StringComparison.Ordinal);
	}

	private string GetValueToCompare(Pawn pawn)
	{
		return GetLabel(pawn);
	}
}
