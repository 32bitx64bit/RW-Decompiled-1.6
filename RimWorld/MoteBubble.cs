using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MoteBubble : MoteDualAttached
{
	public Material iconMat;

	public Pawn arrowTarget;

	public MaterialPropertyBlock iconMatPropertyBlock;

	private static readonly Material InteractionArrowTex = MaterialPool.MatFrom("Things/Mote/InteractionArrow");

	public void SetupMoteBubble(Texture2D icon, Pawn target, Color? iconColor = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		iconMat = MaterialPool.MatFrom(icon, ShaderDatabase.TransparentPostLight, Color.white);
		iconMatPropertyBlock = new MaterialPropertyBlock();
		arrowTarget = target;
		if (iconColor.HasValue)
		{
			iconMatPropertyBlock.SetColor(ShaderPropertyIDs.Color, iconColor.Value);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		base.DrawAt(drawLoc, flip);
		if (Find.UIRoot.HideMotes)
		{
			return;
		}
		if ((Object)(object)iconMat != (Object)null)
		{
			Vector3 drawPos = DrawPos;
			drawPos.y += 0.01f;
			float alpha = Alpha;
			if (alpha <= 0f)
			{
				return;
			}
			Color val = instanceColor;
			val.a *= alpha;
			Material val2 = iconMat;
			if (val != val2.color)
			{
				val2 = MaterialPool.MatFrom((Texture2D)val2.mainTexture, val2.shader, val);
			}
			Vector3 val3 = default(Vector3);
			((Vector3)(ref val3))._002Ector(def.graphicData.drawSize.x * 0.64f, 1f, def.graphicData.drawSize.y * 0.64f);
			Matrix4x4 val4 = default(Matrix4x4);
			((Matrix4x4)(ref val4)).SetTRS(drawPos, Quaternion.identity, val3);
			Graphics.DrawMesh(MeshPool.plane10, val4, val2, 0, (Camera)null, 0, iconMatPropertyBlock);
		}
		if (arrowTarget != null)
		{
			Quaternion val5 = Quaternion.AngleAxis(((arrowTarget.Spawned ? arrowTarget.TrueCenter() : arrowTarget.PositionHeld.ToVector3Shifted()) - DrawPos).AngleFlat(), Vector3.up);
			Vector3 drawPos2 = DrawPos;
			drawPos2.y -= 0.01f;
			drawPos2 += 0.6f * (val5 * Vector3.forward);
			Graphics.DrawMesh(MeshPool.plane05, drawPos2, val5, InteractionArrowTex, 0);
		}
	}
}
