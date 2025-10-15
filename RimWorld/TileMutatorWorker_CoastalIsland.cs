using Verse;
using Verse.Noise;

namespace RimWorld;

public class TileMutatorWorker_CoastalIsland : TileMutatorWorker_Coast
{
	private const float IslandRadius = 0.65f;

	private const float WaterRadius = 0.95f;

	private const float IslandNoiseFrequency = 0.015f;

	private const float IslandNoiseStrength = 35f;

	private const float WaterSmoothMinStrength = 0.25f;

	private static readonly FloatRange IslandSquashRange = new FloatRange(0.65f, 1f);

	protected override FloatRange CoastOffset => new FloatRange(0.6f);

	public TileMutatorWorker_CoastalIsland(TileMutatorDef def)
		: base(def)
	{
	}

	public override void Init(Map map)
	{
		if (ModsConfig.OdysseyActive)
		{
			base.Init(map);
			float randomInRange = IslandSquashRange.RandomInRange;
			float num = Rand.Range(0f, 360f);
			ModuleBase input2 = new Clamp(input: new DistFromPoint((float)map.Size.x * 0.95f), min: MaxForDeepWater, max: 1.0);
			input2 = new Scale(randomInRange, 1.0, 1.0, input2);
			input2 = new Translate(input: new Rotate(0.0, num, 0.0, input2), x: (float)(-map.Size.x) / 2f, y: 0.0, z: (float)(-map.Size.z) / 2f);
			input2 = MapNoiseUtility.AddDisplacementNoise(input2, 0.015f, 35f, 4, map.Tile.tileId);
			ModuleBase input4 = new DistFromPoint((float)map.Size.x * 0.65f);
			input4 = new ScaleBias(-1.0, 1.0, input4);
			input4 = new Scale(randomInRange, 1.0, 1.0, input4);
			input4 = new Rotate(0.0, num, 0.0, input4);
			input4 = new Translate((float)(-map.Size.x) / 2f, 0.0, (float)(-map.Size.z) / 2f, input4);
			input4 = MapNoiseUtility.AddDisplacementNoise(input4, 0.015f, 35f, 4, map.Tile.tileId);
			coastNoise = new SmoothMin(coastNoise, input2, 0.25);
			coastNoise = new Max(coastNoise, input4);
			NoiseDebugUI.StoreNoiseRender(coastNoise, "island & coast");
		}
	}
}
