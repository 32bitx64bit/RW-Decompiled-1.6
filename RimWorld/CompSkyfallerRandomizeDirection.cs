using UnityEngine;
using Verse;

namespace RimWorld;

public class CompSkyfallerRandomizeDirection : ThingComp
{
	private static int DirectionChangeBlendDuration = 100;

	private int directionChangeInterval;

	private int lastDirectionChange;

	private float initialAngle;

	private float currentAngle;

	private float lastAngle;

	private Vector3 currentOffset;

	public CompProperties_SkyfallerRandomizeDirection Props => (CompProperties_SkyfallerRandomizeDirection)props;

	public Skyfaller Skyfaller => (Skyfaller)parent;

	public Vector3 Offset => currentOffset;

	public float ExtraDrawAngle => Mathf.Lerp(lastAngle, currentAngle, Mathf.Clamp((float)(Find.TickManager.TicksGame + DirectionChangeBlendDuration / 2 - lastDirectionChange), 0f, (float)DirectionChangeBlendDuration) / (float)DirectionChangeBlendDuration) / 2f;

	public override void PostPostMake()
	{
		base.PostPostMake();
		initialAngle = Skyfaller.angle;
		directionChangeInterval = Props.directionChangeInterval.RandomInRange;
		lastDirectionChange = Find.TickManager.TicksGame;
	}

	public override void CompTick()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		base.CompTick();
		if (parent.IsHashIntervalTick(directionChangeInterval))
		{
			lastAngle = currentAngle;
			currentAngle = Rand.Value * Props.maxDeviationFromStartingAngle * (float)Rand.Sign;
			lastDirectionChange = Find.TickManager.TicksGame;
		}
		Quaternion val = Quaternion.AngleAxis(currentAngle - initialAngle, Vector3.up);
		currentOffset += val * Vector3.forward * Time.deltaTime;
	}

	public override void PostExposeData()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.PostExposeData();
		Scribe_Values.Look(ref directionChangeInterval, "directionChangeInterval", 0);
		Scribe_Values.Look(ref currentAngle, "currentAngle", 0f);
		Scribe_Values.Look(ref lastAngle, "lastAngle", 0f);
		Scribe_Values.Look(ref currentOffset, "currentOffset");
		Scribe_Values.Look(ref initialAngle, "initialAngle", 0f);
	}
}
