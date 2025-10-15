using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Command_ActionWithCooldown : Command_Action
{
	public Func<float> cooldownPercentGetter;

	private static readonly Texture2D CooldownBarTex = SolidColorMaterials.NewSolidColorTexture(Color32.op_Implicit(new Color32((byte)9, (byte)203, (byte)4, (byte)64)));

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		if (cooldownPercentGetter != null)
		{
			float num = cooldownPercentGetter();
			if (num < 1f)
			{
				Widgets.FillableBar(rect, Mathf.Clamp01(num), CooldownBarTex, null, doBorder: false);
				Text.Font = GameFont.Tiny;
				Text.Anchor = (TextAnchor)1;
				Widgets.Label(rect, num.ToStringPercent("F0"));
				Text.Anchor = (TextAnchor)0;
			}
		}
		return result;
	}
}
