using UnityEngine;
using Verse;

namespace RimWorld;

public struct IncineratorProjectileMotion
{
	public MoteDualAttached mote;

	public Vector3 worldSource;

	public Vector3 worldTarget;

	public IntVec3 targetDest;

	public Vector3 moveVector;

	public float startScale;

	public float endScale;

	public int ticks;

	public int lifespanTicks;

	public float Alpha => Mathf.Clamp01((float)ticks / (float)lifespanTicks);

	public Vector3 LerpdPos => Vector3.Lerp(worldSource, worldTarget, Alpha);

	public void Tick(Map map)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		ticks++;
		Vector3 lerpdPos = LerpdPos;
		IntVec3 cell = lerpdPos.ToIntVec3();
		Vector3 offsetA = lerpdPos - cell.ToVector3Shifted();
		Vector3 val = LerpdPos - moveVector * 2f;
		IntVec3 cell2 = val.ToIntVec3();
		Vector3 offsetB = val - cell2.ToVector3Shifted();
		mote.Scale = Mathf.Lerp(startScale, endScale, Alpha);
		mote.UpdateTargets(new TargetInfo(cell, map), new TargetInfo(cell2, map), offsetA, offsetB);
		if (Alpha < 1f)
		{
			mote.Maintain();
		}
	}
}
