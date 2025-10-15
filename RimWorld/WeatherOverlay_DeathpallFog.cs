using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_DeathpallFog : WeatherOverlayDualPanner
{
	private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/DeathpallFogOverlayWorld");

	public WeatherOverlay_DeathpallFog()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = FogOverlayWorld;
		worldOverlayPanSpeed1 = 0.001f;
		worldOverlayPanSpeed2 = 0.0005f;
		worldPanDir1 = new Vector2(1f, 0.2f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldPanDir2 = new Vector2(-1f, -0.2f);
		((Vector2)(ref worldPanDir2)).Normalize();
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			SetOverlayColor(new Color(1f, 1f, 1f, 0.7f));
		});
	}
}
