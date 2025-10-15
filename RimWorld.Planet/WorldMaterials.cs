using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class WorldMaterials
{
	public static readonly Material WorldTerrain;

	public static readonly Material WorldIce;

	public static readonly Material WorldOcean;

	public static readonly Material UngeneratedPlanetParts;

	public static readonly Material Clouds;

	public static readonly Material Rivers;

	public static readonly Material RiversBorder;

	public static readonly Material Roads;

	public const int DebugTileRenderQueue = 3510;

	public const int WorldObjectRenderQueue = 3550;

	public const int WorldLineRenderQueue = 3590;

	public const int DynamicObjectRenderQueue = 3600;

	public const int FeatureNameRenderQueue = 3610;

	public static readonly Material MouseTile;

	public static readonly Material SelectedTile;

	public static readonly Material ClosestTile;

	public static readonly Material CurrentMapTile;

	public static readonly Material Stars;

	public static readonly Material Sun;

	public static readonly Material SmallHills;

	public static readonly Material LargeHills;

	public static readonly Material Mountains;

	public static readonly Material ImpassableMountains;

	public static readonly Material VertexColor;

	public static readonly Material VertexColorTransparent;

	private static readonly Material TargetSquareMatSingle;

	private static int NumMatsPerMode;

	public static Material OverlayModeMatOcean;

	private static Material[] matsFertility;

	private static readonly Color[] FertilitySpectrum;

	private const float TempRange = 50f;

	private static Material[] matsTemperature;

	private static readonly Color[] TemperatureSpectrum;

	private const float ElevationMax = 5000f;

	private static Material[] matsElevation;

	private static readonly Color[] ElevationSpectrum;

	private const float RainfallMax = 5000f;

	private static Material[] matsRainfall;

	private static readonly Color[] RainfallSpectrum;

	public static Material CurTargetingMat
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			TargetSquareMatSingle.color = GenDraw.CurTargetingColor;
			return TargetSquareMatSingle;
		}
	}

	static WorldMaterials()
	{
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		WorldTerrain = MatLoader.LoadMat("World/WorldTerrain", 3500);
		WorldIce = MatLoader.LoadMat("World/WorldIce", 3500);
		WorldOcean = MatLoader.LoadMat("World/WorldOcean", 3500);
		UngeneratedPlanetParts = MatLoader.LoadMat("World/UngeneratedPlanetParts", 3500);
		Clouds = MatLoader.LoadMat("World/Clouds", 3700);
		Rivers = MatLoader.LoadMat("World/Rivers", 3530);
		RiversBorder = MatLoader.LoadMat("World/RiversBorder", 3520);
		Roads = MatLoader.LoadMat("World/Roads", 3540);
		MouseTile = MaterialPool.MatFrom("World/MouseTile", ShaderDatabase.WorldOverlayAdditive, 3560);
		SelectedTile = MaterialPool.MatFrom("World/SelectedTile", ShaderDatabase.WorldOverlayAdditive, 3560);
		ClosestTile = MaterialPool.MatFrom("World/CurrentMapTile", ShaderDatabase.WorldOverlayAdditive, 3560);
		CurrentMapTile = MaterialPool.MatFrom("World/CurrentMapTile", ShaderDatabase.WorldOverlayTransparent, 3560);
		Stars = MatLoader.LoadMat("World/Stars");
		Sun = MatLoader.LoadMat("World/Sun");
		SmallHills = MaterialPool.MatFrom("World/Hills/SmallHills", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		LargeHills = MaterialPool.MatFrom("World/Hills/LargeHills", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		Mountains = MaterialPool.MatFrom("World/Hills/Mountains", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		ImpassableMountains = MaterialPool.MatFrom("World/Hills/Impassable", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		VertexColor = MatLoader.LoadMat("World/WorldVertexColor");
		VertexColorTransparent = MatLoader.LoadMat("World/WorldVertexColorTransparent", 3545);
		TargetSquareMatSingle = MaterialPool.MatFrom("UI/Overlays/TargetHighlight_Square", ShaderDatabase.Transparent, 3560);
		NumMatsPerMode = 50;
		OverlayModeMatOcean = SolidColorMaterials.NewSolidColorMaterial(new Color(0.09f, 0.18f, 0.2f), ShaderDatabase.Transparent);
		FertilitySpectrum = (Color[])(object)new Color[2]
		{
			new Color(0f, 1f, 0f, 0f),
			new Color(0f, 1f, 0f, 0.5f)
		};
		TemperatureSpectrum = (Color[])(object)new Color[8]
		{
			new Color(1f, 1f, 1f),
			new Color(0f, 0f, 1f),
			new Color(0.25f, 0.25f, 1f),
			new Color(0.6f, 0.6f, 1f),
			new Color(0.5f, 0.5f, 0.5f),
			new Color(0.5f, 0.3f, 0f),
			new Color(1f, 0.6f, 0.18f),
			new Color(1f, 0f, 0f)
		};
		ElevationSpectrum = (Color[])(object)new Color[4]
		{
			new Color(0.224f, 0.18f, 0.15f),
			new Color(0.447f, 0.369f, 0.298f),
			new Color(0.6f, 0.6f, 0.6f),
			new Color(1f, 1f, 1f)
		};
		RainfallSpectrum = (Color[])(object)new Color[12]
		{
			new Color(0.9f, 0.9f, 0.9f),
			GenColor.FromBytes(190, 190, 190),
			new Color(0.58f, 0.58f, 0.58f),
			GenColor.FromBytes(196, 112, 110),
			GenColor.FromBytes(200, 179, 150),
			GenColor.FromBytes(255, 199, 117),
			GenColor.FromBytes(255, 255, 84),
			GenColor.FromBytes(145, 255, 253),
			GenColor.FromBytes(0, 255, 0),
			GenColor.FromBytes(63, 198, 55),
			GenColor.FromBytes(13, 150, 5),
			GenColor.FromBytes(5, 112, 94)
		};
		GenerateMats(ref matsFertility, FertilitySpectrum, NumMatsPerMode);
		GenerateMats(ref matsTemperature, TemperatureSpectrum, NumMatsPerMode);
		GenerateMats(ref matsElevation, ElevationSpectrum, NumMatsPerMode);
		GenerateMats(ref matsRainfall, RainfallSpectrum, NumMatsPerMode);
	}

	private static void GenerateMats(ref Material[] mats, Color[] colorSpectrum, int numMats)
	{
		mats = (Material[])(object)new Material[numMats];
		for (int i = 0; i < numMats; i++)
		{
			mats[i] = MatsFromSpectrum.Get(colorSpectrum, (float)i / (float)numMats);
		}
	}

	public static Material MatForFertilityOverlay(float fert)
	{
		int num = Mathf.FloorToInt(fert * (float)NumMatsPerMode);
		return matsFertility[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForTemperature(float temp)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(-50f, 50f, temp) * (float)NumMatsPerMode);
		return matsTemperature[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForElevation(float elev)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(0f, 5000f, elev) * (float)NumMatsPerMode);
		return matsElevation[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForRainfallOverlay(float rain)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(0f, 5000f, rain) * (float)NumMatsPerMode);
		return matsRainfall[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}
}
