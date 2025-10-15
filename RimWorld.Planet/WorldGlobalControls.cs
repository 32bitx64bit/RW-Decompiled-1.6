using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldGlobalControls
{
	private WidgetRow rowVisibility = new WidgetRow();

	public const float Width = 250f;

	public void WorldGlobalControlsOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 8)
		{
			return;
		}
		float leftX = (float)UI.screenWidth - 250f;
		float curBaseY = (float)UI.screenHeight - 4f;
		if (Current.ProgramState == ProgramState.Playing)
		{
			curBaseY -= 35f;
		}
		GlobalControlsUtility.DoPlaySettings(rowVisibility, worldView: true, ref curBaseY);
		if (Current.ProgramState == ProgramState.Playing)
		{
			curBaseY -= 4f;
			GlobalControlsUtility.DoTimespeedControls(leftX, 250f, ref curBaseY);
			if (Find.CurrentMap != null || Find.WorldSelector.AnyObjectOrTileSelected)
			{
				curBaseY -= 4f;
				GlobalControlsUtility.DoDate(leftX, 250f, ref curBaseY);
			}
			float num = 154f;
			float num2 = Find.World.gameConditionManager.TotalHeightAt(num);
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector((float)UI.screenWidth - num, curBaseY - num2, num, num2);
			Find.World.gameConditionManager.DoConditionsUI(rect);
			curBaseY -= num2;
		}
		if (DebugViewSettings.showMemoryInfo)
		{
			GlobalControlsUtility.DrawMemoryInfo(leftX, 250f, ref curBaseY);
		}
		if (DebugViewSettings.showTpsCounter)
		{
			GlobalControlsUtility.DrawTpsCounter(leftX, 250f, ref curBaseY);
		}
		if (DebugViewSettings.showFpsCounter)
		{
			GlobalControlsUtility.DrawFpsCounter(leftX, 250f, ref curBaseY);
		}
		if (Prefs.ShowRealtimeClock)
		{
			GlobalControlsUtility.DoRealtimeClock(leftX, 250f, ref curBaseY);
		}
		if (!Find.WorldTargeter.IsTargeting)
		{
			Find.WorldRoutePlanner.DoRoutePlannerButton(ref curBaseY);
		}
		if (!Find.PlaySettings.lockNorthUp)
		{
			CompassWidget.CompassOnGUI(ref curBaseY);
		}
		if (Current.ProgramState == ProgramState.Playing)
		{
			curBaseY -= 10f;
			Find.LetterStack.LettersOnGUI(curBaseY);
		}
	}
}
