using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_ThrownMoteEmitter : CompProperties
{
	public ThingDef mote;

	public Vector3 offsetMin;

	public Vector3 offsetMax;

	public int emissionInterval = -1;

	public int burstCount = 1;

	public Color colorA = Color.white;

	public Color colorB = Color.white;

	public FloatRange scale = FloatRange.One;

	public FloatRange rotationRate;

	public FloatRange velocityX;

	public FloatRange velocityY;

	public CompProperties_ThrownMoteEmitter()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompThrownMoteEmitter);
	}

	public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
	{
		if (mote == null)
		{
			yield return "CompThrownMoteEmitter must have a mote assigned.";
		}
	}
}
