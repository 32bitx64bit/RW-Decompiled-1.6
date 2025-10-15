using System;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public struct FleckThrown : IFleck
{
	public FleckStatic baseData;

	public float airTimeLeft;

	public Vector3 velocity;

	public float rotationRate;

	public FleckAttachLink link;

	private Vector3 attacheeLastPosition;

	public float orbitSpeed;

	public float archHeight;

	public float? archDuration;

	public float orbitSnapStrength;

	public const float MinSpeed = 0.02f;

	public const float MinOrbitSpeed = 0.2f;

	public float orbitDistance;

	private float orbitAccum;

	public bool Flying => airTimeLeft > 0f;

	public bool Skidding
	{
		get
		{
			if (!Flying)
			{
				return Speed > 0.01f;
			}
			return false;
		}
	}

	public bool Orbiting => orbitSpeed != 0f;

	public bool Arching => archHeight != 0f;

	public Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			velocity = value;
		}
	}

	public float MoveAngle
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity.AngleFlat();
		}
		set
		{
			SetVelocity(value, Speed);
		}
	}

	public float Speed
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity.MagnitudeHorizontal();
		}
		set
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (value == 0f)
			{
				velocity = Vector3.zero;
			}
			else if (velocity == Vector3.zero)
			{
				velocity = new Vector3(value, 0f, 0f);
			}
			else
			{
				velocity = ((Vector3)(ref velocity)).normalized * value;
			}
		}
	}

	private void ApplyOrbit(float deltaTime, ref Vector3 nextPosition)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		orbitAccum += deltaTime * orbitSpeed;
		Vector3 val = new Vector3(Mathf.Cos(orbitAccum), 0f, Mathf.Sin(orbitAccum)) * orbitDistance;
		Vector3 val2 = link.Target.CenterVector3 + val;
		MoveAngle = (val2 - baseData.position.worldPosition).AngleFlat();
		nextPosition = Vector3.Lerp(nextPosition, val2, orbitSnapStrength);
	}

	public void Setup(FleckCreationData creationData)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		baseData = default(FleckStatic);
		baseData.Setup(creationData);
		airTimeLeft = creationData.airTimeLeft ?? 999999f;
		attacheeLastPosition = new Vector3(-1000f, -1000f, -1000f);
		link = creationData.link;
		if (link.Linked)
		{
			attacheeLastPosition = link.LastDrawPos;
		}
		rotationRate = creationData.rotationRate;
		SetVelocity(creationData.velocityAngle, creationData.velocitySpeed);
		if (creationData.velocity.HasValue)
		{
			velocity += creationData.velocity.Value;
		}
		orbitSpeed = creationData.orbitSpeed;
		orbitSnapStrength = creationData.orbitSnapStrength;
		if (Orbiting)
		{
			orbitDistance = (link.Target.CenterVector3 - baseData.position.worldPosition).MagnitudeHorizontal();
			orbitAccum = Rand.Range(0f, MathF.PI * 2f);
			if (Mathf.Abs(orbitSpeed) < 0.2f)
			{
				orbitSpeed = 0.2f * GenMath.Sign(orbitSpeed);
			}
		}
		archHeight = creationData.def.archHeight.RandomInRange;
		archDuration = creationData.def.archDuration.RandomInRange;
	}

	public bool TimeInterval(float deltaTime, Map map)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (baseData.TimeInterval(deltaTime, map))
		{
			return true;
		}
		if (!Flying && !Skidding)
		{
			return false;
		}
		if (baseData.def.rotateTowardsMoveDirection && velocity != default(Vector3))
		{
			baseData.exactRotation = velocity.AngleFlat() + baseData.def.rotateTowardsMoveDirectionExtraAngle;
		}
		else
		{
			baseData.exactRotation += rotationRate * deltaTime;
		}
		velocity += baseData.def.acceleration * deltaTime;
		if (baseData.def.speedPerTime != FloatRange.Zero)
		{
			Speed = Mathf.Max(Speed + baseData.def.speedPerTime.RandomInRange * deltaTime, 0f);
		}
		if (airTimeLeft > 0f)
		{
			airTimeLeft -= deltaTime;
			if (airTimeLeft < 0f)
			{
				airTimeLeft = 0f;
			}
			if (airTimeLeft <= 0f && !baseData.def.landSound.NullOrUndefined())
			{
				baseData.def.landSound.PlayOneShot(new TargetInfo(new IntVec3(baseData.position.worldPosition), map));
			}
		}
		if (Skidding)
		{
			Speed *= baseData.skidSpeedMultiplierPerTick;
			rotationRate *= baseData.skidSpeedMultiplierPerTick;
			if (Speed < 0.02f)
			{
				Speed = 0f;
			}
		}
		FleckDrawPosition position = NextPosition(deltaTime);
		if (Orbiting)
		{
			ApplyOrbit(deltaTime, ref position.worldPosition);
		}
		IntVec3 intVec = new IntVec3(position.worldPosition);
		if (intVec != new IntVec3(baseData.position.worldPosition))
		{
			if (!intVec.InBounds(map))
			{
				return true;
			}
			if (baseData.def.collide && intVec.Filled(map))
			{
				WallHit();
				return false;
			}
		}
		baseData.position = position;
		return false;
	}

	private FleckDrawPosition NextPosition(float deltaTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = baseData.position.worldPosition + velocity * deltaTime;
		float height = 0f;
		Vector3 attachedOffset = Vector3.zero;
		if (Arching)
		{
			float x = Mathf.Clamp01(baseData.ageSecs / archDuration.Value);
			height = ((baseData.def.archCurve != null) ? (archHeight * baseData.def.archCurve.Evaluate(x)) : (archHeight * GenMath.InverseParabola(x)));
		}
		if (link.Target.HasThing)
		{
			bool flag = link.detachAfterTicks == -1 || baseData.ageTicks < link.detachAfterTicks;
			if (!link.Target.ThingDestroyed && flag)
			{
				link.UpdateDrawPos();
			}
			Vector3 attachedDrawOffset = baseData.def.attachedDrawOffset;
			if (baseData.def.attachedToHead && link.Target.Thing is Pawn { story: not null, SpawnedOrAnyParentSpawned: not false } pawn)
			{
				attachedOffset = pawn.Drawer.renderer.BaseHeadOffsetAt((pawn.GetPosture() == PawnPosture.Standing) ? Rot4.North : pawn.Drawer.renderer.LayingFacing()).RotatedBy(pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			}
			Vector3 val2 = link.LastDrawPos - attacheeLastPosition;
			val += val2;
			val += attachedDrawOffset;
			val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
			attacheeLastPosition = link.LastDrawPos;
		}
		Vector3 anchorOffset = (new Vector3(0.5f, 0f, 0.5f) - baseData.def.scalingAnchor).ScaledBy(baseData.AddedScale);
		return new FleckDrawPosition(val, height, anchorOffset, baseData.def.unattachedDrawOffset, attachedOffset);
	}

	public void SetVelocity(float angle, float speed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		velocity = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * speed;
	}

	public void Draw(DrawBatch batch)
	{
		baseData.Draw(batch);
	}

	private void WallHit()
	{
		airTimeLeft = 0f;
		Speed = 0f;
		rotationRate = 0f;
	}

	public Vector3 GetPosition()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return baseData.position.worldPosition;
	}
}
