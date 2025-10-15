using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class LandingOutcomeWorker_ThrusterBreakdown : LandingOutcomeWorker
{
	public LandingOutcomeWorker_ThrusterBreakdown(LandingOutcomeDef def)
		: base(def)
	{
	}

	public override void ApplyOutcome(Gravship gravship)
	{
		List<ThingWithComps> list = (from comp in gravship.Engine.GravshipComponents
			where comp.parent.HasComp<CompGravshipThruster>()
			select comp.parent).ToList();
		list = list.InRandomOrder().Take(Rand.Range(Mathf.Min(2, list.Count), Mathf.Min(4, list.Count))).ToList();
		foreach (ThingWithComps item in list)
		{
			if (item.TryGetComp<CompBreakdownable>(out var comp2))
			{
				comp2.DoBreakdown();
			}
		}
		TaggedString taggedString = "ThrustersBrokeDown".Translate() + ":\n" + list.Select((ThingWithComps comp) => comp.LabelCap).ToLineList(" - ");
		SendStandardLetter(gravship.Engine, taggedString, new LookTargets(list));
	}
}
