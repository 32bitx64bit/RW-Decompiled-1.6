using RimWorld;
using UnityEngine;

namespace Verse;

public class Dialog_RechargeSettings : Window
{
	private FloatRange range;

	private MechanitorControlGroup controlGroup;

	private string title;

	private string text;

	private const float HeaderHeight = 30f;

	private const float SliderHeight = 30f;

	public override Vector2 InitialSize => new Vector2(450f, 300f);

	public Dialog_RechargeSettings(MechanitorControlGroup controlGroup)
	{
		this.controlGroup = controlGroup;
		range = controlGroup.mechRechargeThresholds;
		title = "MechRechargeSettingsTitle".Translate();
		text = "MechRechargeSettingsExplanation".Translate();
		forcePause = true;
		closeOnClickedOutside = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		float y = ((Rect)(ref inRect)).y;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, y, ((Rect)(ref inRect)).width, 30f);
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect, title);
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		y += ((Rect)(ref rect)).height + 17f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref inRect)).x, y, ((Rect)(ref inRect)).width, Text.CalcHeight(text, ((Rect)(ref inRect)).width));
		Text.Anchor = (TextAnchor)3;
		Text.Font = GameFont.Small;
		Widgets.Label(rect2, text);
		Text.Anchor = (TextAnchor)0;
		y += ((Rect)(ref rect2)).height + 17f;
		Widgets.FloatRange(new Rect(((Rect)(ref inRect)).x, y, ((Rect)(ref inRect)).width, 30f), GetHashCode(), ref range, 0f, 1f, null, ToStringStyle.PercentZero, 0.05f, GameFont.Small, Color.white);
		range.min = GenMath.RoundTo(range.min, 0.01f);
		range.max = GenMath.RoundTo(range.max, 0.01f);
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "CancelButton".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x + ((Rect)(ref inRect)).width / 2f - Window.CloseButSize.x / 2f, ((Rect)(ref inRect)).yMax - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "Reset".Translate()))
		{
			range = MechanitorControlGroup.DefaultMechRechargeThresholds;
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).xMax - Window.CloseButSize.x, ((Rect)(ref inRect)).yMax - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "OK".Translate()))
		{
			controlGroup.mechRechargeThresholds = range;
			Close();
		}
	}
}
