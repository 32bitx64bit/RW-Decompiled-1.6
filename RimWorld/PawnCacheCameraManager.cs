using System;
using UnityEngine;

namespace RimWorld;

public static class PawnCacheCameraManager
{
	private static Camera pawnCacheCameraInt;

	private static PawnCacheRenderer pawnCacheRendererInt;

	public static Camera PawnCacheCamera => pawnCacheCameraInt;

	public static PawnCacheRenderer PawnCacheRenderer => pawnCacheRendererInt;

	static PawnCacheCameraManager()
	{
		pawnCacheCameraInt = CreatePawnCacheCamera();
		pawnCacheRendererInt = ((Component)pawnCacheCameraInt).GetComponent<PawnCacheRenderer>();
	}

	private static Camera CreatePawnCacheCamera()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("PortraitCamera", new Type[1] { typeof(Camera) });
		val.SetActive(false);
		val.AddComponent<PawnCacheRenderer>();
		Object.DontDestroyOnLoad((Object)val);
		Camera component = val.GetComponent<Camera>();
		((Component)component).transform.position = new Vector3(0f, 10f, 0f);
		((Component)component).transform.rotation = Quaternion.Euler(90f, 0f, 0f);
		component.orthographic = true;
		component.cullingMask = 0;
		component.orthographicSize = 1f;
		component.clearFlags = (CameraClearFlags)2;
		component.backgroundColor = new Color(0f, 0f, 0f, 0f);
		component.useOcclusionCulling = false;
		component.renderingPath = (RenderingPath)1;
		component.nearClipPlane = 5f;
		component.farClipPlane = 12f;
		return component;
	}
}
