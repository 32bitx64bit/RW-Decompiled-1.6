using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class GenWorldUI
{
	private static List<Caravan> clickedCaravans = new List<Caravan>();

	private static List<WorldObject> clickedDynamicallyDrawnObjects = new List<WorldObject>();

	public static float CaravanDirectClickRadius => 0.35f * Find.WorldGrid.AverageTileSize;

	private static float CaravanWideClickRadius => 0.75f * Find.WorldGrid.AverageTileSize;

	private static float DynamicallyDrawnObjectDirectClickRadius => 0.35f * Find.WorldGrid.AverageTileSize;

	public static List<WorldObject> WorldObjectsUnderMouse(Vector2 mousePos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		List<WorldObject> list = new List<WorldObject>();
		ExpandableWorldObjectsUtility.GetExpandedWorldObjectUnderMouse(mousePos, list);
		float caravanDirectClickRadius = CaravanDirectClickRadius;
		clickedCaravans.Clear();
		List<Caravan> caravans = Find.WorldObjects.Caravans;
		for (int i = 0; i < caravans.Count; i++)
		{
			Caravan caravan = caravans[i];
			if (caravan.DistanceToMouse(mousePos) < caravanDirectClickRadius)
			{
				clickedCaravans.Add(caravan);
			}
		}
		clickedCaravans.SortBy((Caravan x) => x.DistanceToMouse(mousePos));
		for (int j = 0; j < clickedCaravans.Count; j++)
		{
			if (!list.Contains(clickedCaravans[j]))
			{
				list.Add(clickedCaravans[j]);
			}
		}
		float dynamicallyDrawnObjectDirectClickRadius = DynamicallyDrawnObjectDirectClickRadius;
		clickedDynamicallyDrawnObjects.Clear();
		List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
		for (int k = 0; k < allWorldObjects.Count; k++)
		{
			WorldObject worldObject = allWorldObjects[k];
			if (worldObject.def.useDynamicDrawer && worldObject.DistanceToMouse(mousePos) < dynamicallyDrawnObjectDirectClickRadius)
			{
				clickedDynamicallyDrawnObjects.Add(worldObject);
			}
		}
		clickedDynamicallyDrawnObjects.SortBy((WorldObject x) => x.DistanceToMouse(mousePos));
		for (int l = 0; l < clickedDynamicallyDrawnObjects.Count; l++)
		{
			if (!list.Contains(clickedDynamicallyDrawnObjects[l]))
			{
				list.Add(clickedDynamicallyDrawnObjects[l]);
			}
		}
		PlanetTile planetTile = GenWorld.TileAt(mousePos);
		List<WorldObject> allWorldObjects2 = Find.WorldObjects.AllWorldObjects;
		for (int m = 0; m < allWorldObjects2.Count; m++)
		{
			if (allWorldObjects2[m].Tile == planetTile && !list.Contains(allWorldObjects2[m]))
			{
				list.Add(allWorldObjects2[m]);
			}
		}
		float caravanWideClickRadius = CaravanWideClickRadius;
		clickedCaravans.Clear();
		List<Caravan> caravans2 = Find.WorldObjects.Caravans;
		for (int n = 0; n < caravans2.Count; n++)
		{
			Caravan caravan2 = caravans2[n];
			if (caravan2.DistanceToMouse(mousePos) < caravanWideClickRadius)
			{
				clickedCaravans.Add(caravan2);
			}
		}
		clickedCaravans.SortBy((Caravan x) => x.DistanceToMouse(mousePos));
		for (int num = 0; num < clickedCaravans.Count; num++)
		{
			if (!list.Contains(clickedCaravans[num]))
			{
				list.Add(clickedCaravans[num]);
			}
		}
		clickedCaravans.Clear();
		return list;
	}

	public static Vector2 WorldToUIPosition(Vector3 worldLoc)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Find.WorldCamera.WorldToScreenPoint(worldLoc) / Prefs.UIScale;
		return new Vector2(val.x, (float)UI.screenHeight - val.y);
	}

	public static float CurUITileSize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)Find.WorldCamera).transform;
		Vector3 localPosition = transform.localPosition;
		Quaternion rotation = transform.rotation;
		transform.localPosition = new Vector3(0f, 0f, ((Vector3)(ref localPosition)).magnitude);
		transform.rotation = Quaternion.identity;
		float x = (WorldToUIPosition(new Vector3((0f - Find.WorldGrid.AverageTileSize) / 2f, 0f, 100f)) - WorldToUIPosition(new Vector3(Find.WorldGrid.AverageTileSize / 2f, 0f, 100f))).x;
		transform.localPosition = localPosition;
		transform.rotation = rotation;
		return x;
	}
}
