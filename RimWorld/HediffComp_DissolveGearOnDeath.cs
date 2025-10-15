using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class HediffComp_DissolveGearOnDeath : HediffComp
{
	public HediffCompProperties_DissolveGearOnDeath Props => (HediffCompProperties_DissolveGearOnDeath)props;

	public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
	{
		base.Notify_PawnDied(dinfo, culprit);
		if (Props.injuryCreatedOnDeath == null)
		{
			return;
		}
		List<BodyPartRecord> list = new List<BodyPartRecord>(from part in base.Pawn.health.hediffSet.GetNotMissingParts()
			where part.coverageAbs > 0f
			select part);
		int num = Mathf.Min(Props.injuryCount.RandomInRange, list.Count);
		for (int i = 0; i < num; i++)
		{
			int index = Rand.Range(0, list.Count);
			BodyPartRecord bodyPartRecord = list[index];
			list.RemoveAt(index);
			if (base.Pawn.health.hediffSet.GetNotMissingParts().Contains(bodyPartRecord))
			{
				base.Pawn.health.AddHediff(Props.injuryCreatedOnDeath, bodyPartRecord);
			}
		}
	}

	public override void Notify_PawnKilled()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		base.Pawn.equipment.DestroyAllEquipment();
		base.Pawn.apparel.DestroyAll();
		if (!base.Pawn.Spawned)
		{
			return;
		}
		if (Props.mote != null || Props.fleck != null)
		{
			Vector3 drawPos = base.Pawn.DrawPos;
			Vector3 loc = default(Vector3);
			for (int i = 0; i < Props.moteCount; i++)
			{
				Vector2 val = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * (float)Rand.Sign;
				((Vector3)(ref loc))._002Ector(drawPos.x + val.x, drawPos.y, drawPos.z + val.y);
				if (Props.mote != null)
				{
					MoteMaker.MakeStaticMote(loc, base.Pawn.Map, Props.mote);
				}
				else
				{
					FleckMaker.Static(loc, base.Pawn.Map, Props.fleck);
				}
			}
		}
		if (Props.filth != null)
		{
			FilthMaker.TryMakeFilth(base.Pawn.Position, base.Pawn.Map, Props.filth, Props.filthCount);
		}
		if (Props.sound != null)
		{
			Props.sound.PlayOneShot(SoundInfo.InMap(base.Pawn));
		}
	}
}
