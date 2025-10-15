using System.Collections.Generic;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public static class UnityGUIBugsFixer
{
	private static List<Resolution> resolutions = new List<Resolution>();

	private static Vector2 currentEventDelta;

	private static int lastMousePositionFrame;

	private static bool leftMouseButtonPressed;

	private const float ScrollFactor = -6f;

	private static Vector2? lastMousePosition;

	public static bool IsSteamDeckOrLinuxBuild
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			if (!SteamDeck.IsSteamDeck && (int)Application.platform != 16)
			{
				return (int)Application.platform == 13;
			}
			return true;
		}
	}

	public static List<Resolution> ScreenResolutionsWithoutDuplicates
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			resolutions.Clear();
			Resolution[] array = Screen.resolutions;
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < resolutions.Count; j++)
				{
					Resolution val = resolutions[j];
					if (((Resolution)(ref val)).width == ((Resolution)(ref array[i])).width)
					{
						val = resolutions[j];
						if (((Resolution)(ref val)).height == ((Resolution)(ref array[i])).height)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					resolutions.Add(array[i]);
				}
			}
			return resolutions;
		}
	}

	public static Vector2 CurrentEventDelta => currentEventDelta;

	public static bool MouseDrag(int button = 0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if (!IsSteamDeckOrLinuxBuild)
		{
			if ((int)Event.current.type == 3)
			{
				return Event.current.button == button;
			}
			return false;
		}
		return Input.GetMouseButton(button);
	}

	public static void OnGUI()
	{
		RememberMouseStateForIsLeftMouseButtonPressed();
		FixSteamDeckMousePositionNeverUpdating();
		FixScrolling();
		FixShift();
		FixDelta();
		EnsureSliderDragReset();
	}

	private static void FixScrolling()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 6 && ((int)Application.platform == 16 || (int)Application.platform == 13))
		{
			Vector2 delta = Event.current.delta;
			Event.current.delta = new Vector2(delta.x, delta.y * -6f);
		}
	}

	private static void FixShift()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		if (((int)Application.platform == 16 || (int)Application.platform == 13) && !Event.current.shift)
		{
			Event.current.shift = Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303);
		}
	}

	public static bool ResolutionsEqual(IntVec2 a, IntVec2 b)
	{
		return a == b;
	}

	private static void FixDelta()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Invalid comparison between Unknown and I4
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
		if (IsSteamDeckOrLinuxBuild)
		{
			if ((int)Event.current.rawType == 0)
			{
				lastMousePosition = mousePositionOnUIInverted;
				lastMousePositionFrame = Time.frameCount;
			}
			else if ((int)Event.current.type == 7)
			{
				if (Time.frameCount != lastMousePositionFrame)
				{
					if (lastMousePosition.HasValue)
					{
						currentEventDelta = mousePositionOnUIInverted - lastMousePosition.Value;
					}
					else
					{
						currentEventDelta = default(Vector2);
					}
					lastMousePosition = mousePositionOnUIInverted;
					lastMousePositionFrame = Time.frameCount;
				}
			}
			else
			{
				currentEventDelta = default(Vector2);
			}
		}
		else if ((int)Event.current.rawType == 3)
		{
			Vector2 val = mousePositionOnUIInverted;
			Vector2? val2 = lastMousePosition;
			if (!val2.HasValue || val != val2.GetValueOrDefault() || Time.frameCount != lastMousePositionFrame)
			{
				if (lastMousePosition.HasValue)
				{
					currentEventDelta = mousePositionOnUIInverted - lastMousePosition.Value;
				}
				else
				{
					currentEventDelta = default(Vector2);
				}
				lastMousePosition = mousePositionOnUIInverted;
				lastMousePositionFrame = Time.frameCount;
			}
		}
		else
		{
			currentEventDelta = Event.current.delta;
			if ((int)Event.current.rawType == 0)
			{
				lastMousePosition = mousePositionOnUIInverted;
				lastMousePositionFrame = Time.frameCount;
			}
			else if ((int)Event.current.rawType == 1)
			{
				lastMousePosition = null;
			}
		}
	}

	private static void EnsureSliderDragReset()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 1)
		{
			Widgets.ResetSliderDraggingIDs();
		}
	}

	public static void Notify_BeginGroup()
	{
		FixSteamDeckMousePositionNeverUpdating();
	}

	public static void Notify_EndGroup()
	{
		FixSteamDeckMousePositionNeverUpdating();
	}

	public static void Notify_BeginScrollView()
	{
		FixSteamDeckMousePositionNeverUpdating();
	}

	public static void Notify_EndScrollView()
	{
		FixSteamDeckMousePositionNeverUpdating();
	}

	public static void Notify_GUIMatrixChanged()
	{
		FixSteamDeckMousePositionNeverUpdating();
	}

	private static void FixSteamDeckMousePositionNeverUpdating()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (IsSteamDeckOrLinuxBuild)
		{
			Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
			mousePositionOnUIInverted = GUIUtility.ScreenToGUIPoint(mousePositionOnUIInverted * Prefs.UIScale);
			Event.current.mousePosition = mousePositionOnUIInverted;
		}
	}

	private static void RememberMouseStateForIsLeftMouseButtonPressed()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 0 && Event.current.button == 0)
		{
			leftMouseButtonPressed = true;
		}
		else if ((int)Event.current.rawType == 1 && Event.current.button == 0)
		{
			leftMouseButtonPressed = false;
		}
	}

	public static bool IsLeftMouseButtonPressed()
	{
		if (Input.GetMouseButton(0))
		{
			return true;
		}
		return leftMouseButtonPressed;
	}
}
