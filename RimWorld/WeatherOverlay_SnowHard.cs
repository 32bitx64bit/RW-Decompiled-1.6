using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_SnowHard : WeatherOverlayDualPanner
{
	private static readonly Material SnowOverlayWorld = MatLoader.LoadMat("Weather/SnowOverlayWorld");

	public WeatherOverlay_SnowHard()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = SnowOverlayWorld;
		worldOverlayPanSpeed1 = 0.008f;
		worldPanDir1 = new Vector2(-0.5f, -1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldOverlayPanSpeed2 = 0.009f;
		worldPanDir2 = new Vector2(-0.48f, -1f);
		((Vector2)(ref worldPanDir2)).Normalize();
	}
}
