using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_Atomizer : CompProperties_ThingContainer
{
	public ThingDef thingDef;

	public EffecterDef resolveEffecter;

	public EffecterDef workingEffecter;

	public SoundDef materialsAddedSound;

	public int resolveEffecterTicks = 40;

	public int ticksPerAtomize = 2500;

	public Vector3 contentsDrawOffset = Vector3.zero;

	public CompProperties_Atomizer()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompAtomizer);
	}
}
