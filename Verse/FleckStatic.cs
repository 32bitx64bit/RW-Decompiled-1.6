using UnityEngine;

namespace Verse;

public struct FleckStatic : IFleck
{
	public FleckDef def;

	public Map map;

	public FleckDrawPosition position;

	public float exactRotation;

	public Vector3 originalScale;

	public Vector3 linearScale;

	public Vector3 curvedScale;

	public Color instanceColor;

	public float solidTimeOverride;

	public float ageSecs;

	public int ageTicks;

	public int setupTick;

	public Vector3 spawnPosition;

	public float skidSpeedMultiplierPerTick;

	public float SolidTime
	{
		get
		{
			if (!(solidTimeOverride < 0f))
			{
				return solidTimeOverride;
			}
			return def.solidTime;
		}
	}

	public Vector3 DrawPos => position.ExactPosition;

	public float Lifespan => def.fadeInTime + SolidTime + def.fadeOutTime;

	public bool EndOfLife => ageSecs >= Lifespan;

	public float Alpha
	{
		get
		{
			float num = ageSecs;
			if (num <= def.fadeInTime)
			{
				if (def.fadeInTime > 0f)
				{
					return num / def.fadeInTime;
				}
				return 1f;
			}
			if (num <= def.fadeInTime + SolidTime)
			{
				return 1f;
			}
			if (def.fadeOutTime > 0f)
			{
				return 1f - Mathf.InverseLerp(def.fadeInTime + SolidTime, def.fadeInTime + SolidTime + def.fadeOutTime, num);
			}
			return 1f;
		}
	}

	public Vector3 ExactScale => Vector3.Scale(linearScale, curvedScale);

	public Vector3 AddedScale => ExactScale - originalScale;

	public void Setup(FleckCreationData creationData)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		def = creationData.def;
		linearScale = Vector3.one;
		instanceColor = (Color)(((_003F?)creationData.instanceColor) ?? Color.white);
		solidTimeOverride = creationData.solidTimeOverride ?? (-1f);
		skidSpeedMultiplierPerTick = Rand.Range(0.3f, 0.95f);
		ageSecs = 0f;
		if (creationData.exactScale.HasValue)
		{
			linearScale = creationData.exactScale.Value;
		}
		else
		{
			linearScale = new Vector3(creationData.scale, 1f, creationData.scale);
		}
		originalScale = ExactScale;
		position = new FleckDrawPosition(creationData.spawnPosition, 0f, Vector3.zero, def.unattachedDrawOffset);
		spawnPosition = creationData.spawnPosition;
		exactRotation = creationData.rotation;
		setupTick = Find.TickManager.TicksGame;
		curvedScale = def.scalers?.ScaleAtTime(0f) ?? Vector3.one;
		if (creationData.ageTicksOverride != -1)
		{
			ForceSpawnTick(creationData.ageTicksOverride);
		}
	}

	public bool TimeInterval(float deltaTime, Map map)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (EndOfLife)
		{
			return true;
		}
		ageSecs += deltaTime;
		ageTicks++;
		if (def.growthRate != 0f)
		{
			float num = Mathf.Sign(linearScale.x);
			float num2 = Mathf.Sign(linearScale.z);
			linearScale = new Vector3(linearScale.x + num * (def.growthRate * deltaTime), linearScale.y, linearScale.z + num2 * (def.growthRate * deltaTime));
			linearScale.x = ((num > 0f) ? Mathf.Max(linearScale.x, 0.0001f) : Mathf.Min(linearScale.x, -0.0001f));
			linearScale.z = ((num2 > 0f) ? Mathf.Max(linearScale.z, 0.0001f) : Mathf.Min(linearScale.z, -0.0001f));
		}
		if (def.scalers != null)
		{
			curvedScale = def.scalers.ScaleAtTime(ageSecs);
		}
		return false;
	}

	public void Draw(DrawBatch batch)
	{
		Draw(def.altitudeLayer.AltitudeFor(def.altitudeLayerIncOffset), batch);
	}

	public void Draw(float altitude, DrawBatch batch)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		position.worldPosition.y = altitude;
		int num = setupTick + ((object)(Vector3)(ref spawnPosition)).GetHashCode();
		((Graphic_Fleck)def.GetGraphicData(num).Graphic).DrawFleck(new FleckDrawData
		{
			alpha = Alpha,
			color = instanceColor,
			drawLayer = 0,
			pos = DrawPos,
			rotation = exactRotation,
			scale = ExactScale,
			ageSecs = ageSecs,
			id = num
		}, batch);
	}

	public void ForceSpawnTick(int tick)
	{
		ageTicks = Find.TickManager.TicksGame - tick;
		ageSecs = ageTicks.TicksToSeconds();
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return position.worldPosition;
	}
}
