using UnityEngine;

namespace Verse;

public class Graphic_Multi_Mote : Graphic_Multi_AgeSecs
{
	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (thing is Mote mote)
		{
			propertyBlock.SetColor(ShaderPropertyIDs.Color, mote.instanceColor);
		}
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}

	protected override void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(mesh, loc, quat, mat, 0, (Camera)null, 0, propertyBlock);
	}
}
