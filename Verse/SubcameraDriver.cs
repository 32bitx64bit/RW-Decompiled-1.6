using UnityEngine;

namespace Verse;

public class SubcameraDriver : MonoBehaviour
{
	private Camera[] subcameras;

	public void Init()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (subcameras != null || !PlayDataLoader.Loaded)
		{
			return;
		}
		Camera camera = Find.Camera;
		subcameras = (Camera[])(object)new Camera[DefDatabase<SubcameraDef>.DefCount];
		foreach (SubcameraDef item in DefDatabase<SubcameraDef>.AllDefsListForReading)
		{
			GameObject val = new GameObject
			{
				name = item.defName
			};
			val.transform.parent = ((Component)this).transform;
			val.transform.localPosition = Vector3.zero;
			val.transform.localScale = Vector3.one;
			val.transform.localRotation = Quaternion.identity;
			Camera val2 = val.AddComponent<Camera>();
			val2.orthographic = camera.orthographic;
			val2.orthographicSize = camera.orthographicSize;
			val2.cullingMask = ((!item.layer.NullOrEmpty()) ? LayerMask.GetMask(new string[1] { item.layer }) : 0);
			val2.nearClipPlane = camera.nearClipPlane;
			val2.farClipPlane = camera.farClipPlane;
			val2.useOcclusionCulling = camera.useOcclusionCulling;
			val2.allowHDR = camera.allowHDR;
			val2.renderingPath = camera.renderingPath;
			((Behaviour)val2).enabled = item.startEnabled;
			val2.clearFlags = (CameraClearFlags)2;
			val2.backgroundColor = item.backgroundColor;
			val2.depth = item.depth;
			subcameras[item.index] = val2;
		}
	}

	public void UpdatePositions(Camera camera)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		if (subcameras == null)
		{
			return;
		}
		for (int i = 0; i < subcameras.Length; i++)
		{
			SubcameraDef subcameraDef = DefDatabase<SubcameraDef>.AllDefsListForReading[i];
			if (!subcameraDef.doNotUpdate)
			{
				Camera obj = subcameras[i];
				obj.orthographicSize = camera.orthographicSize;
				RenderTexture val = obj.targetTexture;
				if ((Object)(object)val != (Object)null && (((Texture)val).width != Screen.width || ((Texture)val).height != Screen.height))
				{
					Object.Destroy((Object)(object)val);
					val = null;
				}
				if ((Object)(object)val == (Object)null)
				{
					val = new RenderTexture(Screen.width, Screen.height, 0, subcameraDef.BestFormat);
				}
				if (!val.IsCreated())
				{
					val.Create();
				}
				obj.targetTexture = val;
			}
		}
	}

	public Camera GetSubcamera(SubcameraDef def)
	{
		if (subcameras == null || def == null || subcameras.Length <= def.index)
		{
			return null;
		}
		return subcameras[def.index];
	}
}
