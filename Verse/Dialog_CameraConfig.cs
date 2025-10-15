using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Dialog_CameraConfig : Window
{
	private static readonly FloatRange MoveScaleFactorRange = new FloatRange(0f, 2f);

	private static readonly FloatRange ZoomScaleFactorRange = new FloatRange(0.1f, 10f);

	private const float SliderHeight = 30f;

	private static readonly Texture2D ArrowTex = ContentFinder<Texture2D>.Get("UI/Overlays/TutorArrowRight");

	public override Vector2 InitialSize => new Vector2(260f, 300f);

	private CameraMapConfig Config => Find.CameraDriver.config;

	protected override float Margin => 4f;

	public Dialog_CameraConfig()
	{
		closeOnAccept = false;
		closeOnCancel = false;
		draggable = true;
		layer = WindowLayer.Super;
		doCloseX = true;
		onlyOneOfTypeAllowed = true;
		preventCameraMotion = false;
		focusWhenOpened = false;
		drawShadow = false;
		drawInScreenshotMode = false;
		Reset();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Invalid comparison between Unknown and I4
		Text.Font = GameFont.Small;
		Widgets.Label(new Rect(4f, 0f, ((Rect)(ref rect)).width, 30f), "Camera config");
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, 36f, ((Rect)(ref rect)).width - 8f, 30f);
		Widgets.HorizontalSlider(rect2, ref Config.moveSpeedScale, MoveScaleFactorRange, "Pan speed " + Config.moveSpeedScale, 0.005f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		Widgets.HorizontalSlider(rect2, ref Config.zoomSpeed, ZoomScaleFactorRange, "Zoom speed " + Config.zoomSpeed, 0.1f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		Widgets.FloatRange(rect2, GetHashCode(), ref Config.sizeRange, 0f, 100f, "ZoomRange", ToStringStyle.FloatOne, 1f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		bool checkOn = Config.zoomPreserveFactor > 0f;
		Widgets.CheckboxLabeled(rect2, "Continuous zoom", ref checkOn);
		Config.zoomPreserveFactor = (checkOn ? 1f : 0f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Smooth zoom", ref Config.smoothZoom);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Follow selected pawns", ref Config.followSelected);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Auto pan while paused", ref Config.autoPanWhilePaused);
		Rect val = new Rect(4f, ((Rect)(ref rect2)).yMax, ((Rect)(ref rect)).width - 8f, 9999f);
		float num = 0f;
		GUI.BeginGroup(val);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector((((Rect)(ref rect)).width - 8f) / 2f - 15f, 0f, 30f, 30f);
		Widgets.DrawTextureRotated(((Rect)(ref val2)).center, (Texture)(object)ArrowTex, (0f - Config.autoPanTargetAngle) * 57.29578f, 0.4f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, ((Rect)(ref val2)).yMax + 3f, ((Rect)(ref rect)).width - 8f, 30f);
		float autoPanTargetAngle = Config.autoPanTargetAngle;
		autoPanTargetAngle = Widgets.HorizontalSlider(rect3, autoPanTargetAngle, 0f, MathF.PI * 2f, middleAlignment: false, "Auto pan angle " + (autoPanTargetAngle * 57.29578f).ToString("F0") + "°", "0°", "360°", 0.01f);
		if (autoPanTargetAngle != Config.autoPanTargetAngle)
		{
			Config.autoPanTargetAngle = (Config.autoPanAngle = autoPanTargetAngle);
		}
		num = ((Rect)(ref rect3)).yMax;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(0f, num + 6f, ((Rect)(ref rect)).width - 8f, 30f);
		float autoPanSpeed = Config.autoPanSpeed;
		autoPanSpeed = Widgets.HorizontalSlider(rect4, autoPanSpeed, 0f, 5f, middleAlignment: false, "Auto pan speed " + Config.autoPanSpeed, "0", "10", 0.05f);
		if (autoPanSpeed != Config.autoPanSpeed)
		{
			Config.autoPanSpeed = autoPanSpeed;
		}
		num = ((Rect)(ref rect4)).yMax;
		GUI.EndGroup();
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(4f, ((Rect)(ref rect2)).yMax + num + 10f, ((Rect)(ref rect)).width - 8f, 30f);
		if (ModsConfig.OdysseyActive)
		{
			Widgets.Label(new Rect(4f, ((Rect)(ref rect2)).yMax + num + 10f, ((Rect)(ref rect)).width - 8f, 30f), "Camera Config (Gravship)");
			((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + 30f;
			((Rect)(ref rect5)).height = 30f;
			Widgets.CheckboxLabeled(rect5, "Free camera", ref Config.gravshipFreeCam);
			((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + 30f;
			Widgets.CheckboxLabeled(rect5, "Pan on cutscene start", ref Config.gravshipPanOnCutsceneStart);
			((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + 30f;
			Widgets.CheckboxLabeled(rect5, "Enable custom zoom range", ref Config.gravshipEnableOverrideSizeRange);
			((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + 30f;
			if (Config.gravshipEnableOverrideSizeRange)
			{
				Widgets.FloatRange(rect5, Gen.HashCombine(100, this), ref Config.gravshipOverrideSizeRange, 0f, 100f, "ZoomRangeGravshipCutscene", ToStringStyle.FloatOne, 1f);
				((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + 36f;
			}
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, ((Rect)(ref rect5)).y, ((Rect)(ref rect)).width, 30f);
		Rect rect6 = val3;
		((Rect)(ref rect6)).xMax = ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect6, "Reset"))
		{
			Reset();
		}
		((Rect)(ref rect6)).x = ((Rect)(ref rect6)).x + ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect6, "Save"))
		{
			Find.WindowStack.Add(new Dialog_CameraConfigList_Save(Config));
		}
		((Rect)(ref rect6)).x = ((Rect)(ref rect6)).x + ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect6, "Load"))
		{
			Find.WindowStack.Add(new Dialog_CameraConfigList_Load(delegate(CameraMapConfig c)
			{
				Config.moveSpeedScale = c.moveSpeedScale;
				Config.zoomSpeed = c.zoomSpeed;
				Config.sizeRange = c.sizeRange;
				Config.zoomPreserveFactor = c.zoomPreserveFactor;
				Config.smoothZoom = c.smoothZoom;
				Config.followSelected = c.followSelected;
				Config.autoPanTargetAngle = (Config.autoPanAngle = c.autoPanTargetAngle);
				Config.autoPanSpeed = c.autoPanSpeed;
				Config.fileName = c.fileName;
				Config.autoPanWhilePaused = c.autoPanWhilePaused;
				Config.gravshipFreeCam = c.gravshipFreeCam;
				Config.gravshipPanOnCutsceneStart = c.gravshipPanOnCutsceneStart;
				Config.gravshipEnableOverrideSizeRange = c.gravshipEnableOverrideSizeRange;
				Config.gravshipOverrideSizeRange = c.gravshipOverrideSizeRange;
			}));
		}
		if ((int)Event.current.type == 8)
		{
			((Rect)(ref windowRect)).height = ((Rect)(ref val3)).yMax + Margin * 2f;
		}
	}

	private void Reset()
	{
		Find.CameraDriver.config = new CameraMapConfig_Normal();
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialSize = InitialSize;
		windowRect = GenUI.Rounded(new Rect(5f, 5f, initialSize.x, initialSize.y));
	}
}
