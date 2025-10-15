using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_FadesInOut : Graphic_WithPropertyBlock
{
	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		CompFadesInOut compFadesInOut = thing.TryGetComp<CompFadesInOut>();
		if (compFadesInOut == null)
		{
			Log.ErrorOnce(thingDef.defName + ": Graphic_FadesInOut requires CompFadesInOut.", 5643893);
			return;
		}
		propertyBlock.SetColor(ShaderPropertyIDs.Color, new Color(color.r, color.g, color.b, color.a * compFadesInOut.Opacity()));
		base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}
}
