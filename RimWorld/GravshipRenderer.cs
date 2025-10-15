using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class GravshipRenderer
{
	private Gravship gravship;

	private MaterialPropertyBlock distortionBlock;

	private MaterialPropertyBlock flareBlock;

	private MaterialPropertyBlock thrusterFlameBlock;

	private static EventQueue manualTicker;

	private DrawBatch drawBatch = new DrawBatch();

	private FleckSystem exhaustFleckSystem;

	private Dictionary<Thing, EventQueue> exhaustTimers = new Dictionary<Thing, EventQueue>();

	private Vector3 gravshipOffset;

	private Vector3 takeoffOrLandingPosition;

	private Vector3 takeoffOrLandingEnginePos;

	private Rot4 landingRotation;

	private Map map;

	private const float GravFieldGlowSize = 8f;

	private const float EngineGlowSize = 12.5f;

	private const float GravshipMoveSpeed = 25f;

	private const float ThrusterFlickerSpeed = 100f;

	private const float ThrusterMinBrightness = 0.75f;

	private const int ThrusterFlameRenderQueue = 3201;

	private static readonly int ShaderPropertyColor2 = Shader.PropertyToID("_Color2");

	private static readonly int ShaderPropertyGravshipHeight = Shader.PropertyToID("_GravshipHeight");

	private static readonly int ShaderPropertyIsTakeoff = Shader.PropertyToID("_IsTakeoff");

	private static readonly Material MatGravship = MatLoader.LoadMat("Map/Gravship/Gravship");

	private static readonly Material MatGravshipShadow = MatLoader.LoadMat("Map/Gravship/GravshipShadow");

	private static readonly Material MatGravshipDownwash = MatLoader.LoadMat("Map/Gravship/GravshipDownwash");

	private static readonly Material MatGravshipDistortion = MatLoader.LoadMat("Map/Gravship/GravshipDistortion");

	private static readonly Material MatGravshipLensFlare = MatLoader.LoadMat("Map/Gravship/GravshipLensFlare");

	private static readonly Material MatGravFieldExtenderGlow = MatLoader.LoadMat("Map/Gravship/GravFieldExtenderGlow");

	private static readonly Material MatGravEngineGlow = MatLoader.LoadMat("Map/Gravship/GravEngineGlow");

	public static readonly Material MatTerrainCurtain = MatLoader.LoadMat("Map/Gravship/FakeTerrain");

	public GravshipRenderer()
	{
		manualTicker = new EventQueue(1f / 60f);
	}

	public void Init(Map map)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		if (ModLister.CheckOdyssey("Gravship"))
		{
			this.map = map;
			exhaustFleckSystem = new FleckSystemThrown(map.flecks);
			flareBlock = new MaterialPropertyBlock();
			distortionBlock = new MaterialPropertyBlock();
			thrusterFlameBlock = new MaterialPropertyBlock();
		}
	}

	public void BeginCutscene(Gravship gravship, Vector3 takeoffOrLandingCenter, Vector3 takeoffOrLandingEnginePos, Rot4 landingRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		gravshipOffset = Vector3.zero;
		this.gravship = gravship;
		takeoffOrLandingPosition = takeoffOrLandingCenter;
		this.takeoffOrLandingEnginePos = takeoffOrLandingEnginePos;
		this.landingRotation = landingRotation;
		exhaustFleckSystem.RemoveAllFlecks((IFleck _) => true);
		exhaustTimers.Clear();
		foreach (Thing thruster in gravship.Thrusters)
		{
			if (thruster.TryGetComp(out CompGravshipThruster comp) && comp.Props.exhaustSettings != null && comp.Props.exhaustSettings.enabled && comp.Props.exhaustSettings.ExhaustFleckDef != null)
			{
				exhaustFleckSystem.handledDefs.AddUnique(comp.Props.exhaustSettings.ExhaustFleckDef);
				exhaustTimers.Add(thruster, new EventQueue(1f / comp.Props.exhaustSettings.emissionsPerSecond));
			}
		}
	}

	public void BeginUpdate()
	{
		exhaustFleckSystem.parent = Find.CurrentMap.flecks;
		manualTicker.Push(Time.deltaTime);
		while (manualTicker.Pop())
		{
			exhaustFleckSystem.Tick();
		}
		exhaustFleckSystem.Update(Time.deltaTime);
	}

	public void UpdateTakeoff(float progress)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Pow(progress, 3.5f);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 0f, Find.Camera.orthographicSize * 2.5f * num);
		Vector3 val2 = gravship.launchDirection.ToVector3();
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		if (Mathf.Approximately(normalized.z, -1f))
		{
			val = -val;
		}
		float num2 = ((Mathf.Approximately(normalized.x, 1f) || Mathf.Approximately(normalized.z, -1f)) ? (-1f) : 1f);
		float num3 = Mathf.SmoothStep(0f, num2 * 20f, Mathf.InverseLerp(0.2f, 1f, progress + 0.02f * Mathf.Sin(23.561945f * progress)));
		Quaternion val3 = Quaternion.Euler(0f, num3, 0f);
		gravshipOffset += val3 * normalized * (25f * Time.deltaTime * Mathf.InverseLerp(0.2f, 1f, progress));
		DrawGravshipGroundEffects(gravship.capture, ((Component)Find.Camera).transform.position, takeoffOrLandingPosition + gravshipOffset, val3, progress, num, isTakeoff: true);
		DrawGravship(gravship.capture, ((Component)Find.Camera).transform.position, takeoffOrLandingPosition + gravshipOffset + val, takeoffOrLandingEnginePos + gravshipOffset + val, val3, progress, num, isTakeoff: true);
	}

	public void UpdateLanding(float progress, bool isPollutedLanding)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		progress = progress.RemapClamped(0f, 0.95f, 0f, 1f);
		float num = Mathf.Pow(1f - progress, 5f);
		Vector3 val = default(Vector3);
		Vector3 val2;
		Vector3 val3;
		if (landingRotation == Rot4.North || landingRotation == Rot4.South)
		{
			((Vector3)(ref val))._002Ector(0f, 0f, 100f * num);
			Quaternion asQuat = landingRotation.AsQuat;
			val2 = gravship.launchDirection.ToVector3();
			val3 = asQuat * -((Vector3)(ref val2)).normalized * 200f * num;
		}
		else
		{
			((Vector3)(ref val))._002Ector(0f, 0f, 200f * num);
			Quaternion asQuat2 = landingRotation.AsQuat;
			val2 = gravship.launchDirection.ToVector3();
			val3 = asQuat2 * -((Vector3)(ref val2)).normalized * 100f * Mathf.Pow(1f - progress, 9f);
		}
		Quaternion identity = Quaternion.identity;
		DrawGravshipGroundEffects(gravship.capture, ((Component)Find.Camera).transform.position, takeoffOrLandingPosition + val3, identity, progress, num, isTakeoff: false, landingRotation);
		DrawGravship(gravship.capture, ((Component)Find.Camera).transform.position, takeoffOrLandingPosition + val + val3, takeoffOrLandingEnginePos + val + val3, identity, progress, num, isTakeoff: false, landingRotation, isPollutedLanding);
	}

	public void EndUpdate()
	{
		exhaustFleckSystem.ForceDraw(drawBatch);
		drawBatch.Flush();
	}

	private void DrawGravship(Capture capture, Vector3 cameraPosition, Vector3 gravshipCenter, Vector3 gravEnginePos, Quaternion gravshipRotation, float cutsceneProgressPercent, float cutsceneHeightPercent, bool isTakeoff, Rot4 landingRotation = default(Rot4), bool isPollutedLanding = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = gravEnginePos + gravship.Engine.def.Size.ToVector3() * 0.25f;
		Vector3 val = Find.Camera.WorldToViewportPoint(RotateAroundPivot(position, gravshipCenter, gravshipRotation));
		distortionBlock.SetFloat(ShaderPropertyIDs.Progress, cutsceneProgressPercent);
		distortionBlock.SetFloat(ShaderPropertyGravshipHeight, cutsceneHeightPercent);
		distortionBlock.SetVector(ShaderPropertyIDs.DrawPos, Vector4.op_Implicit(val));
		distortionBlock.SetFloat(ShaderPropertyIsTakeoff, isTakeoff ? 1f : 0f);
		DrawLayer(MatGravshipDistortion, cameraPosition.SetToAltitude(AltitudeLayer.Weather).WithYOffset(0.07317074f), distortionBlock);
		MatGravship.SetFloat(ShaderPropertyIDs.Progress, cutsceneProgressPercent);
		MatGravship.SetFloat(ShaderPropertyGravshipHeight, cutsceneHeightPercent);
		MatGravship.SetFloat(ShaderPropertyIsTakeoff, isTakeoff ? 1f : 0f);
		MatGravship.color = (isPollutedLanding ? Color.white.WithAlpha(cutsceneProgressPercent.Remap(0.9f, 1f, 1f, 0f)) : Color.white);
		MatGravship.mainTexture = (Texture)(object)(Texture2D)capture.capture;
		GenDraw.DrawQuad(MatGravship, gravshipCenter.SetToAltitude(AltitudeLayer.Skyfaller), gravshipRotation, capture.drawSize);
		foreach (LayerSubMesh bakedIndoorMask in gravship.bakedIndoorMasks)
		{
			Graphics.DrawMesh(bakedIndoorMask.mesh, Matrix4x4.TRS(gravshipCenter + Altitudes.AltIncVect * 2f, gravshipRotation, Vector3.one), bakedIndoorMask.material, 0);
		}
		if ((isTakeoff && cutsceneProgressPercent <= 0f) || (!isTakeoff && cutsceneProgressPercent >= 1f))
		{
			return;
		}
		Color val2 = default(Color);
		((Color)(ref val2))._002Ector(1f, 1f, 1f, 1f);
		val2 *= Mathf.Lerp(0.75f, 1f, Mathf.PerlinNoise1D(cutsceneProgressPercent * 100f));
		val2.a = Mathf.InverseLerp(0f, 0.1f, isTakeoff ? cutsceneProgressPercent : (1f - cutsceneProgressPercent));
		MatGravshipLensFlare.SetColor(ShaderPropertyColor2, val2);
		foreach (KeyValuePair<Thing, PositionData.Data> thrusterPlacement in gravship.ThrusterPlacements)
		{
			thrusterPlacement.Deconstruct(out var key, out var value);
			Thing thing = key;
			PositionData.Data data = value;
			IntVec3 intVec = -gravship.launchDirection;
			Rot4 rotation = data.rotation;
			if (rotation.AsIntVec3 == intVec)
			{
				continue;
			}
			CompProperties_GravshipThruster props = thing.TryGetComp<CompGravshipThruster>().Props;
			float num = (float)thing.def.size.x * props.flameSize;
			Vector3 val3 = thing.Rotation.AsQuat * props.flameOffsetsPerDirection[thing.Rotation.AsInt];
			Vector3 val4 = GenThing.TrueCenter(data.local, thing.Rotation, thing.def.size, 0f) - thing.Rotation.AsIntVec3.ToVector3() * ((float)thing.def.size.z * 0.5f + num * 0.5f) + val3;
			Vector3 val5 = RotateAroundPivot(gravEnginePos + val4, gravshipCenter, gravshipRotation).SetToAltitude(AltitudeLayer.Skyfaller).WithYOffset(0.07317074f);
			MaterialRequest req = new MaterialRequest(props.FlameShaderType.Shader);
			req.renderQueue = 3201;
			Material mat = MaterialPool.MatFrom(req);
			thrusterFlameBlock.Clear();
			thrusterFlameBlock.SetColor(ShaderPropertyColor2, val2);
			foreach (ShaderParameter flameShaderParameter in props.flameShaderParameters)
			{
				flameShaderParameter.Apply(thrusterFlameBlock);
			}
			rotation = data.rotation;
			GenDraw.DrawQuad(mat, val5, gravshipRotation * rotation.AsQuat, num, thrusterFlameBlock);
			Vector3 val6 = Find.Camera.WorldToViewportPoint(val5);
			flareBlock.SetVector(ShaderPropertyIDs.DrawPos, Vector4.op_Implicit(val6));
			DrawLayer(MatGravshipLensFlare, cameraPosition.SetToAltitude(AltitudeLayer.MetaOverlays).WithYOffset(0.03658537f), flareBlock);
			if (props.exhaustSettings.enabled)
			{
				EventQueue eventQueue = exhaustTimers[thing];
				eventQueue.Push(Time.deltaTime);
				while (eventQueue.Pop())
				{
					CompProperties_GravshipThruster.ExhaustSettings exhaustSettings = props.exhaustSettings;
					rotation = data.rotation;
					EmitSmoke(exhaustSettings, val5, gravshipRotation, rotation.AsQuat);
				}
			}
		}
		MatGravFieldExtenderGlow.SetColor(ShaderPropertyColor2, val2);
		foreach (IntVec3 gravFieldExtenderPosition in gravship.GravFieldExtenderPositions)
		{
			Vector3 val7 = gravFieldExtenderPosition.ToVector3() + ThingDefOf.GravFieldExtender.graphicData.drawSize.ToVector3() * 0.5f;
			Vector3 position2 = RotateAroundPivot(gravEnginePos + val7, gravshipCenter, gravshipRotation).SetToAltitude(AltitudeLayer.MetaOverlays).WithYOffset(0.07317074f);
			GenDraw.DrawQuad(MatGravFieldExtenderGlow, position2, Quaternion.identity, 8f);
		}
		MatGravEngineGlow.SetColor(ShaderPropertyColor2, val2);
		Vector3 position3 = RotateAroundPivot(position, gravshipCenter, gravshipRotation).SetToAltitude(AltitudeLayer.MetaOverlays).WithYOffset(0.07317074f);
		GenDraw.DrawQuad(MatGravEngineGlow, position3, Quaternion.identity, 12.5f);
	}

	private void DrawGravshipGroundEffects(Capture capture, Vector3 cameraCenter, Vector3 groundCenter, Quaternion rotation, float progress, float height, bool isTakeoff, Rot4 landingRotation = default(Rot4))
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (progress > 0f && !map.Biome.inVacuum)
		{
			MatGravshipDownwash.SetFloat(ShaderPropertyIDs.Progress, progress);
			MatGravshipDownwash.SetFloat(ShaderPropertyGravshipHeight, height);
			MatGravshipDownwash.SetVector(ShaderPropertyIDs.DrawPos, Vector4.op_Implicit(Find.Camera.WorldToViewportPoint(groundCenter)));
			MatGravshipDownwash.SetFloat(ShaderPropertyIsTakeoff, isTakeoff ? 1f : 0f);
			DrawLayer(MatGravshipDownwash, cameraCenter.SetToAltitude(AltitudeLayer.Gas).WithYOffset(0.03658537f));
		}
		MatGravshipShadow.SetFloat(ShaderPropertyIDs.Progress, 1f - progress);
		MatGravshipShadow.SetFloat(ShaderPropertyGravshipHeight, height);
		MatGravshipShadow.SetFloat(ShaderPropertyIsTakeoff, isTakeoff ? 1f : 0f);
		MatGravshipShadow.color = MatGravshipShadow.color.WithAlpha(progress.RemapClamped(0.9f, 1f, 1f, 0f));
		MatGravshipShadow.mainTexture = (Texture)(object)(Texture2D)capture.capture;
		GenDraw.DrawQuad(MatGravshipShadow, groundCenter.SetToAltitude(AltitudeLayer.Gas).WithYOffset(0.03658537f), rotation, capture.drawSize * 1.05f);
	}

	private void EmitSmoke(CompProperties_GravshipThruster.ExhaustSettings settings, Vector3 position, Quaternion gravshipRotation, Quaternion thrusterRotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.identity;
		if (settings.inheritThrusterRotation)
		{
			val = thrusterRotation * val;
		}
		if (settings.inheritGravshipRotation)
		{
			val = gravshipRotation * val;
		}
		FleckSystem fleckSystem = exhaustFleckSystem;
		FleckCreationData fleckData = new FleckCreationData
		{
			def = settings.ExhaustFleckDef
		};
		Vector3 val2 = position + val * settings.spawnOffset;
		Vector3 val3 = Random.insideUnitSphere.WithY(0f);
		fleckData.spawnPosition = val2 + ((Vector3)(ref val3)).normalized * settings.spawnRadiusRange.RandomInRange;
		fleckData.scale = settings.scaleRange.RandomInRange;
		fleckData.velocity = val * Quaternion.Euler(0f, settings.velocityRotationRange.RandomInRange, 0f) * (settings.velocity * settings.velocityMultiplierRange.RandomInRange);
		fleckData.rotationRate = settings.rotationOverTimeRange.RandomInRange;
		fleckData.ageTicksOverride = -1;
		fleckSystem.CreateFleck(fleckData);
	}

	private Vector3 RotateAroundPivot(Vector3 position, Vector3 pivot, Quaternion rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return rotation * (position - pivot) + pivot;
	}

	private void DrawLayer(Material mat, Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DrawLayer(mat, position, Quaternion.identity);
	}

	private void DrawLayer(Material mat, Vector3 position, Quaternion rotation)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float num = Find.Camera.orthographicSize * 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num * Find.Camera.aspect, 1f, num);
		Matrix4x4 val2 = Matrix4x4.TRS(position, rotation, val);
		Graphics.DrawMesh(MeshPool.plane10, val2, mat, 0);
	}

	private void DrawLayer(Material mat, Vector3 position, MaterialPropertyBlock props)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float num = Find.Camera.orthographicSize * 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num * Find.Camera.aspect, 1f, num);
		Matrix4x4 val2 = Matrix4x4.TRS(position, Quaternion.identity, val);
		Graphics.DrawMesh(MeshPool.plane10, val2, mat, 0, (Camera)null, 0, props);
	}
}
