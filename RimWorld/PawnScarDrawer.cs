using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class PawnScarDrawer : PawnOverlayDrawer
{
	protected abstract string ScarTexturePath { get; }

	protected virtual float ScaleFactor => 1f;

	public PawnScarDrawer(Pawn pawn)
		: base(pawn)
	{
	}

	protected override void WriteCache(CacheKey key, PawnDrawParms parms, List<DrawCall> writeTarget)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		Rot4 pawnRot = key.pawnRot;
		Mesh bodyMesh = key.bodyMesh;
		OverlayLayer layer = key.layer;
		Graphic graphic = ((layer == OverlayLayer.Body) ? pawn.Drawer.renderer.BodyGraphic : pawn.Drawer.renderer.HeadGraphic);
		if (graphic == null)
		{
			return;
		}
		Rand.PushState(pawn.thingIDNumber * (int)(layer + 1));
		try
		{
			Mesh val = (((graphic.EastFlipped && pawnRot == Rot4.East) || (graphic.WestFlipped && pawnRot == Rot4.West)) ? MeshPool.GridPlaneFlip(Vector2.one) : MeshPool.GridPlane(Vector2.one));
			Bounds bounds = bodyMesh.bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			float num = ((Vector3)(ref size)).magnitude * ScaleFactor;
			bounds = val.bounds;
			Vector3 val2 = ((Bounds)(ref bounds)).size * num;
			Vector4 value = default(Vector4);
			((Vector4)(ref value))._002Ector(val2.x / size.x, val2.z / size.z);
			Material val3 = MaterialPool.MatFrom(ScarTexturePath, ShaderDatabase.Wound, Color.white);
			MaterialRequest req = default(MaterialRequest);
			req.maskTex = (Texture2D)graphic.MatAt(pawnRot).mainTexture;
			req.mainTex = val3.mainTexture;
			req.color = val3.color;
			req.shader = val3.shader;
			val3 = MaterialPool.MatFrom(req);
			Vector3 val4 = Rand.InsideUnitCircleVec3 / 2f;
			int rotation = Rand.Range(0, 360);
			writeTarget.Add(new DrawCall
			{
				overlayMat = val3,
				matrix = Matrix4x4.Scale(size),
				overlayMesh = val,
				mainTexScale = value,
				mainTexOffset = new Vector4(val4.x, val4.z),
				rotation = rotation
			});
		}
		finally
		{
			Rand.PopState();
		}
	}
}
