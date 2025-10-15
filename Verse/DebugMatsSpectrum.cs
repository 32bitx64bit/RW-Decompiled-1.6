using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class DebugMatsSpectrum
{
	private static readonly Material[] spectrumMatsTranparent;

	private static readonly Material[] spectrumMatsOpaque;

	public const int MaterialCount = 100;

	public static Color[] DebugSpectrum;

	static DebugMatsSpectrum()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		spectrumMatsTranparent = (Material[])(object)new Material[100];
		spectrumMatsOpaque = (Material[])(object)new Material[100];
		DebugSpectrum = (Color[])(object)new Color[5]
		{
			new Color(0.75f, 0f, 0f),
			new Color(0.5f, 0.3f, 0f),
			new Color(0f, 1f, 0f),
			new Color(0f, 0f, 1f),
			new Color(0.7f, 0f, 1f)
		};
		for (int i = 0; i < 100; i++)
		{
			spectrumMatsTranparent[i] = MatsFromSpectrum.Get(DebugSpectrumWithOpacity(0.25f), (float)i / 100f);
			spectrumMatsOpaque[i] = MatsFromSpectrum.Get(DebugSpectrumWithOpacity(1f), (float)i / 100f);
		}
	}

	private static Color[] DebugSpectrumWithOpacity(float opacity)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[DebugSpectrum.Length];
		for (int i = 0; i < DebugSpectrum.Length; i++)
		{
			array[i] = new Color(DebugSpectrum[i].r, DebugSpectrum[i].g, DebugSpectrum[i].b, opacity);
		}
		return array;
	}

	public static Material Mat(int ind, bool transparent)
	{
		if (ind >= 100)
		{
			ind = 99;
		}
		if (ind < 0)
		{
			ind = 0;
		}
		if (!transparent)
		{
			return spectrumMatsOpaque[ind];
		}
		return spectrumMatsTranparent[ind];
	}
}
