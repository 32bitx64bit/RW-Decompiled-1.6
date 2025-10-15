using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompFireOverlay : CompFireOverlayBase
{
	protected CompRefuelable refuelableComp;

	public static readonly Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Fire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);

	public new CompProperties_FireOverlay Props => (CompProperties_FireOverlay)props;

	public override void PostDraw()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		if (refuelableComp == null || refuelableComp.HasFuel)
		{
			Vector3 drawPos = parent.DrawPos;
			drawPos.y += 0.03658537f;
			FireGraphic.Draw(drawPos, parent.Rotation, parent);
		}
	}

	public override void PostSpawnSetup(bool respawningAfterLoad)
	{
		base.PostSpawnSetup(respawningAfterLoad);
		refuelableComp = parent.GetComp<CompRefuelable>();
	}

	public override void CompTick()
	{
		if ((refuelableComp == null || refuelableComp.HasFuel) && startedGrowingAtTick < 0)
		{
			startedGrowingAtTick = GenTicks.TicksAbs;
		}
	}
}
