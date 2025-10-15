using UnityEngine;

namespace Verse;

public class Graphic_LinkedAsymmetric : Graphic_Linked
{
	public override LinkDrawerType LinkerType => LinkDrawerType.Asymmetric;

	public Graphic_LinkedAsymmetric(Graphic subGraphic)
		: base(subGraphic)
	{
	}

	public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new Graphic_LinkedAsymmetric(subGraphic.GetColoredVersion(newShader, newColor, newColorTwo))
		{
			data = data
		};
	}

	public override void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		base.Print(layer, thing, extraRotation);
		IntVec3 cell;
		Map map;
		if (thing.def.graphicData.asymmetricLink != null && thing.def.graphicData.asymmetricLink.linkToDoors)
		{
			cell = thing.Position;
			map = thing.Map;
			if (thing.def.graphicData.asymmetricLink.drawDoorBorderEast != null)
			{
				DrawBorder(IntVec3.East, thing.def.graphicData.asymmetricLink.drawDoorBorderEast);
			}
			if (thing.def.graphicData.asymmetricLink.drawDoorBorderWest != null)
			{
				DrawBorder(IntVec3.West, thing.def.graphicData.asymmetricLink.drawDoorBorderWest);
			}
		}
		void DrawBorder(IntVec3 dir, AsymmetricLinkData.BorderData border)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			IntVec3 c = cell + dir;
			if (c.InBounds(map) && c.GetDoor(map) != null)
			{
				Vector3 center = thing.DrawPos + border.offset + Altitudes.AltIncVect;
				Printer_Plane.PrintPlane(layer, center, border.size, border.Mat, extraRotation);
			}
		}
	}

	public override bool ShouldLinkWith(IntVec3 c, Thing parent)
	{
		if (base.ShouldLinkWith(c, parent))
		{
			return true;
		}
		if (!c.InBounds(parent.Map))
		{
			return (parent.def.graphicData.linkFlags & LinkFlags.MapEdge) != 0;
		}
		if (parent.def.graphicData.asymmetricLink != null)
		{
			if ((parent.Map.linkGrid.LinkFlagsAt(c) & parent.def.graphicData.asymmetricLink.linkFlags) != 0)
			{
				return true;
			}
			if (parent.def.graphicData.asymmetricLink.linkToDoors && c.GetDoor(parent.Map) != null)
			{
				return true;
			}
		}
		return false;
	}
}
