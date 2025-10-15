using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_ProjectileInterceptorHitPoints : Gizmo
{
	public CompProjectileInterceptor interceptor;

	private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

	private const float Width = 140f;

	public const int InRectPadding = 6;

	public Gizmo_ProjectileInterceptorHitPoints()
	{
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 140f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect rect2 = GenUI.ContractedBy(rect, 6f);
		Widgets.DrawWindowBackground(rect);
		bool num = interceptor.ChargingTicksLeft > 0;
		TaggedString label = ((!num) ? "ShieldEnergy".Translate() : "ShieldTimeToRecovery".Translate());
		float fillPercent = ((!num) ? ((float)interceptor.currentHitPoints / (float)interceptor.HitPointsMax) : ((float)interceptor.ChargingTicksLeft / (float)interceptor.Props.chargeDurationTicks));
		string label2 = ((!num) ? (interceptor.currentHitPoints + " / " + interceptor.HitPointsMax) : interceptor.ChargingTicksLeft.ToStringTicksToPeriod());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y - 2f, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height / 2f);
		Widgets.Label(rect3, label);
		Rect rect4 = new Rect(((Rect)(ref rect2)).x, ((Rect)(ref rect3)).yMax, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height / 2f);
		Widgets.FillableBar(rect4, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect4, label2);
		Text.Anchor = (TextAnchor)0;
		if (!interceptor.Props.gizmoTipKey.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect2, interceptor.Props.gizmoTipKey.Translate());
		}
		return new GizmoResult(GizmoState.Clear);
	}
}
