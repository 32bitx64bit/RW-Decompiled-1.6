using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class ExpandableWorldObjectsUtility
{
	private static float transitionPct;

	private static float expandMoreTransitionPct;

	private static readonly List<WorldObject> tmpWorldObjects = new List<WorldObject>();

	private const float WorldObjectIconSize = 30f;

	private const float ExpandMoreWorldObjectIconSizeFactor = 1.35f;

	private const float TransitionSpeed = 3f;

	private const float ExpandMoreTransitionSpeed = 4f;

	private const float BackgroundSpaceOpacity = 0.3f;

	private static readonly Color HasMapColor = new Color(0.1f, 0.9f, 0.1f, 1f);

	public static readonly Color HighlightedColor = new Color(0.9f, 0.8f, 0.1f, 1f);

	public static float RawTransitionPct
	{
		get
		{
			if (!Find.PlaySettings.showImportantExpandingIcons)
			{
				return 0f;
			}
			return transitionPct;
		}
	}

	public static float ExpandMoreTransitionPct
	{
		get
		{
			if (!Find.PlaySettings.showImportantExpandingIcons)
			{
				return 0f;
			}
			return expandMoreTransitionPct;
		}
	}

	public static float TransitionPct(WorldObject wo)
	{
		if (HiddenByRules(wo))
		{
			return 0f;
		}
		if (wo.def.fullyExpandedInSpace && wo.Tile.LayerDef.isSpace && wo.Tile.Layer != Find.WorldSelector.SelectedLayer)
		{
			return 0.3f;
		}
		if (wo.def.fullyExpandedInSpace && wo.Tile.LayerDef.isSpace)
		{
			return 1f;
		}
		return transitionPct;
	}

	public static bool HiddenByRules(WorldObject wo)
	{
		if (wo.Tile.LayerDef.isSpace)
		{
			return false;
		}
		bool flag = IsNonPlayerSettlement(wo);
		if (!Find.PlaySettings.showBasesExpandingIcons && flag)
		{
			return true;
		}
		if (!Find.PlaySettings.showImportantExpandingIcons && !flag)
		{
			return true;
		}
		return false;
	}

	private static bool IsNonPlayerSettlement(WorldObject wo)
	{
		if (wo is Settlement settlement && !settlement.Faction.IsPlayer)
		{
			return !settlement.HasMap;
		}
		return false;
	}

	public static void ExpandableWorldObjectsUpdate()
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
		float num2 = Time.deltaTime * 4f;
		if ((int)Find.WorldCameraDriver.CurrentZoom <= 2)
		{
			expandMoreTransitionPct -= num2;
		}
		else
		{
			expandMoreTransitionPct += num2;
		}
		expandMoreTransitionPct = Mathf.Clamp01(expandMoreTransitionPct);
	}

	public static void ExpandableWorldObjectsOnGUI()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (!DebugViewSettings.drawWorldObjects || (int)Event.current.type != 7)
		{
			return;
		}
		tmpWorldObjects.Clear();
		tmpWorldObjects.AddRange(Find.WorldObjects.AllWorldObjects);
		SortByExpandingIconPriority(tmpWorldObjects);
		WorldTargeter worldTargeter = Find.WorldTargeter;
		List<WorldObject> worldObjectsUnderMouse = null;
		if (worldTargeter.IsTargeting)
		{
			worldObjectsUnderMouse = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
		}
		for (int i = 0; i < tmpWorldObjects.Count; i++)
		{
			try
			{
				WorldObject worldObject = tmpWorldObjects[i];
				if (!worldObject.def.expandingIcon)
				{
					continue;
				}
				float num = TransitionPct(worldObject);
				bool flag = IsHighlighted(worldObject);
				if ((num != 0f || flag) && !worldObject.HiddenBehindTerrainNow())
				{
					Material val = worldObject.ExpandingMaterial;
					Color expandingIconColor = worldObject.ExpandingIconColor;
					expandingIconColor.a = (flag ? 1f : num);
					if (worldTargeter.IsTargetedNow(worldObject, worldObjectsUnderMouse))
					{
						float num2 = GenMath.LerpDouble(-1f, 1f, 0.7f, 1f, Mathf.Sin(Time.time * 8f));
						expandingIconColor.r *= num2;
						expandingIconColor.g *= num2;
						expandingIconColor.b *= num2;
					}
					GUI.color = expandingIconColor;
					Rect rect = ExpandedIconScreenRect(worldObject);
					if (worldObject.ExpandingIconFlipHorizontal)
					{
						((Rect)(ref rect)).x = ((Rect)(ref rect)).xMax;
						((Rect)(ref rect)).width = ((Rect)(ref rect)).width * -1f;
					}
					if ((Object)(object)val == (Object)null && flag)
					{
						val = GetMaterial(expandingIconColor, HighlightedColor);
					}
					else if ((Object)(object)val == (Object)null && worldObject is MapParent { HasMap: not false })
					{
						val = GetMaterial(expandingIconColor, HasMapColor);
					}
					Widgets.DrawTextureRotated(rect, (Texture)(object)worldObject.ExpandingIcon, worldObject.ExpandingIconRotation, val);
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Error while drawing {tmpWorldObjects[i].ToStringSafe()}: {arg}");
			}
		}
		tmpWorldObjects.Clear();
		GUI.color = Color.white;
	}

	private static bool IsHighlighted(WorldObject wo)
	{
		if (!Find.WindowStack.TryGetWindow<Dialog_WorldSearch>(out var window))
		{
			return false;
		}
		if (window.CommonSearchWidget.filter.Text.Length >= 1)
		{
			return window.IsListed(wo);
		}
		return false;
	}

	private static Material GetMaterial(Color color, Color highlight)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MaterialRequest req = new MaterialRequest(null, ShaderDatabase.ExpandingIconUI, color);
		req.needsMainTex = false;
		req.colorTwo = highlight;
		return MaterialPool.MatFrom(req);
	}

	public static Rect ExpandedIconScreenRect(WorldObject o, float factor = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = o.ScreenPos();
		float num = 30f * o.def.expandingIconDrawSize;
		float num2 = ((!o.ExpandMore) ? num : Mathf.Lerp(num, num * 1.35f, ExpandMoreTransitionPct));
		num2 *= factor;
		return new Rect(val.x - num2 / 2f, val.y - num2 / 2f, num2, num2);
	}

	public static bool IsExpanded(WorldObject o)
	{
		if (TransitionPct(o) > 0.5f)
		{
			return o.def.expandingIcon;
		}
		return false;
	}

	public static void GetExpandedWorldObjectUnderMouse(Vector2 mousePos, List<WorldObject> outList)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		outList.Clear();
		Vector2 val = mousePos;
		val.y = (float)UI.screenHeight - val.y;
		List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
		for (int i = 0; i < allWorldObjects.Count; i++)
		{
			WorldObject worldObject = allWorldObjects[i];
			if (IsExpanded(worldObject))
			{
				Rect val2 = ExpandedIconScreenRect(worldObject);
				if (((Rect)(ref val2)).Contains(val) && !worldObject.HiddenBehindTerrainNow())
				{
					outList.Add(worldObject);
				}
			}
		}
		SortByExpandingIconPriority(outList);
		outList.Reverse();
	}

	private static void SortByExpandingIconPriority(List<WorldObject> worldObjects)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Vector3 cameraPos = Find.WorldCameraDriver.CameraPosition;
		worldObjects.SortBy(delegate(WorldObject x)
		{
			float num = x.ExpandingIconPriority;
			if (x.Faction != null && x.Faction.IsPlayer)
			{
				num += 0.001f;
			}
			return num;
		}, delegate(WorldObject x)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = x.DrawPos - cameraPos;
			return 0f - ((Vector3)(ref val)).sqrMagnitude;
		});
	}
}
