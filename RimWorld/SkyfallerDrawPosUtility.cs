using UnityEngine;
using Verse;

namespace RimWorld;

public static class SkyfallerDrawPosUtility
{
	public static Vector3 DrawPos_Accelerate(Vector3 center, int ticksToImpact, float angle, float speed, bool flipHorizontal = false, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = Mathf.Pow((float)ticksToImpact, 0.95f) * 1.7f * speed;
		return PosAtDist(center, dist, angle, flipHorizontal, offsetComp);
	}

	public static Vector3 DrawPos_ConstantSpeed(Vector3 center, int ticksToImpact, float angle, float speed, bool flipHorizontal = false, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = (float)ticksToImpact * speed;
		return PosAtDist(center, dist, angle, flipHorizontal, offsetComp);
	}

	public static Vector3 DrawPos_Decelerate(Vector3 center, int ticksToImpact, float angle, float speed, bool flipHorizontal = false, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = (float)(ticksToImpact * ticksToImpact) * 0.00721f * speed;
		return PosAtDist(center, dist, angle, flipHorizontal, offsetComp);
	}

	private static Vector3 PosAtDist(Vector3 center, float dist, float angle, bool flipHorizontal = false, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Utility.FromAngleFlat(angle - 90f) * dist + (offsetComp?.Offset ?? Vector3.zero);
		if (flipHorizontal)
		{
			val.x *= -1f;
		}
		return val + center;
	}
}
