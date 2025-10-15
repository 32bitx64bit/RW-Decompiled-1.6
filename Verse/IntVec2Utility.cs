using UnityEngine;

namespace Verse;

public static class IntVec2Utility
{
	public static IntVec2 ToIntVec2(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new IntVec2((int)v.x, (int)v.z);
	}

	public static IntVec2 ToIntVec2(this Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new IntVec2((int)v.x, (int)v.y);
	}
}
