using Verse;

namespace RimWorld;

public class CompMoteEmitterRandomColor : CompMoteEmitter
{
	public CompProperties_MoteEmitterRandomColor Props => (CompProperties_MoteEmitterRandomColor)props;

	public override void Emit()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.Emit();
		mote.instanceColor = Props.colors.RandomElement();
	}
}
