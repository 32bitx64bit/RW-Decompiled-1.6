using System.Collections.Generic;
using UnityEngine;

namespace Verse.Noise;

public static class GradientPresets
{
	private static Gradient _empty;

	private static Gradient _grayscale;

	private static Gradient _rgb;

	private static Gradient _rgba;

	private static Gradient _terrain;

	public static Gradient Empty => _empty;

	public static Gradient Grayscale => _grayscale;

	public static Gradient RGB => _rgb;

	public static Gradient RGBA => _rgba;

	public static Gradient Terrain => _terrain;

	static GradientPresets()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		List<GradientColorKey> list = new List<GradientColorKey>
		{
			new GradientColorKey(Color.black, 0f),
			new GradientColorKey(Color.white, 1f)
		};
		List<GradientColorKey> list2 = new List<GradientColorKey>
		{
			new GradientColorKey(Color.red, 0f),
			new GradientColorKey(Color.green, 0.5f),
			new GradientColorKey(Color.blue, 1f)
		};
		List<GradientColorKey> list3 = new List<GradientColorKey>
		{
			new GradientColorKey(Color.red, 0f),
			new GradientColorKey(Color.green, 1f / 3f),
			new GradientColorKey(Color.blue, 2f / 3f),
			new GradientColorKey(Color.black, 1f)
		};
		List<GradientAlphaKey> list4 = new List<GradientAlphaKey>
		{
			new GradientAlphaKey(0f, 2f / 3f),
			new GradientAlphaKey(1f, 1f)
		};
		List<GradientColorKey> list5 = new List<GradientColorKey>
		{
			new GradientColorKey(new Color(0f, 0f, 0.5f), 0f),
			new GradientColorKey(new Color(0.125f, 0.25f, 0.5f), 0.4f),
			new GradientColorKey(new Color(0.25f, 0.375f, 0.75f), 0.48f),
			new GradientColorKey(new Color(0f, 0.75f, 0f), 0.5f),
			new GradientColorKey(new Color(0.75f, 0.75f, 0f), 0.625f),
			new GradientColorKey(new Color(0.625f, 0.375f, 0.25f), 0.75f),
			new GradientColorKey(new Color(0.5f, 1f, 1f), 0.875f),
			new GradientColorKey(Color.white, 1f)
		};
		List<GradientAlphaKey> list6 = new List<GradientAlphaKey>
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		};
		_empty = new Gradient();
		_rgb = new Gradient();
		_rgb.SetKeys(list2.ToArray(), list6.ToArray());
		_rgba = new Gradient();
		_rgba.SetKeys(list3.ToArray(), list4.ToArray());
		_grayscale = new Gradient();
		_grayscale.SetKeys(list.ToArray(), list6.ToArray());
		_terrain = new Gradient();
		_terrain.SetKeys(list5.ToArray(), list6.ToArray());
	}
}
