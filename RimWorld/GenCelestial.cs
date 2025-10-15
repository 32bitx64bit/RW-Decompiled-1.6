using System;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GenCelestial
{
	public struct LightInfo
	{
		public Vector2 vector;

		public float intensity;
	}

	public enum LightType
	{
		Shadow,
		LightingSun,
		LightingMoon
	}

	public const float ShadowMaxLengthDay = 15f;

	public const float ShadowMaxLengthNight = 15f;

	private const float ShadowGlowLerpSpan = 0.15f;

	private const float ShadowDayNightThreshold = 0.6f;

	private static SimpleCurve SunPeekAroundDegreesFactorCurve = new SimpleCurve
	{
		new CurvePoint(70f, 1f),
		new CurvePoint(75f, 0.05f)
	};

	private static SimpleCurve SunOffsetFractionFromLatitudeCurve = new SimpleCurve
	{
		new CurvePoint(70f, 0.2f),
		new CurvePoint(75f, 1.5f)
	};

	private static int TicksAbsForSunPosInWorldSpace
	{
		get
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (Current.ProgramState != 0)
			{
				return GenTicks.TicksAbs;
			}
			PlanetTile startingTile = Find.GameInitData.startingTile;
			float longitude = (startingTile.Valid ? Find.WorldGrid.LongLatOf(startingTile).x : 0f);
			return Mathf.RoundToInt(2500f * (12f - GenDate.TimeZoneFloatAt(longitude)));
		}
	}

	public static float CurCelestialSunGlow(Map map)
	{
		return CelestialSunGlow(map, Find.TickManager.TicksAbs);
	}

	public static float CelestialSunGlow(Map map, int ticksAbs)
	{
		return CelestialSunGlow(map.Tile, ticksAbs);
	}

	public static float CelestialSunGlow(PlanetTile tile, int ticksAbs)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Find.WorldGrid.LongLatOf(tile);
		return CelestialSunGlowPercent(val.y, GenDate.DayOfYear(ticksAbs, val.x), GenDate.DayPercent(ticksAbs, val.x));
	}

	public static float CurShadowStrength(Map map)
	{
		return Mathf.Clamp01(Mathf.Abs(CurCelestialSunGlow(map) - 0.6f) / 0.15f);
	}

	public static LightInfo GetLightSourceInfo(Map map, LightType type)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		float num = GenLocalDate.DayPercent(map);
		bool flag;
		float intensity;
		switch (type)
		{
		case LightType.Shadow:
			flag = IsDaytime(CurCelestialSunGlow(map));
			intensity = CurShadowStrength(map);
			break;
		case LightType.LightingSun:
			flag = true;
			intensity = Mathf.Clamp01((CurCelestialSunGlow(map) - 0.6f + 0.2f) / 0.15f);
			break;
		case LightType.LightingMoon:
			flag = false;
			intensity = Mathf.Clamp01((0f - (CurCelestialSunGlow(map) - 0.6f - 0.2f)) / 0.15f);
			break;
		default:
			Log.ErrorOnce("Invalid light type requested", 64275614);
			flag = true;
			intensity = 0f;
			break;
		}
		float num2;
		float num3;
		float num4;
		if (flag)
		{
			num2 = num;
			num3 = -1.5f;
			num4 = 15f;
		}
		else
		{
			num2 = ((!(num > 0.5f)) ? (0.5f + Mathf.InverseLerp(0f, 0.5f, num) * 0.5f) : (Mathf.InverseLerp(0.5f, 1f, num) * 0.5f));
			num3 = -0.9f;
			num4 = 15f;
		}
		float num5 = Mathf.LerpUnclamped(0f - num4, num4, num2);
		float num6 = num3 - 2.5f * (num5 * num5 / 100f);
		LightInfo result = default(LightInfo);
		result.vector = new Vector2(num5, num6);
		result.intensity = intensity;
		return result;
	}

	public static Vector3 CurSunPositionInWorldSpace()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		int ticksAbsForSunPosInWorldSpace = TicksAbsForSunPosInWorldSpace;
		return SunPositionUnmodified(GenDate.DayOfYear(ticksAbsForSunPosInWorldSpace, 0f), GenDate.DayPercent(ticksAbsForSunPosInWorldSpace, 0f), new Vector3(0f, 0f, -1f));
	}

	public static bool IsDaytime(float glow)
	{
		return glow > 0.6f;
	}

	private static Vector3 SunPosition(float latitude, int dayOfYear, float dayPercent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = SurfaceNormal(latitude);
		Vector3 val2 = SunPositionUnmodified(dayOfYear, dayPercent, new Vector3(1f, 0f, 0f), latitude);
		float num = SunPeekAroundDegreesFactorCurve.Evaluate(latitude);
		val2 = Vector3.RotateTowards(val2, val, MathF.PI * 19f / 180f * num, 9999999f);
		float num2 = Mathf.InverseLerp(60f, 0f, Mathf.Abs(latitude));
		if (num2 > 0f)
		{
			val2 = Vector3.RotateTowards(val2, val, MathF.PI * 2f * (17f * num2 / 360f), 9999999f);
		}
		return ((Vector3)(ref val2)).normalized;
	}

	private static Vector3 SunPositionUnmodified(float dayOfYear, float dayPercent, Vector3 initialSunPos, float latitude = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = initialSunPos * 100f;
		float num = 0f - Mathf.Cos(dayOfYear / 60f * MathF.PI * 2f);
		val.y += num * 100f * SunOffsetFractionFromLatitudeCurve.Evaluate(latitude);
		val = Quaternion.AngleAxis((dayPercent - 0.5f) * 360f, Vector3.up) * val;
		return ((Vector3)(ref val)).normalized;
	}

	private static float CelestialSunGlowPercent(float latitude, int dayOfYear, float dayPercent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = SurfaceNormal(latitude);
		Vector3 val2 = SunPosition(latitude, dayOfYear, dayPercent);
		float num = Vector3.Dot(((Vector3)(ref val)).normalized, val2);
		return Mathf.Clamp01(Mathf.InverseLerp(0f, 0.7f, num));
	}

	public static float AverageGlow(float latitude, int dayOfYear)
	{
		float num = 0f;
		for (int i = 0; i < 24; i++)
		{
			num += CelestialSunGlowPercent(latitude, dayOfYear, (float)i / 24f);
		}
		return num / 24f;
	}

	private static Vector3 SurfaceNormal(float latitude)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(1f, 0f, 0f);
		val = Quaternion.AngleAxis(latitude, new Vector3(0f, 0f, 1f)) * val;
		return val;
	}

	public static void LogSunGlowForYear()
	{
		for (int i = -90; i <= 90; i += 10)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Sun visibility percents for latitude " + i + ", for each hour of each day of the year");
			stringBuilder.AppendLine("---------------------------------------");
			stringBuilder.Append("Day/hr".PadRight(6));
			for (int j = 0; j < 24; j += 2)
			{
				stringBuilder.Append((j + "h").PadRight(6));
			}
			stringBuilder.AppendLine();
			for (int k = 0; k < 60; k += 5)
			{
				stringBuilder.Append(k.ToString().PadRight(6));
				for (int l = 0; l < 24; l += 2)
				{
					stringBuilder.Append(CelestialSunGlowPercent(i, k, (float)l / 24f).ToString("F2").PadRight(6));
				}
				stringBuilder.AppendLine();
			}
			Log.Message(stringBuilder.ToString());
		}
	}

	public static void LogSunAngleForYear()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		for (int i = -90; i <= 90; i += 10)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Sun angles for latitude " + i + ", for each hour of each day of the year");
			stringBuilder.AppendLine("---------------------------------------");
			stringBuilder.Append("Day/hr".PadRight(6));
			for (int j = 0; j < 24; j += 2)
			{
				stringBuilder.Append((j + "h").PadRight(6));
			}
			stringBuilder.AppendLine();
			for (int k = 0; k < 60; k += 5)
			{
				stringBuilder.Append(k.ToString().PadRight(6));
				for (int l = 0; l < 24; l += 2)
				{
					float num = Vector3.Angle(SurfaceNormal(i), SunPositionUnmodified(k, (float)l / 24f, new Vector3(1f, 0f, 0f)));
					stringBuilder.Append((90f - num).ToString("F0").PadRight(6));
				}
				stringBuilder.AppendLine();
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
