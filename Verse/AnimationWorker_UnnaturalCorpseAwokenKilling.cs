using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class AnimationWorker_UnnaturalCorpseAwokenKilling : AnimationWorker_Keyframes
{
	private const float LensFlareScale = 0.7f;

	private const float LensFlareAnimPeriod = 30f;

	private const float Layer_AboveHead = 80f;

	private const float PxToUnitScale = 100f;

	private static readonly List<Vector3> EyePositionsPxNS = new List<Vector3>
	{
		new Vector3(-8f, 0f, -3f),
		new Vector3(12f, 0f, -5f)
	};

	private static readonly List<Vector3> EyePositionsPxEW = new List<Vector3>
	{
		new Vector3(19f, 0f, 9f),
		new Vector3(18f, 0f, -7f)
	};

	private static readonly SimpleCurve LensFlareOpacity = new SimpleCurve
	{
		new CurvePoint(0f, 0.4f),
		new CurvePoint(0.5f, 0.8f),
		new CurvePoint(1f, 0.4f)
	};

	private Material lensFlareMat;

	private Material LensFlareMat
	{
		get
		{
			if ((Object)(object)lensFlareMat == (Object)null)
			{
				lensFlareMat = MaterialPool.MatFrom("Things/Pawn/Revenant/Revenant_LensFlare", ShaderDatabase.MoteGlow);
			}
			return lensFlareMat;
		}
	}

	public override void PostDraw(AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms, Matrix4x4 matrix)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector3 item in (parms.facing == Rot4.North || parms.facing == Rot4.South) ? EyePositionsPxNS : EyePositionsPxEW)
		{
			Vector3 current = item;
			current.y = 1f;
			if (parms.facing == Rot4.North || parms.facing == Rot4.West)
			{
				current.x *= -1f;
			}
			if (parms.facing == Rot4.North)
			{
				current.y = -1f;
			}
			float x = (float)(Find.TickManager.TicksGame - node.tree.animationStartTick) % 30f / 30f;
			LensFlareMat.SetColor(ShaderPropertyIDs.Color, new Color(1f, 1f, 1f, LensFlareOpacity.Evaluate(x)));
			GenDraw.DrawMeshNowOrLater(MeshPool.GridPlane(Vector2.one * 0.7f), matrix * Matrix4x4.Translate(current / 100f), LensFlareMat, parms.DrawNow);
		}
		if (node.tree.pawn.mindState.enemyTarget is Pawn pawn)
		{
			Rot4 rotation = ((pawn.GetPosture() == PawnPosture.Standing) ? Rot4.North : pawn.Drawer.renderer.LayingFacing());
			Vector3 val = pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			Vector3 loc = (pawn.DrawPos + val).WithYOffset(PawnRenderUtility.AltitudeForLayer(80f));
			Quaternion quat = Quaternion.Euler(Vector3.up * pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			GenDraw.DrawMeshNowOrLater(MeshPool.GridPlane(Vector2.one * 2f), loc, quat, LensFlareMat, parms.DrawNow);
		}
	}
}
