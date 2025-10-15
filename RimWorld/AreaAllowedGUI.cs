using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public static class AreaAllowedGUI
{
	private static bool dragging;

	public static void DoAllowedAreaSelectors(Rect rect, Pawn p)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (Find.CurrentMap == null)
		{
			return;
		}
		List<Area> allAreas = Find.CurrentMap.areaManager.AllAreas;
		int num = 1;
		for (int i = 0; i < allAreas.Count; i++)
		{
			if (allAreas[i].AssignableAsAllowed())
			{
				num++;
			}
		}
		float num2 = ((Rect)(ref rect)).width / (float)num;
		Text.WordWrap = false;
		Text.Font = GameFont.Tiny;
		DoAreaSelector(new Rect(((Rect)(ref rect)).x + 0f, ((Rect)(ref rect)).y, num2, ((Rect)(ref rect)).height), p, null);
		int num3 = 1;
		for (int j = 0; j < allAreas.Count; j++)
		{
			if (allAreas[j].AssignableAsAllowed())
			{
				float num4 = (float)num3 * num2;
				DoAreaSelector(new Rect(((Rect)(ref rect)).x + num4, ((Rect)(ref rect)).y, num2, ((Rect)(ref rect)).height), p, allAreas[j]);
				num3++;
			}
		}
		Text.WordWrap = true;
		Text.Font = GameFont.Small;
	}

	private static void DoAreaSelector(Rect rect, Pawn p, Area area)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(rect);
		rect = rect.ContractedBy(1f);
		GUI.DrawTexture(rect, (Texture)(object)((area != null) ? area.ColorTexture : BaseContent.GreyTex));
		Text.Anchor = (TextAnchor)3;
		string text = AreaUtility.AreaAllowedLabel_Area(area);
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 3f;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin + 2f;
		Widgets.Label(rect2, text);
		if (p.playerSettings.AreaRestrictionInPawnCurrentMap == area)
		{
			Widgets.DrawBox(rect, 2);
		}
		if ((int)Event.current.rawType == 1 && Event.current.button == 0)
		{
			dragging = false;
		}
		if (!Input.GetMouseButton(0) && (int)Event.current.type != 0)
		{
			dragging = false;
		}
		if (Mouse.IsOver(rect))
		{
			area?.MarkForDraw();
			if ((int)Event.current.type == 0 && Event.current.button == 0)
			{
				dragging = true;
			}
			if (dragging && p.playerSettings.AreaRestrictionInPawnCurrentMap != area)
			{
				p.playerSettings.AreaRestrictionInPawnCurrentMap = area;
				SoundDefOf.Designate_DragStandard_Changed_NoCam.PlayOneShotOnCamera();
			}
		}
		Text.Anchor = (TextAnchor)0;
		TooltipHandler.TipRegion(rect, text);
	}
}
