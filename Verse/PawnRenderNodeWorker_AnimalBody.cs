using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_AnimalBody : PawnRenderNodeWorker
{
	protected override GraphicStateDef GetGraphicState(PawnRenderNode node, PawnDrawParms parms)
	{
		if (node.tree.currentAnimation == null && parms.pawn.DrawNonHumanlikeSwimmingGraphic)
		{
			return GraphicStateDefOf.Swimming;
		}
		if (node.tree.currentAnimation == null && parms.pawn.DrawNonHumanlikeStationaryGraphic)
		{
			return GraphicStateDefOf.Stationary;
		}
		return base.GetGraphicState(node, parms);
	}

	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return base.OffsetFor(node, parms, out pivot) + node.PrimaryGraphic.DrawOffset(parms.facing);
	}
}
