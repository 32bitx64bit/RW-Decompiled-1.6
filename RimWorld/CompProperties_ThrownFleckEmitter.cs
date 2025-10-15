using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_ThrownFleckEmitter : CompProperties
{
	public FleckDef fleck;

	public Vector3 offsetMin;

	public Vector3 offsetMax;

	public int emissionInterval = -1;

	public int burstCount = 1;

	public Color colorA = Color.white;

	public Color colorB = Color.white;

	public FloatRange scale;

	public FloatRange rotationRate;

	public FloatRange velocityX;

	public FloatRange velocityY;

	public CompProperties_ThrownFleckEmitter()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompThrownFleckEmitter);
	}

	public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
	{
		if (fleck == null)
		{
			yield return "CompThrownFleckEmitter must have a fleck assigned.";
		}
	}
}
