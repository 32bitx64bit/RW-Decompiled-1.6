using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class CurvedScalerExt
{
	public static Vector3 ScaleAtTime(this List<CurvedScaler> scalers, float ageSecs)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.one;
		foreach (CurvedScaler scaler in scalers)
		{
			Rand.PushState(scaler.GetHashCode());
			float randomInRange = scaler.scaleTime.RandomInRange;
			float randomInRange2 = scaler.scaleAmt.RandomInRange;
			float x = Mathf.Clamp01(ageSecs / randomInRange);
			val += randomInRange2 * scaler.curve.Evaluate(x) * scaler.axisMask;
			Rand.PopState();
		}
		return val;
	}
}
