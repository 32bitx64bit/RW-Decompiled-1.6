using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_Carried : PawnRenderNodeWorker
{
	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!base.CanDrawNow(node, parms))
		{
			return false;
		}
		if (parms.Portrait || parms.pawn.Dead || !parms.pawn.Spawned)
		{
			return false;
		}
		return true;
	}

	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = base.OffsetFor(node, parms, out pivot);
		result.y = AltitudeFor(node, parms);
		return result;
	}

	public override void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
	{
		requests.Add(new PawnGraphicDrawRequest(node));
	}

	public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pivot;
		Vector3 val = parms.matrix.Position() + OffsetFor(node, parms, out pivot);
		if (parms.pawn.carryTracker?.CarriedThing != null)
		{
			PawnRenderUtility.DrawCarriedThing(parms.pawn, val, parms.pawn.carryTracker.CarriedThing);
		}
		else
		{
			PawnRenderUtility.DrawEquipmentAndApparelExtras(parms.pawn, val, parms.facing, parms.flags);
		}
	}
}
