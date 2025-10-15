using Verse;

namespace RimWorld;

public class CompRitualTargetMoteSpawner : CompRitualEffectSpawner
{
	private Mote mote;

	private CompProperties_RitualTargetMoteSpawner Props => (CompProperties_RitualTargetMoteSpawner)props;

	protected override void Tick_InRitual(LordJob_Ritual ritual)
	{
		mote?.Maintain();
	}

	protected override void Tick_InRitualInterval(LordJob_Ritual ritual)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (mote == null || mote.Destroyed)
		{
			mote = MoteMaker.MakeStaticMote(parent.Position.ToVector3Shifted(), parent.Map, Props.mote);
		}
	}

	protected override void Tick_OutOfRitualInterval()
	{
		if (mote != null && !mote.Destroyed)
		{
			mote?.Destroy();
		}
		mote = null;
	}
}
