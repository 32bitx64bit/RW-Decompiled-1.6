using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class CharacterCardUtility
{
	private struct LeftRectSection
	{
		public Rect rect;

		public Action<Rect> drawer;

		public float calculatedSize;
	}

	private static Vector2 leftRectScrollPos = Vector2.zero;

	private static bool warnedChangingXenotypeWillRandomizePawn = false;

	private static Rect highlightRect;

	private const float NonArchiteBaselinerChance = 0.5f;

	public const int MainRectsY = 100;

	private const float MainRectsHeight = 355f;

	private const int ConfigRectTitlesHeight = 40;

	private const int FactionIconSize = 22;

	private const int IdeoIconSize = 22;

	private const int GenderIconSize = 22;

	private const float RowHeight = 22f;

	private const float LeftRectHeight = 250f;

	private const float RightRectHeight = 258f;

	public static Vector2 BasePawnCardSize = new Vector2(480f, 455f);

	private static readonly Color FavColorBoxColor = new Color(0.25f, 0.25f, 0.25f);

	public const int MaxNameLength = 12;

	public const int MaxNickLength = 16;

	public const int MaxTitleLength = 25;

	public const int QuestLineHeight = 20;

	public const float RandomizeButtonWidth = 200f;

	public const float HighlightMargin = 6f;

	private static readonly Texture2D QuestIcon = ContentFinder<Texture2D>.Get("UI/Icons/Quest");

	private static readonly Texture2D UnrecruitableIcon = ContentFinder<Texture2D>.Get("UI/Icons/UnwaveringlyLoyal");

	public static readonly Color StackElementBackground = new Color(1f, 1f, 1f, 0.1f);

	public static List<CustomXenotype> cachedCustomXenotypes;

	private static List<ExtraFaction> tmpExtraFactions = new List<ExtraFaction>();

	private static readonly Color TitleCausedWorkTagDisableColor = new Color(0.67f, 0.84f, 0.9f);

	private static List<GenUI.AnonymousStackElement> tmpStackElements = new List<GenUI.AnonymousStackElement>();

	private static float tmpMaxElementStackHeight = 0f;

	private static StringBuilder tmpInspectStrings = new StringBuilder();

	public static Regex ValidNameRegex = new Regex("^[\\p{L}0-9 '\\-.]*$");

	private const int QuestIconSize = 24;

	private static List<CustomXenotype> CustomXenotypes
	{
		get
		{
			if (cachedCustomXenotypes == null)
			{
				cachedCustomXenotypes = new List<CustomXenotype>();
				foreach (FileInfo item in GenFilePaths.AllCustomXenotypeFiles.OrderBy((FileInfo f) => f.LastWriteTime))
				{
					string filePath = GenFilePaths.AbsFilePathForXenotype(Path.GetFileNameWithoutExtension(item.Name));
					PreLoadUtility.CheckVersionAndLoad(filePath, ScribeMetaHeaderUtility.ScribeHeaderMode.Xenotype, delegate
					{
						if (GameDataSaveLoader.TryLoadXenotype(filePath, out var xenotype))
						{
							cachedCustomXenotypes.Add(xenotype);
						}
					}, skipOnMismatch: true);
				}
			}
			return cachedCustomXenotypes;
		}
	}

	public static List<CustomXenotype> CustomXenotypesForReading => CustomXenotypes;

	public static void DrawCharacterCard(Rect rect, Pawn pawn, Action randomizeCallback = null, Rect creationRect = default(Rect), bool showName = true)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		bool flag = randomizeCallback != null;
		Rect rect2 = (flag ? creationRect : rect);
		Widgets.BeginGroup(rect2);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, 300f, (float)(showName ? 30 : 0));
		if (showName)
		{
			if (flag && pawn.Name is NameTriple nameTriple)
			{
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(val);
				((Rect)(ref rect3)).width = ((Rect)(ref rect3)).width * 0.333f;
				Rect rect4 = default(Rect);
				((Rect)(ref rect4))._002Ector(val);
				((Rect)(ref rect4)).width = ((Rect)(ref rect4)).width * 0.333f;
				((Rect)(ref rect4)).x = ((Rect)(ref rect4)).x + ((Rect)(ref rect4)).width;
				Rect rect5 = default(Rect);
				((Rect)(ref rect5))._002Ector(val);
				((Rect)(ref rect5)).width = ((Rect)(ref rect5)).width * 0.333f;
				((Rect)(ref rect5)).x = ((Rect)(ref rect5)).x + ((Rect)(ref rect4)).width * 2f;
				string name = nameTriple.First;
				string name2 = nameTriple.Nick;
				string name3 = nameTriple.Last;
				DoNameInputRect(rect3, ref name, 12);
				if (nameTriple.Nick == nameTriple.First || nameTriple.Nick == nameTriple.Last)
				{
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
				}
				DoNameInputRect(rect4, ref name2, 16);
				GUI.color = Color.white;
				DoNameInputRect(rect5, ref name3, 12);
				if (nameTriple.First != name || nameTriple.Nick != name2 || nameTriple.Last != name3)
				{
					pawn.Name = new NameTriple(name, string.IsNullOrEmpty(name2) ? name : name2, name3);
				}
				TooltipHandler.TipRegionByKey(rect3, "FirstNameDesc");
				TooltipHandler.TipRegionByKey(rect4, "ShortIdentifierDesc");
				TooltipHandler.TipRegionByKey(rect5, "LastNameDesc");
			}
			else
			{
				((Rect)(ref val)).width = 999f;
				Text.Font = GameFont.Medium;
				string text = pawn.Name.ToStringFull.CapitalizeFirst();
				Widgets.Label(val, text);
				if (pawn.guilt != null && pawn.guilt.IsGuilty)
				{
					float x = Text.CalcSize(text).x;
					Rect val2 = new Rect(x + 10f, 0f, 32f, 32f);
					GUI.DrawTexture(val2, (Texture)(object)TexUI.GuiltyTex);
					TooltipHandler.TipRegion(val2, () => pawn.guilt.Tip, 6321623);
				}
				Text.Font = GameFont.Small;
			}
		}
		bool allowsChildSelection = ScenarioUtility.AllowsChildSelection(Find.Scenario);
		if (ModsConfig.BiotechActive && flag)
		{
			Widgets.DrawHighlight(highlightRect.ExpandedBy(6f));
		}
		if (flag)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(((Rect)(ref creationRect)).width - 200f - 6f, 6f, 200f, ((Rect)(ref val)).height);
			if (Widgets.ButtonText(val3, "Randomize".Translate()))
			{
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				randomizeCallback();
			}
			UIHighlighter.HighlightOpportunity(val3, "RandomizePawn");
			if (ModsConfig.BiotechActive)
			{
				LifestageAndXenotypeOptions(pawn, val3, flag, allowsChildSelection, randomizeCallback);
			}
		}
		if (flag)
		{
			Widgets.InfoCardButton(((Rect)(ref val)).xMax + 4f, (((Rect)(ref val)).height - 24f) / 2f, pawn);
		}
		else if (!pawn.health.Dead)
		{
			float num = PawnCardSize(pawn).x - 85f;
			if (pawn.IsFreeColonist && pawn.Spawned && !pawn.IsQuestLodger() && showName)
			{
				Rect val4 = default(Rect);
				((Rect)(ref val4))._002Ector(num, 0f, 30f, 30f);
				if (Mouse.IsOver(val4))
				{
					TooltipHandler.TipRegion(val4, PawnBanishUtility.GetBanishButtonTip(pawn));
				}
				if (Widgets.ButtonImage(val4, TexButton.Banish))
				{
					if (pawn.Downed)
					{
						Messages.Message("MessageCantBanishDownedPawn".Translate(pawn.LabelShort, pawn).AdjustedFor(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
					}
					else
					{
						PawnBanishUtility.ShowBanishPawnConfirmationDialog(pawn);
					}
				}
				num -= 40f;
			}
			if ((pawn.IsColonist || pawn.IsColonySubhuman || DebugSettings.ShowDevGizmos) && showName)
			{
				Rect val5 = new Rect(num, 0f, 30f, 30f);
				TooltipHandler.TipRegionByKey(val5, "RenameColonist");
				if (Widgets.ButtonImage(val5, TexButton.Rename))
				{
					Find.WindowStack.Add(pawn.NamePawnDialog());
				}
				num -= 40f;
			}
			if (pawn.IsFreeColonist && !pawn.IsQuestLodger() && pawn.royalty != null && pawn.royalty.AllTitlesForReading.Count > 0)
			{
				Rect val6 = new Rect(num, 0f, 30f, 30f);
				TooltipHandler.TipRegionByKey(val6, "RenounceTitle");
				if (Widgets.ButtonImage(val6, TexButton.RenounceTitle))
				{
					FloatMenuUtility.MakeMenu(pawn.royalty.AllTitlesForReading, (RoyalTitle title) => "RenounceTitle".Translate() + ": " + "TitleOfFaction".Translate(title.def.GetLabelCapFor(pawn), title.faction.GetCallLabel()), delegate(RoyalTitle title)
					{
						return delegate
						{
							List<FactionPermit> list = pawn.royalty.PermitsFromFaction(title.faction);
							RoyalTitleUtility.FindLostAndGainedPermits(title.def, null, out var _, out var lostPermits);
							StringBuilder stringBuilder = new StringBuilder();
							if (lostPermits.Count > 0 || list.Count > 0)
							{
								stringBuilder.AppendLine("RenounceTitleWillLoosePermits".Translate(pawn.Named("PAWN")) + ":");
								foreach (RoyalTitlePermitDef item in lostPermits)
								{
									stringBuilder.AppendLine("- " + item.LabelCap + " (" + FirstTitleWithPermit(item).GetLabelFor(pawn) + ")");
								}
								foreach (FactionPermit item2 in list)
								{
									stringBuilder.AppendLine("- " + item2.Permit.LabelCap + " (" + item2.Title.GetLabelFor(pawn) + ")");
								}
								stringBuilder.AppendLine();
							}
							int permitPoints = pawn.royalty.GetPermitPoints(title.faction);
							if (permitPoints > 0)
							{
								stringBuilder.AppendLineTagged("RenounceTitleWillLosePermitPoints".Translate(pawn.Named("PAWN"), permitPoints.Named("POINTS"), title.faction.Named("FACTION")));
							}
							if (pawn.abilities.abilities.Any())
							{
								stringBuilder.AppendLine();
								stringBuilder.AppendLineTagged("RenounceTitleWillKeepPsylinkLevels".Translate(pawn.Named("PAWN")));
							}
							if (!title.faction.def.renounceTitleMessage.NullOrEmpty())
							{
								stringBuilder.AppendLine();
								stringBuilder.AppendLine(title.faction.def.renounceTitleMessage);
							}
							Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("RenounceTitleDescription".Translate(pawn.Named("PAWN"), "TitleOfFaction".Translate(title.def.GetLabelCapFor(pawn), title.faction.GetCallLabel()).Named("TITLE"), stringBuilder.ToString().TrimEndNewlines().Named("EFFECTS")), delegate
							{
								pawn.royalty.SetTitle(title.faction, null, grantRewards: false);
								pawn.royalty.ResetPermitsAndPoints(title.faction, title.def);
							}, destructive: true));
						};
						RoyalTitleDef FirstTitleWithPermit(RoyalTitlePermitDef permitDef)
						{
							return title.faction.def.RoyalTitlesAwardableInSeniorityOrderForReading.First((RoyalTitleDef t) => t.permits != null && t.permits.Contains(permitDef));
						}
					});
				}
				num -= 40f;
			}
			if (pawn.guilt != null && pawn.guilt.IsGuilty && pawn.IsFreeColonist && !pawn.IsQuestLodger())
			{
				Rect val7 = default(Rect);
				((Rect)(ref val7))._002Ector(num + 5f, 0f, 30f, 30f);
				TooltipHandler.TipRegionByKey(val7, "ExecuteColonist");
				if (Widgets.ButtonImage(val7, TexButton.ExecuteColonist))
				{
					pawn.guilt.awaitingExecution = !pawn.guilt.awaitingExecution;
					if (pawn.guilt.awaitingExecution)
					{
						Messages.Message("MessageColonistMarkedForExecution".Translate(pawn), pawn, MessageTypeDefOf.SilentInput, historical: false);
					}
				}
				if (pawn.guilt.awaitingExecution)
				{
					Rect val8 = default(Rect);
					((Rect)(ref val8)).x = ((Rect)(ref val8)).x + (((Rect)(ref val7)).x + 22f);
					((Rect)(ref val8)).width = 15f;
					((Rect)(ref val8)).height = 15f;
					GUI.DrawTexture(val8, (Texture)(object)Widgets.CheckboxOnTex);
				}
			}
		}
		float num2 = ((Rect)(ref val)).height + 10f;
		float num3 = num2;
		num2 = DoTopStack(pawn, rect, flag, num2);
		if (num2 - num3 < 78f)
		{
			num2 += 15f;
		}
		Rect leftRect = default(Rect);
		((Rect)(ref leftRect))._002Ector(0f, num2, 250f, ((Rect)(ref rect2)).height - num2);
		DoLeftSection(rect, leftRect, pawn);
		Rect val9 = default(Rect);
		((Rect)(ref val9))._002Ector(((Rect)(ref leftRect)).xMax, num2, 258f, ((Rect)(ref rect2)).height - num2);
		Widgets.BeginGroup(val9);
		SkillUI.DrawSkillsOf(mode: (Current.ProgramState != ProgramState.Playing) ? SkillUI.SkillDrawMode.Menu : SkillUI.SkillDrawMode.Gameplay, p: pawn, offset: Vector2.zero, container: val9);
		Widgets.EndGroup();
		Widgets.EndGroup();
	}

	private static string GetTitleTipString(Pawn pawn, Faction faction, RoyalTitle title, int favor)
	{
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		RoyalTitleDef def = title.def;
		TaggedString taggedString = "RoyalTitleTooltipHasTitle".Translate(pawn.Named("PAWN"), faction.Named("FACTION"), def.GetLabelCapFor(pawn).Named("TITLE"));
		taggedString += "\n\n" + faction.def.royalFavorLabel.CapitalizeFirst() + ": " + favor;
		RoyalTitleDef nextTitle = def.GetNextTitle(faction);
		if (nextTitle != null)
		{
			taggedString += "\n" + "RoyalTitleTooltipNextTitle".Translate() + ": " + nextTitle.GetLabelCapFor(pawn) + " (" + "RoyalTitleTooltipNextTitleFavorCost".Translate(nextTitle.favorCost.ToString(), faction.Named("FACTION")) + ")";
		}
		else
		{
			taggedString += "\n" + "RoyalTitleTooltipFinalTitle".Translate();
		}
		if (title.def.canBeInherited)
		{
			Pawn heir = pawn.royalty.GetHeir(faction);
			if (heir != null)
			{
				taggedString += "\n\n" + "RoyalTitleTooltipInheritance".Translate(pawn.Named("PAWN"), heir.Named("HEIR"));
				if (heir.Faction == null)
				{
					taggedString += " " + "RoyalTitleTooltipHeirNoFaction".Translate(heir.Named("HEIR"));
				}
				else if (heir.Faction != faction)
				{
					taggedString += " " + "RoyalTitleTooltipHeirDifferentFaction".Translate(heir.Named("HEIR"), heir.Faction.Named("FACTION"));
				}
			}
			else
			{
				taggedString += "\n\n" + "RoyalTitleTooltipNoHeir".Translate(pawn.Named("PAWN"));
			}
		}
		else
		{
			taggedString += "\n\n" + "LetterRoyalTitleCantBeInherited".Translate(title.def.Named("TITLE")).CapitalizeFirst() + " " + "LetterRoyalTitleNoHeir".Translate(pawn.Named("PAWN"));
		}
		taggedString += "\n\n" + (title.conceited ? "RoyalTitleTooltipConceited" : "RoyalTitleTooltipNonConceited").Translate(pawn.Named("PAWN"));
		taggedString += "\n\n" + RoyalTitleUtility.GetTitleProgressionInfo(faction, pawn);
		return (taggedString + ("\n\n" + "ClickToLearnMore".Translate().Colorize(ColoredText.SubtleGrayColor))).Resolve();
	}

	private static List<object> GetWorkTypeDisableCauses(Pawn pawn, WorkTags workTag)
	{
		List<object> list = new List<object>();
		if (pawn.story != null && pawn.story.Childhood != null && (pawn.story.Childhood.workDisables & workTag) != 0)
		{
			list.Add(pawn.story.Childhood);
		}
		if (pawn.story != null && pawn.story.Adulthood != null && (pawn.story.Adulthood.workDisables & workTag) != 0)
		{
			list.Add(pawn.story.Adulthood);
		}
		if (pawn.health != null && pawn.health.hediffSet != null)
		{
			foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
			{
				HediffStage curStage = hediff.CurStage;
				if (curStage != null && (curStage.disabledWorkTags & workTag) != 0)
				{
					list.Add(hediff);
				}
			}
		}
		if (pawn.story.traits != null)
		{
			for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
			{
				if (!pawn.story.traits.allTraits[i].Suppressed)
				{
					Trait trait = pawn.story.traits.allTraits[i];
					if ((trait.def.disabledWorkTags & workTag) != 0)
					{
						list.Add(trait);
					}
				}
			}
		}
		if (pawn.royalty != null)
		{
			foreach (RoyalTitle item in pawn.royalty.AllTitlesForReading)
			{
				if (item.conceited && (item.def.disabledWorkTags & workTag) != 0)
				{
					list.Add(item);
				}
			}
		}
		if (ModsConfig.IdeologyActive && pawn.Ideo != null)
		{
			Precept_Role role = pawn.Ideo.GetRole(pawn);
			if (role != null && (role.def.roleDisabledWorkTags & workTag) != 0)
			{
				list.Add(role);
			}
		}
		if (ModsConfig.BiotechActive && pawn.genes != null)
		{
			foreach (Gene item2 in pawn.genes.GenesListForReading)
			{
				if (item2.Active && (item2.def.disabledWorkTags & workTag) != 0)
				{
					list.Add(item2);
				}
			}
		}
		if (ModsConfig.AnomalyActive && pawn.IsMutant && pawn.mutant.IsPassive)
		{
			list.Add(pawn.mutant.Def);
		}
		foreach (QuestPart_WorkDisabled item3 in QuestUtility.GetWorkDisabledQuestPart(pawn))
		{
			if ((item3.disabledWorkTags & workTag) != 0 && !list.Contains(item3.quest))
			{
				list.Add(item3.quest);
			}
		}
		return list;
	}

	private static Color GetDisabledWorkTagLabelColor(Pawn pawn, WorkTags workTag)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		foreach (object workTypeDisableCause in GetWorkTypeDisableCauses(pawn, workTag))
		{
			if (workTypeDisableCause is RoyalTitleDef)
			{
				return TitleCausedWorkTagDisableColor;
			}
		}
		return Color.white;
	}

	private static void LifestageAndXenotypeOptions(Pawn pawn, Rect randomizeRect, bool creationMode, bool allowsChildSelection, Action randomizeCallback)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		highlightRect = randomizeRect;
		((Rect)(ref highlightRect)).yMax = ((Rect)(ref highlightRect)).yMax + (((Rect)(ref randomizeRect)).height + Text.LineHeight + 8f);
		int startingPawnIndex = StartingPawnUtility.PawnIndex(pawn);
		float num = (((Rect)(ref randomizeRect)).width - 4f) / 2f;
		float x2 = ((Rect)(ref randomizeRect)).x;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(x2, ((Rect)(ref randomizeRect)).y + ((Rect)(ref randomizeRect)).height + 4f, num, ((Rect)(ref randomizeRect)).height);
		x2 += ((Rect)(ref val)).width + 4f;
		Text.Anchor = (TextAnchor)4;
		Rect rect = val;
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + (((Rect)(ref val)).height + 4f);
		((Rect)(ref rect)).height = Text.LineHeight;
		Widgets.Label(rect, pawn.DevelopmentalStage.ToString().Translate().CapitalizeFirst());
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref rect)).yMax - ((Rect)(ref val)).yMin);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
			if (Find.WindowStack.FloatMenu == null)
			{
				TaggedString taggedString = GetLabel().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "DevelopmentalAgeSelectionDesc".Translate();
				if (!allowsChildSelection)
				{
					taggedString += "\n\n" + "MessageDevelopmentalStageSelectionDisabledByScenario".Translate().Colorize(ColorLibrary.RedReadable);
				}
				TooltipHandler.TipRegion(rect2, taggedString.Resolve());
			}
		}
		DevelopmentalStage developmentalStage = DevelopmentalStage.Baby | DevelopmentalStage.Child | DevelopmentalStage.Adult;
		if (ModsConfig.AnomalyActive && pawn.IsMutant)
		{
			developmentalStage = pawn.mutant.Def.allowedDevelopmentalStages;
		}
		if (Widgets.ButtonImageWithBG(val, GetDevelopmentalStageIcon(), (Vector2?)new Vector2(22f, 22f)) && TutorSystem.AllowAction("ChangeDevelopmentStage"))
		{
			if (allowsChildSelection)
			{
				int index2 = startingPawnIndex;
				PawnGenerationRequest existing2 = StartingPawnUtility.GetGenerationRequest(index2);
				List<FloatMenuOption> options = new List<FloatMenuOption>
				{
					new FloatMenuOption("Adult".Translate().CapitalizeFirst(), (!developmentalStage.Has(DevelopmentalStage.Adult)) ? null : ((Action)delegate
					{
						if (!existing2.AllowedDevelopmentalStages.Has(DevelopmentalStage.Adult))
						{
							existing2.AllowedDevelopmentalStages = DevelopmentalStage.Adult;
							existing2.AllowDowned = false;
							StartingPawnUtility.SetGenerationRequest(index2, existing2);
							randomizeCallback();
						}
					}), DevelopmentalStageExtensions.AdultTex.Texture, Color.white),
					new FloatMenuOption("Child".Translate().CapitalizeFirst(), (!developmentalStage.Has(DevelopmentalStage.Child)) ? null : ((Action)delegate
					{
						if (!existing2.AllowedDevelopmentalStages.Has(DevelopmentalStage.Child))
						{
							existing2.AllowedDevelopmentalStages = DevelopmentalStage.Child;
							existing2.AllowDowned = false;
							StartingPawnUtility.SetGenerationRequest(index2, existing2);
							randomizeCallback();
						}
					}), DevelopmentalStageExtensions.ChildTex.Texture, Color.white),
					new FloatMenuOption("Baby".Translate().CapitalizeFirst(), (!developmentalStage.Has(DevelopmentalStage.Baby)) ? null : ((Action)delegate
					{
						if (!existing2.AllowedDevelopmentalStages.Has(DevelopmentalStage.Baby))
						{
							existing2.AllowedDevelopmentalStages = DevelopmentalStage.Baby;
							existing2.AllowDowned = true;
							StartingPawnUtility.SetGenerationRequest(index2, existing2);
							randomizeCallback();
						}
					}), DevelopmentalStageExtensions.BabyTex.Texture, Color.white)
				};
				Find.WindowStack.Add(new FloatMenu(options));
			}
			else
			{
				Messages.Message("MessageDevelopmentalStageSelectionDisabledByScenario".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(x2, ((Rect)(ref randomizeRect)).y + ((Rect)(ref randomizeRect)).height + 4f, num, ((Rect)(ref randomizeRect)).height);
		Text.Anchor = (TextAnchor)4;
		Rect rect3 = val2;
		((Rect)(ref rect3)).y = ((Rect)(ref rect3)).y + (((Rect)(ref val2)).height + 4f);
		((Rect)(ref rect3)).height = Text.LineHeight;
		Widgets.Label(rect3, GetXenotypeLabel(startingPawnIndex).Truncate(((Rect)(ref rect3)).width));
		Text.Anchor = (TextAnchor)0;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, ((Rect)(ref rect3)).yMax - ((Rect)(ref val2)).yMin);
		if (Mouse.IsOver(rect4))
		{
			Widgets.DrawHighlight(rect4);
			if (Find.WindowStack.FloatMenu == null)
			{
				TooltipHandler.TipRegion(rect4, GetXenotypeLabel(startingPawnIndex).Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "XenotypeSelectionDesc".Translate());
			}
		}
		if (!Widgets.ButtonImageWithBG(val2, GetXenotypeIcon(startingPawnIndex), (Vector2?)new Vector2(22f, 22f)) || !TutorSystem.AllowAction("ChangeXenotype"))
		{
			return;
		}
		int index = startingPawnIndex;
		List<FloatMenuOption> list = new List<FloatMenuOption>
		{
			new FloatMenuOption("AnyNonArchite".Translate().CapitalizeFirst(), delegate
			{
				List<XenotypeDef> allowedXenotypes = DefDatabase<XenotypeDef>.AllDefs.Where((XenotypeDef x) => !x.Archite && x != XenotypeDefOf.Baseliner).ToList();
				SetupGenerationRequest(index, null, null, allowedXenotypes, 0.5f, (PawnGenerationRequest existing) => existing.ForcedXenotype != null || existing.ForcedCustomXenotype != null, randomizeCallback, randomize: false);
			}),
			new FloatMenuOption("XenotypeEditor".Translate() + "...", delegate
			{
				Find.WindowStack.Add(new Dialog_CreateXenotype(index, delegate
				{
					cachedCustomXenotypes = null;
					randomizeCallback();
				}));
			})
		};
		foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefs.OrderBy((XenotypeDef x) => 0f - x.displayPriority))
		{
			XenotypeDef xenotype2 = item;
			list.Add(new FloatMenuOption(xenotype2.LabelCap, delegate
			{
				SetupGenerationRequest(index, xenotype2, null, null, 0f, (PawnGenerationRequest existing) => XenotypeValidator(existing, xenotype2), randomizeCallback);
			}, xenotype2.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate(Rect r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				TooltipHandler.TipRegion(r, xenotype2.descriptionShort ?? xenotype2.description);
			}, null, 24f, (Rect r) => Widgets.InfoCardButton(((Rect)(ref r)).x, ((Rect)(ref r)).y + 3f, xenotype2) ? true : false, null, playSelectionSound: true, 0, HorizontalJustification.Left, extraPartRightJustified: true));
		}
		foreach (CustomXenotype customXenotype in CustomXenotypes)
		{
			CustomXenotype customInner = customXenotype;
			list.Add(new FloatMenuOption(customInner.name.CapitalizeFirst() + " (" + "Custom".Translate() + ")", delegate
			{
				SetupGenerationRequest(index, null, customInner, null, 0f, (PawnGenerationRequest existing) => CustomXenotypeValidator(existing, customInner), randomizeCallback);
			}, customInner.IconDef.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, null, null, 24f, delegate(Rect r)
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				if (Widgets.ButtonImage(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + (((Rect)(ref r)).height - ((Rect)(ref r)).width) / 2f, ((Rect)(ref r)).width, ((Rect)(ref r)).width), TexButton.Delete, GUI.color))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(customInner.name.CapitalizeFirst()), delegate
					{
						string path = GenFilePaths.AbsFilePathForXenotype(customInner.name);
						if (File.Exists(path))
						{
							File.Delete(path);
							cachedCustomXenotypes = null;
						}
					}, destructive: true));
					return true;
				}
				return false;
			}, null, playSelectionSound: true, 0, HorizontalJustification.Left, extraPartRightJustified: true));
		}
		Find.WindowStack.Add(new FloatMenu(list));
		static bool CustomXenotypeValidator(PawnGenerationRequest req, CustomXenotype xenotype)
		{
			if (TutorSystem.TutorialMode && req.MustBeCapableOfViolence && xenotype.genes.Any((GeneDef g) => g.disabledWorkTags.HasFlag(WorkTags.Violent)))
			{
				Messages.Message("MessageStartingPawnCapableOfViolence".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return req.ForcedCustomXenotype != xenotype;
		}
		Texture2D GetDevelopmentalStageIcon()
		{
			return StartingPawnUtility.GetGenerationRequest(startingPawnIndex).AllowedDevelopmentalStages.Icon().Texture;
		}
		string GetLabel()
		{
			PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(startingPawnIndex);
			if (generationRequest.AllowedDevelopmentalStages.Has(DevelopmentalStage.Baby))
			{
				return "Baby".Translate();
			}
			if (generationRequest.AllowedDevelopmentalStages.Has(DevelopmentalStage.Child))
			{
				return "Child".Translate();
			}
			return "Adult".Translate();
		}
		static bool XenotypeValidator(PawnGenerationRequest req, XenotypeDef xenotype)
		{
			if (TutorSystem.TutorialMode && req.MustBeCapableOfViolence && xenotype.AllGenes.Any((GeneDef g) => g.disabledWorkTags.HasFlag(WorkTags.Violent)))
			{
				Messages.Message("MessageStartingPawnCapableOfViolence".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
			return req.ForcedXenotype != xenotype;
		}
	}

	private static void SetupGenerationRequest(int index, XenotypeDef xenotype, CustomXenotype customXenotype, List<XenotypeDef> allowedXenotypes, float forceBaselinerChance, Func<PawnGenerationRequest, bool> validator, Action randomizeCallback, bool randomize = true)
	{
		PawnGenerationRequest existing = StartingPawnUtility.GetGenerationRequest(index);
		if (!validator(existing))
		{
			return;
		}
		if (!warnedChangingXenotypeWillRandomizePawn && randomize)
		{
			Find.WindowStack.Add(new Dialog_MessageBox("WarnChangingXenotypeWillRandomizePawn".Translate(), "Yes".Translate(), delegate
			{
				warnedChangingXenotypeWillRandomizePawn = true;
				existing.ForcedXenotype = xenotype;
				existing.ForcedCustomXenotype = customXenotype;
				existing.AllowedXenotypes = allowedXenotypes;
				existing.ForceBaselinerChance = forceBaselinerChance;
				StartingPawnUtility.SetGenerationRequest(index, existing);
				randomizeCallback();
			}, "No".Translate()));
		}
		else
		{
			existing.ForcedXenotype = xenotype;
			existing.ForcedCustomXenotype = customXenotype;
			existing.AllowedXenotypes = allowedXenotypes;
			existing.ForceBaselinerChance = forceBaselinerChance;
			StartingPawnUtility.SetGenerationRequest(index, existing);
			if (randomize)
			{
				randomizeCallback();
			}
		}
	}

	private static string GetXenotypeLabel(int startingPawnIndex)
	{
		PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(startingPawnIndex);
		if (generationRequest.ForcedCustomXenotype != null)
		{
			return generationRequest.ForcedCustomXenotype.name.CapitalizeFirst();
		}
		if (generationRequest.ForcedXenotype != null)
		{
			return generationRequest.ForcedXenotype.LabelCap;
		}
		return "AnyLower".Translate().CapitalizeFirst();
	}

	private static Texture2D GetXenotypeIcon(int startingPawnIndex)
	{
		PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(startingPawnIndex);
		if (generationRequest.ForcedXenotype != null)
		{
			return generationRequest.ForcedXenotype.Icon;
		}
		if (generationRequest.ForcedCustomXenotype != null)
		{
			return generationRequest.ForcedCustomXenotype.IconDef.Icon;
		}
		return GeneUtility.UniqueXenotypeTex.Texture;
	}

	private static float DoTopStack(Pawn pawn, Rect rect, bool creationMode, float curY)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		tmpStackElements.Clear();
		float num = ((Rect)(ref rect)).width - 10f;
		float num2 = (creationMode ? (num - 20f - Page_ConfigureStartingPawns.PawnPortraitSize.x) : num);
		Text.Font = GameFont.Small;
		bool flag = ModsConfig.BiotechActive && creationMode;
		string mainDesc = pawn.MainDesc(writeFaction: false, !flag);
		if (flag)
		{
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					GUI.DrawTexture(r, (Texture)(object)pawn.gender.GetIcon());
					if (Mouse.IsOver(r))
					{
						TooltipHandler.TipRegion(r, () => pawn.gender.GetLabel(pawn.AnimalOrWildMan()).CapitalizeFirst(), 7594764);
					}
				},
				width = 22f
			});
		}
		tmpStackElements.Add(new GenUI.AnonymousStackElement
		{
			drawer = delegate(Rect r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				Widgets.Label(r, mainDesc);
				if (Mouse.IsOver(r))
				{
					TooltipHandler.TipRegion(r, () => pawn.ageTracker.AgeTooltipString, 6873641);
				}
			},
			width = Text.CalcSize(mainDesc).x + 5f
		});
		Rect val;
		if (ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.GenesListForReading.Any())
		{
			float num3 = 22f;
			num3 += Text.CalcSize(pawn.genes.XenotypeLabelCap).x + 14f;
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					//IL_0042: Unknown result type (might be due to invalid IL or missing references)
					//IL_0074: Unknown result type (might be due to invalid IL or missing references)
					//IL_0079: Unknown result type (might be due to invalid IL or missing references)
					//IL_0098: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0122: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
					Rect val15 = default(Rect);
					((Rect)(ref val15))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
					GUI.color = StackElementBackground;
					GUI.DrawTexture(val15, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = Color.white;
					if (Mouse.IsOver(val15))
					{
						Widgets.DrawHighlight(val15);
					}
					Rect val16 = new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, 20f, 20f);
					GUI.color = XenotypeDef.IconColor;
					GUI.DrawTexture(val16, (Texture)(object)pawn.genes.XenotypeIcon);
					GUI.color = Color.white;
					Widgets.Label(new Rect(((Rect)(ref r)).x + 22f + 5f, ((Rect)(ref r)).y, ((Rect)(ref r)).width + 22f - 1f, ((Rect)(ref r)).height), pawn.genes.XenotypeLabelCap);
					if (Mouse.IsOver(r))
					{
						TooltipHandler.TipRegion(r, () => ("Xenotype".Translate() + ": " + pawn.genes.XenotypeLabelCap).Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + pawn.genes.XenotypeDescShort + "\n\n" + "ViewGenesDesc".Translate(pawn.Named("PAWN")).ToString().StripTags()
							.Colorize(ColoredText.SubtleGrayColor), 883938493);
					}
					if (Widgets.ButtonInvisible(r))
					{
						if (Current.ProgramState == ProgramState.Playing && Find.WindowStack.WindowOfType<Dialog_InfoCard>() == null && Find.WindowStack.WindowOfType<Dialog_GrowthMomentChoices>() == null)
						{
							InspectPaneUtility.OpenTab(typeof(ITab_Genes));
						}
						else
						{
							Find.WindowStack.Add(new Dialog_ViewGenes(pawn));
						}
					}
				},
				width = num3
			});
			float num4 = curY;
			val = GenUI.DrawElementStack(new Rect(0f, curY, num2, 50f), 22f, tmpStackElements, delegate(Rect r, GenUI.AnonymousStackElement obj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				obj.drawer(r);
			}, (GenUI.AnonymousStackElement obj) => obj.width, 4f, 5f, allowOrderOptimization: false);
			curY = num4 + (((Rect)(ref val)).height + 4f);
			tmpStackElements.Clear();
		}
		if (pawn.Faction != null && !pawn.Faction.Hidden)
		{
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0044: Unknown result type (might be due to invalid IL or missing references)
					//IL_0091: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
					//IL_0115: Unknown result type (might be due to invalid IL or missing references)
					//IL_0153: Unknown result type (might be due to invalid IL or missing references)
					//IL_0170: Unknown result type (might be due to invalid IL or missing references)
					//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
					//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
					Rect val12 = default(Rect);
					((Rect)(ref val12))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
					Color color6 = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(val12, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = color6;
					Widgets.DrawHighlightIfMouseover(val12);
					Rect val13 = default(Rect);
					((Rect)(ref val13))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
					Rect val14 = new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, 20f, 20f);
					GUI.color = pawn.Faction.Color;
					GUI.DrawTexture(val14, (Texture)(object)pawn.Faction.def.FactionIcon);
					GUI.color = color6;
					Widgets.Label(new Rect(((Rect)(ref val13)).x + ((Rect)(ref val13)).height + 5f, ((Rect)(ref val13)).y, ((Rect)(ref val13)).width - 10f, ((Rect)(ref val13)).height), pawn.Faction.Name);
					if (Widgets.ButtonInvisible(val12))
					{
						if (creationMode || Find.WindowStack.AnyWindowAbsorbingAllInput)
						{
							Find.WindowStack.Add(new Dialog_FactionDuringLanding());
						}
						else
						{
							Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Factions);
						}
					}
					if (Mouse.IsOver(val12))
					{
						string text = "Faction".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "FactionDesc".Translate(pawn.Named("PAWN")).Resolve() + "\n\n" + "ClickToViewFactions".Translate().Colorize(ColoredText.SubtleGrayColor);
						TooltipHandler.TipRegion(tip: new TipSignal(text, pawn.Faction.loadID * 37), rect: val12);
					}
				},
				width = Text.CalcSize(pawn.Faction.Name).x + 22f + 15f
			});
		}
		tmpExtraFactions.Clear();
		QuestUtility.GetExtraFactionsFromQuestParts(pawn, tmpExtraFactions);
		GuestUtility.GetExtraFactionsFromGuestStatus(pawn, tmpExtraFactions);
		foreach (ExtraFaction tmpExtraFaction in tmpExtraFactions)
		{
			if (pawn.Faction == tmpExtraFaction.faction)
			{
				continue;
			}
			ExtraFaction localExtraFaction = tmpExtraFaction;
			string factionName = localExtraFaction.faction.Name;
			bool drawExtraFactionIcon = localExtraFaction.factionType == ExtraFactionType.HomeFaction || localExtraFaction.factionType == ExtraFactionType.MiniFaction;
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_002e: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_0040: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0051: Unknown result type (might be due to invalid IL or missing references)
					//IL_014d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
					//IL_0113: Unknown result type (might be due to invalid IL or missing references)
					//IL_015d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0176: Unknown result type (might be due to invalid IL or missing references)
					//IL_0196: Unknown result type (might be due to invalid IL or missing references)
					//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
					//IL_0217: Unknown result type (might be due to invalid IL or missing references)
					Rect val8 = default(Rect);
					((Rect)(ref val8))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
					Rect val9 = (drawExtraFactionIcon ? val8 : r);
					Color color5 = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(val9, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = color5;
					Widgets.DrawHighlightIfMouseover(val9);
					if (drawExtraFactionIcon)
					{
						Rect val10 = default(Rect);
						((Rect)(ref val10))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
						Rect val11 = new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, 20f, 20f);
						GUI.color = localExtraFaction.faction.Color;
						GUI.DrawTexture(val11, (Texture)(object)localExtraFaction.faction.def.FactionIcon);
						GUI.color = color5;
						Widgets.Label(new Rect(((Rect)(ref val10)).x + ((Rect)(ref val10)).height + 5f, ((Rect)(ref val10)).y, ((Rect)(ref val10)).width - 10f, ((Rect)(ref val10)).height), factionName);
					}
					else
					{
						Widgets.Label(new Rect(((Rect)(ref r)).x + 5f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 10f, ((Rect)(ref r)).height), factionName);
					}
					if (Widgets.ButtonInvisible(val8))
					{
						Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Factions);
					}
					if (Mouse.IsOver(val9))
					{
						TipSignal tip3 = new TipSignal((localExtraFaction.factionType.GetLabel().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "ExtraFactionDesc".Translate(pawn.Named("PAWN")) + "\n\n" + "ClickToViewFactions".Translate().Colorize(ColoredText.SubtleGrayColor)).Resolve(), localExtraFaction.faction.loadID ^ 0x738AC053);
						TooltipHandler.TipRegion(val9, tip3);
					}
				},
				width = Text.CalcSize(factionName).x + (float)(drawExtraFactionIcon ? 22 : 0) + 15f
			});
		}
		if (!Find.IdeoManager.classicMode && pawn.Ideo != null && ModsConfig.IdeologyActive)
		{
			float width = Text.CalcSize(pawn.Ideo.name).x + 22f + 15f;
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					GUI.color = StackElementBackground;
					GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = Color.white;
					IdeoUIUtility.DrawIdeoPlate(r, pawn.Ideo, pawn);
				},
				width = width
			});
		}
		if (ModsConfig.IdeologyActive)
		{
			Precept_Role role = pawn.Ideo?.GetRole(pawn);
			if (role != null)
			{
				string roleLabel = role.LabelForPawn(pawn);
				tmpStackElements.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_0028: Unknown result type (might be due to invalid IL or missing references)
						//IL_0032: Unknown result type (might be due to invalid IL or missing references)
						//IL_0042: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
						//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
						//IL_0114: Unknown result type (might be due to invalid IL or missing references)
						//IL_0124: Unknown result type (might be due to invalid IL or missing references)
						//IL_004a: Unknown result type (might be due to invalid IL or missing references)
						//IL_013d: Unknown result type (might be due to invalid IL or missing references)
						//IL_018e: Unknown result type (might be due to invalid IL or missing references)
						Color color4 = GUI.color;
						Rect val5 = default(Rect);
						((Rect)(ref val5))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
						GUI.color = StackElementBackground;
						GUI.DrawTexture(val5, (Texture)(object)BaseContent.WhiteTex);
						GUI.color = color4;
						if (Mouse.IsOver(val5))
						{
							Widgets.DrawHighlight(val5);
						}
						Rect val6 = default(Rect);
						((Rect)(ref val6))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width + 22f + 9f, ((Rect)(ref r)).height);
						Rect val7 = new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, 20f, 20f);
						GUI.color = pawn.Ideo.Color;
						GUI.DrawTexture(val7, (Texture)(object)role.Icon);
						GUI.color = Color.white;
						Widgets.Label(new Rect(((Rect)(ref val6)).x + 22f + 5f, ((Rect)(ref val6)).y, ((Rect)(ref val6)).width - 10f, ((Rect)(ref val6)).height), roleLabel);
						if (Widgets.ButtonInvisible(val5))
						{
							InspectPaneUtility.OpenTab(typeof(ITab_Pawn_Social));
						}
						if (Mouse.IsOver(val5))
						{
							TipSignal tip2 = new TipSignal(() => role.GetTip(), (int)curY * 39);
							TooltipHandler.TipRegion(val5, tip2);
						}
					},
					width = Text.CalcSize(roleLabel).x + 22f + 14f
				});
			}
		}
		int count;
		if (pawn.royalty != null && pawn.royalty.AllTitlesInEffectForReading.Count > 0)
		{
			foreach (RoyalTitle title in pawn.royalty.AllTitlesInEffectForReading)
			{
				RoyalTitle localTitle = title;
				string labelCapFor = localTitle.def.GetLabelCapFor(pawn);
				count = pawn.royalty.GetFavor(localTitle.faction);
				string titleLabel = labelCapFor + " (" + count + ")";
				tmpStackElements.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_000d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0035: Unknown result type (might be due to invalid IL or missing references)
						//IL_003f: Unknown result type (might be due to invalid IL or missing references)
						//IL_004a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0076: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
						//IL_013e: Unknown result type (might be due to invalid IL or missing references)
						//IL_014e: Unknown result type (might be due to invalid IL or missing references)
						//IL_007e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0187: Unknown result type (might be due to invalid IL or missing references)
						//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
						Color color3 = GUI.color;
						Rect val2 = default(Rect);
						((Rect)(ref val2))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
						GUI.color = StackElementBackground;
						GUI.DrawTexture(val2, (Texture)(object)BaseContent.WhiteTex);
						GUI.color = color3;
						int favor = pawn.royalty.GetFavor(localTitle.faction);
						if (Mouse.IsOver(val2))
						{
							Widgets.DrawHighlight(val2);
						}
						Rect val3 = default(Rect);
						((Rect)(ref val3))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width + 22f + 9f, ((Rect)(ref r)).height);
						Rect val4 = new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, 20f, 20f);
						GUI.color = title.faction.Color;
						GUI.DrawTexture(val4, (Texture)(object)localTitle.faction.def.FactionIcon);
						GUI.color = color3;
						Widgets.Label(new Rect(((Rect)(ref val3)).x + 22f + 5f, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width - 10f, ((Rect)(ref val3)).height), titleLabel);
						if (Widgets.ButtonInvisible(val2))
						{
							Find.WindowStack.Add(new Dialog_InfoCard(localTitle.def, localTitle.faction, pawn));
						}
						if (Mouse.IsOver(val2))
						{
							TooltipHandler.TipRegion(tip: new TipSignal(() => GetTitleTipString(pawn, localTitle.faction, localTitle, favor), (int)curY * 37), rect: val2);
						}
					},
					width = Text.CalcSize(titleLabel).x + 22f + 14f
				});
			}
		}
		if (ModsConfig.IdeologyActive && !pawn.DevelopmentalStage.Baby() && pawn.story?.favoriteColor != null)
		{
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_005c: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0078: Unknown result type (might be due to invalid IL or missing references)
					//IL_0082: Unknown result type (might be due to invalid IL or missing references)
					//IL_008a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0094: Unknown result type (might be due to invalid IL or missing references)
					string orIdeoColor = string.Empty;
					if (pawn.Ideo != null && !pawn.Ideo.classicMode)
					{
						orIdeoColor = "OrIdeoColor".Translate(pawn.Named("PAWN"));
					}
					Widgets.DrawRectFast(r, pawn.story.favoriteColor.color);
					GUI.color = FavColorBoxColor;
					Widgets.DrawBox(r);
					GUI.color = Color.white;
					TooltipHandler.TipRegion(r, () => "FavoriteColorTooltip".Translate(pawn.Named("PAWN"), pawn.story.favoriteColor.label.Named("COLOR"), 0.6f.ToStringPercent().Named("PERCENTAGE"), orIdeoColor.Named("ORIDEO")).Resolve(), 837472764);
				},
				width = 22f
			});
		}
		if (pawn.guest != null && !pawn.guest.Recruitable)
		{
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					Color color2 = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = color2;
					GUI.DrawTexture(r, (Texture)(object)UnrecruitableIcon);
					if (Mouse.IsOver(r))
					{
						Widgets.DrawHighlight(r);
						TooltipHandler.TipRegion(r, () => "Unrecruitable".Translate().AsTipTitle().CapitalizeFirst() + "\n\n" + "UnrecruitableDesc".Translate(pawn.Named("PAWN")).Resolve(), 15877733);
					}
				},
				width = 22f
			});
		}
		bool drawMinimized = tmpMaxElementStackHeight > 44f;
		QuestUtility.AppendInspectStringsFromQuestParts(delegate(string str, Quest quest)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			tmpStackElements.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					Color color = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
					GUI.color = color;
					DoQuestLine(r, str, quest, drawMinimized);
				},
				width = GetQuestLineSize(str, quest, drawMinimized).x
			});
		}, pawn, out count);
		val = GenUI.DrawElementStack(new Rect(0f, curY, num2, 50f), 22f, tmpStackElements, delegate(Rect r, GenUI.AnonymousStackElement obj)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			obj.drawer(r);
		}, (GenUI.AnonymousStackElement obj) => obj.width, 4f, 5f, allowOrderOptimization: false);
		float height = ((Rect)(ref val)).height;
		tmpMaxElementStackHeight = Mathf.Max(height, tmpMaxElementStackHeight);
		curY += height;
		if (tmpStackElements.Any())
		{
			curY += 10f;
		}
		return curY;
	}

	private static void DoLeftSection(Rect rect, Rect leftRect, Pawn pawn)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(leftRect);
		float num = 0f;
		Pawn pawnLocal = pawn;
		List<Ability> abilities = (from a in pawn.abilities.AllAbilitiesForReading
			where a.def.showOnCharacterCard
			orderby a.def.level, a.def.EntropyGain
			select a).ToList();
		int numSections = (abilities.Any() ? 5 : 4);
		float num2 = (float)Enum.GetValues(typeof(BackstorySlot)).Length * 22f;
		float stackHeight = 0f;
		if (pawn.story != null && pawn.story.title != null)
		{
			num2 += 22f;
		}
		List<LeftRectSection> list = new List<LeftRectSection>();
		list.Add(new LeftRectSection
		{
			rect = new Rect(0f, 0f, ((Rect)(ref leftRect)).width, num2),
			drawer = delegate(Rect sectionRect)
			{
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				float num8 = ((Rect)(ref sectionRect)).y;
				Text.Font = GameFont.Small;
				Rect val5 = default(Rect);
				Rect val6 = default(Rect);
				foreach (BackstorySlot value6 in Enum.GetValues(typeof(BackstorySlot)))
				{
					BackstoryDef backstory = pawn.story.GetBackstory(value6);
					if (backstory != null)
					{
						((Rect)(ref val5))._002Ector(((Rect)(ref sectionRect)).x, num8, ((Rect)(ref leftRect)).width, 22f);
						Text.Anchor = (TextAnchor)3;
						Widgets.Label(val5, (value6 == BackstorySlot.Adulthood) ? "Adulthood".Translate() : "Childhood".Translate());
						Text.Anchor = (TextAnchor)0;
						string text = backstory.TitleCapFor(pawn.gender);
						((Rect)(ref val6))._002Ector(val5);
						((Rect)(ref val6)).x = ((Rect)(ref val6)).x + 90f;
						((Rect)(ref val6)).width = Text.CalcSize(text).x + 10f;
						Color color4 = GUI.color;
						GUI.color = StackElementBackground;
						GUI.DrawTexture(val6, (Texture)(object)BaseContent.WhiteTex);
						GUI.color = color4;
						Text.Anchor = (TextAnchor)4;
						Widgets.Label(val6, text.Truncate(((Rect)(ref val6)).width));
						Text.Anchor = (TextAnchor)0;
						if (Mouse.IsOver(val6))
						{
							Widgets.DrawHighlight(val6);
						}
						if (Mouse.IsOver(val6))
						{
							TooltipHandler.TipRegion(val6, backstory.FullDescriptionFor(pawn).Resolve());
						}
						num8 += ((Rect)(ref val5)).height + 4f;
					}
				}
				if (pawn.story != null && pawn.story.title != null)
				{
					Rect val7 = default(Rect);
					((Rect)(ref val7))._002Ector(((Rect)(ref sectionRect)).x, num8, ((Rect)(ref leftRect)).width, 22f);
					Text.Anchor = (TextAnchor)3;
					Widgets.Label(val7, "BackstoryTitle".Translate() + ":");
					Text.Anchor = (TextAnchor)0;
					Rect rect4 = default(Rect);
					((Rect)(ref rect4))._002Ector(val7);
					((Rect)(ref rect4)).x = ((Rect)(ref rect4)).x + 90f;
					((Rect)(ref rect4)).width = ((Rect)(ref rect4)).width - 90f;
					Widgets.Label(rect4, pawn.story.title);
					num8 += ((Rect)(ref val7)).height;
				}
			}
		});
		num2 = 30f;
		List<Trait> traits = pawn.story.traits.allTraits;
		if (traits == null || traits.Count == 0)
		{
			num2 += 22f;
			stackHeight = 22f;
		}
		else
		{
			Rect val = GenUI.DrawElementStack(new Rect(0f, 0f, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRect)).height), 22f, pawn.story.traits.TraitsSorted, delegate
			{
			}, (Trait trait) => Text.CalcSize(trait.LabelCap).x + 10f, 4f, 5f, allowOrderOptimization: false);
			num2 += ((Rect)(ref val)).height;
			stackHeight = ((Rect)(ref val)).height;
		}
		list.Add(new LeftRectSection
		{
			rect = new Rect(0f, 0f, ((Rect)(ref leftRect)).width, num2),
			drawer = delegate(Rect sectionRect)
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				float currentY3 = ((Rect)(ref sectionRect)).y;
				Widgets.Label(new Rect(((Rect)(ref sectionRect)).x, currentY3, 200f, 30f), "Traits".Translate().AsTipTitle());
				currentY3 += 24f;
				if (traits == null || traits.Count == 0)
				{
					Color color2 = GUI.color;
					GUI.color = Color.gray;
					Rect rect3 = default(Rect);
					((Rect)(ref rect3))._002Ector(((Rect)(ref sectionRect)).x, currentY3, ((Rect)(ref leftRect)).width, 24f);
					if (Mouse.IsOver(rect3))
					{
						Widgets.DrawHighlight(rect3);
					}
					Widgets.Label(rect3, pawn.DevelopmentalStage.Baby() ? "TraitsDevelopLaterBaby".Translate() : "None".Translate());
					currentY3 += ((Rect)(ref rect3)).height + 2f;
					TooltipHandler.TipRegionByKey(rect3, "None");
					GUI.color = color2;
				}
				else
				{
					GenUI.DrawElementStack(new Rect(((Rect)(ref sectionRect)).x, currentY3, ((Rect)(ref leftRect)).width - 5f, stackHeight), 22f, pawn.story.traits.TraitsSorted, delegate(Rect r, Trait trait)
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_0005: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_0035: Unknown result type (might be due to invalid IL or missing references)
						//IL_007b: Unknown result type (might be due to invalid IL or missing references)
						//IL_008b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0095: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
						Color color3 = GUI.color;
						GUI.color = StackElementBackground;
						GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
						GUI.color = color3;
						if (Mouse.IsOver(r))
						{
							Widgets.DrawHighlight(r);
						}
						if (trait.Suppressed)
						{
							GUI.color = ColoredText.SubtleGrayColor;
						}
						else if (trait.sourceGene != null)
						{
							GUI.color = ColoredText.GeneColor;
						}
						Widgets.Label(new Rect(((Rect)(ref r)).x + 5f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 10f, ((Rect)(ref r)).height), trait.LabelCap);
						GUI.color = Color.white;
						if (Mouse.IsOver(r))
						{
							Trait trLocal = trait;
							TooltipHandler.TipRegion(tip: new TipSignal(() => trLocal.TipString(pawn), (int)currentY3 * 37), rect: r);
						}
					}, (Trait trait) => Text.CalcSize(trait.LabelCap).x + 10f, 4f, 5f, allowOrderOptimization: false);
				}
			}
		});
		num2 = 30f;
		WorkTags disabledTags = pawn.CombinedDisabledWorkTags;
		List<WorkTags> disabledTagsList = WorkTagsFrom(disabledTags).ToList();
		bool allowWorkTagVerticalLayout = false;
		GenUI.StackElementWidthGetter<WorkTags> workTagWidthGetter = (WorkTags tag) => Text.CalcSize(tag.LabelTranslated().CapitalizeFirst()).x + 10f;
		if (disabledTags == WorkTags.None)
		{
			num2 += 22f;
		}
		else
		{
			disabledTagsList.Sort(delegate(WorkTags a, WorkTags b)
			{
				int num7 = (GetWorkTypeDisableCauses(pawn, a).Any((object c) => c is RoyalTitleDef) ? 1 : (-1));
				int value5 = (GetWorkTypeDisableCauses(pawn, b).Any((object c) => c is RoyalTitleDef) ? 1 : (-1));
				return num7.CompareTo(value5);
			});
			Rect val2 = GenUI.DrawElementStack(new Rect(0f, 0f, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRect)).height), 22f, disabledTagsList, delegate
			{
			}, workTagWidthGetter, 4f, 5f, allowOrderOptimization: false);
			num2 += ((Rect)(ref val2)).height;
			stackHeight = ((Rect)(ref val2)).height;
			num2 += 12f;
			Rect val3 = GenUI.DrawElementStackVertical(new Rect(0f, 0f, ((Rect)(ref rect)).width, stackHeight), 22f, disabledTagsList, delegate
			{
			}, workTagWidthGetter);
			allowWorkTagVerticalLayout = ((Rect)(ref val3)).width <= ((Rect)(ref leftRect)).width;
		}
		list.Add(new LeftRectSection
		{
			rect = new Rect(0f, 0f, ((Rect)(ref leftRect)).width, num2),
			drawer = delegate(Rect sectionRect)
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0142: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
				float currentY2 = ((Rect)(ref sectionRect)).y;
				Widgets.Label(new Rect(((Rect)(ref sectionRect)).x, currentY2, 200f, 24f), "IncapableOf".Translate(pawn).AsTipTitle());
				currentY2 += 24f;
				if (disabledTags == WorkTags.None)
				{
					GUI.color = Color.gray;
					Rect rect2 = default(Rect);
					((Rect)(ref rect2))._002Ector(((Rect)(ref sectionRect)).x, currentY2, ((Rect)(ref leftRect)).width, 24f);
					if (Mouse.IsOver(rect2))
					{
						Widgets.DrawHighlight(rect2);
					}
					Widgets.Label(rect2, "None".Translate());
					TooltipHandler.TipRegionByKey(rect2, "None");
				}
				else
				{
					GenUI.StackElementDrawer<WorkTags> drawer = delegate(Rect r, WorkTags tag)
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_0005: Unknown result type (might be due to invalid IL or missing references)
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0030: Unknown result type (might be due to invalid IL or missing references)
						//IL_003a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0070: Unknown result type (might be due to invalid IL or missing references)
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_0042: Unknown result type (might be due to invalid IL or missing references)
						//IL_00be: Unknown result type (might be due to invalid IL or missing references)
						Color color = GUI.color;
						GUI.color = StackElementBackground;
						GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
						GUI.color = color;
						GUI.color = GetDisabledWorkTagLabelColor(pawn, tag);
						if (Mouse.IsOver(r))
						{
							Widgets.DrawHighlight(r);
						}
						Widgets.Label(new Rect(((Rect)(ref r)).x + 5f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 10f, ((Rect)(ref r)).height), tag.LabelTranslated().CapitalizeFirst());
						if (Mouse.IsOver(r))
						{
							WorkTags tagLocal = tag;
							TooltipHandler.TipRegion(tip: new TipSignal(() => GetWorkTypeDisabledCausedBy(pawnLocal, tagLocal) + "\n" + GetWorkTypesDisabledByWorkTag(tagLocal), (int)currentY2 * 32), rect: r);
						}
					};
					if (allowWorkTagVerticalLayout)
					{
						GenUI.DrawElementStackVertical(new Rect(((Rect)(ref sectionRect)).x, currentY2, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRect)).height / (float)numSections), 22f, disabledTagsList, drawer, workTagWidthGetter);
					}
					else
					{
						GenUI.DrawElementStack(new Rect(((Rect)(ref sectionRect)).x, currentY2, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRect)).height / (float)numSections), 22f, disabledTagsList, drawer, workTagWidthGetter, 5f);
					}
				}
				GUI.color = Color.white;
			}
		});
		if (abilities.Any())
		{
			num2 = 30f;
			Rect val4 = GenUI.DrawElementStack(new Rect(0f, 0f, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRect)).height), 32f, abilities, delegate
			{
			}, (Ability abil) => 32f);
			num2 += ((Rect)(ref val4)).height;
			stackHeight = ((Rect)(ref val4)).height;
			list.Add(new LeftRectSection
			{
				rect = new Rect(0f, 0f, ((Rect)(ref leftRect)).width, num2),
				drawer = delegate(Rect sectionRect)
				{
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00da: Unknown result type (might be due to invalid IL or missing references)
					float currentY = ((Rect)(ref sectionRect)).y;
					Widgets.Label(new Rect(((Rect)(ref sectionRect)).x, currentY, 200f, 24f), "Abilities".Translate(pawn).AsTipTitle());
					currentY += 24f;
					GenUI.DrawElementStack(new Rect(((Rect)(ref sectionRect)).x, currentY, ((Rect)(ref leftRect)).width - 5f, stackHeight), 32f, abilities, delegate(Rect r, Ability abil)
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0019: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						//IL_0044: Unknown result type (might be due to invalid IL or missing references)
						//IL_0076: Unknown result type (might be due to invalid IL or missing references)
						GUI.DrawTexture(r, (Texture)(object)BaseContent.ClearTex);
						if (Mouse.IsOver(r))
						{
							Widgets.DrawHighlight(r);
						}
						if (Widgets.ButtonImage(r, abil.def.uiIcon, doMouseoverSound: false))
						{
							Find.WindowStack.Add(new Dialog_InfoCard(abil.def));
						}
						if (Mouse.IsOver(r))
						{
							Ability abilCapture = abil;
							TipSignal tip = new TipSignal(() => abilCapture.Tooltip + "\n\n" + "ClickToLearnMore".Translate().Colorize(ColoredText.SubtleGrayColor), (int)currentY * 37);
							TooltipHandler.TipRegion(r, tip);
						}
					}, (Ability abil) => 32f);
					GUI.color = Color.white;
				}
			});
		}
		else
		{
			num2 += 12f;
		}
		float num3 = ((Rect)(ref leftRect)).height / (float)list.Count;
		float num4 = 0f;
		for (int i = 0; i < list.Count; i++)
		{
			LeftRectSection value = list[i];
			if (((Rect)(ref value.rect)).height > num3)
			{
				num4 += ((Rect)(ref value.rect)).height - num3;
				value.calculatedSize = ((Rect)(ref value.rect)).height;
			}
			else
			{
				value.calculatedSize = num3;
			}
			list[i] = value;
		}
		bool flag = false;
		float num5 = 0f;
		if (num4 > 0f)
		{
			LeftRectSection value2 = list[0];
			float num6 = ((Rect)(ref value2.rect)).height + 12f;
			num4 -= value2.calculatedSize - num6;
			value2.calculatedSize = num6;
			list[0] = value2;
		}
		while (num4 > 0f)
		{
			bool flag2 = true;
			for (int j = 0; j < list.Count; j++)
			{
				LeftRectSection value3 = list[j];
				if (value3.calculatedSize - ((Rect)(ref value3.rect)).height > 0f)
				{
					value3.calculatedSize -= 1f;
					num4 -= 1f;
					flag2 = false;
				}
				list[j] = value3;
			}
			if (!flag2)
			{
				continue;
			}
			for (int k = 0; k < list.Count; k++)
			{
				LeftRectSection value4 = list[k];
				if (k > 0)
				{
					value4.calculatedSize = Mathf.Max(((Rect)(ref value4.rect)).height, num3);
				}
				else
				{
					value4.calculatedSize = ((Rect)(ref value4.rect)).height + 22f;
				}
				num5 += value4.calculatedSize;
				list[k] = value4;
			}
			flag = true;
			break;
		}
		if (flag)
		{
			Widgets.BeginScrollView(new Rect(0f, 0f, ((Rect)(ref leftRect)).width, ((Rect)(ref leftRect)).height), ref leftRectScrollPos, new Rect(0f, 0f, ((Rect)(ref leftRect)).width - 16f, num5));
		}
		num = 0f;
		for (int l = 0; l < list.Count; l++)
		{
			LeftRectSection leftRectSection = list[l];
			leftRectSection.drawer(new Rect(0f, num, ((Rect)(ref leftRect)).width - 5f, ((Rect)(ref leftRectSection.rect)).height));
			num += leftRectSection.calculatedSize;
		}
		if (flag)
		{
			Widgets.EndScrollView();
		}
		Widgets.EndGroup();
	}

	private static string GetWorkTypeDisabledCausedBy(Pawn pawn, WorkTags workTag)
	{
		List<object> workTypeDisableCauses = GetWorkTypeDisableCauses(pawn, workTag);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (object item in workTypeDisableCauses)
		{
			if (item is BackstoryDef backstoryDef)
			{
				stringBuilder.AppendLine("IncapableOfTooltipBackstory".Translate() + ": " + backstoryDef.TitleFor(pawn.gender).CapitalizeFirst());
			}
			else if (item is Trait trait)
			{
				stringBuilder.AppendLine("IncapableOfTooltipTrait".Translate() + ": " + trait.LabelCap);
			}
			else if (item is Hediff hediff)
			{
				stringBuilder.AppendLine("IncapableOfTooltipHediff".Translate() + ": " + hediff.LabelCap);
			}
			else if (item is RoyalTitle royalTitle)
			{
				stringBuilder.AppendLine("IncapableOfTooltipTitle".Translate() + ": " + royalTitle.def.GetLabelFor(pawn));
			}
			else if (item is Quest quest)
			{
				stringBuilder.AppendLine("IncapableOfTooltipQuest".Translate() + ": " + quest.name);
			}
			else if (item is Precept_Role precept_Role)
			{
				stringBuilder.AppendLine("IncapableOfTooltipRole".Translate() + ": " + precept_Role.LabelForPawn(pawn));
			}
			else if (item is Gene gene)
			{
				stringBuilder.AppendLine("IncapableOfTooltipGene".Translate() + ": " + gene.LabelCap);
			}
			else if (item is MutantDef mutantDef)
			{
				stringBuilder.AppendLine("IncapableOfTooltipMutant".Translate() + ": " + mutantDef.LabelCap);
			}
		}
		return stringBuilder.ToString();
	}

	private static string GetWorkTypesDisabledByWorkTag(WorkTags workTag)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("IncapableOfTooltipWorkTypes".Translate().Colorize(ColoredText.TipSectionTitleColor));
		foreach (WorkTypeDef allDef in DefDatabase<WorkTypeDef>.AllDefs)
		{
			if ((allDef.workTags & workTag) > WorkTags.None)
			{
				stringBuilder.Append("- ");
				stringBuilder.AppendLine(allDef.pawnLabel);
			}
		}
		return stringBuilder.ToString();
	}

	public static Vector2 PawnCardSize(Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector2 basePawnCardSize = BasePawnCardSize;
		tmpInspectStrings.Length = 0;
		QuestUtility.AppendInspectStringsFromQuestParts(tmpInspectStrings, pawn, out var count);
		if (count >= 2)
		{
			basePawnCardSize.y += (count - 1) * 20;
		}
		return basePawnCardSize;
	}

	public static void DoNameInputRect(Rect rect, ref string name, int maxLength)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string text = Widgets.TextField(rect, name);
		if (text.Length <= maxLength && ValidNameRegex.IsMatch(text))
		{
			name = text;
		}
	}

	private static IEnumerable<WorkTags> WorkTagsFrom(WorkTags tags)
	{
		foreach (WorkTags allSelectedItem in tags.GetAllSelectedItems<WorkTags>())
		{
			if (allSelectedItem != 0)
			{
				yield return allSelectedItem;
			}
		}
	}

	private static Vector2 GetQuestLineSize(string line, Quest quest, bool drawMinimized)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (drawMinimized)
		{
			return new Vector2(24f, 24f);
		}
		Vector2 val = Text.CalcSize(line);
		return new Vector2(24f + val.x + 10f, Mathf.Max(24f, val.y));
	}

	private static void DoQuestLine(Rect rect, string line, Quest quest, bool drawMinimized)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 29f;
		((Rect)(ref rect2)).height = Text.CalcSize(line).y;
		float x = Text.CalcSize(line).x;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, Mathf.Min(x, ((Rect)(ref rect2)).width) + 24f + 5f, ((Rect)(ref rect)).height);
		if (!quest.hidden)
		{
			Widgets.DrawHighlightIfMouseover(val);
			if (drawMinimized)
			{
				TooltipHandler.TipRegion(val, line + "\n\n" + "ClickToViewInQuestsTab".Translate());
			}
			else
			{
				TooltipHandler.TipRegionByKey(val, "ClickToViewInQuestsTab");
			}
		}
		GUI.DrawTexture(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y - 1f, 24f, 24f), (Texture)(object)QuestIcon);
		if (!drawMinimized)
		{
			Widgets.Label(rect2, line.Truncate(((Rect)(ref rect2)).width));
		}
		if (!quest.hidden && Widgets.ButtonInvisible(val))
		{
			Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Quests);
			((MainTabWindow_Quests)MainButtonDefOf.Quests.TabWindow).Select(quest);
		}
	}
}
