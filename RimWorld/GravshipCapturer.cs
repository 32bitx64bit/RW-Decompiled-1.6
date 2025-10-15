using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class GravshipCapturer
{
	private static readonly int GravshipCaptureLayerMaskExclude = LayerMask.GetMask(new string[2] { "UI", "GravshipExclude" });

	private static readonly int GravshipCaptureLayerMaskInclude = LayerMask.GetMask(new string[1] { "GravshipMask" });

	private static readonly Material MatGravshipBlit = MatLoader.LoadMat("Map/Gravship/GravshipBlit");

	private static readonly Material MatGravshipOpaqueBase = MatLoader.LoadMatDirect("Map/Gravship/GravshipOpaqueBase");

	private static readonly Material MatGravshipChromaKey = MatLoader.LoadMatDirect("Map/Gravship/GravshipChromaKey");

	private const float CAMERA_MAX_ZOOM_PADDING = 10f;

	public static bool IsGravshipRenderInProgress = false;

	public static CellRect GravshipCaptureBounds;

	private Capture captureInProgress;

	private Action<Capture> onComplete;

	private int framesToWait = -1;

	private Camera captureCam;

	public bool IsCaptureComplete { get; private set; }

	public void BeginGravshipRender(Building_GravEngine engine, Action<Capture> onComplete)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckOdyssey("Gravship"))
		{
			return;
		}
		this.onComplete = onComplete;
		captureInProgress = new Capture(engine);
		IsGravshipRenderInProgress = true;
		IsCaptureComplete = false;
		Find.ScreenshotModeHandler.Active = true;
		CellRect bounds = (GravshipCaptureBounds = (captureInProgress.captureBounds = CellRect.FromCellList(engine.ValidSubstructure).ExpandedBy(1)));
		Camera camera = Find.Camera;
		captureCam = Current.SubcameraDriver.GetSubcamera(SubcameraDefOf.GravshipMask);
		captureCam.cullingMask = (camera.cullingMask & ~GravshipCaptureLayerMaskExclude) | GravshipCaptureLayerMaskInclude;
		captureCam.Fit(bounds, 15f);
		captureInProgress.drawSize = bounds.Size.ToVector3().WithY(1f);
		captureInProgress.captureCenter = bounds.CenterVector3;
		captureInProgress.maxCameraSize = Find.CameraDriver.config.sizeRange.max;
		captureInProgress.minCameraSize = Mathf.Max(Mathf.Max(Find.CameraDriver.config.sizeRange.min, captureInProgress.maxCameraSize / 2f), captureCam.orthographicSize + 10f);
		int screenshotWidth = Mathf.RoundToInt((float)Screen.height * captureCam.aspect);
		int screenshotHeight = Screen.height;
		RenderTexture screenshot = RenderTexture.GetTemporary(screenshotWidth, screenshotHeight, 24, (RenderTextureFormat)0, (RenderTextureReadWrite)1, 1);
		captureCam.targetTexture = screenshot;
		DrawChromaKey();
		OnPostRenderHook.HookOnce(captureCam, delegate
		{
			((Behaviour)captureCam).enabled = false;
			captureCam.targetTexture = null;
			if (DebugViewSettings.saveGravshipRenders)
			{
				screenshot.SaveAsPNG("Logs/gravship-screenshot.png");
			}
			RenderTexture temporary = RenderTexture.GetTemporary(screenshotWidth, screenshotHeight, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)0, 1);
			if (DebugViewSettings.disableGravshipRenderShader)
			{
				Graphics.Blit((Texture)(object)screenshot, temporary);
			}
			else
			{
				Graphics.Blit((Texture)(object)screenshot, temporary, MatGravshipBlit);
			}
			captureInProgress.capture = (SavedTexture2D)temporary.CreateTexture2D((TextureFormat)5, mipChain: true);
			((Texture)captureInProgress.capture.Texture).filterMode = (FilterMode)1;
			if (DebugViewSettings.saveGravshipRenders)
			{
				captureInProgress.capture.Texture.SaveAsPNG("Logs/gravship-render.png");
			}
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(screenshot);
			IsGravshipRenderInProgress = false;
			framesToWait = 1;
		});
		((Behaviour)captureCam).enabled = true;
	}

	public void BeginTerrainRender(CellRect landingBounds, Action<Capture> onComplete)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckOdyssey("Gravship"))
		{
			return;
		}
		this.onComplete = onComplete;
		captureInProgress = new Capture(null);
		IsGravshipRenderInProgress = true;
		IsCaptureComplete = false;
		Find.ScreenshotModeHandler.Active = true;
		Camera camera = Find.Camera;
		captureCam = Current.SubcameraDriver.GetSubcamera(SubcameraDefOf.GravshipMask);
		captureCam.cullingMask = camera.cullingMask;
		captureCam.Fit(landingBounds, 15f);
		captureInProgress.captureBounds = landingBounds;
		captureInProgress.drawSize = landingBounds.Size.ToVector3().WithY(1f);
		captureInProgress.captureCenter = landingBounds.CenterVector3;
		captureInProgress.maxCameraSize = Find.CameraDriver.config.sizeRange.max;
		captureInProgress.minCameraSize = Mathf.Max(Mathf.Max(Find.CameraDriver.config.sizeRange.min, captureInProgress.maxCameraSize / 2f), captureCam.orthographicSize + 10f);
		int num = Mathf.RoundToInt((float)Screen.height * captureCam.aspect);
		int height = Screen.height;
		RenderTexture screenshot = RenderTexture.GetTemporary(num, height, 24, (RenderTextureFormat)0, (RenderTextureReadWrite)0, 1);
		captureCam.targetTexture = screenshot;
		OnPostRenderHook.HookOnce(captureCam, delegate
		{
			((Behaviour)captureCam).enabled = false;
			captureCam.targetTexture = null;
			Texture2D val = screenshot.CreateTexture2D((TextureFormat)5, mipChain: true);
			((Texture)val).filterMode = (FilterMode)1;
			if (DebugViewSettings.saveGravshipRenders)
			{
				val.SaveAsPNG("Logs/terrain-screenshot.png");
			}
			captureInProgress.capture = (SavedTexture2D)val;
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(screenshot);
			IsGravshipRenderInProgress = false;
			framesToWait = 1;
		});
		((Behaviour)captureCam).enabled = true;
	}

	public static void CaptureWorldSkybox(Action<Texture2D> callback)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		if (!ModLister.CheckOdyssey("Gravship"))
		{
			return;
		}
		Find.WorldCameraDriver.ApplyMapPositionToGameObject();
		GameObject worldCameraCopyGO = new GameObject("TerrainCurtainWorldCamera");
		GameObject worldSkyboxCameraCopyGO = new GameObject("TerrainCurtainWorldSkyboxCamera");
		Camera worldCameraCopy = worldCameraCopyGO.AddComponent<Camera>();
		Camera worldSkyboxCameraCopy = worldSkyboxCameraCopyGO.AddComponent<Camera>();
		worldCameraCopy.CopyFrom(WorldCameraManager.WorldCamera);
		worldSkyboxCameraCopy.CopyFrom(WorldCameraManager.WorldSkyboxCamera);
		RenderTexture screenshot = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, (RenderTextureFormat)0, (RenderTextureReadWrite)0, 1);
		worldCameraCopy.targetTexture = screenshot;
		worldSkyboxCameraCopy.targetTexture = screenshot;
		OnPostRenderHook.HookOnce(worldCameraCopy, delegate
		{
			OnPostRenderHook.HookOnce(worldCameraCopy, delegate
			{
				worldCameraCopy.targetTexture = null;
				worldSkyboxCameraCopy.targetTexture = null;
				Texture2D val = screenshot.CreateTexture2D((TextureFormat)5);
				if (DebugViewSettings.saveGravshipRenders)
				{
					val.SaveAsPNG("Logs/world-skybox-screenshot.png");
				}
				Object.Destroy((Object)(object)worldCameraCopyGO);
				Object.Destroy((Object)(object)worldSkyboxCameraCopyGO);
				RenderTexture.ReleaseTemporary(screenshot);
				callback?.Invoke(val);
			});
		});
		worldCameraCopyGO.SetActive(true);
		worldSkyboxCameraCopyGO.SetActive(true);
		((Behaviour)worldCameraCopy).enabled = true;
		((Behaviour)worldSkyboxCameraCopy).enabled = true;
	}

	public void Update()
	{
		if (!ModsConfig.OdysseyActive)
		{
			return;
		}
		if (IsGravshipRenderInProgress)
		{
			DrawChromaKey();
		}
		if (DebugViewSettings.drawGravshipMask)
		{
			DrawChromaKey(0, null);
		}
		if (framesToWait > -1)
		{
			if (framesToWait == 0)
			{
				IsCaptureComplete = true;
				onComplete?.Invoke(captureInProgress);
				captureInProgress = null;
			}
			framesToWait--;
		}
	}

	private void DrawChromaKey()
	{
		DrawChromaKey(SubcameraDefOf.GravshipMask.LayerId, captureCam);
	}

	private void DrawChromaKey(int layer, Camera camera)
	{
		SkyOverlay.DrawScreenOverlay(MatGravshipOpaqueBase, -1f, layer, camera);
		SkyOverlay.DrawScreenOverlay(MatGravshipChromaKey, AltitudeLayer.MetaOverlays.AltitudeFor() + 0.03658537f, layer, camera);
	}
}
