using UnityEngine;
using Verse;

namespace RimWorld;

public class ScenPart_Naked : ScenPart_PawnModifier
{
	public override string Summary(Scenario scen)
	{
		return "ScenPart_PawnsAreNaked".Translate(context.ToStringHuman(), chance.ToStringPercent()).CapitalizeFirst();
	}

	protected override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
	{
		if (pawn.apparel != null)
		{
			pawn.apparel.DestroyAll();
		}
	}

	public override void DoEditInterface(Listing_ScenEdit listing)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
		DoPawnModifierEditInterface(scenPartRect.BottomPartPixels(ScenPart.RowHeight * 2f));
	}
}
