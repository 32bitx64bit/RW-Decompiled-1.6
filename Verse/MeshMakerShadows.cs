using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MeshMakerShadows
{
	private static List<Vector3> vertsList = new List<Vector3>();

	private static List<Color32> colorsList = new List<Color32>();

	private static List<int> trianglesList = new List<int>();

	private static readonly Color32 LowVertexColor = new Color32((byte)0, (byte)0, (byte)0, (byte)0);

	public static Mesh NewShadowMesh(float baseWidth, float baseHeight, float tallness)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Expected O, but got Unknown
		Color32 item = default(Color32);
		((Color32)(ref item))._002Ector(byte.MaxValue, (byte)0, (byte)0, (byte)(255f * tallness));
		float num = baseWidth / 2f;
		float num2 = baseHeight / 2f;
		vertsList.Clear();
		colorsList.Clear();
		trianglesList.Clear();
		vertsList.Add(new Vector3(0f - num, 0f, 0f - num2));
		vertsList.Add(new Vector3(0f - num, 0f, num2));
		vertsList.Add(new Vector3(num, 0f, num2));
		vertsList.Add(new Vector3(num, 0f, 0f - num2));
		colorsList.Add(LowVertexColor);
		colorsList.Add(LowVertexColor);
		colorsList.Add(LowVertexColor);
		colorsList.Add(LowVertexColor);
		trianglesList.Add(0);
		trianglesList.Add(1);
		trianglesList.Add(2);
		trianglesList.Add(0);
		trianglesList.Add(2);
		trianglesList.Add(3);
		int count = vertsList.Count;
		vertsList.Add(new Vector3(0f - num, 0f, 0f - num2));
		colorsList.Add(item);
		vertsList.Add(new Vector3(0f - num, 0f, num2));
		colorsList.Add(item);
		trianglesList.Add(0);
		trianglesList.Add(count);
		trianglesList.Add(count + 1);
		trianglesList.Add(0);
		trianglesList.Add(count + 1);
		trianglesList.Add(1);
		int count2 = vertsList.Count;
		vertsList.Add(new Vector3(num, 0f, num2));
		colorsList.Add(item);
		vertsList.Add(new Vector3(num, 0f, 0f - num2));
		colorsList.Add(item);
		trianglesList.Add(2);
		trianglesList.Add(count2);
		trianglesList.Add(count2 + 1);
		trianglesList.Add(count2 + 1);
		trianglesList.Add(3);
		trianglesList.Add(2);
		int count3 = vertsList.Count;
		vertsList.Add(new Vector3(0f - num, 0f, 0f - num2));
		colorsList.Add(item);
		vertsList.Add(new Vector3(num, 0f, 0f - num2));
		colorsList.Add(item);
		trianglesList.Add(0);
		trianglesList.Add(3);
		trianglesList.Add(count3);
		trianglesList.Add(3);
		trianglesList.Add(count3 + 1);
		trianglesList.Add(count3);
		return new Mesh
		{
			name = "NewShadowMesh()",
			vertices = vertsList.ToArray(),
			colors32 = colorsList.ToArray(),
			triangles = trianglesList.ToArray(),
			bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 1000f))
		};
	}
}
