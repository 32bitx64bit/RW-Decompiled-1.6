using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace RimWorld.Planet;

public class WorldCameraDriver : MonoBehaviour
{
	public WorldCameraConfig config = new WorldCameraConfig_Normal();

	public Quaternion sphereRotation = Quaternion.identity;

	private Vector2 rotationVelocity;

	private Vector2 desiredRotation;

	private Vector2 desiredRotationRaw;

	private float desiredAltitude;

	public float altitude;

	private List<CameraDriver.DragTimeStamp> dragTimeStamps = new List<CameraDriver.DragTimeStamp>();

	private bool releasedLeftWhileHoldingMiddle;

	private float layerAltitudeOffset;

	private float layerOffsetVel;

	private Vector3 layerOriginOffset;

	private Vector3 layerOriginOffsetVel;

	private float altitudeDecay;

	private float altitudeDecayVel;

	private PlanetLayer prevSelectedLayer;

	private Camera cachedCamera;

	private bool mouseCoveredByUI;

	private float mouseTouchingScreenBottomEdgeStartTime = -1f;

	private float fixedTimeStepBuffer;

	private Quaternion rotationAnimation_prevSphereRotation = Quaternion.identity;

	private float rotationAnimation_lerpFactor = 1f;

	private const float SphereRadius = 100f;

	private const float ScreenDollyEdgeWidth = 20f;

	private const float ScreenDollyEdgeWidth_BottomFullscreen = 6f;

	private const float MinDurationForMouseToTouchScreenBottomEdgeToDolly = 0.28f;

	private const float MaxXRotationAtMinAltitude = 88.6f;

	private const float MaxXRotationAtMaxAltitude = 78f;

	private const float TileSizeToRotationSpeed = 0.273f;

	private const float VelocityFromMouseDragInitialFactor = 5f;

	private const float StartingAltitude_Playing = 160f;

	private const float StartingAltitude_Entry = 550f;

	private const float MaxAltitude = 1100f;

	private const float ZoomTightness = 0.4f;

	private const float ZoomScaleFromAltDenominator = 12f;

	private const float PageKeyZoomRate = 2f;

	private const float ScrollWheelZoomRate = 0.1f;

	private PlanetLayer lastSelectedLayer;

	public static float MinAltitude => 100f + (SteamDeck.IsSteamDeck ? 17f : 25f);

	public float TrueAltitude => layerAltitudeOffset + altitude;

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

	public WorldCameraZoomRange CurrentZoom
	{
		get
		{
			float altitudePercent = AltitudePercent;
			if (altitudePercent < 0.025f)
			{
				return WorldCameraZoomRange.VeryClose;
			}
			if (altitudePercent < 0.042f)
			{
				return WorldCameraZoomRange.Close;
			}
			if (altitudePercent < 0.125f)
			{
				return WorldCameraZoomRange.Far;
			}
			return WorldCameraZoomRange.VeryFar;
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

	public Vector3 CameraPosition => ((Component)MyCamera).transform.position;

	public float AltitudePercent => Mathf.InverseLerp(MinAltitude, 1100f, altitude);

	public Vector3 CurrentlyLookingAtPointOnSphere => -(Quaternion.Inverse(sphereRotation) * Vector3.forward);

	private bool AnythingPreventsCameraMotion
	{
		get
		{
			if (!Find.WindowStack.WindowsPreventCameraMotion)
			{
				return !WorldRendererUtility.WorldSelected;
			}
			return true;
		}
	}

	public void Awake()
	{
		ResetAltitude();
		ApplyPositionToGameObject();
	}

	public void WorldCameraDriverOnGUI()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Invalid comparison between Unknown and I4
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetMouseButtonUp(0) && Input.GetMouseButton(2))
		{
			releasedLeftWhileHoldingMiddle = true;
		}
		else if ((int)Event.current.rawType == 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
		{
			releasedLeftWhileHoldingMiddle = false;
		}
		mouseCoveredByUI = false;
		if (Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted) != null)
		{
			mouseCoveredByUI = true;
		}
		if (WorldRendererUtility.WorldBackgroundNow)
		{
			ApplyPositionToGameObject();
		}
		else
		{
			if (Find.World == null)
			{
				return;
			}
			if (prevSelectedLayer != null && Find.WorldSelector.SelectedLayer != prevSelectedLayer)
			{
				altitudeDecay = 0f;
			}
			if (AnythingPreventsCameraMotion)
			{
				return;
			}
			if (UnityGUIBugsFixer.MouseDrag(2) && (!SteamDeck.IsSteamDeck || !Find.WorldSelector.AnyCaravanSelected))
			{
				Vector2 currentEventDelta = UnityGUIBugsFixer.CurrentEventDelta;
				if ((int)Event.current.type == 3)
				{
					Event.current.Use();
				}
				if (currentEventDelta != Vector2.zero)
				{
					PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.FrameInteraction);
					currentEventDelta.x *= -1f;
					desiredRotationRaw += currentEventDelta / GenWorldUI.CurUITileSize() * (0.273f * Prefs.MapDragSensitivity);
				}
			}
			float num = 0f;
			if ((int)Event.current.type == 6)
			{
				num -= Event.current.delta.y * 0.1f;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			if (KeyBindingDefOf.MapZoom_In.KeyDownEvent)
			{
				num += 2f;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			if (KeyBindingDefOf.MapZoom_Out.KeyDownEvent)
			{
				num -= 2f;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			float num2 = desiredAltitude - num * config.zoomSpeed * TrueAltitude / 12f;
			if (Current.ProgramState == ProgramState.Playing && Prefs.ZoomSwitchWorldLayer)
			{
				if (num2 < MinAltitude)
				{
					PlanetLayer planetLayer = PlanetLayer.Selected?.zoomInToLayer;
					if (planetLayer != null)
					{
						float num3 = layerAltitudeOffset + num2 - planetLayer.ExtraCameraAltitude;
						num2 = (altitude = num3);
						altitudeDecay = planetLayer.ExtraCameraAltitude - layerAltitudeOffset;
						PlanetLayer.Selected = planetLayer;
					}
				}
				else if (num2 > 1100f)
				{
					PlanetLayer planetLayer2 = PlanetLayer.Selected?.zoomOutToLayer;
					if (planetLayer2 != null)
					{
						float num4 = layerAltitudeOffset + num2 - planetLayer2.ExtraCameraAltitude;
						num2 = (altitude = num4);
						altitudeDecay = planetLayer2.ExtraCameraAltitude - layerAltitudeOffset;
						PlanetLayer.Selected = planetLayer2;
					}
				}
			}
			desiredAltitude = Mathf.Clamp(num2, MinAltitude, 1100f);
			desiredRotation = Vector2.zero;
			if (KeyBindingDefOf.MapDolly_Left.IsDown)
			{
				desiredRotation.x = 0f - config.dollyRateKeys;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			if (KeyBindingDefOf.MapDolly_Right.IsDown)
			{
				desiredRotation.x = config.dollyRateKeys;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			if (KeyBindingDefOf.MapDolly_Up.IsDown)
			{
				desiredRotation.y = config.dollyRateKeys;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			if (KeyBindingDefOf.MapDolly_Down.IsDown)
			{
				desiredRotation.y = 0f - config.dollyRateKeys;
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
			}
			config.ConfigOnGUI();
			prevSelectedLayer = Find.WorldSelector.SelectedLayer;
		}
	}

	public void Update()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		if (LongEventHandler.ShouldWaitForEvent)
		{
			return;
		}
		if (Find.World == null)
		{
			((Component)MyCamera).gameObject.SetActive(false);
			return;
		}
		if (!Find.WorldInterface.everReset)
		{
			Find.WorldInterface.Reset();
		}
		if (WorldRendererUtility.WorldBackgroundNow)
		{
			ApplyMapPositionToGameObject();
			return;
		}
		Vector2 val = CalculateCurInputDollyVect();
		if (val != Vector2.zero)
		{
			float num = (altitude - MinAltitude) / (1100f - MinAltitude) * 0.85f + 0.15f;
			rotationVelocity = new Vector2(val.x, val.y) * num;
		}
		if ((Input.GetMouseButtonUp(2) || (SteamDeck.IsSteamDeck && releasedLeftWhileHoldingMiddle)) && dragTimeStamps.Any())
		{
			rotationVelocity += CameraDriver.GetExtraVelocityFromReleasingDragButton(dragTimeStamps, 5f);
			dragTimeStamps.Clear();
		}
		if (!AnythingPreventsCameraMotion)
		{
			float num2 = Time.deltaTime * CameraDriver.HitchReduceFactor;
			sphereRotation *= Quaternion.AngleAxis(rotationVelocity.x * num2 * config.rotationSpeedScale, ((Component)MyCamera).transform.up);
			sphereRotation *= Quaternion.AngleAxis((0f - rotationVelocity.y) * num2 * config.rotationSpeedScale, ((Component)MyCamera).transform.right);
			if (desiredRotationRaw != Vector2.zero)
			{
				sphereRotation *= Quaternion.AngleAxis(desiredRotationRaw.x, ((Component)MyCamera).transform.up);
				sphereRotation *= Quaternion.AngleAxis(0f - desiredRotationRaw.y, ((Component)MyCamera).transform.right);
			}
			dragTimeStamps.Add(new CameraDriver.DragTimeStamp
			{
				posDelta = desiredRotationRaw,
				time = Time.time
			});
		}
		desiredRotationRaw = Vector2.zero;
		int num3 = Gen.FixedTimeStepUpdate(ref fixedTimeStepBuffer, 60f);
		for (int i = 0; i < num3; i++)
		{
			if (rotationVelocity != Vector2.zero)
			{
				rotationVelocity *= config.camRotationDecayFactor;
				if (((Vector2)(ref rotationVelocity)).magnitude < 0.05f)
				{
					rotationVelocity = Vector2.zero;
				}
			}
			if (config.smoothZoom)
			{
				float num4 = Mathf.Lerp(altitude, desiredAltitude, 0.05f);
				desiredAltitude += (num4 - altitude) * config.zoomPreserveFactor;
				altitude = num4;
			}
			else
			{
				float num5 = (desiredAltitude - altitude) * 0.4f;
				desiredAltitude += config.zoomPreserveFactor * num5;
				altitude += num5;
			}
		}
		rotationAnimation_lerpFactor += Time.deltaTime * 8f;
		if (Find.PlaySettings.lockNorthUp)
		{
			RotateSoNorthIsUp(interpolate: false);
			ClampXRotation(ref sphereRotation);
		}
		for (int j = 0; j < num3; j++)
		{
			config.ConfigFixedUpdate_60(ref rotationVelocity);
		}
		ApplyPositionToGameObject();
	}

	public void ApplyMapPositionToGameObject()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		Map currentMap = Find.CurrentMap;
		if (currentMap == null)
		{
			return;
		}
		Vector3 val = ((!(currentMap.ParentHolder is MapParent mapParent)) ? Find.WorldGrid.GetTileCenter(currentMap.Tile) : mapParent.WorldCameraPosition);
		if (val == Vector3.zero)
		{
			return;
		}
		Vector3 val2 = -((Vector3)(ref val)).normalized;
		val += -val2 * currentMap.Tile.Layer.BackgroundWorldCameraOffset;
		Transform transform = ((Component)MyCamera).transform;
		Quaternion rotation = Quaternion.LookRotation(val2, Vector3.up);
		transform.rotation = rotation;
		float num = currentMap.Tile.Layer.BackgroundWorldCameraParallaxDistancePer100Cells;
		if (num == 0f)
		{
			transform.position = val;
			return;
		}
		Vector2 viewSpacePosition = Find.CameraDriver.ViewSpacePosition;
		IntVec3 size = Find.CurrentMap.Size;
		float num2 = 1f;
		float num3 = 1f;
		if (size.x > size.z)
		{
			num3 = (float)size.z / (float)size.x;
			num = num * (float)size.x / 100f;
		}
		else if (size.z > size.x)
		{
			num2 = (float)size.x / (float)size.z;
			num = num * (float)size.z / 100f;
		}
		Vector3 up = transform.up;
		Vector3 right = transform.right;
		Vector3 val3 = up * (viewSpacePosition.y * num * num3) - up * num / 2f * num3;
		Vector3 val4 = right * (viewSpacePosition.x * num * num2) - right * num / 2f * num2;
		transform.position = val + val3 + val4 + currentMap.Tile.Layer.Origin;
	}

	private void ApplyPositionToGameObject()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		Quaternion invRot = ((!(rotationAnimation_lerpFactor < 1f)) ? sphereRotation : Quaternion.Lerp(rotationAnimation_prevSphereRotation, sphereRotation, rotationAnimation_lerpFactor));
		if (Find.PlaySettings.lockNorthUp)
		{
			ClampXRotation(ref invRot);
		}
		float num = 0f;
		Vector3 val = Vector3.zero;
		if (PlanetLayer.Selected != null)
		{
			num = PlanetLayer.Selected.ExtraCameraAltitude;
			val = PlanetLayer.Selected.Origin;
		}
		float num2 = layerAltitudeOffset;
		layerAltitudeOffset = Mathf.SmoothDamp(layerAltitudeOffset, num, ref layerOffsetVel, 0.1f);
		layerOriginOffset = Vector3.SmoothDamp(layerOriginOffset, val, ref layerOriginOffsetVel, 0.1f);
		float num3 = layerAltitudeOffset - num2;
		if (!Mathf.Approximately(altitudeDecay, 0f))
		{
			altitudeDecay -= num3;
		}
		else
		{
			altitudeDecay = 0f;
		}
		Transform transform = ((Component)MyCamera).transform;
		transform.rotation = Quaternion.Inverse(invRot);
		Vector3 val2 = transform.rotation * Vector3.forward;
		transform.position = -val2 * (altitude + layerAltitudeOffset + altitudeDecay) + layerOriginOffset;
	}

	private Vector2 CalculateCurInputDollyVect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = desiredRotation;
		bool flag = false;
		if ((UnityData.isEditor || Screen.fullScreen || ResolutionUtility.BorderlessFullscreen) && Prefs.EdgeScreenScroll && !mouseCoveredByUI)
		{
			Vector2 mousePositionOnUI = UI.MousePositionOnUI;
			Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector((float)(UI.screenWidth - 250), 0f, 255f, 255f);
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0f, (float)(UI.screenHeight - 250), 225f, 255f);
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector((float)(UI.screenWidth - 250), (float)(UI.screenHeight - 250), 255f, 255f);
			WorldInspectPane inspectPane = Find.World.UI.inspectPane;
			if (Find.WindowStack.IsOpen<WorldInspectPane>() && inspectPane.RecentHeight > ((Rect)(ref val3)).height)
			{
				((Rect)(ref val3)).yMin = (float)UI.screenHeight - inspectPane.RecentHeight;
			}
			if (!((Rect)(ref val3)).Contains(mousePositionOnUIInverted) && !((Rect)(ref val4)).Contains(mousePositionOnUIInverted) && !((Rect)(ref val2)).Contains(mousePositionOnUIInverted))
			{
				Vector2 zero = Vector2.zero;
				if (mousePositionOnUI.x >= 0f && mousePositionOnUI.x < 20f)
				{
					zero.x -= config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.x <= (float)UI.screenWidth && mousePositionOnUI.x > (float)UI.screenWidth - 20f)
				{
					zero.x += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y <= (float)UI.screenHeight && mousePositionOnUI.y > (float)UI.screenHeight - 20f)
				{
					zero.y += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y >= 0f && mousePositionOnUI.y < ScreenDollyEdgeWidthBottom)
				{
					if (mouseTouchingScreenBottomEdgeStartTime < 0f)
					{
						mouseTouchingScreenBottomEdgeStartTime = Time.realtimeSinceStartup;
					}
					if (Time.realtimeSinceStartup - mouseTouchingScreenBottomEdgeStartTime >= 0.28f)
					{
						zero.y -= config.dollyRateScreenEdge;
					}
					flag = true;
				}
				val += zero;
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

	public void ResetAltitude()
	{
		if (Current.ProgramState == ProgramState.Playing)
		{
			altitude = 160f;
		}
		else
		{
			altitude = 550f;
		}
		desiredAltitude = altitude;
	}

	public void JumpTo(Vector3 newLookAt)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!Find.WorldInterface.everReset)
		{
			Find.WorldInterface.Reset();
		}
		if (newLookAt != Vector3.zero)
		{
			sphereRotation = Quaternion.Inverse(Quaternion.LookRotation(-((Vector3)(ref newLookAt)).normalized));
		}
	}

	public void JumpTo(PlanetTile tile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		JumpTo(Find.WorldGrid.GetTileCenter(tile));
		PlanetLayer.Selected = tile.Layer;
	}

	public void RotateSoNorthIsUp(bool interpolate = true)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (interpolate)
		{
			rotationAnimation_prevSphereRotation = sphereRotation;
		}
		sphereRotation = Quaternion.Inverse(Quaternion.LookRotation(Quaternion.Inverse(sphereRotation) * Vector3.forward));
		if (interpolate)
		{
			rotationAnimation_lerpFactor = 0f;
		}
	}

	private void ClampXRotation(ref Quaternion invRot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.Inverse(invRot);
		Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
		float altitudePercent = AltitudePercent;
		float num = Mathf.Lerp(88.6f, 78f, altitudePercent);
		bool flag = false;
		if (eulerAngles.x <= 90f)
		{
			if (eulerAngles.x > num)
			{
				eulerAngles.x = num;
				flag = true;
			}
		}
		else if (eulerAngles.x < 360f - num)
		{
			eulerAngles.x = 360f - num;
			flag = true;
		}
		if (flag)
		{
			invRot = Quaternion.Inverse(Quaternion.Euler(eulerAngles));
		}
	}
}
