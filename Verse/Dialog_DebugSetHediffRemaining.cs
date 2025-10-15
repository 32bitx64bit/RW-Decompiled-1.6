using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Dialog_DebugSetHediffRemaining : Window
{
	private Hediff hediff;

	private const float HeaderLabelHeight = 40f;

	private const float ButtonHeight = 30f;

	private const float ButtonWidth = 40f;

	private const float Padding = 10f;

	private static readonly Vector2 InitialPositionShift = new Vector2(4f, 0f);

	private HediffComp_Disappears Comp => hediff.TryGetComp<HediffComp_Disappears>();

	public override Vector2 InitialSize => new Vector2(300f, 125f);

	public Dialog_DebugSetHediffRemaining(Hediff hediff, IWindowDrawing customWindowDrawing = null)
		: base(customWindowDrawing)
	{
		this.hediff = hediff;
		layer = WindowLayer.Super;
		closeOnClickedOutside = true;
		doWindowBackground = false;
		drawShadow = false;
		preventCameraMotion = false;
		SoundDefOf.FloatMenu_Open.PlayOneShotOnCamera();
	}

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

	public override void DoWindowContents(Rect inRect)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		if (hediff == null || Comp == null || !hediff.pawn.health.hediffSet.hediffs.Contains(hediff))
		{
			Close();
		}
		Widgets.DrawWindowBackground(inRect);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref inRect)).width, 40f), hediff.def.defName + " time remaining");
		Text.Anchor = (TextAnchor)0;
		float num = 190f;
		float num2 = ((Rect)(ref inRect)).width / 2f - num / 2f;
		if (Widgets.ButtonText(new Rect(num2, 50f, 40f, 30f), "1d"))
		{
			SetTicks(60000);
		}
		float num3 = num2 + 50f;
		if (Widgets.ButtonText(new Rect(num3, 50f, 40f, 30f), "12h"))
		{
			SetTicks(30000);
		}
		float num4 = num3 + 50f;
		if (Widgets.ButtonText(new Rect(num4, 50f, 40f, 30f), "6h"))
		{
			SetTicks(15000);
		}
		if (Widgets.ButtonText(new Rect(num4 + 50f, 50f, 40f, 30f), "1h"))
		{
			SetTicks(2500);
		}
	}

	private void SetTicks(int ticks)
	{
		Comp.ticksToDisappear = ticks;
		SoundDefOf.Click.PlayOneShotOnCamera();
		Close();
	}
}
