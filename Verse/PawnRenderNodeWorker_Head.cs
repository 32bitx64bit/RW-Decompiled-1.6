using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Head : PawnRenderNodeWorker_FlipWhenCrawling
{
	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (base.CanDrawNow(node, parms))
		{
			return !parms.flags.FlagSet(PawnRenderFlags.HeadStump);
		}
		return false;
	}

	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = base.OffsetFor(node, parms, out pivot) + parms.pawn.Drawer.renderer.BaseHeadOffsetAt(parms.facing);
		if (parms.pawn.story.headType.narrow && node.Props.narrowCrownHorizontalOffset != 0f && parms.facing.IsHorizontal)
		{
			if (parms.facing == Rot4.East)
			{
				result.x -= node.Props.narrowCrownHorizontalOffset;
			}
			else if (parms.facing == Rot4.West)
			{
				result.x += node.Props.narrowCrownHorizontalOffset;
			}
			result.z -= node.Props.narrowCrownHorizontalOffset;
		}
		if (!parms.Portrait && parms.swimming)
		{
			result.z -= 0.5f;
		}
		return result;
	}

	public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = base.RotationFor(node, parms);
		if (!parms.Portrait && parms.pawn.Crawling)
		{
			val *= PawnRenderUtility.CrawlingHeadAngle(parms.facing).ToQuat();
			if (parms.flipHead)
			{
				val *= 180f.ToQuat();
			}
		}
		if (parms.pawn.IsShambler && parms.pawn.mutant != null && parms.pawn.mutant.HasTurned && !parms.pawn.Dead)
		{
			val *= Quaternion.Euler(Vector3.up * ((parms.pawn.mutant.Hediff as Hediff_Shambler)?.headRotation ?? 0f));
		}
		return val;
	}
}
