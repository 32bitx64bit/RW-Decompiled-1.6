using System;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class WorldCameraManager
{
	private static Camera worldCameraInt;

	private static Camera worldSkyboxCameraInt;

	private static WorldCameraDriver worldCameraDriverInt;

	public static readonly string WorldLayerName;

	public static int WorldLayerMask;

	public static int WorldLayer;

	public static readonly string WorldSkyboxLayerName;

	public static int WorldSkyboxLayerMask;

	public static int WorldSkyboxLayer;

	public const int FarClipPlane = 7500;

	private static readonly Color SkyColor;

	public static Camera WorldCamera => worldCameraInt;

	public static Camera WorldSkyboxCamera => worldSkyboxCameraInt;

	public static WorldCameraDriver WorldCameraDriver => worldCameraDriverInt;

	static WorldCameraManager()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		WorldLayerName = "World";
		WorldLayerMask = LayerMask.GetMask(new string[1] { WorldLayerName });
		WorldLayer = LayerMask.NameToLayer(WorldLayerName);
		WorldSkyboxLayerName = "WorldSkybox";
		WorldSkyboxLayerMask = LayerMask.GetMask(new string[1] { WorldSkyboxLayerName });
		WorldSkyboxLayer = LayerMask.NameToLayer(WorldSkyboxLayerName);
		SkyColor = new Color(0.0627451f, 0.09019608f, 0.11764706f);
		worldCameraInt = CreateWorldCamera();
		worldSkyboxCameraInt = CreateWorldSkyboxCamera(worldCameraInt);
		worldCameraDriverInt = ((Component)worldCameraInt).GetComponent<WorldCameraDriver>();
	}

	private static Camera CreateWorldCamera()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		GameObject val = new GameObject("WorldCamera", new Type[1] { typeof(Camera) });
		val.SetActive(false);
		val.AddComponent<WorldCameraDriver>();
		Object.DontDestroyOnLoad((Object)val);
		Camera component = val.GetComponent<Camera>();
		component.orthographic = false;
		component.cullingMask = WorldLayerMask;
		component.clearFlags = (CameraClearFlags)3;
		component.useOcclusionCulling = true;
		component.renderingPath = (RenderingPath)1;
		component.nearClipPlane = 2f;
		component.farClipPlane = 7500f;
		component.fieldOfView = 20f;
		component.depth = 1f;
		return component;
	}

	private static Camera CreateWorldSkyboxCamera(Camera parent)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("WorldSkyboxCamera", new Type[1] { typeof(Camera) });
		val.SetActive(true);
		Object.DontDestroyOnLoad((Object)val);
		Camera component = val.GetComponent<Camera>();
		((Component)component).transform.SetParent(((Component)parent).transform);
		component.orthographic = false;
		component.cullingMask = WorldSkyboxLayerMask;
		component.clearFlags = (CameraClearFlags)2;
		component.backgroundColor = SkyColor;
		component.useOcclusionCulling = false;
		component.renderingPath = (RenderingPath)1;
		component.nearClipPlane = 2f;
		component.farClipPlane = 7500f;
		component.fieldOfView = 60f;
		component.depth = 0f;
		return component;
	}
}
