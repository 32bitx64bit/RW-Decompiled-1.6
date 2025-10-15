using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public class CameraDriver : MonoBehaviour
{
	public struct DragTimeStamp
	{
		public Vector2 posDelta;

		public float time;
	}

	public CameraShaker shaker = new CameraShaker();

	private CameraPanner panner;

	private Camera cachedCamera;

	private GameObject reverbDummy;

	public CameraMapConfig config = new CameraMapConfig_Normal();

	private Vector3 velocity;

	private Vector3 rootPos;

	private float desiredSize;

	private Vector2 desiredDolly = Vector2.zero;

	private Vector2 desiredDollyRaw = Vector2.zero;

	private List<DragTimeStamp> dragTimeStamps = new List<DragTimeStamp>();

	private bool releasedLeftWhileHoldingMiddle;

	private bool mouseCoveredByUI;

	private float mouseTouchingScreenBottomEdgeStartTime = -1f;

	private float fixedTimeStepBuffer;

	private static int lastViewRectGetFrame = -1;

	private static CellRect lastViewRect;

	public const float MaxDeltaTime = 0.1f;

	private const float ScreenDollyEdgeWidth = 20f;

	private const float ScreenDollyEdgeWidth_BottomFullscreen = 6f;

	private const float MinDurationForMouseToTouchScreenBottomEdgeToDolly = 0.28f;

	private const float DragTimeStampExpireSeconds = 0.05f;

	private const float VelocityFromMouseDragInitialFactor = 0.75f;

	private const float MapEdgeClampMarginCells = -2f;

	public const float StartingSize = 24f;

	private const float ZoomTightness = 0.4f;

	private const float ZoomScaleFromAltDenominator = 35f;

	private const float PageKeyZoomRate = 4f;

	public const float MinAltitude = 15f;

	private const float NearClipPlane = 0.5f;

	private const float MaxAltitude = 65f;

	private const float ReverbDummyAltitude = 65f;

	private float rootSize;

	public const float FullDurationPanDistance = 70f;

	private static float ScrollWheelZoomRate
	{
		get
		{
			if (!SteamDeck.IsSteamDeck)
			{
				return 0.35f;
			}
			return 0.55f;
		}
	}

	private Camera MyCamera
	{
		get
		{
			if ((Object)(object)cachedCamera == (Object)null)
			{
				cachedCamera = ((Component)this).GetComponent<Camera>();
			}
			return cachedCamera;
		}
	}

	private float ScreenDollyEdgeWidthBottom
	{
		get
		{
			if (Screen.fullScreen || ResolutionUtility.BorderlessFullscreen)
			{
				return 6f;
			}
			return 20f;
		}
	}

	public float RootSize
	{
		get
		{
			return rootSize;
		}
		private set
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			if (rootSize != value)
			{
				if (Current.ProgramState != ProgramState.Playing || LongEventHandler.ShouldWaitForEvent || (Object)(object)Find.Camera == (Object)null || !Prefs.ZoomToMouse)
				{
					rootSize = value;
					return;
				}
				ApplyPositionToGameObject();
				Vector3 val = UI.MouseMapPosition();
				rootSize = value;
				ApplyPositionToGameObject();
				rootPos += val - UI.MouseMapPosition();
			}
		}
	}

	public CameraZoomRange CurrentZoom
	{
		get
		{
			if (RootSize < config.sizeRange.min + 1f)
			{
				return CameraZoomRange.Closest;
			}
			if (RootSize < config.sizeRange.max * 0.23f)
			{
				return CameraZoomRange.Close;
			}
			if (RootSize < config.sizeRange.max * 0.7f)
			{
				return CameraZoomRange.Middle;
			}
			if (RootSize < config.sizeRange.max * 0.95f)
			{
				return CameraZoomRange.Far;
			}
			return CameraZoomRange.Furthest;
		}
	}

	private Vector3 CurrentRealPosition => ((Component)MyCamera).transform.position;

	private bool AnythingPreventsCameraMotion
	{
		get
		{
			if (!Find.WindowStack.WindowsPreventCameraMotion && !WorldRendererUtility.WorldSelected)
			{
				return !Current.Game.PlayerHasControl;
			}
			return true;
		}
	}

	public IntVec3 MapPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			IntVec3 result = CurrentRealPosition.ToIntVec3();
			result.y = 0;
			return result;
		}
	}

	public Vector2 ViewSpacePosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			Vector3 currentRealPosition = CurrentRealPosition;
			float num = Mathf.InverseLerp(2f, (float)Find.CurrentMap.Size.x + -2f, currentRealPosition.x);
			float num2 = Mathf.InverseLerp(2f, (float)Find.CurrentMap.Size.z + -2f, currentRealPosition.z);
			return new Vector2(num, num2);
		}
	}

	public CellRect CurrentViewRect
	{
		get
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (Time.frameCount != lastViewRectGetFrame)
			{
				lastViewRect = default(CellRect);
				float num = (float)UI.screenWidth / (float)UI.screenHeight;
				Vector3 currentRealPosition = CurrentRealPosition;
				lastViewRect.minX = Mathf.FloorToInt(currentRealPosition.x - RootSize * num - 1f);
				lastViewRect.maxX = Mathf.CeilToInt(currentRealPosition.x + RootSize * num);
				lastViewRect.minZ = Mathf.FloorToInt(currentRealPosition.z - RootSize - 1f);
				lastViewRect.maxZ = Mathf.CeilToInt(currentRealPosition.z + RootSize);
				lastViewRectGetFrame = Time.frameCount;
			}
			return lastViewRect;
		}
	}

	public static float HitchReduceFactor
	{
		get
		{
			float result = 1f;
			if (Time.deltaTime > 0.1f)
			{
				result = 0.1f / Time.deltaTime;
			}
			return result;
		}
	}

	public float CellSizePixels => (float)UI.screenHeight / (RootSize * 2f);

	public float ZoomRootSize => RootSize;

	public Vector3 InverseFovScale
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f - (float)UI.screenHeight / (4f * RootSize * RootSize);
			return new Vector3(num, num, num);
		}
	}

	public bool InViewOf(Thing thing)
	{
		CellRect cellRect = CurrentViewRect.ExpandedBy(1);
		cellRect.ClipInsideMap(thing.MapHeld);
		return cellRect.Overlaps(thing.OccupiedDrawRect());
	}

	public void Awake()
	{
		ResetSize();
		reverbDummy = GameObject.Find("ReverbZoneDummy");
		ApplyPositionToGameObject();
		MyCamera.nearClipPlane = 0.5f;
		MyCamera.farClipPlane = 65.5f;
	}

	public void OnPreCull()
	{
		if (!LongEventHandler.ShouldWaitForEvent && Find.CurrentMap != null && WorldRendererUtility.DrawingMap)
		{
			Find.CurrentMap.weatherManager.DrawAllWeather();
		}
	}

	public void CameraDriverOnGUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Invalid comparison between Unknown and I4
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		if (Find.CurrentMap == null)
		{
			return;
		}
		if (Input.GetMouseButtonUp(0) && Input.GetMouseButton(2))
		{
			releasedLeftWhileHoldingMiddle = true;
		}
		else if ((int)Event.current.rawType == 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
		{
			releasedLeftWhileHoldingMiddle = false;
		}
		mouseCoveredByUI = Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted) != null;
		if (AnythingPreventsCameraMotion)
		{
			return;
		}
		if (UnityGUIBugsFixer.MouseDrag(2) && (!SteamDeck.IsSteamDeck || !Find.Selector.AnyPawnSelected))
		{
			Vector2 currentEventDelta = UnityGUIBugsFixer.CurrentEventDelta;
			if ((int)Event.current.type == 3)
			{
				Event.current.Use();
			}
			if (currentEventDelta != Vector2.zero)
			{
				currentEventDelta.x *= -1f;
				desiredDollyRaw += currentEventDelta / UI.CurUICellSize() * Prefs.MapDragSensitivity;
				panner.JumpOnNextUpdate();
			}
		}
		float num = 0f;
		if ((int)Event.current.type == 6)
		{
			num -= Event.current.delta.y * ScrollWheelZoomRate;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.TinyInteraction);
		}
		if (KeyBindingDefOf.MapZoom_In.KeyDownEvent)
		{
			num += 4f;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.SmallInteraction);
		}
		if (KeyBindingDefOf.MapZoom_Out.KeyDownEvent)
		{
			num -= 4f;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraZoom, KnowledgeAmount.SmallInteraction);
		}
		desiredSize -= num * config.zoomSpeed * RootSize / 35f;
		desiredSize = Mathf.Clamp(desiredSize, config.sizeRange.min, config.sizeRange.max);
		desiredDolly = Vector2.zero;
		if (KeyBindingDefOf.MapDolly_Left.IsDown)
		{
			desiredDolly.x = 0f - config.dollyRateKeys;
		}
		if (KeyBindingDefOf.MapDolly_Right.IsDown)
		{
			desiredDolly.x = config.dollyRateKeys;
		}
		if (KeyBindingDefOf.MapDolly_Up.IsDown)
		{
			desiredDolly.y = config.dollyRateKeys;
		}
		if (KeyBindingDefOf.MapDolly_Down.IsDown)
		{
			desiredDolly.y = 0f - config.dollyRateKeys;
		}
		if (desiredDolly != Vector2.zero)
		{
			panner.JumpOnNextUpdate();
		}
		config.ConfigOnGUI();
	}

	public void Update()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		if (LongEventHandler.ShouldWaitForEvent)
		{
			if ((Object)(object)Current.SubcameraDriver != (Object)null)
			{
				Current.SubcameraDriver.UpdatePositions(MyCamera);
			}
		}
		else
		{
			if (Find.CurrentMap == null)
			{
				return;
			}
			Vector2 val = CalculateCurInputDollyVect();
			if (val != Vector2.zero)
			{
				float num = (RootSize - config.sizeRange.min) / (config.sizeRange.max - config.sizeRange.min) * 0.7f + 0.3f;
				velocity = new Vector3(val.x, 0f, val.y) * num;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.CameraDolly, KnowledgeAmount.FrameInteraction);
			}
			if ((Input.GetMouseButtonUp(2) || (SteamDeck.IsSteamDeck && releasedLeftWhileHoldingMiddle)) && dragTimeStamps.Any())
			{
				Vector2 extraVelocityFromReleasingDragButton = GetExtraVelocityFromReleasingDragButton(dragTimeStamps, 0.75f);
				velocity += new Vector3(extraVelocityFromReleasingDragButton.x, 0f, extraVelocityFromReleasingDragButton.y);
				dragTimeStamps.Clear();
			}
			if (!AnythingPreventsCameraMotion)
			{
				float num2 = Time.deltaTime * HitchReduceFactor;
				rootPos += velocity * (num2 * config.moveSpeedScale);
				rootPos += new Vector3(desiredDollyRaw.x, 0f, desiredDollyRaw.y);
				dragTimeStamps.Add(new DragTimeStamp
				{
					posDelta = desiredDollyRaw,
					time = Time.time
				});
				rootPos.x = Mathf.Clamp(rootPos.x, 2f, (float)Find.CurrentMap.Size.x + -2f);
				rootPos.z = Mathf.Clamp(rootPos.z, 2f, (float)Find.CurrentMap.Size.z + -2f);
			}
			desiredDollyRaw = Vector2.zero;
			int num3 = Gen.FixedTimeStepUpdate(ref fixedTimeStepBuffer, 60f);
			for (int i = 0; i < num3; i++)
			{
				if (velocity != Vector3.zero)
				{
					velocity *= config.camSpeedDecayFactor;
					if (((Vector3)(ref velocity)).magnitude < 0.1f)
					{
						velocity = Vector3.zero;
					}
				}
				if (config.smoothZoom)
				{
					float num4 = Mathf.Lerp(RootSize, desiredSize, 0.05f);
					desiredSize += (num4 - RootSize) * config.zoomPreserveFactor;
					RootSize = num4;
				}
				else
				{
					float num5 = (desiredSize - RootSize) * 0.4f;
					desiredSize += config.zoomPreserveFactor * num5;
					RootSize += num5;
				}
				config.ConfigFixedUpdate_60(ref rootPos, ref velocity);
			}
			CameraPanner.Interpolant? interpolant = panner.Update();
			if (interpolant.HasValue)
			{
				CameraPanner.Interpolant valueOrDefault = interpolant.GetValueOrDefault();
				rootPos = valueOrDefault.Position;
				RootSize = valueOrDefault.Size;
				desiredSize = RootSize;
			}
			shaker.Update();
			ApplyPositionToGameObject();
			Current.SubcameraDriver.UpdatePositions(MyCamera);
			if (Find.CurrentMap != null)
			{
				RememberedCameraPos rememberedCameraPos = Find.CurrentMap.rememberedCameraPos;
				rememberedCameraPos.rootPos = rootPos;
				rememberedCameraPos.rootSize = RootSize;
			}
		}
	}

	private void ApplyPositionToGameObject()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)((Component)this).gameObject == (Object)null))
		{
			rootPos.y = 15f + (RootSize - config.sizeRange.min) / (config.sizeRange.max - config.sizeRange.min) * 50f;
			MyCamera.orthographicSize = RootSize;
			((Component)MyCamera).transform.position = rootPos + shaker.ShakeOffset;
			if ((Object)(object)reverbDummy != (Object)null)
			{
				Vector3 position = ((Component)this).transform.position;
				position.y = 65f;
				reverbDummy.transform.position = position;
			}
		}
	}

	private Vector2 CalculateCurInputDollyVect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = desiredDolly;
		bool flag = false;
		if ((UnityData.isEditor || Screen.fullScreen || ResolutionUtility.BorderlessFullscreen) && Prefs.EdgeScreenScroll && !mouseCoveredByUI)
		{
			Vector2 mousePositionOnUI = UI.MousePositionOnUI;
			Vector2 val2 = mousePositionOnUI;
			val2.y = (float)UI.screenHeight - val2.y;
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0f, 0f, 200f, 200f);
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector((float)(UI.screenWidth - 250), 0f, 255f, 255f);
			Rect val5 = default(Rect);
			((Rect)(ref val5))._002Ector(0f, (float)(UI.screenHeight - 250), 225f, 255f);
			Rect val6 = default(Rect);
			((Rect)(ref val6))._002Ector((float)(UI.screenWidth - 250), (float)(UI.screenHeight - 250), 255f, 255f);
			MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
			if (Find.MainTabsRoot.OpenTab == MainButtonDefOf.Inspect && mainTabWindow_Inspect.RecentHeight > ((Rect)(ref val5)).height)
			{
				((Rect)(ref val5)).yMin = (float)UI.screenHeight - mainTabWindow_Inspect.RecentHeight;
			}
			if (!((Rect)(ref val3)).Contains(val2) && !((Rect)(ref val5)).Contains(val2) && !((Rect)(ref val4)).Contains(val2) && !((Rect)(ref val6)).Contains(val2))
			{
				Vector2 val7 = default(Vector2);
				((Vector2)(ref val7))._002Ector(0f, 0f);
				if (mousePositionOnUI.x >= 0f && mousePositionOnUI.x < 20f)
				{
					val7.x -= config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.x <= (float)UI.screenWidth && mousePositionOnUI.x > (float)UI.screenWidth - 20f)
				{
					val7.x += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y <= (float)UI.screenHeight && mousePositionOnUI.y > (float)UI.screenHeight - 20f)
				{
					val7.y += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y >= 0f && mousePositionOnUI.y < ScreenDollyEdgeWidthBottom)
				{
					if (mouseTouchingScreenBottomEdgeStartTime < 0f)
					{
						mouseTouchingScreenBottomEdgeStartTime = Time.realtimeSinceStartup;
					}
					if (Time.realtimeSinceStartup - mouseTouchingScreenBottomEdgeStartTime >= 0.28f)
					{
						val7.y -= config.dollyRateScreenEdge;
					}
					flag = true;
				}
				val += val7;
			}
		}
		if (!flag)
		{
			mouseTouchingScreenBottomEdgeStartTime = -1f;
		}
		if (Input.GetKey((KeyCode)304))
		{
			val *= 2.4f;
		}
		return val;
	}

	public void Expose()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Scribe.EnterNode("cameraMap"))
		{
			try
			{
				Scribe_Values.Look(ref rootPos, "camRootPos");
				Scribe_Values.Look(ref desiredSize, "desiredSize", 0f);
				RootSize = desiredSize;
				shaker.Expose();
			}
			finally
			{
				Scribe.ExitNode();
			}
		}
	}

	public void ResetSize()
	{
		desiredSize = 24f;
		RootSize = desiredSize;
	}

	public void JumpToCurrentMapLoc(IntVec3 cell)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		JumpToCurrentMapLoc(cell.ToVector3Shifted());
	}

	public void JumpToCurrentMapLoc(Vector3 loc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		rootPos = new Vector3(loc.x, rootPos.y, loc.z);
	}

	public void PanToMapLoc(IntVec3 cell)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = cell.ToVector3Shifted();
		float x = Vector3.Distance(val, CurrentRealPosition);
		float duration = GenMath.LerpDoubleClamped(0f, 70f, 0f, 0.25f, x);
		panner.PanTo(new CameraPanner.Interpolant(rootPos, RootSize), new CameraPanner.Interpolant(new Vector3(val.x, rootPos.y, val.z), RootSize), duration);
	}

	public void PanToMapLocAndSize(Vector3 loc, float size, float duration = 0.25f, PanCompletionCallback onComplete = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float x = Vector3.Distance(loc, CurrentRealPosition);
		float duration2 = GenMath.LerpDoubleClamped(0f, 70f, 0f, duration, x);
		panner.PanTo(new CameraPanner.Interpolant(rootPos, RootSize), new CameraPanner.Interpolant(new Vector3(loc.x, rootPos.y, loc.z), size), duration2, onComplete);
	}

	public void SetRootPosAndSize(Vector3 rootPos, float rootSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		this.rootPos = rootPos;
		this.rootSize = rootSize;
		desiredDolly = Vector2.zero;
		desiredDollyRaw = Vector2.zero;
		desiredSize = RootSize;
		dragTimeStamps.Clear();
		LongEventHandler.ExecuteWhenFinished(ApplyPositionToGameObject);
	}

	public void SetRootSize(float size)
	{
		rootSize = size;
		desiredSize = RootSize;
		LongEventHandler.ExecuteWhenFinished(ApplyPositionToGameObject);
	}

	public static Vector2 GetExtraVelocityFromReleasingDragButton(List<DragTimeStamp> dragTimeStamps, float velocityFromMouseDragInitialFactor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Vector2 val = Vector2.zero;
		for (int i = 0; i < dragTimeStamps.Count; i++)
		{
			if (dragTimeStamps[i].time < Time.time - 0.05f)
			{
				num = 0.05f;
				continue;
			}
			num = Mathf.Max(num, Time.time - dragTimeStamps[i].time);
			val += dragTimeStamps[i].posDelta;
		}
		if (val != Vector2.zero && num > 0f)
		{
			return val / num * velocityFromMouseDragInitialFactor;
		}
		return Vector2.zero;
	}
}
