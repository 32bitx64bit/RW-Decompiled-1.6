using UnityEngine;
using Verse;

namespace RimWorld;

public class CompRitualEffect_ConstantCircle : CompRitualEffect_Constant
{
	protected CompProperties_RitualEffectConstantCircle Props => (CompProperties_RitualEffectConstantCircle)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public override void OnSetup(RitualVisualEffect parent, LordJob_Ritual ritual, bool loading)
	{
		base.parent = parent;
		Spawn(ritual);
	}

	protected override void Spawn(LordJob_Ritual ritual)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		float num = 360f / (float)Props.numCopies;
		for (int i = 0; i < Props.numCopies; i++)
		{
			Vector3 val = Quaternion.AngleAxis(num * (float)i, Vector3.up) * Vector3.forward;
			Mote mote = SpawnMote(ritual, ritual.selectedTarget.Cell.ToVector3Shifted() + val * Props.radius);
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
			}
		}
		spawned = true;
	}
}
