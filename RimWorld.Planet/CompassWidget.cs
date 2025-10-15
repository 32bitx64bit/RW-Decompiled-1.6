using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class CompassWidget
{
	private const float Padding = 10f;

	private const float Size = 64f;

	private static readonly Texture2D CompassTex = ContentFinder<Texture2D>.Get("UI/Misc/Compass");

	private static float Angle
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = GenWorldUI.WorldToUIPosition(Find.WorldGrid.NorthPolePos);
			Vector2 a = new Vector2((float)UI.screenWidth / 2f, (float)UI.screenHeight / 2f);
			val.y = (float)UI.screenHeight - val.y;
			return Vector2Utility.AngleTo(a, val);
		}
	}

	public static void CompassOnGUI(ref float curBaseY)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		CompassOnGUI(new Vector2((float)UI.screenWidth - 10f - 32f, curBaseY - 10f - 32f));
		curBaseY -= 84f;
	}

	private static void CompassOnGUI(Vector2 center)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawTextureRotated(center, (Texture)(object)CompassTex, Angle);
		Rect val = new Rect(center.x - 32f, center.y - 32f, 64f, 64f);
		TooltipHandler.TipRegionByKey(val, "CompassTip");
		if (Widgets.ButtonInvisible(val))
		{
			Find.WorldCameraDriver.RotateSoNorthIsUp();
		}
	}
}
