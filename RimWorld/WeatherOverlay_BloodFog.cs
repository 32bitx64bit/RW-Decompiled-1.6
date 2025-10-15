using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_BloodFog : WeatherOverlayDualPanner
{
	private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/BloodFogOverlayWorld");

	public WeatherOverlay_BloodFog()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = FogOverlayWorld;
		worldOverlayPanSpeed1 = 0.0005f;
		worldOverlayPanSpeed2 = 0.0004f;
		worldPanDir1 = new Vector2(1f, 1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldPanDir2 = new Vector2(0.5f, -0.1f);
		((Vector2)(ref worldPanDir2)).Normalize();
		base.ForcedOverlayColor = new Color(0.6f, 0f, 0f);
	}
}
