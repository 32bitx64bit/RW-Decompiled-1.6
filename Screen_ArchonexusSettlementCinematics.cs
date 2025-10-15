using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

public class Screen_ArchonexusSettlementCinematics : Window
{
	private Action cameraJumpAction;

	private Action nextStepAction;

	private bool FadeInLatch;

	private float ScreenStartTime;

	public const float FadeSecs = 2f;

	private const float MessageDisplaySecs = 7f;

	private const float FadeBuffer = 0.2f;

	private const int MessageWidth = 800;

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	protected override float Margin => 0f;

	private float FadeToBlackEndTime => ScreenStartTime + 2f + 0.2f;

	private float MessageDisplayEndTime => FadeToBlackEndTime + 7f;

	public Screen_ArchonexusSettlementCinematics(Action cameraJumpAction, Action nextStepAction)
	{
		doWindowBackground = false;
		doCloseButton = false;
		doCloseX = false;
		forcePause = true;
		this.cameraJumpAction = cameraJumpAction;
		this.nextStepAction = nextStepAction;
	}

	public override void PreOpen()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.PreOpen();
		Find.MusicManagerPlay.ForceFadeoutAndSilenceFor(11.2f, 1.5f);
		SoundDefOf.ArchonexusNewColonyAccept.PlayOneShotOnCamera();
		ScreenFader.StartFade(Color.black, 2f);
		ScreenStartTime = Time.realtimeSinceStartup;
	}

	public override void PostClose()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		base.PostOpen();
		ScreenFader.SetColor(Color.black);
		nextStepAction();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (IsFinishedFadingIn())
		{
			if (!FadeInLatch)
			{
				FadeInLatch = true;
				cameraJumpAction();
				ScreenFader.SetColor(Color.clear);
			}
			if (IsFinishedDisplayMessage())
			{
				Close(doCloseSound: false);
				return;
			}
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
			GUI.DrawTexture(val, (Texture)(object)BaseContent.BlackTex);
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(val);
			((Rect)(ref val2)).xMin = ((Rect)(ref val)).center.x - 400f;
			((Rect)(ref val2)).width = 800f;
			((Rect)(ref val2)).yMin = ((Rect)(ref val)).center.y;
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			Text.Font = GameFont.Medium;
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(new Rect(inRect), "SoldColonyDescription".Translate());
			Text.Font = font;
			GUI.color = color;
			Text.Anchor = anchor;
		}
	}

	public bool IsFinishedFadingIn()
	{
		return Time.realtimeSinceStartup > FadeToBlackEndTime;
	}

	public bool IsFinishedDisplayMessage()
	{
		return Time.realtimeSinceStartup > MessageDisplayEndTime;
	}
}
