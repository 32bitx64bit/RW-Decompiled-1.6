using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

[StaticConstructorOnStartup]
public abstract class Projectile : ThingWithComps
{
	private static readonly Material shadowMaterial = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent);

	protected Vector3 origin;

	protected Vector3 destination;

	public LocalTargetInfo usedTarget;

	public LocalTargetInfo intendedTarget;

	protected Thing equipment;

	protected ThingDef equipmentDef;

	protected Thing launcher;

	protected ThingDef targetCoverDef;

	private ProjectileHitFlags desiredHitFlags = ProjectileHitFlags.All;

	protected bool preventFriendlyFire;

	protected int lifetime;

	protected QualityCategory equipmentQuality = QualityCategory.Normal;

	public float stoppingPower;

	public DamageDef damageDefOverride;

	public List<ExtraDamage> extraDamages = new List<ExtraDamage>();

	protected bool landed;

	protected int ticksToImpact;

	private Sustainer ambientSustainer;

	private static List<IntVec3> checkedCells = new List<IntVec3>();

	public DamageDef DamageDef => damageDefOverride ?? def.projectile.damageDef;

	public IEnumerable<ExtraDamage> ExtraDamages
	{
		get
		{
			List<ExtraDamage> first = extraDamages;
			IEnumerable<ExtraDamage> enumerable = def.projectile.extraDamages;
			return first.Concat(enumerable ?? Enumerable.Empty<ExtraDamage>());
		}
	}

	public ProjectileHitFlags HitFlags
	{
		get
		{
			if (def.projectile.alwaysFreeIntercept)
			{
				return ProjectileHitFlags.All;
			}
			if (def.projectile.flyOverhead)
			{
				return ProjectileHitFlags.None;
			}
			return desiredHitFlags;
		}
		set
		{
			desiredHitFlags = value;
		}
	}

	protected float StartingTicksToImpact
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = origin - destination;
			float num = ((Vector3)(ref val)).magnitude / def.projectile.SpeedTilesPerTick;
			if (num <= 0f)
			{
				num = 0.001f;
			}
			return num;
		}
	}

	protected IntVec3 DestinationCell => new IntVec3(destination);

	public virtual Vector3 ExactPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = (destination - origin).Yto0() * DistanceCoveredFraction;
			return origin.Yto0() + val + Vector3.up * def.Altitude;
		}
	}

	protected float DistanceCoveredFraction => Mathf.Clamp01(1f - (float)ticksToImpact / StartingTicksToImpact);

	protected float DistanceCoveredFractionArc => Mathf.Clamp01(1f - (float)(landed ? lifetime : ticksToImpact) / StartingTicksToImpact);

	public virtual Quaternion ExactRotation => Quaternion.LookRotation((destination - origin).Yto0());

	public virtual bool AnimalsFleeImpact => false;

	public override Vector3 DrawPos => ExactPosition;

	public virtual Material DrawMat => def.graphic.MatSingleFor(this);

	public virtual int DamageAmount => def.projectile.GetDamageAmount(equipment);

	public virtual float ArmorPenetration => def.projectile.GetArmorPenetration(equipment);

	public ThingDef EquipmentDef => equipmentDef;

	public Thing Launcher => launcher;

	public override int UpdateRateTicks
	{
		get
		{
			if (base.Spawned && Find.CurrentMap == base.Map && Find.CameraDriver.InViewOf(this))
			{
				return 1;
			}
			return 15;
		}
	}

	private float ArcHeightFactor
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			float num = def.projectile.arcHeightFactor;
			float num2 = (destination - origin).MagnitudeHorizontalSquared();
			if (num * num > num2 * 0.2f * 0.2f)
			{
				num = Mathf.Sqrt(num2) * 0.2f;
			}
			return num;
		}
	}

	public override void ExposeData()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		Scribe_Values.Look(ref origin, "origin");
		Scribe_Values.Look(ref destination, "destination");
		Scribe_Values.Look(ref ticksToImpact, "ticksToImpact", 0);
		Scribe_TargetInfo.Look(ref usedTarget, "usedTarget");
		Scribe_TargetInfo.Look(ref intendedTarget, "intendedTarget");
		Scribe_References.Look(ref launcher, "launcher");
		Scribe_References.Look(ref equipment, "equipment");
		Scribe_Defs.Look(ref equipmentDef, "equipmentDef");
		Scribe_Defs.Look(ref targetCoverDef, "targetCoverDef");
		Scribe_Values.Look(ref desiredHitFlags, "desiredHitFlags", ProjectileHitFlags.All);
		Scribe_Values.Look(ref preventFriendlyFire, "preventFriendlyFire", defaultValue: false);
		Scribe_Values.Look(ref landed, "landed", defaultValue: false);
		Scribe_Values.Look(ref lifetime, "lifetime", 0);
		Scribe_Values.Look(ref equipmentQuality, "equipmentQuality", QualityCategory.Normal);
	}

	public void Launch(Thing launcher, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Launch(launcher, base.Position.ToVector3Shifted(), usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment);
	}

	public virtual void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		this.launcher = launcher;
		this.origin = origin;
		this.usedTarget = usedTarget;
		this.intendedTarget = intendedTarget;
		this.targetCoverDef = targetCoverDef;
		this.preventFriendlyFire = preventFriendlyFire;
		HitFlags = hitFlags;
		stoppingPower = def.projectile.stoppingPower;
		if (stoppingPower == 0f && def.projectile.damageDef != null)
		{
			stoppingPower = def.projectile.damageDef.defaultStoppingPower;
		}
		if (equipment != null)
		{
			this.equipment = equipment;
			equipmentDef = equipment.def;
			equipment.TryGetQuality(out equipmentQuality);
			if (equipment.TryGetComp(out CompUniqueWeapon comp))
			{
				foreach (WeaponTraitDef item in comp.TraitsListForReading)
				{
					if (!Mathf.Approximately(item.additionalStoppingPower, 0f))
					{
						stoppingPower += item.additionalStoppingPower;
					}
				}
			}
		}
		else
		{
			equipmentDef = null;
		}
		destination = usedTarget.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.3f);
		ticksToImpact = Mathf.CeilToInt(StartingTicksToImpact);
		if (ticksToImpact < 1)
		{
			ticksToImpact = 1;
		}
		lifetime = ticksToImpact;
		if (!def.projectile.soundAmbient.NullOrUndefined())
		{
			ambientSustainer = def.projectile.soundAmbient.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
		}
	}

	protected override void Tick()
	{
		base.Tick();
		if (ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && def.projectile.soundImpactAnticipate != null)
		{
			def.projectile.soundImpactAnticipate.PlayOneShot(this);
		}
		if (ambientSustainer != null)
		{
			ambientSustainer.Maintain();
		}
	}

	protected override void TickInterval(int delta)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.TickInterval(delta);
		lifetime -= delta;
		if (landed)
		{
			return;
		}
		Vector3 exactPosition = ExactPosition;
		ticksToImpact -= delta;
		if (!ExactPosition.InBounds(base.Map))
		{
			ticksToImpact += delta;
			base.Position = ExactPosition.ToIntVec3();
			Destroy();
			return;
		}
		Vector3 exactPosition2 = ExactPosition;
		if (CheckForFreeInterceptBetween(exactPosition, exactPosition2))
		{
			return;
		}
		base.Position = ExactPosition.ToIntVec3();
		if (ticksToImpact <= 0)
		{
			if (DestinationCell.InBounds(base.Map))
			{
				base.Position = DestinationCell;
			}
			ImpactSomething();
		}
	}

	private bool CheckForFreeInterceptBetween(Vector3 lastExactPos, Vector3 newExactPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		if (lastExactPos == newExactPos)
		{
			return false;
		}
		List<Thing> list = base.Map.listerThings.ThingsInGroup(ThingRequestGroup.ProjectileInterceptor);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].TryGetComp<CompProjectileInterceptor>().CheckIntercept(this, lastExactPos, newExactPos))
			{
				Impact(null, blockedByShield: true);
				return true;
			}
		}
		IntVec3 intVec = lastExactPos.ToIntVec3();
		IntVec3 intVec2 = newExactPos.ToIntVec3();
		if (intVec2 == intVec)
		{
			return false;
		}
		if (!intVec.InBounds(base.Map) || !intVec2.InBounds(base.Map))
		{
			return false;
		}
		if (intVec2.AdjacentToCardinal(intVec))
		{
			return CheckForFreeIntercept(intVec2);
		}
		if (VerbUtility.InterceptChanceFactorFromDistance(origin, intVec2) <= 0f)
		{
			return false;
		}
		Vector3 val = lastExactPos;
		Vector3 v = newExactPos - lastExactPos;
		Vector3 val2 = ((Vector3)(ref v)).normalized * 0.2f;
		int num = (int)(v.MagnitudeHorizontal() / 0.2f);
		checkedCells.Clear();
		int num2 = 0;
		IntVec3 intVec3;
		do
		{
			val += val2;
			intVec3 = val.ToIntVec3();
			if (!checkedCells.Contains(intVec3))
			{
				if (CheckForFreeIntercept(intVec3))
				{
					return true;
				}
				checkedCells.Add(intVec3);
			}
			num2++;
			if (num2 > num)
			{
				return false;
			}
		}
		while (!(intVec3 == intVec2));
		return false;
	}

	private bool CheckForFreeIntercept(IntVec3 c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (destination.ToIntVec3() == c)
		{
			return false;
		}
		float num = VerbUtility.InterceptChanceFactorFromDistance(origin, c);
		if (num <= 0f)
		{
			return false;
		}
		bool flag = false;
		List<Thing> thingList = c.GetThingList(base.Map);
		for (int i = 0; i < thingList.Count; i++)
		{
			Thing thing = thingList[i];
			if (!CanHit(thing))
			{
				continue;
			}
			bool flag2 = false;
			if (thing.def.Fillage == FillCategory.Full)
			{
				if (!(thing is Building_Door { Open: not false }))
				{
					ThrowDebugText("int-wall", c);
					Impact(thing);
					return true;
				}
				flag2 = true;
			}
			float num2 = 0f;
			if (thing is Pawn pawn)
			{
				num2 = 0.4f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
				if (pawn.GetPosture() != 0)
				{
					num2 *= 0.1f;
				}
				if (launcher != null && pawn.Faction != null && launcher.Faction != null && !pawn.Faction.HostileTo(launcher.Faction))
				{
					if (preventFriendlyFire)
					{
						num2 = 0f;
						ThrowDebugText("ff-miss", c);
					}
					else
					{
						num2 *= Find.Storyteller.difficulty.friendlyFireChanceFactor;
					}
				}
			}
			else if (thing.def.fillPercent > 0.2f)
			{
				num2 = (flag2 ? 0.05f : ((!DestinationCell.AdjacentTo8Way(c)) ? (thing.def.fillPercent * 0.15f) : (thing.def.fillPercent * 1f)));
			}
			num2 *= num;
			if (num2 > 1E-05f)
			{
				if (Rand.Chance(num2))
				{
					ThrowDebugText("int-" + num2.ToStringPercent(), c);
					Impact(thing);
					return true;
				}
				flag = true;
				ThrowDebugText(num2.ToStringPercent(), c);
			}
		}
		if (!flag)
		{
			ThrowDebugText("o", c);
		}
		return false;
	}

	private void ThrowDebugText(string text, IntVec3 c)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (DebugViewSettings.drawShooting)
		{
			MoteMaker.ThrowText(c.ToVector3Shifted(), base.Map, text);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFractionArc);
		Vector3 val = drawLoc + new Vector3(0f, 0f, 1f) * num;
		if (def.projectile.shadowSize > 0f)
		{
			DrawShadow(drawLoc, num);
		}
		Quaternion val2 = ExactRotation;
		if (def.projectile.spinRate != 0f)
		{
			float num2 = 60f / def.projectile.spinRate;
			val2 = Quaternion.AngleAxis((float)Find.TickManager.TicksGame % num2 / num2 * 360f, Vector3.up);
		}
		if (def.projectile.useGraphicClass)
		{
			Graphic.Draw(val, base.Rotation, this, ((Quaternion)(ref val2)).eulerAngles.y);
		}
		else
		{
			Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), val, val2, DrawMat, 0);
		}
		Comps_PostDraw();
	}

	protected bool CanHit(Thing thing)
	{
		if (!thing.Spawned)
		{
			return false;
		}
		if (thing == launcher)
		{
			return false;
		}
		ProjectileHitFlags hitFlags = HitFlags;
		if (hitFlags == ProjectileHitFlags.None)
		{
			return false;
		}
		if (thing.Map != base.Map)
		{
			return false;
		}
		if (CoverUtility.ThingCovered(thing, base.Map))
		{
			return false;
		}
		if (thing == intendedTarget && (hitFlags & ProjectileHitFlags.IntendedTarget) != 0)
		{
			return true;
		}
		if (thing != intendedTarget)
		{
			if (thing is Pawn)
			{
				if ((hitFlags & ProjectileHitFlags.NonTargetPawns) != 0)
				{
					return true;
				}
			}
			else if ((hitFlags & ProjectileHitFlags.NonTargetWorld) != 0)
			{
				return true;
			}
		}
		if (thing == intendedTarget && thing.def.Fillage == FillCategory.Full)
		{
			return true;
		}
		return false;
	}

	protected virtual void ImpactSomething()
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		if (def.projectile.flyOverhead)
		{
			RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
			if (roofDef != null)
			{
				if (roofDef.isThickRoof)
				{
					ThrowDebugText("hit-thick-roof", base.Position);
					if (!def.projectile.soundHitThickRoof.NullOrUndefined())
					{
						def.projectile.soundHitThickRoof.PlayOneShot(new TargetInfo(base.Position, base.Map));
					}
					Destroy();
					return;
				}
				if (base.Position.GetEdifice(base.Map) == null || base.Position.GetEdifice(base.Map).def.Fillage != FillCategory.Full)
				{
					RoofCollapserImmediate.DropRoofInCells(base.Position, base.Map);
				}
			}
		}
		if (usedTarget.HasThing && CanHit(usedTarget.Thing))
		{
			if (usedTarget.Thing is Pawn p && p.GetPosture() != 0 && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f && !Rand.Chance(0.5f))
			{
				ThrowDebugText("miss-laying", base.Position);
				Impact(null);
			}
			else
			{
				Impact(usedTarget.Thing);
			}
			return;
		}
		List<Thing> list = VerbUtility.ThingsToHit(base.Position, base.Map, CanHit);
		list.Shuffle();
		for (int i = 0; i < list.Count; i++)
		{
			Thing thing = list[i];
			float num;
			if (thing is Pawn pawn)
			{
				num = 0.5f * Mathf.Clamp(pawn.BodySize, 0.1f, 2f);
				if (pawn.GetPosture() != 0 && (origin - destination).MagnitudeHorizontalSquared() >= 20.25f)
				{
					num *= 0.5f;
				}
				if (launcher != null && pawn.Faction != null && launcher.Faction != null && !pawn.Faction.HostileTo(launcher.Faction))
				{
					num *= VerbUtility.InterceptChanceFactorFromDistance(origin, base.Position);
				}
			}
			else
			{
				num = 1.5f * thing.def.fillPercent;
			}
			if (Rand.Chance(num))
			{
				ThrowDebugText("hit-" + num.ToStringPercent(), base.Position);
				Impact(thing);
				return;
			}
			ThrowDebugText("miss-" + num.ToStringPercent(), base.Position);
		}
		Impact(null);
	}

	protected virtual void Impact(Thing hitThing, bool blockedByShield = false)
	{
		GenClamor.DoClamor(this, 12f, ClamorDefOf.Impact);
		if (!blockedByShield && def.projectile.landedEffecter != null)
		{
			def.projectile.landedEffecter.Spawn(base.Position, base.Map).Cleanup();
		}
		Destroy();
	}

	private void DrawShadow(Vector3 drawLoc, float height)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)shadowMaterial == (Object)null))
		{
			float num = def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(num, 1f, num);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(0f, -0.01f, 0f);
			Matrix4x4 val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(drawLoc + val2, Quaternion.identity, val);
			Graphics.DrawMesh(MeshPool.plane10, val3, shadowMaterial, 0);
		}
	}
}
