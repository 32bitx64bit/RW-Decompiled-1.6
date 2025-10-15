using System.Collections.Generic;
using UnityEngine;

namespace RimWorld;

public static class LineMeshGenerator
{
	public static Mesh Generate(Vector2[] points, float width)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[points.Length * 2];
		Vector2[] array2 = (Vector2[])(object)new Vector2[array.Length];
		int[] array3 = new int[2 * (points.Length - 1) * 3];
		int num = 0;
		int num2 = 0;
		Vector2 val2 = default(Vector2);
		for (int i = 0; i < points.Length; i++)
		{
			Vector2 val = Vector2.zero;
			if (i < points.Length - 1)
			{
				val += points[(i + 1) % points.Length] - points[i];
			}
			if (i > 0)
			{
				val += points[i] - points[(i - 1 + points.Length) % points.Length];
			}
			((Vector2)(ref val)).Normalize();
			((Vector2)(ref val2))._002Ector(0f - val.y, val.x);
			Vector2 val3 = points[i] + val2 * (width * 0.5f);
			Vector2 val4 = points[i] - val2 * (width * 0.5f);
			array[num] = new Vector3(val3.x, 0f, val3.y);
			array[num + 1] = new Vector3(val4.x, 0f, val4.y);
			float num3 = (float)i / (float)(points.Length - 1);
			float num4 = 1f - Mathf.Abs(2f * num3 - 1f);
			array2[num] = new Vector2(0f, num4);
			array2[num + 1] = new Vector2(1f, num4);
			if (i < points.Length - 1)
			{
				array3[num2] = num;
				array3[num2 + 1] = (num + 2) % array.Length;
				array3[num2 + 2] = num + 1;
				array3[num2 + 3] = num + 1;
				array3[num2 + 4] = (num + 2) % array.Length;
				array3[num2 + 5] = (num + 3) % array.Length;
			}
			num += 2;
			num2 += 6;
		}
		return new Mesh
		{
			vertices = array,
			triangles = array3,
			uv = array2
		};
	}

	public static Vector2[] CalculateEvenlySpacedPoints(List<Vector2> points, float spacing, float resolution = 1f)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2> { points[0] };
		Vector2 val = points[0];
		int num = points.Count / 3;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			Vector2[] pointsInSegment = GetPointsInSegment(points, i);
			float num3 = Vector2.Distance(pointsInSegment[0], pointsInSegment[1]) + Vector2.Distance(pointsInSegment[1], pointsInSegment[2]) + Vector2.Distance(pointsInSegment[2], pointsInSegment[3]);
			int num4 = Mathf.CeilToInt((Vector2.Distance(pointsInSegment[0], pointsInSegment[3]) + num3 / 2f) * resolution * 10f);
			float num5 = 0f;
			while (num5 <= 1f)
			{
				num5 += 1f / (float)num4;
				Vector2 val2 = Bezier.EvaluateCubic(pointsInSegment[0], pointsInSegment[1], pointsInSegment[2], pointsInSegment[3], num5);
				num2 += Vector2.Distance(val, val2);
				while (num2 >= spacing)
				{
					float num6 = num2 - spacing;
					Vector2 val3 = val - val2;
					Vector2 val4 = val2 + ((Vector2)(ref val3)).normalized * num6;
					list.Add(val4);
					num2 = num6;
					val = val4;
				}
				val = val2;
			}
		}
		return list.ToArray();
	}

	private static Vector2[] GetPointsInSegment(List<Vector2> points, int i)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		return (Vector2[])(object)new Vector2[4]
		{
			points[i * 3],
			points[i * 3 + 1],
			points[i * 3 + 2],
			points[LoopIndex(points, i * 3 + 3)]
		};
	}

	private static int LoopIndex(List<Vector2> points, int i)
	{
		return (i + points.Count) % points.Count;
	}
}
