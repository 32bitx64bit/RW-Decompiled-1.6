using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_UnnaturalDarkness : WeatherOverlayDualPanner
{
	private static readonly Material DarkParticlesOverlayWorld = MatLoader.LoadMat("Weather/DarknessOverlayWorld");

	public WeatherOverlay_UnnaturalDarkness()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = DarkParticlesOverlayWorld;
		worldOverlayPanSpeed1 = 0.004f;
		Vector2 val = new Vector2(-0.4f, -0.6f);
		worldPanDir1 = ((Vector2)(ref val)).normalized;
		worldOverlayPanSpeed2 = 0.005f;
		val = new Vector2(0.2f, -0.8f);
		worldPanDir2 = ((Vector2)(ref val)).normalized;
	}
}
