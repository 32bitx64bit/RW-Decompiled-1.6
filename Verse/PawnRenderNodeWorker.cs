using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker
{
	private static readonly List<PawnRenderSubWorker> EmptySubWorkers = new List<PawnRenderSubWorker>();

	public virtual bool ShouldListOnGraph(PawnRenderNode node, PawnDrawParms parms)
	{
		return true;
	}

	public virtual bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!node.Props.rotDrawMode.HasFlag(parms.rotDrawMode))
		{
			return false;
		}
		if (node.Props.visibleFacing != null && !node.Props.visibleFacing.Contains(parms.facing))
		{
			return false;
		}
		if (node.Props.skipFlag != RenderSkipFlagDefOf.None && parms.skipFlags.HasFlag(node.Props.skipFlag))
		{
			return false;
		}
		if (node.bodyPart?.visibleHediffRots != null && !node.bodyPart.visibleHediffRots.Contains(parms.facing))
		{
			return false;
		}
		Rot4 rot = Rot4.Invalid;
		if (node.Props.side != 0)
		{
			rot = ((node.Props.side == PawnRenderNodeProperties.Side.Left) ? Rot4.East : Rot4.West);
		}
		if (node.bodyPart != null && node.bodyPart.def.IsMirroredPart)
		{
			rot = (node.bodyPart.flipGraphic ? Rot4.East : Rot4.West);
		}
		if (rot != Rot4.Invalid && node.Props.flipGraphic && rot.IsHorizontal)
		{
			rot = rot.Opposite;
		}
		if (parms.facing == rot)
		{
			return false;
		}
		if (node.Props.linkedBodyPartsGroup != null && !parms.pawn.health.hediffSet.GetNotMissingParts().Any((BodyPartRecord x) => x.groups.NotNullAndContains(node.Props.linkedBodyPartsGroup)))
		{
			return false;
		}
		return node.DebugEnabled;
	}

	public virtual void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		node.TryAnimationPostDraw(parms, matrix);
	}

	public virtual Material GetFinalizedMaterial(PawnRenderNode node, PawnDrawParms parms)
	{
		Material material = GetMaterial(node, parms);
		foreach (PawnRenderSubWorker subWorker in node.Props.SubWorkers)
		{
			subWorker.EditMaterial(node, parms, ref material);
		}
		return material;
	}

	public virtual void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
	{
		Material finalizedMaterial = GetFinalizedMaterial(node, parms);
		if (!((Object)(object)finalizedMaterial == (Object)null))
		{
			Mesh mesh = node.GetMesh(parms);
			if (!((Object)(object)mesh == (Object)null))
			{
				requests.Add(new PawnGraphicDrawRequest(node, mesh, finalizedMaterial));
			}
		}
	}

	protected virtual Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
	{
		GraphicStateDef graphicState = GetGraphicState(node, parms);
		if (graphicState == null)
		{
			return node.PrimaryGraphic;
		}
		return node.GraphicForState(graphicState);
	}

	protected virtual GraphicStateDef GetGraphicState(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationGraphicState(parms, out var state))
		{
			return state;
		}
		return null;
	}

	public virtual MaterialPropertyBlock GetMaterialPropertyBlock(PawnRenderNode node, Material material, PawnDrawParms parms)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (GetGraphic(node, parms) == null)
		{
			return null;
		}
		MaterialPropertyBlock matPropBlock = node.MatPropBlock;
		if (parms.Statue)
		{
			matPropBlock.SetColor(ShaderPropertyIDs.Color, parms.statueColor.Value);
		}
		else
		{
			matPropBlock.SetColor(ShaderPropertyIDs.Color, parms.tint * material.color);
		}
		if ((Object)(object)material.shader == (Object)(object)ShaderDatabase.CutoutWithOverlay)
		{
			if (parms.pawn.Faction != null && (Object)(object)material.GetMaskTexture() != (Object)null)
			{
				PawnRenderUtility.SetMatPropBlockOverlay(matPropBlock, parms.pawn.Faction.AllegianceColor);
			}
			else
			{
				PawnRenderUtility.SetMatPropBlockOverlay(matPropBlock, Color.white, 0f);
			}
		}
		return matPropBlock;
	}

	protected virtual Material GetMaterial(PawnRenderNode node, PawnDrawParms parms)
	{
		Graphic graphic = GetGraphic(node, parms);
		if (graphic == null)
		{
			return null;
		}
		if (node.Props.flipGraphic && parms.facing.IsHorizontal)
		{
			parms.facing = parms.facing.Opposite;
		}
		Material val = graphic.NodeGetMat(parms);
		if ((Object)(object)val != (Object)null && !parms.Portrait && parms.flags.FlagSet(PawnRenderFlags.Invisible))
		{
			val = InvisibilityMatPool.GetInvisibleMat(val);
		}
		return val;
	}

	public virtual void PreDraw(PawnRenderNode node, Material mat, PawnDrawParms parms)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		node.MatPropBlock.SetColor(ShaderPropertyIDs.Color, parms.tint * mat.color);
	}

	public virtual Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		Vector3 anchorOffset = Vector3.zero;
		pivot = PivotFor(node, parms);
		if (node.Props.drawData != null)
		{
			if (node.Props.drawData.useBodyPartAnchor)
			{
				if (node.bodyPart == null)
				{
					Log.ErrorOnce($"Attempted to use a body part anchor but no body-part record has been assigned to this node {node}", node.GetHashCode());
					return anchorOffset;
				}
				foreach (BodyTypeDef.WoundAnchor item in PawnDrawUtility.FindAnchors(parms.pawn, node.bodyPart))
				{
					if (PawnDrawUtility.AnchorUsable(parms.pawn, item, parms.facing))
					{
						PawnDrawUtility.CalcAnchorData(parms.pawn, item, parms.facing, out anchorOffset, out var _);
					}
				}
			}
			Vector3 val = node.Props.drawData.OffsetForRot(parms.facing);
			if (node.Props.drawData.scaleOffsetByBodySize && parms.pawn.story != null)
			{
				Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
				float num = (bodyGraphicScale.x + bodyGraphicScale.y) / 2f;
				val *= num;
			}
			anchorOffset += val;
		}
		anchorOffset += node.DebugOffset;
		if (!parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationOffset(parms, out var offset))
		{
			anchorOffset += offset;
		}
		return anchorOffset;
	}

	protected virtual Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		if (node.Props.drawData != null)
		{
			val -= (node.Props.drawData.PivotForRot(parms.facing) - DrawData.PivotCenter).ToVector3();
		}
		if (node.tree.TryGetAnimationPartForNode(node, out var animationPart))
		{
			val = (animationPart.pivot - DrawData.PivotCenter).ToVector3();
		}
		if (node.debugPivotOffset != DrawData.PivotCenter)
		{
			val.x += node.debugPivotOffset.x - DrawData.PivotCenter.x;
			val.z += node.debugPivotOffset.y - DrawData.PivotCenter.y;
		}
		return val;
	}

	public float AltitudeFor(PawnRenderNode node, PawnDrawParms parms)
	{
		float layer = LayerFor(node, parms);
		foreach (PawnRenderSubWorker subWorker in node.Props.SubWorkers)
		{
			subWorker.TransformLayer(node, parms, ref layer);
		}
		return PawnRenderUtility.AltitudeForLayer(layer);
	}

	public virtual float LayerFor(PawnRenderNode node, PawnDrawParms parms)
	{
		return (node.Props.drawData?.LayerForRot(parms.facing, node.Props.baseLayer) ?? node.Props.baseLayer) + node.debugLayerOffset;
	}

	public virtual Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		float num = node.DebugAngleOffset;
		if (node.Props.drawData != null)
		{
			num += node.Props.drawData.RotationOffsetForRot(parms.facing);
		}
		if (!parms.flags.FlagSet(PawnRenderFlags.Portrait) && node.TryGetAnimationRotation(parms, out var offset))
		{
			num += offset;
		}
		if (node.FlipGraphic(parms))
		{
			num *= -1f;
		}
		return Quaternion.AngleAxis(num, Vector3.up);
	}

	public virtual Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.one;
		val.x *= node.Props.drawSize.x * node.debugScale;
		val.z *= node.Props.drawSize.y * node.debugScale;
		if (!parms.flags.FlagSet(PawnRenderFlags.Portrait))
		{
			if (node.TryGetAnimationScale(parms, out var offset))
			{
				val = val.ScaledBy(offset);
			}
			GraphicStateDef graphicState = GetGraphicState(node, parms);
			if (graphicState != null && graphicState.TryGetDefaultGraphic(out var graphic))
			{
				val = val.ScaledBy(new Vector3(graphic.drawSize.x, 1f, graphic.drawSize.y));
			}
		}
		if (node.Props.drawData != null)
		{
			val *= node.Props.drawData.ScaleFor(parms.pawn);
		}
		return val;
	}

	public static List<PawnRenderSubWorker> GetSubWorkerList(List<Type> types)
	{
		if (types == null)
		{
			return EmptySubWorkers;
		}
		List<PawnRenderSubWorker> list = new List<PawnRenderSubWorker>();
		foreach (Type type in types)
		{
			list.Add(GenWorker<PawnRenderSubWorker>.Get(type));
		}
		return list;
	}
}
