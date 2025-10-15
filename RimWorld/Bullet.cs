using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Bullet : Projectile
{
	public override bool AnimalsFleeImpact => true;

	protected override void Impact(Thing hitThing, bool blockedByShield = false)
	{
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		Map map = base.Map;
		IntVec3 position = base.Position;
		base.Impact(hitThing, blockedByShield);
		BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
		Find.BattleLog.Add(battleLogEntry_RangedImpact);
		NotifyImpact(hitThing, map, position);
		if (hitThing != null)
		{
			bool instigatorGuilty = !(launcher is Pawn pawn) || !pawn.Drafted;
			DamageDef damageDef = base.DamageDef;
			float amount = DamageAmount;
			float armorPenetration = ArmorPenetration;
			Quaternion exactRotation = ExactRotation;
			DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, ((Quaternion)(ref exactRotation)).eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
			dinfo.SetWeaponQuality(equipmentQuality);
			hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
			(hitThing as Pawn)?.stances?.stagger.Notify_BulletImpact(this);
			if (base.ExtraDamages == null)
			{
				return;
			}
			{
				foreach (ExtraDamage extraDamage in base.ExtraDamages)
				{
					if (Rand.Chance(extraDamage.chance))
					{
						DamageDef damageDef2 = extraDamage.def;
						float amount2 = extraDamage.amount;
						float armorPenetration2 = extraDamage.AdjustedArmorPenetration();
						exactRotation = ExactRotation;
						DamageInfo dinfo2 = new DamageInfo(damageDef2, amount2, armorPenetration2, ((Quaternion)(ref exactRotation)).eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
						hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
					}
				}
				return;
			}
		}
		if (!blockedByShield)
		{
			SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map));
			if (base.Position.GetTerrain(map).takeSplashes)
			{
				FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt((float)DamageAmount) * 1f, 4f);
			}
			else
			{
				FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
			}
		}
		if (Rand.Chance(base.DamageDef.igniteCellChance))
		{
			FireUtility.TryStartFireIn(base.Position, map, Rand.Range(0.55f, 0.85f), launcher);
		}
	}

	private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
	{
		BulletImpactData bulletImpactData = default(BulletImpactData);
		bulletImpactData.bullet = this;
		bulletImpactData.hitThing = hitThing;
		bulletImpactData.impactPosition = position;
		BulletImpactData impactData = bulletImpactData;
		hitThing?.Notify_BulletImpactNearby(impactData);
		int num = 9;
		for (int i = 0; i < num; i++)
		{
			IntVec3 c = position + GenRadial.RadialPattern[i];
			if (!c.InBounds(map))
			{
				continue;
			}
			List<Thing> thingList = c.GetThingList(map);
			for (int j = 0; j < thingList.Count; j++)
			{
				if (thingList[j] != hitThing)
				{
					thingList[j].Notify_BulletImpactNearby(impactData);
				}
			}
		}
	}
}
