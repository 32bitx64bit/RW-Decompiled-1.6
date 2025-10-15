using RimWorld;

namespace Verse;

public class HediffComp_DisappearsPausable_LethalInjuries : HediffComp_DisappearsPausable
{
	private const int PauseCheckInterval = 120;

	protected override bool Paused => SanguophageUtility.ShouldBeDeathrestingOrInComaInsteadOfDead(base.Pawn);

	public override string CompTipStringExtra
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (Paused)
			{
				return "PawnWillKeepRegeneratingLethalInjuries".Translate(base.Pawn.Named("PAWN")).Colorize(ColorLibrary.RedReadable);
			}
			return base.CompTipStringExtra;
		}
	}

	public override void CompPostTick(ref float severityAdjustment)
	{
		if (base.Pawn.IsHashIntervalTick(120) && !Paused)
		{
			ticksToDisappear -= 120;
		}
	}
}
