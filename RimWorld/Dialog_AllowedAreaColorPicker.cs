using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_AllowedAreaColorPicker : Dialog_ColorPickerBase
{
	protected const float AreaColorValue = 0.5f;

	private static readonly List<Color> colors = new List<Color>
	{
		Color.HSVToRGB(0f, 0f, 0.5f),
		Color.HSVToRGB(0f, 0.5f, 0.5f),
		Color.HSVToRGB(0f, 0.33f, 0.5f),
		Color.HSVToRGB(1f / 18f, 1f, 0.5f),
		Color.HSVToRGB(1f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(1f / 18f, 0.33f, 0.5f),
		Color.HSVToRGB(1f / 9f, 1f, 0.5f),
		Color.HSVToRGB(1f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(1f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(1f / 6f, 1f, 0.5f),
		Color.HSVToRGB(1f / 6f, 0.5f, 0.5f),
		Color.HSVToRGB(1f / 6f, 0.33f, 0.5f),
		Color.HSVToRGB(2f / 9f, 1f, 0.5f),
		Color.HSVToRGB(2f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(2f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(5f / 18f, 1f, 0.5f),
		Color.HSVToRGB(5f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(5f / 18f, 0.33f, 0.5f),
		Color.HSVToRGB(1f / 3f, 1f, 0.5f),
		Color.HSVToRGB(1f / 3f, 0.5f, 0.5f),
		Color.HSVToRGB(1f / 3f, 0.33f, 0.5f),
		Color.HSVToRGB(7f / 18f, 1f, 0.5f),
		Color.HSVToRGB(7f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(7f / 18f, 0.33f, 0.5f),
		Color.HSVToRGB(4f / 9f, 1f, 0.5f),
		Color.HSVToRGB(4f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(4f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(0.5f, 1f, 0.5f),
		Color.HSVToRGB(0.5f, 0.5f, 0.5f),
		Color.HSVToRGB(0.5f, 0.33f, 0.5f),
		Color.HSVToRGB(5f / 9f, 1f, 0.5f),
		Color.HSVToRGB(5f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(5f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(11f / 18f, 1f, 0.5f),
		Color.HSVToRGB(11f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(11f / 18f, 0.33f, 0.5f),
		Color.HSVToRGB(2f / 3f, 1f, 0.5f),
		Color.HSVToRGB(2f / 3f, 0.5f, 0.5f),
		Color.HSVToRGB(2f / 3f, 0.33f, 0.5f),
		Color.HSVToRGB(13f / 18f, 1f, 0.5f),
		Color.HSVToRGB(13f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(13f / 18f, 0.33f, 0.5f),
		Color.HSVToRGB(7f / 9f, 1f, 0.5f),
		Color.HSVToRGB(7f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(7f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(5f / 6f, 1f, 0.5f),
		Color.HSVToRGB(5f / 6f, 0.5f, 0.5f),
		Color.HSVToRGB(5f / 6f, 0.33f, 0.5f),
		Color.HSVToRGB(8f / 9f, 1f, 0.5f),
		Color.HSVToRGB(8f / 9f, 0.5f, 0.5f),
		Color.HSVToRGB(8f / 9f, 0.33f, 0.5f),
		Color.HSVToRGB(17f / 18f, 1f, 0.5f),
		Color.HSVToRGB(17f / 18f, 0.5f, 0.5f),
		Color.HSVToRGB(17f / 18f, 0.33f, 0.5f)
	};

	private Area_Allowed area;

	protected override Color DefaultColor => oldColor;

	protected override bool ShowDarklight => false;

	protected override List<Color> PickableColors => colors;

	protected override float ForcedColorValue => 0.5f;

	protected override bool ShowColorTemperatureBar => false;

	public Dialog_AllowedAreaColorPicker(Area_Allowed area)
		: base(Widgets.ColorComponents.None, Widgets.ColorComponents.None)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		this.area = area;
		float num = default(float);
		float num2 = default(float);
		float num3 = default(float);
		Color.RGBToHSV(area.Color, ref num, ref num2, ref num3);
		color = Color.HSVToRGB(num, num2, 0.5f);
		oldColor = color;
	}

	protected override void SaveColor(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float hue = default(float);
		float sat = default(float);
		float num = default(float);
		Color.RGBToHSV(color, ref hue, ref sat, ref num);
		ColorInt colorInt = new ColorInt(area.Color);
		colorInt.SetHueSaturation(hue, sat);
		area.SetColor(colorInt.ToColor);
	}
}
