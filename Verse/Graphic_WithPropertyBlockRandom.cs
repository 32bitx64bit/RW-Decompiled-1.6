using UnityEngine;

namespace Verse;

public abstract class Graphic_WithPropertyBlockRandom : Graphic_Random
{
	protected MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	protected override void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(loc, quat, new Vector3(drawSize.x, 1f, drawSize.y)), mat, 0, (Camera)null, 0, propertyBlock);
	}
}
