using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld;

public class JobDriver_LayDownAwake : JobDriver_LayDown
{
	public override bool CanSleep => false;

	public override bool CanRest => false;

	public override bool LookForOtherJobs => true;

	public override Rot4 ForcedLayingRotation => job.GetTarget(TargetIndex.A).Thing?.Rotation.Rotated(RotationDirection.Clockwise) ?? base.ForcedLayingRotation;

	public override Vector3 ForcedBodyOffset
	{
		get
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Thing thing = job.GetTarget(TargetIndex.A).Thing;
			if (thing != null && thing.def.Size.z > 1)
			{
				return Vector3Utility.RotatedBy(new Vector3(0f, 0f, 0.5f), thing.Rotation);
			}
			return base.ForcedBodyOffset;
		}
	}

	public override string GetReport()
	{
		return "ReportLayingDown".Translate();
	}
}
