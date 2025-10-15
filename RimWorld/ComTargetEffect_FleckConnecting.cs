using Verse;

namespace RimWorld;

public class ComTargetEffect_FleckConnecting : CompTargetEffect
{
	private CompProperties_TargetEffect_FleckConnecting Props => (CompProperties_TargetEffect_FleckConnecting)props;

	public override void DoEffectOn(Pawn user, Thing target)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (Props.fleckDef != null)
		{
			FleckMaker.ConnectingLine(user.DrawPos, target.DrawPos, Props.fleckDef, user.Map);
		}
	}
}
