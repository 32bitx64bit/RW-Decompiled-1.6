using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_SnowGentle : WeatherOverlayDualPanner
{
	private static readonly Material SnowGentleOverlayWorld = MatLoader.LoadMat("Weather/SnowOverlayWorld");

	public WeatherOverlay_SnowGentle()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = SnowGentleOverlayWorld;
		worldOverlayPanSpeed1 = 0.002f;
		worldPanDir1 = new Vector2(-0.25f, -1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldOverlayPanSpeed2 = 0.003f;
		worldPanDir2 = new Vector2(-0.24f, -1f);
		((Vector2)(ref worldPanDir2)).Normalize();
	}
}
