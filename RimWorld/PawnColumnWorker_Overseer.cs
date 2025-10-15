using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_Overseer : PawnColumnWorker_Label
{
	protected override TextAnchor LabelAlignment => (TextAnchor)4;

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Pawn overseer = pawn.GetOverseer();
		if (overseer != null)
		{
			base.DoCell(rect, overseer, table);
		}
	}

	public override bool CanGroupWith(Pawn pawn, Pawn other)
	{
		Pawn overseer = pawn.GetOverseer();
		if (overseer != null)
		{
			return other.GetOverseer() == overseer;
		}
		return false;
	}
}
