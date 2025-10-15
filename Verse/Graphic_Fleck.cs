using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Graphic_Fleck : Graphic_Single
{
	protected virtual bool AllowInstancing => true;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		throw new NotImplementedException();
	}

	public virtual void DrawFleck(FleckDrawData drawData, DrawBatch batch)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Color value;
		if (drawData.overrideColor.HasValue)
		{
			value = drawData.overrideColor.Value;
		}
		else
		{
			float alpha = drawData.alpha;
			if (alpha <= 0f)
			{
				if (drawData.propertyBlock != null)
				{
					batch.ReturnPropertyBlock(drawData.propertyBlock);
				}
				return;
			}
			value = base.Color * drawData.color;
			value.a *= alpha;
		}
		Vector3 scale = drawData.scale;
		scale.x *= data.drawSize.x;
		scale.z *= data.drawSize.y;
		Mesh mesh = MeshPool.plane10;
		float num = drawData.rotation;
		if (scale.x < 0f && scale.y >= 0f)
		{
			scale.x = 0f - scale.x;
			mesh = MeshPool.plane10Flip;
		}
		else if (scale.x >= 0f && scale.y < 0f)
		{
			scale.y = 0f - scale.y;
			mesh = MeshPool.plane10Flip;
			num += 180f;
		}
		Matrix4x4 matrix = default(Matrix4x4);
		((Matrix4x4)(ref matrix)).SetTRS(drawData.pos, Quaternion.AngleAxis(num, Vector3.up), scale);
		Material matSingle = MatSingle;
		batch.DrawMesh(mesh, matrix, matSingle, drawData.drawLayer, value, data.renderInstanced && AllowInstancing, drawData.propertyBlock);
	}

	public override string ToString()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[7]
		{
			"Fleck(path=",
			path,
			", shader=",
			((object)base.Shader)?.ToString(),
			", color=",
			null,
			null
		};
		Color val = color;
		obj[5] = ((object)(Color)(ref val)).ToString();
		obj[6] = ", colorTwo=unsupported)";
		return string.Concat(obj);
	}
}
