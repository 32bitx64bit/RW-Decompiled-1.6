using UnityEngine;
using Verse;

namespace RimWorld;

public class CompRitualEffect_IntervalSpawnDividedCircleEffecter : CompRitualEffect_IntervalSpawnBurst
{
	protected new CompProperties_RitualEffectIntervalSpawnDividedCircle Props => (CompProperties_RitualEffectIntervalSpawnDividedCircle)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public override void OnSetup(RitualVisualEffect parent, LordJob_Ritual ritual, bool loading)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		base.parent = parent;
		float num = 360f / (float)Props.numCopies;
		for (int i = 0; i < Props.numCopies; i++)
		{
			Vector3 val = Quaternion.AngleAxis(num * (float)i, Vector3.up) * Vector3.forward;
			IntVec3 cell = parent.ritual.selectedTarget.Cell;
			TargetInfo targetInfo = new TargetInfo(cell, parent.ritual.Map);
			Vector3 val2 = (val * Props.radius + Props.offset) * ScaleForRoom(ritual);
			Room room = (cell + val2.ToIntVec3() + Props.roomCheckOffset).GetRoom(ritual.Map);
			if (!props.onlySpawnInSameRoom || room == ritual.GetRoom)
			{
				Effecter effecter = Props.effecterDef.Spawn(cell, parent.ritual.Map, val2);
				effecter.Trigger(targetInfo, TargetInfo.Invalid);
				parent.AddEffecterToMaintain(targetInfo, effecter);
			}
		}
	}
}
