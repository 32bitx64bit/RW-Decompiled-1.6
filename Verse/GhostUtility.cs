using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public static class GhostUtility
{
	private static Dictionary<int, Graphic> ghostGraphics = new Dictionary<int, Graphic>();

	public static Graphic GhostGraphicFor(Graphic baseGraphic, ThingDef thingDef, Color ghostCol, ThingDef stuff = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (thingDef.useSameGraphicForGhost)
		{
			return baseGraphic;
		}
		int seed = 0;
		seed = Gen.HashCombine(seed, baseGraphic);
		seed = Gen.HashCombine(seed, thingDef);
		seed = Gen.HashCombineStruct<Color>(seed, ghostCol);
		seed = Gen.HashCombine(seed, stuff);
		if (!ghostGraphics.TryGetValue(seed, out var value))
		{
			if (thingDef.graphicData.Linked || (thingDef.IsDoor && !thingDef.building.isSupportDoor))
			{
				value = GraphicDatabase.Get<Graphic_Single>(thingDef.uiIconPath, ShaderTypeDefOf.EdgeDetect.Shader, thingDef.graphicData.drawSize, ghostCol);
			}
			else
			{
				if (thingDef.useBlueprintGraphicAsGhost)
				{
					baseGraphic = thingDef.blueprintDef.graphic;
				}
				else if (baseGraphic == null)
				{
					baseGraphic = thingDef.graphic;
				}
				GraphicData graphicData = null;
				if (baseGraphic.data != null)
				{
					graphicData = new GraphicData();
					graphicData.CopyFrom(baseGraphic.data);
					graphicData.shadowData = null;
				}
				string path = baseGraphic.path;
				value = ((!(baseGraphic is Graphic_Appearances graphic_Appearances) || stuff == null) ? GraphicDatabase.Get(baseGraphic.GetType(), path, ShaderTypeDefOf.EdgeDetect.Shader, baseGraphic.drawSize, ghostCol, Color.white, graphicData, null) : GraphicDatabase.Get<Graphic_Single>(graphic_Appearances.SubGraphicFor(stuff).path, ShaderTypeDefOf.EdgeDetect.Shader, thingDef.graphicData.drawSize, ghostCol, Color.white, graphicData));
			}
			ghostGraphics.Add(seed, value);
		}
		return value;
	}
}
