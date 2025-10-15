using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld;

public static class LightningBoltMeshMaker
{
	private static List<Vector2> verts2D;

	private static Vector2 lightningTop;

	private const float LightningHeight = 200f;

	private const float LightningRootXVar = 50f;

	private const float VertexInterval = 0.25f;

	private const float MeshWidth = 2f;

	private const float UVIntervalY = 0.04f;

	private const float PerturbAmp = 12f;

	private const float PerturbFreq = 0.007f;

	public static Mesh NewBoltMesh()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		lightningTop = new Vector2(Rand.Range(-50f, 50f), 200f);
		MakeVerticesBase();
		PeturbVerticesRandomly();
		DoubleVertices();
		return MeshFromVerts();
	}

	private static void MakeVerticesBase()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.zero - lightningTop;
		int num = (int)Math.Ceiling(((Vector2)(ref val)).magnitude / 0.25f);
		Vector2 val2 = lightningTop / (float)num;
		verts2D = new List<Vector2>();
		Vector2 val3 = Vector2.zero;
		for (int i = 0; i < num; i++)
		{
			verts2D.Add(val3);
			val3 += val2;
		}
	}

	private static void PeturbVerticesRandomly()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Perlin perlin = new Perlin(0.007000000216066837, 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
		List<Vector2> list = verts2D.ListFullCopy();
		verts2D.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			float num = 12f * (float)perlin.GetValue(i, 0.0, 0.0);
			Vector2 item = list[i] + num * Vector2.right;
			verts2D.Add(item);
		}
	}

	private static void DoubleVertices()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = verts2D.ListFullCopy();
		Vector3 val = default(Vector3);
		Vector2 val2 = default(Vector2);
		verts2D.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			if (i <= list.Count - 2)
			{
				val = Quaternion.AngleAxis(90f, Vector3.up) * Vector2.op_Implicit(list[i] - list[i + 1]);
				((Vector2)(ref val2))._002Ector(val.y, val.z);
				((Vector2)(ref val2)).Normalize();
			}
			Vector2 item = list[i] - 1f * val2;
			Vector2 item2 = list[i] + 1f * val2;
			verts2D.Add(item);
			verts2D.Add(item2);
		}
	}

	private static Mesh MeshFromVerts()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		Vector3[] array = (Vector3[])(object)new Vector3[verts2D.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Vector3(verts2D[i].x, 0f, verts2D[i].y);
		}
		float num = 0f;
		Vector2[] array2 = (Vector2[])(object)new Vector2[verts2D.Count];
		for (int j = 0; j < verts2D.Count; j += 2)
		{
			array2[j] = new Vector2(0f, num);
			array2[j + 1] = new Vector2(1f, num);
			num += 0.04f;
		}
		int[] array3 = new int[verts2D.Count * 3];
		for (int k = 0; k < verts2D.Count - 2; k += 2)
		{
			int num2 = k * 3;
			array3[num2] = k;
			array3[num2 + 1] = k + 1;
			array3[num2 + 2] = k + 2;
			array3[num2 + 3] = k + 2;
			array3[num2 + 4] = k + 1;
			array3[num2 + 5] = k + 3;
		}
		return new Mesh
		{
			vertices = array,
			uv = array2,
			triangles = array3,
			name = "MeshFromVerts()"
		};
	}
}
