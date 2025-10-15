using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MechCarrierGizmo : Gizmo
{
	private CompMechCarrier carrier;

	private const float Width = 160f;

	private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.34f, 0.42f, 0.43f));

	private static readonly Texture2D BarHighlightTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.43f, 0.54f, 0.55f));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

	private static readonly Texture2D DragBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.74f, 0.97f, 0.8f));

	private const int Increments = 24;

	private static bool draggingBar;

	private float lastTargetValue;

	private float targetValue;

	private static List<float> bandPercentages;

	public MechCarrierGizmo(CompMechCarrier carrier)
	{
		this.carrier = carrier;
		targetValue = (float)carrier.maxToFill / (float)carrier.Props.maxIngredientCount;
		if (bandPercentages == null)
		{
			bandPercentages = new List<float>();
			int num = 12;
			for (int i = 0; i <= num; i++)
			{
				float item = 1f / (float)num * (float)i;
				bandPercentages.Add(item);
			}
		}
	}

	public override float GetWidth(float maxWidth)
	{
		return 160f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 10f);
		Widgets.DrawWindowBackground(rect);
		Text.Font = GameFont.Small;
		TaggedString labelCap = carrier.Props.fixedIngredient.LabelCap;
		float num = Text.CalcHeight(labelCap, ((Rect)(ref val)).width);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, num);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, labelCap);
		Text.Anchor = (TextAnchor)0;
		lastTargetValue = targetValue;
		float num2 = ((Rect)(ref val)).height - ((Rect)(ref rect2)).height;
		float num3 = num2 - 4f;
		float num4 = (num2 - num3) / 2f;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref rect2)).yMax + num4, ((Rect)(ref val)).width, num3);
		Widgets.DraggableBar(val2, BarTex, BarHighlightTex, EmptyBarTex, DragBarTex, ref draggingBar, carrier.PercentageFull, ref targetValue, bandPercentages, 24);
		Text.Anchor = (TextAnchor)4;
		((Rect)(ref val2)).y = ((Rect)(ref val2)).y - 2f;
		Widgets.Label(val2, carrier.IngredientCount + " / " + carrier.Props.maxIngredientCount);
		Text.Anchor = (TextAnchor)0;
		TooltipHandler.TipRegion(val2, () => GetResourceBarTip(), Gen.HashCombineInt(carrier.GetHashCode(), 34242369));
		if (lastTargetValue != targetValue)
		{
			carrier.maxToFill = Mathf.RoundToInt(targetValue * (float)carrier.Props.maxIngredientCount);
		}
		return new GizmoResult(GizmoState.Clear);
	}

	private string GetResourceBarTip()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Concat("MechCarrierAutofillResources".Translate() + " " + carrier.Props.fixedIngredient.label + ": ", carrier.maxToFill.ToString()));
		stringBuilder.AppendInNewLine("MechCarrierClickToSetAutofillAmount".Translate());
		stringBuilder.AppendLine();
		stringBuilder.AppendInNewLine("MechCarrierAutofillDesc".Translate(carrier.parent.def.label, carrier.Props.spawnPawnKind.labelPlural));
		return stringBuilder.ToString();
	}
}
