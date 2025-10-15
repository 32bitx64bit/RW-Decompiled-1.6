using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class WorldObjectSelectionUtility
{
	public static IEnumerable<WorldObject> MultiSelectableWorldObjectsInScreenRectDistinct(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		List<WorldObject> allObjects = Find.WorldObjects.AllWorldObjects;
		for (int i = 0; i < allObjects.Count; i++)
		{
			if (allObjects[i].NeverMultiSelect || allObjects[i].HiddenBehindTerrainNow())
			{
				continue;
			}
			if (ExpandableWorldObjectsUtility.IsExpanded(allObjects[i]))
			{
				if (((Rect)(ref rect)).Overlaps(ExpandableWorldObjectsUtility.ExpandedIconScreenRect(allObjects[i])))
				{
					yield return allObjects[i];
				}
			}
			else if (((Rect)(ref rect)).Contains(allObjects[i].ScreenPos()))
			{
				yield return allObjects[i];
			}
		}
	}

	public static bool HiddenBehindTerrainNow(this WorldObject o)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldRendererUtility.HiddenBehindTerrainNow(o.DrawPos))
		{
			if (!o.VisibleInBackground)
			{
				return o.Tile.Layer != PlanetLayer.Selected;
			}
			return false;
		}
		return true;
	}

	public static Vector2 ScreenPos(this WorldObject o)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GenWorldUI.WorldToUIPosition(o.DrawPos);
	}

	public static bool VisibleToCameraNow(this WorldObject o)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldRendererUtility.WorldSelected)
		{
			return false;
		}
		if (o.HiddenBehindTerrainNow())
		{
			return false;
		}
		Vector2 val = o.ScreenPos();
		Rect val2 = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
		return ((Rect)(ref val2)).Contains(val);
	}

	public static float DistanceToMouse(this WorldObject o, Vector2 mousePos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Ray val = Find.WorldCamera.ScreenPointToRay(Vector2.op_Implicit(mousePos * Prefs.UIScale));
		int worldLayerMask = WorldCameraManager.WorldLayerMask;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, ref val2, 1500f, worldLayerMask))
		{
			return Vector3.Distance(((RaycastHit)(ref val2)).point, o.DrawPos);
		}
		Vector3 val3 = Vector3.Cross(((Ray)(ref val)).direction, o.DrawPos - ((Ray)(ref val)).origin);
		return ((Vector3)(ref val3)).magnitude;
	}
}
