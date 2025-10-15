using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class GenMapUI
{
	public static readonly Texture2D OverlayHealthTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0f, 0f, 0.25f));

	public static readonly Texture2D OverlayEntropyTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.55f, 0.84f, 0.5f));

	public const float NameBGHeight_Tiny = 12f;

	public const float NameBGExtraWidth_Tiny = 4f;

	public const float NameBGHeight_Small = 16f;

	public const float NameBGExtraWidth_Small = 6f;

	public const float LabelOffsetYStandard = -0.4f;

	public const float PsychicEntropyBarHeight = 4f;

	private const float AnimalLabelNudgeUpPixels = 4f;

	private const float BabyLabelNudgeUpPixels = 8f;

	public static readonly Color DefaultThingLabelColor = new Color(1f, 1f, 1f, 0.75f);

	public static Vector2 LabelDrawPosFor(Thing thing, float worldOffsetZ)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 drawPos = thing.DrawPos;
		drawPos.z += worldOffsetZ;
		Vector2 val = Vector2.op_Implicit(Find.Camera.WorldToScreenPoint(drawPos) / Prefs.UIScale);
		val.y = (float)UI.screenHeight - val.y;
		if (thing is Pawn)
		{
			Pawn pawn = (Pawn)thing;
			if (!pawn.RaceProps.Humanlike)
			{
				val.y -= 4f;
			}
			else if (pawn.DevelopmentalStage.Baby())
			{
				val.y -= 8f;
			}
		}
		return val;
	}

	public static Vector2 LabelDrawPosFor(IntVec3 center)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = center.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
		Vector2 val2 = Vector2.op_Implicit(Find.Camera.WorldToScreenPoint(val) / Prefs.UIScale);
		val2.y = (float)UI.screenHeight - val2.y;
		val2.y -= 1f;
		return val2;
	}

	public static void DrawThingLabel(Thing thing, string text)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DrawThingLabel(thing, text, DefaultThingLabelColor);
	}

	public static void DrawThingLabel(Thing thing, string text, Color textColor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		DrawThingLabel(LabelDrawPosFor(thing, -0.4f), text, textColor);
	}

	public static void DrawThingLabel(Vector2 screenPos, string text, Color textColor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		float x = Text.CalcSize(text).x;
		float num = (Text.TinyFontSupported ? 4f : 6f);
		float num2 = (Text.TinyFontSupported ? 12f : 16f);
		GUI.DrawTexture(new Rect(screenPos.x - x / 2f - num, screenPos.y, x + num * 2f, num2), (Texture)(object)TexUI.GrayTextBG);
		GUI.color = textColor;
		Text.Anchor = (TextAnchor)1;
		Widgets.Label(new Rect(screenPos.x - x / 2f, screenPos.y - 3f, x, 999f), text);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
	}

	public static void DrawPawnLabel(Pawn pawn, Vector2 pos, float alpha = 1f, float truncateToWidth = 9999f, Dictionary<string, string> truncatedLabelsCache = null, GameFont font = GameFont.Tiny, bool alwaysDrawBg = true, bool alignCenter = true)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float pawnLabelNameWidth = GetPawnLabelNameWidth(pawn, truncateToWidth, truncatedLabelsCache, font);
		float num = (Prefs.DisableTinyText ? 6f : 4f);
		float num2 = (Prefs.DisableTinyText ? 16f : 12f);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(pos.x - pawnLabelNameWidth / 2f - num, pos.y, pawnLabelNameWidth + num * 2f, num2);
		DrawPawnLabel(pawn, bgRect, alpha, truncateToWidth, truncatedLabelsCache, font, alwaysDrawBg, alignCenter);
	}

	public static void DrawPawnLabel(Pawn pawn, Rect bgRect, float alpha = 1f, float truncateToWidth = 9999f, Dictionary<string, string> truncatedLabelsCache = null, GameFont font = GameFont.Tiny, bool alwaysDrawBg = true, bool alignCenter = true)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = new Color(1f, 1f, 1f, alpha);
		Text.Font = font;
		string pawnLabel = GetPawnLabel(pawn, truncateToWidth, truncatedLabelsCache, font);
		float pawnLabelNameWidth = GetPawnLabelNameWidth(pawn, truncateToWidth, truncatedLabelsCache, font);
		float summaryHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
		if (alwaysDrawBg || summaryHealthPercent < 0.999f)
		{
			GUI.DrawTexture(bgRect, (Texture)(object)TexUI.GrayTextBG);
		}
		if (summaryHealthPercent < 0.999f)
		{
			Widgets.FillableBar(bgRect.ContractedBy(1f), summaryHealthPercent, OverlayHealthTex, BaseContent.ClearTex, doBorder: false);
		}
		Color color = PawnNameColorUtility.PawnNameColorOf(pawn);
		color.a = alpha;
		GUI.color = color;
		Rect rect = default(Rect);
		if (alignCenter)
		{
			Text.Anchor = (TextAnchor)1;
			((Rect)(ref rect))._002Ector(((Rect)(ref bgRect)).center.x - pawnLabelNameWidth / 2f, ((Rect)(ref bgRect)).y - 2f, pawnLabelNameWidth, 100f);
		}
		else
		{
			Text.Anchor = (TextAnchor)0;
			((Rect)(ref rect))._002Ector(((Rect)(ref bgRect)).x + 2f, ((Rect)(ref bgRect)).center.y - Text.CalcSize(pawnLabel).y / 2f, pawnLabelNameWidth, 100f);
		}
		Widgets.Label(rect, pawnLabel);
		if (pawn.Drafted)
		{
			Widgets.DrawLineHorizontal(((Rect)(ref bgRect)).center.x - pawnLabelNameWidth / 2f, ((Rect)(ref bgRect)).y + 11f + (float)((!Text.TinyFontSupported) ? 3 : 0), pawnLabelNameWidth);
		}
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawText(Vector2 worldPos, string text, Color textColor)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(worldPos.x, 0f, worldPos.y);
		Vector2 val2 = Vector2.op_Implicit(Find.Camera.WorldToScreenPoint(val) / Prefs.UIScale);
		val2.y = (float)UI.screenHeight - val2.y;
		Text.Font = GameFont.Tiny;
		GUI.color = textColor;
		Text.Anchor = (TextAnchor)1;
		float x = Text.CalcSize(text).x;
		Widgets.Label(new Rect(val2.x - x / 2f, val2.y - 2f, x, 999f), text);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
	}

	private static float GetPawnLabelNameWidth(Pawn pawn, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache, GameFont font)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GameFont font2 = Text.Font;
		Text.Font = font;
		string pawnLabel = GetPawnLabel(pawn, truncateToWidth, truncatedLabelsCache, font);
		float num = ((font != 0) ? Text.CalcSize(pawnLabel).x : pawnLabel.GetWidthCached());
		if (Math.Abs(Math.Round(Prefs.UIScale) - (double)Prefs.UIScale) > 1.401298464324817E-45)
		{
			num += 0.5f;
		}
		if (num < 20f)
		{
			num = 20f;
		}
		Text.Font = font2;
		return num;
	}

	private static string GetPawnLabel(Pawn pawn, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache, GameFont font)
	{
		GameFont font2 = Text.Font;
		Text.Font = font;
		string result = pawn.LabelShortCap.Truncate(truncateToWidth, truncatedLabelsCache);
		Text.Font = font2;
		return result;
	}
}
