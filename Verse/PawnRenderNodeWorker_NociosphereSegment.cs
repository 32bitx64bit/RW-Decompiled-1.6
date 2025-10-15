using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_NociosphereSegment : PawnRenderNodeWorker
{
	private static readonly Color LineColor = new Color(0.89f, 0.21f, 0.13f);

	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.OffsetFor(node, parms, out pivot);
		if (!(node.Props is PawnRenderNodeProperties_NociosphereSegment pawnRenderNodeProperties_NociosphereSegment))
		{
			return val;
		}
		if (!parms.pawn.TryGetComp<CompNociosphere>(out var comp))
		{
			return val;
		}
		val += pawnRenderNodeProperties_NociosphereSegment.offset.ToVector3() / 6f;
		Vector3 val2 = val * comp.segScale;
		return new Vector3(val2.x, 0f, val2.z);
	}

	public override MaterialPropertyBlock GetMaterialPropertyBlock(PawnRenderNode node, Material material, PawnDrawParms parms)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		MaterialPropertyBlock materialPropertyBlock = base.GetMaterialPropertyBlock(node, material, parms);
		if (!parms.pawn.TryGetComp<CompActivity>(out var comp))
		{
			return materialPropertyBlock;
		}
		Color lineColor = LineColor;
		lineColor.a = Mathf.Clamp01(comp.ActivityLevel);
		materialPropertyBlock.SetColor(ShaderPropertyIDs.ColorTwo, lineColor);
		return materialPropertyBlock;
	}
}
