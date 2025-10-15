using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_GrowthMomentChoices : Window
{
	private ChoiceLetter_GrowthMoment letter;

	private List<SkillDef> chosenPassions = new List<SkillDef>();

	private Trait chosenTrait;

	private TaggedString text;

	private bool showBio = true;

	private float scrollHeight;

	private Vector2 scrollPosition;

	private const float WidthWithTabs = 1000f;

	private const float WidthRegular = 480f;

	private const float PassionListingWidth = 230f;

	private const float PassionIconSize = 24f;

	private const float OptionTabIn = 30f;

	private static List<TabRecord> tmpTabs = new List<TabRecord>();

	private SkillDef SinglePassionChoice
	{
		get
		{
			if (chosenPassions.Count == 1)
			{
				return chosenPassions[0];
			}
			return null;
		}
	}

	private float Height => CharacterCardUtility.PawnCardSize(letter.pawn).y + Window.CloseButSize.y + 4f + Margin * 2f;

	public override Vector2 InitialSize
	{
		get
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!letter.ShowInfoTabs)
			{
				return new Vector2(480f, Height);
			}
			return new Vector2(1000f, Height);
		}
	}

	public Dialog_GrowthMomentChoices(TaggedString text, ChoiceLetter_GrowthMoment letter)
	{
		this.text = text;
		this.letter = letter;
		forcePause = true;
		absorbInputAroundWindow = true;
		if (!SelectionsMade())
		{
			closeOnAccept = false;
			closeOnCancel = false;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		bool showInfoTabs = letter.ShowInfoTabs;
		float width = 446f;
		Rect outRect = (showInfoTabs ? inRect.LeftPartPixels(width) : inRect);
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - (4f + Window.CloseButSize.y);
		Text.Font = GameFont.Small;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width - 16f, scrollHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float curY = 0f;
		Widgets.Label(0f, ref curY, ((Rect)(ref viewRect)).width, text.Resolve());
		curY += 14f;
		DrawPassionChoices(((Rect)(ref viewRect)).width, ref curY);
		DrawTraitChoices(((Rect)(ref viewRect)).width, ref curY);
		DrawBottomText(((Rect)(ref viewRect)).width, ref curY);
		if ((int)Event.current.type == 8)
		{
			scrollHeight = Mathf.Max(curY, ((Rect)(ref outRect)).height);
		}
		Widgets.EndScrollView();
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, ((Rect)(ref outRect)).yMax + 4f, ((Rect)(ref inRect)).width, Window.CloseButSize.y);
		AcceptanceReport acceptanceReport = CanClose();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref val)).xMax - Window.CloseButSize.x, ((Rect)(ref val)).y, Window.CloseButSize.x, Window.CloseButSize.y);
		if (letter.ArchiveView)
		{
			((Rect)(ref rect)).x = ((Rect)(ref val)).center.x - Window.CloseButSize.x / 2f;
		}
		else
		{
			if (Widgets.ButtonText(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, Window.CloseButSize.x, Window.CloseButSize.y), "Later".Translate()))
			{
				if (letter.ShouldAutomaticallyOpenLetter)
				{
					Messages.Message("MessageCannotPostponeGrowthMoment".Translate(letter.pawn.Named("PAWN")), null, MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					Close();
				}
			}
			if (!acceptanceReport.Accepted)
			{
				TextAnchor anchor = Text.Anchor;
				GameFont font = Text.Font;
				Text.Font = GameFont.Tiny;
				Text.Anchor = (TextAnchor)5;
				Rect rect2 = val;
				((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).xMin - 4f;
				Widgets.Label(rect2, acceptanceReport.Reason.Colorize(ColoredText.WarningColor));
				Text.Font = font;
				Text.Anchor = anchor;
			}
		}
		if (Widgets.ButtonText(rect, "OK".Translate()))
		{
			if (acceptanceReport.Accepted)
			{
				letter.MakeChoices(chosenPassions, chosenTrait);
				Close();
				Find.LetterStack.RemoveLetter(letter);
			}
			else
			{
				Messages.Message(acceptanceReport.Reason, null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
		if (showInfoTabs)
		{
			Rect val2 = inRect.RightPartPixels(1000f - ((Rect)(ref outRect)).width - 34f);
			((Rect)(ref val2)).xMin = ((Rect)(ref val2)).xMin + 17f;
			((Rect)(ref val2)).yMax = ((Rect)(ref val2)).yMax - (4f + Window.CloseButSize.y);
			tmpTabs.Clear();
			tmpTabs.Add(new TabRecord("TabCharacter".Translate(), delegate
			{
				showBio = true;
			}, showBio));
			tmpTabs.Add(new TabRecord("TabHealth".Translate(), delegate
			{
				showBio = false;
			}, !showBio));
			((Rect)(ref val2)).yMin = ((Rect)(ref val2)).yMin + 32f;
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(((Rect)(ref val2)).x + (showBio ? 17f : 0f), ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height);
			Widgets.DrawMenuSection(val2);
			if (showBio)
			{
				CharacterCardUtility.DrawCharacterCard(rect3, letter.pawn, null, default(Rect), showName: false);
			}
			else
			{
				HealthCardUtility.DrawHediffListing(rect3, letter.pawn, showBloodLoss: false, 17f);
			}
			TabDrawer.DrawTabs(val2, tmpTabs);
			tmpTabs.Clear();
		}
	}

	private bool SelectionsMade()
	{
		if (letter.ArchiveView)
		{
			return true;
		}
		if (!letter.passionChoices.NullOrEmpty() && chosenPassions.NullOrEmpty() && letter.passionGainsCount > 0)
		{
			return false;
		}
		if (!letter.traitChoices.NullOrEmpty() && chosenTrait == null)
		{
			return false;
		}
		return true;
	}

	private AcceptanceReport CanClose()
	{
		if (letter.ArchiveView)
		{
			return true;
		}
		if (!letter.passionChoices.NullOrEmpty() && chosenPassions.Count != letter.passionGainsCount)
		{
			if (letter.passionGainsCount == 1)
			{
				return "SelectPassionSingular".Translate();
			}
			return "SelectPassionsPlural".Translate(letter.passionGainsCount);
		}
		if (!letter.traitChoices.NullOrEmpty() && chosenTrait == null)
		{
			return "SelectATrait".Translate();
		}
		if (!SelectionsMade())
		{
			return "BirthdayMakeChoices".Translate();
		}
		return AcceptanceReport.WasAccepted;
	}

	private void DrawTraitChoices(float width, ref float curY)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		if (!letter.ArchiveView && !letter.traitChoices.NullOrEmpty())
		{
			Widgets.Label(0f, ref curY, width, "BirthdayPickTrait".Translate(letter.pawn).Resolve() + ":");
			Listing_Standard listing_Standard = new Listing_Standard();
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(0f, curY, 230f, 99999f);
			listing_Standard.Begin(rect);
			foreach (Trait traitChoice in letter.traitChoices)
			{
				if (listing_Standard.RadioButton(traitChoice.LabelCap, chosenTrait == traitChoice, 30f, traitChoice.TipString(letter.pawn)))
				{
					chosenTrait = traitChoice;
				}
			}
			if (letter.noTraitOptionShown)
			{
				TaggedString taggedString = "BirthdayNoTraitChoice".Translate();
				TaggedString taggedString2 = "BirthdayNoTraitChoiceTooltip".Translate(letter.pawn);
				if (listing_Standard.RadioButton(taggedString, chosenTrait == ChoiceLetter_GrowthMoment.NoTrait, 30f, taggedString2))
				{
					chosenTrait = ChoiceLetter_GrowthMoment.NoTrait;
				}
			}
			listing_Standard.End();
			curY += listing_Standard.CurHeight + 10f + 4f;
		}
		if (letter.ArchiveView && letter.chosenTrait != null)
		{
			Widgets.Label(0f, ref curY, width, "BirthdayTraitArchive".Translate(letter.chosenTrait.Label.Colorize(ColorLibrary.LightBlue)));
			curY += 14f;
		}
	}

	private void DrawPassionChoices(float width, ref float curY)
	{
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (!letter.ArchiveView && !letter.passionChoices.NullOrEmpty() && letter.passionGainsCount > 0)
		{
			Widgets.Label(0f, ref curY, width, ((letter.passionGainsCount == 1) ? "BirthdayPickPassion".Translate(letter.pawn) : "BirthdayPickPassions".Translate(letter.pawn, letter.passionGainsCount)).Resolve() + ":");
			Listing_Standard listing_Standard = new Listing_Standard();
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(0f, curY, 230f, 99999f);
			listing_Standard.Begin(rect);
			foreach (SkillDef passionChoice in letter.passionChoices)
			{
				SkillRecord skill = letter.pawn.skills.GetSkill(passionChoice);
				Passion passion = (chosenPassions.Contains(passionChoice) ? skill.passion.IncrementPassion() : skill.passion);
				if ((int)passion > 0)
				{
					Texture2D val = ((passion == Passion.Major) ? SkillUI.PassionMajorIcon : SkillUI.PassionMinorIcon);
					GUI.DrawTexture(new Rect(((Rect)(ref rect)).xMax - 55f, listing_Standard.CurHeight, 24f, 24f), (Texture)(object)val);
				}
				if (letter.passionGainsCount > 1)
				{
					bool checkOn = chosenPassions.Contains(passionChoice);
					bool flag = checkOn;
					listing_Standard.CheckboxLabeled(passionChoice.LabelCap, ref checkOn, 30f);
					if (checkOn != flag)
					{
						if (checkOn)
						{
							chosenPassions.Add(passionChoice);
						}
						else
						{
							chosenPassions.Remove(passionChoice);
						}
					}
				}
				else if (listing_Standard.RadioButton(passionChoice.LabelCap, SinglePassionChoice == passionChoice, 30f))
				{
					chosenPassions.Clear();
					chosenPassions.Add(passionChoice);
				}
			}
			listing_Standard.End();
			curY += listing_Standard.CurHeight + 10f + 4f;
		}
		if (letter.ArchiveView && !letter.chosenPassions.NullOrEmpty())
		{
			Widgets.Label(0f, ref curY, width, "BirthdayPassionArchive".Translate(letter.chosenPassions.Select((SkillDef x) => x.label).ToCommaList(useAnd: true).Colorize(ColorLibrary.LightBlue)));
			curY += 14f;
		}
	}

	private void DrawBottomText(float width, ref float curY)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (letter.growthTier >= 0 && (!letter.passionChoices.NullOrEmpty() || !letter.traitChoices.NullOrEmpty()))
		{
			string text = "BirthdayGrowthTier".Translate(letter.pawn, letter.growthTier).Colorize(ColoredText.SubtleGrayColor);
			if (letter.pawn.Name != letter.oldName)
			{
				text = "BirthdayNickname".Translate(letter.oldName.ToStringFull.Colorize(ColoredText.NameColor), letter.pawn.LabelShort.Colorize(ColoredText.NameColor)).Resolve() + "\n\n" + text;
			}
			Widgets.Label(0f, ref curY, width, text);
			curY += 10f;
		}
	}
}
