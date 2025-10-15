using System.Collections.Generic;
using UnityEngine;

namespace RimWorld.Planet;

public static class IcosahedronGenerator
{
	private static readonly TriangleIndices[] IcosahedronTris = new TriangleIndices[20]
	{
		new TriangleIndices(0, 11, 5),
		new TriangleIndices(0, 5, 1),
		new TriangleIndices(0, 1, 7),
		new TriangleIndices(0, 7, 10),
		new TriangleIndices(0, 10, 11),
		new TriangleIndices(1, 5, 9),
		new TriangleIndices(5, 11, 4),
		new TriangleIndices(11, 10, 2),
		new TriangleIndices(10, 7, 6),
		new TriangleIndices(7, 1, 8),
		new TriangleIndices(3, 9, 4),
		new TriangleIndices(3, 4, 2),
		new TriangleIndices(3, 2, 6),
		new TriangleIndices(3, 6, 8),
		new TriangleIndices(3, 8, 9),
		new TriangleIndices(4, 9, 5),
		new TriangleIndices(2, 4, 11),
		new TriangleIndices(6, 2, 10),
		new TriangleIndices(8, 6, 7),
		new TriangleIndices(9, 8, 1)
	};

	public static void GenerateIcosahedron(List<Vector3> outVerts, List<TriangleIndices> outTris, float radius, Vector3 viewCenter, float viewAngle)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		float num = (1f + Mathf.Sqrt(5f)) / 2f;
		outVerts.Clear();
		Vector3 val = new Vector3(-1f, num, 0f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(1f, num, 0f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(-1f, 0f - num, 0f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(1f, 0f - num, 0f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f, -1f, num);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f, 1f, num);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f, -1f, 0f - num);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f, 1f, 0f - num);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(num, 0f, -1f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(num, 0f, 1f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f - num, 0f, -1f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		val = new Vector3(0f - num, 0f, 1f);
		outVerts.Add(((Vector3)(ref val)).normalized * radius);
		outTris.Clear();
		int i = 0;
		for (int num2 = IcosahedronTris.Length; i < num2; i++)
		{
			TriangleIndices item = IcosahedronTris[i];
			if (IcosahedronFaceNeeded(item.v1, item.v2, item.v3, outVerts, radius, viewCenter, viewAngle))
			{
				outTris.Add(item);
			}
		}
		MeshUtility.RemoveUnusedVertices(outVerts, outTris);
	}

	private static bool IcosahedronFaceNeeded(int v1, int v2, int v3, List<Vector3> verts, float radius, Vector3 viewCenter, float viewAngle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		viewAngle += 18f;
		if (!MeshUtility.Visible(verts[v1], radius, viewCenter, viewAngle) && !MeshUtility.Visible(verts[v2], radius, viewCenter, viewAngle))
		{
			return MeshUtility.Visible(verts[v3], radius, viewCenter, viewAngle);
		}
		return true;
	}
}
