using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace LudeonTK;

[StaticConstructorOnStartup]
public static class DevGUI
{
	private static Stack<bool> mouseOverScrollViewStack;

	public static readonly GUIStyle EmptyStyle;

	public static readonly Color WindowBGFillColor;

	public const float CheckboxSize = 24f;

	private static Texture2D ButtonBackground;

	private static Texture2D ButtonBackgroundMouseover;

	public static Texture2D ButtonBackgroundClick;

	public static readonly Texture2D LightHighlight;

	private static readonly Texture2D PinTex;

	private static readonly Texture2D PinOutlineTex;

	public static readonly Texture2D CheckOn;

	public static readonly Texture2D CheckOff;

	public static readonly Texture2D InspectMode;

	public static readonly Texture2D Close;

	private static Texture2D LineTexAA;

	public const float CloseButtonSize = 22f;

	public const float CloseButtonMargin = 4f;

	static DevGUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		mouseOverScrollViewStack = new Stack<bool>();
		EmptyStyle = new GUIStyle();
		WindowBGFillColor = new ColorInt(21, 25, 29).ToColor;
		ButtonBackground = null;
		ButtonBackgroundMouseover = null;
		ButtonBackgroundClick = null;
		LightHighlight = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.04f));
		PinTex = ContentFinder<Texture2D>.Get("UI/Developer/Pin");
		PinOutlineTex = ContentFinder<Texture2D>.Get("UI/Developer/Pin-Outline");
		CheckOn = ContentFinder<Texture2D>.Get("UI/Developer/CheckOn");
		CheckOff = ContentFinder<Texture2D>.Get("UI/Developer/CheckOff");
		InspectMode = ContentFinder<Texture2D>.Get("UI/Developer/InspectModeToggle");
		Close = ContentFinder<Texture2D>.Get("UI/Developer/Close");
		LineTexAA = null;
		Color val = default(Color);
		((Color)(ref val))._002Ector(1f, 1f, 1f, 0f);
		LineTexAA = new Texture2D(1, 3, (TextureFormat)5, false);
		((Object)LineTexAA).name = "LineTexAA";
		LineTexAA.SetPixel(0, 0, val);
		LineTexAA.SetPixel(0, 1, Color.white);
		LineTexAA.SetPixel(0, 2, val);
		LineTexAA.Apply();
		ButtonBackground = new Texture2D(1, 1, (TextureFormat)5, false);
		((Object)ButtonBackground).name = "ButtonBGAtlas";
		ButtonBackground.SetPixel(0, 0, Color32.op_Implicit(new Color32((byte)65, (byte)65, (byte)65, byte.MaxValue)));
		ButtonBackground.Apply();
		ButtonBackgroundMouseover = new Texture2D(1, 1, (TextureFormat)5, false);
		((Object)ButtonBackgroundMouseover).name = "ButtonBGAtlasMouseover";
		ButtonBackgroundMouseover.SetPixel(0, 0, Color32.op_Implicit(new Color32((byte)85, (byte)85, (byte)85, byte.MaxValue)));
		ButtonBackgroundMouseover.Apply();
		ButtonBackgroundClick = new Texture2D(1, 1, (TextureFormat)5, false);
		((Object)ButtonBackgroundClick).name = "ButtonBGAtlasClick";
		ButtonBackgroundClick.SetPixel(0, 0, Color32.op_Implicit(new Color32((byte)45, (byte)45, (byte)45, byte.MaxValue)));
		ButtonBackgroundClick.Apply();
	}

	public static void BeginGroup(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(rect);
		UnityGUIBugsFixer.Notify_BeginGroup();
	}

	public static void EndGroup()
	{
		GUI.EndGroup();
		UnityGUIBugsFixer.Notify_EndGroup();
	}

	public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		float num = end.x - start.x;
		float num2 = end.y - start.y;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		if (!(num3 < 0.01f))
		{
			width *= 3f;
			float num4 = width * num2 / num3;
			float num5 = width * num / num3;
			float num6 = (0f - Mathf.Atan2(0f - num2, num)) * 57.29578f;
			Vector2 val = start + new Vector2(0.5f * num4, -0.5f * num5);
			Matrix4x4 val2 = Matrix4x4.TRS(Vector2.op_Implicit(val), Quaternion.Euler(0f, 0f, num6), Vector3.one) * Matrix4x4.TRS(Vector2.op_Implicit(-val), Quaternion.identity, Vector3.one);
			Rect val3 = new Rect(start.x, start.y - 0.5f * num5, num3, width);
			GL.PushMatrix();
			GL.MultMatrix(val2);
			GUI.DrawTexture(val3, (Texture)(object)LineTexAA, (ScaleMode)0, true, 0f, color, 0f, 0f);
			GL.PopMatrix();
		}
	}

	public static void DrawBox(Rect rect, int thickness = 1, Texture2D lineTexture = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height);
		if (val.x > val2.x)
		{
			float x = val.x;
			val.x = val2.x;
			val2.x = x;
		}
		if (val.y > val2.y)
		{
			float y = val.y;
			val.y = val2.y;
			val2.y = y;
		}
		Vector3 val3 = Vector2.op_Implicit(val2 - val);
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x, val.y, (float)thickness, val3.y)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val2.x - (float)thickness, val.y, (float)thickness, val3.y)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x + (float)thickness, val.y, val3.x - (float)(thickness * 2), (float)thickness)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x + (float)thickness, val2.y - (float)thickness, val3.x - (float)(thickness * 2), (float)thickness)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
	}

	public static void Label(Rect rect, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		float num = Prefs.UIScale / 2f;
		if (Prefs.UIScale > 1f && Math.Abs(num - Mathf.Floor(num)) > float.Epsilon)
		{
			val = UIScaling.AdjustRectToUIScaling(rect);
		}
		GUI.Label(val, label, Text.CurFontStyle);
	}

	public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)3;
		Label(rect, label);
		if (ButtonInvisible(rect))
		{
			checkOn = !checkOn;
			if (checkOn)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
		}
		Checkbox(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 24f, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - 24f) / 2f, 24f, 24f), ref checkOn);
		Text.Anchor = anchor;
	}

	public static void CheckboxImage(Rect rect, Texture2D icon, ref bool checkOn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (ButtonImage(rect, icon))
		{
			checkOn = !checkOn;
			if (checkOn)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
		}
		float num = ((Rect)(ref rect)).height / 2f;
		Checkbox(new Rect(((Rect)(ref rect)).center.x, ((Rect)(ref rect)).yMin, num, num), ref checkOn);
	}

	public static void Checkbox(Rect rect, ref bool checkOn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ButtonImage(rect, checkOn ? CheckOn : CheckOff))
		{
			checkOn = !checkOn;
			if (checkOn)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
		}
	}

	public static bool ButtonText(Rect rect, string label, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		TextAnchor anchor = Text.Anchor;
		Color color = GUI.color;
		Texture2D val = ButtonBackground;
		if (Mouse.IsOver(rect))
		{
			val = ButtonBackgroundMouseover;
			if (Input.GetMouseButton(0))
			{
				val = ButtonBackgroundClick;
			}
		}
		GUI.DrawTexture(rect, (Texture)(object)val);
		if (overrideTextAnchor.HasValue)
		{
			Text.Anchor = overrideTextAnchor.Value;
		}
		else
		{
			Text.Anchor = (TextAnchor)4;
		}
		bool wordWrap = Text.WordWrap;
		if (((Rect)(ref rect)).height < Text.LineHeight * 2f)
		{
			Text.WordWrap = false;
		}
		GUI.color = Color.white;
		Label(rect, label);
		Text.Anchor = anchor;
		GUI.color = color;
		Text.WordWrap = wordWrap;
		return ButtonInvisible(rect);
	}

	public static void DrawRect(Rect position, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box(position, GUIContent.none, TexUI.FastFillStyle);
		GUI.backgroundColor = backgroundColor;
	}

	public static bool ButtonImage(Rect butRect, Texture2D tex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(butRect) && !ReorderableWidget.Dragging)
		{
			GUI.color = GenUI.MouseoverColor;
		}
		else
		{
			GUI.color = Color.white;
		}
		GUI.DrawTexture(butRect, (Texture)(object)tex);
		GUI.color = Color.white;
		return ButtonInvisible(butRect);
	}

	public static bool ButtonInvisible(Rect butRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(butRect);
		return GUI.Button(butRect, "", EmptyStyle);
	}

	public static string TextField(Rect rect, string text)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (text == null)
		{
			text = "";
		}
		return GUI.TextField(rect, text, Text.CurTextFieldStyle);
	}

	public static void BeginScrollView(Rect outRect, ref Vector2 scrollPosition, Rect viewRect, bool showScrollbars = true)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (mouseOverScrollViewStack.Count > 0)
		{
			mouseOverScrollViewStack.Push(mouseOverScrollViewStack.Peek() && ((Rect)(ref outRect)).Contains(Event.current.mousePosition));
		}
		else
		{
			mouseOverScrollViewStack.Push(((Rect)(ref outRect)).Contains(Event.current.mousePosition));
		}
		SteamDeck.HandleTouchScreenScrollViewScroll(outRect, ref scrollPosition);
		if (showScrollbars)
		{
			scrollPosition = GUI.BeginScrollView(outRect, scrollPosition, viewRect);
		}
		else
		{
			scrollPosition = GUI.BeginScrollView(outRect, scrollPosition, viewRect, GUIStyle.none, GUIStyle.none);
		}
		UnityGUIBugsFixer.Notify_BeginScrollView();
	}

	public static void EndScrollView()
	{
		mouseOverScrollViewStack.Pop();
		GUI.EndScrollView();
	}

	public static void DrawHighlightSelected(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)TexUI.HighlightSelectedTex);
	}

	public static void DrawHighlightIfMouseover(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			DrawHighlight(rect);
		}
	}

	public static void DrawHighlight(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)TexUI.HighlightTex);
	}

	public static void DrawLightHighlight(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)LightHighlight);
	}

	public static DebugActionButtonResult CheckboxPinnable(Rect rect, string label, ref bool checkOn, bool highlight, bool pinned)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
		DebugActionButtonResult result = DebugActionButtonResult.None;
		CheckboxLabeled(rect, label.Truncate(((Rect)(ref rect)).width - 15f), ref checkOn);
		if (highlight)
		{
			GUI.color = Color.yellow;
			DrawBox(rect, 2);
			GUI.color = Color.white;
		}
		Rect val = GenUI.ContractedBy(new Rect(((Rect)(ref rect)).xMax + 2f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height), 4f);
		GUI.color = (Color)(pinned ? Color.white : new Color(1f, 1f, 1f, 0.2f));
		GUI.DrawTexture(val, (Texture)(object)(pinned ? PinTex : PinOutlineTex));
		GUI.color = Color.white;
		if (ButtonInvisible(val))
		{
			result = DebugActionButtonResult.PinPressed;
		}
		DrawHighlightIfMouseover(val);
		return result;
	}

	public static DebugActionButtonResult ButtonDebugPinnable(Rect rect, string label, bool highlight, bool pinned)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		DebugActionButtonResult result = DebugActionButtonResult.None;
		bool wordWrap = Text.WordWrap;
		Text.WordWrap = false;
		if (ButtonText(rect, "  " + label, (TextAnchor)3))
		{
			result = DebugActionButtonResult.ButtonPressed;
		}
		Text.WordWrap = wordWrap;
		if (highlight)
		{
			GUI.color = Color.yellow;
			DrawBox(rect, 2);
			GUI.color = Color.white;
		}
		Rect val = GenUI.ContractedBy(new Rect(((Rect)(ref rect)).xMax + 2f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height), 4f);
		GUI.color = (Color)(pinned ? Color.white : new Color(1f, 1f, 1f, 0.2f));
		GUI.DrawTexture(val, (Texture)(object)(pinned ? PinTex : PinOutlineTex));
		GUI.color = Color.white;
		if (ButtonInvisible(val))
		{
			result = DebugActionButtonResult.PinPressed;
		}
		DrawHighlightIfMouseover(val);
		return result;
	}

	public static string TextAreaScrollable(Rect rect, string text, ref Vector2 scrollbarPosition, bool readOnly = false)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, Mathf.Max(Text.CalcHeight(text, ((Rect)(ref rect)).width) + 10f, ((Rect)(ref rect)).height));
		BeginScrollView(rect, ref scrollbarPosition, val);
		string result = GUI.TextArea(val, text, readOnly ? Text.CurTextAreaReadOnlyStyle : Text.CurTextAreaStyle);
		EndScrollView();
		return result;
	}

	public static float HorizontalSlider(Rect rect, float value, float leftValue, float rightValue)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		float num = GUI.HorizontalSlider(rect, value, leftValue, rightValue);
		if (value != num)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
		}
		return num;
	}
}
