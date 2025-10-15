using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld;

public class FloatMenuOptionProvider_DropEquipment : FloatMenuOptionProvider
{
	protected override bool Drafted => true;

	protected override bool Undrafted => true;

	protected override bool Multiselect => false;

	protected override bool CanSelfTarget => true;

	protected override bool AppliesInt(FloatMenuContext context)
	{
		return context.FirstSelectedPawn.equipment?.Primary != null;
	}

	protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (clickedPawn != context.FirstSelectedPawn)
		{
			return null;
		}
		if (clickedPawn.IsQuestLodger() && !EquipmentUtility.QuestLodgerCanUnequip(clickedPawn.equipment.Primary, clickedPawn))
		{
			return new FloatMenuOption("CannotDrop".Translate(clickedPawn.equipment.Primary.Label, clickedPawn.equipment.Primary) + ": " + "QuestRelated".Translate().CapitalizeFirst(), null);
		}
		Action action = delegate
		{
			clickedPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.DropEquipment, clickedPawn.equipment.Primary), JobTag.Misc);
		};
		return new FloatMenuOption((string)"Drop".Translate(clickedPawn.equipment.Primary.Label, clickedPawn.equipment.Primary), action, (Thing)clickedPawn.equipment.Primary, Color.white, MenuOptionPriority.Default, (Action<Rect>)null, (Thing)clickedPawn, 0f, (Func<Rect, bool>)null, (WorldObject)null, playSelectionSound: true, 0);
	}
}
