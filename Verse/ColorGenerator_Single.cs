using UnityEngine;

namespace Verse;

public class ColorGenerator_Single : ColorGenerator
{
	public Color color;

	public override Color NewRandomizedColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return color;
	}
}
