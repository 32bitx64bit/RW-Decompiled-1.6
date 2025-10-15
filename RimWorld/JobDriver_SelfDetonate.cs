using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld;

public class JobDriver_SelfDetonate : JobDriver
{
	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		OverlayHandle? overlayBurningWick = null;
		Toil warmupToil = Toils_General.Wait(120);
		warmupToil.initAction = delegate
		{
			Pawn actor2 = warmupToil.actor;
			actor2.Map.overlayDrawer.Disable(actor2, ref overlayBurningWick);
			overlayBurningWick = actor2.Map.overlayDrawer.Enable(actor2, OverlayTypes.BurningWick);
		};
		warmupToil.AddFinishAction(delegate
		{
			Pawn actor = warmupToil.actor;
			actor.Map.overlayDrawer.Disable(actor, ref overlayBurningWick);
		});
		warmupToil.PlaySustainerOrSound(SoundDefOf.HissSmall);
		yield return warmupToil;
		yield return Toils_General.Do(delegate
		{
			warmupToil.actor.Kill(null, null);
		});
	}
}
