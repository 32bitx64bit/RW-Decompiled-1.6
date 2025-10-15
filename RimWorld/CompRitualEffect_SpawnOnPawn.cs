using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class CompRitualEffect_SpawnOnPawn : CompRitualEffect_IntervalSpawn
{
	protected new CompProperties_RitualEffectSpawnOnPawn Props => (CompProperties_RitualEffectSpawnOnPawn)props;

	protected override Vector3 SpawnPos(LordJob_Ritual ritual)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	protected abstract Pawn GetPawn(LordJob_Ritual ritual);

	public override void SpawnFleck(LordJob_Ritual ritual, Vector3? forcedPos = null, float? exactRotation = null)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (Props.fleckDef == null)
		{
			return;
		}
		Pawn pawn = GetPawn(ritual);
		if (pawn != null && (Props.requiredTag.NullOrEmpty() || ritual.PawnTagSet(pawn, Props.requiredTag)))
		{
			Vector3 val = props.offset.RotatedBy(pawn.Rotation);
			if (pawn.Rotation == Rot4.East)
			{
				val += Props.eastRotationOffset;
			}
			else if (pawn.Rotation == Rot4.West)
			{
				val += Props.westRotationOffset;
			}
			else if (pawn.Rotation == Rot4.North)
			{
				val += Props.northRotationOffset;
			}
			else if (pawn.Rotation == Rot4.South)
			{
				val += Props.southRotationOffset;
			}
			base.SpawnFleck(parent.ritual, pawn.Position.ToVector3Shifted() + val, pawn.Rotation.AsAngle);
		}
		burstsDone++;
		lastSpawnTick = GenTicks.TicksGame;
	}

	public override Mote SpawnMote(LordJob_Ritual ritual, Vector3? forcedPos = null)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Mote result = null;
		if (Props.moteDef != null)
		{
			Pawn pawn = GetPawn(ritual);
			if (pawn != null && (Props.requiredTag.NullOrEmpty() || ritual.PawnTagSet(pawn, Props.requiredTag)))
			{
				Vector3 val = props.offset.RotatedBy(pawn.Rotation);
				result = base.SpawnMote(parent.ritual, pawn.Position.ToVector3Shifted() + val);
			}
			burstsDone++;
			lastSpawnTick = GenTicks.TicksGame;
		}
		return result;
	}

	public override Effecter SpawnEffecter(LordJob_Ritual ritual, TargetInfo target, Vector3? forcedPos = null)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Effecter result = null;
		if (Props.effecterDef != null)
		{
			Pawn pawn = GetPawn(ritual);
			if (pawn != null && (Props.requiredTag.NullOrEmpty() || ritual.PawnTagSet(pawn, Props.requiredTag)))
			{
				Vector3 value = props.offset.RotatedBy(pawn.Rotation);
				result = base.SpawnEffecter(parent.ritual, pawn, value);
			}
			burstsDone++;
			lastSpawnTick = GenTicks.TicksGame;
		}
		return result;
	}
}
