using UnityEngine;

namespace Verse;

public static class MatsFromSpectrum
{
	public static Material Get(Color[] spectrum, float val)
	{
		return Get(spectrum, val, ShaderDatabase.MetaOverlay);
	}

	public static Material Get(Color[] spectrum, float val, Shader shader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return SolidColorMaterials.NewSolidColorMaterial(ColorsFromSpectrum.Get(spectrum, val), shader);
	}
}
