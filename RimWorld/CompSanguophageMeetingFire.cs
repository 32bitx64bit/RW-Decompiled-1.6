using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompSanguophageMeetingFire : CompFireOverlayBase
{
	public static readonly Graphic RedlightGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Redlight", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

	public new CompProperties_SanguophageMeetingFire Props => (CompProperties_SanguophageMeetingFire)props;

	public override void PostDraw()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		CompGlower compGlower = parent.TryGetComp<CompGlower>();
		if (compGlower == null || compGlower.Glows)
		{
			Vector3 drawPos = parent.DrawPos;
			drawPos.y += 0.03658537f;
			RedlightGraphic.Draw(drawPos + Props.offset, Rot4.North, parent);
		}
	}

	public override bool CompPreventClaimingBy(Faction faction)
	{
		return ((Building)parent).GetLord()?.CurLordToil is LordToil_SanguophageMeeting;
	}
}
