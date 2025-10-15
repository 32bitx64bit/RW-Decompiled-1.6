using UnityEngine;

namespace Verse;

public abstract class SkyOverlay
{
	public Color? ForcedOverlayColor { get; protected set; }

	public static void DrawWorldOverlay(Map map, Material mat, int layer = 0)
	{
		DrawWorldOverlay(map, mat, AltitudeLayer.Weather.AltitudeFor(), layer);
	}

	public static void DrawWorldOverlay(Map map, Material mat, float altitude, int layer = 0)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = map.Center.ToVector3Shifted().WithY(altitude);
		Graphics.DrawMesh(MeshPool.wholeMapPlane, val, Quaternion.identity, mat, layer);
	}

	public static void DrawScreenOverlay(Material mat, int layer = 0, Camera camera = null)
	{
		DrawScreenOverlay(mat, AltitudeLayer.Weather.AltitudeFor() + 0.03658537f, layer, camera);
	}

	public virtual void Reset()
	{
	}

	public static void DrawScreenOverlay(Material mat, float altitude, int layer = 0, Camera camera = null)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)camera))
		{
			camera = Find.Camera;
		}
		float num = camera.orthographicSize * 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num * camera.aspect, 1f, num);
		Vector3 position = ((Component)camera).transform.position;
		position.y = altitude;
		Matrix4x4 val2 = default(Matrix4x4);
		((Matrix4x4)(ref val2)).SetTRS(position, Quaternion.identity, val);
		Graphics.DrawMesh(MeshPool.plane10, val2, mat, layer);
	}

	public abstract void TickOverlay(Map map, float lerpFactor);

	public abstract void DrawOverlay(Map map);

	public abstract void SetOverlayColor(Color color);

	public override string ToString()
	{
		return "NoOverlayOverlay";
	}
}
