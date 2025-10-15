using UnityEngine;

namespace Verse;

public class Graphic_Single_SquashNStretch : Graphic_WithPropertyBlock
{
	private Vector4 snsProps;

	public float AgeSecs(Thing thing)
	{
		return (float)(Find.TickManager.TicksGame - thing.TickSpawned) / 60f;
	}

	public override void Init(GraphicRequest req)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.Init(req);
		snsProps = new Vector4(data.maxSnS.x, data.maxSnS.y, data.offsetSnS.x, data.offsetSnS.y);
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		mat.SetFloat(ShaderPropertyIDs.AgeSecs, AgeSecs(thing));
		propertyBlock.SetVector(ShaderPropertyIDs.SquashNStretch, snsProps);
		propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObject, (float)thing.thingIDNumber.HashOffset());
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}
}
