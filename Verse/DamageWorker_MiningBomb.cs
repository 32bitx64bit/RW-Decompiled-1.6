using RimWorld;
using UnityEngine;

namespace Verse;

public class DamageWorker_MiningBomb : DamageWorker_AddInjury
{
	public override DamageResult Apply(DamageInfo dinfo, Thing thing)
	{
		if (!ModLister.CheckOdyssey("Mining explosives"))
		{
			return base.Apply(dinfo, thing);
		}
		if (thing is Mineable mineable)
		{
			float amount = dinfo.Amount;
			amount *= dinfo.Def.buildingDamageFactor;
			amount = ((thing.def.passability != Traversability.Impassable) ? (amount * dinfo.Def.buildingDamageFactorPassable) : (amount * dinfo.Def.buildingDamageFactorImpassable));
			int amount2 = Mathf.Min(thing.HitPoints, GenMath.RoundRandom(amount));
			mineable.Notify_TookMiningDamage(amount2, null);
		}
		return base.Apply(dinfo, thing);
	}
}
