using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class GenUI
{
	private struct StackedElementRect
	{
		public Rect rect;

		public int elementIndex;

		public StackedElementRect(Rect rect, int elementIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			this.rect = rect;
			this.elementIndex = elementIndex;
		}
	}

	public class AnonymousStackElement
	{
		public Action<Rect> drawer;

		public float width;
	}

	private struct SpacingCache
	{
		private int maxElements;

		private float[] spaces;

		public void Reset(int elem = 16)
		{
			if (spaces == null || maxElements != elem)
			{
				maxElements = elem;
				spaces = new float[maxElements];
				return;
			}
			for (int i = 0; i < maxElements; i++)
			{
				spaces[i] = 0f;
			}
		}

		public float GetSpaceFor(int elem)
		{
			if (spaces == null || maxElements < 1)
			{
				Reset();
			}
			if (elem >= 0 && elem < maxElements)
			{
				return spaces[elem];
			}
			return 0f;
		}

		public void AddSpace(int elem, float space)
		{
			if (spaces == null || maxElements < 1)
			{
				Reset();
			}
			if (elem >= 0 && elem < maxElements)
			{
				spaces[elem] += space;
			}
		}
	}

	public delegate void StackElementDrawer<T>(Rect rect, T element);

	public delegate float StackElementWidthGetter<T>(T element);

	public const float Pad = 10f;

	public const float GapTiny = 4f;

	public const float GapSmall = 10f;

	public const float Gap = 17f;

	public const float GapWide = 26f;

	public const float GapLabel = 22f;

	public const float ListSpacing = 28f;

	public const float MouseAttachIconSize = 32f;

	public const float MouseAttachIconOffset = 8f;

	public const float ScrollBarWidth = 16f;

	public const float HorizontalSliderHeight = 10f;

	public static readonly Vector2 TradeableDrawSize = new Vector2(150f, 45f);

	public static readonly Color MouseoverColor = new Color(0.3f, 0.7f, 0.9f);

	public static readonly Color SubtleMouseoverColor = new Color(0.7f, 0.7f, 0.7f);

	public static readonly Color FillableBar_Green = new Color(0.40392157f, 0.7019608f, 0.28627452f);

	public static readonly Color FillableBar_Empty = new Color(0.03f, 0.035f, 0.05f);

	public static readonly Vector2 MaxWinSize = new Vector2(1010f, 754f);

	public const float SmallIconSize = 24f;

	public const int RootGUIDepth = 50;

	private const float MouseIconSize = 32f;

	private const float MouseIconOffset = 12f;

	private static readonly Material MouseoverBracketMaterial = MaterialPool.MatFrom("UI/Overlays/MouseoverBracketTex", ShaderDatabase.MetaOverlay);

	private static readonly Texture2D UnderShadowTex = ContentFinder<Texture2D>.Get("UI/Misc/ScreenCornerShadow");

	private static readonly Texture2D UIFlash = ContentFinder<Texture2D>.Get("UI/Misc/Flash");

	private static Dictionary<string, Vector2> labelWidthCache = new Dictionary<string, Vector2>();

	private static readonly Vector2 PieceBarSize = new Vector2(100f, 17f);

	public const float PawnDirectClickRadius = 0.4f;

	private static List<Thing> cellThings = new List<Thing>(32);

	private static readonly Texture2D ArrowTex = ContentFinder<Texture2D>.Get("UI/Overlays/Arrow");

	private static List<StackedElementRect> tmpRects = new List<StackedElementRect>();

	private static List<StackedElementRect> tmpRects2 = new List<StackedElementRect>();

	public const float ElementStackDefaultElementMargin = 5f;

	public const float ElementStackDefaultRowMargin = 4f;

	private static SpacingCache spacingCache;

	public static void SetLabelAlign(TextAnchor a)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = a;
	}

	public static void ResetLabelAlign()
	{
		Text.Anchor = (TextAnchor)0;
	}

	public static float BackgroundDarkAlphaForText()
	{
		if (Find.CurrentMap == null)
		{
			return 0f;
		}
		float num = GenCelestial.CurCelestialSunGlow(Find.CurrentMap);
		float num2 = ((Find.CurrentMap.Biome == BiomeDefOf.IceSheet) ? 1f : Mathf.Clamp01(Find.CurrentMap.snowGrid.TotalDepth / 1000f));
		return num * num2 * 0.41f;
	}

	public static void DrawTextWinterShadow(Rect rect)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float num = BackgroundDarkAlphaForText();
		if (num > 0.001f)
		{
			GUI.color = new Color(1f, 1f, 1f, num);
			GUI.DrawTexture(rect, (Texture)(object)UnderShadowTex);
			GUI.color = Color.white;
		}
	}

	public static void DrawTextureWithMaterial(Rect rect, Texture texture, Material material, Rect texCoords = default(Rect))
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Invalid comparison between Unknown and I4
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (texCoords == default(Rect))
		{
			if ((Object)(object)material == (Object)null)
			{
				GUI.DrawTexture(rect, texture);
			}
			else if ((int)Event.current.type == 7)
			{
				Color val = (Color)(material.shader.SupportsMaskTex() ? GUI.color : new Color(GUI.color.r * 0.5f, GUI.color.g * 0.5f, GUI.color.b * 0.5f, GUI.color.a));
				Graphics.DrawTexture(rect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, val, material);
			}
		}
		else if ((Object)(object)material == (Object)null)
		{
			GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
		}
		else if ((int)Event.current.type == 7)
		{
			Color val2 = (Color)(material.shader.SupportsMaskTex() ? GUI.color : new Color(GUI.color.r * 0.5f, GUI.color.g * 0.5f, GUI.color.b * 0.5f, GUI.color.a));
			Graphics.DrawTexture(rect, texture, texCoords, 0, 0, 0, 0, val2, material);
		}
	}

	public static void DrawTexturePartWithMaterial(Rect rect, Texture texture, Material material, Rect texCoords)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (texCoords == default(Rect))
		{
			if ((Object)(object)material == (Object)null)
			{
				GUI.DrawTexture(rect, texture);
			}
			else if ((int)Event.current.type == 7)
			{
				Color val = (material.shader.SupportsMaskTex() ? GUI.color : (GUI.color * 0.5f));
				Graphics.DrawTexture(rect, texture, new Rect(0f, 0f, 1f, 1f), 0, 0, 0, 0, val, material);
			}
			return;
		}
		((Rect)(ref texCoords)).y = 1f - ((Rect)(ref texCoords)).y - ((Rect)(ref texCoords)).height;
		if ((Object)(object)material == (Object)null)
		{
			GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
		}
		else if ((int)Event.current.type == 7)
		{
			Color val2 = (material.shader.SupportsMaskTex() ? GUI.color : (GUI.color * 0.5f));
			Graphics.DrawTexture(rect, texture, texCoords, 0, 0, 0, 0, val2, material);
		}
	}

	public static float IconDrawScale(ThingDef tDef)
	{
		float num = tDef.uiIconScale;
		if (tDef.uiIconPath.NullOrEmpty() && tDef.graphicData != null)
		{
			num *= Mathf.Min(tDef.graphicData.drawSize.x / (float)tDef.Size.x, tDef.graphicData.drawSize.y / (float)tDef.Size.z);
		}
		return num;
	}

	public static void ErrorDialog(string message)
	{
		if (Find.WindowStack != null)
		{
			Find.WindowStack.Add(new Dialog_MessageBox(message));
		}
	}

	public static void DrawFlash(float centerX, float centerY, float size, float alpha, Color color)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Rect val = new Rect(centerX - size / 2f, centerY - size / 2f, size, size);
		Color color2 = color;
		color2.a = alpha;
		GUI.color = color2;
		GUI.DrawTexture(val, (Texture)(object)UIFlash);
		GUI.color = Color.white;
	}

	public static Vector2 GetSizeCached(this string s)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (labelWidthCache.Count > 2000 || (Time.frameCount % 40000 == 0 && labelWidthCache.Count > 100))
		{
			labelWidthCache.Clear();
		}
		s = s.StripTags();
		if (!labelWidthCache.TryGetValue(s, out var value))
		{
			value = Text.CalcSize(s);
			labelWidthCache.Add(s, value);
		}
		return value;
	}

	public static float GetWidthCached(this string s)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return s.GetSizeCached().x;
	}

	public static float GetHeightCached(this string s)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return s.GetSizeCached().y;
	}

	public static void ClearLabelWidthCache()
	{
		labelWidthCache.Clear();
	}

	public static Rect Rounded(this Rect r)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return new Rect((float)(int)((Rect)(ref r)).x, (float)(int)((Rect)(ref r)).y, (float)(int)((Rect)(ref r)).width, (float)(int)((Rect)(ref r)).height);
	}

	public static Rect RoundedCeil(this Rect r)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return new Rect((float)Mathf.CeilToInt(((Rect)(ref r)).x), (float)Mathf.CeilToInt(((Rect)(ref r)).y), (float)Mathf.CeilToInt(((Rect)(ref r)).width), (float)Mathf.CeilToInt(((Rect)(ref r)).height));
	}

	public static Vector2 Rounded(this Vector2 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)(int)v.x, (float)(int)v.y);
	}

	public static float DistFromRect(Rect r, Vector2 p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Abs(p.x - ((Rect)(ref r)).center.x) - ((Rect)(ref r)).width / 2f;
		if (num < 0f)
		{
			num = 0f;
		}
		float num2 = Mathf.Abs(p.y - ((Rect)(ref r)).center.y) - ((Rect)(ref r)).height / 2f;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		return Mathf.Sqrt(num * num + num2 * num2);
	}

	public static void DrawMouseAttachment(Texture iconTex, string text = "", float angle = 0f, Vector2 offset = default(Vector2), Rect? customRect = null, Color? textColor = null, bool drawTextBackground = false, Color textBgColor = default(Color), Color? iconColor = null, Action<Rect> postDrawAction = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePosition = Event.current.mousePosition;
		float num = mousePosition.y + 12f;
		if (drawTextBackground && text != "")
		{
			Rect value = default(Rect);
			if (customRect.HasValue)
			{
				value = customRect.Value;
			}
			else
			{
				float num2 = Mathf.Min(Text.CalcSize(text).x, 200f);
				float num3 = Text.CalcHeight(text, 200f);
				float num4 = (((Object)(object)iconTex != (Object)null) ? 42f : 0f);
				((Rect)(ref value))._002Ector(mousePosition.x + 12f - 4f, num + num4, num2 + 8f, num3);
			}
			Widgets.DrawBoxSolid(value, textBgColor);
		}
		if ((Object)(object)iconTex != (Object)null)
		{
			Rect mouseRect;
			if (customRect.HasValue)
			{
				mouseRect = customRect.Value;
			}
			else
			{
				mouseRect = new Rect(mousePosition.x + 8f, num + 8f, 32f, 32f);
			}
			Find.WindowStack.ImmediateWindow(34003428, mouseRect, WindowLayer.Super, delegate
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				Rect val = mouseRect.AtZero();
				((Rect)(ref val)).position = ((Rect)(ref val)).position + new Vector2(offset.x * ((Rect)(ref val)).size.x, offset.y * ((Rect)(ref val)).size.y);
				GUI.color = (Color)(((_003F?)iconColor) ?? Color.white);
				Widgets.DrawTextureRotated(val, iconTex, angle);
				GUI.color = Color.white;
				postDrawAction?.Invoke(val);
			}, doBackground: false, absorbInputAroundWindow: false, 0f);
			num += ((Rect)(ref mouseRect)).height + 10f;
		}
		if (text != "")
		{
			Rect textRect = new Rect(mousePosition.x + 12f, num, 200f, 9999f);
			Find.WindowStack.ImmediateWindow(34003429, textRect, WindowLayer.Super, delegate
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				GameFont font = Text.Font;
				Text.Font = GameFont.Small;
				GUI.color = (Color)(((_003F?)textColor) ?? Color.white);
				Widgets.Label(textRect.AtZero(), text);
				GUI.color = Color.white;
				Text.Font = font;
			}, doBackground: false, absorbInputAroundWindow: false, 0f);
		}
	}

	public static void DrawMouseAttachment(Texture2D icon)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePosition = Event.current.mousePosition;
		Rect mouseRect = new Rect(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f);
		Find.WindowStack.ImmediateWindow(34003428, mouseRect, WindowLayer.Super, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			GUI.DrawTexture(mouseRect.AtZero(), (Texture)(object)icon);
		}, doBackground: false, absorbInputAroundWindow: false, 0f);
	}

	public static void RenderMouseoverBracket()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = UI.MouseCell().ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
		Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, MouseoverBracketMaterial, 0);
	}

	public static void DrawStatusLevel(Need status, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Widgets.Label(new Rect(0f, 2f, ((Rect)(ref rect)).width, 25f), status.LabelCap);
		Rect val = new Rect(100f, 3f, PieceBarSize.x, PieceBarSize.y);
		Widgets.FillableBar(val, status.CurLevelPercentage);
		Widgets.FillableBarChangeArrows(val, status.GUIChangeArrow);
		Widgets.EndGroup();
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, status.GetTipString());
		}
		if (Mouse.IsOver(rect))
		{
			GUI.DrawTexture(rect, (Texture)(object)TexUI.HighlightTex);
		}
	}

	public static IEnumerable<LocalTargetInfo> TargetsAtMouse(TargetingParameters clickParams, bool thingsOnly = false, ITargetingSource source = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return TargetsAt(UI.MouseMapPosition(), clickParams, thingsOnly, source);
	}

	public static IEnumerable<LocalTargetInfo> TargetsAt(Vector3 clickPos, TargetingParameters clickParams, bool thingsOnly = false, ITargetingSource source = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		List<Thing> clickableList = ThingsUnderMouse(clickPos, 0.8f, clickParams, source);
		Thing caster = source?.Caster;
		for (int i = 0; i < clickableList.Count; i++)
		{
			if (!(clickableList[i] is Pawn pawn) || !pawn.IsPsychologicallyInvisible() || caster == null || caster.Faction == pawn.Faction)
			{
				yield return clickableList[i];
			}
		}
		if (!thingsOnly)
		{
			IntVec3 intVec = UI.MouseCell();
			if (intVec.InBounds(Find.CurrentMap, clickParams.mapBoundsContractedBy) && clickParams.CanTarget(new TargetInfo(intVec, Find.CurrentMap), source))
			{
				yield return intVec;
			}
		}
	}

	public static List<Thing> ThingsUnderMouse(Vector3 clickPos, float pawnWideClickRadius, TargetingParameters clickParams, ITargetingSource source = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 intVec = IntVec3.FromVector3(clickPos);
		List<Thing> list = new List<Thing>();
		IReadOnlyList<Pawn> allPawnsSpawned = Find.CurrentMap.mapPawns.AllPawnsSpawned;
		for (int i = 0; i < allPawnsSpawned.Count; i++)
		{
			Pawn pawn = allPawnsSpawned[i];
			if ((pawn.DrawPos - clickPos).MagnitudeHorizontal() < 0.4f && clickParams.CanTarget(pawn, source))
			{
				list.Add(pawn);
				list.AddRange(ContainingSelectionUtility.SelectableContainedThings(pawn));
			}
		}
		list.Sort(CompareThingsByDistanceToMousePointer);
		cellThings.Clear();
		foreach (Thing item in Find.CurrentMap.thingGrid.ThingsAt(intVec))
		{
			if (!list.Contains(item) && clickParams.CanTarget(item, source))
			{
				cellThings.Add(item);
				cellThings.AddRange(ContainingSelectionUtility.SelectableContainedThings(item));
			}
		}
		IntVec3[] adjacentCells = GenAdj.AdjacentCells;
		for (int j = 0; j < adjacentCells.Length; j++)
		{
			IntVec3 c = adjacentCells[j] + intVec;
			if (!c.InBounds(Find.CurrentMap) || c.GetItemCount(Find.CurrentMap) <= 1)
			{
				continue;
			}
			foreach (Thing item2 in Find.CurrentMap.thingGrid.ThingsAt(c))
			{
				if (item2.def.category == ThingCategory.Item && (item2.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared() <= 0.25f && !list.Contains(item2) && clickParams.CanTarget(item2, source))
				{
					cellThings.Add(item2);
				}
			}
		}
		List<Thing> list2 = Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.WithCustomRectForSelector);
		for (int k = 0; k < list2.Count; k++)
		{
			Thing thing2 = list2[k];
			if (thing2.CustomRectForSelector.HasValue && thing2.CustomRectForSelector.Value.Contains(intVec) && !list.Contains(thing2) && clickParams.CanTarget(thing2, source))
			{
				cellThings.Add(thing2);
			}
		}
		cellThings.Sort(CompareThingsByDrawAltitudeOrDistToItem);
		list.AddRange(cellThings);
		cellThings.Clear();
		IReadOnlyList<Pawn> allPawnsSpawned2 = Find.CurrentMap.mapPawns.AllPawnsSpawned;
		for (int l = 0; l < allPawnsSpawned2.Count; l++)
		{
			Pawn pawn2 = allPawnsSpawned2[l];
			if ((pawn2.DrawPos - clickPos).MagnitudeHorizontal() < pawnWideClickRadius && clickParams.CanTarget(pawn2, source))
			{
				cellThings.Add(pawn2);
			}
		}
		cellThings.Sort(CompareThingsByDistanceToMousePointer);
		for (int m = 0; m < cellThings.Count; m++)
		{
			if (!list.Contains(cellThings[m]))
			{
				list.Add(cellThings[m]);
				list.AddRange(ContainingSelectionUtility.SelectableContainedThings(cellThings[m]));
			}
		}
		list.RemoveAll((Thing thing) => !clickParams.CanTarget(thing, source));
		list.RemoveAll((Thing thing) => thing is Pawn pawn3 && pawn3.IsHiddenFromPlayer());
		cellThings.Clear();
		return list;
	}

	private static int CompareThingsByDistanceToMousePointer(Thing a, Thing b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = UI.MouseMapPosition();
		float num = (a.DrawPosHeld.Value - val).MagnitudeHorizontalSquared();
		float num2 = (b.DrawPosHeld.Value - val).MagnitudeHorizontalSquared();
		if (num < num2)
		{
			return -1;
		}
		if (num == num2)
		{
			return b.Spawned.CompareTo(a.Spawned);
		}
		return 1;
	}

	private static int CompareThingsByDrawAltitudeOrDistToItem(Thing A, Thing B)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (A.def.category == ThingCategory.Item && B.def.category == ThingCategory.Item)
		{
			return (A.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared().CompareTo((B.TrueCenter() - UI.MouseMapPosition()).MagnitudeHorizontalSquared());
		}
		Thing spawnedParentOrMe = A.SpawnedParentOrMe;
		Thing spawnedParentOrMe2 = B.SpawnedParentOrMe;
		if (spawnedParentOrMe.def.Altitude != spawnedParentOrMe2.def.Altitude)
		{
			return spawnedParentOrMe2.def.Altitude.CompareTo(spawnedParentOrMe.def.Altitude);
		}
		return B.Spawned.CompareTo(A.Spawned);
	}

	public static int CurrentAdjustmentMultiplier()
	{
		if (KeyBindingDefOf.ModifierIncrement_10x.IsDownEvent && KeyBindingDefOf.ModifierIncrement_100x.IsDownEvent)
		{
			return 1000;
		}
		if (KeyBindingDefOf.ModifierIncrement_100x.IsDownEvent)
		{
			return 100;
		}
		if (KeyBindingDefOf.ModifierIncrement_10x.IsDownEvent)
		{
			return 10;
		}
		return 1;
	}

	public static Rect GetInnerRect(this Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return rect.ContractedBy(17f);
	}

	public static Rect ExpandedBy(this Rect rect, float margin)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x - margin, ((Rect)(ref rect)).y - margin, ((Rect)(ref rect)).width + margin * 2f, ((Rect)(ref rect)).height + margin * 2f);
	}

	public static Rect ExpandedBy(this Rect rect, float marginX, float marginY)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x - marginX, ((Rect)(ref rect)).y - marginY, ((Rect)(ref rect)).width + marginX * 2f, ((Rect)(ref rect)).height + marginY * 2f);
	}

	public static Rect ContractedBy(this Rect rect, float margin)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + margin, ((Rect)(ref rect)).y + margin, ((Rect)(ref rect)).width - margin * 2f, ((Rect)(ref rect)).height - margin * 2f);
	}

	public static Rect ContractedBy(this Rect rect, float marginX, float marginY)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + marginX, ((Rect)(ref rect)).y + marginY, ((Rect)(ref rect)).width - marginX * 2f, ((Rect)(ref rect)).height - marginY * 2f);
	}

	public static Rect ScaledBy(this Rect rect, float scale)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x - ((Rect)(ref rect)).width * (scale - 1f) / 2f;
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y - ((Rect)(ref rect)).height * (scale - 1f) / 2f;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width * scale;
		((Rect)(ref rect)).height = ((Rect)(ref rect)).height * scale;
		return rect;
	}

	public static Rect CenteredOnXIn(this Rect rect, Rect otherRect)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref otherRect)).x + (((Rect)(ref otherRect)).width - ((Rect)(ref rect)).width) / 2f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	public static Rect CenteredOnYIn(this Rect rect, Rect otherRect)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref otherRect)).y + (((Rect)(ref otherRect)).height - ((Rect)(ref rect)).height) / 2f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	public static Rect AtZero(this Rect rect)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
	}

	public static Rect Union(this Rect a, Rect b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Rect result = default(Rect);
		((Rect)(ref result)).min = Vector2.Min(((Rect)(ref a)).min, ((Rect)(ref b)).min);
		((Rect)(ref result)).max = Vector2.Max(((Rect)(ref a)).max, ((Rect)(ref b)).max);
		return result;
	}

	public static void AbsorbClicksInRect(Rect r)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0 && ((Rect)(ref r)).Contains(Event.current.mousePosition))
		{
			Event.current.Use();
		}
	}

	public static Rect LeftHalf(this Rect rect)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).height);
	}

	public static Rect LeftPart(this Rect rect, float pct)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width * pct, ((Rect)(ref rect)).height);
	}

	public static Rect LeftPartPixels(this Rect rect, float width)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, width, ((Rect)(ref rect)).height);
	}

	public static Rect RightHalf(this Rect rect)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f, ((Rect)(ref rect)).height);
	}

	public static Rect RightPart(this Rect rect, float pct)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width * (1f - pct), ((Rect)(ref rect)).y, ((Rect)(ref rect)).width * pct, ((Rect)(ref rect)).height);
	}

	public static Rect RightPartPixels(this Rect rect, float width)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - width, ((Rect)(ref rect)).y, width, ((Rect)(ref rect)).height);
	}

	public static Rect TopHalf(this Rect rect)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
	}

	public static Rect TopPart(this Rect rect, float pct)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height * pct);
	}

	public static Rect TopPartPixels(this Rect rect, float height)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, height);
	}

	public static Rect BottomHalf(this Rect rect)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
	}

	public static Rect BottomPart(this Rect rect, float pct)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height * (1f - pct), ((Rect)(ref rect)).width, ((Rect)(ref rect)).height * pct);
	}

	public static Rect BottomPartPixels(this Rect rect, float height)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - height, ((Rect)(ref rect)).width, height);
	}

	public static Rect MiddlePart(this Rect rect, float pctWidth, float pctHeight)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f - ((Rect)(ref rect)).width * pctWidth / 2f, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height / 2f - ((Rect)(ref rect)).height * pctHeight / 2f, ((Rect)(ref rect)).width * pctWidth, ((Rect)(ref rect)).height * pctHeight);
	}

	public static Rect MiddlePartPixels(this Rect rect, float width, float height)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f - width / 2f, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height / 2f - height / 2f, width, height);
	}

	public static IEnumerable<Vector2> Corners(this Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		yield return new Vector2(((Rect)(ref rect)).x, ((Rect)(ref rect)).y);
		yield return new Vector2(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMax);
		yield return new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
		yield return new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).y);
	}

	public static bool SplitHorizontallyWithMargin(this Rect rect, out Rect top, out Rect bottom, out float overflow, float compressibleMargin = 0f, float? topHeight = null, float? bottomHeight = null)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (topHeight.HasValue == bottomHeight.HasValue)
		{
			throw new ArgumentException("Exactly one null height and one non-null height must be provided.");
		}
		overflow = Mathf.Max(0f, (topHeight ?? bottomHeight.GetValueOrDefault()) - ((Rect)(ref rect)).height);
		float num = Mathf.Clamp(topHeight ?? (((Rect)(ref rect)).height - bottomHeight.Value - compressibleMargin), 0f, ((Rect)(ref rect)).height);
		float num2 = Mathf.Clamp(bottomHeight ?? (((Rect)(ref rect)).height - topHeight.Value - compressibleMargin), 0f, ((Rect)(ref rect)).height);
		top = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, num);
		bottom = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMax - num2, ((Rect)(ref rect)).width, num2);
		return overflow == 0f;
	}

	public static void SplitVerticallyWithMargin(this Rect rect, out Rect left, out Rect right, float margin)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).width / 2f;
		left = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, num - margin / 2f, ((Rect)(ref rect)).height);
		right = new Rect(((Rect)(ref left)).xMax + margin, ((Rect)(ref rect)).y, num - margin / 2f, ((Rect)(ref rect)).height);
	}

	public static bool SplitVerticallyWithMargin(this Rect rect, out Rect left, out Rect right, out float overflow, float compressibleMargin = 0f, float? leftWidth = null, float? rightWidth = null)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (leftWidth.HasValue == rightWidth.HasValue)
		{
			throw new ArgumentException("Exactly one null width and one non-null width must be provided.");
		}
		overflow = Mathf.Max(0f, (leftWidth ?? rightWidth.GetValueOrDefault()) - ((Rect)(ref rect)).width);
		float num = Mathf.Clamp(leftWidth ?? (((Rect)(ref rect)).width - rightWidth.Value - compressibleMargin), 0f, ((Rect)(ref rect)).width);
		float num2 = Mathf.Clamp(rightWidth ?? (((Rect)(ref rect)).width - leftWidth.Value - compressibleMargin), 0f, ((Rect)(ref rect)).width);
		left = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, num, ((Rect)(ref rect)).height);
		right = new Rect(((Rect)(ref rect)).xMax - num2, ((Rect)(ref rect)).y, num2, ((Rect)(ref rect)).height);
		return overflow == 0f;
	}

	public static void SplitHorizontally(this Rect rect, float topHeight, out Rect top, out Rect bottom)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		rect.SplitHorizontallyWithMargin(out top, out bottom, out var _, 0f, topHeight);
	}

	public static void SplitVertically(this Rect rect, float leftWidth, out Rect left, out Rect right)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		rect.SplitVerticallyWithMargin(out left, out right, out var _, 0f, leftWidth);
	}

	public static Color LerpColor(List<Pair<float, Color>> colors, float value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (colors.Count == 0)
		{
			return Color.white;
		}
		for (int i = 0; i < colors.Count; i++)
		{
			if (value < colors[i].First)
			{
				if (i == 0)
				{
					return colors[i].Second;
				}
				return Color.Lerp(colors[i - 1].Second, colors[i].Second, Mathf.InverseLerp(colors[i - 1].First, colors[i].First, value));
			}
		}
		return colors.Last().Second;
	}

	public static Vector2 GetMouseAttachedWindowPos(float width, float height)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePosition = Event.current.mousePosition;
		float num = 0f;
		num = ((mousePosition.y + 14f + height < (float)UI.screenHeight) ? (mousePosition.y + 14f) : ((!(mousePosition.y - 5f - height >= 0f)) ? ((float)UI.screenHeight - (14f + height)) : (mousePosition.y - 5f - height)));
		float num2 = 0f;
		num2 = ((!(mousePosition.x + 16f + width < (float)UI.screenWidth)) ? (mousePosition.x - 4f - width) : (mousePosition.x + 16f));
		return new Vector2(num2, num);
	}

	public static float GetCenteredButtonPos(int buttonIndex, int buttonsCount, float totalWidth, float buttonWidth, float pad = 10f)
	{
		float num = (float)buttonsCount * buttonWidth + (float)(buttonsCount - 1) * pad;
		return Mathf.Floor((totalWidth - num) / 2f + (float)buttonIndex * (buttonWidth + pad));
	}

	public static void DrawArrowPointingAt(Rect rect)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2((float)UI.screenWidth, (float)UI.screenHeight) / 2f;
		float angle = Mathf.Atan2(((Rect)(ref rect)).center.x - val.x, val.y - ((Rect)(ref rect)).center.y) * 57.29578f;
		Bounds val2 = new Bounds(Vector2.op_Implicit(((Rect)(ref rect)).center), Vector2.op_Implicit(((Rect)(ref rect)).size));
		Vector2 val3 = Vector2.op_Implicit(((Bounds)(ref val2)).ClosestPoint(Vector2.op_Implicit(val)));
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(val3 + Vector2.left * (float)((Texture)ArrowTex).width * 0.5f, new Vector2((float)((Texture)ArrowTex).width, (float)((Texture)ArrowTex).height));
		Matrix4x4 matrix = GUI.matrix;
		GUI.matrix = Matrix4x4.identity;
		Vector2 center = GUIUtility.GUIToScreenPoint(val3);
		GUI.matrix = matrix;
		UI.RotateAroundPivot(angle, center);
		GUI.DrawTexture(val4, (Texture)(object)ArrowTex);
		GUI.matrix = matrix;
		UnityGUIBugsFixer.Notify_GUIMatrixChanged();
	}

	public static void DrawArrowPointingAtWorldspace(Vector3 worldspace, Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = camera.WorldToScreenPoint(worldspace) / Prefs.UIScale;
		DrawArrowPointingAt(new Rect(new Vector2(val.x, (float)UI.screenHeight - val.y) + new Vector2(-2f, 2f), new Vector2(4f, 4f)));
	}

	public static Rect DrawElementStack<T>(Rect rect, float rowHeight, List<T> elements, StackElementDrawer<T> drawer, StackElementWidthGetter<T> widthGetter, float rowMargin = 4f, float elementMargin = 5f, bool allowOrderOptimization = true)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		tmpRects.Clear();
		tmpRects2.Clear();
		for (int i = 0; i < elements.Count; i++)
		{
			tmpRects.Add(new StackedElementRect(new Rect(0f, 0f, widthGetter(elements[i]), rowHeight), i));
		}
		int num = Mathf.FloorToInt(((Rect)(ref rect)).height / rowHeight);
		List<StackedElementRect> list = tmpRects;
		float num3;
		float num2;
		if (allowOrderOptimization)
		{
			num3 = (num2 = 0f);
			while (num2 < (float)num)
			{
				StackedElementRect item = default(StackedElementRect);
				int num4 = -1;
				for (int j = 0; j < list.Count; j++)
				{
					StackedElementRect stackedElementRect = list[j];
					if (num4 == -1 || (((Rect)(ref item.rect)).width < ((Rect)(ref stackedElementRect.rect)).width && ((Rect)(ref stackedElementRect.rect)).width < ((Rect)(ref rect)).width - num3))
					{
						num4 = j;
						item = stackedElementRect;
					}
				}
				if (num4 == -1)
				{
					if (num3 == 0f)
					{
						break;
					}
					num3 = 0f;
					num2 += 1f;
				}
				else
				{
					num3 += ((Rect)(ref item.rect)).width + elementMargin;
					tmpRects2.Add(item);
				}
				list.RemoveAt(num4);
				if (list.Count <= 0)
				{
					break;
				}
			}
			list = tmpRects2;
		}
		num3 = (num2 = 0f);
		while (list.Count > 0)
		{
			StackedElementRect stackedElementRect2 = list[0];
			if (num3 + ((Rect)(ref stackedElementRect2.rect)).width > ((Rect)(ref rect)).width)
			{
				num3 = 0f;
				num2 += rowHeight + rowMargin;
			}
			drawer?.Invoke(new Rect(((Rect)(ref rect)).x + num3, ((Rect)(ref rect)).y + num2, ((Rect)(ref stackedElementRect2.rect)).width, ((Rect)(ref stackedElementRect2.rect)).height), elements[stackedElementRect2.elementIndex]);
			num3 += ((Rect)(ref stackedElementRect2.rect)).width + elementMargin;
			list.RemoveAt(0);
		}
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, num2 + rowHeight);
	}

	public static Rect DrawElementStackVertical<T>(Rect rect, float rowHeight, List<T> elements, StackElementDrawer<T> drawer, StackElementWidthGetter<T> widthGetter, float elementMargin = 5f, bool highlightOdd = false)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		tmpRects.Clear();
		for (int i = 0; i < elements.Count; i++)
		{
			tmpRects.Add(new StackedElementRect(new Rect(0f, 0f, widthGetter(elements[i]), rowHeight), i));
		}
		int elem = Mathf.FloorToInt(((Rect)(ref rect)).height / rowHeight);
		spacingCache.Reset(elem);
		int num = 0;
		float num2 = 0f;
		float num3 = 0f;
		Rect rect2 = default(Rect);
		for (int j = 0; j < tmpRects.Count; j++)
		{
			StackedElementRect stackedElementRect = tmpRects[j];
			if (num3 + ((Rect)(ref stackedElementRect.rect)).height > ((Rect)(ref rect)).height)
			{
				num3 = 0f;
				num = 0;
			}
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + spacingCache.GetSpaceFor(num), ((Rect)(ref rect)).y + num3, ((Rect)(ref stackedElementRect.rect)).width, ((Rect)(ref stackedElementRect.rect)).height);
			if (highlightOdd && j % 2 == 1)
			{
				Widgets.DrawLightHighlight(rect2);
			}
			drawer(rect2, elements[stackedElementRect.elementIndex]);
			num3 += ((Rect)(ref stackedElementRect.rect)).height + elementMargin;
			spacingCache.AddSpace(num, ((Rect)(ref stackedElementRect.rect)).width + elementMargin);
			num2 = Mathf.Max(num2, spacingCache.GetSpaceFor(num));
			num++;
		}
		return new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, num2, num3 + rowHeight);
	}
}
