using UnityEngine;
using Verse;

namespace RimWorld;

public class FireArrow : Projectile
{
	private static FloatRange FireSizeRange = new FloatRange(0.5f, 0.8f);

	private const int FuelToSpreadOnImpact = 4;

	private const int MaxCellsToSpread = 30;

	protected override void Impact(Thing hitThing, bool blockedByShield = false)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Position.GetTerrain(base.Map).IsWater)
		{
			int remainingFuel = 4;
			base.Map.floodFiller.FloodFill(base.Position, (IntVec3 c) => c.InBounds(base.Map) && !c.Filled(base.Map), delegate(IntVec3 c)
			{
				foreach (Thing thing in c.GetThingList(base.Map))
				{
					if (thing.def == ThingDefOf.Filth_Fuel)
					{
						return false;
					}
				}
				if (FilthMaker.TryMakeFilth(c, base.Map, ThingDefOf.Filth_Fuel))
				{
					remainingFuel--;
				}
				return remainingFuel <= 0;
			}, 30);
			FireUtility.TryStartFireIn(base.Position, base.Map, FireSizeRange.RandomInRange, launcher);
		}
		FleckCreationData dataStatic = FleckMaker.GetDataStatic(DrawPos, base.Map, FleckDefOf.MicroSparksFast);
		dataStatic.velocitySpeed = 0.8f;
		Quaternion exactRotation = ExactRotation;
		dataStatic.velocityAngle = ((Quaternion)(ref exactRotation)).eulerAngles.y;
		base.Map.flecks.CreateFleck(dataStatic);
		base.Impact(hitThing, blockedByShield);
	}
}
