using UnityEngine;

namespace Verse;

public class JitterHandler
{
	private Vector3 curOffset = new Vector3(0f, 0f, 0f);

	private float DamageJitterDistance = 0.17f;

	private float DeflectJitterDistance = 0.1f;

	private float JitterDropPerTick = 0.018f;

	private float JitterMax = 0.35f;

	public Vector3 CurrentOffset => curOffset;

	public void ProcessPostTickVisuals(int ticksPassed)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)ticksPassed * JitterDropPerTick;
		if (((Vector3)(ref curOffset)).sqrMagnitude < num * num)
		{
			curOffset = new Vector3(0f, 0f, 0f);
		}
		else
		{
			curOffset -= ((Vector3)(ref curOffset)).normalized * num;
		}
	}

	public void Notify_DamageApplied(DamageInfo dinfo)
	{
		if (dinfo.Def.hasForcefulImpact)
		{
			AddOffset(DamageJitterDistance, dinfo.Angle);
		}
	}

	public void Notify_DamageDeflected(DamageInfo dinfo)
	{
		if (dinfo.Def.hasForcefulImpact)
		{
			AddOffset(DeflectJitterDistance, dinfo.Angle);
		}
	}

	public void AddOffset(float dist, float dir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		curOffset += Quaternion.AngleAxis(dir, Vector3.up) * Vector3.forward * dist;
		if (((Vector3)(ref curOffset)).sqrMagnitude > JitterMax * JitterMax)
		{
			curOffset *= JitterMax / ((Vector3)(ref curOffset)).magnitude;
		}
	}
}
