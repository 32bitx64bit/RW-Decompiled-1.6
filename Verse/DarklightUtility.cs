using UnityEngine;

namespace Verse;

public static class DarklightUtility
{
	private static FloatRange DarklightHueRange = new FloatRange(0.49f, 0.51f);

	public static readonly Color DefaultDarklight = Color32.op_Implicit(new Color32((byte)78, (byte)226, (byte)229, byte.MaxValue));

	public static bool IsDarklight(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (color.r > color.g || color.r > color.b)
		{
			return false;
		}
		float num;
		float num2;
		if (color.g > color.b)
		{
			num = color.g;
			num2 = color.b;
		}
		else
		{
			num = color.b;
			num2 = color.g;
		}
		if (num == 0f)
		{
			return false;
		}
		if (color.r > num / 2f)
		{
			return false;
		}
		if (num2 / num <= 0.85f)
		{
			return false;
		}
		return true;
	}

	public static bool IsDarklightAt(IntVec3 position, Map map)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (position.InBounds(map) && position.Roofed(map) && (int)map.glowGrid.PsychGlowAt(position) >= 1)
		{
			return IsDarklight(Color32.op_Implicit(map.glowGrid.VisualGlowAt(position)));
		}
		return false;
	}
}
