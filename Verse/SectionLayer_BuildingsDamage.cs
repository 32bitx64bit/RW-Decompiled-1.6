using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class SectionLayer_BuildingsDamage : SectionLayer
{
	private static List<Vector2> scratches = new List<Vector2>();

	public SectionLayer_BuildingsDamage(Section section)
		: base(section)
	{
		relevantChangeTypes = (ulong)MapMeshFlagDefOf.BuildingsDamage | (ulong)MapMeshFlagDefOf.Buildings;
	}

	public override void Regenerate()
	{
		ClearSubMeshes(MeshParts.All);
		foreach (IntVec3 item in section.CellRect)
		{
			List<Thing> list = base.Map.thingGrid.ThingsListAt(item);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] is Building building && building.def.useHitPoints && building.HitPoints < building.MaxHitPoints && building.def.drawDamagedOverlay && building.Position.x == item.x && building.Position.z == item.z)
				{
					PrintDamageVisualsFrom(building);
				}
			}
		}
		FinalizeMesh(MeshParts.All);
	}

	private void PrintDamageVisualsFrom(Building b)
	{
		if (b.def.graphicData == null || b.def.graphicData.damageData == null || b.def.graphicData.damageData.enabled)
		{
			PrintScratches(b);
			PrintCornersAndEdges(b);
		}
	}

	private void PrintScratches(Building b)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		List<DamageOverlay> overlays = BuildingsDamageSectionLayerUtility.GetOverlays(b);
		for (int i = 0; i < overlays.Count; i++)
		{
			if (overlays[i] == DamageOverlay.Scratch)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return;
		}
		Rect rect = BuildingsDamageSectionLayerUtility.GetDamageRect(b);
		float num2 = Mathf.Min(0.5f * Mathf.Min(((Rect)(ref rect)).width, ((Rect)(ref rect)).height), 1f);
		rect = rect.ContractedBy(num2 / 2f);
		if (((Rect)(ref rect)).width <= 0f || ((Rect)(ref rect)).height <= 0f)
		{
			return;
		}
		float minDist = Mathf.Max(((Rect)(ref rect)).width, ((Rect)(ref rect)).height) * 0.7f;
		scratches.Clear();
		Rand.PushState();
		Rand.Seed = b.thingIDNumber * 3697;
		for (int j = 0; j < num; j++)
		{
			AddScratch(((Rect)(ref rect)).width, ((Rect)(ref rect)).height, ref minDist);
		}
		Rand.PopState();
		float damageTexturesAltitude = GetDamageTexturesAltitude(b);
		IList<Material> scratchMats = BuildingsDamageSectionLayerUtility.GetScratchMats(b);
		Rand.PushState();
		Rand.Seed = b.thingIDNumber * 7;
		Vector3 center = default(Vector3);
		for (int k = 0; k < scratches.Count; k++)
		{
			float x = scratches[k].x;
			float y = scratches[k].y;
			float rot = Rand.Range(0f, 360f);
			float num3 = num2;
			if (((Rect)(ref rect)).width > 0.95f && ((Rect)(ref rect)).height > 0.95f)
			{
				num3 *= Rand.Range(0.85f, 1f);
			}
			((Vector3)(ref center))._002Ector(((Rect)(ref rect)).xMin + x, damageTexturesAltitude, ((Rect)(ref rect)).yMin + y);
			Material material = scratchMats.RandomElement();
			Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out var uvs, out var _);
			Printer_Plane.PrintPlane(this, center, new Vector2(num3, num3), material, rot, flipUv: false, uvs, null, 0f);
		}
		Rand.PopState();
	}

	private void AddScratch(float rectWidth, float rectHeight, ref float minDist)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		float num = 0f;
		float num2 = 0f;
		while (!flag)
		{
			for (int i = 0; i < 5; i++)
			{
				num = Rand.Value * rectWidth;
				num2 = Rand.Value * rectHeight;
				float num3 = float.MaxValue;
				for (int j = 0; j < scratches.Count; j++)
				{
					float num4 = (num - scratches[j].x) * (num - scratches[j].x) + (num2 - scratches[j].y) * (num2 - scratches[j].y);
					if (num4 < num3)
					{
						num3 = num4;
					}
				}
				if (num3 >= minDist * minDist)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				minDist *= 0.85f;
				if (minDist < 0.001f)
				{
					break;
				}
			}
		}
		if (flag)
		{
			scratches.Add(new Vector2(num, num2));
		}
	}

	private void PrintCornersAndEdges(Building b)
	{
		Rand.PushState();
		Rand.Seed = b.thingIDNumber * 3;
		if (BuildingsDamageSectionLayerUtility.UsesLinkableCornersAndEdges(b))
		{
			DrawLinkableCornersAndEdges(b);
		}
		else
		{
			DrawFullThingCorners(b);
		}
		Rand.PopState();
	}

	private void DrawLinkableCornersAndEdges(Building b)
	{
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		if (b.def.graphicData == null)
		{
			return;
		}
		DamageGraphicData damageData = b.def.graphicData.damageData;
		if (damageData == null)
		{
			return;
		}
		float damageTexturesAltitude = GetDamageTexturesAltitude(b);
		List<DamageOverlay> overlays = BuildingsDamageSectionLayerUtility.GetOverlays(b);
		IntVec3 position = b.Position;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)position.x + 0.5f, damageTexturesAltitude, (float)position.z + 0.5f);
		float num = Rand.Range(0.4f, 0.6f);
		float num2 = Rand.Range(0.4f, 0.6f);
		float num3 = Rand.Range(0.4f, 0.6f);
		float num4 = Rand.Range(0.4f, 0.6f);
		Vector2[] uvs = null;
		for (int i = 0; i < overlays.Count; i++)
		{
			Color32 vertexColor;
			switch (overlays[i])
			{
			case DamageOverlay.TopEdge:
			{
				Material material = damageData.edgeTopMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val + new Vector3(num, 0f, 0f), Vector2.one, material, 0f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.RightEdge:
			{
				Material material = damageData.edgeRightMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val + new Vector3(0f, 0f, num2), Vector2.one, material, 90f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.BotEdge:
			{
				Material material = damageData.edgeBotMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val + new Vector3(num3, 0f, 0f), Vector2.one, material, 180f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.LeftEdge:
			{
				Material material = damageData.edgeLeftMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val + new Vector3(0f, 0f, num4), Vector2.one, material, 270f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.TopLeftCorner:
			{
				Material material = damageData.cornerTLMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val, Vector2.one, material, 0f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.TopRightCorner:
			{
				Material material = damageData.cornerTRMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val, Vector2.one, material, 90f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.BotRightCorner:
			{
				Material material = damageData.cornerBRMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val, Vector2.one, material, 180f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.BotLeftCorner:
			{
				Material material = damageData.cornerBLMat;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, val, Vector2.one, material, 270f, flipUv: false, uvs, null, 0f);
				break;
			}
			}
		}
	}

	private void DrawFullThingCorners(Building b)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		if (b.def.graphicData?.damageData == null)
		{
			return;
		}
		Rect damageRect = BuildingsDamageSectionLayerUtility.GetDamageRect(b);
		float damageTexturesAltitude = GetDamageTexturesAltitude(b);
		float num = Mathf.Min(Mathf.Min(((Rect)(ref damageRect)).width, ((Rect)(ref damageRect)).height), 1.5f);
		BuildingsDamageSectionLayerUtility.GetCornerMats(out var topLeft, out var topRight, out var botRight, out var botLeft, b);
		float num2 = num * Rand.Range(0.9f, 1f);
		float num3 = num * Rand.Range(0.9f, 1f);
		float num4 = num * Rand.Range(0.9f, 1f);
		float num5 = num * Rand.Range(0.9f, 1f);
		Vector2[] uvs = null;
		List<DamageOverlay> overlays = BuildingsDamageSectionLayerUtility.GetOverlays(b);
		Rect val4 = default(Rect);
		Rect val3 = default(Rect);
		Rect val2 = default(Rect);
		Rect val = default(Rect);
		for (int i = 0; i < overlays.Count; i++)
		{
			Color32 vertexColor;
			switch (overlays[i])
			{
			case DamageOverlay.TopLeftCorner:
			{
				((Rect)(ref val4))._002Ector(((Rect)(ref damageRect)).xMin, ((Rect)(ref damageRect)).yMax - num2, num2, num2);
				Material material = topLeft;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, new Vector3(((Rect)(ref val4)).center.x, damageTexturesAltitude, ((Rect)(ref val4)).center.y), ((Rect)(ref val4)).size, material, 0f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.TopRightCorner:
			{
				((Rect)(ref val3))._002Ector(((Rect)(ref damageRect)).xMax - num3, ((Rect)(ref damageRect)).yMax - num3, num3, num3);
				Material material = topRight;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, new Vector3(((Rect)(ref val3)).center.x, damageTexturesAltitude, ((Rect)(ref val3)).center.y), ((Rect)(ref val3)).size, material, 90f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.BotRightCorner:
			{
				((Rect)(ref val2))._002Ector(((Rect)(ref damageRect)).xMax - num4, ((Rect)(ref damageRect)).yMin, num4, num4);
				Material material = botRight;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, new Vector3(((Rect)(ref val2)).center.x, damageTexturesAltitude, ((Rect)(ref val2)).center.y), ((Rect)(ref val2)).size, material, 180f, flipUv: false, uvs, null, 0f);
				break;
			}
			case DamageOverlay.BotLeftCorner:
			{
				((Rect)(ref val))._002Ector(((Rect)(ref damageRect)).xMin, ((Rect)(ref damageRect)).yMin, num5, num5);
				Material material = botLeft;
				Graphic.TryGetTextureAtlasReplacementInfo(material, TextureAtlasGroup.Building, flipUv: false, vertexColors: false, out material, out uvs, out vertexColor);
				Printer_Plane.PrintPlane(this, new Vector3(((Rect)(ref val)).center.x, damageTexturesAltitude, ((Rect)(ref val)).center.y), ((Rect)(ref val)).size, material, 270f, flipUv: false, uvs, null, 0f);
				break;
			}
			}
		}
	}

	private float GetDamageTexturesAltitude(Building b)
	{
		return b.def.Altitude + 15f / 82f;
	}
}
