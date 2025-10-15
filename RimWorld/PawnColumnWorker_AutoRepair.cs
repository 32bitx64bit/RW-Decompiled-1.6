using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_AutoRepair : PawnColumnWorker
{
	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.Faction != Faction.OfPlayer || !pawn.RaceProps.IsMechanoid || pawn.GetOverseer() != null)
		{
			CompMechRepairable comp = pawn.GetComp<CompMechRepairable>();
			if (comp != null)
			{
				((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + (((Rect)(ref rect)).width - 24f) / 2f;
				((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + (((Rect)(ref rect)).height - 24f) / 2f;
				Widgets.Checkbox(((Rect)(ref rect)).position, ref comp.autoRepair, 24f, disabled: false, def.paintable);
			}
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), 24);
	}

	public override int GetMaxWidth(PawnTable table)
	{
		return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
	}

	public override int GetMinCellHeight(Pawn pawn)
	{
		return Mathf.Max(base.GetMinCellHeight(pawn), 24);
	}
}
