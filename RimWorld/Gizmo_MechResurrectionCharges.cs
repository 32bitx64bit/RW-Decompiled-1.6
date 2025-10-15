using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_MechResurrectionCharges : Gizmo
{
	private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

	private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

	private static readonly float Width = 110f;

	private CompAbilityEffect_ResurrectMech ability;

	public Gizmo_MechResurrectionCharges(CompAbilityEffect_ResurrectMech ability)
	{
		this.ability = ability;
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return Width;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val2 = val.ContractedBy(6f);
		Widgets.DrawWindowBackground(val);
		Rect rect = val2;
		((Rect)(ref rect)).height = ((Rect)(ref val)).height / 2f;
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(rect, "MechResurrectionCharges".Translate());
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = val;
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + (((Rect)(ref rect)).height - 5f);
		((Rect)(ref rect2)).height = ((Rect)(ref val)).height / 2f;
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect2, ability.ChargesRemaining.ToString());
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		return new GizmoResult(GizmoState.Clear);
	}
}
