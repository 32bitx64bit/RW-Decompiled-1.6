using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Hediff : PawnRenderNodeWorker
{
	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 anchorOffset = Vector3.zero;
		if (node.Props.drawData != null && node.hediff != null && node.Props.drawData.useBodyPartAnchor)
		{
			foreach (BodyTypeDef.WoundAnchor item in PawnDrawUtility.FindAnchors(parms.pawn, node.hediff.Part))
			{
				if (PawnDrawUtility.AnchorUsable(parms.pawn, item, parms.facing))
				{
					PawnDrawUtility.CalcAnchorData(parms.pawn, item, parms.facing, out anchorOffset, out var _);
				}
			}
		}
		anchorOffset += base.OffsetFor(node, parms, out pivot);
		DrawData drawData = node.Props.drawData;
		if (drawData != null && !drawData.useBodyPartAnchor && (node.hediff?.Part?.flipGraphic).GetValueOrDefault())
		{
			anchorOffset.x *= -1f;
		}
		return anchorOffset;
	}
}
