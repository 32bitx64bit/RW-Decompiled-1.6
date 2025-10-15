using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class MapEdgeClipDrawer
{
	public static readonly Material ClipMat = SolidColorMaterials.NewSolidColorMaterial(new Color(0.1f, 0.1f, 0.1f), ShaderDatabase.MetaOverlay);

	public static readonly Material ClipMatMetalhell = SolidColorMaterials.NewSolidColorMaterial(new Color(0.03f, 0.04f, 0.04f), ShaderDatabase.MetaOverlay);

	private static readonly float ClipAltitude = AltitudeLayer.WorldClipper.AltitudeFor();

	private const float ClipSize = 500f;

	private static MaterialPropertyBlock vertPropertyBlock = new MaterialPropertyBlock();

	private static MaterialPropertyBlock horPropertyBlock = new MaterialPropertyBlock();

	public static void DrawClippers(Map map)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		if (map.DrawMapClippers)
		{
			Material mapEdgeMaterial = map.MapEdgeMaterial;
			IntVec3 size = map.Size;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(500f, 1f, (float)size.z);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(-250f, 0f, (float)size.z / 2f);
			horPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureScale, Vector4.op_Implicit(val));
			horPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureOffset, Vector4.op_Implicit(val2));
			Matrix4x4 val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(val2.WithYOffset(ClipAltitude), Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.plane10, val3, mapEdgeMaterial, 0, (Camera)null, 0, horPropertyBlock);
			((Vector3)(ref val2))._002Ector((float)size.x + 250f, 0f, (float)size.z / 2f);
			horPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureOffset, Vector4.op_Implicit(val2));
			val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(val2.WithYOffset(ClipAltitude), Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.plane10, val3, mapEdgeMaterial, 0, (Camera)null, 0, horPropertyBlock);
			((Vector3)(ref val))._002Ector(1000f, 1f, 500f);
			((Vector3)(ref val2))._002Ector((float)size.x / 2f, 0f, (float)size.z + 250f);
			vertPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureScale, Vector4.op_Implicit(val));
			vertPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureOffset, Vector4.op_Implicit(val2));
			val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(val2.WithYOffset(ClipAltitude), Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.plane10, val3, mapEdgeMaterial, 0, (Camera)null, 0, vertPropertyBlock);
			((Vector3)(ref val2))._002Ector((float)size.x / 2f, 0f, -250f);
			vertPropertyBlock.SetVector(ShaderPropertyIDs.MainTextureOffset, Vector4.op_Implicit(val2));
			val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(val2.WithYOffset(ClipAltitude), Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.plane10, val3, mapEdgeMaterial, 0, (Camera)null, 0, vertPropertyBlock);
		}
	}
}
