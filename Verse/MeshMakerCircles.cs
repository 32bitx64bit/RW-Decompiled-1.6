using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MeshMakerCircles
{
	private const float PieRadius = 0.55f;

	private const int MaxDegreesWide = 361;

	private const int CircleVertexCount = 92;

	private static readonly List<Vector3> tmpVerts = new List<Vector3>(361);

	private static readonly Vector2[] emptyUVs = (Vector2[])(object)new Vector2[361];

	private static readonly Vector3[] tmpCircleVerts = (Vector3[])(object)new Vector3[92];

	private static readonly int[] tmpTris = new int[273];

	public static void MakePieMeshes(Mesh[] meshes)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (meshes.Length < 361)
		{
			throw new ArgumentException();
		}
		Vector3[] array = (Vector3[])(object)new Vector3[362];
		array[0] = new Vector3(0f, 0f, 0f);
		for (int i = 0; i <= 360; i++)
		{
			float num = (float)i / 180f * MathF.PI;
			float num2 = (float)(0.550000011920929 * Math.Cos(num));
			float num3 = (float)(0.550000011920929 * Math.Sin(num));
			array[i + 1] = new Vector3(num2, 0f, num3);
		}
		int[] array2 = new int[1080];
		for (int j = 0; j < 360; j++)
		{
			array2[j * 3] = j + 2;
			array2[j * 3 + 1] = j + 1;
			array2[j * 3 + 2] = 0;
		}
		Bounds bounds = default(Bounds);
		Vector3 val = 0.55f * new Vector3(1f, 0f, 1f);
		((Bounds)(ref bounds)).SetMinMax(-val, val);
		for (int k = 0; k < 361; k++)
		{
			meshes[k] = new Mesh();
			meshes[k].vertices = array;
			meshes[k].SetTriangles(array2, 0, 3 * k, 0, false, 0);
			meshes[k].RecalculateNormals();
			meshes[k].bounds = bounds;
		}
	}

	public static Mesh MakeCircleMesh(float radius)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		tmpCircleVerts[0] = Vector3.zero;
		int num = 0;
		int num2 = 1;
		while (num <= 360)
		{
			float num3 = (float)num / 180f * MathF.PI;
			tmpCircleVerts[num2] = new Vector3(radius * Mathf.Cos(num3), 0f, radius * Mathf.Sin(num3));
			num += 4;
			num2++;
		}
		for (int i = 1; i < tmpCircleVerts.Length; i++)
		{
			int num4 = (i - 1) * 3;
			tmpTris[num4] = 0;
			tmpTris[num4 + 1] = (i + 1) % tmpCircleVerts.Length;
			tmpTris[num4 + 2] = i;
		}
		Mesh val = new Mesh
		{
			name = $"CircleMesh_{radius:0.#}"
		};
		val.SetVertices(tmpCircleVerts);
		val.SetTriangles(tmpTris, 0);
		return val;
	}
}
