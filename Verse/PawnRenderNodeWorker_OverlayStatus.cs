using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_OverlayStatus : PawnRenderNodeWorker_Overlay
{
	protected override PawnOverlayDrawer OverlayDrawer(Pawn pawn)
	{
		return null;
	}

	public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (base.CanDrawNow(node, parms) && !parms.Portrait)
		{
			return !parms.Cache;
		}
		return false;
	}

	public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pivot;
		Vector3 val = OffsetFor(node, parms, out pivot);
		Quaternion quat = RotationFor(node, parms);
		if (node.Props.overlayLayer == PawnOverlayDrawer.OverlayLayer.Head)
		{
			val += parms.pawn.Drawer.renderer.BaseHeadOffsetAt(Rot4.North);
		}
		parms.pawn.Drawer.renderer.StatusOverlays.RenderStatusOverlays(val, quat, mesh);
	}
}
