using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherEvent_LightningStrike : WeatherEvent_LightningFlash
{
	private IntVec3 strikeLoc = IntVec3.Invalid;

	private Mesh boltMesh;

	private static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt");

	public WeatherEvent_LightningStrike(Map map)
		: base(map)
	{
	}

	public WeatherEvent_LightningStrike(Map map, IntVec3 forcedStrikeLoc)
		: base(map)
	{
		strikeLoc = forcedStrikeLoc;
	}

	public override void FireEvent()
	{
		DoStrike(strikeLoc, map, ref boltMesh);
	}

	public static void DoStrike(IntVec3 strikeLoc, Map map, ref Mesh boltMesh)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		SoundDefOf.Thunder_OffMap.PlayOneShotOnCamera(map);
		if (!strikeLoc.IsValid)
		{
			strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(map) && !map.roofGrid.Roofed(sq), map);
		}
		boltMesh = LightningBoltMeshPool.RandomBoltMesh;
		if (!strikeLoc.Fogged(map))
		{
			GenExplosion.DoExplosion(strikeLoc, map, 1.9f, DamageDefOf.Flame, null);
			Vector3 loc = strikeLoc.ToVector3Shifted();
			for (int i = 0; i < 4; i++)
			{
				FleckMaker.ThrowSmoke(loc, map, 1.5f);
				FleckMaker.ThrowMicroSparks(loc, map);
				FleckMaker.ThrowLightningGlow(loc, map, 1.5f);
			}
		}
		SoundInfo info = SoundInfo.InMap(new TargetInfo(strikeLoc, map));
		SoundDefOf.Thunder_OnMap.PlayOneShot(info);
	}

	public override void WeatherEventDraw()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(LightningMat, LightningBrightness), 0);
	}
}
