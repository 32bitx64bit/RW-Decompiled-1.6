using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_DeathpallAshes : WeatherOverlayDualPanner
{
	private static readonly Material AshesMat = MatLoader.LoadMat("Weather/DeathpallAshesOverlayWorld");

	public WeatherOverlay_DeathpallAshes()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = AshesMat;
		worldOverlayPanSpeed1 = 0.003f;
		worldPanDir1 = new Vector2(-0.12f, -1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldOverlayPanSpeed2 = 0.001f;
		worldPanDir2 = new Vector2(0.12f, -1f);
		((Vector2)(ref worldPanDir2)).Normalize();
		base.ForcedOverlayColor = new Color(0.5f, 0.5f, 0.5f);
	}
}
