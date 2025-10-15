using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class ExpandableLandmarksUtility
{
	private static float transitionPct;

	private static List<PlanetTile> cachedLandmarksToShow = new List<PlanetTile>();

	private const float LandmarkIconSize = 30f;

	private const float TransitionSpeed = 3f;

	private static readonly Color HighlightedColor = ExpandableWorldObjectsUtility.HighlightedColor;

	private static Texture2D landmarkTex;

	private static Texture2D LandmarkTex => landmarkTex ?? (landmarkTex = ContentFinder<Texture2D>.Get("World/WorldObjects/Expanding/LandmarkExpanding"));

	private static float TransitionPct
	{
		get
		{
			if (!Find.PlaySettings.showExpandingLandmarks)
			{
				return 0f;
			}
			return transitionPct;
		}
	}

	private static List<PlanetTile> LandmarksToShow
	{
		get
		{
			if (!cachedLandmarksToShow.Empty())
			{
				return cachedLandmarksToShow;
			}
			foreach (PlanetTile key in Find.World.landmarks.landmarks.Keys)
			{
				if (!Find.WorldObjects.AnyWorldObjectAt(key))
				{
					cachedLandmarksToShow.Add(key);
				}
			}
			return cachedLandmarksToShow;
		}
	}

	public static void ResetStaticData()
	{
		landmarkTex = null;
		cachedLandmarksToShow.Clear();
	}

	public static void ExpandableLandmarksUpdate()
	{
		float num = Time.deltaTime * 3f;
		if ((int)Find.WorldCameraDriver.CurrentZoom <= 0)
		{
			transitionPct -= num;
		}
		else
		{
			transitionPct += num;
		}
		transitionPct = Mathf.Clamp01(transitionPct);
	}

	public static void ExpandableLandmarksOnGUI()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		foreach (PlanetTile item in LandmarksToShow)
		{
			try
			{
				bool flag = IsHighlighted(item);
				if ((TransitionPct != 0f || flag) && !HiddenBehindTerrainNow(item))
				{
					Color white = Color.white;
					white.a = (flag ? 1f : transitionPct);
					GUI.color = white;
					Rect rect = ExpandedIconScreenRect(item);
					Material material = null;
					if (flag)
					{
						material = GetMaterial(white);
					}
					Widgets.DrawTextureRotated(rect, (Texture)(object)LandmarkTex, 0f, material);
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Error while drawing landmark at {item.ToStringSafe()}: {arg}");
			}
		}
		GUI.color = Color.white;
	}

	private static bool IsHighlighted(PlanetTile tile)
	{
		if (!Find.WindowStack.TryGetWindow<Dialog_WorldSearch>(out var window))
		{
			return false;
		}
		if (window.Highlighted != null && window.Highlighted.tile == tile)
		{
			return true;
		}
		if (window.CommonSearchWidget.filter.Text.Length >= 1)
		{
			return window.IsListed(tile);
		}
		return false;
	}

	private static Material GetMaterial(Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		MaterialRequest req = new MaterialRequest(null, ShaderDatabase.ExpandingIconUI, color);
		req.needsMainTex = false;
		req.colorTwo = HighlightedColor;
		return MaterialPool.MatFrom(req);
	}

	private static Rect ExpandedIconScreenRect(PlanetTile tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = GenWorldUI.WorldToUIPosition(DrawPos(tile));
		bool num = tile.Layer != Find.WorldSelector.SelectedLayer;
		float num2 = 30f;
		if (num)
		{
			num2 /= 2f;
		}
		return new Rect(val.x - num2 / 2f, val.y - num2 / 2f, num2, num2);
	}

	private static bool HiddenBehindTerrainNow(PlanetTile tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldRendererUtility.HiddenBehindTerrainNow(DrawPos(tile)))
		{
			return tile.Layer != PlanetLayer.Selected;
		}
		return true;
	}

	private static Vector3 DrawPos(PlanetTile tile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return tile.Layer.Origin + Find.WorldGrid.GetTileCenter(tile);
	}

	public static void Notify_WorldObjectsChanged()
	{
		cachedLandmarksToShow.Clear();
	}
}
