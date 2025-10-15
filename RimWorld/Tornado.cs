using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Tornado : ThingWithComps
{
	private Vector2 realPosition;

	private float direction;

	private int spawnTick;

	private int leftFadeOutTicks = -1;

	private int ticksLeftToDisappear = -1;

	private Sustainer sustainer;

	private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

	private static ModuleBase directionNoise;

	private const float Wind = 5f;

	private const int CloseDamageIntervalTicks = 15;

	private const int RoofDestructionIntervalTicks = 20;

	private const float FarDamageMTBTicks = 15f;

	private const float CloseDamageRadius = 4.2f;

	private const float FarDamageRadius = 10f;

	private const float BaseDamage = 30f;

	private const int SpawnMoteEveryTicks = 4;

	private static readonly IntRange DurationTicks = new IntRange(2700, 10080);

	private const float DownedPawnDamageFactor = 0.2f;

	private const float AnimalPawnDamageFactor = 0.75f;

	private const float BuildingDamageFactor = 0.8f;

	private const float PlantDamageFactor = 1.7f;

	private const float ItemDamageFactor = 0.68f;

	private const float CellsPerSecond = 1.7f;

	private const float DirectionChangeSpeed = 0.78f;

	private const float DirectionNoiseFrequency = 0.002f;

	private const float TornadoAnimationSpeed = 25f;

	private const float ThreeDimensionalEffectStrength = 4f;

	private const int FadeInTicks = 120;

	private const int FadeOutTicks = 120;

	private const float MaxMidOffset = 2f;

	private static readonly Material TornadoMaterial = MaterialPool.MatFrom("Things/Ethereal/Tornado", ShaderDatabase.Transparent, MapMaterialRenderQueues.Tornado);

	private static readonly FloatRange PartsDistanceFromCenter = new FloatRange(1f, 10f);

	private static readonly float ZOffsetBias = -4f * PartsDistanceFromCenter.min;

	private List<IntVec3> removedRoofsTmp = new List<IntVec3>();

	private static List<Thing> tmpThings = new List<Thing>();

	private float FadeInOutFactor
	{
		get
		{
			float num = Mathf.Clamp01((float)(Find.TickManager.TicksGame - spawnTick) / 120f);
			float num2 = ((leftFadeOutTicks < 0) ? 1f : Mathf.Min((float)leftFadeOutTicks / 120f, 1f));
			return Mathf.Min(num, num2);
		}
	}

	public override Vector2 DrawSize => new Vector2(45f, 100f);

	public override void ExposeData()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		Scribe_Values.Look(ref realPosition, "realPosition");
		Scribe_Values.Look(ref direction, "direction", 0f);
		Scribe_Values.Look(ref spawnTick, "spawnTick", 0);
		Scribe_Values.Look(ref leftFadeOutTicks, "leftFadeOutTicks", 0);
		Scribe_Values.Look(ref ticksLeftToDisappear, "ticksLeftToDisappear", 0);
	}

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.SpawnSetup(map, respawningAfterLoad);
		if (!respawningAfterLoad)
		{
			Vector3 val = base.Position.ToVector3Shifted();
			realPosition = new Vector2(val.x, val.z);
			direction = Rand.Range(0f, 360f);
			spawnTick = Find.TickManager.TicksGame;
			leftFadeOutTicks = -1;
			ticksLeftToDisappear = DurationTicks.RandomInRange;
		}
		CreateSustainer();
	}

	protected override void Tick()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		if (!base.Spawned)
		{
			return;
		}
		if (sustainer == null)
		{
			Log.Error("Tornado sustainer is null.");
			CreateSustainer();
		}
		sustainer?.Maintain();
		UpdateSustainerVolume();
		GetComp<CompWindSource>().wind = 5f * FadeInOutFactor;
		if (leftFadeOutTicks > 0)
		{
			leftFadeOutTicks--;
			if (leftFadeOutTicks == 0)
			{
				Destroy();
			}
			return;
		}
		if (directionNoise == null)
		{
			directionNoise = new Perlin(0.0020000000949949026, 2.0, 0.5, 4, 1948573612, QualityMode.Medium);
		}
		direction += (float)directionNoise.GetValue(Find.TickManager.TicksAbs, (float)(thingIDNumber % 500) * 1000f, 0.0) * 0.78f;
		realPosition = realPosition.Moved(direction, 0.028333334f);
		IntVec3 intVec = IntVec3Utility.ToIntVec3(new Vector3(realPosition.x, 0f, realPosition.y));
		if (intVec.InBounds(base.Map))
		{
			base.Position = intVec;
			if (this.IsHashIntervalTick(15))
			{
				DamageCloseThings();
			}
			if (Rand.MTBEventOccurs(15f, 1f, 1f))
			{
				DamageFarThings();
			}
			if (this.IsHashIntervalTick(20))
			{
				DestroyRoofs();
			}
			if (ticksLeftToDisappear > 0)
			{
				ticksLeftToDisappear--;
				if (ticksLeftToDisappear == 0)
				{
					leftFadeOutTicks = 120;
					Messages.Message("MessageTornadoDissipated".Translate(), new TargetInfo(base.Position, base.Map), MessageTypeDefOf.PositiveEvent);
				}
			}
			if (this.IsHashIntervalTick(4) && !CellImmuneToDamage(base.Position))
			{
				float num = Rand.Range(0.6f, 1f);
				Vector3 val = default(Vector3);
				((Vector3)(ref val))._002Ector(realPosition.x, 0f, realPosition.y);
				val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
				FleckMaker.ThrowTornadoDustPuff(val + Vector3Utility.RandomHorizontalOffset(1.5f), base.Map, Rand.Range(1.5f, 3f), new Color(num, num, num));
			}
		}
		else
		{
			leftFadeOutTicks = 120;
			Messages.Message("MessageTornadoLeftMap".Translate(), new TargetInfo(base.Position, base.Map), MessageTypeDefOf.PositiveEvent);
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		Rand.PushState();
		Rand.Seed = thingIDNumber;
		for (int i = 0; i < 180; i++)
		{
			DrawTornadoPart(PartsDistanceFromCenter.RandomInRange, Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f), Rand.Range(0.52f, 0.88f));
		}
		Rand.PopState();
	}

	private void DrawTornadoPart(float distanceFromCenter, float initialAngle, float speedMultiplier, float colorMultiplier)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		int ticksGame = Find.TickManager.TicksGame;
		float num = 1f / distanceFromCenter;
		float num2 = 25f * speedMultiplier * num;
		float num3 = (initialAngle + (float)ticksGame * num2) % 360f;
		Vector2 val = realPosition.Moved(num3, AdjustedDistanceFromCenter(distanceFromCenter));
		val.y += distanceFromCenter * 4f;
		val.y += ZOffsetBias;
		Vector3 val2 = new Vector3(val.x, AltitudeLayer.Weather.AltitudeFor() + 0.03658537f * Rand.Range(0f, 1f), val.y);
		float num4 = distanceFromCenter * 3f;
		float num5 = 1f;
		if (num3 > 270f)
		{
			num5 = GenMath.LerpDouble(270f, 360f, 0f, 1f, num3);
		}
		else if (num3 > 180f)
		{
			num5 = GenMath.LerpDouble(180f, 270f, 1f, 0f, num3);
		}
		float num6 = Mathf.Min(distanceFromCenter / (PartsDistanceFromCenter.max + 2f), 1f);
		float num7 = Mathf.InverseLerp(0.18f, 0.4f, num6);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(Mathf.Sin((float)ticksGame / 1000f + (float)(thingIDNumber * 10)) * 2f, 0f, 0f);
		Vector3 val4 = val2 + val3 * num7;
		float num8 = Mathf.Max(1f - num6, 0f) * num5 * FadeInOutFactor;
		Color val5 = default(Color);
		((Color)(ref val5))._002Ector(colorMultiplier, colorMultiplier, colorMultiplier, num8);
		matPropertyBlock.SetColor(ShaderPropertyIDs.Color, val5);
		Matrix4x4 val6 = Matrix4x4.TRS(val4, Quaternion.Euler(0f, num3, 0f), new Vector3(num4, 1f, num4));
		Graphics.DrawMesh(MeshPool.plane10, val6, TornadoMaterial, 0, (Camera)null, 0, matPropertyBlock);
	}

	private float AdjustedDistanceFromCenter(float distanceFromCenter)
	{
		float num = Mathf.Min(distanceFromCenter / 8f, 1f);
		num *= num;
		return distanceFromCenter * num;
	}

	private void UpdateSustainerVolume()
	{
		sustainer.info.volumeFactor = FadeInOutFactor;
	}

	private void CreateSustainer()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			SoundDef tornado = SoundDefOf.Tornado;
			sustainer = tornado.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
			UpdateSustainerVolume();
		});
	}

	private void DamageCloseThings()
	{
		int num = GenRadial.NumCellsInRadius(4.2f);
		for (int i = 0; i < num; i++)
		{
			IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
			if (intVec.InBounds(base.Map) && !CellImmuneToDamage(intVec))
			{
				Pawn firstPawn = intVec.GetFirstPawn(base.Map);
				if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
				{
					float damageFactor = GenMath.LerpDouble(0f, 4.2f, 1f, 0.2f, intVec.DistanceTo(base.Position));
					DoDamage(intVec, damageFactor);
				}
			}
		}
	}

	private void DamageFarThings()
	{
		IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, 10f, useCenter: true)
			where x.InBounds(base.Map)
			select x).RandomElement();
		if (!CellImmuneToDamage(c))
		{
			DoDamage(c, 0.5f);
		}
	}

	private void DestroyRoofs()
	{
		removedRoofsTmp.Clear();
		foreach (IntVec3 item in from x in GenRadial.RadialCellsAround(base.Position, 4.2f, useCenter: true)
			where x.InBounds(base.Map)
			select x)
		{
			if (!CellImmuneToDamage(item) && item.Roofed(base.Map))
			{
				RoofDef roof = item.GetRoof(base.Map);
				if (!roof.isThickRoof && !roof.isNatural)
				{
					RoofCollapserImmediate.DropRoofInCells(item, base.Map);
					removedRoofsTmp.Add(item);
				}
			}
		}
		if (removedRoofsTmp.Count > 0)
		{
			RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(removedRoofsTmp, base.Map, removalMode: true);
		}
	}

	private bool CellImmuneToDamage(IntVec3 c)
	{
		if (c.Roofed(base.Map) && c.GetRoof(base.Map).isThickRoof)
		{
			return true;
		}
		Building edifice = c.GetEdifice(base.Map);
		if (edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || (edifice.def == ThingDefOf.Wall && edifice.Faction == null)))
		{
			return true;
		}
		return false;
	}

	private void DoDamage(IntVec3 c, float damageFactor)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		tmpThings.Clear();
		tmpThings.AddRange(c.GetThingList(base.Map));
		Vector3 val = c.ToVector3Shifted();
		Vector2 b = default(Vector2);
		((Vector2)(ref b))._002Ector(val.x, val.z);
		float angle = 0f - realPosition.AngleTo(b) + 180f;
		for (int i = 0; i < tmpThings.Count; i++)
		{
			BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
			switch (tmpThings[i].def.category)
			{
			case ThingCategory.Pawn:
			{
				Pawn pawn = (Pawn)tmpThings[i];
				battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_Tornado);
				Find.BattleLog.Add(battleLogEntry_DamageTaken);
				if (pawn.RaceProps.baseHealthScale < 1f)
				{
					damageFactor *= pawn.RaceProps.baseHealthScale;
				}
				if (pawn.RaceProps.Animal)
				{
					damageFactor *= 0.75f;
				}
				if (pawn.Downed)
				{
					damageFactor *= 0.2f;
				}
				break;
			}
			case ThingCategory.Building:
				damageFactor *= 0.8f;
				break;
			case ThingCategory.Item:
				damageFactor *= 0.68f;
				break;
			case ThingCategory.Plant:
				damageFactor *= 1.7f;
				break;
			}
			int num = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
			tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.TornadoScratch, num, 0f, angle, this)).AssociateWithLog(battleLogEntry_DamageTaken);
		}
		tmpThings.Clear();
	}
}
