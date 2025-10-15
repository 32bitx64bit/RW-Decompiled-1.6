using UnityEngine;

namespace Verse;

public struct MoteAttachLink
{
	private TargetInfo targetInt;

	private Vector3 offsetInt;

	private Vector3 lastDrawPosInt;

	public bool rotateWithTarget;

	public static readonly MoteAttachLink Invalid = new MoteAttachLink(TargetInfo.Invalid, Vector3.zero);

	public bool Linked => targetInt.IsValid;

	public TargetInfo Target => targetInt;

	public Vector3 LastDrawPos => lastDrawPosInt;

	public MoteAttachLink(TargetInfo target, Vector3 offset, bool rotateWithTarget = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		targetInt = target;
		offsetInt = offset;
		this.rotateWithTarget = rotateWithTarget;
		lastDrawPosInt = Vector3.zero;
		if (target.IsValid)
		{
			UpdateDrawPos();
		}
	}

	public void UpdateTarget(TargetInfo target, Vector3 offset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		targetInt = target;
		offsetInt = offset;
	}

	public Vector3 GetOffset()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!rotateWithTarget || !targetInt.HasThing || !targetInt.Thing.SpawnedOrAnyParentSpawned)
		{
			return offsetInt;
		}
		return offsetInt.RotatedBy(targetInt.Thing.Rotation);
	}

	public void UpdateDrawPos()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (targetInt.HasThing && targetInt.Thing.SpawnedOrAnyParentSpawned)
		{
			lastDrawPosInt = targetInt.Thing.SpawnedParentOrMe.DrawPos + GetOffset();
		}
		else
		{
			lastDrawPosInt = targetInt.Cell.ToVector3Shifted() + GetOffset();
		}
	}
}
