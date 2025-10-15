using System.Collections.Generic;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public abstract class Gizmo_Slider : Gizmo
{
	private Texture2D barTex;

	private Texture2D barHighlightTex;

	private Texture2D barDragTex;

	private float targetValuePct;

	private bool initialized;

	protected Rect barRect;

	private const float Spacing = 8f;

	private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.34f, 0.42f, 0.43f));

	private static readonly Texture2D BarHighlightTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.43f, 0.54f, 0.55f));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

	private static readonly Texture2D DragBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.74f, 0.97f, 0.8f));

	protected virtual float Width => 160f;

	protected abstract float Target { get; set; }

	protected abstract float ValuePercent { get; }

	protected virtual Color BarColor { get; }

	protected virtual Color BarHighlightColor { get; }

	protected virtual Color BarDragColor { get; }

	protected virtual FloatRange DragRange { get; } = FloatRange.ZeroToOne;


	protected virtual bool IsDraggable { get; }

	protected virtual string BarLabel => ValuePercent.ToStringPercent("0");

	protected abstract string Title { get; }

	protected virtual int Increments { get; } = 20;


	protected virtual string HighlightTag => null;

	protected abstract bool DraggingBar { get; set; }

	public sealed override float Order
	{
		get
		{
			return -100f;
		}
		set
		{
			base.Order = value;
		}
	}

	public sealed override float GetWidth(float maxWidth)
	{
		return Width;
	}

	protected virtual IEnumerable<float> GetBarThresholds()
	{
		yield break;
	}

	protected abstract string GetTooltip();

	private void Initialize()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			initialized = true;
			targetValuePct = Mathf.Clamp(Target, DragRange.min, DragRange.max);
			barTex = ((BarColor == default(Color)) ? BarTex : SolidColorMaterials.NewSolidColorTexture(BarColor));
			barHighlightTex = ((BarHighlightColor == default(Color)) ? BarHighlightTex : SolidColorMaterials.NewSolidColorTexture(BarHighlightColor));
			barDragTex = ((BarDragColor == default(Color)) ? DragBarTex : SolidColorMaterials.NewSolidColorTexture(BarDragColor));
		}
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			Initialize();
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = rect.ContractedBy(8f);
		Widgets.DrawWindowBackground(rect);
		bool mouseOverElement = false;
		Text.Font = GameFont.Small;
		Rect headerRect = val;
		((Rect)(ref headerRect)).height = Text.LineHeight;
		DrawHeader(headerRect, ref mouseOverElement);
		barRect = val;
		((Rect)(ref barRect)).yMin = ((Rect)(ref headerRect)).yMax + 8f;
		if (!IsDraggable)
		{
			Widgets.FillableBar(barRect, ValuePercent, barTex, EmptyBarTex, doBorder: true);
			foreach (float barThreshold in GetBarThresholds())
			{
				Rect val2 = default(Rect);
				((Rect)(ref val2)).x = ((Rect)(ref barRect)).x + 3f + (((Rect)(ref barRect)).width - 8f) * barThreshold;
				((Rect)(ref val2)).y = ((Rect)(ref barRect)).y + ((Rect)(ref barRect)).height - 9f;
				((Rect)(ref val2)).width = 2f;
				((Rect)(ref val2)).height = 6f;
				GUI.DrawTexture(val2, (Texture)(object)((ValuePercent < barThreshold) ? BaseContent.GreyTex : BaseContent.BlackTex));
			}
		}
		else
		{
			bool draggingBar = DraggingBar;
			Widgets.DraggableBar(barRect, barTex, barHighlightTex, EmptyBarTex, barDragTex, ref draggingBar, ValuePercent, ref targetValuePct, GetBarThresholds(), Increments, DragRange.min, DragRange.max);
			DraggingBar = draggingBar;
			targetValuePct = Mathf.Clamp(targetValuePct, DragRange.min, DragRange.max);
			Target = targetValuePct;
		}
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(barRect, BarLabel);
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect) && !mouseOverElement)
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, GetTooltip, Gen.HashCombineInt(GetHashCode(), 8573612));
		}
		if (!HighlightTag.NullOrEmpty())
		{
			UIHighlighter.HighlightOpportunity(rect, HighlightTag);
		}
		return new GizmoResult(GizmoState.Clear);
	}

	protected virtual void DrawHeader(Rect headerRect, ref bool mouseOverElement)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		string title = Title;
		title = title.Truncate(((Rect)(ref headerRect)).width);
		Widgets.Label(headerRect, title);
	}
}
