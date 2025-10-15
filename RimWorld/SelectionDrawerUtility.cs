using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class SelectionDrawerUtility
{
	private const float SelJumpDuration = 0.07f;

	private const float SelJumpDistance = 0.2f;

	public static readonly Texture2D SelectedTexGUI = ContentFinder<Texture2D>.Get("UI/Overlays/SelectionBracketGUI");

	private static Vector2[] bracketLocs = (Vector2[])(object)new Vector2[4];

	public static void CalculateSelectionBracketPositionsUI<T>(Vector2[] bracketLocs, T obj, Rect rect, Dictionary<T, float> selectTimes, Vector2 textureSize, float jumpDistanceFactor = 1f)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		float value;
		float num = (selectTimes.TryGetValue(obj, out value) ? Mathf.Max(0f, 1f - (Time.realtimeSinceStartup - value) / 0.07f) : 1f);
		float num2 = num * 0.2f * jumpDistanceFactor;
		float num3 = 0.5f * (((Rect)(ref rect)).width - textureSize.x) + num2;
		float num4 = 0.5f * (((Rect)(ref rect)).height - textureSize.y) + num2;
		bracketLocs[0] = new Vector2(((Rect)(ref rect)).center.x - num3, ((Rect)(ref rect)).center.y - num4);
		bracketLocs[1] = new Vector2(((Rect)(ref rect)).center.x + num3, ((Rect)(ref rect)).center.y - num4);
		bracketLocs[2] = new Vector2(((Rect)(ref rect)).center.x + num3, ((Rect)(ref rect)).center.y + num4);
		bracketLocs[3] = new Vector2(((Rect)(ref rect)).center.x - num3, ((Rect)(ref rect)).center.y + num4);
	}

	public static void CalculateSelectionBracketPositionsWorld<T>(Vector3[] bracketLocs, T obj, Vector3 worldPos, Vector2 worldSize, Dictionary<T, float> selectTimes, Vector2 textureSize, float jumpDistanceFactor = 1f, float deselectedJumpFactor = 1f)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		float value;
		float num = (selectTimes.TryGetValue(obj, out value) ? Mathf.Max(0f, 1f - (Time.realtimeSinceStartup - value) / 0.07f) : deselectedJumpFactor);
		float num2 = num * 0.2f * jumpDistanceFactor;
		Vector3 val = worldPos;
		float num3 = 0.5f * (worldSize.x - textureSize.x) + num2;
		float num4 = 0.5f * (worldSize.y - textureSize.y) + num2;
		float num5 = AltitudeLayer.MetaOverlays.AltitudeFor();
		if (obj is Thing thing)
		{
			ThingDef thingDef = GenConstruct.BuiltDefOf(thing.def) as ThingDef;
			if (thingDef?.building != null && thingDef.building.isAttachment)
			{
				val += (thing.Rotation.AsVector2 * 0.5f).ToVector3();
			}
		}
		bracketLocs[0] = new Vector3(val.x - num3, num5, val.z - num4);
		bracketLocs[1] = new Vector3(val.x + num3, num5, val.z - num4);
		bracketLocs[2] = new Vector3(val.x + num3, num5, val.z + num4);
		bracketLocs[3] = new Vector3(val.x - num3, num5, val.z + num4);
	}

	public static void DrawSelectionOverlayOnGUI(object target, Rect rect, float scale, float selectedTextJump)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 textureSize = default(Vector2);
		((Vector2)(ref textureSize))._002Ector((float)((Texture)SelectedTexGUI).width * scale, (float)((Texture)SelectedTexGUI).height * scale);
		CalculateSelectionBracketPositionsUI(bracketLocs, target, rect, SelectionDrawer.SelectTimes, textureSize, selectedTextJump);
		DrawSelectionOverlayOnGUI(bracketLocs, scale);
	}

	public static void DrawSelectionOverlayOnGUI(Vector2[] bracketLocs, float selectedTexScale)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		int num = 90;
		for (int i = 0; i < 4; i++)
		{
			Widgets.DrawTextureRotated(bracketLocs[i], (Texture)(object)SelectedTexGUI, num, selectedTexScale);
			num += 90;
		}
	}

	public static void DrawSelectionOverlayWholeGUI(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)TexUI.SelectionBracketWhole);
	}
}
