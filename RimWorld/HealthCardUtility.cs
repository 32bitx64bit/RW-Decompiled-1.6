using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class HealthCardUtility
{
	private static Vector2 scrollPosition = Vector2.zero;

	private static float scrollViewHeight = 0f;

	private static bool highlight = true;

	private static bool onOperationTab = false;

	private static Vector2 billsScrollPosition = Vector2.zero;

	private static float billsScrollHeight = 1000f;

	private static bool showAllHediffs = false;

	private static bool showHediffsDebugInfo = false;

	private static float lastMaxIconsTotalWidth;

	private const float TopPadding = 20f;

	private const float ThoughtLevelHeight = 25f;

	private const float ThoughtLevelSpacing = 4f;

	private const float IconSize = 20f;

	private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private static readonly Color StaticHighlightColor = new Color(0.75f, 0.75f, 0.85f, 1f);

	private static readonly Color MediumPainColor = new Color(0.9f, 0.9f, 0f);

	private static readonly Color SeverePainColor = new Color(0.9f, 0.5f, 0f);

	private static readonly Texture2D BleedingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Bleeding");

	private const int HideBloodLossTicksThreshold = 60000;

	private static readonly Dictionary<EfficiencyEstimate, Color> efficiencyToColor = new Dictionary<EfficiencyEstimate, Color>
	{
		{
			EfficiencyEstimate.None,
			ColorLibrary.RedReadable
		},
		{
			EfficiencyEstimate.VeryPoor,
			new Color(0.75f, 0.45f, 0.45f)
		},
		{
			EfficiencyEstimate.Poor,
			new Color(0.55f, 0.55f, 0.55f)
		},
		{
			EfficiencyEstimate.Weakened,
			new Color(0.7f, 0.7f, 0.7f)
		},
		{
			EfficiencyEstimate.GoodCondition,
			HealthUtility.GoodConditionColor
		},
		{
			EfficiencyEstimate.Enhanced,
			new Color(0.5f, 0.5f, 0.9f)
		}
	};

	private static List<Hediff> tmpHediffImpactors = new List<Hediff>();

	private static List<BodyPartRecord> tmpBodyPartImpactors = new List<BodyPartRecord>();

	private static List<Gene> tmpGeneImpactors = new List<Gene>();

	public static void DrawPawnHealthCard(Rect outRect, Pawn pawn, bool allowOperations, bool showBloodLoss, Thing thingForMedBills)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.Dead && allowOperations)
		{
			Log.Error("Called DrawPawnHealthCard with a dead pawn and allowOperations=true. Operations are disallowed on corpses.");
			allowOperations = false;
		}
		((Rect)(ref outRect)).y = ((Rect)(ref outRect)).y + 20f;
		((Rect)(ref outRect)).height = ((Rect)(ref outRect)).height - 20f;
		outRect = outRect.Rounded();
		Rect rect = GenUI.Rounded(new Rect(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width * 0.375f, ((Rect)(ref outRect)).height));
		Rect rect2 = new Rect(((Rect)(ref rect)).xMax, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width - ((Rect)(ref rect)).width, ((Rect)(ref outRect)).height);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 11f;
		DrawHealthSummary(rect, pawn, allowOperations, thingForMedBills);
		DrawHediffListing(GenUI.ContractedBy(rect2, 10f), pawn, showBloodLoss, 0f, Prefs.DevMode && Current.ProgramState == ProgramState.Playing);
	}

	public static void DrawHealthSummary(Rect rect, Pawn pawn, bool allowOperations, Thing thingForMedBills)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		if (!allowOperations)
		{
			onOperationTab = false;
		}
		Widgets.DrawMenuSection(rect);
		List<TabRecord> list = new List<TabRecord>
		{
			new TabRecord("HealthOverview".Translate(), delegate
			{
				onOperationTab = false;
			}, !onOperationTab)
		};
		if (allowOperations)
		{
			string text = (pawn.RaceProps.IsMechanoid ? "MedicalOperationsMechanoidsShort".Translate() : "MedicalOperationsShort".Translate());
			if (pawn.BillStack.Count > 0)
			{
				text = text + " (" + pawn.BillStack.Count + ")";
			}
			list.Add(new TabRecord(text, delegate
			{
				onOperationTab = true;
			}, onOperationTab));
		}
		TabDrawer.DrawTabs(rect, list);
		rect = rect.ContractedBy(9f);
		Widgets.BeginGroup(rect);
		float curY = 0f;
		Text.Font = GameFont.Medium;
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)1;
		if (onOperationTab)
		{
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.MedicalOperations, KnowledgeAmount.FrameDisplayed);
			curY = DrawMedOperationsTab(rect, pawn, thingForMedBills, curY);
		}
		else
		{
			curY = DrawOverviewTab(rect, pawn, curY);
		}
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
	}

	public static bool AnyHediffsDisplayed(Pawn pawn, bool showBloodLoss)
	{
		return VisibleHediffGroupsInOrder(pawn, showBloodLoss).Any();
	}

	public static void DrawHediffListing(Rect rect, Pawn pawn, bool showBloodLoss, float rowLeftPad = 0f, bool showDebugOptions = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Invalid comparison between Unknown and I4
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Invalid comparison between Unknown and I4
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		if (showDebugOptions)
		{
			DoDebugOptions(rect, pawn);
		}
		Widgets.BeginGroup(rect);
		float lineHeight = Text.LineHeight;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - lineHeight);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollViewHeight);
		Rect rect2 = rect;
		if (((Rect)(ref viewRect)).height > ((Rect)(ref outRect)).height)
		{
			((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - 16f;
		}
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		GUI.color = Color.white;
		float curY = 0f;
		highlight = true;
		bool flag = false;
		lastMaxIconsTotalWidth = 0f;
		foreach (IGrouping<BodyPartRecord, Hediff> item in VisibleHediffGroupsInOrder(pawn, showBloodLoss))
		{
			flag = true;
			DrawHediffRow(rect2, pawn, item, ref curY, rowLeftPad);
		}
		if (!flag)
		{
			Widgets.NoneLabelCenteredVertically(new Rect(0f, 0f, ((Rect)(ref viewRect)).width, ((Rect)(ref outRect)).height), "(" + "NoHealthConditions".Translate() + ")");
			curY = ((Rect)(ref outRect)).height - 1f;
		}
		if ((int)Event.current.type == 7)
		{
			scrollViewHeight = curY;
		}
		else if ((int)Event.current.type == 8)
		{
			scrollViewHeight = Mathf.Max(scrollViewHeight, curY);
		}
		Widgets.EndScrollView();
		float bleedRateTotal = pawn.health.hediffSet.BleedRateTotal;
		if (bleedRateTotal > 0.01f)
		{
			Rect rect3 = new Rect(0f, ((Rect)(ref rect)).height - lineHeight, ((Rect)(ref rect)).width, lineHeight);
			string text = "BleedingRate".Translate() + ": " + bleedRateTotal.ToStringPercent() + "/" + "LetterDay".Translate();
			int num = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);
			text = ((ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.HasActiveGene(GeneDefOf.Deathless)) ? ((string)(text + (" (" + "Deathless".Translate() + ")"))) : ((num >= 60000) ? ((string)(text + (" (" + "WontBleedOutSoon".Translate() + ")"))) : ((string)(text + (" (" + "TimeToDeath".Translate(num.ToStringTicksToPeriod()) + ")")))));
			Widgets.Label(rect3, text);
		}
		Widgets.EndGroup();
		GUI.color = Color.white;
	}

	private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> VisibleHediffGroupsInOrder(Pawn pawn, bool showBloodLoss)
	{
		foreach (IGrouping<BodyPartRecord, Hediff> item in from x in VisibleHediffs(pawn, showBloodLoss)
			group x by x.Part into x
			orderby GetListPriority(x.First().Part) descending
			select x)
		{
			yield return item;
		}
	}

	private static float GetListPriority(BodyPartRecord rec)
	{
		if (rec == null)
		{
			return 9999999f;
		}
		return (float)((int)rec.height * 10000) + rec.coverageAbsWithChildren;
	}

	private static IEnumerable<Hediff> VisibleHediffs(Pawn pawn, bool showBloodLoss)
	{
		if (!showAllHediffs)
		{
			List<Hediff_MissingPart> mpca = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
			for (int i = 0; i < mpca.Count; i++)
			{
				yield return mpca[i];
			}
			IEnumerable<Hediff> enumerable = pawn.health.hediffSet.hediffs.Where(delegate(Hediff d)
			{
				if (d is Hediff_MissingPart)
				{
					return false;
				}
				if (!d.Visible)
				{
					return false;
				}
				return (showBloodLoss || d.def != HediffDefOf.BloodLoss) ? true : false;
			});
			foreach (Hediff item in enumerable)
			{
				yield return item;
			}
			yield break;
		}
		foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
		{
			yield return hediff;
		}
	}

	private static float DrawMedOperationsTab(Rect leftRect, Pawn pawn, Thing thingForMedBills, float curY)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		curY += 2f;
		Func<List<FloatMenuOption>> recipeOptionsMaker = delegate
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			int num = 0;
			foreach (RecipeDef allRecipe in thingForMedBills.def.AllRecipes)
			{
				if (allRecipe.AvailableNow)
				{
					AcceptanceReport report = allRecipe.Worker.AvailableReport(pawn);
					if (report.Accepted || !report.Reason.NullOrEmpty())
					{
						IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, thingForMedBills.MapHeld);
						if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
						{
							if (allRecipe.targetsBodyPart)
							{
								foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
								{
									if (allRecipe.AvailableOnNow(pawn, item))
									{
										list.Add(GenerateSurgeryOption(pawn, thingForMedBills, allRecipe, enumerable, report, num, item));
										num++;
									}
								}
							}
							else if (!pawn.health.hediffSet.HasHediff(allRecipe.addsHediff))
							{
								list.Add(GenerateSurgeryOption(pawn, thingForMedBills, allRecipe, enumerable, report, num));
								num++;
							}
						}
					}
				}
			}
			return list;
		};
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref leftRect)).x - 9f, curY, ((Rect)(ref leftRect)).width, ((Rect)(ref leftRect)).height - curY - 20f);
		((IBillGiver)thingForMedBills).BillStack.DoListing(rect, recipeOptionsMaker, ref billsScrollPosition, ref billsScrollHeight);
		return curY;
	}

	private static FloatMenuOption GenerateSurgeryOption(Pawn pawn, Thing thingForMedBills, RecipeDef recipe, IEnumerable<ThingDef> missingIngredients, AcceptanceReport report, int index, BodyPartRecord part = null)
	{
		string label = recipe.Worker.GetLabelWhenUsedOn(pawn, part).CapitalizeFirst();
		if (part != null && !recipe.hideBodyPartNames)
		{
			label = label + " (" + part.Label + ")";
		}
		FloatMenuOption floatMenuOption;
		if (!report.Reason.NullOrEmpty())
		{
			label = label + " (" + report.Reason + ")";
			floatMenuOption = new FloatMenuOption(label, null);
		}
		else if (missingIngredients.Any())
		{
			label += " (";
			bool flag = true;
			foreach (ThingDef missingIngredient in missingIngredients)
			{
				if (!flag)
				{
					label += ", ";
				}
				flag = false;
				label += "MissingMedicalBillIngredient".Translate(missingIngredient.label);
			}
			label += ")";
			floatMenuOption = new FloatMenuOption(label, null);
		}
		else
		{
			Action action = delegate
			{
				Pawn medPawn = thingForMedBills as Pawn;
				if (medPawn != null)
				{
					HediffDef hediffDef = recipe.addsHediff ?? recipe.changesHediffLevel;
					if (hediffDef != null)
					{
						TaggedString text = CompRoyalImplant.CheckForViolations(medPawn, hediffDef, recipe.hediffLevelOffset);
						if (!text.NullOrEmpty())
						{
							Find.WindowStack.Add(new Dialog_MessageBox(text, "Yes".Translate(), delegate
							{
								CreateSurgeryBill(medPawn, recipe, part);
							}, "No".Translate()));
						}
						else
						{
							TaggedString confirmation = recipe.Worker.GetConfirmation(medPawn);
							if (!confirmation.NullOrEmpty())
							{
								Find.WindowStack.Add(new Dialog_MessageBox(confirmation, "Yes".Translate(), delegate
								{
									CreateSurgeryBill(medPawn, recipe, part);
								}, "No".Translate()));
							}
							else
							{
								CreateSurgeryBill(medPawn, recipe, part);
							}
						}
					}
					else if (recipe.Worker is Recipe_ImplantXenogerm)
					{
						Find.WindowStack.Add(new Dialog_SelectXenogerm(pawn, pawn.Map, null, delegate(Xenogerm x)
						{
							x.SetTargetPawn(pawn);
						}));
					}
					else
					{
						TaggedString confirmation2 = recipe.Worker.GetConfirmation(medPawn);
						if (!confirmation2.NullOrEmpty())
						{
							Find.WindowStack.Add(new Dialog_MessageBox(confirmation2, "Yes".Translate(), delegate
							{
								CreateSurgeryBill(medPawn, recipe, part);
							}, "No".Translate()));
						}
						else
						{
							CreateSurgeryBill(medPawn, recipe, part);
						}
					}
				}
			};
			floatMenuOption = ((recipe.Worker is Recipe_AdministerIngestible) ? new FloatMenuOption(label, action, recipe.ingredients.FirstOrDefault()?.FixedIngredient) : ((!(recipe.Worker is Recipe_RemoveBodyPart)) ? new FloatMenuOption(label, action, recipe.UIIconThing, recipe.UIIcon) : new FloatMenuOption(label, action, part.def.spawnThingOnRemoved)));
		}
		floatMenuOption.extraPartWidth = 29f;
		floatMenuOption.extraPartOnGUI = (Rect rect) => Widgets.InfoCardButton(((Rect)(ref rect)).x + 5f, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - 24f) / 2f, recipe);
		floatMenuOption.mouseoverGuiAction = delegate(Rect rect)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			BillUtility.DoBillInfoWindow(index, label, rect, recipe, part, pawn);
		};
		return floatMenuOption;
	}

	public static Bill_Medical CreateSurgeryBill(Pawn medPawn, RecipeDef recipe, BodyPartRecord part, List<Thing> uniqueIngredients = null, bool sendMessages = true)
	{
		Bill_Medical bill_Medical = new Bill_Medical(recipe, uniqueIngredients);
		medPawn.BillStack.AddBill(bill_Medical);
		bill_Medical.Part = part;
		if (recipe.conceptLearned != null)
		{
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(recipe.conceptLearned, KnowledgeAmount.Total);
		}
		if (sendMessages)
		{
			Map mapHeld = medPawn.MapHeld;
			if (!(from p in mapHeld.mapPawns.PawnsInFaction(Faction.OfPlayer)
				where p.IsFreeColonist || p.IsColonyMechPlayerControlled
				select p).Any((Pawn col) => recipe.PawnSatisfiesSkillRequirements(col)))
			{
				Bill.CreateNoPawnsWithSkillDialog(recipe);
			}
			if (!medPawn.InBed() && medPawn.RaceProps.IsFlesh)
			{
				if (medPawn.RaceProps.Humanlike)
				{
					if (!mapHeld.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed && RestUtility.CanUseBedEver(medPawn, x.def) && ((Building_Bed)x).Medical))
					{
						Messages.Message("MessageNoMedicalBeds".Translate(), medPawn, MessageTypeDefOf.CautionInput, historical: false);
					}
				}
				else if (!mapHeld.listerBuildings.allBuildingsColonist.Any((Building x) => x is Building_Bed && RestUtility.CanUseBedEver(medPawn, x.def)))
				{
					Messages.Message("MessageNoAnimalBeds".Translate(), medPawn, MessageTypeDefOf.CautionInput, historical: false);
				}
			}
			if (medPawn.Faction != null && !medPawn.Faction.Hidden && !medPawn.Faction.HostileTo(Faction.OfPlayer) && recipe.Worker.IsViolationOnPawn(medPawn, part, Faction.OfPlayer))
			{
				Messages.Message("MessageMedicalOperationWillAngerFaction".Translate(medPawn.HomeFaction), medPawn, MessageTypeDefOf.CautionInput, historical: false);
			}
			if (!CanDoRecipeWithMedicineRestriction(medPawn, recipe))
			{
				Messages.Message("MessageWarningNoMedicineForRestriction".Translate(medPawn.Named("PAWN"), medPawn.playerSettings.medCare.GetLabel().Named("RESTRICTIONLABEL")), medPawn, MessageTypeDefOf.CautionInput, historical: false);
			}
			recipe.Worker.CheckForWarnings(medPawn);
		}
		return bill_Medical;
	}

	private static float DrawOverviewTab(Rect rect, Pawn pawn, float curY)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		curY += 4f;
		bool flag = false;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		GUI.color = new Color(0.9f, 0.9f, 0.9f);
		if (pawn.foodRestriction != null && pawn.foodRestriction.Configurable && !pawn.DevelopmentalStage.Baby() && pawn.needs?.food != null && (!pawn.IsMutant || !pawn.mutant.Def.disablePolicies))
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref rect)).width, 23f);
			flag = true;
			TooltipHandler.TipRegionByKey(val, "FoodRestrictionDescription");
			Widgets.DrawHighlightIfMouseover(val);
			Rect rect2 = val;
			((Rect)(ref rect2)).xMax = ((Rect)(ref val)).center.x - 4f;
			Rect rect3 = val;
			((Rect)(ref rect3)).xMin = ((Rect)(ref val)).center.x + 4f;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, string.Format("{0}:", "AllowFood".Translate()));
			Text.Anchor = (TextAnchor)0;
			if (Widgets.ButtonText(rect3, pawn.foodRestriction.CurrentFoodPolicy.label))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (FoodPolicy restriction in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
				{
					list.Add(new FloatMenuOption(restriction.label, delegate
					{
						pawn.foodRestriction.CurrentFoodPolicy = restriction;
					}));
				}
				list.Add(new FloatMenuOption("ManageFoodPolicies".Translate() + "...", delegate
				{
					Find.WindowStack.Add(new Dialog_ManageFoodPolicies(pawn.foodRestriction.CurrentFoodPolicy));
				}));
				Find.WindowStack.Add(new FloatMenu(list));
			}
			curY += ((Rect)(ref val)).height + 4f;
		}
		bool flag2 = pawn.Faction == Faction.OfPlayer || pawn.HostFaction == Faction.OfPlayer;
		bool flag3 = pawn.NonHumanlikeOrWildMan() && pawn.InBed() && pawn.CurrentBed().Faction == Faction.OfPlayer;
		if (pawn.RaceProps.IsFlesh && (flag2 || flag3) && (!pawn.IsMutant || pawn.mutant.Def.entitledToMedicalCare) && pawn.playerSettings != null && !pawn.Dead && Current.ProgramState == ProgramState.Playing)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(0f, curY, ((Rect)(ref rect)).width, 23f);
			flag = true;
			TooltipHandler.TipRegionByKey(val2, "MedicineQualityDescription");
			Widgets.DrawHighlightIfMouseover(val2);
			Rect rect4 = val2;
			((Rect)(ref rect4)).xMax = ((Rect)(ref val2)).center.x - 4f;
			Rect rect5 = val2;
			((Rect)(ref rect5)).xMin = ((Rect)(ref val2)).center.x + 4f;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect4, string.Format("{0}:", "AllowMedicine".Translate()));
			Text.Anchor = (TextAnchor)0;
			Widgets.DrawButtonGraphic(rect5);
			MedicalCareUtility.MedicalCareSelectButton(rect5, pawn);
			curY += ((Rect)(ref val2)).height + 4f;
		}
		if (Current.ProgramState == ProgramState.Playing && pawn.IsColonist && !pawn.Dead && !pawn.DevelopmentalStage.Baby() && pawn.playerSettings != null)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0f, curY, ((Rect)(ref rect)).width, 23f);
			flag = true;
			TooltipHandler.TipRegion(val3, "AllowSelfTendTip".Translate(Faction.OfPlayer.def.pawnsPlural, 0.7f.ToStringPercent()).CapitalizeFirst());
			Widgets.DrawHighlightIfMouseover(val3);
			Rect rect6 = val3;
			((Rect)(ref rect6)).xMax = ((Rect)(ref val3)).center.x - 4f;
			Rect rect7 = val3;
			((Rect)(ref rect7)).xMin = ((Rect)(ref val3)).center.x + 4f;
			((Rect)(ref rect7)).width = ((Rect)(ref rect7)).height;
			rect7.ContractedBy(4f);
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect6, string.Format("{0}:", "AllowSelfTend".Translate()));
			Text.Anchor = (TextAnchor)0;
			bool selfTend = pawn.playerSettings.selfTend;
			Widgets.Checkbox(((Rect)(ref rect7)).x, ((Rect)(ref rect7)).y, ref pawn.playerSettings.selfTend, ((Rect)(ref rect7)).height);
			if (pawn.playerSettings.selfTend && !selfTend)
			{
				if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
				{
					pawn.playerSettings.selfTend = false;
					Messages.Message("MessageCannotSelfTendEver".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.RejectInput, historical: false);
				}
				else if (pawn.workSettings.GetPriority(WorkTypeDefOf.Doctor) == 0)
				{
					Messages.Message("MessageSelfTendUnsatisfied".Translate(pawn.LabelShort, pawn), MessageTypeDefOf.CautionInput, historical: false);
				}
			}
			curY += ((Rect)(ref val3)).height + 10f;
		}
		if (flag)
		{
			Widgets.DrawLineHorizontal(((Rect)(ref rect)).x - 8f, curY, ((Rect)(ref rect)).width, Color.gray);
		}
		curY += 10f;
		if (pawn.def.race.IsFlesh)
		{
			Pair<string, Color> painLabel = GetPainLabel(pawn);
			string painTip = GetPainTip(pawn);
			DrawLeftRow(rect, ref curY, "PainLevel".Translate(), painLabel.First, painLabel.Second, painTip);
		}
		curY += 6f;
		if (!pawn.Dead)
		{
			IEnumerable<PawnCapacityDef> source = (pawn.def.race.Humanlike ? DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnHumanlikes) : (pawn.def.race.Animal ? DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnAnimals) : (pawn.def.race.IsAnomalyEntity ? DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnAnomalyEntities) : ((!pawn.def.race.IsDrone) ? DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnMechanoids) : DefDatabase<PawnCapacityDef>.AllDefs.Where((PawnCapacityDef x) => x.showOnDrones)))));
			foreach (PawnCapacityDef item in source.OrderBy((PawnCapacityDef act) => act.listOrder))
			{
				PawnCapacityDef activityLocal;
				if (PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, item))
				{
					activityLocal = item;
					Pair<string, Color> efficiencyLabel = GetEfficiencyLabel(pawn, item);
					DrawLeftRow(rect, ref curY, item.GetLabelFor(pawn).CapitalizeFirst(), efficiencyLabel.First, efficiencyLabel.Second, new TipSignal(TipGetter, pawn.thingIDNumber ^ item.index));
				}
				string TipGetter()
				{
					if (!pawn.Dead)
					{
						return GetPawnCapacityTip(pawn, activityLocal);
					}
					return "";
				}
			}
		}
		return curY;
	}

	private static void DrawLeftRow(Rect rect, ref float curY, string leftLabel, string rightLabel, Color rightLabelColor, TipSignal tipSignal)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(17f, curY, ((Rect)(ref rect)).width - 34f - 10f, 22f);
		if (Mouse.IsOver(val))
		{
			using (new TextBlock(HighlightColor))
			{
				GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
			}
		}
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(val, leftLabel);
		GUI.color = rightLabelColor;
		Text.Anchor = (TextAnchor)5;
		Widgets.Label(val, rightLabel);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, curY, ((Rect)(ref rect)).width, 20f);
		if (Mouse.IsOver(rect2))
		{
			TooltipHandler.TipRegion(rect2, tipSignal);
		}
		curY += ((Rect)(ref val)).height;
	}

	private static void DrawHediffRow(Rect rect, Pawn pawn, IEnumerable<Hediff> diffs, ref float curY, float rowLeftPad = 0f)
	{
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).width * 0.375f;
		float num2 = ((Rect)(ref rect)).width - num - lastMaxIconsTotalWidth;
		BodyPartRecord part = diffs.First().Part;
		float num3 = ((part != null) ? Text.CalcHeight(part.LabelCap, num) : Text.CalcHeight("WholeBody".Translate(), num));
		float num4 = 0f;
		float num5 = curY;
		float num6 = 0f;
		foreach (IGrouping<int, Hediff> item in from x in diffs
			group x by x.UIGroupKey)
		{
			int num7 = item.Count();
			string text = item.First().LabelCap;
			if (num7 != 1)
			{
				text = text + " x" + num7;
			}
			num6 += Text.CalcHeight(text, num2);
		}
		num4 = num6;
		DoRightRowHighlight(new Rect(0f, curY, ((Rect)(ref rect)).width, Mathf.Max(num3, num4)));
		if (part != null)
		{
			GUI.color = HealthUtility.GetPartConditionLabel(pawn, part).Second;
			Widgets.Label(new Rect(rowLeftPad, curY, num, 100f), part.LabelCap);
		}
		else
		{
			GUI.color = HealthUtility.RedColor;
			Widgets.Label(new Rect(rowLeftPad, curY, num, 100f), "WholeBody".Translate());
		}
		GUI.color = Color.white;
		Rect val = default(Rect);
		foreach (IGrouping<int, Hediff> item2 in from x in diffs
			group x by x.UIGroupKey)
		{
			int num8 = 0;
			Hediff hediff2 = null;
			Texture2D bleedingIcon = null;
			TextureAndColor stateIcon = null;
			float totalBleedRate = 0f;
			foreach (Hediff item3 in item2)
			{
				if (num8 == 0)
				{
					hediff2 = item3;
				}
				stateIcon = item3.StateIcon;
				if (item3.Bleeding)
				{
					bleedingIcon = BleedingIcon;
				}
				totalBleedRate += item3.BleedRate;
				num8++;
			}
			string text2 = hediff2.LabelCap;
			if (num8 != 1)
			{
				text2 = text2 + " x" + num8.ToStringCached();
			}
			float num9 = Text.CalcHeight(text2, num2);
			((Rect)(ref val))._002Ector(num, curY, num2, num9);
			Widgets.DrawHighlightIfMouseover(val);
			GUI.color = hediff2.LabelColor;
			Widgets.Label(val, text2);
			GUI.color = Color.white;
			Rect iconsRect = new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y, ((Rect)(ref rect)).width - num - 10f, ((Rect)(ref val)).height);
			List<GenUI.AnonymousStackElement> list = new List<GenUI.AnonymousStackElement>();
			Hediff localHediff = hediff2;
			if (DebugSettings.ShowDevGizmos && Current.ProgramState == ProgramState.Playing)
			{
				list.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0064: Unknown result type (might be due to invalid IL or missing references)
						//IL_008e: Unknown result type (might be due to invalid IL or missing references)
						//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
						float num16 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
						((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num16, ((Rect)(ref r)).y, 20f, 20f);
						GUI.color = Color.red;
						TooltipHandler.TipRegion(r, () => "DEV: Remove hediff", 1071045645);
						if (GUI.Button(r, (Texture)(object)TexButton.Delete))
						{
							pawn.health.RemoveHediff(localHediff);
						}
						GUI.color = Color.white;
					},
					width = 20f
				});
				if (localHediff.def.maxSeverity < float.MaxValue || localHediff.def.lethalSeverity > 0f)
				{
					list.Add(new GenUI.AnonymousStackElement
					{
						drawer = delegate(Rect r)
						{
							//IL_005a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0064: Unknown result type (might be due to invalid IL or missing references)
							//IL_008e: Unknown result type (might be due to invalid IL or missing references)
							//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
							float num15 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
							((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num15, ((Rect)(ref r)).y, 20f, 20f);
							GUI.color = Color.cyan;
							TooltipHandler.TipRegion(r, () => "DEV: Set severity", 2131648723);
							if (GUI.Button(r, (Texture)(object)TexButton.Save))
							{
								Find.WindowStack.Add(new Dialog_DebugSetSeverity(localHediff));
							}
							GUI.color = Color.white;
						},
						width = 20f
					});
				}
				if (localHediff.TryGetComp<HediffComp_Disappears>() != null)
				{
					list.Add(new GenUI.AnonymousStackElement
					{
						drawer = delegate(Rect r)
						{
							//IL_005a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0064: Unknown result type (might be due to invalid IL or missing references)
							//IL_008e: Unknown result type (might be due to invalid IL or missing references)
							//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
							float num14 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
							((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num14, ((Rect)(ref r)).y, 20f, 20f);
							GUI.color = Color.yellow;
							TooltipHandler.TipRegion(r, () => "DEV: Set remaining time", 6234623);
							if (GUI.Button(r, (Texture)(object)TexButton.Save))
							{
								Find.WindowStack.Add(new Dialog_DebugSetHediffRemaining(localHediff));
							}
							GUI.color = Color.white;
						},
						width = 20f
					});
				}
			}
			list.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					float num13 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
					((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num13, ((Rect)(ref r)).y, 20f, 20f);
					Widgets.InfoCardButton(r, localHediff);
				},
				width = 20f
			});
			if (Object.op_Implicit((Object)(object)bleedingIcon))
			{
				list.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_004b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0075: Unknown result type (might be due to invalid IL or missing references)
						float num12 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
						((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num12, ((Rect)(ref r)).y, 20f, 20f);
						GUI.DrawTexture(r.ContractedBy(GenMath.LerpDouble(0f, 0.6f, 5f, 0f, Mathf.Min(totalBleedRate, 1f))), (Texture)(object)bleedingIcon);
					},
					width = 20f
				});
			}
			if (stateIcon.HasValue)
			{
				list.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_0051: Unknown result type (might be due to invalid IL or missing references)
						//IL_005b: Unknown result type (might be due to invalid IL or missing references)
						//IL_006c: Unknown result type (might be due to invalid IL or missing references)
						float num11 = ((Rect)(ref iconsRect)).width - (((Rect)(ref r)).x - ((Rect)(ref iconsRect)).x) - 20f;
						((Rect)(ref r))._002Ector(((Rect)(ref iconsRect)).x + num11, ((Rect)(ref r)).y, 20f, 20f);
						GUI.color = stateIcon.Color;
						GUI.DrawTexture(r, (Texture)(object)stateIcon.Texture);
						GUI.color = Color.white;
					},
					width = 20f
				});
			}
			GenUI.DrawElementStack(iconsRect, num9, list, delegate(Rect r, GenUI.AnonymousStackElement obj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				obj.drawer(r);
			}, (GenUI.AnonymousStackElement obj) => obj.width);
			lastMaxIconsTotalWidth = Mathf.Max(lastMaxIconsTotalWidth, list.Sum((GenUI.AnonymousStackElement x) => x.width + 5f) - 5f);
			if (Mouse.IsOver(val))
			{
				int num10 = 0;
				foreach (Hediff hediff in item2)
				{
					TooltipHandler.TipRegion(val, new TipSignal(() => hediff.GetTooltip(pawn, showHediffsDebugInfo), (int)curY + 7857 + num10++, TooltipPriority.Default));
				}
				if (part != null)
				{
					TooltipHandler.TipRegion(val, new TipSignal(() => GetTooltip(pawn, part), (int)curY + 7857 + ++num10, TooltipPriority.Pawn));
					if (DebugViewSettings.drawWoundAnchorsOnHover && pawn.Drawer.renderer.WoundOverlays.debugDrawPart != part)
					{
						pawn.Drawer.renderer.WoundOverlays.debugDrawPart = part;
						pawn.Drawer.renderer.WoundOverlays.ClearCache();
						PortraitsCache.SetDirty(pawn);
						GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
					}
				}
			}
			if (Widgets.ButtonInvisible(val, CanEntryBeClicked(item2, pawn)))
			{
				EntryClicked(item2, pawn);
			}
			curY += num9;
		}
		GUI.color = Color.white;
		curY = num5 + Mathf.Max(num3, num4);
	}

	private static bool CanDoRecipeWithMedicineRestriction(IBillGiver giver, RecipeDef recipe)
	{
		if (!(giver is Pawn { playerSettings: not null } pawn))
		{
			return true;
		}
		if (!recipe.ingredients.Any((IngredientCount x) => x.filter.AnyAllowedDef.IsMedicine))
		{
			return true;
		}
		MedicalCareCategory medicalCareCategory = WorkGiver_DoBill.GetMedicalCareCategory(pawn);
		foreach (Thing item in pawn.MapHeld.listerThings.ThingsInGroup(ThingRequestGroup.Medicine))
		{
			foreach (IngredientCount ingredient in recipe.ingredients)
			{
				if (ingredient.filter.Allows(item) && medicalCareCategory.AllowsMedicine(item.def))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string GetPainTip(Pawn pawn)
	{
		return "PainLevel".Translate() + ": " + (pawn.health.hediffSet.PainTotal * 100f).ToString("F0") + "%";
	}

	private static string GetTooltip(Pawn pawn, BodyPartRecord part)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(part.LabelCap + ": ");
		stringBuilder.AppendLine(" " + pawn.health.hediffSet.GetPartHealth(part) + " / " + part.def.GetMaxHealth(pawn));
		float num = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part);
		if (num != 1f)
		{
			stringBuilder.AppendLine("Efficiency".Translate() + ": " + num.ToStringPercent());
		}
		return stringBuilder.ToString();
	}

	public static string GetPawnCapacityTip(Pawn pawn, PawnCapacityDef capacity)
	{
		List<PawnCapacityUtility.CapacityImpactor> list = new List<PawnCapacityUtility.CapacityImpactor>();
		float eff = PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, list);
		list.RemoveAll((PawnCapacityUtility.CapacityImpactor x) => x is PawnCapacityUtility.CapacityImpactorCapacity capacityImpactorCapacity && !capacityImpactorCapacity.capacity.CanShowOnPawn(pawn));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(capacity.GetLabelFor(pawn).CapitalizeFirst() + ": " + GetEfficiencyEstimateLabel(eff));
		if (list.Count > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("AffectedBy".Translate());
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is PawnCapacityUtility.CapacityImpactorHediff capacityImpactorHediff && !tmpHediffImpactors.Contains(capacityImpactorHediff.hediff))
				{
					stringBuilder.AppendLine($"  {list[i].Readable(pawn)}");
					tmpHediffImpactors.Add(capacityImpactorHediff.hediff);
				}
			}
			tmpHediffImpactors.Clear();
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j] is PawnCapacityUtility.CapacityImpactorBodyPartHealth capacityImpactorBodyPartHealth && !tmpBodyPartImpactors.Contains(capacityImpactorBodyPartHealth.bodyPart))
				{
					stringBuilder.AppendLine($"  {list[j].Readable(pawn)}");
					tmpBodyPartImpactors.Add(capacityImpactorBodyPartHealth.bodyPart);
				}
			}
			tmpBodyPartImpactors.Clear();
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k] is PawnCapacityUtility.CapacityImpactorGene capacityImpactorGene && !tmpGeneImpactors.Contains(capacityImpactorGene.gene))
				{
					stringBuilder.AppendLine($"  {list[k].Readable(pawn)}");
					tmpGeneImpactors.Add(capacityImpactorGene.gene);
				}
			}
			tmpGeneImpactors.Clear();
			for (int l = 0; l < list.Count; l++)
			{
				if (list[l] is PawnCapacityUtility.CapacityImpactorCapacity)
				{
					stringBuilder.AppendLine($"  {list[l].Readable(pawn)}");
				}
			}
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m] is PawnCapacityUtility.CapacityImpactorPain)
				{
					stringBuilder.AppendLine($"  {list[m].Readable(pawn)}");
				}
			}
		}
		return stringBuilder.ToString();
	}

	private static bool CanEntryBeClicked(IEnumerable<Hediff> diffs, Pawn pawn)
	{
		if (!GetCombatLogInfo(diffs, out var _, out var combatLogEntry) || combatLogEntry == null)
		{
			return false;
		}
		if (!Find.BattleLog.Battles.Any((Battle b) => b.Concerns(pawn) && b.Entries.Any((LogEntry e) => e == combatLogEntry)))
		{
			return false;
		}
		return true;
	}

	private static void EntryClicked(IEnumerable<Hediff> diffs, Pawn pawn)
	{
		if (GetCombatLogInfo(diffs, out var _, out var combatLogEntry) && combatLogEntry != null && Find.BattleLog.Battles.Any((Battle b) => b.Concerns(pawn) && b.Entries.Any((LogEntry e) => e == combatLogEntry)) && InspectPaneUtility.OpenTab(typeof(ITab_Pawn_Log)) is ITab_Pawn_Log tab_Pawn_Log)
		{
			tab_Pawn_Log.SeekTo(combatLogEntry);
			tab_Pawn_Log.Highlight(combatLogEntry);
		}
	}

	public static bool GetCombatLogInfo(IEnumerable<Hediff> diffs, out TaggedString combatLogText, out LogEntry combatLogEntry)
	{
		combatLogText = null;
		combatLogEntry = null;
		foreach (Hediff diff in diffs)
		{
			if ((diff.combatLogEntry != null && diff.combatLogEntry.Target != null) || (combatLogText.NullOrEmpty() && !diff.combatLogText.NullOrEmpty()))
			{
				combatLogEntry = ((diff.combatLogEntry != null) ? diff.combatLogEntry.Target : null);
				combatLogText = diff.combatLogText;
			}
			if (combatLogEntry != null)
			{
				return true;
			}
		}
		return false;
	}

	private static void DoRightRowHighlight(Rect rowRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (highlight)
		{
			GUI.color = StaticHighlightColor;
			GUI.DrawTexture(rowRect, (Texture)(object)TexUI.HighlightTex);
		}
		highlight = !highlight;
	}

	private static void DoDebugOptions(Rect rightRect, Pawn pawn)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!Widgets.ButtonText(new Rect(((Rect)(ref rightRect)).x + 240f, ((Rect)(ref rightRect)).y - 27f, 115f, 25f), "Dev tool..."))
		{
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>(6);
		list.Add(new FloatMenuOption("Add hediff", delegate
		{
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugTools_Health.Options_AddHediff(pawn)));
		}));
		FloatMenuOption floatMenuOption = FloatMenuOption.CheckboxLabeled("Hediff debug tooltips", delegate
		{
			showHediffsDebugInfo = !showHediffsDebugInfo;
		}, showHediffsDebugInfo);
		floatMenuOption.tooltip = "Hover over hediffs in the health window to get extra debug information about them.";
		list.Add(floatMenuOption);
		list.Add(FloatMenuOption.CheckboxLabeled("Show hidden Hediffs", delegate
		{
			showAllHediffs = !showAllHediffs;
		}, showAllHediffs));
		list.Add(new FloatMenuOption("Table: BodyPartRecord info", delegate
		{
			float totalCorpseNutrition = 0f;
			if (pawn.RaceProps.hasCorpse)
			{
				totalCorpseNutrition = StatDefOf.Nutrition.Worker.GetValueAbstract(pawn.RaceProps.corpseDef);
			}
			DebugTables.MakeTablesDialog(pawn.RaceProps.body.AllParts, new TableDataGetter<BodyPartRecord>("defName", (BodyPartRecord b) => b.def.defName), new TableDataGetter<BodyPartRecord>("Coverage", (BodyPartRecord b) => pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(b).ToStringPercent()), new TableDataGetter<BodyPartRecord>("Hit chance\n(this or any child)", (BodyPartRecord b) => b.coverageAbsWithChildren.ToStringPercent()), new TableDataGetter<BodyPartRecord>("Hit chance\n(this part)", (BodyPartRecord b) => b.coverageAbs.ToStringPercent()), new TableDataGetter<BodyPartRecord>("Efficiency", (BodyPartRecord b) => PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, b).ToStringPercent()), new TableDataGetter<BodyPartRecord>("Nutrition", (BodyPartRecord b) => (!(totalCorpseNutrition > 0f)) ? 0f : FoodUtility.GetBodyPartNutrition(totalCorpseNutrition, pawn, b)), new TableDataGetter<BodyPartRecord>("Solid", (BodyPartRecord b) => (!pawn.health.hediffSet.PartIsMissing(b)) ? b.def.IsSolid(b, pawn.health.hediffSet.hediffs).ToStringCheckBlank() : ""), new TableDataGetter<BodyPartRecord>("Skin covered", (BodyPartRecord b) => (!pawn.health.hediffSet.PartIsMissing(b)) ? b.def.IsSkinCovered(b, pawn.health.hediffSet).ToStringCheckBlank() : ""), new TableDataGetter<BodyPartRecord>("Is missing", (BodyPartRecord b) => pawn.health.hediffSet.PartIsMissing(b).ToStringCheckBlank()), new TableDataGetter<BodyPartRecord>("Is missing parts", (BodyPartRecord b) => pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(b).ToStringCheckBlank()));
		}));
		list.Add(new FloatMenuOption("Table: BodyPartGroupDef info", delegate
		{
			DebugTables.MakeTablesDialog(DefDatabase<BodyPartGroupDef>.AllDefs.Where((BodyPartGroupDef x) => pawn.RaceProps.body.AllParts.Any((BodyPartRecord y) => y.groups.Contains(x))), new TableDataGetter<BodyPartGroupDef>("defName", (BodyPartGroupDef b) => b.defName), new TableDataGetter<BodyPartGroupDef>("Efficiency", (BodyPartGroupDef b) => PawnCapacityUtility.CalculateNaturalPartsAverageEfficiency(pawn.health.hediffSet, b).ToStringPercent()));
		}));
		list.Add(new FloatMenuOption("Table: HediffGiver_Birthday", delegate
		{
			List<TableDataGetter<HediffGiver_Birthday>> list3 = new List<TableDataGetter<HediffGiver_Birthday>>
			{
				new TableDataGetter<HediffGiver_Birthday>("label", (HediffGiver_Birthday b) => b.hediff.LabelCap)
			};
			for (int j = 1; (float)j < pawn.RaceProps.lifeExpectancy + 20f; j++)
			{
				int age2 = j;
				list3.Add(new TableDataGetter<HediffGiver_Birthday>("Chance at\n" + j, (HediffGiver_Birthday h) => h.DebugChanceToHaveAtAge(pawn, age2).ToStringPercent()));
			}
			list3.Add(new TableDataGetter<HediffGiver_Birthday>("Spacing", (HediffGiver_Birthday h) => ""));
			for (int k = 1; (float)k < pawn.RaceProps.lifeExpectancy + 20f; k++)
			{
				int age = k;
				list3.Add(new TableDataGetter<HediffGiver_Birthday>("Count at\n" + k, delegate
				{
					float num = 0f;
					foreach (HediffGiverSetDef hediffGiverSet in pawn.RaceProps.hediffGiverSets)
					{
						foreach (HediffGiver_Birthday item in hediffGiverSet.hediffGivers.OfType<HediffGiver_Birthday>())
						{
							num += item.DebugChanceToHaveAtAge(pawn, age);
						}
					}
					return num.ToStringDecimalIfSmall();
				}));
			}
			DebugTables.MakeTablesDialog(pawn.RaceProps.hediffGiverSets.SelectMany((HediffGiverSetDef x) => x.hediffGivers.OfType<HediffGiver_Birthday>()), list3.ToArray());
		}));
		if (pawn.health.immunity.ImmunityListForReading.Any())
		{
			list.Add(new FloatMenuOption("Table: Immunities", delegate
			{
				List<TableDataGetter<ImmunityRecord>> list2 = new List<TableDataGetter<ImmunityRecord>>
				{
					new TableDataGetter<ImmunityRecord>("hediff", (ImmunityRecord i) => i.hediffDef.LabelCap),
					new TableDataGetter<ImmunityRecord>("source", (ImmunityRecord i) => i.source.LabelCap),
					new TableDataGetter<ImmunityRecord>("immunity", (ImmunityRecord i) => i.immunity.ToStringPercent())
				};
				DebugTables.MakeTablesDialog(pawn.health.immunity.ImmunityListForReading, list2.ToArray());
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list));
	}

	public static Pair<string, Color> GetEfficiencyLabel(Pawn pawn, PawnCapacityDef activity)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		float level = pawn.health.capacities.GetLevel(activity);
		return new Pair<string, Color>(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, activity).ToStringPercent(), efficiencyToColor[EfficiencyValueToEstimate(level)]);
	}

	public static string GetEfficiencyEstimateLabel(float eff)
	{
		return EfficiencyValueToEstimate(eff).ToString().Translate();
	}

	public static EfficiencyEstimate EfficiencyValueToEstimate(float eff)
	{
		if (eff <= 0f)
		{
			return EfficiencyEstimate.None;
		}
		if (eff < 0.4f)
		{
			return EfficiencyEstimate.VeryPoor;
		}
		if (eff < 0.7f)
		{
			return EfficiencyEstimate.Poor;
		}
		if (eff < 1f && !Mathf.Approximately(eff, 1f))
		{
			return EfficiencyEstimate.Weakened;
		}
		if (Mathf.Approximately(eff, 1f))
		{
			return EfficiencyEstimate.GoodCondition;
		}
		return EfficiencyEstimate.Enhanced;
	}

	public static Pair<string, Color> GetPainLabel(Pawn pawn)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		float painTotal = pawn.health.hediffSet.PainTotal;
		string text = "";
		Color white = Color.white;
		if (Mathf.Approximately(painTotal, 0f))
		{
			text = "NoPain".Translate();
			white = HealthUtility.GoodConditionColor;
		}
		else if (painTotal < 0.15f)
		{
			text = "LittlePain".Translate();
			white = Color.gray;
		}
		else if (painTotal < 0.4f)
		{
			text = "MediumPain".Translate();
			white = MediumPainColor;
		}
		else if (painTotal < 0.8f)
		{
			text = "SeverePain".Translate();
			white = SeverePainColor;
		}
		else
		{
			text = "ExtremePain".Translate();
			white = HealthUtility.RedColor;
		}
		return new Pair<string, Color>(text, white);
	}

	public static bool ShowBloodLoss(Thing thing)
	{
		if (thing is Corpse corpse)
		{
			return corpse.Age < 60000;
		}
		return true;
	}
}
