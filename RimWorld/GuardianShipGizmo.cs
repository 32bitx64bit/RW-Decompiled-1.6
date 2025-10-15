using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class GuardianShipGizmo : Gizmo
{
	private QuestPart_GuardianShipDelay delay;

	public GuardianShipGizmo(QuestPart_GuardianShipDelay delay)
	{
		this.delay = delay;
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 110f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect rect2 = GenUI.ContractedBy(rect, 6f);
		Widgets.DrawWindowBackground(rect);
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect2, "GuardianShipPayment".Translate(delay.TicksLeft.ToStringTicksToPeriod()));
		GenUI.ResetLabelAlign();
		return new GizmoResult(GizmoState.Clear);
	}
}
