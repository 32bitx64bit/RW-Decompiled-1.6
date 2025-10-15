using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public static class BeautyDrawer
{
	private static HashSet<Thing> beautyCountedThings = new HashSet<Thing>();

	private static Color ColorUgly = Color.red;

	private static Color ColorBeautiful = Color.green;

	public static void BeautyDrawerOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 7 && ShouldShow())
		{
			DrawBeautyAroundMouse();
		}
	}

	private static bool ShouldShow()
	{
		if (Mouse.IsInputBlockedNow)
		{
			return false;
		}
		if (!UI.MouseCell().InBounds(Find.CurrentMap) || UI.MouseCell().Fogged(Find.CurrentMap))
		{
			return false;
		}
		if (CellInspectorDrawer.active)
		{
			return true;
		}
		if (!Find.PlaySettings.showBeauty)
		{
			return false;
		}
		return true;
	}

	private static void DrawBeautyAroundMouse()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		BeautyUtility.FillBeautyRelevantCells(UI.MouseCell(), Find.CurrentMap);
		for (int i = 0; i < BeautyUtility.beautyRelevantCells.Count; i++)
		{
			IntVec3 intVec = BeautyUtility.beautyRelevantCells[i];
			float num = BeautyUtility.CellBeauty(intVec, Find.CurrentMap, beautyCountedThings);
			if (num != 0f)
			{
				GenMapUI.DrawThingLabel(Vector2.op_Implicit(Vector2.op_Implicit(GenMapUI.LabelDrawPosFor(intVec))), Mathf.RoundToInt(num).ToStringCached(), BeautyColor(num, 8f));
			}
		}
		beautyCountedThings.Clear();
	}

	public static Color BeautyColor(float beauty, float scale)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.InverseLerp(0f - scale, scale, beauty);
		num = Mathf.Clamp01(num);
		return Color.Lerp(Color.Lerp(ColorUgly, ColorBeautiful, num), Color.white, 0.5f);
	}
}
