using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_Fog : WeatherOverlayDualPanner
{
	private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/FogOverlayWorld");

	public WeatherOverlay_Fog()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = FogOverlayWorld;
		worldOverlayPanSpeed1 = 0.0005f;
		worldOverlayPanSpeed2 = 0.0004f;
		worldPanDir1 = new Vector2(1f, 1f);
		worldPanDir2 = new Vector2(1f, -1f);
	}
}
