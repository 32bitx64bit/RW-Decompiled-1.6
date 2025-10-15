using System;
using UnityEngine;

namespace Verse;

public static class Vector2Utility
{
	public static Vector2 Rotated(this Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(v.y, v.x);
	}

	public static Vector2 RotatedBy(this Vector2 v, Rot4 rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return v.RotatedBy(rot.AsAngle);
	}

	public static Vector2 RotatedBy(this Vector2 v, float degrees)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Sin(degrees * (MathF.PI / 180f));
		float num2 = Mathf.Cos(degrees * (MathF.PI / 180f));
		return new Vector2(num2 * v.x - num * v.y, num * v.x + num2 * v.y);
	}

	public static float AngleTo(this Vector2 a, Vector2 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Atan2(0f - (b.y - a.y), b.x - a.x) * 57.29578f;
	}

	public static Vector3 ScaledBy(this Vector3 a, Vector3 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector2 Moved(this Vector2 v, float angle, float distance)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(v.x + Mathf.Cos(angle * (MathF.PI / 180f)) * distance, v.y - Mathf.Sin(angle * (MathF.PI / 180f)) * distance);
	}

	public static Vector2 FromAngle(float angle)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Mathf.Cos(angle * (MathF.PI / 180f)), 0f - Mathf.Sin(angle * (MathF.PI / 180f)));
	}

	public static float ToAngle(this Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Atan2(0f - v.y, v.x) * 57.29578f;
	}

	public static Vector3 ToVector3(this Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(v.x, 0f, v.y);
	}

	public static Vector2 ToVector2(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(v.x, v.z);
	}

	public static float Cross(this Vector2 u, Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return u.x * v.y - u.y * v.x;
	}

	public static float DistanceToRect(this Vector2 u, Rect rect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		if (((Rect)(ref rect)).Contains(u))
		{
			return 0f;
		}
		if (u.x < ((Rect)(ref rect)).xMin && u.y < ((Rect)(ref rect)).yMin)
		{
			return Vector2.Distance(u, new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin));
		}
		if (u.x > ((Rect)(ref rect)).xMax && u.y < ((Rect)(ref rect)).yMin)
		{
			return Vector2.Distance(u, new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin));
		}
		if (u.x < ((Rect)(ref rect)).xMin && u.y > ((Rect)(ref rect)).yMax)
		{
			return Vector2.Distance(u, new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax));
		}
		if (u.x > ((Rect)(ref rect)).xMax && u.y > ((Rect)(ref rect)).yMax)
		{
			return Vector2.Distance(u, new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax));
		}
		if (u.x < ((Rect)(ref rect)).xMin)
		{
			return ((Rect)(ref rect)).xMin - u.x;
		}
		if (u.x > ((Rect)(ref rect)).xMax)
		{
			return u.x - ((Rect)(ref rect)).xMax;
		}
		if (u.y < ((Rect)(ref rect)).yMin)
		{
			return ((Rect)(ref rect)).yMin - u.y;
		}
		return u.y - ((Rect)(ref rect)).yMax;
	}

	public static Vector3 MultipliedBy(this Vector3 a, Vector3 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector2 Cardinalize(this Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return vec.Cardinalize(Vector2.zero);
	}

	public static Vector2 Cardinalize(this Vector2 vec, Vector2 bias)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector2)(ref vec)).sqrMagnitude == 0f)
		{
			return vec;
		}
		Vector2 val = vec + bias;
		Vector2 normalized = ((Vector2)(ref val)).normalized;
		float num = Mathf.Abs(normalized.x);
		float num2 = Mathf.Abs(normalized.y);
		if (num > num2)
		{
			if (normalized.x > 0f)
			{
				return Vector2.right;
			}
			return Vector2.left;
		}
		if (normalized.y > 0f)
		{
			return Vector2.up;
		}
		return Vector2.down;
	}
}
