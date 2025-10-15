using Verse;

namespace RimWorld;

public class ComTargetEffect_MoteConnecting : CompTargetEffect
{
	private CompProperties_TargetEffect_MoteConnecting Props => (CompProperties_TargetEffect_MoteConnecting)props;

	public override void DoEffectOn(Pawn user, Thing target)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (Props.moteDef != null)
		{
			MoteMaker.MakeConnectingLine(user.DrawPos, target.DrawPos, Props.moteDef, user.Map);
		}
	}
}
