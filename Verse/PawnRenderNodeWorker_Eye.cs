using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Eye : PawnRenderNodeWorker_FlipWhenCrawling
{
	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.OffsetFor(node, parms, out pivot);
		if (TryGetWoundAnchor(node.Props.anchorTag, parms, out var anchor))
		{
			PawnDrawUtility.CalcAnchorData(parms.pawn, anchor, parms.facing, out var anchorOffset, out var _);
			val += anchorOffset;
		}
		return val;
	}

	public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return base.ScaleFor(node, parms) * parms.pawn.ageTracker.CurLifeStage.eyeSizeFactor.GetValueOrDefault(1f);
	}

	protected bool TryGetWoundAnchor(string anchorTag, PawnDrawParms parms, out BodyTypeDef.WoundAnchor anchor)
	{
		anchor = null;
		if (anchorTag.NullOrEmpty())
		{
			return false;
		}
		List<BodyTypeDef.WoundAnchor> woundAnchors = parms.pawn.story.bodyType.woundAnchors;
		for (int i = 0; i < woundAnchors.Count; i++)
		{
			BodyTypeDef.WoundAnchor woundAnchor = woundAnchors[i];
			if (woundAnchor.tag == anchorTag)
			{
				Rot4? rotation = woundAnchor.rotation;
				Rot4 facing = parms.facing;
				if (rotation.HasValue && (!rotation.HasValue || rotation.GetValueOrDefault() == facing) && (parms.facing == Rot4.South || woundAnchor.narrowCrown.GetValueOrDefault() == parms.pawn.story.headType.narrow))
				{
					anchor = woundAnchor;
					return true;
				}
			}
		}
		return false;
	}
}
