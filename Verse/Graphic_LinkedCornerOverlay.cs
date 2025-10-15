using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_LinkedCornerOverlay : Graphic_Linked
{
	public Graphic_Random overlayGraphic;

	public Graphic_LinkedCornerOverlay(Graphic subGraphic)
		: base(subGraphic)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.subGraphic = subGraphic;
		data = subGraphic.data;
		overlayGraphic = GraphicDatabase.Get<Graphic_Random>(data.cornerOverlayPath, ShaderDatabase.Transparent, drawSize, color) as Graphic_Random;
	}

	public override void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		base.Print(layer, thing, extraRotation);
		IntVec3 position = thing.Position;
		if (ShouldLinkWith(position + IntVec3.East, thing) && ShouldLinkWith(position + IntVec3.North, thing) && ShouldLinkWith(position + IntVec3.NorthEast, thing))
		{
			Rand.PushState(thing.thingIDNumber * 9);
			Material material = overlayGraphic.MatSingleFor(thing);
			float rot = Rand.Range(0f, 360f);
			bool @bool = Rand.Bool;
			Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, @bool, vertexColors: false, out material, out var uvs, out var _);
			Printer_Plane.PrintPlane(layer, thing.TrueCenter() + new Vector3(0.5f, 0.1f, 0.5f), Vector2.op_Implicit(Vector3.one), material, rot, flipUv: false, uvs);
			Rand.PopState();
		}
	}
}
