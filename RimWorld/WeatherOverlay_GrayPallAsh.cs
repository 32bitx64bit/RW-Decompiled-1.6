using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_GrayPallAsh : WeatherOverlayDualPanner
{
	private static readonly Material DarkParticlesOverlayWorld = MatLoader.LoadMat("Weather/GraypallAshesOverlayWorld");

	public WeatherOverlay_GrayPallAsh()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = DarkParticlesOverlayWorld;
		worldOverlayPanSpeed1 = 0.003f;
		worldPanDir1 = new Vector2(-0.12f, -1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldOverlayPanSpeed2 = 0.001f;
		worldPanDir2 = new Vector2(0.12f, -1f);
		((Vector2)(ref worldPanDir2)).Normalize();
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			SetOverlayColor(new Color(1f, 1f, 1f, 0.5f));
		});
	}
}
