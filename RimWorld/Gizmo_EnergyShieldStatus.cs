using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_EnergyShieldStatus : Gizmo
{
	public CompShield shield;

	private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

	private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

	public Gizmo_EnergyShieldStatus()
	{
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 140f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = rect.ContractedBy(6f);
		Widgets.DrawWindowBackground(rect);
		Rect rect2 = val;
		((Rect)(ref rect2)).height = ((Rect)(ref rect)).height / 2f;
		Text.Font = GameFont.Tiny;
		Widgets.Label(rect2, shield.IsApparel ? shield.parent.LabelCap : "ShieldInbuilt".Translate().Resolve());
		Rect rect3 = val;
		((Rect)(ref rect3)).yMin = ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f;
		float fillPercent = shield.Energy / Mathf.Max(1f, shield.parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
		Widgets.FillableBar(rect3, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect3, (shield.Energy * 100f).ToString("F0") + " / " + (shield.parent.GetStatValue(StatDefOf.EnergyShieldEnergyMax) * 100f).ToString("F0"));
		Text.Anchor = (TextAnchor)0;
		TooltipHandler.TipRegion(val, "ShieldPersonalTip".Translate());
		return new GizmoResult(GizmoState.Clear);
	}
}
