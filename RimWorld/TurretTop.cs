using UnityEngine;
using Verse;

namespace RimWorld;

public class TurretTop
{
	private Building_Turret parentTurret;

	private float curRotationInt;

	private int ticksUntilIdleTurn;

	private int idleTurnTicksLeft;

	private bool idleTurnClockwise;

	private const float IdleTurnDegreesPerTick = 0.26f;

	private const int IdleTurnDuration = 140;

	private const int IdleTurnIntervalMin = 150;

	private const int IdleTurnIntervalMax = 350;

	public static readonly int ArtworkRotation = -90;

	public float CurRotation
	{
		get
		{
			return curRotationInt;
		}
		set
		{
			curRotationInt = value;
			if (curRotationInt > 360f)
			{
				curRotationInt -= 360f;
			}
			if (curRotationInt < 0f)
			{
				curRotationInt += 360f;
			}
		}
	}

	public void SetRotationFromOrientation()
	{
		CurRotation = parentTurret.Rotation.AsAngle;
	}

	public TurretTop(Building_Turret ParentTurret)
	{
		parentTurret = ParentTurret;
	}

	public void ForceFaceTarget(LocalTargetInfo targ)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (targ.IsValid)
		{
			float curRotation = (targ.Cell.ToVector3Shifted() - parentTurret.DrawPos).AngleFlat();
			CurRotation = curRotation;
		}
	}

	public void TurretTopTick()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		LocalTargetInfo currentTarget = parentTurret.CurrentTarget;
		if (currentTarget.IsValid)
		{
			float curRotation = (currentTarget.Cell.ToVector3Shifted() - parentTurret.DrawPos).AngleFlat();
			CurRotation = curRotation;
			ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
		}
		else if (ticksUntilIdleTurn > 0)
		{
			ticksUntilIdleTurn--;
			if (ticksUntilIdleTurn == 0)
			{
				if (Rand.Value < 0.5f)
				{
					idleTurnClockwise = true;
				}
				else
				{
					idleTurnClockwise = false;
				}
				idleTurnTicksLeft = 140;
			}
		}
		else
		{
			if (idleTurnClockwise)
			{
				CurRotation += 0.26f;
			}
			else
			{
				CurRotation -= 0.26f;
			}
			idleTurnTicksLeft--;
			if (idleTurnTicksLeft <= 0)
			{
				ticksUntilIdleTurn = Rand.RangeInclusive(150, 350);
			}
		}
	}

	public void DrawTurret(Vector3 drawLoc, Vector3 recoilDrawOffset, float recoilAngleOffset)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 v = Vector3Utility.RotatedBy(new Vector3(parentTurret.def.building.turretTopOffset.x, 0f, parentTurret.def.building.turretTopOffset.y), CurRotation);
		float turretTopDrawSize = parentTurret.def.building.turretTopDrawSize;
		v = v.RotatedBy(recoilAngleOffset);
		v += recoilDrawOffset;
		float num = parentTurret.CurrentEffectiveVerb?.AimAngleOverride ?? CurRotation;
		Vector3 val = drawLoc + Altitudes.AltIncVect + v;
		Quaternion val2 = ((float)ArtworkRotation + num).ToQuat();
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(turretTopDrawSize, 1f, turretTopDrawSize);
		Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(val, val2, val3), parentTurret.TurretTopMaterial, 0);
	}
}
