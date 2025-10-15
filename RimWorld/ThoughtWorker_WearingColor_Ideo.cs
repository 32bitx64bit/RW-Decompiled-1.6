using UnityEngine;
using Verse;

namespace RimWorld;

public class ThoughtWorker_WearingColor_Ideo : ThoughtWorker_WearingColor
{
	protected override Color? Color(Pawn p)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return p.Ideo?.ApparelColor;
	}
}
