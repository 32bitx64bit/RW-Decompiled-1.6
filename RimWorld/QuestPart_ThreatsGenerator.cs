using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class QuestPart_ThreatsGenerator : QuestPartActivable, IIncidentMakerQuestPart
{
	public ThreatsGeneratorParams parms;

	public MapParent mapParent;

	public int threatStartTicks;

	public IEnumerable<FiringIncident> MakeIntervalIncidents()
	{
		if (mapParent != null && mapParent.HasMap)
		{
			return ThreatsGenerator.MakeIntervalIncidents(parms, mapParent.Map, base.EnableTick + threatStartTicks);
		}
		return Enumerable.Empty<FiringIncident>();
	}

	public override void DoDebugWindowContents(Rect innerRect, ref float curY)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (base.State == QuestPartState.Enabled)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, 500f, 25f);
			if (Widgets.ButtonText(rect, "Log future incidents from " + GetType().Name))
			{
				StorytellerUtility.DebugLogTestFutureIncidents(currentMapOnly: false, null, this, 50);
			}
			curY += ((Rect)(ref rect)).height + 4f;
		}
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Deep.Look(ref parms, "parms");
		Scribe_References.Look(ref mapParent, "mapParent");
		Scribe_Values.Look(ref threatStartTicks, "threatStartTicks", 0);
	}
}
