using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class ITab_Pawn_Visitor : ITab
{
	private static readonly List<PrisonerInteractionModeDef> tmpPrisonerInteractionModes = new List<PrisonerInteractionModeDef>();

	private static readonly List<SlaveInteractionModeDef> tmpSlaveInteractionModes = new List<SlaveInteractionModeDef>();

	private static readonly Texture2D Subarrow = ContentFinder<Texture2D>.Get("UI/Misc/Subarrow");

	private const float SuppressionBarHeight = 30f;

	private const float SuppressionBarMargin = 7f;

	public ITab_Pawn_Visitor()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(280f, 0f);
	}

	protected override void FillTab()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.PrisonerTab, KnowledgeAmount.FrameDisplayed);
		Text.Font = GameFont.Small;
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 10f);
		bool isPrisonerOfColony = SelPawn.IsPrisonerOfColony;
		bool isSlaveOfColony = SelPawn.IsSlaveOfColony;
		bool wildMan = SelPawn.IsWildMan();
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.maxOneColumn = true;
		listing_Standard.Begin(rect);
		if (!isSlaveOfColony && !isPrisonerOfColony)
		{
			Rect rect2 = listing_Standard.GetRect(28f);
			((Rect)(ref rect2)).width = 140f;
			MedicalCareUtility.MedicalCareSetter(rect2, ref SelPawn.playerSettings.medCare);
			listing_Standard.Gap(4f);
		}
		if (isPrisonerOfColony)
		{
			DoPrisonerTab(wildMan, listing_Standard);
		}
		if (isSlaveOfColony)
		{
			DoSlaveTab(listing_Standard);
		}
		listing_Standard.End();
		size = new Vector2(280f, listing_Standard.CurHeight + 20f);
	}

	private void DoSlaveTab(Listing_Standard listing)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		if (!SelPawn.needs.TryGetNeed(out Need_Suppression need))
		{
			return;
		}
		Rect val = listing.Label("Suppression".Translate() + ": " + need.CurLevel.ToStringPercent());
		Rect rect = listing.GetRect(30f);
		Rect rect2 = rect.ContractedBy(7f);
		need.DrawSuppressionBar(rect2);
		Rect rect3 = new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height + ((Rect)(ref rect)).height);
		Widgets.DrawHighlightIfMouseover(rect3);
		TaggedString taggedString = "SuppressionDesc".Translate();
		TooltipHandler.TipRegion(rect3, taggedString);
		float statValue = SelPawn.GetStatValue(StatDefOf.SlaveSuppressionFallRate);
		string text = StatDefOf.SlaveSuppressionFallRate.ValueToString(statValue);
		Rect rect4 = listing.Label("SuppressionFallRate".Translate() + ": " + text);
		if (Mouse.IsOver(rect4))
		{
			TaggedString taggedString2 = "SuppressionFallRateDesc".Translate(0.2f.ToStringPercent(), 0.3f.ToStringPercent(), 0.1f.ToStringPercent(), 0.15f.ToStringPercent(), 0.15f.ToStringPercent(), 0.05f.ToStringPercent(), 0.15f.ToStringPercent());
			string explanationForTooltip = ((StatWorker_SuppressionFallRate)StatDefOf.SlaveSuppressionFallRate.Worker).GetExplanationForTooltip(StatRequest.For(SelPawn));
			TooltipHandler.TipRegion(rect4, taggedString2 + ":\n\n" + explanationForTooltip);
			Widgets.DrawHighlight(rect4);
		}
		Rect rect5 = listing.Label(string.Format("{0}: {1}", "Terror".Translate(), SelPawn.GetStatValue(StatDefOf.Terror).ToStringPercent()));
		if (Mouse.IsOver(rect5))
		{
			IOrderedEnumerable<Thought_MemoryObservationTerror> source = from t in TerrorUtility.GetTerrorThoughts(SelPawn)
				orderby t.intensity descending
				select t;
			TaggedString taggedString3 = "TerrorDescription".Translate() + ":" + "\n\n" + TerrorUtility.SuppressionFallRateOverTerror.Points.Select((CurvePoint p) => string.Format("- {0} {1}: {2}", "Terror".Translate(), (p.x / 100f).ToStringPercent(), (p.y / 100f).ToStringPercent())).ToLineList();
			if (source.Any())
			{
				string text2 = source.Select((Thought_MemoryObservationTerror t) => $"{t.LabelCap}: {t.intensity}%").ToLineList("- ", capitalizeItems: true);
				taggedString3 += "\n\n" + "TerrorCurrentThoughts".Translate() + ":" + "\n\n" + text2;
			}
			TooltipHandler.TipRegion(rect5, taggedString3);
			Widgets.DrawHighlight(rect5);
		}
		float num = SlaveRebellionUtility.InitiateSlaveRebellionMtbDays(SelPawn);
		TaggedString label = "SlaveRebellionMTBDays".Translate() + ": ";
		if (!SelPawn.Awake())
		{
			label += "NotWhileAsleep".Translate();
		}
		else if (num < 0f)
		{
			label += "Never".Translate();
		}
		else
		{
			label += ((int)(num * 60000f)).ToStringTicksToPeriod();
		}
		Rect rect6 = listing.Label(label);
		TooltipHandler.TipRegion(rect6, delegate
		{
			TaggedString taggedString5 = "SlaveRebellionMTBDaysDescription".Translate();
			string text4 = SlaveRebellionUtility.GetSlaveRebellionMtbCalculationExplanation(SelPawn) + "\n" + SlaveRebellionUtility.GetAnySlaveRebellionExplanation(SelPawn);
			if (!text4.NullOrEmpty())
			{
				taggedString5 += "\n\n" + text4;
			}
			return taggedString5;
		}, "SlaveRebellionTooltip".GetHashCode());
		Widgets.DrawHighlightIfMouseover(rect6);
		DoSlavePriceListing(listing);
		Faction faction = SelPawn.SlaveFaction ?? SelPawn.Faction;
		TaggedString taggedString4;
		if (faction == null || faction.IsPlayer || !faction.CanChangeGoodwillFor(Faction.OfPlayer, 1))
		{
			taggedString4 = "None".Translate();
		}
		else
		{
			bool isHealthy;
			bool isInMentalState;
			int i = faction.CalculateAdjustedGoodwillChange(Faction.OfPlayer, faction.GetGoodwillGainForExit(SelPawn, freed: true, out isHealthy, out isInMentalState));
			taggedString4 = ((isHealthy && !isInMentalState) ? (faction.NameColored + " " + i.ToStringWithSign()) : ((!isHealthy) ? ("None".Translate() + " (" + "UntendedInjury".Translate() + ")") : ((!isInMentalState) ? "None".Translate() : ("None".Translate() + " (" + SelPawn.MentalState.InspectLine + ")"))));
		}
		Rect rect7 = listing.Label("SlaveReleasePotentialRelationGains".Translate() + ": " + taggedString4);
		TooltipHandler.TipRegionByKey(rect7, "SlaveReleaseRelationGainsDesc");
		Widgets.DrawHighlightIfMouseover(rect7);
		tmpSlaveInteractionModes.Clear();
		tmpSlaveInteractionModes.AddRange(DefDatabase<SlaveInteractionModeDef>.AllDefs.OrderBy((SlaveInteractionModeDef pim) => pim.listOrder));
		int num2 = 32 * tmpSlaveInteractionModes.Count;
		Rect rect8 = listing.GetRect(num2).Rounded();
		Widgets.DrawMenuSection(rect8);
		Rect rect9 = rect8.ContractedBy(10f);
		Widgets.BeginGroup(rect9);
		SlaveInteractionModeDef currentSlaveIteractionMode = SelPawn.guest.slaveInteractionMode;
		Rect rect10 = default(Rect);
		((Rect)(ref rect10))._002Ector(0f, 0f, ((Rect)(ref rect9)).width, 30f);
		foreach (SlaveInteractionModeDef tmpSlaveInteractionMode in tmpSlaveInteractionModes)
		{
			if (Widgets.RadioButtonLabeled(rect10, tmpSlaveInteractionMode.LabelCap, SelPawn.guest.slaveInteractionMode == tmpSlaveInteractionMode))
			{
				if (tmpSlaveInteractionMode == SlaveInteractionModeDefOf.Imprison && RestUtility.FindBedFor(SelPawn, SelPawn, checkSocialProperness: false, ignoreOtherReservations: false, GuestStatus.Prisoner) == null)
				{
					Messages.Message("CannotImprison".Translate() + ": " + "NoPrisonerBed".Translate(), SelPawn, MessageTypeDefOf.RejectInput, historical: false);
					continue;
				}
				SelPawn.guest.slaveInteractionMode = tmpSlaveInteractionMode;
				if (tmpSlaveInteractionMode == SlaveInteractionModeDefOf.Execute && SelPawn.SlaveFaction != null && !SelPawn.SlaveFaction.HostileTo(Faction.OfPlayer))
				{
					Dialog_MessageBox window = new Dialog_MessageBox("ExectueNeutralFactionSlave".Translate(SelPawn.Named("PAWN"), SelPawn.SlaveFaction.Named("FACTION")), "Confirm".Translate(), delegate
					{
					}, "Cancel".Translate(), delegate
					{
						SelPawn.guest.slaveInteractionMode = currentSlaveIteractionMode;
					});
					Find.WindowStack.Add(window);
				}
			}
			if (!tmpSlaveInteractionMode.description.NullOrEmpty() && Mouse.IsOver(rect10))
			{
				Widgets.DrawHighlight(rect10);
				string text3 = tmpSlaveInteractionMode.description;
				if (tmpSlaveInteractionMode == SlaveInteractionModeDefOf.Emancipate)
				{
					text3 = ((SelPawn.SlaveFaction == Faction.OfPlayer) ? ((string)(text3 + (" " + "EmancipateCololonistTooltip".Translate()))) : ((SelPawn.SlaveFaction == null) ? ((string)(text3 + (" " + "EmancipateNonCololonistWithoutFactionTooltip".Translate()))) : ((string)(text3 + (" " + "EmancipateNonCololonistWithFactionTooltip".Translate(SelPawn.SlaveFaction.NameColored))))));
				}
				TooltipHandler.TipRegion(rect10, text3);
			}
			((Rect)(ref rect10)).y = ((Rect)(ref rect10)).y + 28f;
		}
		Widgets.EndGroup();
		tmpSlaveInteractionModes.Clear();
	}

	private void DoPrisonerTab(bool wildMan, Listing_Standard listing)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		int num = (int)PrisonBreakUtility.InitiatePrisonBreakMtbDays(SelPawn, stringBuilder, ignoreAsleep: true);
		string text = "PrisonBreakMTBDays".Translate() + ": ";
		if (PrisonBreakUtility.IsPrisonBreaking(SelPawn))
		{
			text += "CurrentlyPrisonBreaking".Translate();
		}
		else if (num < 0)
		{
			text += "Never".Translate();
			if (PrisonBreakUtility.GenePreventsPrisonBreaking(SelPawn, out var gene))
			{
				stringBuilder.AppendLineIfNotEmpty();
				stringBuilder.AppendTagged("PrisonBreakingDisabledDueToGene".Translate(gene.def.Named("GENE")).Colorize(ColorLibrary.RedReadable));
			}
		}
		else
		{
			text += "PeriodDays".Translate(num).ToString().Colorize(ColoredText.DateTimeColor);
		}
		Rect rect = listing.Label(text);
		string text2 = "PrisonBreakMTBDaysDescription".Translate();
		if (stringBuilder.Length > 0)
		{
			text2 = text2 + "\n\n" + stringBuilder;
		}
		TooltipHandler.TipRegion(rect, text2);
		Widgets.DrawHighlightIfMouseover(rect);
		if (!wildMan)
		{
			if (SelPawn.guest.Recruitable)
			{
				float num2 = ((SelPawn.guest.resistance > 0f) ? Mathf.Max(0.1f, SelPawn.guest.resistance) : 0f);
				Rect rect2 = listing.Label("RecruitmentResistance".Translate() + ": " + num2.ToString("F1").Colorize(ColoredText.DateTimeColor));
				if (Mouse.IsOver(rect2))
				{
					TaggedString taggedString = "RecruitmentResistanceDesc".Translate();
					FloatRange value = SelPawn.kindDef.initialResistanceRange.Value;
					taggedString += string.Format("\n\n{0}: {1}~{2}", "RecruitmentResistanceFromPawnKind".Translate(SelPawn.kindDef.LabelCap), value.min, value.max);
					if (SelPawn.royalty != null)
					{
						RoyalTitle mostSeniorTitle = SelPawn.royalty.MostSeniorTitle;
						if (mostSeniorTitle != null && mostSeniorTitle.def.recruitmentResistanceOffset != 0f)
						{
							string text3 = ((mostSeniorTitle.def.recruitmentResistanceOffset > 0f) ? "+" : "-");
							taggedString += "\n" + "RecruitmentResistanceRoyalTitleOffset".Translate(mostSeniorTitle.Label.CapitalizeFirst()) + (": " + text3) + mostSeniorTitle.def.recruitmentResistanceOffset.ToString();
						}
					}
					TooltipHandler.TipRegion(rect2, taggedString);
				}
				Widgets.DrawHighlightIfMouseover(rect2);
			}
			else
			{
				Rect rect3 = listing.Label("RecruitmentResistance".Translate() + ": " + "NonRecruitable".Translate());
				string text4 = "NonRecruitableTip".Translate();
				TooltipHandler.TipRegion(rect3, text4);
				Widgets.DrawHighlightIfMouseover(rect3);
			}
			if (ModsConfig.IdeologyActive)
			{
				Rect rect4 = listing.Label("WillLevel".Translate() + ": " + SelPawn.guest.will.ToString("F1").Colorize(ColoredText.DateTimeColor));
				TaggedString taggedString2 = "WillLevelDesc".Translate(2.5f);
				if (!SelPawn.guest.EverEnslaved)
				{
					FloatRange value2 = SelPawn.kindDef.initialWillRange.Value;
					taggedString2 += string.Format("\n\n{0} : {1}~{2}", "WillFromPawnKind".Translate(SelPawn.kindDef.LabelCap), value2.min, value2.max);
				}
				TooltipHandler.TipRegion(rect4, taggedString2);
				Widgets.DrawHighlightIfMouseover(rect4);
			}
		}
		DoSlavePriceListing(listing);
		if (IsStudiable(SelPawn))
		{
			CompStudiable compStudiable = SelPawn.TryGetComp<CompStudiable>();
			if (compStudiable != null)
			{
				ITab_Entity.DoStudyPeriodListing(listing, compStudiable);
			}
			if (compStudiable != null)
			{
				ITab_Entity.DoKnowledgeGainListing(listing, compStudiable);
			}
		}
		TaggedString taggedString3;
		if (SelPawn.Faction == null || SelPawn.Faction.IsPlayer || !SelPawn.Faction.CanChangeGoodwillFor(Faction.OfPlayer, 1))
		{
			taggedString3 = "None".Translate();
		}
		else
		{
			bool isHealthy;
			bool isInMentalState;
			int i = SelPawn.Faction.CalculateAdjustedGoodwillChange(Faction.OfPlayer, SelPawn.Faction.GetGoodwillGainForExit(SelPawn, freed: true, out isHealthy, out isInMentalState));
			taggedString3 = ((isHealthy && !isInMentalState) ? (SelPawn.Faction.NameColored + " " + i.ToStringWithSign()) : (isHealthy ? ("None".Translate() + " (" + SelPawn.MentalState.InspectLine + ")") : ("None".Translate() + " (" + "UntendedInjury".Translate() + ")")));
		}
		Rect rect5 = listing.Label("PrisonerReleasePotentialRelationGains".Translate() + ": " + taggedString3);
		TooltipHandler.TipRegionByKey(rect5, "PrisonerReleaseRelationGainsDesc");
		Widgets.DrawHighlightIfMouseover(rect5);
		if (SelPawn.guilt.IsGuilty)
		{
			if (!SelPawn.InAggroMentalState)
			{
				listing.Label("ConsideredGuilty".Translate(SelPawn.guilt.TicksUntilInnocent.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor)));
			}
			else
			{
				listing.Label("ConsideredGuiltyNoTimer".Translate() + " (" + SelPawn.MentalStateDef.label + ")");
			}
		}
		if (ModsConfig.IdeologyActive && SelPawn.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Convert) && SelPawn.guest.ideoForConversion != null)
		{
			Rect rect6 = listing.GetRect(24f);
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref rect6)).xMax - 24f - 4f, ((Rect)(ref rect6)).y, 24f, 24f);
			((Rect)(ref rect6)).xMax = ((Rect)(ref val)).xMin;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect6, "IdeoConversionTarget".Translate());
			Text.Anchor = (TextAnchor)0;
			Widgets.DrawHighlightIfMouseover(rect6);
			TooltipHandler.TipRegionByKey(rect6, "IdeoConversionTargetDesc");
			SelPawn.guest.ideoForConversion.DrawIcon(val.ContractedBy(2f));
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
				TooltipHandler.TipRegion(val, SelPawn.guest.ideoForConversion.name);
			}
			if (Widgets.ButtonInvisible(val))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Ideo allIdeo in Faction.OfPlayer.ideos.AllIdeos)
				{
					Ideo newIdeo = allIdeo;
					string text5 = allIdeo.name;
					Action action = delegate
					{
						SelPawn.guest.ideoForConversion = newIdeo;
					};
					if (!ColonyHasAnyWardenOfIdeo(newIdeo, SelPawn.MapHeld))
					{
						text5 += " (" + "NoWardenOfIdeo".Translate(newIdeo.memberName.Named("MEMBERNAME")) + ")";
						action = null;
					}
					list.Add(new FloatMenuOption(text5, action, newIdeo.Icon, newIdeo.Color));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}
		if (SelPawn.guest.finalResistanceInteractionData != null)
		{
			ResistanceInteractionData finalResistanceInteractionData = SelPawn.guest.finalResistanceInteractionData;
			Rect rect7 = listing.Label("LastRecruitment".Translate() + ": " + finalResistanceInteractionData.resistanceReduction.ToStringByStyle(ToStringStyle.FloatTwo));
			if (Mouse.IsOver(rect7))
			{
				Widgets.DrawHighlight(rect7);
				TaggedString taggedString4 = "LastRecruitmentDescription".Translate(SelPawn, finalResistanceInteractionData.initiatorName);
				taggedString4 += "\n\n";
				taggedString4 += "StatsReport_BaseValue".Translate() + ": " + 1f.ToStringByStyle(ToStringStyle.FloatTwo);
				taggedString4 += "\n\n" + "Mood".Translate() + ": x" + finalResistanceInteractionData.recruiteeMoodFactor.ToStringByStyle(ToStringStyle.FloatTwo);
				taggedString4 += "\n" + "RecruiterNegotiationAbility".Translate() + ": x" + finalResistanceInteractionData.initiatorNegotiationAbilityFactor.ToStringByStyle(ToStringStyle.FloatTwo);
				taggedString4 += "\n" + "OpinionOfRecruiter".Translate() + ": x" + finalResistanceInteractionData.recruiterOpinionFactor.ToStringByStyle(ToStringStyle.FloatTwo);
				taggedString4 += "\n" + "StatsReport_FinalValue".Translate() + ": " + finalResistanceInteractionData.resistanceReduction.ToStringByStyle(ToStringStyle.FloatTwo);
				TooltipHandler.TipRegion(rect7, taggedString4);
			}
		}
		listing.Gap(1f);
		Rect val2 = listing.GetRect(24f).Rounded();
		TooltipHandler.TipRegionByKey(val2, "MedicineQualityDescriptionPrisoner");
		Widgets.DrawHighlightIfMouseover(val2);
		Rect rect8 = val2;
		((Rect)(ref rect8)).xMax = ((Rect)(ref val2)).center.x - 4f;
		Rect rect9 = val2;
		((Rect)(ref rect9)).xMin = ((Rect)(ref val2)).center.x + 4f;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect8, string.Format("{0}:", "AllowMedicine".Translate()));
		Text.Anchor = (TextAnchor)0;
		Widgets.DrawButtonGraphic(rect9);
		MedicalCareUtility.MedicalCareSelectButton(rect9, SelPawn);
		listing.Gap(4f);
		tmpPrisonerInteractionModes.Clear();
		tmpPrisonerInteractionModes.AddRange(from m in DefDatabase<PrisonerInteractionModeDef>.AllDefs
			where !m.isNonExclusiveInteraction && CanUsePrisonerInteractionMode(SelPawn, m)
			select m into pim
			orderby pim.listOrder
			select pim);
		float height = 28f * (float)tmpPrisonerInteractionModes.Count + 20f;
		Rect rect10 = listing.GetRect(height).Rounded();
		Widgets.DrawMenuSection(rect10);
		Rect rect11 = rect10.ContractedBy(10f);
		Widgets.BeginGroup(rect11);
		Rect rect12 = default(Rect);
		((Rect)(ref rect12))._002Ector(0f, 0f, ((Rect)(ref rect11)).width, 28f);
		foreach (PrisonerInteractionModeDef tmpPrisonerInteractionMode in tmpPrisonerInteractionModes)
		{
			DrawExclusiveInteractionRow(ref rect12, tmpPrisonerInteractionMode);
		}
		Widgets.EndGroup();
		tmpPrisonerInteractionModes.Clear();
		tmpPrisonerInteractionModes.AddRange(from m in DefDatabase<PrisonerInteractionModeDef>.AllDefs
			where m.isNonExclusiveInteraction && CanUsePrisonerInteractionMode(SelPawn, m)
			select m into pim
			orderby pim.listOrder
			select pim);
		if (tmpPrisonerInteractionModes.Any())
		{
			listing.Gap();
			height = 28f * (float)tmpPrisonerInteractionModes.Count + 20f;
			Rect rect13 = listing.GetRect(height).Rounded();
			Widgets.DrawMenuSection(rect13);
			rect11 = rect13.ContractedBy(10f);
			Widgets.BeginGroup(rect11);
			Rect rect14 = default(Rect);
			((Rect)(ref rect14))._002Ector(0f, 0f, ((Rect)(ref rect11)).width, 28f);
			foreach (PrisonerInteractionModeDef tmpPrisonerInteractionMode2 in tmpPrisonerInteractionModes)
			{
				DrawNonExclusiveInteractionRow(ref rect14, tmpPrisonerInteractionMode2);
			}
			Widgets.EndGroup();
		}
		tmpPrisonerInteractionModes.Clear();
		bool CanUsePrisonerInteractionMode(Pawn pawn, PrisonerInteractionModeDef mode)
		{
			if (!pawn.guest.Recruitable && mode.hideIfNotRecruitable)
			{
				return false;
			}
			if (wildMan && !mode.allowOnWildMan)
			{
				return false;
			}
			if (mode.hideIfNoBloodfeeders && pawn.MapHeld != null && !ColonyHasAnyBloodfeeder(pawn.MapHeld))
			{
				return false;
			}
			if (mode.hideOnHemogenicPawns && ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.HasActiveGene(GeneDefOf.Hemogenic))
			{
				return false;
			}
			if (!mode.allowInClassicIdeoMode && Find.IdeoManager.classicMode)
			{
				return false;
			}
			if (ModsConfig.AnomalyActive)
			{
				if (mode.hideIfNotStudiableAsPrisoner && !IsStudiable(pawn))
				{
					return false;
				}
				if (mode.hideIfGrayFleshNotAppeared && !Find.Anomaly.hasSeenGrayFlesh)
				{
					return false;
				}
			}
			return true;
		}
	}

	private static bool IsStudiable(Pawn pawn)
	{
		if (!ModsConfig.AnomalyActive)
		{
			return false;
		}
		if (!pawn.TryGetComp<CompStudiable>(out var comp) || !comp.EverStudiable())
		{
			return false;
		}
		if (pawn.kindDef.studiableAsPrisoner)
		{
			return !pawn.everLostEgo;
		}
		return false;
	}

	private void DrawExclusiveInteractionRow(ref Rect rect, PrisonerInteractionModeDef mode)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		float xMin = ((Rect)(ref rect)).xMin;
		if (mode.isChildInteraction)
		{
			Rect outerRect = rect;
			((Rect)(ref outerRect)).x = ((Rect)(ref outerRect)).x - 3f;
			((Rect)(ref outerRect)).width = ((Rect)(ref outerRect)).height;
			Widgets.DrawTextureFitted(outerRect, (Texture)(object)Subarrow, 0.75f);
			((Rect)(ref rect)).xMin = ((Rect)(ref outerRect)).xMax + 2f;
		}
		if (Widgets.RadioButtonLabeled(rect, mode.LabelCap, SelPawn.guest.ExclusiveInteractionMode == mode))
		{
			SelPawn.guest.SetExclusiveInteraction(mode);
			InteractionModeChanged(mode);
		}
		if (!mode.description.NullOrEmpty() && Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, mode.description);
		}
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + 28f;
		((Rect)(ref rect)).xMin = xMin;
	}

	private void DrawNonExclusiveInteractionRow(ref Rect rect, PrisonerInteractionModeDef mode)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		bool checkOn = SelPawn.guest.IsInteractionEnabled(mode);
		bool flag = checkOn;
		Widgets.CheckboxLabeled(rect, mode.LabelCap, ref checkOn, disabled: false, null, null, placeCheckboxNearText: false, paintable: true);
		if (!mode.description.NullOrEmpty() && Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, mode.description);
		}
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + 28f;
		if (checkOn != flag)
		{
			SelPawn.guest.ToggleNonExclusiveInteraction(mode, checkOn);
			NonExclusiveInteractionToggled(mode, checkOn);
		}
	}

	private void InteractionModeChanged(PrisonerInteractionModeDef newMode)
	{
		if (newMode == PrisonerInteractionModeDefOf.Enslave && SelPawn.MapHeld != null && !ColonyHasAnyWardenCapableOfEnslavement(SelPawn.MapHeld))
		{
			Messages.Message("MessageNoWardenCapableOfEnslavement".Translate(), SelPawn, MessageTypeDefOf.CautionInput, historical: false);
		}
		if (newMode == PrisonerInteractionModeDefOf.Convert && SelPawn.guest.ideoForConversion == null)
		{
			SelPawn.guest.ideoForConversion = Faction.OfPlayer.ideos.PrimaryIdeo;
		}
		if (newMode == PrisonerInteractionModeDefOf.Execution && SelPawn.MapHeld != null && !ColonyHasAnyWardenCapableOfViolence(SelPawn.MapHeld))
		{
			Messages.Message("MessageCantDoExecutionBecauseNoWardenCapableOfViolence".Translate(), SelPawn, MessageTypeDefOf.CautionInput, historical: false);
		}
	}

	public void NonExclusiveInteractionToggled(PrisonerInteractionModeDef mode, bool enabled)
	{
		if (!ModsConfig.BiotechActive || mode != PrisonerInteractionModeDefOf.HemogenFarm)
		{
			return;
		}
		Bill bill = SelPawn.BillStack?.Bills?.FirstOrDefault((Bill x) => x.recipe == RecipeDefOf.ExtractHemogenPack);
		if (enabled)
		{
			if (bill == null && SanguophageUtility.CanSafelyBeQueuedForHemogenExtraction(SelPawn))
			{
				HealthCardUtility.CreateSurgeryBill(SelPawn, RecipeDefOf.ExtractHemogenPack, null);
			}
		}
		else if (bill != null)
		{
			SelPawn.BillStack.Bills.Remove(bill);
		}
	}

	private void DoSlavePriceListing(Listing_Standard listing)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float statValue = SelPawn.GetStatValue(StatDefOf.MarketValue);
		Rect rect = listing.Label("SlavePrice".Translate() + ": " + statValue.ToStringMoney());
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TaggedString taggedString = "SlavePriceDescription".Translate() + "\n\n" + StatDefOf.MarketValue.Worker.GetExplanationFull(StatRequest.For(SelPawn), StatDefOf.MarketValue.toStringNumberSense, statValue);
			TooltipHandler.TipRegion(rect, taggedString);
		}
	}

	private bool ColonyHasAnyBloodfeeder(Map map)
	{
		if (ModsConfig.BiotechActive)
		{
			foreach (Pawn item in map.mapPawns.FreeColonistsAndPrisonersSpawned)
			{
				if (item.IsBloodfeeder())
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool ColonyHasAnyWardenCapableOfViolence(Map map)
	{
		foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
		{
			if (item.workSettings.WorkIsActive(WorkTypeDefOf.Warden) && !item.WorkTagIsDisabled(WorkTags.Violent))
			{
				return true;
			}
		}
		return false;
	}

	private bool ColonyHasAnyWardenCapableOfEnslavement(Map map)
	{
		foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
		{
			if (item.workSettings.WorkIsActive(WorkTypeDefOf.Warden) && new HistoryEvent(HistoryEventDefOf.EnslavedPrisoner, item.Named(HistoryEventArgsNames.Doer)).DoerWillingToDo())
			{
				return true;
			}
		}
		return false;
	}

	private bool ColonyHasAnyWardenOfIdeo(Ideo ideo, Map map)
	{
		foreach (Pawn item in map.mapPawns.FreeColonistsSpawned)
		{
			if (item.workSettings.WorkIsActive(WorkTypeDefOf.Warden) && item.Ideo == ideo)
			{
				return true;
			}
		}
		return false;
	}
}
