using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Entity : ITab
{
	public override bool IsVisible
	{
		get
		{
			if (!ModsConfig.AnomalyActive)
			{
				return false;
			}
			if (base.SelThing is Building_HoldingPlatform building_HoldingPlatform)
			{
				return building_HoldingPlatform.HeldPawn != null;
			}
			if (base.SelThing is Pawn && base.SelThing.IsOnHoldingPlatform)
			{
				return true;
			}
			return false;
		}
	}

	protected override Pawn SelPawn
	{
		get
		{
			Pawn selPawn = base.SelPawn;
			if (selPawn != null)
			{
				return selPawn;
			}
			if (base.SelThing is Building_HoldingPlatform building_HoldingPlatform)
			{
				return building_HoldingPlatform.HeldPawn;
			}
			return null;
		}
	}

	public ITab_Entity()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(280f, 0f);
		labelKey = "TabEntity";
		tutorTag = "Entity";
	}

	public override void OnOpen()
	{
		if (!ModLister.CheckAnomaly("entity itab"))
		{
			CloseTab();
		}
		else
		{
			base.OnOpen();
		}
	}

	protected override void FillTab()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 10f);
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.maxOneColumn = true;
		listing_Standard.Begin(rect);
		if (SelPawn.ParentHolder is Thing thing && thing.TryGetComp(out CompEntityHolder comp))
		{
			StatDef containmentStrength = StatDefOf.ContainmentStrength;
			float containmentStrength2 = comp.ContainmentStrength;
			string text = containmentStrength.description + "\n\n" + containmentStrength.Worker.GetExplanationFull(StatRequest.For(thing), containmentStrength.toStringNumberSense, containmentStrength2);
			Widgets.DrawHighlightIfMouseover(listing_Standard.Label(containmentStrength.LabelCap + ": " + containmentStrength.ValueToString(containmentStrength2), -1f, new TipSignal(text, GetHashCode() + 1)));
		}
		StringBuilder stringBuilder = new StringBuilder();
		float num = ContainmentUtility.InitiateEscapeMtbDays(SelPawn, stringBuilder);
		int numTicks = Mathf.FloorToInt(num * 60000f);
		TaggedString taggedString = "HoldingPlatformEscapeMTBDays".Translate() + ": ";
		if (num < 0f)
		{
			taggedString += "Never".Translate();
		}
		else
		{
			taggedString += numTicks.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
		}
		string text2 = "HoldingPlatformEscapeMTBDaysDesc".Translate();
		if (stringBuilder.Length > 0)
		{
			text2 = text2 + "\n\n" + stringBuilder;
		}
		Widgets.DrawHighlightIfMouseover(listing_Standard.Label(taggedString, -1f, new TipSignal(text2, GetHashCode() + 2)));
		CompStudiable compStudiable = SelPawn.TryGetComp<CompStudiable>();
		if (compStudiable != null)
		{
			DoStudyPeriodListing(listing_Standard, compStudiable);
		}
		if (compStudiable != null)
		{
			DoKnowledgeGainListing(listing_Standard, compStudiable);
		}
		listing_Standard.Gap(1f);
		Rect val = listing_Standard.GetRect(24f).Rounded();
		TooltipHandler.TipRegionByKey(val, "MedicineQualityDescriptionEntity");
		Widgets.DrawHighlightIfMouseover(val);
		Rect rect2 = val;
		((Rect)(ref rect2)).xMax = ((Rect)(ref val)).center.x - 4f;
		Rect rect3 = val;
		((Rect)(ref rect3)).xMin = ((Rect)(ref val)).center.x + 4f;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, string.Format("{0}:", "AllowMedicine".Translate()));
		Text.Anchor = (TextAnchor)0;
		Widgets.DrawButtonGraphic(rect3);
		MedicalCareUtility.MedicalCareSelectButton(rect3, SelPawn);
		listing_Standard.Gap(4f);
		CompHoldingPlatformTarget compHoldingPlatformTarget = SelPawn.TryGetComp<CompHoldingPlatformTarget>();
		if (compHoldingPlatformTarget != null)
		{
			float height = 132f;
			Rect rect4 = listing_Standard.GetRect(height).Rounded();
			Widgets.DrawMenuSection(rect4);
			Rect rect5 = rect4.ContractedBy(10f);
			Widgets.BeginGroup(rect5);
			Rect rect6 = default(Rect);
			((Rect)(ref rect6))._002Ector(0f, 0f, ((Rect)(ref rect5)).width, 28f);
			if (Widgets.RadioButtonLabeled(rect6, "EntityStudyMode_MaintainOnly".Translate(), compHoldingPlatformTarget.containmentMode == EntityContainmentMode.MaintainOnly))
			{
				compHoldingPlatformTarget.containmentMode = EntityContainmentMode.MaintainOnly;
			}
			Widgets.DrawHighlightIfMouseover(rect6);
			TooltipHandler.TipRegion(rect6, "EntityStudyMode_MaintainOnlyDesc".Translate());
			((Rect)(ref rect6)).y = ((Rect)(ref rect6)).y + 28f;
			if (Widgets.RadioButtonLabeled(rect6, "EntityStudyMode_Study".Translate(), compHoldingPlatformTarget.containmentMode == EntityContainmentMode.Study))
			{
				compHoldingPlatformTarget.containmentMode = EntityContainmentMode.Study;
			}
			Widgets.DrawHighlightIfMouseover(rect6);
			TooltipHandler.TipRegion(rect6, "EntityStudyMode_StudyDesc".Translate());
			((Rect)(ref rect6)).y = ((Rect)(ref rect6)).y + 28f;
			if (Widgets.RadioButtonLabeled(rect6, "EntityStudyMode_Release".Translate(), compHoldingPlatformTarget.containmentMode == EntityContainmentMode.Release))
			{
				compHoldingPlatformTarget.containmentMode = EntityContainmentMode.Release;
			}
			Widgets.DrawHighlightIfMouseover(rect6);
			TooltipHandler.TipRegion(rect6, "EntityStudyMode_ReleaseDesc".Translate());
			((Rect)(ref rect6)).y = ((Rect)(ref rect6)).y + 28f;
			if (Widgets.RadioButtonLabeled(rect6, "EntityStudyMode_Execute".Translate(), compHoldingPlatformTarget.containmentMode == EntityContainmentMode.Execute, !compHoldingPlatformTarget.Props.canBeExecuted))
			{
				if (!compHoldingPlatformTarget.Props.canBeExecuted)
				{
					Messages.Message("CantBeExecuted".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					compHoldingPlatformTarget.containmentMode = EntityContainmentMode.Execute;
				}
			}
			Widgets.DrawHighlightIfMouseover(rect6);
			TooltipHandler.TipRegion(rect6, "EntityStudyMode_ExecuteDesc".Translate() + (compHoldingPlatformTarget.Props.canBeExecuted ? "" : ("\n\n" + "CantBeExecuted".Translate().ToString())));
			((Rect)(ref rect6)).y = ((Rect)(ref rect6)).y + 28f;
			Widgets.EndGroup();
			listing_Standard.Gap();
			height = 48f;
			Rect rect7 = listing_Standard.GetRect(height).Rounded();
			Widgets.DrawMenuSection(rect7);
			rect5 = rect7.ContractedBy(10f);
			Widgets.BeginGroup(rect5);
			string text3 = null;
			if (!ResearchProjectDefOf.BioferriteExtraction.IsFinished)
			{
				text3 = "RequiresBioferriteExtraction".Translate();
			}
			else
			{
				Building_HoldingPlatform heldPlatform = compHoldingPlatformTarget.HeldPlatform;
				if (heldPlatform != null && heldPlatform.HasAttachedBioferriteHarvester)
				{
					text3 = "BioferriteHarvesterAttached".Translate();
				}
			}
			Rect rect8 = new Rect(0f, 0f, ((Rect)(ref rect5)).width, 28f);
			Widgets.CheckboxLabeled(rect8, "EntityStudyMode_Extract".Translate(), ref compHoldingPlatformTarget.extractBioferrite, text3 != null);
			Widgets.DrawHighlightIfMouseover(rect8);
			TaggedString taggedString2 = "EntityStudyMode_ExtractDesc".Translate();
			if (text3 != null)
			{
				taggedString2 += "\n\n" + text3.Colorize(ColoredText.WarningColor);
			}
			TooltipHandler.TipRegion(rect8, taggedString2);
			Widgets.EndGroup();
		}
		listing_Standard.End();
		size = new Vector2(280f, listing_Standard.CurHeight + 10f + 24f);
	}

	public static void DoStudyPeriodListing(Listing_Standard listing, CompStudiable studiable)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (studiable.Props.frequencyTicks > 0)
		{
			Widgets.DrawHighlightIfMouseover(listing.Label("StudyInterval".Translate() + ": " + studiable.Props.frequencyTicks.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor), -1f, "StudyIntervalDesc".Translate()));
		}
	}

	public static void DoKnowledgeGainListing(Listing_Standard listing, CompStudiable studiable)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		string text = "StudyKnowledgeGainDesc".Translate();
		CompHoldingPlatformTarget compHoldingPlatformTarget = studiable.Pawn.TryGetComp<CompHoldingPlatformTarget>();
		string text2 = "";
		if (compHoldingPlatformTarget != null && compHoldingPlatformTarget.CurrentlyHeldOnPlatform)
		{
			CompEntityHolder comp = compHoldingPlatformTarget.HeldPlatform.GetComp<CompEntityHolder>();
			float studyKnowledgeAmountMultiplier = ContainmentUtility.GetStudyKnowledgeAmountMultiplier(studiable.Pawn, comp);
			text2 += string.Format("  - {0}: x{1:F1}", "FactorContainmentStrength".Translate(), studyKnowledgeAmountMultiplier);
			if (compHoldingPlatformTarget.HeldPlatform.HasAttachedElectroharvester)
			{
				text2 += string.Format("\n  - {0}: x{1:F1}", "FactorElectroharvester".Translate(), 0.5f);
			}
		}
		if (studiable.Pawn.TryGetComp<CompActivity>(out var comp2))
		{
			if (text2 != "")
			{
				text2 += "\n";
			}
			text2 += string.Format("  - {0}: x{1:F1}", "FactorActivity".Translate(), comp2.ActivityResearchFactor);
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text = text + "\n\n" + text2;
		}
		Widgets.DrawHighlightIfMouseover(listing.Label("StudyKnowledgeGain".Translate() + ": " + (studiable.AdjustedAnomalyKnowledgePerStudy * 5f).ToStringDecimalIfSmall() + " (" + studiable.KnowledgeCategory.label + ")", -1f, text));
	}
}
