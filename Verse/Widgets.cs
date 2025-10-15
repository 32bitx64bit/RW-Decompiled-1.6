using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.Sound;
using Verse.Steam;

namespace Verse;

[StaticConstructorOnStartup]
public static class Widgets
{
	public enum DraggableResult
	{
		Idle,
		Pressed,
		Dragged,
		DraggedThenPressed
	}

	private enum RangeEnd : byte
	{
		None,
		Min,
		Max
	}

	[Flags]
	public enum ColorComponents
	{
		Red = 1,
		Green = 2,
		Blue = 4,
		Hue = 8,
		Sat = 0x10,
		Value = 0x20,
		None = 0,
		All = 0x3F
	}

	public struct DropdownMenuElement<Payload>
	{
		public FloatMenuOption option;

		public Payload payload;
	}

	public static Stack<bool> mouseOverScrollViewStack;

	public static readonly GUIStyle EmptyStyle;

	[TweakValue("Input", 0f, 100f)]
	private static float DragStartDistanceSquared;

	public const int LeftMouseButton = 0;

	public static readonly Color InactiveColor;

	public static readonly Color HighlightStrongBgColor;

	public static readonly Color HighlightTextBgColor;

	private static readonly Texture2D DefaultBarBgTex;

	public static readonly Texture2D BarFullTexHor;

	public static readonly Texture2D CheckboxOnTex;

	public static readonly Texture2D CheckboxOffTex;

	public static readonly Texture2D CheckboxPartialTex;

	public const float CheckboxSize = 24f;

	public const float RadioButtonSize = 24f;

	public static readonly Texture2D RadioButOnTex;

	public static readonly Texture2D HSVColorWheelTex;

	public static readonly Texture2D ColorSelectionCircle;

	public static readonly Texture2D ColorTemperatureExp;

	public static readonly Texture2D SelectionArrow;

	private static readonly Texture2D RadioButOffTex;

	private static readonly Texture2D FillArrowTexRight;

	private static readonly Texture2D FillArrowTexLeft;

	public static readonly Texture2D PlaceholderIconTex;

	private const int FillableBarBorderWidth = 3;

	private const int MaxFillChangeArrowHeight = 16;

	private const int FillChangeArrowWidth = 8;

	public const float CloseButtonSize = 18f;

	public const float CloseButtonMargin = 4f;

	public const float BackButtonWidth = 120f;

	public const float BackButtonHeight = 40f;

	public const float BackButtonMargin = 16f;

	private const float ColorHighlightCircleFraction = 0.125f;

	private const float ColorTextfieldHeight = 30f;

	private const float SelectionArrowSize = 12f;

	private static readonly Texture2D ShadowAtlas;

	public static readonly Texture2D ButtonBGAtlas;

	private static readonly Texture2D ButtonBGAtlasMouseover;

	public static readonly Texture2D ButtonBGAtlasClick;

	private static readonly Texture2D FloatRangeSliderTex;

	public static readonly Texture2D LightHighlight;

	private static readonly Rect DefaultTexCoords;

	private static readonly Rect LinkedTexCoords;

	[TweakValue("Input", 0f, 100f)]
	private static int IntEntryButtonWidth;

	private static Texture2D LineTexAA;

	private static readonly Texture2D AltTexture;

	public static readonly Color NormalOptionColor;

	public static readonly Color MouseoverOptionColor;

	private static Dictionary<string, float> LabelCache;

	private const float TileSize = 64f;

	public static readonly Color SeparatorLabelColor;

	public static readonly Color SeparatorLineColor;

	private const float SeparatorLabelHeight = 20f;

	public const float ListSeparatorHeight = 25f;

	private static bool checkboxPainting;

	private static bool checkboxPaintingState;

	public static readonly Texture2D ButtonSubtleAtlas;

	private static readonly Texture2D SliderRailAtlas;

	private static readonly Texture2D SliderHandle;

	private static readonly Texture2D ButtonBarTex;

	public const float ButtonSubtleDefaultMarginPct = 0.15f;

	private static int buttonInvisibleDraggable_activeControl;

	private static bool buttonInvisibleDraggable_dragged;

	private static Vector3 buttonInvisibleDraggable_mouseStart;

	private static int sliderDraggingID;

	private const float SliderHandleSize = 12f;

	public const float RangeControlIdealHeight = 31f;

	public const float RangeControlCompactHeight = 32f;

	private const float RangeSliderSize = 16f;

	private static readonly Color RangeControlTextColor;

	private static int draggingId;

	private static RangeEnd curDragEnd;

	private static float lastDragSliderSoundTime;

	private static float FillableBarChangeRateDisplayRatio;

	public static int MaxFillableBarChangeRate;

	private static readonly Color WindowBGBorderColor;

	public static readonly Color WindowBGFillColor;

	public static readonly Color MenuSectionBGFillColor;

	private static readonly Color MenuSectionBGBorderColor;

	private static readonly Color TutorWindowBGFillColor;

	private static readonly Color TutorWindowBGBorderColor;

	private static readonly Color OptionUnselectedBGFillColor;

	private static readonly Color OptionUnselectedBGBorderColor;

	private static readonly Color OptionSelectedBGFillColor;

	private static readonly Color OptionSelectedBGBorderColor;

	private static readonly Rect AtlasUV_TopLeft;

	private static readonly Rect AtlasUV_TopRight;

	private static readonly Rect AtlasUV_BottomLeft;

	private static readonly Rect AtlasUV_BottomRight;

	private static readonly Rect AtlasUV_Top;

	private static readonly Rect AtlasUV_Bottom;

	private static readonly Rect AtlasUV_Left;

	private static readonly Rect AtlasUV_Right;

	private static readonly Rect AtlasUV_Center;

	private static int[] maxColorComponentValues;

	private static string[] colorComponentLabels;

	private static string[] tmpTranslatedColorComponentLabels;

	private static int[] intColorComponents;

	public const float InfoCardButtonSize = 24f;

	private static bool dropdownPainting;

	private static object dropdownPainting_Payload;

	private static Type dropdownPainting_Type;

	private static string dropdownPainting_Text;

	private static Texture2D dropdownPainting_Icon;

	public static bool Painting
	{
		get
		{
			if (!dropdownPainting)
			{
				return checkboxPainting;
			}
			return true;
		}
	}

	static Widgets()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Expected O, but got Unknown
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		mouseOverScrollViewStack = new Stack<bool>();
		EmptyStyle = new GUIStyle();
		DragStartDistanceSquared = 20f;
		InactiveColor = new Color(0.37f, 0.37f, 0.37f, 0.8f);
		HighlightStrongBgColor = ColorLibrary.SkyBlue;
		HighlightTextBgColor = HighlightStrongBgColor.ToTransparent(0.25f);
		DefaultBarBgTex = BaseContent.BlackTex;
		BarFullTexHor = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.8f, 0.85f));
		CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckOn");
		CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckOff");
		CheckboxPartialTex = ContentFinder<Texture2D>.Get("UI/Widgets/CheckPartial");
		RadioButOnTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
		HSVColorWheelTex = ContentFinder<Texture2D>.Get("UI/Widgets/HSVColorWheel");
		ColorSelectionCircle = ContentFinder<Texture2D>.Get("UI/Overlays/TargetHighlight_Square");
		ColorTemperatureExp = ContentFinder<Texture2D>.Get("UI/Widgets/ColorTemperatureExp");
		SelectionArrow = ContentFinder<Texture2D>.Get("Things/Mote/InteractionArrow");
		RadioButOffTex = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
		FillArrowTexRight = ContentFinder<Texture2D>.Get("UI/Widgets/FillChangeArrowRight");
		FillArrowTexLeft = ContentFinder<Texture2D>.Get("UI/Widgets/FillChangeArrowLeft");
		PlaceholderIconTex = ContentFinder<Texture2D>.Get("UI/Icons/MenuOptionNoIcon");
		ShadowAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/DropShadow");
		ButtonBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG");
		ButtonBGAtlasMouseover = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover");
		ButtonBGAtlasClick = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick");
		FloatRangeSliderTex = ContentFinder<Texture2D>.Get("UI/Widgets/RangeSlider");
		LightHighlight = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.04f));
		DefaultTexCoords = new Rect(0f, 0f, 1f, 1f);
		LinkedTexCoords = new Rect(0f, 0.5f, 0.25f, 0.25f);
		IntEntryButtonWidth = 40;
		LineTexAA = null;
		AltTexture = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.05f));
		NormalOptionColor = new Color(0.8f, 0.85f, 1f);
		MouseoverOptionColor = Color.yellow;
		LabelCache = new Dictionary<string, float>();
		SeparatorLabelColor = new Color(0.8f, 0.8f, 0.8f, 1f);
		SeparatorLineColor = new Color(0.3f, 0.3f, 0.3f, 1f);
		checkboxPainting = false;
		checkboxPaintingState = false;
		ButtonSubtleAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonSubtleAtlas");
		SliderRailAtlas = ContentFinder<Texture2D>.Get("UI/Buttons/SliderRail");
		SliderHandle = ContentFinder<Texture2D>.Get("UI/Buttons/SliderHandle");
		ButtonBarTex = SolidColorMaterials.NewSolidColorTexture(TexUI.FinishedResearchColorTransparent);
		buttonInvisibleDraggable_activeControl = 0;
		buttonInvisibleDraggable_dragged = false;
		buttonInvisibleDraggable_mouseStart = Vector3.zero;
		RangeControlTextColor = new Color(0.6f, 0.6f, 0.6f);
		draggingId = 0;
		curDragEnd = RangeEnd.None;
		lastDragSliderSoundTime = -1f;
		FillableBarChangeRateDisplayRatio = 100000000f;
		MaxFillableBarChangeRate = 3;
		WindowBGBorderColor = new ColorInt(97, 108, 122).ToColor;
		WindowBGFillColor = new ColorInt(21, 25, 29).ToColor;
		MenuSectionBGFillColor = new ColorInt(42, 43, 44).ToColor;
		MenuSectionBGBorderColor = new ColorInt(135, 135, 135).ToColor;
		TutorWindowBGFillColor = new ColorInt(133, 85, 44).ToColor;
		TutorWindowBGBorderColor = new ColorInt(176, 139, 61).ToColor;
		OptionUnselectedBGFillColor = new Color(0.21f, 0.21f, 0.21f);
		OptionUnselectedBGBorderColor = OptionUnselectedBGFillColor * 1.8f;
		OptionSelectedBGFillColor = new Color(0.32f, 0.28f, 0.21f);
		OptionSelectedBGBorderColor = OptionSelectedBGFillColor * 1.8f;
		AtlasUV_TopLeft = new Rect(0f, 0f, 0.25f, 0.25f);
		AtlasUV_TopRight = new Rect(0.75f, 0f, 0.25f, 0.25f);
		AtlasUV_BottomLeft = new Rect(0f, 0.75f, 0.25f, 0.25f);
		AtlasUV_BottomRight = new Rect(0.75f, 0.75f, 0.25f, 0.25f);
		AtlasUV_Top = new Rect(0.25f, 0f, 0.5f, 0.25f);
		AtlasUV_Bottom = new Rect(0.25f, 0.75f, 0.5f, 0.25f);
		AtlasUV_Left = new Rect(0f, 0.25f, 0.25f, 0.5f);
		AtlasUV_Right = new Rect(0.75f, 0.25f, 0.25f, 0.5f);
		AtlasUV_Center = new Rect(0.25f, 0.25f, 0.5f, 0.5f);
		maxColorComponentValues = new int[6] { 255, 255, 255, 360, 100, 100 };
		colorComponentLabels = new string[6] { "Red", "Green", "Blue", "Hue", "Saturation", "ColorValue" };
		tmpTranslatedColorComponentLabels = new string[6];
		intColorComponents = new int[6];
		dropdownPainting = false;
		dropdownPainting_Payload = null;
		dropdownPainting_Type = null;
		dropdownPainting_Text = "";
		dropdownPainting_Icon = null;
		Color val = default(Color);
		((Color)(ref val))._002Ector(1f, 1f, 1f, 0f);
		LineTexAA = new Texture2D(1, 3, (TextureFormat)5, false);
		((Object)LineTexAA).name = "LineTexAA";
		LineTexAA.SetPixel(0, 0, val);
		LineTexAA.SetPixel(0, 1, Color.white);
		LineTexAA.SetPixel(0, 2, val);
		LineTexAA.Apply();
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

	public static void ClearLabelCache()
	{
		LabelCache.Clear();
	}

	public static bool CanDrawIconFor(Def def)
	{
		if (def is BuildableDef buildableDef)
		{
			return (Object)(object)buildableDef.uiIcon != (Object)null;
		}
		if (def is FactionDef factionDef)
		{
			return (Object)(object)factionDef.FactionIcon != (Object)null;
		}
		return false;
	}

	public static void DefIcon(Rect rect, Def def, ThingDef stuffDef = null, float scale = 1f, ThingStyleDef thingStyleDef = null, bool drawPlaceholder = false, Color? color = null, Material material = null, int? graphicIndexOverride = null, float alpha = 1f)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		if (def is BuildableDef buildableDef)
		{
			((Rect)(ref rect)).position = ((Rect)(ref rect)).position + new Vector2(buildableDef.uiIconOffset.x * ((Rect)(ref rect)).size.x, buildableDef.uiIconOffset.y * ((Rect)(ref rect)).size.y);
		}
		if (def is ThingDef { IsFrame: not false, entityDefToBuild: not null } thingDef)
		{
			def = thingDef.entityDefToBuild;
		}
		if (def is ThingDef thingDef2)
		{
			ThingIcon(rect, thingDef2, stuffDef, thingStyleDef, scale, color, graphicIndexOverride, alpha);
		}
		else if (def is PawnKindDef pawnKindDef)
		{
			ThingIcon(rect, pawnKindDef.race, stuffDef, thingStyleDef, scale, color, graphicIndexOverride, alpha);
		}
		else if (def is RecipeDef recipeDef && (recipeDef.UIIconThing != null || (Object)(object)recipeDef.UIIcon != (Object)null))
		{
			if (recipeDef.UIIconThing != null)
			{
				ThingIcon(rect, recipeDef.UIIconThing, null, thingStyleDef, scale, color, graphicIndexOverride, alpha);
			}
			else if ((Object)(object)recipeDef.UIIcon != (Object)null)
			{
				DrawTextureFitted(rect, (Texture)(object)recipeDef.UIIcon, scale, material, alpha);
			}
		}
		else if (def is TerrainDef terrainDef && (Object)(object)terrainDef.uiIcon != (Object)null)
		{
			GUI.color = terrainDef.uiIconColor;
			Rect texCoords = (Rect)(terrainDef.cropIcon ? CroppedTerrainTextureRect(terrainDef.uiIcon) : new Rect(0f, 0f, 1f, 1f));
			DrawTextureFitted(rect, (Texture)(object)terrainDef.uiIcon, scale, Vector2.one, texCoords, 0f, material, alpha);
			GUI.color = Color.white;
		}
		else if (def is FactionDef factionDef)
		{
			if (!factionDef.colorSpectrum.NullOrEmpty())
			{
				GUI.color = factionDef.colorSpectrum.FirstOrDefault();
			}
			DrawTextureFitted(rect, (Texture)(object)factionDef.FactionIcon, scale, material, alpha);
			GUI.color = Color.white;
		}
		else if (def is StyleItemDef styleItemDef)
		{
			DrawTextureFitted(rect, (Texture)(object)styleItemDef.Icon, scale, material, alpha);
		}
		else if (def is BodyTypeDef bodyTypeDef)
		{
			DrawTextureFitted(rect, (Texture)(object)bodyTypeDef.Icon, scale, material, alpha);
		}
		else if (def is HeadTypeDef headTypeDef)
		{
			DrawTextureFitted(rect, (Texture)(object)headTypeDef.Icon, scale, material, alpha);
		}
		else if (def is GeneDef geneDef)
		{
			GUI.color = (Color)(((_003F?)color) ?? geneDef.IconColor);
			DrawTextureFitted(rect, (Texture)(object)geneDef.Icon, scale, material, alpha);
			GUI.color = Color.white;
		}
		else if (def is XenotypeDef xenotypeDef)
		{
			GUI.color = (Color)(((_003F?)color) ?? XenotypeDef.IconColor);
			DrawTextureFitted(rect, (Texture)(object)xenotypeDef.Icon, scale, material, alpha);
			GUI.color = Color.white;
		}
		else if (def is PsychicRitualDef psychicRitualDef)
		{
			DrawTextureFitted(rect, (Texture)(object)psychicRitualDef.uiIcon, scale, material, alpha);
		}
		else if (drawPlaceholder)
		{
			DrawTextureFitted(rect, (Texture)(object)PlaceholderIconTex, scale, material, alpha);
		}
	}

	public static void ThingIcon(Rect rect, Thing thing, float alpha = 1f, Rot4? rot = null, bool stackOfOne = false, float scale = 1f, bool grayscale = false)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		thing = thing.GetInnerIfMinified();
		if (thing is Blueprint blueprint && blueprint.EntityToBuild() != null)
		{
			DefIcon(rect, blueprint.EntityToBuild(), blueprint.EntityToBuildStuff(), 1f, blueprint.EntityToBuildStyle(), drawPlaceholder: false, null, null, null, alpha);
			return;
		}
		float scale2;
		float angle;
		Vector2 iconProportions;
		Color color;
		Material material;
		Texture iconFor = GetIconFor(thing, new Vector2(((Rect)(ref rect)).width, ((Rect)(ref rect)).height), rot, stackOfOne, out scale2, out angle, out iconProportions, out color, out material);
		if (thing is Frame { BuildDef: not null } frame)
		{
			iconFor = (Texture)(object)GetIconFor(frame.BuildDef, frame.Stuff, frame.StyleDef);
		}
		if ((Object)(object)iconFor == (Object)null || (Object)(object)iconFor == (Object)(object)BaseContent.BadTex)
		{
			return;
		}
		GUI.color = color;
		ThingStyleDef styleDef = thing.StyleDef;
		if ((styleDef != null && (Object)(object)styleDef.UIIcon != (Object)null) || !thing.def.uiIconPath.NullOrEmpty())
		{
			((Rect)(ref rect)).position = ((Rect)(ref rect)).position + new Vector2(thing.def.uiIconOffset.x * ((Rect)(ref rect)).size.x, thing.def.uiIconOffset.y * ((Rect)(ref rect)).size.y);
		}
		Material mat = material;
		if (grayscale)
		{
			MaterialRequest materialRequest = default(MaterialRequest);
			materialRequest.shader = ShaderDatabase.GrayscaleGUI;
			materialRequest.color = color;
			MaterialRequest req = materialRequest;
			if ((Object)(object)material != (Object)null)
			{
				req.maskTex = (Texture2D)material.GetTexture(ShaderPropertyIDs.MaskTex);
				req.color = material.GetColor(ShaderPropertyIDs.Color);
				req.colorTwo = material.GetColor(ShaderPropertyIDs.ColorTwo);
			}
			else
			{
				req.maskTex = Texture2D.redTexture;
			}
			mat = MaterialPool.MatFrom(req);
		}
		ThingIconWorker(rect, thing.def, iconFor, angle, scale2 * scale, rot, mat, alpha);
		GUI.color = Color.white;
	}

	public static void ThingIcon(Rect rect, ThingDef thingDef, ThingDef stuffDef = null, ThingStyleDef thingStyleDef = null, float scale = 1f, Color? color = null, int? graphicIndexOverride = null, float alpha = 1f)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)thingDef.uiIcon == (Object)null || (Object)(object)thingDef.uiIcon == (Object)(object)BaseContent.BadTex)
		{
			return;
		}
		Material material;
		Texture2D iconFor = GetIconFor(thingDef, out material, stuffDef, thingStyleDef, graphicIndexOverride);
		if (!((Object)(object)iconFor == (Object)null))
		{
			Color color2 = GUI.color;
			if (color.HasValue)
			{
				GUI.color = color.Value;
			}
			else if (stuffDef != null)
			{
				GUI.color = thingDef.GetColorForStuff(stuffDef);
			}
			else if ((Object)(object)material != (Object)null)
			{
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = (thingDef.MadeFromStuff ? thingDef.GetColorForStuff(GenStuff.DefaultStuffFor(thingDef)) : thingDef.uiIconColor);
			}
			scale = ((thingStyleDef == null) ? (scale * GenUI.IconDrawScale(thingDef)) : (scale * thingStyleDef.uiIconScale));
			float num = (float)((Texture)iconFor).width / (float)((Texture)iconFor).height;
			rect = ((num < 1f) ? rect.MiddlePart(num, 1f) : rect.MiddlePart(1f, num));
			Rect rect2 = rect;
			float uiIconAngle = thingDef.uiIconAngle;
			float scale2 = scale;
			Material mat = material;
			ThingIconWorker(rect2, thingDef, (Texture)(object)iconFor, uiIconAngle, scale2, null, mat, alpha);
			GUI.color = color2;
		}
	}

	public static Texture2D GetIconFor(ThingDef thingDef, ThingDef stuffDef = null, ThingStyleDef thingStyleDef = null, int? graphicIndexOverride = null)
	{
		Material material;
		return GetIconFor(thingDef, out material, stuffDef, thingStyleDef, graphicIndexOverride);
	}

	public static Texture2D GetIconFor(ThingDef thingDef, out Material material, ThingDef stuffDef = null, ThingStyleDef thingStyleDef = null, int? graphicIndexOverride = null)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		if (thingDef.IsCorpse && thingDef.ingestible?.sourceDef != null)
		{
			thingDef = thingDef.ingestible.sourceDef;
		}
		material = null;
		Texture2D result = thingDef.GetUIIconForStuff(stuffDef);
		if (thingStyleDef != null && (Object)(object)thingStyleDef.UIIcon != (Object)null)
		{
			result = ((!graphicIndexOverride.HasValue) ? thingStyleDef.UIIcon : thingStyleDef.IconForIndex(graphicIndexOverride.Value));
		}
		else if (thingDef.graphic is Graphic_Appearances graphic_Appearances)
		{
			result = (Texture2D)graphic_Appearances.SubGraphicFor(stuffDef ?? GenStuff.DefaultStuffFor(thingDef)).MatAt(thingDef.defaultPlacingRot).mainTexture;
		}
		else if ((Object)(object)thingDef.uiIconMaterial != (Object)null)
		{
			material = thingDef.uiIconMaterial;
		}
		return result;
	}

	private static Color GetDrawColor(BuildableDef buildable, ThingDef stuff)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (buildable is ThingDef thingDef)
		{
			if (stuff != null)
			{
				return thingDef.GetColorForStuff(stuff);
			}
			if (thingDef.graphicData != null)
			{
				return thingDef.graphicData.color;
			}
		}
		else if (buildable is TerrainDef terrainDef)
		{
			return terrainDef.DrawColor;
		}
		return Color.white;
	}

	public static Texture GetIconFor(Thing thing, Vector2 size, Rot4? rot, bool stackOfOne, out float scale, out float angle, out Vector2 iconProportions, out Color color, out Material material)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		if (thing == null)
		{
			scale = 1f;
			angle = 0f;
			iconProportions = Vector2.one;
			color = Color.white;
			material = null;
			return null;
		}
		material = null;
		thing = thing.GetInnerIfMinified();
		if (thing is Corpse corpse)
		{
			thing = corpse.InnerPawn;
		}
		Texture result = null;
		ThingStyleDef styleDef = thing.StyleDef;
		iconProportions = thing.DrawSize;
		color = thing.DrawColor;
		scale = GenUI.IconDrawScale(thing.def);
		if (thing is Blueprint blueprint)
		{
			color = GetDrawColor(blueprint.EntityToBuild(), blueprint.EntityToBuildStuff());
		}
		if (rot.HasValue && rot.Value.IsHorizontal)
		{
			iconProportions = new Vector2(iconProportions.y, iconProportions.x);
		}
		angle = 0f;
		if ((Object)(object)thing.UIIconOverride != (Object)null)
		{
			result = thing.UIIconOverride;
			angle = thing.def.uiIconAngle;
		}
		else if (styleDef != null && (Object)(object)styleDef.UIIcon != (Object)null)
		{
			Rot4 valueOrDefault = rot.GetValueOrDefault();
			if (!rot.HasValue)
			{
				valueOrDefault = thing.def.defaultPlacingRot;
				rot = valueOrDefault;
			}
			result = (Texture)(object)styleDef.IconForIndex(thing.OverrideGraphicIndex ?? thing.thingIDNumber, rot);
			angle = thing.def.uiIconAngle;
		}
		else if (!thing.def.uiIconPath.NullOrEmpty())
		{
			result = (Texture)(object)thing.def.uiIcon;
			angle = thing.def.uiIconAngle;
		}
		else if (thing is Pawn pawn)
		{
			if (!pawn.RaceProps.Humanlike)
			{
				Rot4 valueOrDefault = rot.GetValueOrDefault();
				if (!rot.HasValue)
				{
					valueOrDefault = Rot4.East;
					rot = valueOrDefault;
				}
				pawn.Drawer?.renderer?.EnsureGraphicsInitialized();
				Material val = pawn.Drawer?.renderer?.BodyGraphic?.MatAt(rot.Value);
				if ((Object)(object)val != (Object)null)
				{
					result = val.mainTexture;
					if (ShaderDatabase.TryGetUIShader(val.shader, out var uiShader) && MaterialPool.TryGetRequestForMat(val, out var request))
					{
						request.shader = uiShader;
						material = MaterialPool.MatFrom(request);
						color = Color.white;
					}
					else
					{
						color = val.color;
					}
				}
			}
			else
			{
				Rot4 valueOrDefault = rot.GetValueOrDefault();
				if (!rot.HasValue)
				{
					valueOrDefault = Rot4.South;
					rot = valueOrDefault;
				}
				Rect r = GenUI.ScaledBy(new Rect(0f, 0f, size.x, size.y), 1.8f);
				r = r.Rounded();
				float num = 1.8f;
				if (ChildcareUtility.CanSuckle(pawn, out var _))
				{
					num = 3f;
				}
				else
				{
					((Rect)(ref r)).y = ((Rect)(ref r)).y + 3f;
				}
				Vector2 size2 = new Vector2(((Rect)(ref r)).width, ((Rect)(ref r)).height);
				Rot4 value = rot.Value;
				float cameraZoom = num;
				result = (Texture)(object)PortraitsCache.Get(pawn, size2, value, default(Vector3), cameraZoom);
			}
		}
		else
		{
			Rot4 valueOrDefault = rot.GetValueOrDefault();
			if (!rot.HasValue)
			{
				valueOrDefault = thing.def.defaultPlacingRot;
				rot = valueOrDefault;
			}
			Material val2 = (stackOfOne ? ((!(thing.Graphic is Graphic_StackCount graphic_StackCount) || thing.Graphic is Graphic_MealVariants) ? thing.Graphic.ExtractInnerGraphicFor(thing).MatAt(rot.Value) : graphic_StackCount.SubGraphicForStackCount(1, thing.def).MatSingleFor(thing)) : ((!(thing.Graphic is Graphic_Linked graphic_Linked)) ? thing.Graphic.ExtractInnerGraphicFor(thing).MatAt(rot.Value) : graphic_Linked.SubGraphic.ExtractInnerGraphicFor(thing).MatAt(rot.Value)));
			result = val2.mainTexture;
			if (ShaderDatabase.TryGetUIShader(val2.shader, out var uiShader2) && MaterialPool.TryGetRequestForMat(val2, out var request2))
			{
				request2.shader = uiShader2;
				material = MaterialPool.MatFrom(request2);
				color = Color.white;
			}
		}
		return result;
	}

	private static void ThingIconWorker(Rect rect, ThingDef thingDef, Texture resolvedIcon, float resolvedIconAngle, float scale = 1f, Rot4? rot = null, Material mat = null, float alpha = 1f)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Vector2 texProportions = default(Vector2);
		((Vector2)(ref texProportions))._002Ector((float)resolvedIcon.width, (float)resolvedIcon.height);
		Rect texCoords = DefaultTexCoords;
		Rot4 valueOrDefault = rot.GetValueOrDefault();
		if (!rot.HasValue)
		{
			valueOrDefault = thingDef.defaultPlacingRot;
			rot = valueOrDefault;
		}
		if (thingDef.graphicData != null)
		{
			texProportions = (rot.Value.IsHorizontal ? thingDef.graphicData.drawSize.Rotated() : thingDef.graphicData.drawSize);
			if (thingDef.uiIconPath.NullOrEmpty() && thingDef.graphicData.linkFlags != 0)
			{
				texCoords = LinkedTexCoords;
			}
		}
		DrawTextureFitted(rect, resolvedIcon, scale, texProportions, texCoords, resolvedIconAngle, mat, alpha);
	}

	public static Rect CroppedTerrainTextureRect(Texture2D tex)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(0f, 0f, 64f / (float)((Texture)tex).width, 64f / (float)((Texture)tex).height);
	}

	public static void DrawAltRect(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)AltTexture);
	}

	public static void ListSeparator(ref RectDivider divider, string label)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		RectDivider rectDivider = divider.NewRow(25f);
		GUI.BeginGroup((Rect)rectDivider);
		float curY = 0f;
		Rect rect = rectDivider.Rect;
		ListSeparator(ref curY, ((Rect)(ref rect)).width, label);
		GUI.EndGroup();
	}

	public static void ListSeparator(ref float curY, float width, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		curY += 3f;
		GUI.color = SeparatorLabelColor;
		Rect rect = new Rect(0f, curY, width, 30f);
		Text.Anchor = (TextAnchor)0;
		Label(rect, label);
		curY += 20f;
		GUI.color = SeparatorLineColor;
		DrawLineHorizontal(0f, curY, width);
		curY += 2f;
		GUI.color = color;
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

	public static void DrawLineHorizontal(float x, float y, float length, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		DrawBoxSolid(new Rect(x, y, length, 1f), color);
	}

	public static void DrawLineHorizontal(float x, float y, float length)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(new Rect(x, y, length, 1f), (Texture)(object)BaseContent.WhiteTex);
	}

	public static void DrawLineVertical(float x, float y, float length)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(new Rect(x, y, 1f, length), (Texture)(object)BaseContent.WhiteTex);
	}

	public static void DrawBoxSolid(Rect rect, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Color color2 = GUI.color;
		GUI.color = color;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = color2;
	}

	public static void DrawBoxSolidWithOutline(Rect rect, Color solidColor, Color outlineColor, int outlineThickness = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		DrawBoxSolid(rect, solidColor);
		Color color = GUI.color;
		GUI.color = outlineColor;
		DrawBox(rect, outlineThickness);
		GUI.color = color;
	}

	public static void DrawBox(Rect rect, int thickness = 1, Texture2D lineTexture = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height);
		if (val.x > val2.x)
		{
			ref float x = ref val.x;
			ref float x2 = ref val2.x;
			float x3 = val2.x;
			float x4 = val.x;
			x = x3;
			x2 = x4;
		}
		if (val.y > val2.y)
		{
			ref float x = ref val.y;
			ref float y = ref val2.y;
			float x4 = val2.y;
			float x3 = val.y;
			x = x4;
			y = x3;
		}
		Vector3 val3 = Vector2.op_Implicit(val2 - val);
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x, val.y, (float)thickness, val3.y)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val2.x - (float)thickness, val.y, (float)thickness, val3.y)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x + (float)thickness, val.y, val3.x - (float)(thickness * 2), (float)thickness)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
		GUI.DrawTexture(UIScaling.AdjustRectToUIScaling(new Rect(val.x + (float)thickness, val2.y - (float)thickness, val3.x - (float)(thickness * 2), (float)thickness)), (Texture)(object)(lineTexture ?? BaseContent.WhiteTex));
	}

	public static void LabelCacheHeight(ref Rect rect, string label, bool renderLabel = true, bool forceInvalidation = false)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		bool flag = LabelCache.ContainsKey(label);
		float num = 0f;
		if (forceInvalidation)
		{
			flag = false;
		}
		num = ((!flag) ? Text.CalcHeight(label, ((Rect)(ref rect)).width) : LabelCache[label]);
		((Rect)(ref rect)).height = num;
		if (renderLabel)
		{
			Label(rect, label);
		}
	}

	public static void Label(Rect rect, GUIContent content)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.Label(rect, content, Text.CurFontStyle);
	}

	public static void LabelEllipses(Rect rect, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		label = Text.ClampTextWithEllipsis(rect, label);
		Label(rect, label);
	}

	public static void Label(Rect rect, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		float num = Prefs.UIScale / 2f;
		if (Prefs.UIScale > 1f && Math.Abs(num - Mathf.Floor(num)) > float.Epsilon)
		{
			((Rect)(ref val)).xMin = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect)).xMin);
			((Rect)(ref val)).yMin = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect)).yMin);
			((Rect)(ref val)).xMax = UIScaling.AdjustCoordToUIScalingCeil(((Rect)(ref rect)).xMax + 1E-05f);
			((Rect)(ref val)).yMax = UIScaling.AdjustCoordToUIScalingCeil(((Rect)(ref rect)).yMax + 1E-05f);
		}
		GUI.Label(val, label, Text.CurFontStyle);
	}

	public static void Label(Rect rect, TaggedString label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Label(rect, label.Resolve());
	}

	public static void Label(float x, ref float curY, float width, string text, TipSignal tip = default(TipSignal))
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!text.NullOrEmpty())
		{
			float num = Text.CalcHeight(text, width);
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(x, curY, width, num);
			if (!tip.text.NullOrEmpty() || tip.textGetter != null)
			{
				float x2 = Text.CalcSize(text).x;
				Rect rect2 = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, x2, num);
				DrawHighlightIfMouseover(rect2);
				TooltipHandler.TipRegion(rect2, tip);
			}
			Label(rect, text);
			curY += num;
		}
	}

	public static void Label(Rect rect, ref float y, string text, TipSignal tip = default(TipSignal))
	{
		if (!text.NullOrEmpty())
		{
			Label(((Rect)(ref rect)).x, ref y, ((Rect)(ref rect)).width, text, tip);
		}
	}

	public static void LongLabel(float x, float width, string label, ref float curY, bool draw = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (label.Length < 2500)
		{
			if (draw)
			{
				Label(new Rect(x, curY, width, 1000f), label);
			}
			curY += Text.CalcHeight(label, width);
			return;
		}
		int num = 0;
		int num2 = -1;
		bool flag = false;
		for (int i = 0; i < label.Length; i++)
		{
			if (label[i] != '\n')
			{
				continue;
			}
			num++;
			if (num >= 50)
			{
				string text = label.Substring(num2 + 1, i - num2 - 1);
				num2 = i;
				num = 0;
				if (flag)
				{
					curY += Text.SpaceBetweenLines;
				}
				if (draw)
				{
					Label(new Rect(x, curY, width, 10000f), text);
				}
				curY += Text.CalcHeight(text, width);
				flag = true;
			}
		}
		if (num2 != label.Length - 1)
		{
			if (flag)
			{
				curY += Text.SpaceBetweenLines;
			}
			string text2 = label.Substring(num2 + 1);
			if (draw)
			{
				Label(new Rect(x, curY, width, 10000f), text2);
			}
			curY += Text.CalcHeight(text2, width);
			flag = true;
		}
	}

	public static void LabelScrollable(Rect rect, string label, ref Vector2 scrollbarPosition, bool dontConsumeScrollEventsIfNoScrollbar = false, bool takeScrollbarSpaceEvenIfNoScrollbar = true, bool longLabel = false)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		int num;
		int num2;
		if (!takeScrollbarSpaceEvenIfNoScrollbar)
		{
			num = ((Text.CalcHeight(label, ((Rect)(ref rect)).width) > ((Rect)(ref rect)).height) ? 1 : 0);
			if (num == 0)
			{
				num2 = 0;
				goto IL_0045;
			}
		}
		else
		{
			num = 1;
		}
		num2 = ((!dontConsumeScrollEventsIfNoScrollbar || Text.CalcHeight(label, ((Rect)(ref rect)).width - 16f) > ((Rect)(ref rect)).height) ? 1 : 0);
		goto IL_0045;
		IL_0045:
		bool flag = (byte)num2 != 0;
		float num3 = ((Rect)(ref rect)).width;
		if (num != 0)
		{
			num3 -= 16f;
		}
		float curY;
		if (longLabel)
		{
			curY = 0f;
			LongLabel(0f, num3, label, ref curY, draw: false);
		}
		else
		{
			curY = Text.CalcHeight(label, num3);
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, num3, Mathf.Max(curY + 5f, ((Rect)(ref rect)).height));
		if (flag)
		{
			BeginScrollView(rect, ref scrollbarPosition, val);
		}
		else
		{
			BeginGroup(rect);
		}
		if (longLabel)
		{
			float curY2 = ((Rect)(ref val)).y;
			LongLabel(((Rect)(ref val)).x, ((Rect)(ref val)).width, label, ref curY2);
		}
		else
		{
			Label(val, label);
		}
		if (flag)
		{
			EndScrollView();
		}
		else
		{
			EndGroup();
		}
	}

	public static void LabelWithIcon(Rect rect, string label, Texture2D labelIcon, float labelIconScale = 1f)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Min((float)((Texture)labelIcon).width, ((Rect)(ref rect)).height);
		Rect outerRect = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, num, ((Rect)(ref rect)).height);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + num;
		DrawTextureFitted(outerRect, (Texture)(object)labelIcon, labelIconScale);
		Label(rect, label);
	}

	public static void DefLabelWithIcon(Rect rect, Def def, float iconMargin = 2f, float textOffsetX = 6f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		DrawHighlightIfMouseover(rect);
		TooltipHandler.TipRegion(rect, def.description);
		BeginGroup(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 0f, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
		if (iconMargin != 0f)
		{
			rect2 = rect2.ContractedBy(iconMargin);
		}
		DefIcon(rect2, def, null, 1f, null, drawPlaceholder: true);
		Rect rect3 = new Rect(((Rect)(ref rect2)).xMax + textOffsetX, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Label(rect3, def.LabelCap);
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		EndGroup();
	}

	public static bool LabelFit(Rect rect, string label)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		GameFont font = Text.Font;
		Text.Font = GameFont.Small;
		if (Text.CalcSize(label).x <= ((Rect)(ref rect)).width)
		{
			Label(rect, label);
		}
		else
		{
			Text.Font = GameFont.Tiny;
			if (Text.CalcSize(label).x <= ((Rect)(ref rect)).width)
			{
				Label(rect, label);
			}
			else
			{
				LabelEllipses(rect, label);
				result = true;
			}
			Text.Font = GameFont.Small;
		}
		Text.Font = font;
		return result;
	}

	public static void HyperlinkWithIcon(Rect rect, Dialog_InfoCard.Hyperlink hyperlink, string text = null, float iconMargin = 2f, float textOffsetX = 6f, Color? color = null, bool truncateLabel = false, string textSuffix = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		string text2 = text ?? hyperlink.Label.CapitalizeFirst();
		if (textSuffix != null)
		{
			text2 += textSuffix;
		}
		BeginGroup(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
		if (iconMargin != 0f)
		{
			val = val.ContractedBy(iconMargin);
		}
		if (hyperlink.IsHidden)
		{
			DrawTextureFitted(val, (Texture)(object)PlaceholderIconTex, 1f);
		}
		else if (hyperlink.thing != null)
		{
			ThingIcon(val, hyperlink.thing);
		}
		else
		{
			DefIcon(val, hyperlink.def, null, 1f, null, drawPlaceholder: true);
		}
		float num = ((Rect)(ref val)).xMax + textOffsetX;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).xMax + textOffsetX, 0f, ((Rect)(ref rect)).width - num, ((Rect)(ref rect)).height);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Color textColor = (Color)(((_003F?)color) ?? NormalOptionColor);
		if (hyperlink.IsHidden)
		{
			textColor = Color.gray;
		}
		ButtonText(val2, truncateLabel ? text2.Truncate(((Rect)(ref val2)).width) : text2, drawBackground: false, doMouseoverSound: false, textColor, active: false);
		if (ButtonInvisible(val2))
		{
			hyperlink.ActivateHyperlink();
		}
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		EndGroup();
	}

	public static void DrawNumberOnMap(Vector2 screenPos, int number, Color textColor)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = (TextAnchor)4;
		Text.Font = GameFont.Medium;
		string text = number.ToStringCached();
		float val = Text.CalcSize(text).x + 8f;
		Rect val2 = new Rect(screenPos.x - 20f, screenPos.y - 15f, Math.Max(40f, val), 30f);
		GUI.DrawTexture(val2, (Texture)(object)TexUI.GrayBg);
		GUI.color = textColor;
		Label(val2, text);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawStringOnMap(Vector2 screenPos, string str, Color textColor)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = (TextAnchor)4;
		Text.Font = GameFont.Medium;
		float num = Text.CalcSize(str).x + 8f;
		Rect val = new Rect(screenPos.x - num / 2f, screenPos.y - 15f, num, 30f);
		GUI.DrawTexture(val, (Texture)(object)TexUI.GrayBg);
		GUI.color = textColor;
		Label(val, str);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
	}

	public static void Checkbox(Vector2 topLeft, ref bool checkOn, float size = 24f, bool disabled = false, bool paintable = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Checkbox(topLeft.x, topLeft.y, ref checkOn, size, disabled, paintable, texChecked, texUnchecked);
	}

	public static void Checkbox(float x, float y, ref bool checkOn, float size = 24f, bool disabled = false, bool paintable = false, Texture2D texChecked = null, Texture2D texUnchecked = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (disabled)
		{
			GUI.color = InactiveColor;
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, size, size);
		CheckboxDraw(x, y, checkOn, disabled, size, texChecked, texUnchecked);
		if (!disabled)
		{
			ToggleInvisibleDraggable(rect, ref checkOn, doMouseoverSound: true, paintable);
		}
		if (disabled)
		{
			GUI.color = Color.white;
		}
	}

	public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, bool paintable = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)3;
		if (placeCheckboxNearText)
		{
			((Rect)(ref rect)).width = Mathf.Min(((Rect)(ref rect)).width, Text.CalcSize(label).x + 24f + 10f);
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 24f;
		Label(rect2, label);
		if (!disabled)
		{
			ToggleInvisibleDraggable(rect, ref checkOn, doMouseoverSound: true, paintable);
		}
		CheckboxDraw(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 24f, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - 24f) / 2f, checkOn, disabled, 24f, texChecked, texUnchecked);
		Text.Anchor = anchor;
	}

	public static void ToggleInvisibleDraggable(Rect rect, ref bool checkOn, bool doMouseoverSound = false, bool paintable = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		DraggableResult draggableResult = ButtonInvisibleDraggable(rect, doMouseoverSound);
		bool flag = false;
		if (draggableResult == DraggableResult.Pressed)
		{
			checkOn = !checkOn;
			flag = true;
		}
		else if (draggableResult == DraggableResult.Dragged && paintable)
		{
			checkOn = !checkOn;
			flag = true;
			checkboxPainting = true;
			checkboxPaintingState = checkOn;
		}
		if (paintable && Mouse.IsOver(rect) && checkboxPainting && Input.GetMouseButton(0) && checkOn != checkboxPaintingState)
		{
			checkOn = checkboxPaintingState;
			flag = true;
		}
		if (doMouseoverSound && flag)
		{
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

	public static bool CheckboxLabeledSelectable(Rect rect, string label, ref bool selected, ref bool checkOn, Texture2D labelIcon = null, float labelIconScale = 1f)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (selected)
		{
			DrawHighlight(rect);
		}
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)3;
		if ((Object)(object)labelIcon != (Object)null)
		{
			Rect outerRect = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, (float)((Texture)labelIcon).width, ((Rect)(ref rect)).height);
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + (float)((Texture)labelIcon).width;
			DrawTextureFitted(outerRect, (Texture)(object)labelIcon, labelIconScale);
		}
		Label(rect, label);
		Text.Anchor = anchor;
		bool flag = selected;
		Rect butRect = rect;
		((Rect)(ref butRect)).width = ((Rect)(ref butRect)).width - 24f;
		if (!selected && ButtonInvisible(butRect))
		{
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			selected = true;
		}
		Color color = GUI.color;
		GUI.color = Color.white;
		CheckboxDraw(((Rect)(ref rect)).xMax - 24f, ((Rect)(ref rect)).y, checkOn, disabled: false);
		GUI.color = color;
		if (ButtonInvisible(new Rect(((Rect)(ref rect)).xMax - 24f, ((Rect)(ref rect)).y, 24f, 24f)))
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
		if (selected)
		{
			return !flag;
		}
		return false;
	}

	public static Texture2D GetCheckboxTexture(bool state)
	{
		if (state)
		{
			return CheckboxOnTex;
		}
		return CheckboxOffTex;
	}

	public static void CheckboxDraw(float x, float y, bool active, bool disabled, float size = 24f, Texture2D texChecked = null, Texture2D texUnchecked = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		if (disabled)
		{
			GUI.color = InactiveColor;
		}
		Texture2D val = ((!active) ? (((Object)(object)texUnchecked != (Object)null) ? texUnchecked : CheckboxOffTex) : (((Object)(object)texChecked != (Object)null) ? texChecked : CheckboxOnTex));
		GUI.DrawTexture(new Rect(x, y, size, size), (Texture)(object)val);
		if (disabled)
		{
			GUI.color = color;
		}
	}

	public static MultiCheckboxState CheckboxMulti(Rect rect, MultiCheckboxState state, bool paintable = false)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Texture2D tex = (Texture2D)(state switch
		{
			MultiCheckboxState.On => CheckboxOnTex, 
			MultiCheckboxState.Off => CheckboxOffTex, 
			_ => CheckboxPartialTex, 
		});
		MouseoverSounds.DoRegion(rect);
		MultiCheckboxState multiCheckboxState = ((state != MultiCheckboxState.Off) ? MultiCheckboxState.Off : MultiCheckboxState.On);
		bool flag = false;
		DraggableResult draggableResult = ButtonImageDraggable(rect, tex);
		if (paintable && draggableResult == DraggableResult.Dragged)
		{
			checkboxPainting = true;
			checkboxPaintingState = multiCheckboxState == MultiCheckboxState.On;
			flag = true;
		}
		else if (draggableResult.AnyPressed())
		{
			flag = true;
		}
		else if (paintable && checkboxPainting && Mouse.IsOver(rect))
		{
			multiCheckboxState = ((!checkboxPaintingState) ? MultiCheckboxState.Off : MultiCheckboxState.On);
			if (state != multiCheckboxState)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (multiCheckboxState == MultiCheckboxState.On)
			{
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
			return multiCheckboxState;
		}
		return state;
	}

	public static bool RadioButton(Vector2 topLeft, bool chosen, bool disabled = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return RadioButton(topLeft.x, topLeft.y, chosen, disabled);
	}

	public static bool RadioButton(float x, float y, bool chosen, bool disabled = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Rect butRect = new Rect(x, y, 24f, 24f);
		RadioButtonDraw(x, y, chosen, disabled);
		bool num = ButtonInvisible(butRect);
		if (num && !chosen)
		{
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
		}
		return num;
	}

	public static bool RadioButtonLabeled(Rect rect, string labelText, bool chosen, bool disabled = false)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		TextBlock textBlock = new TextBlock((TextAnchor)3, disabled ? ColoredText.SubtleGrayColor : Color.white);
		try
		{
			Label(rect, labelText);
		}
		finally
		{
			((IDisposable)textBlock).Dispose();
		}
		bool num = ButtonInvisible(rect);
		if (num && !chosen && !disabled)
		{
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
		}
		RadioButtonDraw(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 24f, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height / 2f - 12f, chosen, disabled);
		return num;
	}

	private static void RadioButtonDraw(float x, float y, bool chosen, bool disabled)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		GUI.color = Color.white;
		Texture2D val = ((!chosen) ? RadioButOffTex : RadioButOnTex);
		Rect val2 = new Rect(x, y, 24f, 24f);
		if (disabled)
		{
			GUI.color = Color.gray;
		}
		GUI.DrawTexture(val2, (Texture)(object)val);
		GUI.color = color;
	}

	public static bool ButtonText(Rect rect, string label, bool drawBackground = true, bool doMouseoverSound = true, bool active = true, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return ButtonText(rect, label, drawBackground, doMouseoverSound, NormalOptionColor, active, overrideTextAnchor);
	}

	public static bool ButtonText(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor, bool active = true, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return ButtonTextWorker(rect, label, drawBackground, doMouseoverSound, textColor, active, draggable: false, overrideTextAnchor).AnyPressed();
	}

	public static DraggableResult ButtonTextDraggable(Rect rect, string label, bool drawBackground = true, bool doMouseoverSound = false, bool active = true, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return ButtonTextDraggable(rect, label, drawBackground, doMouseoverSound, NormalOptionColor, active, overrideTextAnchor);
	}

	public static DraggableResult ButtonTextDraggable(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor, bool active = true, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return ButtonTextWorker(rect, label, drawBackground, doMouseoverSound, NormalOptionColor, active, draggable: true, overrideTextAnchor);
	}

	public static void DrawButtonGraphic(Rect rect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Texture2D atlas = ButtonBGAtlas;
		if (Mouse.IsOver(rect))
		{
			atlas = ButtonBGAtlasMouseover;
			if (Input.GetMouseButton(0))
			{
				atlas = ButtonBGAtlasClick;
			}
		}
		DrawAtlas(rect, atlas);
	}

	private static DraggableResult ButtonTextWorker(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor, bool active, bool draggable, TextAnchor? overrideTextAnchor = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		TextAnchor anchor = Text.Anchor;
		Color color = GUI.color;
		if (drawBackground)
		{
			DrawButtonGraphic(rect);
		}
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(rect);
		}
		if (!drawBackground)
		{
			GUI.color = textColor;
			if (Mouse.IsOver(rect))
			{
				GUI.color = MouseoverOptionColor;
			}
		}
		if (overrideTextAnchor.HasValue)
		{
			Text.Anchor = overrideTextAnchor.Value;
		}
		else if (drawBackground)
		{
			Text.Anchor = (TextAnchor)4;
		}
		else
		{
			Text.Anchor = (TextAnchor)3;
		}
		bool wordWrap = Text.WordWrap;
		if (((Rect)(ref rect)).height < Text.LineHeight * 2f)
		{
			Text.WordWrap = false;
		}
		Label(rect, label);
		Text.Anchor = anchor;
		GUI.color = color;
		Text.WordWrap = wordWrap;
		if (active && draggable)
		{
			return ButtonInvisibleDraggable(rect);
		}
		if (active)
		{
			if (!ButtonInvisible(rect, doMouseoverSound: false))
			{
				return DraggableResult.Idle;
			}
			return DraggableResult.Pressed;
		}
		return DraggableResult.Idle;
	}

	public static void DrawRectFast(Rect position, Color color, GUIContent content = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box(position, content ?? GUIContent.none, TexUI.FastFillStyle);
		GUI.backgroundColor = backgroundColor;
	}

	public static bool CustomButtonText(ref Rect rect, string label, Color bgColor, Color textColor, Color borderColor, Color unfilledBgColor = default(Color), bool cacheHeight = false, float borderSize = 1f, bool doMouseoverSound = true, bool active = true, float fillPercent = 1f)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (cacheHeight)
		{
			LabelCacheHeight(ref rect, label, renderLabel: false);
		}
		Rect position = default(Rect);
		((Rect)(ref position))._002Ector(rect);
		((Rect)(ref position)).x = ((Rect)(ref position)).x + borderSize;
		((Rect)(ref position)).y = ((Rect)(ref position)).y + borderSize;
		((Rect)(ref position)).width = ((Rect)(ref position)).width - borderSize * 2f;
		((Rect)(ref position)).height = ((Rect)(ref position)).height - borderSize * 2f;
		DrawRectFast(rect, borderColor);
		if (unfilledBgColor != default(Color))
		{
			DrawRectFast(position, unfilledBgColor);
		}
		((Rect)(ref position)).width = ((Rect)(ref position)).width * fillPercent;
		DrawRectFast(position, bgColor);
		TextAnchor anchor = Text.Anchor;
		Color color = GUI.color;
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(rect);
		}
		GUI.color = textColor;
		if (Mouse.IsOver(rect))
		{
			GUI.color = MouseoverOptionColor;
		}
		Text.Anchor = (TextAnchor)4;
		Label(rect, label);
		Text.Anchor = anchor;
		GUI.color = color;
		if (active)
		{
			return ButtonInvisible(rect, doMouseoverSound: false);
		}
		return false;
	}

	public static bool ButtonTextSubtle(Rect rect, string label, float barPercent = 0f, float textLeftMargin = -1f, SoundDef mouseoverSound = null, Vector2 functionalSizeOffset = default(Vector2), Color? labelColor = null, bool highlight = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).width = ((Rect)(ref val)).width + functionalSizeOffset.x;
		((Rect)(ref val)).height = ((Rect)(ref val)).height + functionalSizeOffset.y;
		bool flag = false;
		if (Mouse.IsOver(val))
		{
			flag = true;
			GUI.color = GenUI.MouseoverColor;
		}
		if (mouseoverSound != null)
		{
			MouseoverSounds.DoRegion(val, mouseoverSound);
		}
		DrawAtlas(rect, ButtonSubtleAtlas);
		if (highlight)
		{
			GUI.color = Color.grey;
			DrawBox(rect, 2);
		}
		GUI.color = Color.white;
		if (barPercent > 0.001f)
		{
			FillableBar(rect.ContractedBy(1f), barPercent, ButtonBarTex, null, doBorder: false);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		if (textLeftMargin < 0f)
		{
			textLeftMargin = ((Rect)(ref rect)).width * 0.15f;
		}
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + textLeftMargin;
		if (flag)
		{
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 2f;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - 2f;
		}
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Text.Font = GameFont.Small;
		GUI.color = (Color)(((_003F?)labelColor) ?? Color.white);
		Label(rect2, label);
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		GUI.color = Color.white;
		return ButtonInvisible(val, doMouseoverSound: false);
	}

	public static bool ButtonImage(Rect butRect, Texture2D tex, bool doMouseoverSound = true, string tooltip = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImage(butRect, tex, Color.white, doMouseoverSound, tooltip);
	}

	public static bool ButtonImage(Rect butRect, Texture2D tex, Color baseColor, bool doMouseoverSound = true, string tooltip = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImage(butRect, tex, baseColor, GenUI.MouseoverColor, doMouseoverSound, tooltip);
	}

	public static bool ButtonImage(Rect butRect, Texture2D tex, Color baseColor, Color mouseoverColor, bool doMouseoverSound = true, string tooltip = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = (Mouse.IsOver(butRect) ? mouseoverColor : baseColor);
		GUI.DrawTexture(butRect, (Texture)(object)tex);
		GUI.color = baseColor;
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(butRect, tooltip);
		}
		bool result = ButtonInvisible(butRect, doMouseoverSound);
		GUI.color = Color.white;
		return result;
	}

	public static DraggableResult ButtonImageDraggable(Rect butRect, Texture2D tex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImageDraggable(butRect, tex, Color.white);
	}

	public static DraggableResult ButtonImageDraggable(Rect butRect, Texture2D tex, Color baseColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImageDraggable(butRect, tex, baseColor, GenUI.MouseoverColor);
	}

	public static DraggableResult ButtonImageDraggable(Rect butRect, Texture2D tex, Color baseColor, Color mouseoverColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(butRect))
		{
			GUI.color = mouseoverColor;
		}
		else
		{
			GUI.color = baseColor;
		}
		GUI.DrawTexture(butRect, (Texture)(object)tex);
		GUI.color = baseColor;
		return ButtonInvisibleDraggable(butRect);
	}

	public static bool ButtonImageFitted(Rect butRect, Texture2D tex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImageFitted(butRect, tex, Color.white);
	}

	public static bool ButtonImageFitted(Rect butRect, Texture2D tex, Color baseColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImageFitted(butRect, tex, baseColor, GenUI.MouseoverColor);
	}

	public static bool ButtonImageFitted(Rect butRect, Texture2D tex, Color baseColor, Color mouseoverColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(butRect))
		{
			GUI.color = mouseoverColor;
		}
		else
		{
			GUI.color = baseColor;
		}
		DrawTextureFitted(butRect, (Texture)(object)tex, 1f);
		GUI.color = baseColor;
		return ButtonInvisible(butRect);
	}

	public static bool ButtonImageWithBG(Rect butRect, Texture2D image, Vector2? imageSize = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		bool result = ButtonText(butRect, "");
		Rect val = default(Rect);
		if (imageSize.HasValue)
		{
			((Rect)(ref val))._002Ector(Mathf.Floor(((Rect)(ref butRect)).x + ((Rect)(ref butRect)).width / 2f - imageSize.Value.x / 2f), Mathf.Floor(((Rect)(ref butRect)).y + ((Rect)(ref butRect)).height / 2f - imageSize.Value.y / 2f), imageSize.Value.x, imageSize.Value.y);
		}
		else
		{
			val = butRect;
		}
		GUI.DrawTexture(val, (Texture)(object)image);
		return result;
	}

	public static bool CloseButtonFor(Rect rectToClose)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return ButtonImage(new Rect(((Rect)(ref rectToClose)).x + ((Rect)(ref rectToClose)).width - 18f - 4f, ((Rect)(ref rectToClose)).y + 4f, 18f, 18f), TexButton.CloseXSmall);
	}

	public static bool BackButtonFor(Rect rectToBack)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		return ButtonText(new Rect(((Rect)(ref rectToBack)).x + ((Rect)(ref rectToBack)).width - 18f - 4f - 120f - 16f, ((Rect)(ref rectToBack)).y + 18f, 120f, 40f), "Back".Translate());
	}

	public static bool ButtonInvisible(Rect butRect, bool doMouseoverSound = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(butRect);
		}
		return GUI.Button(butRect, "", EmptyStyle);
	}

	public static DraggableResult ButtonInvisibleDraggable(Rect butRect, bool doMouseoverSound = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (doMouseoverSound)
		{
			MouseoverSounds.DoRegion(butRect);
		}
		int controlID = GUIUtility.GetControlID((FocusType)2, butRect);
		if (Input.GetMouseButtonDown(0) && Mouse.IsOver(butRect))
		{
			GUIUtility.keyboardControl = 0;
			buttonInvisibleDraggable_activeControl = controlID;
			buttonInvisibleDraggable_mouseStart = Input.mousePosition;
			buttonInvisibleDraggable_dragged = false;
		}
		if (buttonInvisibleDraggable_activeControl == controlID)
		{
			if (Input.GetMouseButtonUp(0))
			{
				buttonInvisibleDraggable_activeControl = 0;
				if (Mouse.IsOver(butRect))
				{
					if (!buttonInvisibleDraggable_dragged)
					{
						return DraggableResult.Pressed;
					}
					return DraggableResult.DraggedThenPressed;
				}
				return DraggableResult.Idle;
			}
			if (!Input.GetMouseButton(0))
			{
				buttonInvisibleDraggable_activeControl = 0;
				return DraggableResult.Idle;
			}
			if (!buttonInvisibleDraggable_dragged)
			{
				Vector3 val = buttonInvisibleDraggable_mouseStart - Input.mousePosition;
				if (((Vector3)(ref val)).sqrMagnitude > DragStartDistanceSquared)
				{
					buttonInvisibleDraggable_dragged = true;
					return DraggableResult.Dragged;
				}
			}
		}
		return DraggableResult.Idle;
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

	public static string TextField(Rect rect, string text, int maxLength, Regex inputValidator = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string text2 = TextField(rect, text);
		if (text2.Length <= maxLength && (inputValidator == null || inputValidator.IsMatch(text2)))
		{
			return text2;
		}
		return text;
	}

	public static string TextArea(Rect rect, string text, bool readOnly = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (text == null)
		{
			text = "";
		}
		return GUI.TextArea(rect, text, readOnly ? Text.CurTextAreaReadOnlyStyle : Text.CurTextAreaStyle);
	}

	public static string TextEntryLabeled(Rect rect, string label, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect.LeftHalf().Rounded();
		Rect rect3 = rect.RightHalf().Rounded();
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)5;
		Label(rect2, label);
		Text.Anchor = anchor;
		if (((Rect)(ref rect)).height <= 30f)
		{
			return TextField(rect3, text);
		}
		return TextArea(rect3, text);
	}

	public static string DelayedTextField(Rect rect, string text, ref string buffer, string previousFocusedControlName, string controlName = null)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Invalid comparison between Unknown and I4
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		controlName = controlName ?? $"TextField{((Rect)(ref rect)).x},{((Rect)(ref rect)).y}";
		bool num = previousFocusedControlName == controlName;
		bool flag = GUI.GetNameOfFocusedControl() == controlName;
		string text2 = controlName + "_unfocused";
		GUI.SetNextControlName(text2);
		GUI.Label(rect, "");
		GUI.SetNextControlName(controlName);
		bool flag2 = false;
		if (flag && (int)Event.current.type == 4 && ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271))
		{
			Event.current.Use();
			flag2 = true;
		}
		bool flag3 = false;
		if ((int)Event.current.type == 0 && !((Rect)(ref rect)).Contains(Event.current.mousePosition))
		{
			flag3 = true;
		}
		if (num)
		{
			buffer = TextField(rect, buffer);
			if (!flag)
			{
				return buffer;
			}
			if (flag3 || flag2)
			{
				GUI.FocusControl(text2);
				return buffer;
			}
			return text;
		}
		buffer = TextField(rect, text);
		return buffer;
	}

	public static void TextFieldVector(Rect rect, ref Vector3 vector, ref string[] buffer, float min = 0f, float max = 1E+09f)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (buffer == null)
		{
			buffer = new string[3];
		}
		float width = ((Rect)(ref rect)).width / 3f - 4f;
		Rect val = rect.LeftPartPixels(width);
		Rect val2 = val;
		Rect rect2 = val2;
		((Rect)(ref val2)).x = ((Rect)(ref val)).xMax + 4f;
		((Rect)(ref rect2)).x = ((Rect)(ref val2)).xMax + 4f;
		TextFieldNumeric(val, ref vector.x, ref buffer[0], min, max);
		TextFieldNumeric(val2, ref vector.y, ref buffer[1], min, max);
		TextFieldNumeric(rect2, ref vector.z, ref buffer[2], min, max);
	}

	public static void TextFieldNumeric<T>(Rect rect, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (buffer == null)
		{
			buffer = val.ToString();
		}
		GUI.SetNextControlName("TextField" + ((Rect)(ref rect)).y.ToString("F0") + ((Rect)(ref rect)).x.ToString("F0"));
		string text = TextField(rect, buffer);
		if (text != buffer && IsPartiallyOrFullyTypedNumber(ref val, text, min, max))
		{
			buffer = text;
			if (text.IsFullyTypedNumber<T>())
			{
				ResolveParseNow(text, ref val, ref buffer, min, max, force: false);
			}
		}
	}

	private static void ResolveParseNow<T>(string edited, ref T val, ref string buffer, float min, float max, bool force)
	{
		if (typeof(T) == typeof(int))
		{
			int result;
			if (edited.NullOrEmpty())
			{
				ResetValue(edited, ref val, ref buffer, min, max);
			}
			else if (int.TryParse(edited, out result))
			{
				val = (T)(object)Mathf.RoundToInt(Mathf.Clamp((float)result, min, max));
				buffer = ToStringTypedIn(val);
			}
			else if (force)
			{
				ResetValue(edited, ref val, ref buffer, min, max);
			}
		}
		else if (typeof(T) == typeof(float))
		{
			if (float.TryParse(edited, out var result2))
			{
				val = (T)(object)Mathf.Clamp(result2, min, max);
				buffer = ToStringTypedIn(val);
			}
			else if (force)
			{
				ResetValue(edited, ref val, ref buffer, min, max);
			}
		}
		else
		{
			Log.Error("TextField<T> does not support " + typeof(T));
		}
	}

	private static void ResetValue<T>(string edited, ref T val, ref string buffer, float min, float max)
	{
		val = default(T);
		if (min > 0f)
		{
			val = (T)(object)Mathf.RoundToInt(min);
		}
		if (max < 0f)
		{
			val = (T)(object)Mathf.RoundToInt(max);
		}
		buffer = ToStringTypedIn(val);
	}

	private static string ToStringTypedIn<T>(T val)
	{
		if (typeof(T) == typeof(float))
		{
			return ((float)(object)val).ToString("0.##########");
		}
		return val.ToString();
	}

	private static bool IsPartiallyOrFullyTypedNumber<T>(ref T val, string s, float min, float max)
	{
		if (s == "")
		{
			return true;
		}
		if (s[0] == '-' && min >= 0f)
		{
			return false;
		}
		if (s.Length > 1 && s[s.Length - 1] == '-')
		{
			return false;
		}
		if (s == "00")
		{
			return false;
		}
		if (s.Length > 12)
		{
			return false;
		}
		if (typeof(T) == typeof(float) && s.CharacterCount('.') <= 1 && s.ContainsOnlyCharacters("-.0123456789"))
		{
			return true;
		}
		if (s.IsFullyTypedNumber<T>())
		{
			return true;
		}
		return false;
	}

	private static bool IsFullyTypedNumber<T>(this string s)
	{
		if (s == "")
		{
			return false;
		}
		if (typeof(T) == typeof(float))
		{
			string[] array = s.Split('.');
			if (array.Length > 2 || array.Length < 1)
			{
				return false;
			}
			if (!array[0].ContainsOnlyCharacters("-0123456789"))
			{
				return false;
			}
			if (array.Length == 2 && (array[1].Length == 0 || !array[1].ContainsOnlyCharacters("0123456789")))
			{
				return false;
			}
		}
		if (typeof(T) == typeof(int) && !s.ContainsOnlyCharacters("-0123456789"))
		{
			return false;
		}
		return true;
	}

	private static bool ContainsOnlyCharacters(this string s, string allowedChars)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (!allowedChars.Contains(s[i]))
			{
				return false;
			}
		}
		return true;
	}

	private static int CharacterCount(this string s, char c)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == c)
			{
				num++;
			}
		}
		return num;
	}

	public static void TextFieldNumericLabeled<T>(Rect rect, string label, ref T val, ref string buffer, float min = 0f, float max = 1E+09f) where T : struct
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect.LeftHalf().Rounded();
		Rect rect3 = rect.RightHalf().Rounded();
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)5;
		Label(rect2, label);
		Text.Anchor = anchor;
		TextFieldNumeric(rect3, ref val, ref buffer, min, max);
	}

	public static void TextFieldPercent(Rect rect, ref float val, ref string buffer, float min = 0f, float max = 1f)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 25f, ((Rect)(ref rect)).height);
		Label(new Rect(((Rect)(ref rect2)).xMax, ((Rect)(ref rect)).y, 25f, ((Rect)(ref rect2)).height), "%");
		float val2 = val * 100f;
		TextFieldNumeric(rect2, ref val2, ref buffer, min * 100f, max * 100f);
		val = val2 / 100f;
		if (val > max)
		{
			val = max;
			buffer = val.ToString();
		}
	}

	public static T ChangeType<T>(this object obj)
	{
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		return (T)Convert.ChangeType(obj, typeof(T), invariantCulture);
	}

	public static void ResetSliderDraggingIDs()
	{
		sliderDraggingID = 0;
		draggingId = 0;
		curDragEnd = RangeEnd.None;
	}

	public static void HorizontalSlider(Rect rect, ref float value, FloatRange range, string label = null, float roundTo = -1f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		float trueMin = range.TrueMin;
		float trueMax = range.TrueMax;
		value = HorizontalSlider(rect, value, trueMin, trueMax, middleAlignment: false, label, trueMin.ToString(), trueMax.ToString(), roundTo);
	}

	public static float HorizontalSlider(Rect rect, float value, float min, float max, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Invalid comparison between Unknown and I4
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		float num = value;
		if (middleAlignment || !label.NullOrEmpty())
		{
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y + Mathf.Round((((Rect)(ref rect)).height - 10f) / 2f);
		}
		if (!label.NullOrEmpty())
		{
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y + 5f;
		}
		Vector2 val = UI.GUIToScreenPoint(new Vector2(((Rect)(ref rect)).x, ((Rect)(ref rect)).y));
		int hashCode = ((object)(Vector2)(ref val)).GetHashCode();
		hashCode = Gen.HashCombine(hashCode, ((Rect)(ref rect)).width);
		hashCode = Gen.HashCombine(hashCode, ((Rect)(ref rect)).height);
		hashCode = Gen.HashCombine(hashCode, min);
		hashCode = Gen.HashCombine(hashCode, max);
		Rect val2 = rect;
		((Rect)(ref val2)).xMin = ((Rect)(ref val2)).xMin + 6f;
		((Rect)(ref val2)).xMax = ((Rect)(ref val2)).xMax - 6f;
		GUI.color = RangeControlTextColor;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y + 2f, ((Rect)(ref val2)).width, 8f);
		DrawAtlas(rect2, SliderRailAtlas);
		GUI.color = Color.white;
		float num2 = Mathf.Clamp(((Rect)(ref val2)).x - 6f + ((Rect)(ref val2)).width * Mathf.InverseLerp(min, max, num), ((Rect)(ref val2)).xMin - 6f, ((Rect)(ref val2)).xMax - 6f);
		GUI.DrawTexture(new Rect(num2, ((Rect)(ref rect2)).center.y - 6f, 12f, 12f), (Texture)(object)SliderHandle);
		if ((int)Event.current.type == 0 && Mouse.IsOver(rect) && sliderDraggingID != hashCode)
		{
			sliderDraggingID = hashCode;
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			Event.current.Use();
		}
		if (sliderDraggingID == hashCode && UnityGUIBugsFixer.MouseDrag())
		{
			num = Mathf.Clamp((Event.current.mousePosition.x - ((Rect)(ref val2)).x) / ((Rect)(ref val2)).width * (max - min) + min, min, max);
			if ((int)Event.current.type == 3)
			{
				Event.current.Use();
			}
		}
		if (!label.NullOrEmpty() || !leftAlignedLabel.NullOrEmpty() || !rightAlignedLabel.NullOrEmpty())
		{
			TextAnchor anchor = Text.Anchor;
			GameFont font = Text.Font;
			Text.Font = GameFont.Small;
			float num3 = (label.NullOrEmpty() ? 18f : Text.CalcSize(label).y);
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y - num3 + 3f;
			if (!leftAlignedLabel.NullOrEmpty())
			{
				Text.Anchor = (TextAnchor)0;
				Label(rect, leftAlignedLabel);
			}
			if (!rightAlignedLabel.NullOrEmpty())
			{
				Text.Anchor = (TextAnchor)2;
				Label(rect, rightAlignedLabel);
			}
			if (!label.NullOrEmpty())
			{
				Text.Anchor = (TextAnchor)1;
				Label(rect, label);
			}
			Text.Anchor = anchor;
			Text.Font = font;
		}
		if (roundTo > 0f)
		{
			num = (float)Mathf.RoundToInt(num / roundTo) * roundTo;
		}
		if (value != num)
		{
			CheckPlayDragSliderSound();
		}
		return num;
	}

	public static float FrequencyHorizontalSlider(Rect rect, float freq, float minFreq, float maxFreq, bool roundToInt = false)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		float num;
		if (freq < 1f)
		{
			float x = 1f / freq;
			num = GenMath.LerpDouble(1f, 1f / minFreq, 0.5f, 1f, x);
		}
		else
		{
			num = GenMath.LerpDouble(maxFreq, 1f, 0f, 0.5f, freq);
		}
		string label = ((freq == 1f) ? ((string)"EveryDay".Translate()) : ((!(freq < 1f)) ? ((string)"EveryDays".Translate(freq.ToString("0.##"))) : ((string)"TimesPerDay".Translate((1f / freq).ToString("0.##")))));
		float num2 = HorizontalSlider(rect, num, 0f, 1f, middleAlignment: true, label);
		if (num != num2)
		{
			float num3;
			if (num2 < 0.5f)
			{
				num3 = GenMath.LerpDouble(0.5f, 0f, 1f, maxFreq, num2);
				if (roundToInt)
				{
					num3 = Mathf.Round(num3);
				}
			}
			else
			{
				float num4 = GenMath.LerpDouble(1f, 0.5f, 1f / minFreq, 1f, num2);
				if (roundToInt)
				{
					num4 = Mathf.Round(num4);
				}
				num3 = 1f / num4;
			}
			freq = num3;
		}
		return freq;
	}

	public static void IntEntry(Rect rect, ref int value, ref string editBuffer, int multiplier = 1)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Min(IntEntryButtonWidth, (int)((Rect)(ref rect)).width / 5);
		if (ButtonText(new Rect(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin, (float)num, ((Rect)(ref rect)).height), (-10 * multiplier).ToStringCached()))
		{
			value -= 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
			editBuffer = value.ToStringCached();
			SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
		}
		if (ButtonText(new Rect(((Rect)(ref rect)).xMin + (float)num, ((Rect)(ref rect)).yMin, (float)num, ((Rect)(ref rect)).height), (-1 * multiplier).ToStringCached()))
		{
			value -= multiplier * GenUI.CurrentAdjustmentMultiplier();
			editBuffer = value.ToStringCached();
			SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
		}
		if (ButtonText(new Rect(((Rect)(ref rect)).xMax - (float)num, ((Rect)(ref rect)).yMin, (float)num, ((Rect)(ref rect)).height), "+" + (10 * multiplier).ToStringCached()))
		{
			value += 10 * multiplier * GenUI.CurrentAdjustmentMultiplier();
			editBuffer = value.ToStringCached();
			SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
		}
		if (ButtonText(new Rect(((Rect)(ref rect)).xMax - (float)(num * 2), ((Rect)(ref rect)).yMin, (float)num, ((Rect)(ref rect)).height), "+" + multiplier.ToStringCached()))
		{
			value += multiplier * GenUI.CurrentAdjustmentMultiplier();
			editBuffer = value.ToStringCached();
			SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
		}
		TextFieldNumeric(new Rect(((Rect)(ref rect)).xMin + (float)(num * 2), ((Rect)(ref rect)).yMin, ((Rect)(ref rect)).width - (float)(num * 4), ((Rect)(ref rect)).height), ref value, ref editBuffer);
	}

	public static void FloatRange(Rect rect, int id, ref FloatRange range, float min = 0f, float max = 1f, string labelKey = null, ToStringStyle valueStyle = ToStringStyle.FloatTwo, float gap = 0f, GameFont sliderLabelFont = GameFont.Small, Color? sliderLabelColor = null, float roundTo = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Invalid comparison between Unknown and I4
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Invalid comparison between Unknown and I4
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 8f;
		((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - 8f;
		GUI.color = (Color)(((_003F?)sliderLabelColor) ?? RangeControlTextColor);
		string text = range.min.ToStringByStyle(valueStyle) + " - " + range.max.ToStringByStyle(valueStyle);
		if (labelKey != null)
		{
			text = labelKey.Translate(text);
		}
		GameFont font = Text.Font;
		Text.Font = sliderLabelFont;
		Text.Anchor = (TextAnchor)1;
		Rect rect2 = val;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - 2f;
		((Rect)(ref rect2)).height = Mathf.Max(((Rect)(ref rect2)).height, Text.CalcHeight(text, ((Rect)(ref rect2)).width));
		LabelFit(rect2, text);
		Text.Anchor = (TextAnchor)0;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 8f - 1f, ((Rect)(ref val)).width, 2f);
		GUI.DrawTexture(val2, (Texture)(object)BaseContent.WhiteTex);
		float num = ((Rect)(ref val)).x + ((Rect)(ref val)).width * Mathf.InverseLerp(min, max, range.min);
		float num2 = ((Rect)(ref val)).x + ((Rect)(ref val)).width * Mathf.InverseLerp(min, max, range.max);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num, ((Rect)(ref val)).yMax - 8f - 2f, num2 - num, 4f), (Texture)(object)BaseContent.WhiteTex);
		float num3 = num;
		float num4 = num2;
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(num3 - 16f, ((Rect)(ref val2)).center.y - 8f, 16f, 16f);
		GUI.DrawTexture(val3, (Texture)(object)FloatRangeSliderTex);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(num4 + 16f, ((Rect)(ref val2)).center.y - 8f, -16f, 16f);
		GUI.DrawTexture(val4, (Texture)(object)FloatRangeSliderTex);
		if (curDragEnd != 0 && ((int)Event.current.type == 1 || (int)Event.current.rawType == 0))
		{
			draggingId = 0;
			curDragEnd = RangeEnd.None;
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			Event.current.Use();
		}
		bool flag = false;
		if (Mouse.IsOver(rect) || draggingId == id)
		{
			if ((int)Event.current.type == 0 && Event.current.button == 0 && id != draggingId)
			{
				draggingId = id;
				float x = Event.current.mousePosition.x;
				if (x < ((Rect)(ref val3)).xMax)
				{
					curDragEnd = RangeEnd.Min;
				}
				else if (x > ((Rect)(ref val4)).xMin)
				{
					curDragEnd = RangeEnd.Max;
				}
				else
				{
					float num5 = Mathf.Abs(x - ((Rect)(ref val3)).xMax);
					float num6 = Mathf.Abs(x - (((Rect)(ref val4)).x - 16f));
					curDragEnd = ((num5 < num6) ? RangeEnd.Min : RangeEnd.Max);
				}
				flag = true;
				Event.current.Use();
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			if (flag || (curDragEnd != 0 && UnityGUIBugsFixer.MouseDrag()))
			{
				float num7 = (Event.current.mousePosition.x - ((Rect)(ref val)).x) / ((Rect)(ref val)).width * (max - min) + min;
				num7 = Mathf.Clamp(num7, min, max);
				if (curDragEnd == RangeEnd.Min)
				{
					if (num7 != range.min)
					{
						range.min = Mathf.Min(num7, max - gap);
						if (range.max < range.min + gap)
						{
							range.max = range.min + gap;
						}
						CheckPlayDragSliderSound();
					}
				}
				else if (curDragEnd == RangeEnd.Max && num7 != range.max)
				{
					range.max = Mathf.Max(num7, min + gap);
					if (range.min > range.max - gap)
					{
						range.min = range.max - gap;
					}
					CheckPlayDragSliderSound();
				}
				if (roundTo != 0f)
				{
					range.min = Mathf.Round(range.min / roundTo) * roundTo;
					range.max = Mathf.Round(range.max / roundTo) * roundTo;
				}
				if ((int)Event.current.type == 3)
				{
					Event.current.Use();
				}
			}
		}
		Text.Font = font;
	}

	public static void IntRange(Rect rect, int id, ref IntRange range, int min = 0, int max = 100, string labelKey = null, int minWidth = 0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Invalid comparison between Unknown and I4
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Invalid comparison between Unknown and I4
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 8f;
		((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - 8f;
		GUI.color = RangeControlTextColor;
		string text = range.min.ToStringCached() + " - " + range.max.ToStringCached();
		if (labelKey != null)
		{
			text = labelKey.Translate(text);
		}
		GameFont font = Text.Font;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)1;
		Rect rect2 = val;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - 2f;
		Label(rect2, text);
		Text.Anchor = (TextAnchor)0;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 8f - 1f, ((Rect)(ref val)).width, 2f);
		GUI.DrawTexture(val2, (Texture)(object)BaseContent.WhiteTex);
		float num = ((Rect)(ref val)).x + ((Rect)(ref val)).width * (float)(range.min - min) / (float)(max - min);
		float num2 = ((Rect)(ref val)).x + ((Rect)(ref val)).width * (float)(range.max - min) / (float)(max - min);
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num, ((Rect)(ref val)).yMax - 8f - 2f, num2 - num, 4f), (Texture)(object)BaseContent.WhiteTex);
		float num3 = num;
		float num4 = num2;
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(num3 - 16f, ((Rect)(ref val2)).center.y - 8f, 16f, 16f);
		GUI.DrawTexture(val3, (Texture)(object)FloatRangeSliderTex);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(num4 + 16f, ((Rect)(ref val2)).center.y - 8f, -16f, 16f);
		GUI.DrawTexture(val4, (Texture)(object)FloatRangeSliderTex);
		if (curDragEnd != 0 && ((int)Event.current.type == 1 || (int)Event.current.rawType == 0))
		{
			draggingId = 0;
			curDragEnd = RangeEnd.None;
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
		}
		bool flag = false;
		if (Mouse.IsOver(rect) || draggingId == id)
		{
			if ((int)Event.current.type == 0 && Event.current.button == 0 && id != draggingId)
			{
				draggingId = id;
				float x = Event.current.mousePosition.x;
				if (x < ((Rect)(ref val3)).xMax)
				{
					curDragEnd = RangeEnd.Min;
				}
				else if (x > ((Rect)(ref val4)).xMin)
				{
					curDragEnd = RangeEnd.Max;
				}
				else
				{
					float num5 = Mathf.Abs(x - ((Rect)(ref val3)).xMax);
					float num6 = Mathf.Abs(x - (((Rect)(ref val4)).x - 16f));
					curDragEnd = ((num5 < num6) ? RangeEnd.Min : RangeEnd.Max);
				}
				flag = true;
				Event.current.Use();
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			if (flag || (curDragEnd != 0 && UnityGUIBugsFixer.MouseDrag()))
			{
				int num7 = Mathf.RoundToInt(Mathf.Clamp((Event.current.mousePosition.x - ((Rect)(ref val)).x) / ((Rect)(ref val)).width * (float)(max - min) + (float)min, (float)min, (float)max));
				if (curDragEnd == RangeEnd.Min)
				{
					if (num7 != range.min)
					{
						range.min = num7;
						if (range.min > max - minWidth)
						{
							range.min = max - minWidth;
						}
						int num8 = Mathf.Max(min, range.min + minWidth);
						if (range.max < num8)
						{
							range.max = num8;
						}
						CheckPlayDragSliderSound();
					}
				}
				else if (curDragEnd == RangeEnd.Max && num7 != range.max)
				{
					range.max = num7;
					if (range.max < min + minWidth)
					{
						range.max = min + minWidth;
					}
					int num9 = Mathf.Min(max, range.max - minWidth);
					if (range.min > num9)
					{
						range.min = num9;
					}
					CheckPlayDragSliderSound();
				}
				if ((int)Event.current.type == 3)
				{
					Event.current.Use();
				}
			}
		}
		Text.Font = font;
	}

	private static void CheckPlayDragSliderSound()
	{
		if (Time.realtimeSinceStartup > lastDragSliderSoundTime + 0.075f)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
			lastDragSliderSoundTime = Time.realtimeSinceStartup;
		}
	}

	public static void QualityRange(Rect rect, int id, ref QualityRange range)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Invalid comparison between Unknown and I4
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Invalid comparison between Unknown and I4
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 8f;
		((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - 8f;
		GUI.color = RangeControlTextColor;
		string label = ((range == RimWorld.QualityRange.All) ? ((string)"AnyQuality".Translate()) : ((range.max != range.min) ? (range.min.GetLabel() + " - " + range.max.GetLabel()) : ((string)"OnlyQuality".Translate(range.min.GetLabel()))));
		GameFont font = Text.Font;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)1;
		Rect rect2 = val;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - 2f;
		Label(rect2, label);
		Text.Anchor = (TextAnchor)0;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 8f - 1f, ((Rect)(ref val)).width, 2f);
		GUI.DrawTexture(val2, (Texture)(object)BaseContent.WhiteTex);
		int qualityCount = QualityUtility.QualityCount;
		float num = ((Rect)(ref val)).x + ((Rect)(ref val)).width / (float)(qualityCount - 1) * (float)(int)range.min;
		float num2 = ((Rect)(ref val)).x + ((Rect)(ref val)).width / (float)(qualityCount - 1) * (float)(int)range.max;
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(num, ((Rect)(ref val)).yMax - 8f - 2f, num2 - num, 4f), (Texture)(object)BaseContent.WhiteTex);
		float num3 = num;
		float num4 = num2;
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(num3 - 16f, ((Rect)(ref val2)).center.y - 8f, 16f, 16f);
		GUI.DrawTexture(val3, (Texture)(object)FloatRangeSliderTex);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(num4 + 16f, ((Rect)(ref val2)).center.y - 8f, -16f, 16f);
		GUI.DrawTexture(val4, (Texture)(object)FloatRangeSliderTex);
		if (curDragEnd != 0 && ((int)Event.current.type == 1 || (int)Event.current.type == 0))
		{
			draggingId = 0;
			curDragEnd = RangeEnd.None;
			SoundDefOf.DragSlider.PlayOneShotOnCamera();
		}
		bool flag = false;
		if (Mouse.IsOver(rect) || id == draggingId)
		{
			if ((int)Event.current.type == 0 && Event.current.button == 0 && id != draggingId)
			{
				draggingId = id;
				float x = Event.current.mousePosition.x;
				if (x < ((Rect)(ref val3)).xMax)
				{
					curDragEnd = RangeEnd.Min;
				}
				else if (x > ((Rect)(ref val4)).xMin)
				{
					curDragEnd = RangeEnd.Max;
				}
				else
				{
					float num5 = Mathf.Abs(x - ((Rect)(ref val3)).xMax);
					float num6 = Mathf.Abs(x - (((Rect)(ref val4)).x - 16f));
					curDragEnd = ((num5 < num6) ? RangeEnd.Min : RangeEnd.Max);
				}
				flag = true;
				Event.current.Use();
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			if (flag || (curDragEnd != 0 && UnityGUIBugsFixer.MouseDrag()))
			{
				int num7 = Mathf.RoundToInt((Event.current.mousePosition.x - ((Rect)(ref val)).x) / ((Rect)(ref val)).width * (float)(qualityCount - 1));
				num7 = Mathf.Clamp(num7, 0, qualityCount - 1);
				if (curDragEnd == RangeEnd.Min)
				{
					if ((uint)range.min != (byte)num7)
					{
						range.min = (QualityCategory)num7;
						if ((int)range.max < (int)range.min)
						{
							range.max = range.min;
						}
						SoundDefOf.DragSlider.PlayOneShotOnCamera();
					}
				}
				else if (curDragEnd == RangeEnd.Max && (uint)range.max != (byte)num7)
				{
					range.max = (QualityCategory)num7;
					if ((int)range.min > (int)range.max)
					{
						range.min = range.max;
					}
					SoundDefOf.DragSlider.PlayOneShotOnCamera();
				}
				if ((int)Event.current.type == 3)
				{
					Event.current.Use();
				}
			}
		}
		Text.Font = font;
	}

	public static void FloatRangeWithTypeIn(Rect rect, int id, ref FloatRange fRange, float sliderMin = 0f, float sliderMax = 1f, ToStringStyle valueStyle = ToStringStyle.FloatTwo, string labelKey = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		((Rect)(ref rect2)).width = ((Rect)(ref rect)).width / 4f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(rect);
		((Rect)(ref val)).width = ((Rect)(ref rect)).width / 2f;
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 4f;
		((Rect)(ref val)).height = ((Rect)(ref rect)).height / 2f;
		((Rect)(ref val)).width = ((Rect)(ref val)).width - ((Rect)(ref rect)).height;
		Rect butRect = default(Rect);
		((Rect)(ref butRect))._002Ector(val);
		((Rect)(ref butRect)).x = ((Rect)(ref val)).xMax;
		((Rect)(ref butRect)).height = ((Rect)(ref rect)).height;
		((Rect)(ref butRect)).width = ((Rect)(ref rect)).height;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(rect);
		((Rect)(ref rect3)).x = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width * 0.75f;
		((Rect)(ref rect3)).width = ((Rect)(ref rect)).width / 4f;
		((Rect)(ref val)).y = ((Rect)(ref val)).y + 4f;
		((Rect)(ref val)).height = ((Rect)(ref val)).height + 4f;
		FloatRange(val, id, ref fRange, sliderMin, sliderMax, labelKey, valueStyle);
		if (ButtonImage(butRect, TexButton.RangeMatch))
		{
			fRange.max = fRange.min;
		}
		float.TryParse(TextField(rect2, fRange.min.ToString()), out fRange.min);
		float.TryParse(TextField(rect3, fRange.max.ToString()), out fRange.max);
	}

	public static Rect FillableBar(Rect rect, float fillPercent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return FillableBar(rect, fillPercent, BarFullTexHor);
	}

	public static Rect FillableBar(Rect rect, float fillPercent, Texture2D fillTex)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		bool doBorder = ((Rect)(ref rect)).height > 15f && ((Rect)(ref rect)).width > 20f;
		return FillableBar(rect, fillPercent, fillTex, DefaultBarBgTex, doBorder);
	}

	public static Rect FillableBar(Rect rect, float fillPercent, Texture2D fillTex, Texture2D bgTex, bool doBorder)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (doBorder)
		{
			GUI.DrawTexture(rect, (Texture)(object)BaseContent.BlackTex);
			rect = rect.ContractedBy(3f);
		}
		if ((Object)(object)bgTex != (Object)null)
		{
			GUI.DrawTexture(rect, (Texture)(object)bgTex);
		}
		Rect result = rect;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width * fillPercent;
		GUI.DrawTexture(rect, (Texture)(object)fillTex);
		return result;
	}

	public static void FillableBarLabeled(Rect rect, float fillPercent, int labelWidth, string label)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (fillPercent < 0f)
		{
			fillPercent = 0f;
		}
		if (fillPercent > 1f)
		{
			fillPercent = 1f;
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = labelWidth;
		Label(rect2, label);
		Rect rect3 = rect;
		((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x + (float)labelWidth;
		((Rect)(ref rect3)).width = ((Rect)(ref rect3)).width - (float)labelWidth;
		FillableBar(rect3, fillPercent);
	}

	public static void FillableBarChangeArrows(Rect barRect, float changeRate)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		int changeRate2 = (int)(changeRate * FillableBarChangeRateDisplayRatio);
		FillableBarChangeArrows(barRect, changeRate2);
	}

	public static void FillableBarChangeArrows(Rect barRect, int changeRate)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (changeRate != 0)
		{
			if (changeRate > MaxFillableBarChangeRate)
			{
				changeRate = MaxFillableBarChangeRate;
			}
			if (changeRate < -MaxFillableBarChangeRate)
			{
				changeRate = -MaxFillableBarChangeRate;
			}
			float num = ((Rect)(ref barRect)).height;
			if (num > 16f)
			{
				num = 16f;
			}
			int num2 = Mathf.Abs(changeRate);
			float num3 = ((Rect)(ref barRect)).y + ((Rect)(ref barRect)).height / 2f - num / 2f;
			float num4;
			float num5;
			Texture2D val;
			if (changeRate > 0)
			{
				num4 = ((Rect)(ref barRect)).x + ((Rect)(ref barRect)).width + 2f;
				num5 = 8f;
				val = FillArrowTexRight;
			}
			else
			{
				num4 = ((Rect)(ref barRect)).x - 8f - 2f;
				num5 = -8f;
				val = FillArrowTexLeft;
			}
			for (int i = 0; i < num2; i++)
			{
				GUI.DrawTexture(new Rect(num4, num3, 8f, num), (Texture)(object)val);
				num4 += num5;
			}
		}
	}

	public static void DrawWindowBackground(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = WindowBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = WindowBGBorderColor;
		DrawBox(rect);
		GUI.color = Color.white;
	}

	public static void DrawWindowBackground(Rect rect, Color colorFactor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		GUI.color = WindowBGFillColor * colorFactor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = WindowBGBorderColor * colorFactor;
		DrawBox(rect);
		GUI.color = color;
	}

	public static void DrawMenuSection(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = MenuSectionBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = MenuSectionBGBorderColor;
		DrawBox(rect);
		GUI.color = Color.white;
	}

	public static void DrawWindowBackgroundTutor(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = TutorWindowBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = TutorWindowBGBorderColor;
		DrawBox(rect);
		GUI.color = Color.white;
	}

	public static void DrawOptionUnselected(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = OptionUnselectedBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = OptionUnselectedBGBorderColor;
		DrawBox(rect);
		GUI.color = Color.white;
	}

	public static void DrawOptionSelected(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = OptionSelectedBGFillColor;
		GUI.DrawTexture(rect, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = OptionSelectedBGBorderColor;
		DrawBox(rect.ExpandedBy(3f), 3);
		GUI.color = Color.white;
	}

	public static void DrawOptionBackground(Rect rect, bool selected)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (selected)
		{
			DrawOptionSelected(rect);
		}
		else
		{
			DrawOptionUnselected(rect);
		}
		DrawHighlightIfMouseover(rect);
	}

	public static void DrawShadowAround(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect.ContractedBy(-9f);
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 2f;
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 2f;
		DrawAtlas(rect2, ShadowAtlas);
	}

	public static void DrawAtlas(Rect rect, Texture2D atlas)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DrawAtlas(rect, atlas, drawTop: true);
	}

	public static void DrawAtlas(Rect rect, Texture2D atlas, bool drawTop)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 7)
		{
			((Rect)(ref rect)).x = Mathf.Round(((Rect)(ref rect)).x);
			((Rect)(ref rect)).y = Mathf.Round(((Rect)(ref rect)).y);
			((Rect)(ref rect)).width = Mathf.Round(((Rect)(ref rect)).width);
			((Rect)(ref rect)).height = Mathf.Round(((Rect)(ref rect)).height);
			rect = UIScaling.AdjustRectToUIScaling(rect);
			float a = (float)((Texture)atlas).width * 0.25f;
			a = UIScaling.AdjustCoordToUIScalingCeil(GenMath.Min(a, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width / 2f));
			Rect drawRect = default(Rect);
			if (drawTop)
			{
				((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, a, a);
				DrawTexturePart(drawRect, AtlasUV_TopLeft, atlas);
				((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - a, ((Rect)(ref rect)).y, a, a);
				DrawTexturePart(drawRect, AtlasUV_TopRight, atlas);
			}
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - a, a, a);
			DrawTexturePart(drawRect, AtlasUV_BottomLeft, atlas);
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - a, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - a, a, a);
			DrawTexturePart(drawRect, AtlasUV_BottomRight, atlas);
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + a, ((Rect)(ref rect)).y + a, ((Rect)(ref rect)).width - a * 2f, ((Rect)(ref rect)).height - a * 2f);
			if (!drawTop)
			{
				((Rect)(ref drawRect)).height = ((Rect)(ref drawRect)).height + a;
				((Rect)(ref drawRect)).y = ((Rect)(ref drawRect)).y - a;
			}
			DrawTexturePart(drawRect, AtlasUV_Center, atlas);
			if (drawTop)
			{
				((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + a, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - a * 2f, a);
				DrawTexturePart(drawRect, AtlasUV_Top, atlas);
			}
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + a, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - a, ((Rect)(ref rect)).width - a * 2f, a);
			DrawTexturePart(drawRect, AtlasUV_Bottom, atlas);
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + a, a, ((Rect)(ref rect)).height - a * 2f);
			if (!drawTop)
			{
				((Rect)(ref drawRect)).height = ((Rect)(ref drawRect)).height + a;
				((Rect)(ref drawRect)).y = ((Rect)(ref drawRect)).y - a;
			}
			DrawTexturePart(drawRect, AtlasUV_Left, atlas);
			((Rect)(ref drawRect))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - a, ((Rect)(ref rect)).y + a, a, ((Rect)(ref rect)).height - a * 2f);
			if (!drawTop)
			{
				((Rect)(ref drawRect)).height = ((Rect)(ref drawRect)).height + a;
				((Rect)(ref drawRect)).y = ((Rect)(ref drawRect)).y - a;
			}
			DrawTexturePart(drawRect, AtlasUV_Right, atlas);
		}
	}

	public static void DrawAtlasWithMaterial(Rect rect, Texture2D atlas, Material mat, bool drawTop = true)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).x = Mathf.Round(((Rect)(ref rect)).x);
		((Rect)(ref rect)).y = Mathf.Round(((Rect)(ref rect)).y);
		((Rect)(ref rect)).width = Mathf.Round(((Rect)(ref rect)).width);
		((Rect)(ref rect)).height = Mathf.Round(((Rect)(ref rect)).height);
		rect = UIScaling.AdjustRectToUIScaling(rect);
		float a = (float)((Texture)atlas).width * 0.25f;
		a = UIScaling.AdjustCoordToUIScalingCeil(GenMath.Min(a, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width / 2f));
		BeginGroup(rect);
		Rect rect2 = default(Rect);
		if (drawTop)
		{
			((Rect)(ref rect2))._002Ector(0f, 0f, a, a);
			GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_TopLeft);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width - a, 0f, a, a);
			GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_TopRight);
		}
		((Rect)(ref rect2))._002Ector(0f, ((Rect)(ref rect)).height - a, a, a);
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_BottomLeft);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width - a, ((Rect)(ref rect)).height - a, a, a);
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_BottomRight);
		((Rect)(ref rect2))._002Ector(a, a, ((Rect)(ref rect)).width - a * 2f, ((Rect)(ref rect)).height - a * 2f);
		if (!drawTop)
		{
			((Rect)(ref rect2)).height = ((Rect)(ref rect2)).height + a;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - a;
		}
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_Center);
		if (drawTop)
		{
			((Rect)(ref rect2))._002Ector(a, 0f, ((Rect)(ref rect)).width - a * 2f, a);
			GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_Top);
		}
		((Rect)(ref rect2))._002Ector(a, ((Rect)(ref rect)).height - a, ((Rect)(ref rect)).width - a * 2f, a);
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_Bottom);
		((Rect)(ref rect2))._002Ector(0f, a, a, ((Rect)(ref rect)).height - a * 2f);
		if (!drawTop)
		{
			((Rect)(ref rect2)).height = ((Rect)(ref rect2)).height + a;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - a;
		}
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_Left);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width - a, a, a, ((Rect)(ref rect)).height - a * 2f);
		if (!drawTop)
		{
			((Rect)(ref rect2)).height = ((Rect)(ref rect2)).height + a;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - a;
		}
		GenUI.DrawTexturePartWithMaterial(rect2, (Texture)(object)atlas, mat, AtlasUV_Right);
		EndGroup();
	}

	public static Rect ToUVRect(this Rect r, Vector2 texSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref r)).x / texSize.x, ((Rect)(ref r)).y / texSize.y, ((Rect)(ref r)).width / texSize.x, ((Rect)(ref r)).height / texSize.y);
	}

	public static void DrawTexturePart(Rect drawRect, Rect uvRect, Texture2D tex)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref uvRect)).y = 1f - ((Rect)(ref uvRect)).y - ((Rect)(ref uvRect)).height;
		GUI.DrawTextureWithTexCoords(drawRect, (Texture)(object)tex, uvRect);
	}

	public static void ScrollHorizontal(Rect outRect, ref Vector2 scrollPosition, Rect viewRect, float ScrollWheelSpeed = 20f)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 6 && Mouse.IsOver(outRect))
		{
			scrollPosition.x += Event.current.delta.y * ScrollWheelSpeed;
			float num = 0f;
			float num2 = ((Rect)(ref viewRect)).width - ((Rect)(ref outRect)).width + 16f;
			if (scrollPosition.x < num)
			{
				scrollPosition.x = num;
			}
			if (scrollPosition.x > num2)
			{
				scrollPosition.x = num2;
			}
			Event.current.Use();
		}
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

	public static void AdjustRectsForScrollView(Rect parentRect, ref Rect outRect, ref Rect viewRect)
	{
		if (((Rect)(ref viewRect)).height >= ((Rect)(ref outRect)).height)
		{
			((Rect)(ref viewRect)).width = ((Rect)(ref viewRect)).width - 20f;
			((Rect)(ref outRect)).xMax = ((Rect)(ref outRect)).xMax - 4f;
			((Rect)(ref outRect)).yMin = Mathf.Max(((Rect)(ref parentRect)).yMin + 6f, ((Rect)(ref outRect)).yMin);
			((Rect)(ref outRect)).yMax = Mathf.Min(((Rect)(ref parentRect)).yMax - 6f, ((Rect)(ref outRect)).yMax);
		}
	}

	public static void EndScrollView()
	{
		mouseOverScrollViewStack.Pop();
		GUI.EndScrollView();
	}

	public static void EnsureMousePositionStackEmpty()
	{
		if (mouseOverScrollViewStack.Count > 0)
		{
			Log.Error("Mouse position stack is not empty. There were more calls to BeginScrollView than EndScrollView. Fixing.");
			mouseOverScrollViewStack.Clear();
		}
	}

	public static void ColorSelectorIcon(Rect rect, Texture icon, Color color, bool drawColor = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)icon != (Object)null)
		{
			GUI.color = color;
			GUI.DrawTexture(rect, icon);
			GUI.color = Color.white;
		}
		else if (drawColor)
		{
			DrawBoxSolid(rect, color);
		}
	}

	public static bool ColorBox(Rect rect, ref Color color, Color boxColor, int colorSize = 22, int colorPadding = 2, Action<Color, Rect> extraOnGUI = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		DrawLightHighlight(rect);
		DrawHighlightIfMouseover(rect);
		if (color.IndistinguishableFrom(boxColor))
		{
			DrawBox(rect);
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + (float)colorPadding, ((Rect)(ref rect)).y + (float)colorPadding, (float)colorSize, (float)colorSize);
		DrawBoxSolid(val, boxColor);
		extraOnGUI?.Invoke(boxColor, val);
		bool result = false;
		if (ButtonInvisible(rect))
		{
			result = true;
			color = boxColor;
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
		return result;
	}

	public static bool ColorSelector(Rect rect, ref Color color, List<Color> colors, out float height, Texture icon = null, int colorSize = 22, int colorPadding = 2, Action<Color, Rect> extraOnGUI = null)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		height = 0f;
		bool result = false;
		int num = colorSize + colorPadding * 2;
		float num2 = (((Object)(object)icon != (Object)null) ? ((float)(colorSize * 4) + 10f) : 0f);
		int num3 = Mathf.FloorToInt((((Rect)(ref rect)).width - num2 + (float)colorPadding) / (float)(num + colorPadding));
		int num4 = Mathf.CeilToInt((float)colors.Count / (float)num3);
		BeginGroup(rect);
		ColorSelectorIcon(new Rect(5f, 5f, (float)(colorSize * 4), (float)(colorSize * 4)), icon, color);
		Rect rect2 = default(Rect);
		for (int i = 0; i < colors.Count; i++)
		{
			int num5 = i / num3;
			int num6 = i % num3;
			float num7 = (((Object)(object)icon != (Object)null) ? ((num2 - (float)(num * num4) - (float)colorPadding) / 2f) : 0f);
			((Rect)(ref rect2))._002Ector(num2 + (float)(num6 * num) + (float)(num6 * colorPadding), num7 + (float)(num5 * num) + (float)(num5 * colorPadding), (float)num, (float)num);
			if (ColorBox(rect2, ref color, colors[i], colorSize, colorPadding, extraOnGUI))
			{
				result = true;
			}
			height = Mathf.Max(height, ((Rect)(ref rect2)).yMax);
		}
		EndGroup();
		return result;
	}

	private static void DrawColorSelectionCircle(Rect hsvColorWheelRect, Vector2Int center, Color color)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)Mathf.Round(((Rect)(ref hsvColorWheelRect)).width * 0.125f);
		GUI.DrawTexture(new Rect((float)(((Vector2Int)(ref center)).x - num / 2), (float)(((Vector2Int)(ref center)).y - num / 2), (float)num, (float)num), (Texture)(object)ColorSelectionCircle, (ScaleMode)2, true, 1f, color, 0f, 0f);
	}

	private static bool ClickedInsideRect(Rect rect)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0)
		{
			return ((Rect)(ref rect)).Contains(Event.current.mousePosition);
		}
		return false;
	}

	public static void HSVColorWheel(Rect rect, ref Color color, ref bool currentlyDragging, float? colorValueOverride = null, string controlName = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Invalid comparison between Unknown and I4
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (((Rect)(ref rect)).width != ((Rect)(ref rect)).height)
		{
			throw new ArgumentException("HSV color wheel must be drawn in a square rect.");
		}
		float num2 = default(float);
		float num3 = default(float);
		float num = default(float);
		Color.RGBToHSV(color, ref num, ref num2, ref num3);
		float num4 = colorValueOverride ?? num3;
		GUI.DrawTexture(rect, (Texture)(object)HSVColorWheelTex, (ScaleMode)2, true, 1f, Color.HSVToRGB(0f, 0f, num4), 0f, 0f);
		num = (num + 0.25f) * 2f * MathF.PI;
		Vector2 val = new Vector2(Mathf.Cos(num), 0f - Mathf.Sin(num)) * num2 * ((Rect)(ref rect)).width / 2f;
		DrawColorSelectionCircle(rect, Vector2Int.RoundToInt(val + ((Rect)(ref rect)).center), (num4 > 0.5f) ? Color.black : Color.white);
		if (!currentlyDragging)
		{
			MouseoverSounds.DoRegion(rect);
		}
		if (Event.current.isMouse && Event.current.button == 0)
		{
			if (currentlyDragging && (int)Event.current.type == 1)
			{
				currentlyDragging = false;
			}
			else if (ClickedInsideRect(rect) | currentlyDragging)
			{
				GUI.FocusControl(controlName);
				currentlyDragging = true;
				Vector2 val2 = (Event.current.mousePosition - ((Rect)(ref rect)).center) / (((Rect)(ref rect)).size / 2f);
				float num5 = Mathf.Atan2(0f - val2.y, val2.x) / (MathF.PI * 2f);
				num5 += 1.75f;
				num5 %= 1f;
				float num6 = Mathf.Clamp01(((Vector2)(ref val2)).magnitude);
				color = Color.HSVToRGB(num5, num6, num4);
				Event.current.Use();
			}
		}
	}

	public static void ColorTemperatureBar(Rect rect, ref Color color, ref bool dragging, float? colorValueOverride = null)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Invalid comparison between Unknown and I4
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Invalid comparison between Unknown and I4
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		float num = colorValueOverride ?? Mathf.Max(new float[3] { color.r, color.g, color.b });
		float? num2 = color.ColorTemperature();
		string label = num2?.ToString("0.K") ?? "";
		RectDivider rectDivider = new RectDivider(rect, 661493905, (Vector2?)new Vector2(17f, 0f));
		using (new TextBlock((TextAnchor)3))
		{
			string text = "ColorTemperature".Translate().CapitalizeFirst();
			Label(rectDivider.NewCol(Text.CalcSize(text).x), text);
			Label(rectDivider.NewCol(Text.CalcSize("XXXXXK").x), label);
		}
		if (!dragging)
		{
			TooltipHandler.TipRegion(rect, "ColorTemperatureTooltip".Translate());
			MouseoverSounds.DoRegion(rect);
		}
		Rect rect2;
		if (Event.current.button == 0)
		{
			if (dragging && (int)Event.current.type == 1)
			{
				dragging = false;
			}
			else if (ClickedInsideRect(rectDivider) || (dragging && UnityGUIBugsFixer.MouseDrag()))
			{
				dragging = true;
				if ((int)Event.current.type == 3)
				{
					Event.current.Use();
				}
				float x = Event.current.mousePosition.x;
				rect2 = rectDivider.Rect;
				float num3 = x - ((Rect)(ref rect2)).xMin;
				rect2 = rectDivider.Rect;
				float fraction = Mathf.Clamp01(num3 / ((Rect)(ref rect2)).width);
				num2 = GenMath.ExponentialWarpInterpolation(1000f, 40000f, fraction, new Vector2(0.5f, 6600f));
				color = GenColor.FromColorTemperature(num2.Value);
				color *= num;
			}
		}
		rectDivider.NewRow(6f);
		rectDivider.NewRow(6f, VerticalJustification.Bottom);
		GUI.DrawTexture((Rect)rectDivider, (Texture)(object)ColorTemperatureExp, (ScaleMode)0, true, 1f, Color.HSVToRGB(0f, 0f, num), 0f, 0f);
		if (num2.HasValue)
		{
			rect2 = rectDivider.Rect;
			float num4 = ((Rect)(ref rect2)).width * GenMath.InverseExponentialWarpInterpolation(1000f, 40000f, num2.Value, new Vector2(0.5f, 6600f));
			rect2 = rectDivider.Rect;
			float num5 = ((Rect)(ref rect2)).x + num4 - 6f;
			rect2 = rectDivider.Rect;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(num5, ((Rect)(ref rect2)).y - 6f, 12f, 12f);
			rect2 = rectDivider.Rect;
			float num6 = ((Rect)(ref rect2)).x + num4 - 6f;
			rect2 = rectDivider.Rect;
			Rect val2 = new Rect(num6, ((Rect)(ref rect2)).yMax - 6f, 12f, 12f);
			GUI.DrawTextureWithTexCoords(val, (Texture)(object)SelectionArrow, new Rect(0f, 1f, 1f, -1f), true);
			GUI.DrawTextureWithTexCoords(val2, (Texture)(object)SelectionArrow, new Rect(0f, 0f, 1f, 1f), true);
		}
	}

	private static int ToIntegerRange(float fraction, int min, int max)
	{
		return Mathf.Clamp(Mathf.RoundToInt(fraction * (float)max), min, max);
	}

	public static bool ColorTextfields(ref RectAggregator aggregator, ref Color color, ref string[] buffers, ref Color colorBuffer, string previousFocusedControlName, string controlName = null, ColorComponents editable = ColorComponents.All, ColorComponents visible = ColorComponents.All)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		if (visible == ColorComponents.None)
		{
			return false;
		}
		if ((~visible & editable) != 0)
		{
			throw new ArgumentException($"Cannot have editable but invisible components {~visible & editable}.");
		}
		string text = controlName;
		if (text == null)
		{
			Rect rect = aggregator.Rect;
			object arg = ((Rect)(ref rect)).x;
			rect = aggregator.Rect;
			text = $"ColorTextfields{arg}{((Rect)(ref rect)).y}";
		}
		controlName = text;
		bool flag = previousFocusedControlName?.StartsWith(controlName) ?? false;
		bool flag2 = GUI.GetNameOfFocusedControl().StartsWith(controlName);
		using (new TextBlock((TextAnchor)3))
		{
			float num = 30f;
			float num2 = 0f;
			for (int i = 0; i < colorComponentLabels.Length; i++)
			{
				tmpTranslatedColorComponentLabels[i] = colorComponentLabels[i].Translate().CapitalizeFirst();
				num = Mathf.Max(num, tmpTranslatedColorComponentLabels[i].GetHeightCached());
				num2 = Mathf.Max(num2, tmpTranslatedColorComponentLabels[i].GetWidthCached());
			}
			float fraction = default(float);
			float fraction2 = default(float);
			float fraction3 = default(float);
			Color.RGBToHSV(colorBuffer, ref fraction, ref fraction2, ref fraction3);
			intColorComponents[0] = ToIntegerRange(colorBuffer.r, 0, maxColorComponentValues[0]);
			intColorComponents[1] = ToIntegerRange(colorBuffer.g, 0, maxColorComponentValues[1]);
			intColorComponents[2] = ToIntegerRange(colorBuffer.b, 0, maxColorComponentValues[2]);
			intColorComponents[3] = ToIntegerRange(fraction, 0, maxColorComponentValues[3]);
			intColorComponents[4] = ToIntegerRange(fraction2, 0, maxColorComponentValues[4]);
			intColorComponents[5] = ToIntegerRange(fraction3, 0, maxColorComponentValues[5]);
			for (int j = 0; j <= 5; j++)
			{
				ColorComponents colorComponents = (ColorComponents)(1 << j);
				if ((visible & colorComponents) == 0)
				{
					continue;
				}
				RectDivider rectDivider = aggregator.NewRow(num);
				Label(rectDivider.NewCol(num2), tmpTranslatedColorComponentLabels[j]);
				if ((editable & colorComponents) == 0)
				{
					Label(rectDivider, intColorComponents[j].ToString());
					continue;
				}
				string text2 = intColorComponents[j].ToString();
				string text3 = DelayedTextField(rectDivider, text2, ref buffers[j], previousFocusedControlName, $"{controlName}_{j}");
				if (text2 != text3 && int.TryParse(text3, out var result))
				{
					intColorComponents[j] = result;
					if (j < 3)
					{
						colorBuffer = new ColorInt(intColorComponents[0], intColorComponents[1], intColorComponents[2]).ToColor;
					}
					else
					{
						colorBuffer = Color.HSVToRGB((float)intColorComponents[3] / 360f, (float)intColorComponents[4] / 100f, (float)intColorComponents[5] / 100f);
					}
				}
			}
		}
		if (flag)
		{
			if (!flag2)
			{
				color = colorBuffer;
				return true;
			}
		}
		else
		{
			colorBuffer = color;
		}
		return false;
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

	public static void DrawHighlight(Rect rect, float opacity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white.ToTransparent(opacity);
		GUI.DrawTexture(rect, (Texture)(object)TexUI.HighlightTex);
		GUI.color = Color.white;
	}

	public static void DrawLightHighlight(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)LightHighlight);
	}

	public static void DrawStrongHighlight(Rect rect, Color? color = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Color color2 = GUI.color;
		GUI.color = color.GetValueOrDefault(HighlightStrongBgColor);
		DrawAtlas(rect, TexUI.RectHighlight);
		GUI.color = color2;
	}

	public static void DrawTextHighlight(Rect rect, float expandBy = 4f, Color? color = null)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y - expandBy;
		((Rect)(ref rect)).height = ((Rect)(ref rect)).height + expandBy * 2f;
		Color color2 = GUI.color;
		GUI.color = color.GetValueOrDefault(HighlightTextBgColor);
		DrawAtlas(rect, TexUI.RectHighlight);
		GUI.color = color2;
	}

	public static void DrawTitleBG(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)TexUI.TitleBGTex);
	}

	public static bool InfoCardButton(float x, float y, Thing thing)
	{
		if (thing is IConstructible constructible)
		{
			if (thing.def.entityDefToBuild is ThingDef thingDef)
			{
				return InfoCardButton(x, y, thingDef, constructible.EntityToBuildStuff());
			}
			return InfoCardButton(x, y, thing.def.entityDefToBuild);
		}
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(thing));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(float x, float y, Def def)
	{
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(def));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(Rect rect, Def def)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (InfoCardButtonWorker(rect))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(def));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(float x, float y, Def def, Precept_ThingStyle precept)
	{
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(def, precept));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(float x, float y, ThingDef thingDef, ThingDef stuffDef)
	{
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(thingDef, stuffDef));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(float x, float y, WorldObject worldObject)
	{
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(worldObject));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(Rect rect, Hediff hediff)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (InfoCardButtonWorker(rect))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(hediff));
			return true;
		}
		return false;
	}

	public static bool InfoCardButton(float x, float y, Faction faction)
	{
		if (InfoCardButtonWorker(x, y))
		{
			Find.WindowStack.Add(new Dialog_InfoCard(faction));
			return true;
		}
		return false;
	}

	public static bool InfoCardButtonCentered(Rect rect, Thing thing)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return InfoCardButton(((Rect)(ref rect)).center.x - 12f, ((Rect)(ref rect)).center.y - 12f, thing);
	}

	public static bool InfoCardButtonCentered(Rect rect, Faction faction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return InfoCardButton(((Rect)(ref rect)).center.x - 12f, ((Rect)(ref rect)).center.y - 12f, faction);
	}

	private static bool InfoCardButtonWorker(float x, float y)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return InfoCardButtonWorker(new Rect(x, y, 24f, 24f));
	}

	private static bool InfoCardButtonWorker(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(rect);
		TooltipHandler.TipRegionByKey(rect, "DefInfoTip");
		bool result = ButtonImage(rect, TexButton.Info, GUI.color);
		UIHighlighter.HighlightOpportunity(rect, "InfoCard");
		return result;
	}

	public static void DrawTextureFitted(Rect outerRect, Texture tex, float scale, float alpha = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		DrawTextureFitted(outerRect, tex, scale, new Vector2((float)tex.width, (float)tex.height), new Rect(0f, 0f, 1f, 1f), 0f, null, alpha);
	}

	public static void DrawTextureFitted(Rect outerRect, Texture tex, float scale, Material mat, float alpha = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		DrawTextureFitted(outerRect, tex, scale, new Vector2((float)tex.width, (float)tex.height), new Rect(0f, 0f, 1f, 1f), 0f, mat, alpha);
	}

	public static void DrawTextureFitted(Rect outerRect, Texture tex, float scale, Vector2 texProportions, Rect texCoords, float angle = 0f, Material mat = null, float alpha = 1f)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 7)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(0f, 0f, texProportions.x, texProportions.y);
			float num = ((!(((Rect)(ref rect)).width / ((Rect)(ref rect)).height < ((Rect)(ref outerRect)).width / ((Rect)(ref outerRect)).height)) ? (((Rect)(ref outerRect)).width / ((Rect)(ref rect)).width) : (((Rect)(ref outerRect)).height / ((Rect)(ref rect)).height));
			num *= scale;
			((Rect)(ref rect)).width = ((Rect)(ref rect)).width * num;
			((Rect)(ref rect)).height = ((Rect)(ref rect)).height * num;
			((Rect)(ref rect)).x = ((Rect)(ref outerRect)).x + ((Rect)(ref outerRect)).width / 2f - ((Rect)(ref rect)).width / 2f;
			((Rect)(ref rect)).y = ((Rect)(ref outerRect)).y + ((Rect)(ref outerRect)).height / 2f - ((Rect)(ref rect)).height / 2f;
			Matrix4x4 matrix = Matrix4x4.identity;
			if (angle != 0f)
			{
				matrix = GUI.matrix;
				UI.RotateAroundPivot(angle, ((Rect)(ref rect)).center);
			}
			Color color = Color.white;
			if (!Mathf.Approximately(alpha, 1f))
			{
				Color color2 = (color = GUI.color);
				color2.a *= alpha;
				GUI.color = color2;
			}
			GenUI.DrawTextureWithMaterial(rect, tex, mat, texCoords);
			if (angle != 0f)
			{
				GUI.matrix = matrix;
			}
			if (!Mathf.Approximately(alpha, 1f))
			{
				GUI.color = color;
			}
		}
	}

	public static void DrawTextureRotated(Vector2 center, Texture tex, float angle, float scale = 1f, Material material = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)tex.width * scale;
		float num2 = (float)tex.height * scale;
		DrawTextureRotated(new Rect(center.x - num / 2f, center.y - num2 / 2f, num, num2), tex, angle, material);
	}

	public static void DrawTextureRotated(Rect rect, Texture tex, float angle, Material material = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7)
		{
			return;
		}
		if (angle == 0f)
		{
			if ((Object)(object)material == (Object)null)
			{
				GUI.DrawTexture(rect, tex);
			}
			else
			{
				GenUI.DrawTextureWithMaterial(rect, tex, material);
			}
			return;
		}
		Matrix4x4 matrix = GUI.matrix;
		UI.RotateAroundPivot(angle, ((Rect)(ref rect)).center);
		if ((Object)(object)material == (Object)null)
		{
			GUI.DrawTexture(rect, tex);
		}
		else
		{
			GenUI.DrawTextureWithMaterial(rect, tex, material);
		}
		GUI.matrix = matrix;
	}

	public static void NoneLabel(float y, float width, string customLabel = null)
	{
		NoneLabel(ref y, width, customLabel);
	}

	public static void NoneLabel(ref float curY, float width, string customLabel = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.gray;
		Text.Anchor = (TextAnchor)1;
		Label(new Rect(0f, curY, width, 30f), customLabel ?? ((string)"NoneBrackets".Translate()));
		Text.Anchor = (TextAnchor)0;
		curY += 25f;
		GUI.color = Color.white;
	}

	public static void NoneLabelCenteredVertically(Rect rect, string customLabel = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.gray;
		Text.Anchor = (TextAnchor)4;
		Label(rect, customLabel ?? ((string)"NoneBrackets".Translate()));
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
	}

	public static void DraggableBar(Rect barRect, Texture2D barTexture, Texture2D barHighlightTexture, Texture2D emptyBarTex, Texture2D dragBarTex, ref bool draggingBar, float barValue, ref float targetValue, IEnumerable<float> bandPercentages = null, int increments = 20, float min = 0f, float max = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Invalid comparison between Unknown and I4
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Mouse.IsOver(barRect);
		FillableBar(barRect, Mathf.Min(barValue, 1f), flag ? barHighlightTexture : barTexture, emptyBarTex, doBorder: true);
		if (bandPercentages != null)
		{
			foreach (float bandPercentage in bandPercentages)
			{
				DrawDraggableBarThreshold(barRect, bandPercentage, barValue);
			}
		}
		float num = Mathf.Clamp(Mathf.Round((Event.current.mousePosition.x - ((Rect)(ref barRect)).x) / ((Rect)(ref barRect)).width * (float)increments) / (float)increments, min, max);
		Event current2 = Event.current;
		if ((int)current2.type == 0 && current2.button == 0 && flag)
		{
			targetValue = num;
			draggingBar = true;
			current2.Use();
		}
		if ((UnityGUIBugsFixer.MouseDrag() & draggingBar) && flag)
		{
			if (Math.Abs(num - targetValue) > float.Epsilon)
			{
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			targetValue = num;
			if ((int)Event.current.type == 3)
			{
				current2.Use();
			}
		}
		if (((int)current2.type == 1 && current2.button == 0) & draggingBar)
		{
			draggingBar = false;
			current2.Use();
		}
		DrawDraggableBarTarget(barRect, draggingBar ? num : targetValue, dragBarTex);
		GUI.color = Color.white;
	}

	private static void DrawDraggableBarThreshold(Rect rect, float percent, float curValue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + 3f + (((Rect)(ref rect)).width - 8f) * percent;
		((Rect)(ref val)).y = ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - 9f;
		((Rect)(ref val)).width = 2f;
		((Rect)(ref val)).height = 6f;
		Rect val2 = val;
		if (curValue < percent)
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.GreyTex);
		}
		else
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.BlackTex);
		}
	}

	private static void DrawDraggableBarTarget(Rect rect, float percent, Texture2D targetTex)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Round((((Rect)(ref rect)).width - 8f) * percent);
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + 3f + num;
		((Rect)(ref val)).y = ((Rect)(ref rect)).y;
		((Rect)(ref val)).width = 2f;
		((Rect)(ref val)).height = ((Rect)(ref rect)).height;
		GUI.DrawTexture(val, (Texture)(object)targetTex);
		float num2 = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect)).x + 2f + num);
		float xMax = UIScaling.AdjustCoordToUIScalingCeil(num2 + 4f);
		val = default(Rect);
		((Rect)(ref val)).y = ((Rect)(ref rect)).y - 3f;
		((Rect)(ref val)).height = 5f;
		((Rect)(ref val)).xMin = num2;
		((Rect)(ref val)).xMax = xMax;
		Rect val2 = val;
		GUI.DrawTexture(val2, (Texture)(object)targetTex);
		Rect val3 = val2;
		((Rect)(ref val3)).y = ((Rect)(ref rect)).yMax - 2f;
		GUI.DrawTexture(val3, (Texture)(object)targetTex);
	}

	public static void Dropdown<Target, Payload>(Rect rect, Target target, Func<Target, Payload> getPayload, Func<Target, IEnumerable<DropdownMenuElement<Payload>>> menuGenerator, string buttonLabel = null, Texture2D buttonIcon = null, string dragLabel = null, Texture2D dragIcon = null, Action dropdownOpened = null, bool paintable = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Dropdown(rect, target, Color.white, getPayload, menuGenerator, buttonLabel, buttonIcon, dragLabel, dragIcon, dropdownOpened, paintable);
	}

	public static void Dropdown<Target, Payload>(Rect rect, Target target, Color iconColor, Func<Target, Payload> getPayload, Func<Target, IEnumerable<DropdownMenuElement<Payload>>> menuGenerator, string buttonLabel = null, Texture2D buttonIcon = null, string dragLabel = null, Texture2D dragIcon = null, Action dropdownOpened = null, bool paintable = false, float? contractButtonIcon = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(rect);
		DraggableResult draggableResult;
		if ((Object)(object)buttonIcon != (Object)null)
		{
			DrawHighlightIfMouseover(rect);
			GUI.color = iconColor;
			Rect val = rect;
			if (contractButtonIcon.HasValue)
			{
				val = val.ContractedBy(contractButtonIcon.Value);
			}
			DrawTextureFitted(val, (Texture)(object)buttonIcon, 1f);
			GUI.color = Color.white;
			draggableResult = ButtonInvisibleDraggable(rect);
		}
		else
		{
			draggableResult = ButtonTextDraggable(rect, buttonLabel);
		}
		if (draggableResult == DraggableResult.Pressed)
		{
			List<FloatMenuOption> options = (from opt in menuGenerator(target)
				select opt.option).ToList();
			Find.WindowStack.Add(new FloatMenu(options));
			dropdownOpened?.Invoke();
		}
		else if (paintable && draggableResult == DraggableResult.Dragged)
		{
			dropdownPainting = true;
			dropdownPainting_Payload = getPayload(target);
			dropdownPainting_Type = typeof(Payload);
			dropdownPainting_Text = ((dragLabel != null) ? dragLabel : buttonLabel);
			dropdownPainting_Icon = (((Object)(object)dragIcon != (Object)null) ? dragIcon : buttonIcon);
		}
		else
		{
			if (!paintable || !dropdownPainting || !Mouse.IsOver(rect) || !(dropdownPainting_Type == typeof(Payload)))
			{
				return;
			}
			FloatMenuOption floatMenuOption = (from opt in menuGenerator(target)
				where object.Equals(opt.payload, dropdownPainting_Payload)
				select opt.option).FirstOrDefault();
			if (floatMenuOption != null && !floatMenuOption.Disabled)
			{
				Payload x = getPayload(target);
				floatMenuOption.action();
				Payload y = getPayload(target);
				if (!EqualityComparer<Payload>.Default.Equals(x, y))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
				}
			}
		}
	}

	public static void MouseAttachedLabel(string label, float xOffset = 0f, float yOffset = 0f, Color? colorOverride = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = CreateMouseAttachedLabelRect(label, xOffset, yOffset);
		if (colorOverride.HasValue)
		{
			GUI.color = colorOverride.Value;
		}
		Label(rect, label);
		GUI.color = Color.white;
	}

	public static void MouseAttachedLabel(TaggedString label, float xOffset = 0f, float yOffset = 0f, Color? colorOverride = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = CreateMouseAttachedLabelRect(label, xOffset, yOffset);
		if (colorOverride.HasValue)
		{
			GUI.color = colorOverride.Value;
		}
		Label(rect, label);
		GUI.color = Color.white;
	}

	public static void WorldAttachedLabel(Vector3 worldPos, string label, float xOffset = 0f, float yOffset = 0f, Color? colorOverride = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Find.WorldCamera.WorldToScreenPoint(worldPos);
		val.y = (float)Screen.height - val.y;
		val /= Prefs.UIScale;
		Rect rect = CreateAttachedLabelRect(Vector2.op_Implicit(val), label, xOffset, yOffset);
		if (colorOverride.HasValue)
		{
			GUI.color = colorOverride.Value;
		}
		Label(rect, label);
		GUI.color = Color.white;
	}

	private static Rect CreateMouseAttachedLabelRect(string label, float xOffset, float yOffset)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return CreateAttachedLabelRect(Event.current.mousePosition, label, xOffset, yOffset);
	}

	private static Rect CreateAttachedLabelRect(Vector2 screenPosition, string label, float xOffset, float yOffset)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(screenPosition.x + 8f + xOffset, screenPosition.y + 8f + yOffset, 32f, 32f);
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Rect result = default(Rect);
		((Rect)(ref result))._002Ector(((Rect)(ref val)).xMax, ((Rect)(ref val)).y, 9999f, 100f);
		Vector2 val2 = Text.CalcSize(label);
		((Rect)(ref result)).height = Mathf.Max(((Rect)(ref result)).height, val2.y);
		GUI.DrawTexture(new Rect(((Rect)(ref result)).x - val2.x * 0.1f, ((Rect)(ref result)).y, val2.x * 1.2f, val2.y), (Texture)(object)TexUI.GrayTextBG);
		return result;
	}

	public static void WidgetsOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.rawType == 1 || Input.GetMouseButtonUp(0))
		{
			checkboxPainting = false;
			dropdownPainting = false;
		}
		if (checkboxPainting)
		{
			GenUI.DrawMouseAttachment(checkboxPaintingState ? CheckboxOnTex : CheckboxOffTex);
		}
		if (dropdownPainting)
		{
			GenUI.DrawMouseAttachment((Texture)(object)dropdownPainting_Icon, dropdownPainting_Text);
		}
	}
}
