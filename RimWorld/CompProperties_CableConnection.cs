using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompProperties_CableConnection : CompProperties
{
	public Color color = Color.white;

	public List<List<Vector3>> offsets = new List<List<Vector3>>();

	public bool drawMote;

	public ThingDef moteDef;

	public CompProperties_CableConnection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		compClass = typeof(CompCableConnection);
	}
}
