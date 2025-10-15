using System;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public static class Text
{
	private static GameFont fontInt;

	private static TextAnchor anchorInt;

	private static bool wordWrapInt;

	private static Font[] fonts;

	public static readonly GUIStyle[] fontStyles;

	public static readonly GUIStyle[] textFieldStyles;

	public static readonly GUIStyle[] textAreaStyles;

	public static readonly GUIStyle[] textAreaReadOnlyStyles;

	private static readonly float[] lineHeights;

	private static readonly float[] spaceBetweenLines;

	private static GUIContent tmpTextGUIContent;

	private const int NumFonts = 3;

	public const float SmallFontHeight = 22f;

	public static bool TinyFontSupported
	{
		get
		{
			if (!LongEventHandler.AnyEventNowOrWaiting && !LanguageDatabase.activeLanguage.info.canBeTiny)
			{
				return false;
			}
			if (Prefs.DisableTinyText)
			{
				return false;
			}
			if (SteamDeck.IsSteamDeck)
			{
				return false;
			}
			return true;
		}
	}

	public static GameFont Font
	{
		get
		{
			return fontInt;
		}
		set
		{
			if (value == GameFont.Tiny && !TinyFontSupported)
			{
				fontInt = GameFont.Small;
			}
			else
			{
				fontInt = value;
			}
		}
	}

	public static TextAnchor Anchor
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return anchorInt;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			anchorInt = value;
		}
	}

	public static bool WordWrap
	{
		get
		{
			return wordWrapInt;
		}
		set
		{
			wordWrapInt = value;
		}
	}

	public static float LineHeight => lineHeights[(uint)Font];

	public static float SpaceBetweenLines => spaceBetweenLines[(uint)Font];

	public static GUIStyle CurFontStyle
	{
		get
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			GUIStyle val = null;
			val = (GUIStyle)(fontInt switch
			{
				GameFont.Tiny => fontStyles[0], 
				GameFont.Small => fontStyles[1], 
				GameFont.Medium => fontStyles[2], 
				_ => throw new NotImplementedException(), 
			});
			val.alignment = anchorInt;
			val.wordWrap = wordWrapInt;
			return val;
		}
	}

	public static GUIStyle CurTextFieldStyle => (GUIStyle)(fontInt switch
	{
		GameFont.Tiny => textFieldStyles[0], 
		GameFont.Small => textFieldStyles[1], 
		GameFont.Medium => textFieldStyles[2], 
		_ => throw new NotImplementedException(), 
	});

	public static GUIStyle CurTextAreaStyle => (GUIStyle)(fontInt switch
	{
		GameFont.Tiny => textAreaStyles[0], 
		GameFont.Small => textAreaStyles[1], 
		GameFont.Medium => textAreaStyles[2], 
		_ => throw new NotImplementedException(), 
	});

	public static GUIStyle CurTextAreaReadOnlyStyle => (GUIStyle)(fontInt switch
	{
		GameFont.Tiny => textAreaReadOnlyStyles[0], 
		GameFont.Small => textAreaReadOnlyStyles[1], 
		GameFont.Medium => textAreaReadOnlyStyles[2], 
		_ => throw new NotImplementedException(), 
	});

	static Text()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		fontInt = GameFont.Small;
		anchorInt = (TextAnchor)0;
		wordWrapInt = true;
		fonts = (Font[])(object)new Font[3];
		fontStyles = (GUIStyle[])(object)new GUIStyle[3];
		textFieldStyles = (GUIStyle[])(object)new GUIStyle[3];
		textAreaStyles = (GUIStyle[])(object)new GUIStyle[3];
		textAreaReadOnlyStyles = (GUIStyle[])(object)new GUIStyle[3];
		lineHeights = new float[3];
		spaceBetweenLines = new float[3];
		tmpTextGUIContent = new GUIContent();
		fonts[0] = (Font)Resources.Load("Fonts/Calibri_tiny");
		fonts[1] = (Font)Resources.Load("Fonts/Arial_small");
		fonts[2] = (Font)Resources.Load("Fonts/Arial_medium");
		fontStyles[0] = new GUIStyle(GUI.skin.label);
		fontStyles[0].font = fonts[0];
		fontStyles[1] = new GUIStyle(GUI.skin.label);
		fontStyles[1].font = fonts[1];
		fontStyles[1].contentOffset = new Vector2(0f, -1f);
		fontStyles[2] = new GUIStyle(GUI.skin.label);
		fontStyles[2].font = fonts[2];
		for (int i = 0; i < textFieldStyles.Length; i++)
		{
			textFieldStyles[i] = new GUIStyle(GUI.skin.textField);
			textFieldStyles[i].alignment = (TextAnchor)3;
		}
		textFieldStyles[0].font = fonts[0];
		textFieldStyles[1].font = fonts[1];
		textFieldStyles[2].font = fonts[2];
		for (int j = 0; j < textAreaStyles.Length; j++)
		{
			textAreaStyles[j] = new GUIStyle(textFieldStyles[j]);
			textAreaStyles[j].alignment = (TextAnchor)0;
			textAreaStyles[j].wordWrap = true;
		}
		for (int k = 0; k < textAreaReadOnlyStyles.Length; k++)
		{
			textAreaReadOnlyStyles[k] = new GUIStyle(textAreaStyles[k]);
			GUIStyle obj = textAreaReadOnlyStyles[k];
			obj.normal.background = null;
			obj.active.background = null;
			obj.onHover.background = null;
			obj.hover.background = null;
			obj.onFocused.background = null;
			obj.focused.background = null;
		}
		GUI.skin.settings.doubleClickSelectsWord = true;
		int num = 0;
		foreach (GameFont value in Enum.GetValues(typeof(GameFont)))
		{
			Font = value;
			lineHeights[num] = CalcHeight("W", 999f);
			spaceBetweenLines[num] = CalcHeight("W\nW", 999f) - CalcHeight("W", 999f) * 2f;
			num++;
		}
		Font = GameFont.Small;
	}

	public static float LineHeightOf(GameFont font)
	{
		return lineHeights[(uint)font];
	}

	public static float CalcHeight(string text, float width)
	{
		tmpTextGUIContent.text = text.StripTags();
		return CurFontStyle.CalcHeight(tmpTextGUIContent, width);
	}

	public static Vector2 CalcSize(string text)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		tmpTextGUIContent.text = text.StripTags();
		return CurFontStyle.CalcSize(tmpTextGUIContent);
	}

	public static string ClampTextWithEllipsis(Rect rect, string text)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (text.Length <= 4)
		{
			return text;
		}
		Vector2 val = CalcSize(text);
		if (val.x <= ((Rect)(ref rect)).width - 13f)
		{
			return text;
		}
		while (val.x > ((Rect)(ref rect)).width - 13f)
		{
			if (text.Length == 0)
			{
				return "";
			}
			text = text.Substring(0, text.Length - 1);
			val = CalcSize(text + "...");
		}
		return text + "...";
	}

	public static void StartOfOnGUI()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!WordWrap)
		{
			Log.ErrorOnce("Word wrap was false at end of frame.", 764362);
			WordWrap = true;
		}
		if ((int)Anchor != 0)
		{
			TextAnchor anchor = Anchor;
			Log.ErrorOnce("Alignment was " + ((object)(TextAnchor)(ref anchor)).ToString() + " at end of frame.", 15558);
			Anchor = (TextAnchor)0;
		}
		Font = GameFont.Small;
	}
}
