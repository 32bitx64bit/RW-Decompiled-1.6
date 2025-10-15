using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class TutorUtility
{
	public static bool BuildingOrBlueprintOrFrameCenterExists(IntVec3 c, Map map, ThingDef buildingDef)
	{
		List<Thing> thingList = c.GetThingList(map);
		for (int i = 0; i < thingList.Count; i++)
		{
			Thing thing = thingList[i];
			if (!(thing.Position != c))
			{
				if (thing.def == buildingDef)
				{
					return true;
				}
				if (thing.def.entityDefToBuild == buildingDef)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static CellRect FindUsableRect(int width, int height, Map map, float minFertility = 0f, bool noItems = false)
	{
		IntVec3 center = map.Center;
		float num = 1f;
		CellRect cellRect;
		while (true)
		{
			cellRect = CellRect.CenteredOn(center + new IntVec3((int)Rand.Range(0f - num, num), 0, (int)Rand.Range(0f - num, num)), width / 2);
			cellRect.Width = width;
			cellRect.Height = height;
			cellRect = cellRect.ExpandedBy(1);
			bool flag = true;
			foreach (IntVec3 item in cellRect)
			{
				if (item.Fogged(map) || !item.Walkable(map) || !item.GetAffordances(map).Contains(TerrainAffordanceDefOf.Heavy) || item.GetFertility(map) < minFertility || item.GetZone(map) != null || ContainsBlockingThing(item, map, noItems) || item.InNoBuildEdgeArea(map) || item.InNoZoneEdgeArea(map))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			num += 0.25f;
		}
		return cellRect.ContractedBy(1);
	}

	private static bool ContainsBlockingThing(IntVec3 cell, Map map, bool noItems)
	{
		List<Thing> thingList = cell.GetThingList(map);
		for (int i = 0; i < thingList.Count; i++)
		{
			if (thingList[i].def.category == ThingCategory.Building)
			{
				return true;
			}
			if (thingList[i] is Blueprint)
			{
				return true;
			}
			if (noItems && thingList[i].def.category == ThingCategory.Item)
			{
				return true;
			}
		}
		return false;
	}

	public static void DrawLabelOnThingOnGUI(Thing t, string label)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = (t.DrawPos + new Vector3(0f, 0f, 0.5f)).MapToUIPosition();
		Vector2 val2 = Text.CalcSize(label);
		Rect val3 = new Rect(val.x - val2.x / 2f, val.y - val2.y / 2f, val2.x, val2.y);
		GUI.DrawTexture(val3, (Texture)(object)TexUI.GrayTextBG);
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(val3, label);
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawLabelOnGUI(Vector3 mapPos, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = mapPos.MapToUIPosition();
		Vector2 val2 = Text.CalcSize(label);
		Rect val3 = new Rect(val.x - val2.x / 2f, val.y - val2.y / 2f, val2.x, val2.y);
		GUI.DrawTexture(val3, (Texture)(object)TexUI.GrayTextBG);
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(val3, label);
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawCellRectOnGUI(CellRect cellRect, string label = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (label != null)
		{
			DrawLabelOnGUI(cellRect.CenterVector3, label);
		}
	}

	public static void DrawCellRectUpdate(CellRect cellRect)
	{
		foreach (IntVec3 item in cellRect)
		{
			CellRenderer.RenderCell(item);
		}
	}

	public static void DoModalDialogIfNotKnown(ConceptDef conc, params string[] input)
	{
		if (!PlayerKnowledgeDatabase.IsComplete(conc))
		{
			DoModalDialogIfNotKnownInner(conc, string.Format(conc.HelpTextAdjusted, input));
		}
	}

	public static void DoModalDialogWithArgsIfNotKnown(ConceptDef conc, params NamedArgument[] args)
	{
		if (!PlayerKnowledgeDatabase.IsComplete(conc))
		{
			DoModalDialogIfNotKnownInner(conc, conc.HelpTextAdjusted.Formatted(args));
		}
	}

	public static void DoModalDialogWithArgsIfNotKnown(ConceptDef conc, string buttonAText, Action buttonAAction, string buttonBText = null, Action buttonBAction = null, params NamedArgument[] args)
	{
		if (!PlayerKnowledgeDatabase.IsComplete(conc))
		{
			Find.WindowStack.Add(new Dialog_MessageBox(conc.HelpTextAdjusted.Formatted(args), buttonAText, buttonAAction, buttonBText, buttonBAction));
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(conc, KnowledgeAmount.Total);
		}
	}

	private static void DoModalDialogIfNotKnownInner(ConceptDef conc, string msg)
	{
		Find.WindowStack.Add(new Dialog_MessageBox(msg));
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(conc, KnowledgeAmount.Total);
	}

	public static bool EventCellsMatchExactly(EventPack ep, List<IntVec3> targetCells)
	{
		if (ep.Cell.IsValid)
		{
			if (targetCells.Count == 1)
			{
				return ep.Cell == targetCells[0];
			}
			return false;
		}
		if (ep.Cells == null)
		{
			return false;
		}
		int num = 0;
		foreach (IntVec3 cell in ep.Cells)
		{
			if (!targetCells.Contains(cell))
			{
				return false;
			}
			num++;
		}
		return num == targetCells.Count;
	}

	public static bool EventCellsAreWithin(EventPack ep, List<IntVec3> targetCells)
	{
		if (ep.Cell.IsValid)
		{
			return targetCells.Contains(ep.Cell);
		}
		if (ep.Cells != null)
		{
			return !ep.Cells.Any((IntVec3 c) => !targetCells.Contains(c));
		}
		return false;
	}
}
