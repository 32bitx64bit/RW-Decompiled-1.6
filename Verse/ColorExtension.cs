using UnityEngine;

namespace Verse;

public static class ColorExtension
{
	public static Color ToOpaque(this Color c)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		c.a = 1f;
		return c;
	}

	public static Color ToTransparent(this Color c, float transparency)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		c.a = transparency;
		return c;
	}

	public static Color Min(this Color c, Color other)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		c.r = Mathf.Min(c.r, other.r);
		c.g = Mathf.Min(c.g, other.g);
		c.b = Mathf.Min(c.b, other.b);
		return c;
	}
}
