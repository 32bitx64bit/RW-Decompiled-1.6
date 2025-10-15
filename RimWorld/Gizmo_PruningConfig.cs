using LudeonTK;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_PruningConfig : Gizmo
{
	private CompTreeConnection connection;

	private float selectedStrengthTarget = -1f;

	private bool draggingBar;

	private const float Width = 212f;

	private static readonly Texture2D StrengthTex = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.Orange);

	private static readonly Texture2D StrengthHighlightTex = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.LightOrange);

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

	private static readonly Texture2D StrengthTargetTex = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.DarkOrange);

	private float ExtraHeight = Text.LineHeight * 1.5f;

	private float DesiredConnectionStrength
	{
		get
		{
			if (!draggingBar)
			{
				return connection.DesiredConnectionStrength;
			}
			return selectedStrengthTarget;
		}
	}

	private float OverrideHeight => 75f + ExtraHeight;

	public Gizmo_PruningConfig(CompTreeConnection connection)
	{
		this.connection = connection;
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 212f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		if (!ModsConfig.IdeologyActive)
		{
			return new GizmoResult(GizmoState.Clear);
		}
		Rect rect = new Rect(topLeft.x, topLeft.y - ExtraHeight, GetWidth(maxWidth), OverrideHeight);
		Rect val = GenUI.ContractedBy(rect, 6f);
		Widgets.DrawWindowBackground(rect);
		Rect val2 = val;
		float curY = ((Rect)(ref val2)).yMin;
		Text.Anchor = (TextAnchor)1;
		Text.Font = GameFont.Small;
		Widgets.Label(((Rect)(ref val2)).x, ref curY, ((Rect)(ref val2)).width, "ConnectionStrength".Translate());
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(((Rect)(ref val2)).x, ref curY, ((Rect)(ref val2)).width, "DesiredConnectionStrength".Translate() + ": " + DesiredConnectionStrength.ToStringPercent());
		Widgets.Label(((Rect)(ref val2)).x, ref curY, ((Rect)(ref val2)).width, "PruningHoursToMaintain".Translate() + ": " + connection.PruningHoursToMaintain(DesiredConnectionStrength).ToString("F1"));
		Text.Font = GameFont.Small;
		if (Mouse.IsOver(val) && !draggingBar)
		{
			Widgets.DrawHighlight(val);
			TooltipHandler.TipRegion(val, () => GetTip(), 9493937);
		}
		DrawBar(val, curY);
		return new GizmoResult(GizmoState.Clear);
	}

	private string GetTip()
	{
		string text = "DesiredConnectionStrengthDesc".Translate(connection.parent.Named("TREE"), connection.ConnectedPawn.Named("CONNECTEDPAWN"), connection.ConnectionStrengthLossPerDay.ToStringPercent().Named("FALL")).Resolve();
		string text2 = connection.AffectingBuildingsDescription("CurrentlyAffectedBy");
		if (!text2.NullOrEmpty())
		{
			text = text + "\n\n" + text2;
		}
		return text;
	}

	private void DrawThreshold(Rect rect, float percent, float strValue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + 3f + (((Rect)(ref rect)).width - 8f) * percent;
		((Rect)(ref val)).y = ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - 9f;
		((Rect)(ref val)).width = 2f;
		((Rect)(ref val)).height = 6f;
		Rect val2 = val;
		if (strValue < percent)
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.GreyTex);
		}
		else
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.BlackTex);
		}
	}

	private void DrawStrengthTarget(Rect rect, float percent)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Round((((Rect)(ref rect)).width - 8f) * percent);
		GUI.DrawTexture(new Rect(((Rect)(ref rect)).x + 3f + num, ((Rect)(ref rect)).y, 2f, ((Rect)(ref rect)).height), (Texture)(object)StrengthTargetTex);
		float num2 = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect)).x + 2f + num);
		float xMax = UIScaling.AdjustCoordToUIScalingCeil(num2 + 4f);
		Rect val = default(Rect);
		((Rect)(ref val)).y = ((Rect)(ref rect)).y - 3f;
		((Rect)(ref val)).height = 5f;
		((Rect)(ref val)).xMin = num2;
		((Rect)(ref val)).xMax = xMax;
		Rect val2 = val;
		GUI.DrawTexture(val2, (Texture)(object)StrengthTargetTex);
		Rect val3 = val2;
		((Rect)(ref val3)).y = ((Rect)(ref rect)).yMax - 2f;
		GUI.DrawTexture(val3, (Texture)(object)StrengthTargetTex);
	}

	private void DrawBar(Rect inRect, float curY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Invalid comparison between Unknown and I4
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Invalid comparison between Unknown and I4
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = inRect;
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 10f;
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 10f;
		((Rect)(ref rect)).yMax = ((Rect)(ref inRect)).yMax - 4f;
		((Rect)(ref rect)).yMin = curY + 10f;
		bool flag = Mouse.IsOver(rect);
		float connectionStrength = connection.ConnectionStrength;
		Widgets.FillableBar(rect, connectionStrength, flag ? StrengthHighlightTex : StrengthTex, EmptyBarTex, doBorder: true);
		foreach (CurvePoint point in connection.Props.maxDryadsPerConnectionStrengthCurve.Points)
		{
			if (point.x > 0f)
			{
				DrawThreshold(rect, point.x, connectionStrength);
			}
		}
		float num = Mathf.Clamp(Mathf.Round((Event.current.mousePosition.x - (((Rect)(ref rect)).x + 3f)) / (((Rect)(ref rect)).width - 8f) * 20f) / 20f, 0f, 1f);
		Event current2 = Event.current;
		if ((int)current2.type == 0 && current2.button == 0 && flag)
		{
			selectedStrengthTarget = num;
			draggingBar = true;
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			current2.Use();
		}
		if ((int)current2.type == 3 && current2.button == 0 && draggingBar && flag)
		{
			if (Mathf.Abs(num - selectedStrengthTarget) > float.Epsilon)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			selectedStrengthTarget = num;
			current2.Use();
		}
		if ((int)current2.type == 1 && current2.button == 0 && draggingBar)
		{
			if (selectedStrengthTarget >= 0f)
			{
				connection.DesiredConnectionStrength = selectedStrengthTarget;
			}
			selectedStrengthTarget = -1f;
			draggingBar = false;
			current2.Use();
		}
		DrawStrengthTarget(rect, DesiredConnectionStrength);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, connection.ConnectionStrength.ToStringPercent());
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
	}
}
