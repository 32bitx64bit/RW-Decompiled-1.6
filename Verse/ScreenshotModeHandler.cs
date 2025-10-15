using RimWorld;
using UnityEngine;

namespace Verse;

public class ScreenshotModeHandler
{
	private bool active;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public bool FiltersCurrentEvent
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Invalid comparison between Unknown and I4
			if (!active)
			{
				return false;
			}
			if ((int)Event.current.type == 7 || (int)Event.current.type == 8)
			{
				return true;
			}
			if ((int)Event.current.type == 0 || (int)Event.current.type == 1 || (int)Event.current.type == 3)
			{
				return true;
			}
			return false;
		}
	}

	public void ScreenshotModesOnGUI()
	{
		if (KeyBindingDefOf.ToggleScreenshotMode.KeyDownEvent && !WorldComponent_GravshipController.CutsceneInProgress)
		{
			active = !active;
			Event.current.Use();
		}
	}
}
