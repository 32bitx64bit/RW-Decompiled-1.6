using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_ActivityMask : Graphic_WithPropertyBlock
{
	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		CompActivity compActivity = thing.TryGetComp<CompActivity>();
		if (compActivity == null)
		{
			Log.ErrorOnce(thingDef.defName + ": Graphic_ActivityMask requires CompActivity.", 6134621);
			return;
		}
		Color val = colorTwo;
		val.a = Mathf.Clamp01(compActivity.ActivityLevel);
		propertyBlock.SetColor(ShaderPropertyIDs.ColorTwo, val);
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}
}
