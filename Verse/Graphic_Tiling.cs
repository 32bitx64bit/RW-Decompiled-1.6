using UnityEngine;

namespace Verse;

public class Graphic_Tiling : Graphic_WithPropertyBlock
{
	public Vector2 Tiling;

	public Graphic_Tiling WithTiling(Vector2 tiling)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Tiling = tiling;
		return this;
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		propertyBlock.SetVector(ShaderPropertyIDs.Tiling, Vector4.op_Implicit(Tiling));
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}
}
