using System;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GlobalControlsUtility
{
	private const int VisibilityControlsPerRow = 5;

	public static void DoPlaySettings(WidgetRow rowVisibility, bool worldView, ref float curBaseY)
	{
		float y = curBaseY - TimeControls.TimeButSize.y;
		rowVisibility.Init(UI.screenWidth, y, UIDirection.LeftThenUp, 141f);
		Find.PlaySettings.DoPlaySettingsGlobalControls(rowVisibility, worldView);
		curBaseY = rowVisibility.FinalY;
	}

	public static void DoTimespeedControls(float leftX, float width, ref float curBaseY)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		leftX += Mathf.Max(0f, width - 150f);
		width = Mathf.Min(width, 150f);
		float y = TimeControls.TimeButSize.y;
		Rect timerRect = default(Rect);
		((Rect)(ref timerRect))._002Ector(leftX + 16f, curBaseY - y, width, y);
		TimeControls.DoTimeControlsGUI(timerRect);
		curBaseY -= ((Rect)(ref timerRect)).height;
	}

	public static void DoDate(float leftX, float width, ref float curBaseY)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Rect dateRect = default(Rect);
		((Rect)(ref dateRect))._002Ector(leftX, curBaseY - DateReadout.Height, width, DateReadout.Height);
		DateReadout.DateOnGUI(dateRect);
		curBaseY -= ((Rect)(ref dateRect)).height;
	}

	public static void DoRealtimeClock(float leftX, float width, ref float curBaseY)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(leftX - 20f, curBaseY - 26f, width + 20f - 7f, 26f);
		string text = (Prefs.TwelveHourClockMode ? "hh:mm" : "HH:mm");
		string text2 = "";
		if (Prefs.TwelveHourClockMode)
		{
			text2 = string.Format(" {0}", (DateTime.Now.Hour >= 12) ? "PM".Translate() : "AM".Translate());
		}
		using (new TextBlock((TextAnchor)5))
		{
			Widgets.Label(rect, DateTime.Now.ToString(text) + text2);
		}
		curBaseY -= 26f;
	}

	public static void DrawFpsCounter(float leftX, float width, ref float curBaseY)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float averageFrameTime = Root.AverageFrameTime;
		float num = 1000f / averageFrameTime;
		Rect rect = new Rect(leftX, curBaseY - 26f, width - 7f, 26f);
		Text.Anchor = (TextAnchor)5;
		Widgets.Label(rect, $"FPS: {num:F1} ({averageFrameTime:F2}ms/frame)");
		Text.Anchor = (TextAnchor)0;
		curBaseY -= 26f;
	}

	public static void DrawTpsCounter(float leftX, float width, ref float curBaseY)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		float meanTickTime = Find.TickManager.MeanTickTime;
		float num = 1000f / meanTickTime;
		float num2 = 60f * Find.TickManager.TickRateMultiplier;
		num = Mathf.Min(num, num2);
		Rect rect = new Rect(leftX, curBaseY - 26f, width - 7f, 26f);
		Text.Anchor = (TextAnchor)5;
		Widgets.Label(rect, $"TPS: {num:F1} ({meanTickTime:F2}ms/tick)");
		Text.Anchor = (TextAnchor)0;
		curBaseY -= 26f;
	}

	private static void FormatMemoryUsage(StringBuilder sb, long bytes)
	{
		if (bytes > 1073741824)
		{
			sb.Append(((float)bytes / 1.0737418E+09f).ToString("F2"));
			sb.Append(" GiB");
		}
		else if (bytes > 1048576)
		{
			sb.Append(((float)bytes / 1048576f).ToString("F0"));
			sb.Append(" MiB");
		}
		else if (bytes > 1024)
		{
			sb.Append(((float)bytes / 1024f).ToString("F0"));
			sb.Append(" KiB");
		}
		else
		{
			sb.Append(bytes.ToString("F0"));
			sb.Append(" B");
		}
	}

	public static void DrawMemoryInfo(float leftX, float width, ref float curBaseY)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		long trackedMemoryUsageBytes = MemoryUsageUtility.TrackedMemoryUsageBytes;
		long osMemoryUsageBytes = MemoryUsageUtility.OsMemoryUsageBytes;
		long graphicsMemoryUsageBytes = MemoryUsageUtility.GraphicsMemoryUsageBytes;
		long managedMemoryUsageBytes = MemoryUsageUtility.ManagedMemoryUsageBytes;
		long managedMemoryReservedBytes = MemoryUsageUtility.ManagedMemoryReservedBytes;
		long audioMemoryUsageBytes = MemoryUsageUtility.AudioMemoryUsageBytes;
		long textureMemoryUsageBytes = MemoryUsageUtility.TextureMemoryUsageBytes;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(leftX, curBaseY - 104f, width - 7f, 104f);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Memory: ");
		FormatMemoryUsage(stringBuilder, osMemoryUsageBytes);
		stringBuilder.AppendLine();
		stringBuilder.Append("(");
		FormatMemoryUsage(stringBuilder, trackedMemoryUsageBytes);
		stringBuilder.AppendLine(" tracked)");
		stringBuilder.Append("C#: ");
		FormatMemoryUsage(stringBuilder, managedMemoryUsageBytes);
		stringBuilder.Append("/");
		FormatMemoryUsage(stringBuilder, managedMemoryReservedBytes);
		stringBuilder.AppendLine();
		stringBuilder.Append("VRAM: ");
		FormatMemoryUsage(stringBuilder, graphicsMemoryUsageBytes);
		stringBuilder.AppendLine();
		stringBuilder.Append("(");
		FormatMemoryUsage(stringBuilder, textureMemoryUsageBytes);
		stringBuilder.AppendLine(" by textures)");
		stringBuilder.Append("Audio: ");
		FormatMemoryUsage(stringBuilder, audioMemoryUsageBytes);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, new TipSignal("Memory usage may not be accurate in optimized builds.", 5670913));
		}
		Text.Anchor = (TextAnchor)5;
		Widgets.Label(rect, stringBuilder.ToString());
		Text.Anchor = (TextAnchor)0;
		curBaseY -= 104f;
	}
}
