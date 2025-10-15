using RimWorld;
using UnityEngine;
using Verse;

namespace LudeonTK;

[StaticConstructorOnStartup]
public class Dialog_DevCelestial : Window_Dev
{
	private Vector2 windowPosition;

	private const string Title = "Celestial Debugger";

	public override bool IsDebug => true;

	protected override float Margin => 4f;

	public override Vector2 InitialSize => new Vector2(230f, 275f);

	public Dialog_DevCelestial()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		draggable = true;
		focusWhenOpened = false;
		drawShadow = false;
		closeOnAccept = false;
		closeOnCancel = false;
		preventCameraMotion = false;
		drawInScreenshotMode = false;
		windowPosition = Prefs.DevPalettePosition;
		onlyDrawInDevMode = true;
		doCloseX = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 24f);
		DevGUI.Label(rect, "Celestial Debugger");
		float y = ((Rect)(ref rect)).height + 6f;
		Text.Font = GameFont.Tiny;
		Map currentMap = Find.CurrentMap;
		GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(currentMap, GenCelestial.LightType.Shadow);
		GenCelestial.LightInfo lightSourceInfo2 = GenCelestial.GetLightSourceInfo(currentMap, GenCelestial.LightType.LightingSun);
		Vector2 val = Find.WorldGrid.LongLatOf(currentMap.Tile);
		PrintLabel("Map", inRect, ref y);
		PrintLabel($"Day progress: {GenLocalDate.DayPercent(currentMap) * 100f:0}%", inRect, ref y);
		PrintLabel($"Long, Lat: {val.x:0.0}, {val.y:0.0}", inRect, ref y);
		PrintLabel("", inRect, ref y);
		PrintLabel("Shadow Info", inRect, ref y);
		PrintLabel($"Vector: {lightSourceInfo.vector}", inRect, ref y);
		PrintLabel($"Intensity: {lightSourceInfo.intensity * 100f:0}%", inRect, ref y);
		PrintLabel("", inRect, ref y);
		PrintLabel("Sun Info", inRect, ref y);
		PrintLabel($"Vector: {lightSourceInfo2.vector}", inRect, ref y);
		PrintLabel($"Intensity: {lightSourceInfo2.intensity * 100f:0}%", inRect, ref y);
	}

	private void PrintLabel(string text, Rect container, ref float y)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		DevGUI.Label(new Rect(((Rect)(ref container)).x, y, ((Rect)(ref container)).width, 20f), text);
		y += 20f;
	}
}
