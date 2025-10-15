using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MechPowerCellGizmo : Gizmo
{
	private CompMechPowerCell powerCell;

	private const float Width = 160f;

	private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(Color32.op_Implicit(new Color32((byte)12, (byte)45, (byte)45, byte.MaxValue)));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(GenUI.FillableBar_Empty);

	private const int BarThresholdTickIntervals = 2500;

	public MechPowerCellGizmo(CompMechPowerCell carrier)
	{
		powerCell = carrier;
	}

	public override float GetWidth(float maxWidth)
	{
		return 160f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect rect2 = GenUI.ContractedBy(rect, 10f);
		Widgets.DrawWindowBackground(rect);
		string text = (powerCell.Props.labelOverride.NullOrEmpty() ? ((string)"MechPowerCell".Translate()) : powerCell.Props.labelOverride);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width, Text.CalcHeight(text, ((Rect)(ref rect2)).width) + 8f);
		Text.Font = GameFont.Small;
		Widgets.Label(rect3, text);
		Rect barRect = new Rect(((Rect)(ref rect2)).x, ((Rect)(ref rect3)).yMax, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height - ((Rect)(ref rect3)).height);
		Widgets.FillableBar(barRect, powerCell.PercentFull, BarTex, EmptyBarTex, doBorder: true);
		for (int i = 2500; i < powerCell.Props.totalPowerTicks; i += 2500)
		{
			DoBarThreshold((float)i / (float)powerCell.Props.totalPowerTicks);
		}
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(barRect, Mathf.CeilToInt((float)powerCell.PowerTicksLeft / 2500f).ToString() + "LetterHour".Translate());
		Text.Anchor = (TextAnchor)0;
		string tooltip;
		if (!powerCell.Props.tooltipOverride.NullOrEmpty())
		{
			tooltip = powerCell.Props.tooltipOverride;
		}
		else
		{
			tooltip = "MechPowerCellTip".Translate();
		}
		TooltipHandler.TipRegion(rect2, () => tooltip, Gen.HashCombineInt(powerCell.GetHashCode(), 34242369));
		return new GizmoResult(GizmoState.Clear);
		void DoBarThreshold(float percent)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			Rect val = default(Rect);
			((Rect)(ref val)).x = ((Rect)(ref barRect)).x + 3f + (((Rect)(ref barRect)).width - 8f) * percent;
			((Rect)(ref val)).y = ((Rect)(ref barRect)).y + ((Rect)(ref barRect)).height - 9f;
			((Rect)(ref val)).width = 2f;
			((Rect)(ref val)).height = 6f;
			GUI.DrawTexture(val, (Texture)(object)BaseContent.BlackTex);
		}
	}
}
