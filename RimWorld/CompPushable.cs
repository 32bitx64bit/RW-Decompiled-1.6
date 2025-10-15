using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld;

public class CompPushable : ThingComp
{
	public bool canBePushed = true;

	public Vector3 drawPos;

	public Vector3 drawVel;

	public CompProperties_Pushable Props => (CompProperties_Pushable)props;

	public Pawn Pawn => (Pawn)parent;

	public override void Notify_Killed(Map prevMap, DamageInfo? dinfo = null)
	{
	}

	public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
	{
		if (!canBePushed || !Props.givePushOption)
		{
			yield break;
		}
		FloatMenuOption floatMenuOption = new FloatMenuOption("CommandPushTo".Translate(), delegate
		{
			Find.Targeter.BeginTargeting(TargetingParameters.ForCell(), delegate(LocalTargetInfo target)
			{
				Job job = JobMaker.MakeJob(JobDefOf.HaulToCell, Pawn, target.Cell);
				job.count = 1;
				selPawn.jobs.StartJob(job, JobCondition.InterruptForced);
			});
		});
		if (!selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
		{
			floatMenuOption.Label = string.Format("{0} ({1})", floatMenuOption.Label, "RequiredCapacity".Translate(PawnCapacityDefOf.Manipulation.label));
			floatMenuOption.Disabled = true;
		}
		yield return floatMenuOption;
	}

	public void OnStartedCarrying(Pawn pawn)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		Vector3 v = (pawn.Position - Pawn.Position).ToVector3();
		Vector2 val = Vector2Utility.RotatedBy(new Vector2(0f, 0f - Props.offsetDistance), v.AngleFlat());
		drawPos = new Vector3(val.x, 0f, 0f - val.y);
		drawVel = Vector3.zero;
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		_ = Prefs.DevMode;
		yield break;
	}

	public override void PostExposeData()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base.PostExposeData();
		Scribe_Values.Look(ref canBePushed, "canBePushed", defaultValue: false);
		Scribe_Values.Look(ref drawPos, "drawPos");
		Scribe_Values.Look(ref drawVel, "drawVel");
	}
}
