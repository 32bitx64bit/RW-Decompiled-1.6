using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GenWorld
{
	private static PlanetTile cachedTile_noSnap = PlanetTile.Invalid;

	private static int cachedFrame_noSnap = -1;

	private static PlanetTile cachedTile_snap = PlanetTile.Invalid;

	private static int cachedFrame_snap = -1;

	private static PlanetLayer cachedLayer;

	public const float MaxRayLength = 1500f;

	private static readonly List<WorldObject> tmpWorldObjectsUnderMouse = new List<WorldObject>();

	public static PlanetTile MouseTile(bool snapToExpandableWorldObjects = false)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		PlanetLayer selected = PlanetLayer.Selected;
		if (snapToExpandableWorldObjects)
		{
			if (cachedFrame_snap == Time.frameCount && cachedLayer == selected)
			{
				return cachedTile_snap;
			}
			cachedTile_snap = TileAt(UI.MousePositionOnUI, snapToExpandableWorldObjects: true);
			cachedFrame_snap = Time.frameCount;
			cachedLayer = selected;
			return cachedTile_snap;
		}
		if (cachedFrame_noSnap == Time.frameCount && cachedLayer == selected)
		{
			return cachedTile_noSnap;
		}
		cachedTile_noSnap = TileAt(UI.MousePositionOnUI);
		cachedFrame_noSnap = Time.frameCount;
		cachedLayer = selected;
		return cachedTile_noSnap;
	}

	public static PlanetTile TileAt(Vector2 clickPos, bool snapToExpandableWorldObjects = false)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Camera worldCamera = Find.WorldCamera;
		if (!((Component)worldCamera).gameObject.activeInHierarchy)
		{
			return PlanetTile.Invalid;
		}
		if (snapToExpandableWorldObjects)
		{
			ExpandableWorldObjectsUtility.GetExpandedWorldObjectUnderMouse(UI.MousePositionOnUI, tmpWorldObjectsUnderMouse);
			if (tmpWorldObjectsUnderMouse.Any())
			{
				PlanetTile tile = tmpWorldObjectsUnderMouse[0].Tile;
				tmpWorldObjectsUnderMouse.Clear();
				return tile;
			}
		}
		Ray val = worldCamera.ScreenPointToRay(Vector2.op_Implicit(clickPos * Prefs.UIScale));
		int worldLayerMask = WorldCameraManager.WorldLayerMask;
		WorldTerrainColliderManager.EnsureRaycastCollidersUpdated();
		RaycastHit hit = default(RaycastHit);
		if (Physics.Raycast(val, ref hit, 1500f, worldLayerMask))
		{
			return Find.World.renderer.GetTileFromRayHit(hit);
		}
		return PlanetTile.Invalid;
	}

	public static string GetPollutionDescription(float pollution)
	{
		if (pollution <= 0f)
		{
			return "TilePollutionNone".Translate();
		}
		if (pollution <= 0.333f)
		{
			return "TilePollutionLight".Translate();
		}
		if (pollution <= 0.666f)
		{
			return "TilePollutionModerate".Translate();
		}
		return "TilePollutionExtreme".Translate();
	}
}
