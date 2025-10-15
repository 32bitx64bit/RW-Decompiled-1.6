using UnityEngine;

namespace Verse;

public abstract class SubEffecter_DrifterEmote : SubEffecter
{
	public SubEffecter_DrifterEmote(SubEffecterDef def, Effecter parent)
		: base(def, parent)
	{
	}

	protected void MakeMote(TargetInfo A, int overrideSpawnTick = -1)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((A.HasThing && A.Thing.DrawPosHeld.HasValue) ? A.Thing.DrawPosHeld.Value : A.Cell.ToVector3Shifted());
		if (!val.ShouldSpawnMotesAt(A.Map))
		{
			return;
		}
		int randomInRange = def.burstCount.RandomInRange;
		for (int i = 0; i < randomInRange; i++)
		{
			Mote mote = (Mote)ThingMaker.MakeThing(def.moteDef);
			mote.Scale = def.scale.RandomInRange;
			mote.exactPosition = val + base.EffectiveOffset + Gen.RandomHorizontalVector(def.positionRadius);
			mote.rotationRate = def.rotationRate.RandomInRange;
			mote.exactRotation = def.rotation.RandomInRange;
			if (overrideSpawnTick != -1)
			{
				mote.ForceSpawnTick(overrideSpawnTick);
			}
			if (mote is MoteThrown moteThrown)
			{
				moteThrown.airTimeLeft = def.airTime.RandomInRange;
				moteThrown.SetVelocity(def.angle.RandomInRange, def.speed.RandomInRange);
			}
			if (A.HasThing)
			{
				mote.Attach(A);
			}
			GenSpawn.Spawn(mote, val.ToIntVec3(), A.Map);
		}
	}
}
