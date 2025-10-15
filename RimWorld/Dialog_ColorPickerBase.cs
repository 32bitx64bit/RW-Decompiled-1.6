using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_ColorPickerBase : Window
{
	private const int ContextHash = 195906069;

	private static readonly List<string> focusableControlNames = new List<string> { "title", "colorTextfields_0", "colorTextfields_1", "colorTextfields_2", "colorTextfields_3", "colorTextfields_4" };

	private const int ColorWheelSize = 128;

	private const int ColorTextfieldsWidth = 125;

	private const int PaletteColumns = 9;

	private const int ColorIconSize = 22;

	private const int ColorIconPadding = 2;

	private const int CurrentColorLabelWidth = 100;

	private const int PaletteWidth = 250;

	private const int ColorTemperatureBarHeight = 34;

	protected Color color;

	protected Color oldColor;

	private bool hsvColorWheelDragging;

	private bool colorTemperatureDragging;

	private string[] textfieldBuffers = new string[6];

	private Color textfieldColorBuffer;

	private string previousFocusedControlName;

	private Widgets.ColorComponents visibleTextfields;

	private Widgets.ColorComponents editableTextfields;

	protected static readonly Vector2 ButSize = new Vector2(150f, 38f);

	public override Vector2 InitialSize => new Vector2(600f, 450f);

	protected abstract bool ShowDarklight { get; }

	protected abstract Color DefaultColor { get; }

	protected abstract List<Color> PickableColors { get; }

	protected abstract float ForcedColorValue { get; }

	protected abstract bool ShowColorTemperatureBar { get; }

	protected abstract void SaveColor(Color color);

	public Dialog_ColorPickerBase(Widgets.ColorComponents visibleTextfields, Widgets.ColorComponents editableTextfields)
	{
		this.visibleTextfields = visibleTextfields;
		this.editableTextfields = editableTextfields;
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnClickedOutside = true;
		closeOnAccept = false;
	}

	private static void HeaderRow(ref RectDivider layout)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock(GameFont.Medium))
		{
			TaggedString taggedString = "ChooseAColor".Translate().CapitalizeFirst();
			string text = taggedString;
			Rect rect = layout.Rect;
			RectDivider rectDivider = layout.NewRow(Text.CalcHeight(text, ((Rect)(ref rect)).width));
			GUI.SetNextControlName(focusableControlNames[0]);
			Rect rect2 = rectDivider.Rect;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - 5f;
			Widgets.Label(rect2, taggedString);
		}
	}

	private void BottomButtons(ref RectDivider layout)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		RectDivider rectDivider = layout.NewRow(ButSize.y, VerticalJustification.Bottom);
		if (Widgets.ButtonText(rectDivider.NewCol(ButSize.x), "Cancel".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(rectDivider.NewCol(ButSize.x, HorizontalJustification.Right), "Accept".Translate()))
		{
			SaveColor(color);
			Close();
		}
	}

	private void ColorPalette(ref RectDivider layout, ref Color color, Color defaultColor, bool showDarklight, out float paletteHeight)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock((TextAnchor)3))
		{
			RectDivider rectDivider = layout;
			RectDivider rectDivider2 = rectDivider.NewCol(250f, HorizontalJustification.Right);
			int num = 26;
			RectDivider rectDivider3 = rectDivider2.NewRow(num);
			int num2 = 4;
			rectDivider3.Rect.SplitVertically(num2 * (num + 2), out var left, out var right);
			RectDivider rectDivider4 = new RectDivider(left, 195906069, (Vector2?)new Vector2(10f, 2f));
			Widgets.ColorBox(rectDivider4.NewCol(num), ref color, defaultColor);
			Widgets.Label(rectDivider4, "Default".Translate().CapitalizeFirst());
			RectDivider rectDivider5 = new RectDivider(right, 195906069, (Vector2?)new Vector2(10f, 2f));
			Color defaultDarklight = DarklightUtility.DefaultDarklight;
			Rect rect = rectDivider5.NewCol(num);
			if (showDarklight)
			{
				Widgets.ColorBox(rect, ref color, defaultDarklight);
				Widgets.Label(rectDivider5, "Darklight".Translate().CapitalizeFirst());
			}
			Widgets.ColorSelector(rectDivider2, ref color, PickableColors, out paletteHeight);
			paletteHeight += num + 2;
		}
	}

	private void ColorTextfields(ref RectDivider layout, out Vector2 size)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = layout.Rect;
		RectAggregator aggregator = new RectAggregator(new Rect(((Rect)(ref rect)).position, new Vector2(125f, 0f)), 195906069);
		bool num = Widgets.ColorTextfields(ref aggregator, ref color, ref textfieldBuffers, ref textfieldColorBuffer, previousFocusedControlName, "colorTextfields", editableTextfields, visibleTextfields);
		rect = aggregator.Rect;
		size = ((Rect)(ref rect)).size;
		if (num)
		{
			float num2 = default(float);
			float num3 = default(float);
			float num4 = default(float);
			Color.RGBToHSV(color, ref num2, ref num3, ref num4);
			color = Color.HSVToRGB(num2, num3, ForcedColorValue);
		}
	}

	private static void ColorReadback(Rect rect, Color color, Color oldColor, bool showDarklight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		rect.SplitVertically((((Rect)(ref rect)).width - 26f) / 2f, out var left, out var right);
		RectDivider rectDivider = new RectDivider(left, 195906069);
		TaggedString label = "CurrentColor".Translate().CapitalizeFirst();
		TaggedString label2 = "OldColor".Translate().CapitalizeFirst();
		float width = Mathf.Max(new float[3]
		{
			100f,
			label.GetWidthCached(),
			label2.GetWidthCached()
		});
		RectDivider rectDivider2 = rectDivider.NewRow(Text.LineHeight);
		Widgets.Label(rectDivider2.NewCol(width), label);
		Widgets.DrawBoxSolid(rectDivider2, color);
		RectDivider rectDivider3 = rectDivider.NewRow(Text.LineHeight);
		Widgets.Label(rectDivider3.NewCol(width), label2);
		Widgets.DrawBoxSolid(rectDivider3, oldColor);
		RectDivider rectDivider4 = new RectDivider(right, 195906069);
		rectDivider4.NewCol(26f);
		if (showDarklight)
		{
			if (DarklightUtility.IsDarklight(color))
			{
				Widgets.Label(rectDivider4, "Darklight".Translate().CapitalizeFirst());
			}
			else
			{
				Widgets.Label(rectDivider4, "NotDarklight".Translate().CapitalizeFirst());
			}
		}
	}

	private static void TabControl()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 4 && (int)Event.current.keyCode == 9)
		{
			bool num = !Event.current.shift;
			Event.current.Use();
			string text = GUI.GetNameOfFocusedControl();
			if (text.NullOrEmpty())
			{
				text = focusableControlNames[0];
			}
			int num2 = focusableControlNames.IndexOf(text);
			if (num2 < 0)
			{
				num2 = focusableControlNames.Count;
			}
			num2 = ((!num) ? (num2 - 1) : (num2 + 1));
			if (num2 >= focusableControlNames.Count)
			{
				num2 = 0;
			}
			else if (num2 < 0)
			{
				num2 = focusableControlNames.Count - 1;
			}
			GUI.FocusControl(focusableControlNames[num2]);
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Invalid comparison between Unknown and I4
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		using (TextBlock.Default())
		{
			RectDivider layout = new RectDivider(inRect, 195906069);
			HeaderRow(ref layout);
			layout.NewRow(0f);
			BottomButtons(ref layout);
			layout.NewRow(0f, VerticalJustification.Bottom);
			float num = default(float);
			float num2 = default(float);
			float num3 = default(float);
			Color.RGBToHSV(DefaultColor, ref num, ref num2, ref num3);
			Color defaultColor = Color.HSVToRGB(num, num2, ForcedColorValue);
			defaultColor.a = 1f;
			ColorPalette(ref layout, ref color, defaultColor, ShowDarklight, out var paletteHeight);
			ColorTextfields(ref layout, out var size);
			float height = Mathf.Max(new float[3] { paletteHeight, 128f, size.y });
			RectDivider rectDivider = layout.NewRow(height);
			rectDivider.NewCol(size.x);
			rectDivider.NewCol(250f, HorizontalJustification.Right);
			Rect rect = rectDivider.Rect;
			Rect rect2 = rectDivider.Rect;
			float marginX = (((Rect)(ref rect2)).width - 128f) / 2f;
			rect2 = rectDivider.Rect;
			Widgets.HSVColorWheel(rect.ContractedBy(marginX, (((Rect)(ref rect2)).height - 128f) / 2f), ref color, ref hsvColorWheelDragging, ForcedColorValue);
			layout.NewRow(10f);
			Rect rect3 = layout.NewRow(34f);
			if (ShowColorTemperatureBar)
			{
				Widgets.ColorTemperatureBar(rect3, ref color, ref colorTemperatureDragging, ForcedColorValue);
			}
			layout.NewRow(26f);
			ColorReadback(layout, color, oldColor, ShowDarklight);
			TabControl();
			if ((int)Event.current.type == 8)
			{
				previousFocusedControlName = GUI.GetNameOfFocusedControl();
			}
		}
	}
}
