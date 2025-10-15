using UnityEngine;
using Verse;

namespace RimWorld;

public static class FleckMaker
{
	public static FleckCreationData GetDataStatic(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		FleckCreationData result = default(FleckCreationData);
		result.def = fleckDef;
		result.spawnPosition = loc;
		result.scale = scale;
		result.ageTicksOverride = -1;
		return result;
	}

	public static void Static(IntVec3 cell, Map map, FleckDef fleckDef, float scale = 1f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Static(cell.ToVector3Shifted(), map, fleckDef, scale);
	}

	public static void Static(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		map.flecks.CreateFleck(GetDataStatic(loc, map, fleckDef, scale));
	}

	public static FleckCreationData GetDataThrowMetaIcon(IntVec3 cell, Map map, FleckDef fleckDef, float velocitySpeed = 0.42f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		FleckCreationData result = default(FleckCreationData);
		result.def = fleckDef;
		result.spawnPosition = cell.ToVector3Shifted() + new Vector3(0.35f, 0f, 0.35f) + new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f;
		result.velocityAngle = Rand.Range(30, 60);
		result.velocitySpeed = velocitySpeed;
		result.rotationRate = Rand.Range(-3f, 3f);
		result.scale = 0.7f;
		result.ageTicksOverride = -1;
		return result;
	}

	public static void ThrowMetaIcon(IntVec3 cell, Map map, FleckDef fleckDef, float velocitySpeed = 0.42f)
	{
		if (cell.ShouldSpawnMotesAt(map))
		{
			map.flecks.CreateFleck(GetDataThrowMetaIcon(cell, map, fleckDef, velocitySpeed));
		}
	}

	public static FleckCreationData GetDataAttachedOverlay(Thing thing, FleckDef fleckDef, Vector3 offset, float scale = 1f, float solidTimeOverride = -1f)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		FleckCreationData result = default(FleckCreationData);
		result.def = fleckDef;
		result.spawnPosition = thing.DrawPos + offset;
		result.solidTimeOverride = solidTimeOverride;
		result.scale = scale;
		result.ageTicksOverride = -1;
		return result;
	}

	public static void AttachedOverlay(Thing thing, FleckDef fleckDef, Vector3 offset, float scale = 1f, float solidTimeOverride = -1f)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!thing.Destroyed)
		{
			thing.MapHeld.flecks.CreateFleck(GetDataAttachedOverlay(thing, fleckDef, offset, scale, solidTimeOverride));
		}
	}

	public static void ThrowShamblerParticles(Thing thing)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (thing.Spawned)
		{
			FleckCreationData dataAttachedOverlay = GetDataAttachedOverlay(thing, FleckDefOf.FleckShamblerDecay, Vector3.zero, Rand.Range(0.8f, 1.2f));
			ref Vector3 spawnPosition = ref dataAttachedOverlay.spawnPosition;
			spawnPosition += new Vector3(Rand.Range(-0.1f, 0.1f), 0f, Rand.Range(0f, 0.5f));
			dataAttachedOverlay.rotationRate = Rand.Range(-12f, 12f);
			dataAttachedOverlay.velocityAngle = Rand.Range(-35f, 35f);
			dataAttachedOverlay.velocitySpeed = Rand.Range(0.1f, 0.3f);
			dataAttachedOverlay.solidTimeOverride = Rand.Range(0.7f, 1f);
			thing.MapHeld.flecks.CreateFleck(dataAttachedOverlay);
		}
	}

	public static void ThrowMetaPuffs(CellRect rect, Map map)
	{
		if (Find.TickManager.Paused)
		{
			return;
		}
		for (int i = rect.minX; i <= rect.maxX; i++)
		{
			for (int j = rect.minZ; j <= rect.maxZ; j++)
			{
				ThrowMetaPuffs(new TargetInfo(new IntVec3(i, 0, j), map));
			}
		}
	}

	public static void ThrowMetaPuffs(TargetInfo targ)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (targ.HasThing ? targ.Thing.TrueCenter() : targ.Cell.ToVector3Shifted());
		int num = Rand.RangeInclusive(4, 6);
		for (int i = 0; i < num; i++)
		{
			ThrowMetaPuff(val + new Vector3(Rand.Range(-0.5f, 0.5f), 0f, Rand.Range(-0.5f, 0.5f)), targ.Map);
		}
	}

	public static void ThrowMetaPuff(Vector3 loc, Map map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.MetaPuff, 1.9f);
			dataStatic.rotationRate = Rand.Range(-60, 60);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = Rand.Range(0.6f, 0.78f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowAirPuffUp(Vector3 loc, Map map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ToIntVec3().ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc + new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f)), map, FleckDefOf.AirPuff, 1.5f);
			dataStatic.rotationRate = Rand.RangeInclusive(-240, 240);
			dataStatic.velocityAngle = Rand.Range(-45, 45);
			dataStatic.velocitySpeed = Rand.Range(1.2f, 1.5f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowBreathPuff(Vector3 loc, Map map, float throwAngle, Vector3 inheritVelocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ToIntVec3().ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc + new Vector3(Rand.Range(-0.005f, 0.005f), 0f, Rand.Range(-0.005f, 0.005f)), map, FleckDefOf.AirPuff, Rand.Range(0.6f, 0.7f));
			dataStatic.rotationRate = Rand.RangeInclusive(-240, 240);
			dataStatic.velocityAngle = throwAngle + (float)Rand.Range(-10, 10);
			dataStatic.velocitySpeed = Rand.Range(0.1f, 0.8f);
			dataStatic.velocity = inheritVelocity * 0.5f;
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowDustPuff(IntVec3 cell, Map map, float scale)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ThrowDustPuff(cell.ToVector3() + new Vector3(Rand.Value, 0f, Rand.Value), map, scale);
	}

	public static void ThrowDustPuff(Vector3 loc, Map map, float scale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.DustPuff, 1.9f * scale);
			dataStatic.rotationRate = Rand.Range(-60, 60);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = Rand.Range(0.6f, 0.75f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowDustPuffThick(Vector3 loc, Map map, float scale, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.DustPuffThick, scale);
			dataStatic.rotationRate = Rand.Range(-60, 60);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = Rand.Range(0.6f, 0.75f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowTornadoDustPuff(Vector3 loc, Map map, float scale, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.TornadoDustPuff, 1.9f * scale);
			dataStatic.rotationRate = Rand.Range(-60, 60);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = Rand.Range(0.6f, 0.75f);
			dataStatic.instanceColor = color;
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowSmoke(Vector3 loc, Map map, float size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.Smoke, Rand.Range(1.5f, 2.5f) * size);
			dataStatic.rotationRate = Rand.Range(-30f, 30f);
			dataStatic.velocityAngle = Rand.Range(30, 40);
			dataStatic.velocitySpeed = Rand.Range(0.5f, 0.7f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowFireGlow(Vector3 c, Map map, float size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = c;
		if (val.ShouldSpawnMotesAt(map))
		{
			val += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
			if (val.InBounds(map))
			{
				FleckCreationData dataStatic = GetDataStatic(val, map, FleckDefOf.FireGlow, Rand.Range(4f, 6f) * size);
				dataStatic.rotationRate = Rand.Range(-3f, 3f);
				dataStatic.velocityAngle = Rand.Range(0, 360);
				dataStatic.velocitySpeed = 0.12f;
				map.flecks.CreateFleck(dataStatic);
			}
		}
	}

	public static void ThrowHeatGlow(IntVec3 c, Map map, float size)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = c.ToVector3Shifted();
		if (val.ShouldSpawnMotesAt(map))
		{
			val += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
			if (val.InBounds(map))
			{
				FleckCreationData dataStatic = GetDataStatic(val, map, FleckDefOf.HeatGlow, Rand.Range(4f, 6f) * size);
				dataStatic.rotationRate = Rand.Range(-3f, 3f);
				dataStatic.velocityAngle = Rand.Range(0, 360);
				dataStatic.velocitySpeed = 0.12f;
				map.flecks.CreateFleck(dataStatic);
			}
		}
	}

	public static void ThrowMicroSparks(Vector3 loc, Map map)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			loc -= new Vector3(0.5f, 0f, 0.5f);
			loc += new Vector3(Rand.Value, 0f, Rand.Value);
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.MicroSparks, Rand.Range(0.8f, 1.2f));
			dataStatic.rotationRate = Rand.Range(-12f, 12f);
			dataStatic.velocityAngle = Rand.Range(35, 45);
			dataStatic.velocitySpeed = 1.2f;
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowLightningGlow(Vector3 loc, Map map, float size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f), map, FleckDefOf.LightningGlow, Rand.Range(4f, 6f) * size);
			dataStatic.rotationRate = Rand.Range(-3f, 3f);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = 1.2f;
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void PlaceFootprint(Vector3 loc, Map map, float rot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, FleckDefOf.Footprint, 0.5f);
			dataStatic.rotation = rot;
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowHorseshoe(Pawn thrower, IntVec3 targetCell)
	{
		ThrowObjectAt(thrower, targetCell, FleckDefOf.Horseshoe);
	}

	public static void ThrowStone(Pawn thrower, IntVec3 targetCell)
	{
		ThrowObjectAt(thrower, targetCell, FleckDefOf.Stone);
	}

	private static void ThrowObjectAt(Pawn thrower, IntVec3 targetCell, FleckDef fleck)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (thrower.Position.ShouldSpawnMotesAt(thrower.Map))
		{
			float num = Rand.Range(3.8f, 5.6f);
			Vector3 val = targetCell.ToVector3Shifted() + Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(SkillDefOf.Shooting).Level / 20f) * 1.8f);
			val.y = thrower.DrawPos.y;
			FleckCreationData dataStatic = GetDataStatic(thrower.DrawPos, thrower.Map, fleck);
			dataStatic.rotationRate = Rand.Range(-300, 300);
			dataStatic.velocityAngle = (val - dataStatic.spawnPosition).AngleFlat();
			dataStatic.velocitySpeed = num;
			dataStatic.airTimeLeft = Mathf.RoundToInt((dataStatic.spawnPosition - val).MagnitudeHorizontal() / num);
			thrower.Map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void ThrowExplosionCell(IntVec3 cell, Map map, FleckDef fleckDef, Color color)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (cell.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(cell.ToVector3Shifted(), map, fleckDef);
			dataStatic.rotation = 90 * Rand.RangeInclusive(0, 3);
			dataStatic.instanceColor = color;
			map.flecks.CreateFleck(dataStatic);
			if (Rand.Value < 0.7f)
			{
				ThrowDustPuff(cell, map, 1.2f);
			}
		}
	}

	public static void ThrowExplosionInterior(Vector3 loc, Map map, FleckDef fleckDef)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			FleckCreationData dataStatic = GetDataStatic(loc, map, fleckDef, Rand.Range(3f, 4.5f));
			dataStatic.rotationRate = Rand.Range(-30f, 30f);
			dataStatic.velocityAngle = Rand.Range(0, 360);
			dataStatic.velocitySpeed = Rand.Range(0.48f, 0.72f);
			map.flecks.CreateFleck(dataStatic);
		}
	}

	public static void WaterSplash(Vector3 loc, Map map, float size, float velocity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			map.flecks.CreateFleck(new FleckCreationData
			{
				def = FleckDefOf.WaterSplash,
				targetSize = size,
				velocitySpeed = velocity,
				spawnPosition = loc
			});
		}
	}

	public static void WaterRipple(Vector3 loc, Map map, float size)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (loc.ShouldSpawnMotesAt(map))
		{
			map.flecks.CreateFleck(new FleckCreationData
			{
				def = FleckDefOf.WaterRipple,
				spawnPosition = loc,
				scale = size,
				ageTicksOverride = -1
			});
		}
	}

	public static void ConnectingLine(Vector3 start, Vector3 end, FleckDef fleckDef, Map map, float width = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		float num = val.MagnitudeHorizontal();
		FleckCreationData dataStatic = GetDataStatic(start + val * 0.5f, map, fleckDef);
		dataStatic.exactScale = new Vector3(num, 1f, width);
		dataStatic.rotation = Mathf.Atan2(0f - val.z, val.x) * 57.29578f;
		map.flecks.CreateFleck(dataStatic);
	}
}
