using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class ResourceAsteroidMapParent : SpaceMapParent
{
	private Color cachedStuffColor;

	public override Color ExpandingIconColor => cachedStuffColor;

	public override void SpawnSetup()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base.SpawnSetup();
		ThingDef mineableThing = preciousResource.building.mineableThing;
		cachedStuffColor = mineableThing.stuffProps.color;
	}
}
