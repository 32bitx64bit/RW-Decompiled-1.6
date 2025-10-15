using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnFirefoamDrawer : PawnOverlayDrawer
{
	public bool coveredInFoam;

	public const float TextureScaleFactor = 2.8f;

	public const float TextureTiles = 1.4f;

	public const float TextureOffsetVecMagnitude = 2f;

	private static readonly string[] FoamTexturePaths = new string[4] { "Things/Pawn/Overlays/Firefoam/FireFoamOverlayA", "Things/Pawn/Overlays/Firefoam/FireFoamOverlayB", "Things/Pawn/Overlays/Firefoam/FireFoamOverlayC", "Things/Pawn/Overlays/Firefoam/FireFoamOverlayD" };

	public PawnFirefoamDrawer(Pawn pawn)
		: base(pawn)
	{
	}

	protected override void WriteCache(CacheKey key, PawnDrawParms parms, List<DrawCall> writeTarget)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		Rot4 pawnRot = key.pawnRot;
		Mesh bodyMesh = key.bodyMesh;
		OverlayLayer layer = key.layer;
		Graphic graphic = ((layer == OverlayLayer.Body) ? pawn.Drawer.renderer.BodyGraphic : pawn.Drawer.renderer.HeadGraphic);
		Rand.PushState(pawn.thingIDNumber * (int)(layer + 1));
		try
		{
			bool num = (graphic.EastFlipped && pawnRot == Rot4.East) || (graphic.WestFlipped && pawnRot == Rot4.West);
			int num2 = (Rand.Range(0, FoamTexturePaths.Length) + pawnRot.AsInt) % FoamTexturePaths.Length;
			Material val = MaterialPool.MatFrom(FoamTexturePaths[num2], ShaderDatabase.FirefoamOverlay, Color.white);
			Mesh val2 = (num ? MeshPool.GridPlaneFlip(Vector2.one * 0.25f) : MeshPool.GridPlane(Vector2.one * 0.25f));
			Bounds bounds = bodyMesh.bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			float num3 = ((Vector3)(ref size)).magnitude * 2.8f;
			MaterialRequest req = default(MaterialRequest);
			req.maskTex = (Texture2D)graphic.MatAt(pawnRot).mainTexture;
			req.mainTex = val.mainTexture;
			req.color = val.color;
			req.shader = val.shader;
			val = MaterialPool.MatFrom(req);
			Vector3 val3 = Rand.InsideUnitCircleVec3 * 2f;
			bounds = val2.bounds;
			Vector3 val4 = ((Bounds)(ref bounds)).size * num3;
			Vector4 value = default(Vector4);
			((Vector4)(ref value))._002Ector(val4.x / size.x, val4.z / size.z);
			Vector4 value2 = default(Vector4);
			((Vector4)(ref value2))._002Ector(val3.x, val3.z);
			Vector4 value3 = default(Vector4);
			((Vector4)(ref value3))._002Ector(1.4f, 1.4f, 1f, 1f);
			writeTarget.Add(new DrawCall
			{
				overlayMat = val,
				matrix = Matrix4x4.Scale(Vector3.one * num3),
				overlayMesh = val2,
				displayOverApparel = true,
				maskTexScale = value,
				mainTexScale = value3,
				mainTexOffset = value2
			});
		}
		finally
		{
			Rand.PopState();
		}
	}
}
