using UnityEngine;

namespace Verse.Noise;

public static class NoiseRenderer
{
	public static IntVec2 renderSize = new IntVec2(200, 200);

	private static Color[] spectrum = (Color[])(object)new Color[4]
	{
		Color.black,
		Color.blue,
		Color.green,
		Color.white
	};

	public static Texture2D NoiseRendered(ModuleBase noise)
	{
		return NoiseRendered(new CellRect(0, 0, renderSize.x, renderSize.z), noise);
	}

	public static Texture2D NoiseRendered(CellRect rect, ModuleBase noise)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(rect.Width, rect.Height);
		((Object)val).name = "NoiseRender";
		foreach (IntVec2 item in rect.Cells2D)
		{
			val.SetPixel(item.x, item.z, ColorForValue(noise.GetValue(item)));
		}
		val.Apply();
		return val;
	}

	private static Color ColorForValue(float val)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		val = val * 0.5f + 0.5f;
		return ColorsFromSpectrum.Get(spectrum, val);
	}
}
