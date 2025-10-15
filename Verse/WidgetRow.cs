using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class WidgetRow
{
	private float startX;

	private float curX;

	private float curY;

	private float maxWidth = 99999f;

	private float gap;

	private UIDirection growDirection = UIDirection.RightThenUp;

	public const float IconSize = 24f;

	public const float DefaultGap = 4f;

	private const float DefaultMaxWidth = 99999f;

	public const float LabelGap = 2f;

	public const float ButtonExtraSpace = 16f;

	public float FinalX => curX;

	public float FinalY => curY;

	public float CellGap
	{
		get
		{
			return gap;
		}
		set
		{
			gap = value;
		}
	}

	public WidgetRow()
	{
	}

	public WidgetRow(float x, float y, UIDirection growDirection = UIDirection.RightThenUp, float maxWidth = 99999f, float gap = 4f)
	{
		Init(x, y, growDirection, maxWidth, gap);
	}

	public void Init(float x, float y, UIDirection growDirection = UIDirection.RightThenUp, float maxWidth = 99999f, float gap = 4f)
	{
		this.growDirection = growDirection;
		startX = x;
		curX = x;
		curY = y;
		this.maxWidth = maxWidth;
		this.gap = gap;
	}

	private float LeftX(float elementWidth)
	{
		if (growDirection == UIDirection.RightThenUp || growDirection == UIDirection.RightThenDown)
		{
			return curX;
		}
		return curX - elementWidth;
	}

	private void IncrementPosition(float amount)
	{
		if (growDirection == UIDirection.RightThenUp || growDirection == UIDirection.RightThenDown)
		{
			curX += amount;
		}
		else
		{
			curX -= amount;
		}
		if (Mathf.Abs(curX - startX) > maxWidth)
		{
			IncrementY();
		}
	}

	private void IncrementY()
	{
		if (growDirection == UIDirection.RightThenUp || growDirection == UIDirection.LeftThenUp)
		{
			curY -= 24f + gap;
		}
		else
		{
			curY += 24f + gap;
		}
		curX = startX;
	}

	private void IncrementYIfWillExceedMaxWidth(float width)
	{
		if (Mathf.Abs(curX - startX) + Mathf.Abs(width) > maxWidth)
		{
			IncrementY();
		}
	}

	public void Gap(float width)
	{
		if (curX != startX)
		{
			IncrementPosition(width);
		}
	}

	public bool ButtonIcon(Texture2D tex, string tooltip = null, Color? mouseoverColor = null, Color? backgroundColor = null, Color? mouseoverBackgroundColor = null, bool doMouseoverSound = true, float overrideSize = -1f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Rect val = ButtonIconRect(overrideSize);
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(val);
		}
		if (mouseoverBackgroundColor.HasValue && Mouse.IsOver(val))
		{
			Widgets.DrawRectFast(val, mouseoverBackgroundColor.Value);
		}
		else if (backgroundColor.HasValue && !Mouse.IsOver(val))
		{
			Widgets.DrawRectFast(val, backgroundColor.Value);
		}
		bool result = Widgets.ButtonImage(val, tex, Color.white, (Color)(((_003F?)mouseoverColor) ?? GenUI.MouseoverColor));
		IncrementPosition((overrideSize > 0f) ? overrideSize : 24f);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		return result;
	}

	public Rect ButtonIconRect(float overrideSize = -1f)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = ((overrideSize > 0f) ? overrideSize : 24f);
		float num2 = (24f - num) / 2f;
		IncrementYIfWillExceedMaxWidth(num);
		return new Rect(LeftX(num) + num2, curY + num2, num, num);
	}

	public bool ButtonIconWithBG(Texture2D texture, float width = -1f, string tooltip = null, bool doMouseoverSound = true)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (width < 0f)
		{
			width = 24f;
		}
		width += 16f;
		IncrementYIfWillExceedMaxWidth(width);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(width), curY, width, 26f);
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(val);
		}
		bool result = Widgets.ButtonImageWithBG(val, texture, Vector2.one * 24f);
		IncrementPosition(width + gap);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		return result;
	}

	public void ToggleableIcon(ref bool toggleable, Texture2D tex, string tooltip, SoundDef mouseoverSound = null, string tutorTag = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		IncrementYIfWillExceedMaxWidth(24f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(24f), curY, 24f, 24f);
		bool num = Widgets.ButtonImage(val, tex);
		IncrementPosition(24f + gap);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		Rect val2 = new Rect(((Rect)(ref val)).x + ((Rect)(ref val)).width / 2f, ((Rect)(ref val)).y, ((Rect)(ref val)).height / 2f, ((Rect)(ref val)).height / 2f);
		Texture2D val3 = (toggleable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
		GUI.DrawTexture(val2, (Texture)(object)val3);
		if (mouseoverSound != null)
		{
			MouseoverSounds.DoRegion(val, mouseoverSound);
		}
		if (num)
		{
			toggleable = !toggleable;
			if (toggleable)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}
		if (tutorTag != null)
		{
			UIHighlighter.HighlightOpportunity(val, tutorTag);
		}
	}

	public Rect Icon(Texture tex, string tooltip = null, float contractedBy = 0f)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		IncrementYIfWillExceedMaxWidth(24f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(24f), curY, 24f, 24f);
		val = val.ContractedBy(contractedBy);
		GUI.DrawTexture(val, tex);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		IncrementPosition(24f + gap);
		return val;
	}

	public Rect DefIcon(ThingDef def, string tooltip = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		IncrementYIfWillExceedMaxWidth(24f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(24f), curY, 24f, 24f);
		Widgets.DefIcon(val, def);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		IncrementPosition(24f + gap);
		return val;
	}

	public bool ButtonText(string label, string tooltip = null, bool drawBackground = true, bool doMouseoverSound = true, bool active = true, float? fixedWidth = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = ButtonRect(label, fixedWidth);
		bool result = Widgets.ButtonText(rect, label, drawBackground, doMouseoverSound, active);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
		return result;
	}

	public Rect ButtonRect(string label, float? fixedWidth = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = (Vector2)(fixedWidth.HasValue ? new Vector2(fixedWidth.Value, 24f) : Text.CalcSize(label));
		val.x += 16f;
		val.y += 2f;
		IncrementYIfWillExceedMaxWidth(val.x);
		Rect result = default(Rect);
		((Rect)(ref result))._002Ector(LeftX(val.x), curY, val.x, val.y);
		IncrementPosition(((Rect)(ref result)).width + gap);
		return result;
	}

	public Rect Label(string text, float width = -1f, string tooltip = null, float height = -1f)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (height < 0f)
		{
			height = 24f;
		}
		if (width < 0f)
		{
			width = Text.CalcSize(text).x;
		}
		IncrementYIfWillExceedMaxWidth(width + 2f);
		IncrementPosition(2f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(width), curY, width, height);
		Widgets.Label(val, text);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		IncrementPosition(2f);
		IncrementPosition(((Rect)(ref val)).width);
		return val;
	}

	public Rect LabelEllipses(string text, float width, string tooltip = null, float height = -1f)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (height < 0f)
		{
			height = 24f;
		}
		IncrementYIfWillExceedMaxWidth(width + 2f);
		IncrementPosition(2f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(width), curY, width, height);
		Widgets.LabelEllipses(val, text);
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, tooltip);
		}
		IncrementPosition(2f);
		IncrementPosition(((Rect)(ref val)).width);
		return val;
	}

	public Rect TextFieldNumeric<T>(ref int val, ref string buffer, float width = -1f) where T : struct
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (width < 0f)
		{
			width = Text.CalcSize(val.ToString()).x;
		}
		IncrementYIfWillExceedMaxWidth(width + 2f);
		IncrementPosition(2f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(LeftX(width), curY, width, 24f);
		Widgets.TextFieldNumeric(val2, ref val, ref buffer);
		IncrementPosition(2f);
		IncrementPosition(((Rect)(ref val2)).width);
		return val2;
	}

	public Rect FillableBar(float width, float height, float fillPct, string label, Texture2D fillTex, Texture2D bgTex = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		IncrementYIfWillExceedMaxWidth(width);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(LeftX(width), curY, width, height);
		Widgets.FillableBar(val, fillPct, fillTex, bgTex, doBorder: false);
		if (!label.NullOrEmpty())
		{
			Rect rect = val;
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 2f;
			((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 2f;
			if (!Text.TinyFontSupported)
			{
				((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 2f;
			}
			if ((int)Text.Anchor >= 0)
			{
				((Rect)(ref rect)).height = ((Rect)(ref rect)).height + 14f;
			}
			using (new TextBlock((GameFont?)GameFont.Tiny, (TextAnchor?)null, (bool?)false))
			{
				Widgets.Label(rect, label);
			}
		}
		IncrementPosition(width);
		return val;
	}
}
