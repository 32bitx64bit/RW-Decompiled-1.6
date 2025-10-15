using UnityEngine;

namespace Verse;

public static class AnimationUtility
{
	public static Vector3 AdjustOffsetForRotationMode(RotationMode mode, Vector3 offset, Pawn pawn)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		switch (mode)
		{
		case RotationMode.None:
			return offset;
		case RotationMode.OneD:
			if (!(pawn.Rotation == Rot4.South))
			{
				return offset;
			}
			return offset * -1f;
		case RotationMode.TwoD:
			return offset.RotatedBy(pawn.Rotation);
		case RotationMode.PawnAimTarget:
			if (pawn.TargetCurrentlyAimingAt.IsValid)
			{
				return offset.RotatedBy((pawn.TargetCurrentlyAimingAt.CenterVector3 - pawn.Position.ToVector3()).ToAngleFlat());
			}
			break;
		}
		return offset;
	}
}
