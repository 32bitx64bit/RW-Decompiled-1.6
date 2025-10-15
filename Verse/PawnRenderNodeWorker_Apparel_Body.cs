using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Apparel_Body : PawnRenderNodeWorker_Body
{
	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!base.CanDrawNow(node, parms))
		{
			return false;
		}
		if (!parms.flags.FlagSet(PawnRenderFlags.Clothes))
		{
			return false;
		}
		return true;
	}

	public override Vector3 OffsetFor(PawnRenderNode n, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = base.OffsetFor(n, parms, out pivot);
		PawnRenderNode_Apparel pawnRenderNode_Apparel = (PawnRenderNode_Apparel)n;
		if (pawnRenderNode_Apparel.apparel.def.apparel.wornGraphicData != null && pawnRenderNode_Apparel.apparel.RenderAsPack())
		{
			Vector2 val = pawnRenderNode_Apparel.apparel.def.apparel.wornGraphicData.BeltOffsetAt(parms.facing, parms.pawn.story.bodyType);
			result.x += val.x;
			result.z += val.y;
		}
		return result;
	}

	public override Vector3 ScaleFor(PawnRenderNode n, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = base.ScaleFor(n, parms);
		PawnRenderNode_Apparel pawnRenderNode_Apparel = (PawnRenderNode_Apparel)n;
		if (pawnRenderNode_Apparel.apparel.def.apparel.wornGraphicData != null && pawnRenderNode_Apparel.apparel.RenderAsPack())
		{
			Vector2 val = pawnRenderNode_Apparel.apparel.def.apparel.wornGraphicData.BeltScaleAt(parms.facing, parms.pawn.story.bodyType);
			result.x *= val.x;
			result.z *= val.y;
		}
		return result;
	}

	public override float LayerFor(PawnRenderNode n, PawnDrawParms parms)
	{
		if (parms.flipHead && n.Props.oppositeFacingLayerWhenFlipped)
		{
			PawnDrawParms parms2 = parms;
			parms2.facing = parms.facing.Opposite;
			parms2.flipHead = false;
			return base.LayerFor(n, parms2);
		}
		return base.LayerFor(n, parms);
	}
}
