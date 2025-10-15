using UnityEngine;

namespace Verse;

public class ColorOption
{
	public float weight = 1f;

	public Color min = new Color(-1f, -1f, -1f, -1f);

	public Color max = new Color(-1f, -1f, -1f, -1f);

	public Color only = new Color(-1f, -1f, -1f, -1f);

	public Color RandomizedColor()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (only.a >= 0f)
		{
			return only;
		}
		return new Color(Rand.Range(min.r, max.r), Rand.Range(min.g, max.g), Rand.Range(min.b, max.b), Rand.Range(min.a, max.a));
	}

	public void SetSingle(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		only = color;
	}

	public void SetMin(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		min = color;
	}

	public void SetMax(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		max = color;
	}
}
