using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class ColorsFromSpectrum
{
	public static Color Get(IList<Color> spectrum, float val)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (spectrum.Count == 0)
		{
			Log.Warning("Color spectrum empty.");
			return Color.white;
		}
		if (spectrum.Count == 1)
		{
			return spectrum[0];
		}
		val = Mathf.Clamp01(val);
		float num = 1f / (float)(spectrum.Count - 1);
		int num2 = (int)(val / num);
		if (num2 == spectrum.Count - 1)
		{
			return spectrum[spectrum.Count - 1];
		}
		float num3 = (val - (float)num2 * num) / num;
		return Color.Lerp(spectrum[num2], spectrum[num2 + 1], num3);
	}
}
