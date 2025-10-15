using System.Collections.Generic;
using RimWorld;
using Steamworks;
using UnityEngine;

namespace Verse.Steam;

public static class SteamDeck
{
	private static bool isSteamDeck;

	private static bool keyboardShowing;

	private static int lastFocusedTextFieldID;

	private static int lastFocusedTextFieldCursorIndex;

	private static bool nextLeftMouseUpEventIsRightMouseUpEvent;

	private static bool unfocusCurrentTextField;

	private static Callback<FloatingGamepadTextInputDismissed_t> keyboardDismissedCallback;

	private static Rect? currentScrollView;

	private static Vector2 currentScrollViewStartMousePos;

	private static bool scrollMouseTraveledEnoughDist;

	private static int consumeAllMouseUpEventsOnFrame = -1;

	private static Rect? scrollViewWithVelocity;

	private static Vector2 scrollViewVelocity;

	private static List<(float, Vector2)> scrollVelocityRecords = new List<(float, Vector2)>();

	public static bool IsSteamDeck
	{
		get
		{
			if (!isSteamDeck)
			{
				return DebugSettings.simulateUsingSteamDeck;
			}
			return true;
		}
	}

	public static bool IsSteamDeckInNonKeyboardMode
	{
		get
		{
			if (IsSteamDeck)
			{
				return !Prefs.SteamDeckKeyboardMode;
			}
			return false;
		}
	}

	public static bool KeyboardShowing => keyboardShowing;

	public static void Init()
	{
		isSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
		if (isSteamDeck)
		{
			SteamInput.Init(false);
			keyboardDismissedCallback = Callback<FloatingGamepadTextInputDismissed_t>.Create((DispatchDelegate<FloatingGamepadTextInputDismissed_t>)KeyboardDismissedCallback);
		}
	}

	public static void ShowOnScreenKeyboard(string initialText, Rect textFieldRect, bool multiline)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (isSteamDeck && !keyboardShowing)
		{
			Rect val = GUIUtility.GUIToScreenRect(textFieldRect);
			if (SteamUtils.ShowFloatingGamepadTextInput((EFloatingGamepadTextInputMode)(multiline ? 1 : 0), (int)((Rect)(ref val)).x, (int)((Rect)(ref val)).y, (int)((Rect)(ref val)).width, (int)((Rect)(ref val)).height))
			{
				keyboardShowing = true;
			}
		}
	}

	public static void HideOnScreenKeyboard()
	{
		if (isSteamDeck && keyboardShowing)
		{
			SteamUtils.DismissFloatingGamepadTextInput();
		}
	}

	public static void ShowConfigPage()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (isSteamDeck)
		{
			SteamInput.ShowBindingPanel(new InputHandle_t(0uL));
		}
	}

	public static void Shutdown()
	{
		if (isSteamDeck)
		{
			SteamInput.Shutdown();
		}
	}

	public static void Update()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!isSteamDeck)
		{
			return;
		}
		if (IsSteamDeckInNonKeyboardMode)
		{
			TextEditor val = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			if (val != null && (val.controlID != lastFocusedTextFieldID || val.cursorIndex != lastFocusedTextFieldCursorIndex) && val.position != default(Rect) && !UnityGUIBugsFixer.IsLeftMouseButtonPressed())
			{
				lastFocusedTextFieldID = val.controlID;
				lastFocusedTextFieldCursorIndex = val.cursorIndex;
				ShowOnScreenKeyboard(val.text, val.position, val.multiline);
			}
		}
		else
		{
			keyboardShowing = false;
		}
	}

	public static void OnGUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!keyboardShowing)
		{
			return;
		}
		TextEditor val = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
		string text = val.text;
		if (!text.NullOrEmpty())
		{
			Text.Font = GameFont.Medium;
			Text.Anchor = (TextAnchor)1;
			float num = Text.CalcHeight(text, 464f) + 36f;
			float num2 = Mathf.Min(Text.CalcSize(text).x, 464f) + 36f;
			Text.Anchor = (TextAnchor)0;
			Text.Font = GameFont.Small;
			Rect rect = new Rect((float)UI.screenWidth / 2f - num2 / 2f, 30f, num2, num);
			Find.WindowStack.ImmediateWindow(84906312, rect, WindowLayer.Super, delegate
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				Text.Font = GameFont.Medium;
				Text.Anchor = (TextAnchor)1;
				Rect rect2 = rect.AtZero().ContractedBy(18f);
				((Rect)(ref rect2)).height = ((Rect)(ref rect2)).height + 10f;
				Widgets.Label(rect2, text);
				Text.Anchor = (TextAnchor)0;
				Text.Font = GameFont.Small;
			});
		}
	}

	public static void Vibrate()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (isSteamDeck)
		{
			SteamInput.TriggerVibration(new InputHandle_t(0uL), (ushort)10000, (ushort)10000);
		}
	}

	public static void RootOnGUI()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		if (!IsSteamDeck)
		{
			return;
		}
		SimulateRightClickIfHoldingMiddleButton();
		if (unfocusCurrentTextField && (int)Event.current.type == 7)
		{
			unfocusCurrentTextField = false;
			UI.UnfocusCurrentTextField();
			TextEditor val = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
			if (val != null)
			{
				lastFocusedTextFieldID = val.controlID;
				lastFocusedTextFieldCursorIndex = val.cursorIndex;
			}
		}
	}

	public static void WindowOnGUI()
	{
		if (IsSteamDeck)
		{
			SimulateRightClickIfHoldingMiddleButton();
		}
	}

	private static void SimulateRightClickIfHoldingMiddleButton()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		if (((int)Event.current.type == 0 || (int)Event.current.type == 1) && Event.current.button == 0 && (Input.GetMouseButton(2) || Input.GetMouseButtonUp(2) || (nextLeftMouseUpEventIsRightMouseUpEvent && (int)Event.current.type == 1)))
		{
			Event.current.button = 1;
			if ((int)Event.current.type == 0)
			{
				nextLeftMouseUpEventIsRightMouseUpEvent = true;
			}
			else if ((int)Event.current.type == 1)
			{
				nextLeftMouseUpEventIsRightMouseUpEvent = false;
			}
		}
	}

	private static void KeyboardDismissedCallback(FloatingGamepadTextInputDismissed_t data)
	{
		keyboardShowing = false;
		unfocusCurrentTextField = true;
	}

	public static void ShowSteamDeckGameControlsIfNotKnown()
	{
		if (IsSteamDeckInNonKeyboardMode && Find.TickManager.TicksGame > 120 && !Find.TickManager.Paused && !PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.SteamDeckControlsGame))
		{
			Dialog_MessageBox dialog_MessageBox = new Dialog_MessageBox(ConceptDefOf.SteamDeckControlsGame.HelpTextAdjusted);
			dialog_MessageBox.image = ContentFinder<Texture2D>.Get("UI/Misc/SteamDeck2");
			Find.WindowStack.Add(dialog_MessageBox);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.SteamDeckControlsGame, KnowledgeAmount.Total);
		}
	}

	public static void ShowSteamDeckMainMenuControlsIfNotKnown()
	{
		if (IsSteamDeckInNonKeyboardMode && !PlayerKnowledgeDatabase.IsComplete(ConceptDefOf.SteamDeckControlsMainMenu))
		{
			Dialog_MessageBox dialog_MessageBox = new Dialog_MessageBox(ConceptDefOf.SteamDeckControlsMainMenu.HelpTextAdjusted);
			dialog_MessageBox.image = ContentFinder<Texture2D>.Get("UI/Misc/SteamDeck1");
			Find.WindowStack.Add(dialog_MessageBox);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.SteamDeckControlsMainMenu, KnowledgeAmount.Total);
		}
	}

	public static void HandleTouchScreenScrollViewScroll(Rect outRect, ref Vector2 scrollPosition)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Invalid comparison between Unknown and I4
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Invalid comparison between Unknown and I4
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSteamDeck || DragAndDropWidget.Dragging || Widgets.Painting || ReorderableWidget.Dragging)
		{
			return;
		}
		Vector2 val4;
		if (Input.GetMouseButtonDown(0) && Mouse.IsOver(outRect) && !Mouse.IsOver(new Rect(((Rect)(ref outRect)).xMax - 16f, ((Rect)(ref outRect)).y, 16f, ((Rect)(ref outRect)).height)) && !Mouse.IsOver(new Rect(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).yMax - 16f, ((Rect)(ref outRect)).width, 16f)))
		{
			currentScrollView = outRect;
			currentScrollViewStartMousePos = UI.MousePositionOnUIInverted;
			scrollMouseTraveledEnoughDist = false;
			scrollVelocityRecords.Clear();
			Rect? val = scrollViewWithVelocity;
			Rect? val2 = currentScrollView;
			if (val.HasValue == val2.HasValue && (!val.HasValue || val.GetValueOrDefault() == val2.GetValueOrDefault()))
			{
				scrollViewWithVelocity = null;
				scrollViewVelocity = default(Vector2);
			}
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				Rect? val2 = currentScrollView;
				Rect val3 = outRect;
				if (val2.HasValue && (!val2.HasValue || val2.GetValueOrDefault() == val3))
				{
					if (!scrollMouseTraveledEnoughDist)
					{
						val4 = UI.MousePositionOnUIInverted - currentScrollViewStartMousePos;
						if (((Vector2)(ref val4)).sqrMagnitude > 100f)
						{
							scrollMouseTraveledEnoughDist = true;
						}
					}
					if (scrollMouseTraveledEnoughDist)
					{
						_ = UnityGUIBugsFixer.CurrentEventDelta;
						scrollPosition -= UnityGUIBugsFixer.CurrentEventDelta;
						scrollVelocityRecords.Add((Time.time, -UnityGUIBugsFixer.CurrentEventDelta));
						ClampScrollPosition(ref scrollPosition);
					}
					goto IL_028e;
				}
			}
			if (!Input.GetMouseButton(0))
			{
				Rect? val2 = currentScrollView;
				Rect val3 = outRect;
				if (val2.HasValue && (!val2.HasValue || val2.GetValueOrDefault() == val3))
				{
					if (scrollMouseTraveledEnoughDist)
					{
						scrollViewWithVelocity = currentScrollView;
						scrollViewVelocity = default(Vector2);
						for (int i = 0; i < scrollVelocityRecords.Count; i++)
						{
							if (Time.time - scrollVelocityRecords[i].Item1 < 0.057f)
							{
								scrollViewVelocity += scrollVelocityRecords[i].Item2;
							}
						}
						consumeAllMouseUpEventsOnFrame = Time.frameCount;
					}
					currentScrollView = null;
				}
			}
		}
		goto IL_028e;
		IL_028e:
		if ((int)Event.current.type == 7)
		{
			Rect? val2 = scrollViewWithVelocity;
			Rect val3 = outRect;
			if (val2.HasValue && (!val2.HasValue || val2.GetValueOrDefault() == val3))
			{
				Vector2 val5 = scrollViewVelocity;
				val4 = default(Vector2);
				if (val5 != val4)
				{
					scrollPosition += scrollViewVelocity;
					Vector2 val6 = scrollViewVelocity;
					val4 = default(Vector2);
					scrollViewVelocity = Vector2.MoveTowards(val6, val4, Time.deltaTime * 90f);
					ClampScrollPosition(ref scrollPosition);
				}
			}
		}
		if (consumeAllMouseUpEventsOnFrame == Time.frameCount && (int)Event.current.type == 1)
		{
			Event.current.Use();
		}
		static void ClampScrollPosition(ref Vector2 pos)
		{
			if (pos.x < 0f)
			{
				pos.x = 0f;
			}
			if (pos.y < 0f)
			{
				pos.y = 0f;
			}
		}
	}

	public static string GetKeyBindingLabel(KeyBindingDef keyDef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected I4, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected I4, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		KeyCode mainKey = keyDef.MainKey;
		if ((int)mainKey <= 100)
		{
			if ((int)mainKey <= 27)
			{
				if ((int)mainKey == 9)
				{
					return "X";
				}
				if ((int)mainKey == 13)
				{
					return "A";
				}
				if ((int)mainKey == 27)
				{
					return "B";
				}
			}
			else
			{
				if ((int)mainKey == 32)
				{
					return "Y";
				}
				if ((int)mainKey == 97)
				{
					return "←";
				}
				if ((int)mainKey == 100)
				{
					return "→";
				}
			}
		}
		else if ((int)mainKey <= 281)
		{
			if ((int)mainKey == 115)
			{
				return "↓";
			}
			if ((int)mainKey == 119)
			{
				return "↑";
			}
			switch (mainKey - 278)
			{
			case 3:
				return "R1";
			case 2:
				return "L1";
			case 0:
				return "R4";
			case 1:
				return "L4";
			}
		}
		else
		{
			if ((int)mainKey == 303)
			{
				return "R5";
			}
			if ((int)mainKey == 304)
			{
				return "R5";
			}
			switch (mainKey - 323)
			{
			case 0:
				return "R2";
			case 1:
				return "L2";
			case 2:
				return "L5";
			}
		}
		if (keyDef == KeyBindingDefOf.MapZoom_In || keyDef == KeyBindingDefOf.MapZoom_Out || keyDef == KeyBindingDefOf.Accept || keyDef == KeyBindingDefOf.Cancel || keyDef == KeyBindingDefOf.TogglePause || keyDef == KeyBindingDefOf.TimeSpeed_Faster || keyDef == KeyBindingDefOf.TimeSpeed_Slower || keyDef == KeyBindingDefOf.QueueOrder)
		{
			return "?";
		}
		return keyDef.MainKeyLabel;
	}
}
