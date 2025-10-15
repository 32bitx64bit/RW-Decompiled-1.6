using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_GlowerColorPicker : Dialog_ColorPickerBase
{
	protected const float GlowValue = 1f;

	private static readonly List<Color> colors = new List<Color>
	{
		Color.HSVToRGB(0f, 0f, 1f),
		Color.HSVToRGB(0f, 0.5f, 1f),
		Color.HSVToRGB(0f, 0.33f, 1f),
		Color.HSVToRGB(1f / 18f, 1f, 1f),
		Color.HSVToRGB(1f / 18f, 0.5f, 1f),
		Color.HSVToRGB(1f / 18f, 0.33f, 1f),
		Color.HSVToRGB(1f / 9f, 1f, 1f),
		Color.HSVToRGB(1f / 9f, 0.5f, 1f),
		Color.HSVToRGB(1f / 9f, 0.33f, 1f),
		Color.HSVToRGB(1f / 6f, 1f, 1f),
		Color.HSVToRGB(1f / 6f, 0.5f, 1f),
		Color.HSVToRGB(1f / 6f, 0.33f, 1f),
		Color.HSVToRGB(2f / 9f, 1f, 1f),
		Color.HSVToRGB(2f / 9f, 0.5f, 1f),
		Color.HSVToRGB(2f / 9f, 0.33f, 1f),
		Color.HSVToRGB(5f / 18f, 1f, 1f),
		Color.HSVToRGB(5f / 18f, 0.5f, 1f),
		Color.HSVToRGB(5f / 18f, 0.33f, 1f),
		Color.HSVToRGB(1f / 3f, 1f, 1f),
		Color.HSVToRGB(1f / 3f, 0.5f, 1f),
		Color.HSVToRGB(1f / 3f, 0.33f, 1f),
		Color.HSVToRGB(7f / 18f, 1f, 1f),
		Color.HSVToRGB(7f / 18f, 0.5f, 1f),
		Color.HSVToRGB(7f / 18f, 0.33f, 1f),
		Color.HSVToRGB(4f / 9f, 1f, 1f),
		Color.HSVToRGB(4f / 9f, 0.5f, 1f),
		Color.HSVToRGB(4f / 9f, 0.33f, 1f),
		Color.HSVToRGB(0.5f, 1f, 1f),
		Color.HSVToRGB(0.5f, 0.5f, 1f),
		Color.HSVToRGB(0.5f, 0.33f, 1f),
		Color.HSVToRGB(5f / 9f, 1f, 1f),
		Color.HSVToRGB(5f / 9f, 0.5f, 1f),
		Color.HSVToRGB(5f / 9f, 0.33f, 1f),
		Color.HSVToRGB(11f / 18f, 1f, 1f),
		Color.HSVToRGB(11f / 18f, 0.5f, 1f),
		Color.HSVToRGB(11f / 18f, 0.33f, 1f),
		Color.HSVToRGB(2f / 3f, 1f, 1f),
		Color.HSVToRGB(2f / 3f, 0.5f, 1f),
		Color.HSVToRGB(2f / 3f, 0.33f, 1f),
		Color.HSVToRGB(13f / 18f, 1f, 1f),
		Color.HSVToRGB(13f / 18f, 0.5f, 1f),
		Color.HSVToRGB(13f / 18f, 0.33f, 1f),
		Color.HSVToRGB(7f / 9f, 1f, 1f),
		Color.HSVToRGB(7f / 9f, 0.5f, 1f),
		Color.HSVToRGB(7f / 9f, 0.33f, 1f),
		Color.HSVToRGB(5f / 6f, 1f, 1f),
		Color.HSVToRGB(5f / 6f, 0.5f, 1f),
		Color.HSVToRGB(5f / 6f, 0.33f, 1f),
		Color.HSVToRGB(8f / 9f, 1f, 1f),
		Color.HSVToRGB(8f / 9f, 0.5f, 1f),
		Color.HSVToRGB(8f / 9f, 0.33f, 1f),
		Color.HSVToRGB(17f / 18f, 1f, 1f),
		Color.HSVToRGB(17f / 18f, 0.5f, 1f),
		Color.HSVToRGB(17f / 18f, 0.33f, 1f)
	};

	private CompGlower glower;

	private CompGlower[] extraGlowers;

	protected override Color DefaultColor => glower.Props.glowColor.ToColor;

	protected override bool ShowDarklight { get; } = true;


	protected override List<Color> PickableColors => colors;

	protected override float ForcedColorValue => 1f;

	protected override bool ShowColorTemperatureBar => true;

	public Dialog_GlowerColorPicker(CompGlower glower, IList<CompGlower> extraGlowers, Widgets.ColorComponents visibleTextfields, Widgets.ColorComponents editableTextfields)
		: base(visibleTextfields, editableTextfields)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		this.glower = glower;
		this.extraGlowers = new CompGlower[extraGlowers.Count];
		extraGlowers.CopyTo(this.extraGlowers, 0);
		float num = default(float);
		float num2 = default(float);
		float num3 = default(float);
		Color.RGBToHSV(glower.GlowColor.ToColor, ref num, ref num2, ref num3);
		color = Color.HSVToRGB(num, num2, 1f);
		oldColor = color;
	}

	protected override void SaveColor(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		float hue = default(float);
		float sat = default(float);
		float num = default(float);
		Color.RGBToHSV(color, ref hue, ref sat, ref num);
		ColorInt glowColor = glower.GlowColor;
		glowColor.SetHueSaturation(hue, sat);
		glower.GlowColor = glowColor;
		CompGlower[] array = extraGlowers;
		foreach (CompGlower obj in array)
		{
			glowColor = obj.GlowColor;
			glowColor.SetHueSaturation(hue, sat);
			obj.GlowColor = glowColor;
		}
	}
}
