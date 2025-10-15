using LudeonTK;
using UnityEngine;

namespace Verse;

public static class Printer_Shadow
{
	private static readonly Color32 LowVertexColor = new Color32((byte)0, (byte)0, (byte)0, (byte)0);

	[TweakValue("Graphics_Shadow", -5f, 5f)]
	private static float GlobalShadowSizeOffsetX = 0f;

	[TweakValue("Graphics_Shadow", -5f, 5f)]
	private static float GlobalShadowSizeOffsetY = 0f;

	[TweakValue("Graphics_Shadow", -5f, 5f)]
	private static float GlobalShadowSizeOffsetZ = 0f;

	public static void PrintShadow(SectionLayer layer, Vector3 center, ShadowData shadow, Rot4 rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		PrintShadow(layer, center, shadow.volume, rotation);
	}

	public static void PrintShadow(SectionLayer layer, Vector3 center, Vector3 volume, Rot4 rotation)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		if (DebugViewSettings.drawShadows)
		{
			LayerSubMesh subMesh = layer.GetSubMesh(MatBases.SunShadowFade);
			Color32 item = default(Color32);
			((Color32)(ref item))._002Ector(byte.MaxValue, (byte)0, (byte)0, (byte)Mathf.Min(255f * (volume.y + GlobalShadowSizeOffsetY), 255f));
			Vector3 val = (volume + new Vector3(GlobalShadowSizeOffsetX, 0f, GlobalShadowSizeOffsetZ)).RotatedBy(rotation).Abs() / 2f;
			float x = center.x;
			float z = center.z;
			int count = subMesh.verts.Count;
			subMesh.verts.Add(new Vector3(x - val.x, 0f, z - val.z));
			subMesh.verts.Add(new Vector3(x - val.x, 0f, z + val.z));
			subMesh.verts.Add(new Vector3(x + val.x, 0f, z + val.z));
			subMesh.verts.Add(new Vector3(x + val.x, 0f, z - val.z));
			subMesh.colors.Add(LowVertexColor);
			subMesh.colors.Add(LowVertexColor);
			subMesh.colors.Add(LowVertexColor);
			subMesh.colors.Add(LowVertexColor);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 1);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count + 3);
			int count2 = subMesh.verts.Count;
			subMesh.verts.Add(new Vector3(x - val.x, 0f, z - val.z));
			subMesh.verts.Add(new Vector3(x - val.x, 0f, z + val.z));
			subMesh.colors.Add(item);
			subMesh.colors.Add(item);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count2);
			subMesh.tris.Add(count2 + 1);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count2 + 1);
			subMesh.tris.Add(count + 1);
			int count3 = subMesh.verts.Count;
			subMesh.verts.Add(new Vector3(x + val.x, 0f, z + val.z));
			subMesh.verts.Add(new Vector3(x + val.x, 0f, z - val.z));
			subMesh.colors.Add(item);
			subMesh.colors.Add(item);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count3);
			subMesh.tris.Add(count3 + 1);
			subMesh.tris.Add(count3 + 1);
			subMesh.tris.Add(count + 3);
			subMesh.tris.Add(count + 2);
			int count4 = subMesh.verts.Count;
			subMesh.verts.Add(new Vector3(x - val.x, 0f, z - val.z));
			subMesh.verts.Add(new Vector3(x + val.x, 0f, z - val.z));
			subMesh.colors.Add(item);
			subMesh.colors.Add(item);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 3);
			subMesh.tris.Add(count4);
			subMesh.tris.Add(count + 3);
			subMesh.tris.Add(count4 + 1);
			subMesh.tris.Add(count4);
		}
	}
}
