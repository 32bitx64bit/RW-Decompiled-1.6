using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_PawnBodySilhouette : Graphic_Mote
{
	private GraphicRequest request;

	private Pawn lastPawn;

	private Rot4 lastFacing;

	private Material bodyMaterial;

	private Material headMaterial;

	protected override bool ForcePropertyBlock => true;

	public override void Init(GraphicRequest req)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		data = req.graphicData;
		path = req.path;
		maskPath = req.maskPath;
		color = req.color;
		colorTwo = req.colorTwo;
		drawSize = req.drawSize;
		request = req;
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		Mote mote = (Mote)thing;
		Color val = color;
		val.a *= mote.Alpha;
		Corpse obj = mote.link1.Target.Thing as Corpse;
		Pawn pawn = mote.link1.Target.Thing as Pawn;
		Pawn pawn2 = obj?.InnerPawn ?? pawn;
		if (pawn2 == null)
		{
			pawn2 = lastPawn;
		}
		PawnRenderer pawnRenderer = pawn2?.Drawer?.renderer;
		if (pawnRenderer?.renderTree == null || !pawnRenderer.renderTree.Resolved)
		{
			return;
		}
		Rot4 rot2 = ((pawn2.GetPosture() == PawnPosture.Standing) ? pawn2.Rotation : pawnRenderer.LayingFacing());
		Vector3 val2 = pawn2.DrawPos;
		Building_Bed building_Bed = pawn2.CurrentBed();
		if (building_Bed != null)
		{
			Rot4 rotation = building_Bed.Rotation;
			rotation.AsInt += 2;
			val2 -= rotation.FacingCell.ToVector3() * (pawn2.story.bodyType.bedOffset + pawn2.Drawer.renderer.BaseHeadOffsetAt(Rot4.South).z);
		}
		PawnPosture posture = pawn2.GetPosture();
		val2.y = mote.def.Altitude;
		if (lastPawn != pawn2 || lastFacing != rot2)
		{
			bodyMaterial = MakeMatFrom(request, pawnRenderer.BodyGraphic.MatAt(rot2).mainTexture);
		}
		Mesh val3 = ((!pawn2.RaceProps.Humanlike) ? pawnRenderer.BodyGraphic.MeshAt(rot2) : HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn2).MeshAt(rot2));
		bodyMaterial.SetVector(ShaderPropertyIDs.PawnCenterWorld, new Vector4(val2.x, val2.z, 0f, 0f));
		Material obj2 = bodyMaterial;
		int pawnDrawSizeWorld = ShaderPropertyIDs.PawnDrawSizeWorld;
		Bounds bounds = val3.bounds;
		float x = ((Bounds)(ref bounds)).size.x;
		bounds = val3.bounds;
		obj2.SetVector(pawnDrawSizeWorld, new Vector4(x, ((Bounds)(ref bounds)).size.z, 0f, 0f));
		bodyMaterial.SetFloat(ShaderPropertyIDs.AgeSecs, mote.AgeSecs);
		bodyMaterial.SetColor(ShaderPropertyIDs.Color, val);
		Quaternion val4 = Quaternion.AngleAxis((posture == PawnPosture.Standing) ? 0f : pawnRenderer.BodyAngle(PawnRenderFlags.None), Vector3.up);
		if (building_Bed == null || building_Bed.def.building.bed_showSleeperBody)
		{
			GenDraw.DrawMeshNowOrLater(val3, val2, val4, bodyMaterial, drawNow: false);
		}
		if (pawn2.RaceProps.Humanlike)
		{
			bool flag = true;
			if (lastPawn != pawn2 || lastFacing != rot2)
			{
				if (pawnRenderer.HeadGraphic == null)
				{
					flag = false;
					headMaterial = null;
				}
				else
				{
					headMaterial = MakeMatFrom(request, pawnRenderer.HeadGraphic.MatAt(rot2).mainTexture);
				}
			}
			if (flag && (Object)(object)headMaterial != (Object)null)
			{
				Vector3 val5 = val4 * pawnRenderer.BaseHeadOffsetAt(rot2) + new Vector3(0f, 0.001f, 0f);
				Mesh val6 = HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn2).MeshAt(rot2);
				headMaterial.SetVector(ShaderPropertyIDs.PawnCenterWorld, new Vector4(val2.x, val2.z, 0f, 0f));
				Material obj3 = headMaterial;
				int pawnDrawSizeWorld2 = ShaderPropertyIDs.PawnDrawSizeWorld;
				bounds = val6.bounds;
				float x2 = ((Bounds)(ref bounds)).size.x;
				bounds = val3.bounds;
				obj3.SetVector(pawnDrawSizeWorld2, new Vector4(x2, ((Bounds)(ref bounds)).size.z, 0f, 0f));
				headMaterial.SetFloat(ShaderPropertyIDs.AgeSecs, mote.AgeSecs);
				headMaterial.SetColor(ShaderPropertyIDs.Color, val);
				GenDraw.DrawMeshNowOrLater(val6, val2 + val5, val4, headMaterial, drawNow: false);
			}
		}
		if (pawn2 != null)
		{
			lastPawn = pawn2;
		}
		lastFacing = rot2;
	}

	private Material MakeMatFrom(GraphicRequest req, Texture mainTex)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		MaterialRequest req2 = default(MaterialRequest);
		req2.mainTex = mainTex;
		req2.shader = req.shader;
		req2.color = color;
		req2.colorTwo = colorTwo;
		req2.renderQueue = req.renderQueue;
		req2.shaderParameters = req.shaderParameters;
		return MaterialPool.MatFrom(req2);
	}
}
