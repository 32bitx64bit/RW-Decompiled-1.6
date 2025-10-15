using UnityEngine;

namespace Verse;

public class Graphic_Multi_AgeSecs : Graphic_Multi
{
	public MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	public float AgeSecs(Thing thing)
	{
		return (float)(Find.TickManager.TicksGame - thing.TickSpawned) / 60f;
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Material val = MatAt(rot, thing);
		if (thing != null)
		{
			val.SetFloat(ShaderPropertyIDs.AgeSecs, AgeSecs(thing));
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
