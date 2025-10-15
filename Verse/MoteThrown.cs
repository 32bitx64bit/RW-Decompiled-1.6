using UnityEngine;
using Verse.Sound;

namespace Verse;

public class MoteThrown : Mote
{
	public float airTimeLeft = 999999f;

	protected Vector3 velocity = Vector3.zero;

	protected bool Flying => airTimeLeft > 0f;

	protected bool Skidding
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

	protected override void TimeInterval(float deltaTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.TimeInterval(deltaTime);
		if (base.Destroyed || (!Flying && !Skidding))
		{
			return;
		}
		Vector3 v = NextExactPosition(deltaTime);
		IntVec3 intVec = new IntVec3(v);
		if (intVec != base.Position)
		{
			if (!intVec.InBounds(base.Map))
			{
				Destroy();
				return;
			}
			if (def.mote.collide && intVec.Filled(base.Map))
			{
				WallHit();
				return;
			}
		}
		base.Position = intVec;
		exactPosition = v;
		if (def.mote.rotateTowardsMoveDirection && velocity != default(Vector3))
		{
			exactRotation = velocity.AngleFlat();
		}
		else
		{
			exactRotation += rotationRate * deltaTime;
		}
		velocity += def.mote.acceleration * deltaTime;
		if (def.mote.speedPerTime != 0f)
		{
			Speed = Mathf.Max(Speed + def.mote.speedPerTime * deltaTime, 0f);
		}
		if (airTimeLeft > 0f)
		{
			airTimeLeft -= deltaTime;
			if (airTimeLeft < 0f)
			{
				airTimeLeft = 0f;
			}
			if (airTimeLeft <= 0f && !def.mote.landSound.NullOrUndefined())
			{
				def.mote.landSound.PlayOneShot(new TargetInfo(base.Position, base.Map));
			}
		}
		if (Skidding)
		{
			Speed *= skidSpeedMultiplierPerTick;
			rotationRate *= skidSpeedMultiplierPerTick;
			if (Speed < 0.02f)
			{
				Speed = 0f;
			}
		}
	}

	protected virtual Vector3 NextExactPosition(float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return exactPosition + velocity * deltaTime;
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

	protected virtual void WallHit()
	{
		airTimeLeft = 0f;
		Speed = 0f;
		rotationRate = 0f;
	}
}
