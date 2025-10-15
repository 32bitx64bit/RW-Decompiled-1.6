using UnityEngine;

namespace Verse;

public class Graphic_Single_AgeSecs : Graphic_Single
{
	public float AgeSecs(Thing thing)
	{
		return (float)(Find.TickManager.TicksGame - thing.TickSpawned) / 60f;
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		mat.SetFloat(ShaderPropertyIDs.AgeSecs, AgeSecs(thing));
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}
}
