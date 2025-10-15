using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Pawn_Feeding : ITab
{
	private struct BabyFeederPair
	{
		public Pawn baby;

		public Pawn feeder;
	}

	private const float FeederNameWidthPx = 220f;

	private const float BreastfeedOptWidthPx = 120f;

	private const int contextHash = 2074307015;

	public const float TabWidth = 500f;

	private Vector2 scrollPositionAutoBreastfeed = Vector2.zero;

	private Vector2 scrollPositionBabyConsumables = Vector2.zero;

	private static List<ThingDef> babyConsumableFoods = new List<ThingDef>();

	private static List<Pawn> tmpAllowedFeedingPawns = new List<Pawn>(8);

	public override bool IsVisible
	{
		get
		{
			if (ChildcareUtility.CanSuckle(SelPawn, out var _))
			{
				return ChildcareUtility.HasBreastfeedCompatibleFactions(Faction.OfPlayer, SelPawn);
			}
			return false;
		}
	}

	public static List<ThingDef> BabyConsumableFoods
	{
		get
		{
			if (babyConsumableFoods.Count == 0)
			{
				babyConsumableFoods.AddRange(DefDatabase<ThingDef>.AllDefs.Where((ThingDef t) => t.ingestible != null && t.ingestible.babiesCanIngest));
			}
			return babyConsumableFoods;
		}
	}

	public ITab_Pawn_Feeding()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(500f, 500f);
		labelKey = "TabFeeding";
		tutorTag = "Feeding";
	}

	public static void FillTab(Pawn baby, Rect rect, ref Vector2 scrollPositionAutoBreastfeed, ref Vector2 scrollPositionBabyConsumables, List<Pawn> allowedPawns = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		RectDivider divider = new RectDivider(rect.ContractedBy(10f, 10f), 2074307015);
		Rect rect2 = divider.Rect;
		RectDivider divider2 = divider.NewRow(((Rect)(ref rect2)).height * 0.66f);
		Widgets.ListSeparator(ref divider2, "AutofeedSectionHeader".Translate().CapitalizeFirst());
		List<Pawn> list = tmpAllowedFeedingPawns;
		list.Clear();
		list.AddRange(PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_OfPlayerFaction);
		list.Remove(baby);
		list.RemoveAll(delegate(Pawn feeder)
		{
			if (!feeder.RaceProps.Humanlike)
			{
				return true;
			}
			if (ChildcareUtility.CanSuckle(feeder, out var _))
			{
				return true;
			}
			if (feeder.IsWorkTypeDisabledByAge(WorkTypeDefOf.Childcare, out var _))
			{
				return true;
			}
			return (allowedPawns != null && !allowedPawns.Contains(feeder)) ? true : false;
		});
		Pawn surrogate = baby.GetBirthParent();
		Pawn mother = baby.GetMother();
		Pawn father = baby.GetFather();
		list.Sort(delegate(Pawn lhs, Pawn rhs)
		{
			int num = rhs.health.hediffSet.HasHediff(HediffDefOf.Lactating).CompareTo(lhs.health.hediffSet.HasHediff(HediffDefOf.Lactating));
			if (num != 0)
			{
				return num;
			}
			if (lhs == mother)
			{
				return -1;
			}
			if (rhs == mother)
			{
				return 1;
			}
			if (lhs == father)
			{
				return -1;
			}
			if (rhs == father)
			{
				return 1;
			}
			if (lhs == surrogate)
			{
				return -1;
			}
			return (rhs == surrogate) ? 1 : 0;
		});
		if (list.Count == 0)
		{
			using (new TextBlock((TextAnchor)4))
			{
				Widgets.Label(divider2.Rect, "AutofeedNone".Translate());
			}
		}
		else
		{
			using (new TextBlock((TextAnchor)3))
			{
				RectDivider rectDivider = divider2.CreateViewRect(list.Count, 28f);
				Widgets.BeginScrollView(divider2.Rect, ref scrollPositionAutoBreastfeed, rectDivider);
				foreach (Pawn item in list)
				{
					RectDivider row = rectDivider.NewRow(28f);
					DrawRow(baby, ref row, item, mother, father, surrogate);
				}
				Widgets.EndScrollView();
			}
		}
		Widgets.ListSeparator(ref divider, "BabyFoodConsumables".Translate().CapitalizeFirst());
		List<ThingDef> list2 = BabyConsumableFoods;
		RectDivider rectDivider2 = divider.CreateViewRect(list2.Count, Text.LineHeight);
		Widgets.BeginScrollView(divider, ref scrollPositionBabyConsumables, rectDivider2);
		foreach (ThingDef item2 in list2)
		{
			RectDivider rectDivider3 = rectDivider2.NewRow(Text.LineHeight);
			Widgets.DefIcon(rectDivider3.NewCol(Text.LineHeight), item2);
			Rect val = rectDivider3.NewCol(24f, HorizontalJustification.Right);
			Widgets.InfoCardButton(((Rect)(ref val)).x, ((Rect)(ref val)).y, item2);
			if (baby.foodRestriction != null)
			{
				Rect val2 = rectDivider3.NewCol(24f, HorizontalJustification.Right);
				bool checkOn = baby.foodRestriction.BabyFoodAllowed(item2);
				Widgets.Checkbox(((Rect)(ref val2)).x + 12f, ((Rect)(ref val2)).y, ref checkOn, 24f, disabled: false, paintable: true);
				baby.foodRestriction.SetBabyFoodAllowed(item2, checkOn);
			}
			Widgets.Label(rectDivider3, item2.LabelCap);
			if (Mouse.IsOver(rectDivider3))
			{
				Widgets.DrawHighlight(rectDivider3);
				TooltipHandler.TipRegion(rectDivider3, $"{item2.LabelCap}\n\n{item2.description}");
			}
		}
		Widgets.EndScrollView();
	}

	protected override void FillTab()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		using (TextBlock.Default())
		{
			FillTab(SelPawn, new Rect(Vector2.zero, size), ref scrollPositionAutoBreastfeed, ref scrollPositionBabyConsumables);
		}
	}

	private static FloatMenuOption GenerateFloatMenuOption(AutofeedMode setting, BabyFeederPair pair)
	{
		return new FloatMenuOption(setting.Translate().CapitalizeFirst(), delegate
		{
			pair.baby.mindState.SetAutofeeder(pair.feeder, setting);
		})
		{
			tooltip = setting.GetTooltip(feeder: pair.feeder, baby: pair.baby)
		};
	}

	private static IEnumerable<Widgets.DropdownMenuElement<AutofeedMode>> MenuGenerator(BabyFeederPair pair)
	{
		yield return new Widgets.DropdownMenuElement<AutofeedMode>
		{
			option = GenerateFloatMenuOption(AutofeedMode.Never, pair),
			payload = AutofeedMode.Never
		};
		yield return new Widgets.DropdownMenuElement<AutofeedMode>
		{
			option = GenerateFloatMenuOption(AutofeedMode.Childcare, pair),
			payload = AutofeedMode.Childcare
		};
		yield return new Widgets.DropdownMenuElement<AutofeedMode>
		{
			option = GenerateFloatMenuOption(AutofeedMode.Urgent, pair),
			payload = AutofeedMode.Urgent
		};
	}

	private static void DrawRow(Pawn baby, ref RectDivider row, Pawn feeder, Pawn mother, Pawn father, Pawn surrogate)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		AutofeedMode mode = baby.mindState.AutofeedSetting(feeder);
		string text = feeder.LabelShortCap;
		Hediff firstHediffOfDef = feeder.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Lactating);
		if (firstHediffOfDef != null)
		{
			text = text + " (" + firstHediffOfDef.LabelBaseCap + ")";
		}
		Widgets.Label(row.NewCol(220f), text);
		Rect rect = row.NewCol(120f);
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, mode.GetTooltip(baby, feeder));
		}
		BabyFeederPair babyFeederPair = default(BabyFeederPair);
		babyFeederPair.baby = baby;
		babyFeederPair.feeder = feeder;
		BabyFeederPair target = babyFeederPair;
		Widgets.Dropdown(rect, target, (BabyFeederPair _target) => _target.baby.mindState.AutofeedSetting(_target.feeder), MenuGenerator, mode.Translate().CapitalizeFirst(), null, null, null, null, paintable: true);
		if (feeder == mother)
		{
			Widgets.Label(row, PawnRelationDefOf.Parent.labelFemale.CapitalizeFirst());
		}
		else if (feeder == father)
		{
			Widgets.Label(row, PawnRelationDefOf.Parent.label.CapitalizeFirst());
		}
		else if (feeder == surrogate)
		{
			Widgets.Label(row, PawnRelationDefOf.ParentBirth.GetGenderSpecificLabelCap(feeder));
		}
	}
}
