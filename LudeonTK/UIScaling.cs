using System;
using UnityEngine;
using Verse;

namespace LudeonTK;

public static class UIScaling
{
	public static Rect AdjustRectToUIScaling(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Rect result = rect;
		((Rect)(ref result)).xMin = AdjustCoordToUIScalingFloor(((Rect)(ref rect)).xMin);
		((Rect)(ref result)).yMin = AdjustCoordToUIScalingFloor(((Rect)(ref rect)).yMin);
		((Rect)(ref result)).xMax = AdjustCoordToUIScalingCeil(((Rect)(ref rect)).xMax);
		((Rect)(ref result)).yMax = AdjustCoordToUIScalingCeil(((Rect)(ref rect)).yMax);
		return result;
	}

	public static float AdjustCoordToUIScalingFloor(float coord)
	{
		double num = Prefs.UIScale * coord;
		float num2 = (float)(num - Math.Floor(num)) / Prefs.UIScale;
		return coord - num2;
	}

	public static float AdjustCoordToUIScalingCeil(float coord)
	{
		double num = Prefs.UIScale * coord;
		float num2 = (float)(num - Math.Ceiling(num)) / Prefs.UIScale;
		return coord - num2;
	}
}
