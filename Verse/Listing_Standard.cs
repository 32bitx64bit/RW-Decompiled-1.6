using System;
using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

[StaticConstructorOnStartup]
public class Listing_Standard : Listing
{
	private GameFont font;

	private Rect? boundingRect;

	private Func<Vector2> boundingScrollPositionGetter;

	private List<Pair<Vector2, Vector2>> labelScrollbarPositions;

	private List<Vector2> labelScrollbarPositionsSetThisFrame;

	private int boundingRectCachedForFrame = -1;

	private Rect? boundingRectCached;

	private static readonly Texture2D PinTex = ContentFinder<Texture2D>.Get("UI/Icons/Pin");

	private static readonly Texture2D PinOutlineTex = ContentFinder<Texture2D>.Get("UI/Icons/Pin-Outline");

	public const float PinnableActionHeight = 22f;

	private const float DefSelectionLineHeight = 21f;

	public Rect? BoundingRectCached
	{
		get
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (boundingRectCachedForFrame != Time.frameCount)
			{
				if (boundingRect.HasValue && boundingScrollPositionGetter != null)
				{
					Rect value = boundingRect.Value;
					Vector2 val = boundingScrollPositionGetter();
					((Rect)(ref value)).x = ((Rect)(ref value)).x + val.x;
					((Rect)(ref value)).y = ((Rect)(ref value)).y + val.y;
					boundingRectCached = value;
				}
				boundingRectCachedForFrame = Time.frameCount;
			}
			return boundingRectCached;
		}
	}

	public Listing_Standard(GameFont font)
	{
		this.font = font;
	}

	public Listing_Standard()
	{
		font = GameFont.Small;
	}

	public Listing_Standard(Rect boundingRect, Func<Vector2> boundingScrollPositionGetter)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		font = GameFont.Small;
		this.boundingRect = boundingRect;
		this.boundingScrollPositionGetter = boundingScrollPositionGetter;
	}

	public override void Begin(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.Begin(rect);
		Text.Font = font;
	}

	public override void End()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		base.End();
		if (labelScrollbarPositions == null)
		{
			return;
		}
		for (int num = labelScrollbarPositions.Count - 1; num >= 0; num--)
		{
			if (!labelScrollbarPositionsSetThisFrame.Contains(labelScrollbarPositions[num].First))
			{
				labelScrollbarPositions.RemoveAt(num);
			}
		}
		labelScrollbarPositionsSetThisFrame.Clear();
	}

	public Rect Label(TaggedString label, float maxHeight = -1f, string tooltip = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Label(label.Resolve(), maxHeight, (TipSignal?)(TipSignal)tooltip);
	}

	public Rect Label(string label, float maxHeight = -1f, TipSignal? tipSignal = null)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		float num = Text.CalcHeight(label, base.ColumnWidth);
		bool flag = false;
		if (maxHeight >= 0f && num > maxHeight)
		{
			num = maxHeight;
			flag = true;
		}
		Rect rect = GetRect(num);
		if (BoundingRectCached.HasValue && !((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			return rect;
		}
		if (flag)
		{
			Vector2 scrollbarPosition = GetLabelScrollbarPosition(curX, curY);
			Widgets.LabelScrollable(rect, label, ref scrollbarPosition);
			SetLabelScrollbarPosition(curX, curY, scrollbarPosition);
		}
		else
		{
			Widgets.Label(rect, label);
		}
		if (tipSignal.HasValue)
		{
			TooltipHandler.TipRegion(rect, tipSignal.Value);
		}
		Gap(verticalSpacing);
		return rect;
	}

	public void LabelDouble(string leftLabel, string rightLabel, string tip = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		float num = base.ColumnWidth / 2f;
		float width = base.ColumnWidth - num;
		float num2 = Text.CalcHeight(leftLabel, num);
		float num3 = Text.CalcHeight(rightLabel, width);
		float height = Mathf.Max(num2, num3);
		Rect rect = GetRect(height);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			if (!tip.NullOrEmpty())
			{
				Widgets.DrawHighlightIfMouseover(rect);
				TooltipHandler.TipRegion(rect, tip);
			}
			Widgets.Label(rect.LeftHalf(), leftLabel);
			Widgets.Label(rect.RightHalf(), rightLabel);
			Gap(verticalSpacing);
		}
	}

	public Rect SubLabel(string label, float widthPct)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		float height = Text.CalcHeight(label, base.ColumnWidth * widthPct);
		Rect rect = GetRect(height, widthPct);
		float num = 20f;
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + num;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - num;
		Text.Font = GameFont.Tiny;
		GUI.color = Color.gray;
		Widgets.Label(rect, label);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Gap(verticalSpacing);
		return rect;
	}

	public bool RadioButton(string label, bool active, float tabIn = 0f, string tooltip = null, float? tooltipDelay = null)
	{
		return RadioButton(label, active, tabIn, tooltip, tooltipDelay, disabled: false);
	}

	public bool RadioButton(string label, bool active, float tabIn, string tooltip, float? tooltipDelay, bool disabled)
	{
		return RadioButton(label, active, tabIn, 0f, tooltip, tooltipDelay, disabled);
	}

	public bool RadioButton(string label, bool active, float tabIn, float tabInRight, string tooltip, float? tooltipDelay, bool disabled)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = GetRect(lineHeight);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + tabIn;
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - tabInRight;
		if (BoundingRectCached.HasValue && !((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			return false;
		}
		if (!tooltip.NullOrEmpty())
		{
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			TipSignal tip = (tooltipDelay.HasValue ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip));
			TooltipHandler.TipRegion(rect, tip);
		}
		bool result = Widgets.RadioButtonLabeled(rect, label, active, disabled);
		Gap(verticalSpacing);
		return result;
	}

	public void CheckboxLabeled(string label, ref bool checkOn, float tabIn)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float height = Text.CalcHeight(label, base.ColumnWidth);
		Rect rect = GetRect(height);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + tabIn;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.CheckboxLabeled(rect, label, ref checkOn);
			Gap(verticalSpacing);
		}
	}

	public void CheckboxLabeled(string label, ref bool checkOn, string tooltip = null, float height = 0f, float labelPct = 1f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		float height2 = ((height != 0f) ? height : Text.CalcHeight(label, base.ColumnWidth * labelPct));
		Rect rect = GetRect(height2, labelPct);
		((Rect)(ref rect)).width = Math.Min(((Rect)(ref rect)).width + 24f, base.ColumnWidth);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			if (!tooltip.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Widgets.CheckboxLabeled(rect, label, ref checkOn);
		}
		Gap(verticalSpacing);
	}

	public bool CheckboxLabeledSelectable(string label, ref bool selected, ref bool checkOn)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = GetRect(lineHeight);
		bool result = false;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			result = Widgets.CheckboxLabeledSelectable(rect, label, ref selected, ref checkOn);
		}
		Gap(verticalSpacing);
		return result;
	}

	public bool ButtonText(string label, string highlightTag = null, float widthPct = 1f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(30f, widthPct);
		bool result = false;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			result = Widgets.ButtonText(rect, label);
			if (highlightTag != null)
			{
				UIHighlighter.HighlightOpportunity(rect, highlightTag);
			}
		}
		Gap(verticalSpacing);
		return result;
	}

	public bool ButtonTextLabeled(string label, string buttonLabel, TextAnchor anchor = 0, string highlightTag = null, string tooltip = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ButtonTextLabeledPct(label, buttonLabel, 0.5f, anchor, highlightTag, tooltip);
	}

	public bool ButtonTextLabeledPct(string label, string buttonLabel, float labelPct, TextAnchor anchor = 0, string highlightTag = null, string tooltip = null, Texture2D labelIcon = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		float height = Math.Max(Text.CalcHeight(label, base.ColumnWidth * labelPct), 30f);
		Rect rect = GetRect(height);
		Rect rect2 = rect.RightPart(1f - labelPct);
		((Rect)(ref rect2)).height = 30f;
		if (highlightTag != null)
		{
			UIHighlighter.HighlightOpportunity(rect, highlightTag);
		}
		bool result = false;
		Rect rect3 = rect.LeftPart(labelPct);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Text.Anchor = anchor;
			Widgets.Label(rect3, label);
			result = Widgets.ButtonText(rect2, buttonLabel.Truncate(((Rect)(ref rect2)).width - 20f));
			Text.Anchor = (TextAnchor)0;
		}
		if ((Object)(object)labelIcon != (Object)null)
		{
			GUI.DrawTexture(new Rect(Text.CalcSize(label).x + 10f, ((Rect)(ref rect3)).y + (((Rect)(ref rect3)).height - Text.LineHeight) / 2f, Text.LineHeight, Text.LineHeight), (Texture)(object)labelIcon);
		}
		if (!tooltip.NullOrEmpty())
		{
			if (Mouse.IsOver(rect3))
			{
				Widgets.DrawHighlight(rect3);
			}
			TooltipHandler.TipRegion(rect3, tooltip);
		}
		Gap(verticalSpacing);
		return result;
	}

	public bool ButtonImage(Texture2D tex, float width, float height)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NewColumnIfNeeded(height);
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(curX, curY, width, height);
		bool result = false;
		if (!BoundingRectCached.HasValue || ((Rect)(ref butRect)).Overlaps(BoundingRectCached.Value))
		{
			result = Widgets.ButtonImage(butRect, tex);
		}
		Gap(height + verticalSpacing);
		return result;
	}

	public void None()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.gray;
		Text.Anchor = (TextAnchor)1;
		Label("NoneBrackets".Translate());
		GenUI.ResetLabelAlign();
		GUI.color = Color.white;
	}

	public string TextEntry(string text, int lineCount = 1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight * (float)lineCount);
		string result = ((lineCount != 1) ? Widgets.TextArea(rect, text) : Widgets.TextField(rect, text));
		Gap(verticalSpacing);
		return result;
	}

	public string TextEntryLabeled(string label, string text, int lineCount = 1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		string result = Widgets.TextEntryLabeled(GetRect(Text.LineHeight * (float)lineCount), label, text);
		Gap(verticalSpacing);
		return result;
	}

	public void TextFieldNumeric<T>(ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.TextFieldNumeric(rect, ref val, ref buffer, min, max);
		}
		Gap(verticalSpacing);
	}

	public void TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(Text.LineHeight);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.TextFieldNumericLabeled(rect, label, ref val, ref buffer, min, max);
		}
		Gap(verticalSpacing);
	}

	public Rect IntRange(ref IntRange range, int min, int max)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(32f);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.IntRange(rect, (int)base.CurHeight, ref range, min, max);
		}
		Gap(verticalSpacing);
		return rect;
	}

	public float Slider(float val, float min, float max)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		float result = Widgets.HorizontalSlider(GetRect(22f), val, min, max);
		Gap(verticalSpacing);
		return result;
	}

	public float SliderLabeled(string label, float val, float min, float max, float labelPct = 0.5f, string tooltip = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(30f);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect.LeftPart(labelPct), label);
		if (tooltip != null)
		{
			TooltipHandler.TipRegion(rect.LeftPart(labelPct), tooltip);
		}
		Text.Anchor = (TextAnchor)0;
		float result = Widgets.HorizontalSlider(rect.RightPart(1f - labelPct), val, min, max, middleAlignment: true);
		Gap(verticalSpacing);
		return result;
	}

	public void IntAdjuster(ref int val, int countChange, int min = 0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(24f);
		((Rect)(ref rect)).width = 42f;
		if (Widgets.ButtonText(rect, "-" + countChange))
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			val -= countChange * GenUI.CurrentAdjustmentMultiplier();
			if (val < min)
			{
				val = min;
			}
		}
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + (((Rect)(ref rect)).width + 2f);
		if (Widgets.ButtonText(rect, "+" + countChange))
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			val += countChange * GenUI.CurrentAdjustmentMultiplier();
			if (val < min)
			{
				val = min;
			}
		}
		Gap(verticalSpacing);
	}

	public void IntSetter(ref int val, int target, string label, float width = 42f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(GetRect(24f), label))
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			val = target;
		}
		Gap(verticalSpacing);
	}

	public void IntEntry(ref int val, ref string editBuffer, int multiplier = 1, int min = 0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(24f);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.IntEntry(rect, ref val, ref editBuffer, multiplier);
			if (val < min)
			{
				val = min;
				editBuffer = val.ToString();
			}
		}
		Gap(verticalSpacing);
	}

	public Listing_Standard BeginSection(float height, float sectionBorder = 4f, float bottomBorder = 4f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GetRect(height + sectionBorder + bottomBorder);
		Widgets.DrawMenuSection(rect);
		Listing_Standard listing_Standard = new Listing_Standard();
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + sectionBorder, ((Rect)(ref rect)).y + sectionBorder, ((Rect)(ref rect)).width - sectionBorder * 2f, ((Rect)(ref rect)).height - (sectionBorder + bottomBorder));
		listing_Standard.Begin(rect2);
		return listing_Standard;
	}

	public void EndSection(Listing_Standard listing)
	{
		listing.End();
	}

	private Vector2 GetLabelScrollbarPosition(float x, float y)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (labelScrollbarPositions == null)
		{
			return Vector2.zero;
		}
		for (int i = 0; i < labelScrollbarPositions.Count; i++)
		{
			Vector2 first = labelScrollbarPositions[i].First;
			if (first.x == x && first.y == y)
			{
				return labelScrollbarPositions[i].Second;
			}
		}
		return Vector2.zero;
	}

	private void SetLabelScrollbarPosition(float x, float y, Vector2 scrollbarPosition)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (labelScrollbarPositions == null)
		{
			labelScrollbarPositions = new List<Pair<Vector2, Vector2>>();
			labelScrollbarPositionsSetThisFrame = new List<Vector2>();
		}
		labelScrollbarPositionsSetThisFrame.Add(new Vector2(x, y));
		for (int i = 0; i < labelScrollbarPositions.Count; i++)
		{
			Vector2 first = labelScrollbarPositions[i].First;
			if (first.x == x && first.y == y)
			{
				labelScrollbarPositions[i] = new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition);
				return;
			}
		}
		labelScrollbarPositions.Add(new Pair<Vector2, Vector2>(new Vector2(x, y), scrollbarPosition));
	}

	public bool SelectableDef(string name, bool selected, Action deleteCallback)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		float num = ((Rect)(ref listingRect)).width - 21f;
		Text.Anchor = (TextAnchor)3;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, num, 21f);
		if (selected)
		{
			Widgets.DrawHighlight(val);
		}
		if (Mouse.IsOver(val))
		{
			Widgets.DrawBox(val);
		}
		Text.WordWrap = false;
		Widgets.Label(val, name);
		Text.WordWrap = true;
		if (deleteCallback != null && Widgets.ButtonImage(new Rect(((Rect)(ref val)).xMax, ((Rect)(ref val)).y, 21f, 21f), TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
		{
			deleteCallback();
		}
		Text.Anchor = (TextAnchor)0;
		curY += 21f;
		return Widgets.ButtonInvisible(val);
	}

	public void LabelCheckboxDebug(string label, ref bool checkOn, bool highlight)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, base.ColumnWidth, 22f);
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.CheckboxLabeled(rect, label.Truncate(((Rect)(ref rect)).width - 15f), ref checkOn);
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
		}
		Gap(22f + verticalSpacing);
	}

	public bool ButtonDebug(string label, bool highlight)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, base.ColumnWidth, 22f);
		bool result = false;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			bool wordWrap = Text.WordWrap;
			Text.WordWrap = false;
			result = Widgets.ButtonText(rect, "  " + label, drawBackground: true, doMouseoverSound: true, active: true, (TextAnchor)3);
			Text.WordWrap = wordWrap;
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
		}
		Gap(22f + verticalSpacing);
		return result;
	}

	public DebugActionButtonResult ButtonDebugPinnable(string label, bool highlight, bool pinned)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, base.ColumnWidth - 22f, 22f);
		DebugActionButtonResult result = DebugActionButtonResult.None;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			bool wordWrap = Text.WordWrap;
			Text.WordWrap = false;
			if (Widgets.ButtonText(rect, "  " + label, drawBackground: true, doMouseoverSound: true, active: true, (TextAnchor)3))
			{
				result = DebugActionButtonResult.ButtonPressed;
			}
			Text.WordWrap = wordWrap;
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
			Rect val = GenUI.ContractedBy(new Rect(((Rect)(ref rect)).xMax + 2f, ((Rect)(ref rect)).y, 22f, 22f), 4f);
			GUI.color = (Color)(pinned ? Color.white : new Color(1f, 1f, 1f, 0.2f));
			GUI.DrawTexture(val, (Texture)(object)(pinned ? PinTex : PinOutlineTex));
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(val))
			{
				result = DebugActionButtonResult.PinPressed;
			}
			Widgets.DrawHighlightIfMouseover(val);
		}
		Gap(22f + verticalSpacing);
		return result;
	}

	public DebugActionButtonResult CheckboxPinnable(string label, ref bool checkOn, bool highlight, bool pinned)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, base.ColumnWidth - 22f, 22f);
		DebugActionButtonResult result = DebugActionButtonResult.None;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			Widgets.CheckboxLabeled(rect, label.Truncate(((Rect)(ref rect)).width - 24f - 15f), ref checkOn);
			if (highlight)
			{
				GUI.color = Color.yellow;
				Widgets.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, label);
			}
			Rect val = GenUI.ContractedBy(new Rect(((Rect)(ref rect)).xMax + 2f, ((Rect)(ref rect)).y, 22f, 22f), 4f);
			GUI.color = (Color)(pinned ? Color.white : new Color(1f, 1f, 1f, 0.2f));
			GUI.DrawTexture(val, (Texture)(object)(pinned ? PinTex : PinOutlineTex));
			GUI.color = Color.white;
			if (Widgets.ButtonInvisible(val))
			{
				result = DebugActionButtonResult.PinPressed;
			}
			Widgets.DrawHighlightIfMouseover(val);
		}
		Gap(22f + verticalSpacing);
		return result;
	}
}
