using UnityEngine;

namespace Verse;

public static class MaterialUtility
{
	public static Texture2D GetMaskTexture(this Material mat)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (!mat.HasProperty(ShaderPropertyIDs.MaskTex))
		{
			return null;
		}
		return (Texture2D)mat.GetTexture(ShaderPropertyIDs.MaskTex);
	}

	public static Color GetColorTwo(this Material mat)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!mat.HasProperty(ShaderPropertyIDs.ColorTwo))
		{
			return Color.white;
		}
		return mat.GetColor(ShaderPropertyIDs.ColorTwo);
	}
}
