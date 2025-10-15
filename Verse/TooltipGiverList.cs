using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace Verse;

public sealed class TooltipGiverList
{
	private List<Thing> givers = new List<Thing>();

	public void Notify_ThingSpawned(Thing t)
	{
		if (t.def.hasTooltip || ShouldShowShotReport(t))
		{
			givers.Add(t);
		}
	}

	public void Notify_ThingDespawned(Thing t)
	{
		if (t.def.hasTooltip || ShouldShowShotReport(t))
		{
			givers.Remove(t);
		}
	}

	public void DispenseAllThingTooltips()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 7 || Find.WindowStack.FloatMenu != null || (Find.Targeter.IsTargeting && Find.Targeter.targetingSource != null && Find.Targeter.targetingSource.HidePawnTooltips))
		{
			return;
		}
		CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
		float cellSizePixels = Find.CameraDriver.CellSizePixels;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(cellSizePixels, cellSizePixels);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, val.x, val.y);
		for (int i = 0; i < givers.Count; i++)
		{
			Thing thing = givers[i];
			if (!currentViewRect.Contains(thing.Position) || thing.Position.Fogged(thing.Map))
			{
				continue;
			}
			Vector2 val2 = thing.DrawPos.MapToUIPosition();
			((Rect)(ref rect)).x = val2.x - val.x / 2f;
			((Rect)(ref rect)).y = val2.y - val.y / 2f;
			if (!((Rect)(ref rect)).Contains(Event.current.mousePosition))
			{
				continue;
			}
			string text = (ShouldShowShotReport(thing) ? TooltipUtility.ShotCalculationTipString(thing) : null);
			if (thing.def.hasTooltip || !text.NullOrEmpty())
			{
				if (thing is Pawn pawn && pawn.IsHiddenFromPlayer())
				{
					break;
				}
				TipSignal tooltip = thing.GetTooltip();
				if (!text.NullOrEmpty())
				{
					ref string text2 = ref tooltip.text;
					text2 = text2 + "\n\n" + text;
				}
				TooltipHandler.TipRegion(rect, tooltip);
			}
		}
	}

	private bool ShouldShowShotReport(Thing t)
	{
		if (!t.def.hasTooltip && !(t is Hive))
		{
			return t is IAttackTarget;
		}
		return true;
	}
}
