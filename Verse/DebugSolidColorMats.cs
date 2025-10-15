using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class DebugSolidColorMats
{
	private static Dictionary<Color, Material> colorMatDict = new Dictionary<Color, Material>();

	public static Material MaterialOf(Color col)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (colorMatDict.TryGetValue(col, out var value))
		{
			return value;
		}
		value = SolidColorMaterials.SimpleSolidColorMaterial(col);
		colorMatDict.Add(col, value);
		return value;
	}
}
