using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class Triangulator
{
	private readonly List<Vector3> m_points;

	private readonly List<int> indices = new List<int>();

	public Triangulator(List<Vector3> points)
	{
		m_points = points;
	}

	public List<int> Triangulate()
	{
		indices.Clear();
		int count = m_points.Count;
		if (count < 3)
		{
			return indices;
		}
		int[] array = new int[count];
		if (Area() > 0f)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				array[j] = count - 1 - j;
			}
		}
		int num = count;
		int num2 = 2 * num;
		int num3 = num - 1;
		while (num > 2)
		{
			if (num2-- <= 0)
			{
				return indices;
			}
			int num4 = num3;
			if (num <= num4)
			{
				num4 = 0;
			}
			num3 = num4 + 1;
			if (num <= num3)
			{
				num3 = 0;
			}
			int num5 = num3 + 1;
			if (num <= num5)
			{
				num5 = 0;
			}
			if (Snip(num4, num3, num5, num, array))
			{
				int item = array[num4];
				int item2 = array[num3];
				int item3 = array[num5];
				indices.Add(item);
				indices.Add(item2);
				indices.Add(item3);
				int num6 = num3;
				for (int k = num3 + 1; k < num; k++)
				{
					array[num6] = array[k];
					num6++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		indices.Reverse();
		return indices;
	}

	private float Area()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		int count = m_points.Count;
		float num = 0f;
		int index = count - 1;
		int num2 = 0;
		while (num2 < count)
		{
			Vector3 val = m_points[index];
			Vector3 val2 = m_points[num2];
			num += val.x * val2.z - val2.x * val.z;
			index = num2++;
		}
		return num * 0.5f;
	}

	private bool Snip(int u, int v, int w, int n, int[] V)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = m_points[V[u]];
		Vector3 val2 = m_points[V[v]];
		Vector3 val3 = m_points[V[w]];
		if (Mathf.Epsilon > (val2.x - val.x) * (val3.z - val.z) - (val2.z - val.z) * (val3.x - val.x))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector3 p = m_points[V[i]];
				if (InsideTriangle(val, val2, val3, p))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		float num = C.x - B.x;
		float num2 = C.z - B.z;
		float num3 = A.x - C.x;
		float num4 = A.z - C.z;
		float num5 = B.x - A.x;
		float num6 = B.z - A.z;
		float num7 = P.x - A.x;
		float num8 = P.z - A.z;
		float num9 = P.x - B.x;
		float num10 = P.z - B.z;
		float num11 = P.x - C.x;
		float num12 = P.z - C.z;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		if (num13 >= 0f && num15 >= 0f)
		{
			return num14 >= 0f;
		}
		return false;
	}
}
