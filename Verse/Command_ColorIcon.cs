using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Command_ColorIcon : Command_Action
{
	private static readonly Texture2D ColorIndicatorTex = ContentFinder<Texture2D>.Get("UI/Icons/ColorIndicatorBulb");

	public Color32? color;

	private const int colorCircleDiameter = 16;

	private const float colorCircleGap = 4f;

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		if (color.HasValue)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			RectDivider rectDivider = new RectDivider(rect.ContractedBy(4f), 1552930585);
			GUI.DrawTexture((Rect)rectDivider.NewCol(16f, HorizontalJustification.Right).NewRow(16f), (Texture)(object)ColorIndicatorTex, (ScaleMode)2, true, 1f, Color32.op_Implicit(color.Value), 0f, 0f);
		}
		return result;
	}
}
