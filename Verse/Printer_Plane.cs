using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class Printer_Plane
{
	private static readonly Color32[] DefaultColors = (Color32[])(object)new Color32[4]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
	};

	private static readonly Vector2[] DefaultUvs = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(1f, 0f)
	};

	private static readonly Vector2[] DefaultUvsFlipped = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(1f, 0f),
		new Vector2(1f, 1f),
		new Vector2(0f, 1f),
		new Vector2(0f, 0f)
	};

	public static void GetUVs(Rect rect, out Vector2 uv1, out Vector2 uv2, out Vector2 uv3, out Vector2 uv4, bool flipUv)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (flipUv)
		{
			uv1 = new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin);
			uv2 = new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
			uv3 = new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax);
			uv4 = new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin);
		}
		else
		{
			uv1 = new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin);
			uv2 = new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax);
			uv3 = new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
			uv4 = new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin);
		}
	}

	public static void PrintPlane(MapDrawLayer layer, Vector3 center, Vector2 size, Material mat, float rot = 0f, bool flipUv = false, Vector2[] uvs = null, Color32[] colors = null, float topVerticesAltitudeBias = 0.01f, float uvzPayload = 0f)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (colors == null)
		{
			colors = DefaultColors;
		}
		if (uvs == null)
		{
			uvs = ((!flipUv) ? DefaultUvs : DefaultUvsFlipped);
		}
		LayerSubMesh subMesh = layer.GetSubMesh(mat);
		int count = subMesh.verts.Count;
		subMesh.verts.Add(new Vector3(-0.5f * size.x, 0f, -0.5f * size.y));
		subMesh.verts.Add(new Vector3(-0.5f * size.x, topVerticesAltitudeBias, 0.5f * size.y));
		subMesh.verts.Add(new Vector3(0.5f * size.x, topVerticesAltitudeBias, 0.5f * size.y));
		subMesh.verts.Add(new Vector3(0.5f * size.x, 0f, -0.5f * size.y));
		if (rot != 0f)
		{
			float num = rot * (MathF.PI / 180f);
			num *= -1f;
			for (int i = 0; i < 4; i++)
			{
				float x = subMesh.verts[count + i].x;
				float z = subMesh.verts[count + i].z;
				float num2 = Mathf.Cos(num);
				float num3 = Mathf.Sin(num);
				float num4 = x * num2 - z * num3;
				float num5 = x * num3 + z * num2;
				subMesh.verts[count + i] = new Vector3(num4, subMesh.verts[count + i].y, num5);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			List<Vector3> verts = subMesh.verts;
			int index = count + j;
			verts[index] += center;
			subMesh.uvs.Add(new Vector3(uvs[j].x, uvs[j].y, uvzPayload));
			subMesh.colors.Add(colors[j]);
		}
		subMesh.tris.Add(count);
		subMesh.tris.Add(count + 1);
		subMesh.tris.Add(count + 2);
		subMesh.tris.Add(count);
		subMesh.tris.Add(count + 2);
		subMesh.tris.Add(count + 3);
	}
}
