using UnityEngine;

namespace Verse;

public static class Printer_Mesh
{
	public static void PrintMesh(SectionLayer layer, Matrix4x4 TRS, Mesh mesh, Material mat)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		LayerSubMesh subMesh = layer.GetSubMesh(mat);
		int count = subMesh.verts.Count;
		int vertexCount = mesh.vertexCount;
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = mesh.colors32;
		Vector2[] uv = mesh.uv;
		for (int i = 0; i < vertexCount; i++)
		{
			subMesh.verts.Add(((Matrix4x4)(ref TRS)).MultiplyPoint3x4(vertices[i]));
			if (colors.Length > i)
			{
				subMesh.colors.Add(colors[i]);
			}
			else
			{
				subMesh.colors.Add(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
			}
			if (uv.Length > i)
			{
				subMesh.uvs.Add(Vector2.op_Implicit(uv[i]));
			}
			else
			{
				subMesh.uvs.Add(Vector2.op_Implicit(Vector2.zero));
			}
		}
		int[] triangles = mesh.triangles;
		foreach (int num in triangles)
		{
			subMesh.tris.Add(count + num);
		}
	}
}
