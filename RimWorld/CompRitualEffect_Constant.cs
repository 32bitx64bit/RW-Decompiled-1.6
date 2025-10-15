using Verse;

namespace RimWorld;

public abstract class CompRitualEffect_Constant : RitualVisualEffectComp
{
	protected bool spawned;

	public override void OnSetup(RitualVisualEffect parent, LordJob_Ritual ritual, bool loading)
	{
		base.OnSetup(parent, ritual, loading);
		Spawn(ritual);
	}

	protected virtual void Spawn(LordJob_Ritual ritual)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Mote mote = SpawnMote(ritual);
		if (mote != null)
		{
			parent.AddMoteToMaintain(mote);
			if (props.colorOverride.HasValue)
			{
				mote.instanceColor = props.colorOverride.Value;
			}
			else
			{
				mote.instanceColor = parent.def.tintColor;
			}
			spawned = true;
		}
	}

	public override void TickInterval(int delta)
	{
		base.TickInterval(delta);
		if (!spawned)
		{
			Spawn(parent.ritual);
		}
	}
}
