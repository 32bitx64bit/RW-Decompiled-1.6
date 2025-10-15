using UnityEngine;

namespace Verse;

public static class CellRenderer
{
	private static int lastCameraUpdateFrame = -1;

	private static CellRect viewRect;

	private static void InitFrame()
	{
		if (Time.frameCount != lastCameraUpdateFrame)
		{
			viewRect = Find.CameraDriver.CurrentViewRect;
			lastCameraUpdateFrame = Time.frameCount;
		}
	}

	private static Material MatFromColorPct(float colorPct, bool transparent)
	{
		return DebugMatsSpectrum.Mat(GenMath.PositiveMod(Mathf.RoundToInt(colorPct * 100f), 100), transparent);
	}

	public static void RenderCell(IntVec3 c, float colorPct = 0.5f)
	{
		RenderCell(c, MatFromColorPct(colorPct, transparent: true));
	}

	public static void RenderCell(IntVec3 c, Material mat)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		InitFrame();
		if (viewRect.Contains(c))
		{
			Vector3 val = c.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
			Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, mat, 0);
		}
	}

	public static void RenderSpot(IntVec3 c, float colorPct = 0.5f, float scale = 0.15f)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		RenderSpot(c.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays), colorPct, scale);
	}

	public static void RenderSpot(Vector3 loc, float colorPct = 0.5f, float scale = 0.15f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		RenderSpot(loc, MatFromColorPct(colorPct, transparent: false), scale);
	}

	public static void RenderSpot(Vector3 loc, Material mat, float scale = 0.15f)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		InitFrame();
		if (viewRect.Contains(loc.ToIntVec3()))
		{
			loc.y = AltitudeLayer.MetaOverlays.AltitudeFor();
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(scale, 1f, scale);
			Matrix4x4 val2 = default(Matrix4x4);
			((Matrix4x4)(ref val2)).SetTRS(loc, Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.circle, val2, mat, 0);
		}
	}
}
