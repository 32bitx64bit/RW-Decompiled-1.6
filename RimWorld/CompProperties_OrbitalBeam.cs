using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_OrbitalBeam : CompProperties
{
	public float width = 8f;

	public Color color = Color.white;

	public SoundDef sound;

	public CompProperties_OrbitalBeam()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompOrbitalBeam);
	}

	public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
	{
		foreach (string item in base.ConfigErrors(parentDef))
		{
			yield return item;
		}
		if (parentDef.drawerType != DrawerType.RealtimeOnly && parentDef.drawerType != DrawerType.MapMeshAndRealTime)
		{
			yield return "orbital beam requires realtime drawer";
		}
	}
}
