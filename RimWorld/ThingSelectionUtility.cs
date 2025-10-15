using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class ThingSelectionUtility
{
	private static HashSet<Thing> yieldedThings = new HashSet<Thing>();

	private static readonly HashSet<Plan> yieldedPlans = new HashSet<Plan>();

	private static readonly HashSet<Zone> yieldedZones = new HashSet<Zone>();

	private static readonly List<Pawn> tmpColonists = new List<Pawn>();

	public static bool SelectableByMapClick(Thing t)
	{
		if (!t.def.selectable)
		{
			return false;
		}
		if (t is Pawn pawn && pawn.IsHiddenFromPlayer())
		{
			return false;
		}
		Thing spawnedParentOrMe = t.SpawnedParentOrMe;
		if (spawnedParentOrMe == null)
		{
			return false;
		}
		if (spawnedParentOrMe.def.size.x == 1 && spawnedParentOrMe.def.size.z == 1)
		{
			return !spawnedParentOrMe.Position.Fogged(spawnedParentOrMe.Map);
		}
		foreach (IntVec3 item in spawnedParentOrMe.OccupiedRect())
		{
			if (!item.Fogged(spawnedParentOrMe.Map))
			{
				return true;
			}
		}
		return false;
	}

	public static bool SelectableByHotkey(Thing t)
	{
		if (t.def.selectable)
		{
			return t.Spawned;
		}
		return false;
	}

	public static IEnumerable<Thing> MultiSelectableThingsInScreenRectDistinct(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CellRect mapRect = GetMapRect(rect);
		yieldedThings.Clear();
		try
		{
			foreach (IntVec3 item in mapRect)
			{
				if (!item.InBounds(Find.CurrentMap))
				{
					continue;
				}
				List<Thing> cellThings = Find.CurrentMap.thingGrid.ThingsListAt(item);
				if (cellThings == null)
				{
					continue;
				}
				for (int i = 0; i < cellThings.Count; i++)
				{
					Thing t2 = cellThings[i];
					if (SelectableByMapClick(t2) && !t2.def.neverMultiSelect && !yieldedThings.Contains(t2))
					{
						yield return t2;
						yieldedThings.Add(t2);
					}
				}
			}
			Rect rectInWorldSpace = GetRectInWorldSpace(rect);
			Rect val2 = default(Rect);
			foreach (IntVec3 edgeCell in mapRect.ExpandedBy(1).EdgeCells)
			{
				if (!edgeCell.InBounds(Find.CurrentMap) || edgeCell.GetItemCount(Find.CurrentMap) <= 1)
				{
					continue;
				}
				foreach (Thing t2 in Find.CurrentMap.thingGrid.ThingsAt(edgeCell))
				{
					if (t2.def.category == ThingCategory.Item && SelectableByMapClick(t2) && !t2.def.neverMultiSelect && !yieldedThings.Contains(t2))
					{
						Vector3 val = t2.TrueCenter();
						((Rect)(ref val2))._002Ector(val.x - 0.5f, val.z - 0.5f, 1f, 1f);
						if (((Rect)(ref val2)).Overlaps(rectInWorldSpace))
						{
							yield return t2;
							yieldedThings.Add(t2);
						}
					}
				}
			}
		}
		finally
		{
			yieldedThings.Clear();
		}
	}

	private static Rect GetRectInWorldSpace(Rect rect)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Vector2 screenLoc = default(Vector2);
		((Vector2)(ref screenLoc))._002Ector(((Rect)(ref rect)).x, (float)UI.screenHeight - ((Rect)(ref rect)).y);
		Vector2 screenLoc2 = new Vector2(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, (float)UI.screenHeight - (((Rect)(ref rect)).y + ((Rect)(ref rect)).height));
		Vector3 val = UI.UIToMapPosition(screenLoc);
		Vector3 val2 = UI.UIToMapPosition(screenLoc2);
		return new Rect(val.x, val2.z, val2.x - val.x, val.z - val2.z);
	}

	public static IEnumerable<Plan> MultiSelectablePlansInScreenRectDistinct(Rect rect, ColorDef match = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CellRect mapRect = GetMapRect(rect);
		yieldedPlans.Clear();
		try
		{
			foreach (IntVec3 item in mapRect)
			{
				if (item.InBounds(Find.CurrentMap))
				{
					Plan plan = item.GetPlan(Find.CurrentMap);
					if (plan != null && (match == null || plan.Color == match) && !yieldedPlans.Contains(plan))
					{
						yield return plan;
						yieldedPlans.Add(plan);
					}
				}
			}
		}
		finally
		{
			yieldedPlans.Clear();
		}
	}

	public static IEnumerable<Zone> MultiSelectableZonesInScreenRectDistinct(Rect rect, Zone matchType = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CellRect mapRect = GetMapRect(rect);
		yieldedZones.Clear();
		try
		{
			foreach (IntVec3 item in mapRect)
			{
				if (item.InBounds(Find.CurrentMap))
				{
					Zone zone = item.GetZone(Find.CurrentMap);
					if (zone != null && zone.IsMultiselectable && (matchType == null || !(zone.GetType() != matchType.GetType())) && !yieldedZones.Contains(zone))
					{
						yield return zone;
						yieldedZones.Add(zone);
					}
				}
			}
		}
		finally
		{
			yieldedZones.Clear();
		}
	}

	private static CellRect GetMapRect(Rect rect)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Vector2 screenLoc = default(Vector2);
		((Vector2)(ref screenLoc))._002Ector(((Rect)(ref rect)).x, (float)UI.screenHeight - ((Rect)(ref rect)).y);
		Vector2 screenLoc2 = new Vector2(((Rect)(ref rect)).x + ((Rect)(ref rect)).width, (float)UI.screenHeight - (((Rect)(ref rect)).y + ((Rect)(ref rect)).height));
		Vector3 val = UI.UIToMapPosition(screenLoc);
		Vector3 val2 = UI.UIToMapPosition(screenLoc2);
		CellRect result = default(CellRect);
		result.minX = Mathf.FloorToInt(val.x);
		result.minZ = Mathf.FloorToInt(val2.z);
		result.maxX = Mathf.FloorToInt(val2.x);
		result.maxZ = Mathf.FloorToInt(val.z);
		return result;
	}

	public static void SelectNextColonist()
	{
		tmpColonists.Clear();
		tmpColonists.AddRange(Find.ColonistBar.GetColonistsInOrder().Where(SelectableByHotkey));
		if (tmpColonists.Count == 0)
		{
			return;
		}
		bool worldSelected = WorldRendererUtility.WorldSelected;
		int num = -1;
		for (int num2 = tmpColonists.Count - 1; num2 >= 0; num2--)
		{
			if ((!worldSelected && Find.Selector.IsSelected(tmpColonists[num2])) || (worldSelected && tmpColonists[num2].IsCaravanMember() && Find.WorldSelector.IsSelected(tmpColonists[num2].GetCaravan())))
			{
				num = num2;
				break;
			}
		}
		if (num == -1)
		{
			CameraJumper.TryJumpAndSelect(tmpColonists[0]);
		}
		else
		{
			CameraJumper.TryJumpAndSelect(tmpColonists[(num + 1) % tmpColonists.Count]);
		}
		tmpColonists.Clear();
	}

	public static void SelectPreviousColonist()
	{
		tmpColonists.Clear();
		tmpColonists.AddRange(Find.ColonistBar.GetColonistsInOrder().Where(SelectableByHotkey));
		if (tmpColonists.Count == 0)
		{
			return;
		}
		bool worldSelected = WorldRendererUtility.WorldSelected;
		int num = -1;
		for (int i = 0; i < tmpColonists.Count; i++)
		{
			if ((!worldSelected && Find.Selector.IsSelected(tmpColonists[i])) || (worldSelected && tmpColonists[i].IsCaravanMember() && Find.WorldSelector.IsSelected(tmpColonists[i].GetCaravan())))
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			CameraJumper.TryJumpAndSelect(tmpColonists[tmpColonists.Count - 1]);
		}
		else
		{
			CameraJumper.TryJumpAndSelect(tmpColonists[GenMath.PositiveMod(num - 1, tmpColonists.Count)]);
		}
		tmpColonists.Clear();
	}
}
