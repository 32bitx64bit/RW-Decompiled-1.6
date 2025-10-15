using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_StudyNotes : ITab
{
	private Vector2 leftScroll;

	private Vector2 rightScroll;

	private ChoiceLetter selectedLetter;

	private Thing previous;

	private const float TopPadding = 20f;

	private const float InitialHeight = 350f;

	private const float TitleHeight = 30f;

	private const float InitialWidth = 610f;

	private const float DateSize = 90f;

	private const float RowHeight = 30f;

	protected Thing StudiableThing => (base.SelThing as Building_HoldingPlatform)?.HeldPawn ?? base.SelThing;

	protected bool Studiable => StudiableThing.TryGetComp<CompStudiable>()?.StudyUnlocked() ?? false;

	public override bool IsVisible
	{
		get
		{
			if (Studiable)
			{
				return StudyUnlocks != null;
			}
			return false;
		}
	}

	private CompStudyUnlocks StudyUnlocks => StudiableThing.TryGetComp<CompStudyUnlocks>();

	protected virtual IReadOnlyList<ChoiceLetter> Letters => StudyUnlocks.Letters;

	protected virtual bool StudyCompleted => StudyUnlocks.Completed;

	protected virtual bool DrawThingIcon => true;

	public ITab_StudyNotes()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(Mathf.Min(610f, (float)UI.screenWidth), 350f);
		labelKey = "TabStudyNotesContents";
	}

	public override void OnOpen()
	{
		selectedLetter = (Letters.EnumerableNullOrEmpty() ? null : Letters.Last());
	}

	protected override void FillTab()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		if (previous != StudiableThing)
		{
			selectedLetter = (Letters.EnumerableNullOrEmpty() ? null : Letters.Last());
			previous = StudiableThing;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 20f, size.x, size.y - 20f);
		val = val.ContractedBy(10f);
		Rect rect = val;
		((Rect)(ref rect)).y = 10f;
		((Rect)(ref rect)).height = 30f;
		Rect rect2 = val;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect)).yMax + 17f;
		rect2.SplitVerticallyWithMargin(out var left, out var right, 17f);
		((Rect)(ref right)).yMin = ((Rect)(ref right)).yMin + 17f;
		Rect rect3 = right;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMin - 17f;
		((Rect)(ref rect3)).yMin = ((Rect)(ref val)).yMin;
		DrawTitle(rect);
		DrawLetters(left);
		if (selectedLetter != null)
		{
			Widgets.LabelScrollable(right, selectedLetter.Text, ref rightScroll);
			return;
		}
		using (new TextBlock(GameFont.Small, (TextAnchor)4, Color.gray))
		{
			Widgets.Label(rect3, "StudyNotesTab_NoDiscoveries".Translate());
		}
	}

	private void DrawTitle(Rect rect)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (DrawThingIcon)
		{
			Rect val = rect;
			((Rect)(ref val)).width = ((Rect)(ref val)).height;
			num = ((Rect)(ref rect)).height + 10f;
			GUI.DrawTexture(val, (Texture)(object)StudiableThing.def.uiIcon);
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + num;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f;
		Rect val2 = rect;
		((Rect)(ref val2)).xMin = ((Rect)(ref rect2)).xMax;
		Rect rect3 = val2;
		((Rect)(ref rect3)).y = ((Rect)(ref rect2)).yMax - 4f;
		using (new TextBlock(GameFont.Medium, (TextAnchor)3))
		{
			Widgets.LabelFit(rect2, StudiableThing.LabelCap);
		}
		CompStudiable compStudiable = StudiableThing.TryGetComp<CompStudiable>();
		if (compStudiable == null)
		{
			return;
		}
		Widgets.CheckboxLabeled(val2, "StudyNotesTab_ToggleStudy".Translate(), ref compStudiable.studyEnabled, disabled: false, null, null, placeCheckboxNearText: true);
		if (!compStudiable.EverStudiable(out var reason) && !reason.NullOrEmpty())
		{
			using (new TextBlock(ColorLibrary.RedReadable))
			{
				Widgets.Label(rect3, reason);
			}
		}
	}

	private void DrawLetters(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).height = Text.LineHeight;
		TaggedString taggedString = (StudyCompleted ? "StudyNotesTab_StudyProgressCompleted".Translate() : "StudyNotesTab_StudyProgressOngoing".Translate());
		TaggedString taggedString2 = "StudyNotesTab_StudyProgress".Translate();
		using (new TextBlock((TextAnchor)3))
		{
			Widgets.Label(rect2, $"{taggedString2}: {taggedString}");
		}
		Widgets.DrawLineHorizontal(((Rect)(ref rect)).x, ((Rect)(ref rect2)).yMax + 4f, ((Rect)(ref rect)).width, Color.gray);
		int num = ((!Letters.EnumerableNullOrEmpty()) ? Letters.Count : 0);
		Rect outRect = rect;
		((Rect)(ref outRect)).yMin = ((Rect)(ref rect2)).yMax + 10f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width, 30f * (float)num);
		float y = 0f;
		Widgets.BeginScrollView(outRect, ref leftScroll, val);
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			DoLetterRow(val, ref y, Letters[num2], num2);
		}
		Widgets.EndScrollView();
	}

	private void DoLetterRow(Rect rect, ref float y, ChoiceLetter letter, int index)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).y = y;
		((Rect)(ref rect)).height = 30f;
		y += 30f;
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlightSelected(rect);
			selectedLetter = letter;
		}
		else if (selectedLetter == letter)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		else if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = 90f;
		Vector2 location = (Vector2)((Find.CurrentMap != null) ? Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile) : default(Vector2));
		string str = GenDate.DateShortStringAt(GenDate.TickGameToAbs(letter.arrivalTick), location);
		Rect rect3 = rect;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax + 4f;
		using (new TextBlock((GameFont?)GameFont.Small, (TextAnchor?)(TextAnchor)3, (bool?)false))
		{
			Widgets.Label(rect2, str.Truncate(((Rect)(ref rect2)).width));
			using (new TextBlock(new Color(0.75f, 0.75f, 0.75f)))
			{
				Widgets.LabelEllipses(rect3, letter.Label);
			}
		}
	}
}
