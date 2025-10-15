using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_MoteEmitter : CompProperties
{
	public ThingDef mote;

	public List<ThingDef> perRotationMotes;

	public Vector3 offset;

	public Vector3 offsetMin = Vector3.zero;

	public Vector3 offsetMax = Vector3.zero;

	public Vector3 offsetNorth = Vector3.zero;

	public Vector3 offsetSouth = Vector3.zero;

	public Vector3 offsetEast = Vector3.zero;

	public Vector3 offsetWest = Vector3.zero;

	public bool useParentRotation;

	public SoundDef soundOnEmission;

	public int emissionInterval = -1;

	public int ticksSinceLastEmittedMaxOffset;

	public bool maintain;

	[NoTranslate]
	public string saveKeysPrefix;

	public Vector3 EmissionOffset => new Vector3(Rand.Range(offsetMin.x, offsetMax.x), Rand.Range(offsetMin.y, offsetMax.y), Rand.Range(offsetMin.z, offsetMax.z));

	public CompProperties_MoteEmitter()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompMoteEmitter);
	}

	public Vector3 RotationOffset(Rot4 rot)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		return (Vector3)(rot.AsInt switch
		{
			0 => offsetNorth, 
			1 => offsetEast, 
			2 => offsetSouth, 
			3 => offsetWest, 
			_ => Vector3.zero, 
		});
	}

	public ThingDef RotationMote(Rot4 rot)
	{
		return perRotationMotes?[rot.AsInt];
	}

	public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
	{
		if (mote == null && perRotationMotes.NullOrEmpty())
		{
			yield return "CompMoteEmitter must have a mote assigned.";
		}
		if (!perRotationMotes.NullOrEmpty() && perRotationMotes.Count != 4)
		{
			yield return "perRotationMotes must contain 4 elements for North, East, South, West";
		}
	}
}
