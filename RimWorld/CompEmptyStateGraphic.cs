using UnityEngine;
using Verse;

namespace RimWorld;

public class CompEmptyStateGraphic : ThingComp
{
	private CompProperties_EmptyStateGraphic Props => (CompProperties_EmptyStateGraphic)props;

	public bool ParentIsEmpty
	{
		get
		{
			if (parent is IThingHolder thingHolder && thingHolder.GetDirectlyHeldThings().NullOrEmpty())
			{
				return true;
			}
			CompPawnSpawnOnWakeup compPawnSpawnOnWakeup = parent.TryGetComp<CompPawnSpawnOnWakeup>();
			if (compPawnSpawnOnWakeup != null && !compPawnSpawnOnWakeup.CanSpawn)
			{
				return true;
			}
			return false;
		}
	}

	public override bool DontDrawParent()
	{
		if (ParentIsEmpty)
		{
			return !Props.alwaysDrawParent;
		}
		return false;
	}

	public override void PostDraw()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (ParentIsEmpty && parent.def.drawerType != DrawerType.MapMeshOnly)
		{
			Mesh obj = Props.graphicData.Graphic.MeshAt(parent.Rotation);
			Vector3 drawPos = parent.DrawPos;
			Graphics.DrawMesh(obj, drawPos + Props.graphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, Props.graphicData.Graphic.MatAt(parent.Rotation), 0);
		}
	}

	public override void PostPrintOnto(SectionLayer layer)
	{
		if (ParentIsEmpty)
		{
			Props.graphicData.Graphic.Print(layer, parent, 0f);
		}
	}
}
