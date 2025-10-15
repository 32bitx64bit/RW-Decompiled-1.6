using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld;

public class TileMutatorWorker_Plateau : TileMutatorWorker
{
	private ModuleBase plateauNoise;

	private const float PlateauRadius = 0.4f;

	private const float PlateauExp = 0.2f;

	private const float PlateauNoiseFrequency = 0.015f;

	private const float PlateauNoiseStrength = 35f;

	private const float PlateauThreshold = 0f;

	private static readonly FloatRange CenterOffsetRange = new FloatRange(-0.2f, 0.2f);

	public TileMutatorWorker_Plateau(TileMutatorDef def)
		: base(def)
	{
	}

	public override void Init(Map map)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (ModsConfig.OdysseyActive)
		{
			Vector2 offset = map.Center.ToVector2() + new Vector2((float)map.Size.x * CenterOffsetRange.RandomInRange, (float)map.Size.z * CenterOffsetRange.RandomInRange);
			float? num = Find.World.CoastAngleAt(map.Tile, BiomeDefOf.Ocean) ?? Find.World.CoastAngleAt(map.Tile, BiomeDefOf.Lake);
			if (num.HasValue)
			{
				offset = map.Center.ToVector2() + Vector2Utility.FromAngle(num.Value) * (float)map.Size.x * CenterOffsetRange.max;
			}
			plateauNoise = MapNoiseUtility.CreateFalloffRadius((float)map.Size.x * 0.4f, offset, 0.2f);
			plateauNoise = MapNoiseUtility.AddDisplacementNoise(plateauNoise, 0.015f, 35f);
			NoiseDebugUI.StoreNoiseRender(plateauNoise, "plateau");
		}
	}

	public override void GeneratePostElevationFertility(Map map)
	{
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		foreach (IntVec3 allCell in map.AllCells)
		{
			float value = plateauNoise.GetValue(allCell);
			if (value > 0f)
			{
				elevation[allCell] = value;
			}
		}
	}
}
