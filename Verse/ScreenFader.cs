using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class ScreenFader
{
	private static GUIStyle backgroundStyle;

	private static Texture2D fadeTexture;

	private static Color sourceColor;

	private static Color targetColor;

	private static float sourceTime;

	private static float targetTime;

	private static bool fadeTextureDirty;

	private static float CurTime => Time.realtimeSinceStartup;

	static ScreenFader()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		backgroundStyle = new GUIStyle();
		sourceColor = new Color(0f, 0f, 0f, 0f);
		targetColor = new Color(0f, 0f, 0f, 0f);
		sourceTime = 0f;
		targetTime = 0f;
		fadeTextureDirty = true;
		fadeTexture = new Texture2D(1, 1);
		((Object)fadeTexture).name = "ScreenFader";
		backgroundStyle.normal.background = fadeTexture;
		fadeTextureDirty = true;
	}

	public static void OverlayOnGUI(Vector2 windowSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Color val = CurrentInstantColor();
		if (val.a > 0f)
		{
			if (fadeTextureDirty)
			{
				fadeTexture.SetPixel(0, 0, val);
				fadeTexture.Apply();
			}
			GUI.Label(new Rect(-10f, -10f, windowSize.x + 10f, windowSize.y + 10f), (Texture)(object)fadeTexture, backgroundStyle);
		}
	}

	private static Color CurrentInstantColor()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (CurTime > targetTime || targetTime == sourceTime)
		{
			return targetColor;
		}
		return Color.Lerp(sourceColor, targetColor, (CurTime - sourceTime) / (targetTime - sourceTime));
	}

	public static void SetColor(Color newColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		sourceColor = newColor;
		targetColor = newColor;
		targetTime = 0f;
		sourceTime = 0f;
		fadeTextureDirty = true;
	}

	public static void StartFade(Color finalColor, float duration)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		StartFade(finalColor, duration, 0f);
	}

	public static void StartFade(Color finalColor, float duration, float delay)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (duration <= 0f)
		{
			SetColor(finalColor);
			return;
		}
		sourceColor = CurrentInstantColor();
		targetColor = finalColor;
		sourceTime = CurTime + delay;
		targetTime = sourceTime + duration;
	}

	public static bool IsFading()
	{
		if (Current.ProgramState != ProgramState.Playing || LongEventHandler.ShouldWaitForEvent)
		{
			return false;
		}
		return CurTime < targetTime;
	}
}
