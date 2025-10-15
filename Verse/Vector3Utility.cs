using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse;

public static class Vector3Utility
{
	public static Vector3 HorizontalVectorFromAngle(float angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
	}

	public static float AngleFlat(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (v.x == 0f && v.z == 0f)
		{
			return 0f;
		}
		Quaternion val = Quaternion.LookRotation(v);
		return ((Quaternion)(ref val)).eulerAngles.y;
	}

	public static Vector3 RandomHorizontalOffset(float maxDist)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = Rand.Range(0f, maxDist);
		float num2 = Rand.Range(0, 360);
		return Quaternion.Euler(new Vector3(0f, num2, 0f)) * Vector3.forward * num;
	}

	public static Vector3 Yto0(this Vector3 v3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(v3.x, 0f, v3.z);
	}

	public static Vector3 WithYOffset(this Vector3 v3, float offset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(v3.x, v3.y + offset, v3.z);
	}

	public static Vector3 WithY(this Vector3 v3, float y)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(v3.x, y, v3.z);
	}

	public static Vector3 SetToAltitude(this Vector3 v3, AltitudeLayer altitude)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		v3.y = altitude.AltitudeFor();
		return v3;
	}

	public static Vector3 RotatedBy(this Vector3 v3, float angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.AngleAxis(angle, Vector3.up) * v3;
	}

	public static Vector3 RotatedBy(this Vector3 orig, Rot4 rot)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		return (Vector3)(rot.AsInt switch
		{
			0 => orig, 
			1 => new Vector3(orig.z, orig.y, 0f - orig.x), 
			2 => new Vector3(0f - orig.x, orig.y, 0f - orig.z), 
			3 => new Vector3(0f - orig.z, orig.y, orig.x), 
			_ => orig, 
		});
	}

	public static float AngleToFlat(this Vector3 a, Vector3 b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return Vector2Utility.AngleTo(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
	}

	public static Vector3 FromAngleFlat(float angle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2Utility.FromAngle(angle);
		return new Vector3(val.x, 0f, val.y);
	}

	public static float ToAngleFlat(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector2Utility.ToAngle(new Vector2(v.x, v.z));
	}

	public static Vector3 Abs(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
	}

	public static Vector3 ClosestTo(this IEnumerable<Vector3> pts, Vector3 target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		Vector3 result = pts.FirstOrDefault();
		foreach (Vector3 pt in pts)
		{
			float num2 = Vector3.Distance(pt, target);
			if (num2 < num)
			{
				num = num2;
				result = pt;
			}
		}
		return result;
	}
}
