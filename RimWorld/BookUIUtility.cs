using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class BookUIUtility
{
	private const float KeyValSplit = 140f;

	private const float LineHeight = 25f;

	private const float HyperlinkBaseGap = 4f;

	public static void DrawTitle(Rect rect, Book book)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).width = ((Rect)(ref val)).height;
		GUI.DrawTexture(val, (Texture)(object)book.def.uiIcon);
		Text.Anchor = (TextAnchor)3;
		Text.Font = GameFont.Medium;
		Rect rect2 = rect;
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + (((Rect)(ref rect)).height + 10f);
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - (((Rect)(ref rect)).height + 10f);
		Widgets.LabelFit(rect2, book.LabelCap);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawBookDescPanel(Rect rect, Book book, ref Vector2 scroll)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Widgets.LabelScrollable(rect, book.FlavorUI, ref scroll);
	}

	public static void DrawBookInfoPanel(Rect rect, Book book)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float y = ((Rect)(ref rect)).y;
		DrawBenefits(rect, ref y, book);
		DrawDangers(rect, ref y, book);
		Rect rect2 = rect;
		((Rect)(ref rect2)).y = y;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect)).yMax;
		DrawHyperlinks(rect2, ref y, book);
	}

	private static void DrawDangers(Rect rect, ref float y, Book book)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (book.MentalBreakChancePerHour > 0f)
		{
			DrawSubheader(rect, ref y, "Dangers".Translate());
			y += 10f;
			Widgets.Label(rect, ref y, string.Format("- {0}: {1}", "BookMentalBreak".Translate(), "PerHour".Translate(book.MentalBreakChancePerHour.ToStringPercent("0.0"))));
			y += 10f;
		}
	}

	private static void DrawSubheader(Rect rect, ref float y, string title)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x;
		((Rect)(ref val)).y = y;
		((Rect)(ref val)).xMax = ((Rect)(ref rect)).xMax;
		((Rect)(ref val)).yMax = ((Rect)(ref rect)).yMax;
		Rect val2 = val;
		GUI.BeginGroup(val2);
		float curY = 0f;
		Widgets.ListSeparator(ref curY, ((Rect)(ref val2)).width, title);
		y += curY;
		GUI.EndGroup();
	}

	private static void DrawBenefits(Rect rect, ref float y, Book book)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (BookOutcomeDoer doer in book.BookComp.Doers)
		{
			if (!string.IsNullOrEmpty(doer.GetBenefitsString()))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		DrawSubheader(rect, ref y, "Benefits".Translate());
		y += 10f;
		foreach (BookOutcomeDoer doer2 in book.BookComp.Doers)
		{
			string benefitsString = doer2.GetBenefitsString();
			if (!string.IsNullOrEmpty(benefitsString))
			{
				Widgets.Label(rect, ref y, benefitsString);
			}
		}
		y += 10f;
	}

	private static void DrawHyperlinks(Rect rect, ref float y, Book book)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Color normalOptionColor = Widgets.NormalOptionColor;
		float num = 0f;
		foreach (Dialog_InfoCard.Hyperlink item in book.GetHyperlinks().Reverse())
		{
			float num2 = Text.CalcHeight(item.Label, ((Rect)(ref rect)).width);
			Rect rect2 = rect;
			((Rect)(ref rect2)).y = ((Rect)(ref rect)).yMax - num2 - num - 4f;
			((Rect)(ref rect2)).height = num2;
			TaggedString taggedString = "ViewHyperlink".Translate(item.Label);
			Widgets.HyperlinkWithIcon(rect2, item, taggedString, 2f, 6f, normalOptionColor);
			num += num2;
			y += num2;
		}
	}
}
