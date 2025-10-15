using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Spastic : PawnRenderNodeWorker
{
	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.OffsetFor(node, parms, out pivot);
		if (node is PawnRenderNode_Spastic pawnRenderNode_Spastic && pawnRenderNode_Spastic.CheckAndDoSpasm(parms, out var dat, out var progress))
		{
			val += Vector3.Lerp(dat.offsetStart, dat.offsetTarget, progress);
		}
		return val;
	}

	public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = base.RotationFor(node, parms);
		if (!(node is PawnRenderNode_Spastic pawnRenderNode_Spastic))
		{
			return val;
		}
		float num = 0f;
		if (node.Props is PawnRenderNodeProperties_Spastic { rotateFacing: not false })
		{
			num += parms.facing.AsAngle;
		}
		if (pawnRenderNode_Spastic.CheckAndDoSpasm(parms, out var dat, out var progress))
		{
			num += Mathf.Lerp(dat.rotationStart, dat.rotationTarget, progress);
		}
		return val * num.ToQuat();
	}

	public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.ScaleFor(node, parms);
		if (node is PawnRenderNode_Spastic pawnRenderNode_Spastic && pawnRenderNode_Spastic.CheckAndDoSpasm(parms, out var dat, out var progress))
		{
			val *= Mathf.Lerp(dat.scaleStart, dat.scaleTarget, progress);
			val.y = 1f;
		}
		return val;
	}
}
