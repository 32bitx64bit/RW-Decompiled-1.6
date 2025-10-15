using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Dialog_DebugSetSeverity : Window
{
	private Hediff hediff;

	private float maxSeverity;

	private float sliderValue;

	private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);

	private readonly Vector2 BottomButtonSize = new Vector2(100f, 20f);

	private const float HeaderLabelHeight = 40f;

	private const float Padding = 20f;

	public override Vector2 InitialSize => new Vector2(300f, 125f);

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = UI.MousePositionOnUIInverted + InitialPositionShift;
		if (val.x + InitialSize.x > (float)UI.screenWidth)
		{
			val.x = (float)UI.screenWidth - InitialSize.x;
		}
		if (val.y + InitialSize.y > (float)UI.screenHeight)
		{
			val.y = (float)UI.screenHeight - InitialSize.y;
		}
		windowRect = new Rect(val.x, val.y, InitialSize.x, InitialSize.y);
	}

	public Dialog_DebugSetSeverity(Hediff hediff)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.hediff = hediff;
		maxSeverity = ((hediff.def.maxSeverity >= float.MaxValue) ? 1f : hediff.def.maxSeverity);
		sliderValue = Mathf.InverseLerp(hediff.def.minSeverity, maxSeverity, hediff.Severity);
		layer = WindowLayer.Super;
		closeOnClickedOutside = true;
		doWindowBackground = false;
		drawShadow = false;
		preventCameraMotion = false;
		SoundDefOf.FloatMenu_Open.PlayOneShotOnCamera();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if (hediff == null || !hediff.pawn.health.hediffSet.hediffs.Contains(hediff))
		{
			Close();
		}
		Widgets.DrawWindowBackground(inRect);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref inRect)).width, 40f), hediff.def.defName + " severity");
		Text.Anchor = (TextAnchor)0;
		Rect val = inRect;
		((Rect)(ref val)).y = 40f;
		((Rect)(ref val)).height = 10f;
		((Rect)(ref val)).x = ((Rect)(ref val)).x + 20f;
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 40f;
		sliderValue = GUI.HorizontalSlider(val, sliderValue, 0f, 1f);
		Rect rect = inRect;
		((Rect)(ref rect)).y = ((Rect)(ref inRect)).height - 10f - BottomButtonSize.y;
		((Rect)(ref rect)).height = BottomButtonSize.y;
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + 20f;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - 40f;
		float num = Mathf.Round(sliderValue * 100f);
		if (Widgets.ButtonText(rect, $"Set to {num}%"))
		{
			float severity = Mathf.Lerp(hediff.def.minSeverity, maxSeverity, Mathf.Clamp01(num / 100f));
			hediff.Severity = severity;
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
	}
}
