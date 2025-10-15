using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class WorldSelectionDrawer
{
	private static readonly Dictionary<WorldObject, float> selectTimes = new Dictionary<WorldObject, float>();

	private const float BaseSelectedTexJump = 25f;

	private const float BaseSelectedTexScale = 0.4f;

	private const float BaseSelectionRectSize = 35f;

	private static readonly Color HiddenSelectionBracketColor = new Color(1f, 1f, 1f, 0.35f);

	private static readonly Vector2[] bracketLocs = (Vector2[])(object)new Vector2[4];

	public static Dictionary<WorldObject, float> SelectTimes => selectTimes;

	public static void Notify_Selected(WorldObject t)
	{
		selectTimes[t] = Time.realtimeSinceStartup;
	}

	public static void Clear()
	{
		selectTimes.Clear();
	}

	public static void SelectionOverlaysOnGUI()
	{
		List<WorldObject> selectedObjects = Find.WorldSelector.SelectedObjects;
		for (int i = 0; i < selectedObjects.Count; i++)
		{
			WorldObject worldObject = selectedObjects[i];
			DrawSelectionBracketOnGUIFor(worldObject);
			worldObject.ExtraSelectionOverlaysOnGUI();
		}
	}

	public static void DrawSelectionOverlays()
	{
		List<WorldObject> selectedObjects = Find.WorldSelector.SelectedObjects;
		for (int i = 0; i < selectedObjects.Count; i++)
		{
			selectedObjects[i].DrawExtraSelectionOverlays();
		}
	}

	private static void DrawSelectionBracketOnGUIFor(WorldObject obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = obj.ScreenPos();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(val.x - 17.5f, val.y - 17.5f, 35f, 35f);
		Vector2 textureSize = default(Vector2);
		((Vector2)(ref textureSize))._002Ector((float)((Texture)SelectionDrawerUtility.SelectedTexGUI).width * 0.4f, (float)((Texture)SelectionDrawerUtility.SelectedTexGUI).height * 0.4f);
		SelectionDrawerUtility.CalculateSelectionBracketPositionsUI(bracketLocs, obj, rect, selectTimes, textureSize, 25f);
		GUI.color = (obj.HiddenBehindTerrainNow() ? HiddenSelectionBracketColor : Color.white);
		int num = 90;
		for (int i = 0; i < 4; i++)
		{
			Widgets.DrawTextureRotated(bracketLocs[i], (Texture)(object)SelectionDrawerUtility.SelectedTexGUI, num, 0.4f);
			num += 90;
		}
		GUI.color = Color.white;
	}
}
