using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_DraftMech : PawnColumnWorker
{
	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!ModsConfig.BiotechActive || !pawn.IsColonyMechPlayerControlled || !pawn.Spawned)
		{
			return;
		}
		AcceptanceReport acceptanceReport = MechanitorUtility.CanDraftMech(pawn);
		if ((bool)acceptanceReport || !acceptanceReport.Reason.NullOrEmpty())
		{
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + (((Rect)(ref rect)).width - 24f) / 2f;
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + (((Rect)(ref rect)).height - 24f) / 2f;
			bool drafted = pawn.Drafted;
			Widgets.Checkbox(((Rect)(ref rect)).position, ref drafted, 24f, paintable: def.paintable, disabled: !acceptanceReport);
			if (!acceptanceReport.Reason.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, acceptanceReport.Reason.CapitalizeFirst());
			}
			if (drafted != pawn.Drafted)
			{
				pawn.drafter.Drafted = drafted;
			}
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), 44);
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
