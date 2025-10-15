using System.Collections.Generic;
using RimWorld;

namespace Verse.AI;

public class JobDriver_ExitMapFlying : JobDriver
{
	private int waitTicks = -1;

	public override bool TryMakePreToilReservations(bool errorOnFailed)
	{
		return true;
	}

	protected override IEnumerable<Toil> MakeNewToils()
	{
		if (waitTicks < 0)
		{
			waitTicks = Rand.Range(0, 10);
		}
		yield return Toils_General.Wait(waitTicks);
		yield return Toils_General.Do(delegate
		{
			Map map = pawn.Map;
			IntVec3 position = pawn.Position;
			pawn.DeSpawn();
			Skyfaller skyfaller = SkyfallerMaker.MakeSkyfaller(ThingDefOf.FlyerLeaving, pawn);
			GenSpawn.Spawn(skyfaller, position, map);
			bool @bool = Rand.Bool;
			skyfaller.OverrideFlightFlippedHorizontal = @bool;
			pawn.Rotation = (@bool ? Rot4.West : Rot4.East);
		});
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref waitTicks, "waitTicks", -1);
	}
}
