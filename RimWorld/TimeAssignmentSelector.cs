using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public static class TimeAssignmentSelector
{
	public static TimeAssignmentDef selectedAssignment = TimeAssignmentDefOf.Work;

	public static void DrawTimeAssignmentSelectorGrid(Rect rect)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 2f;
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).center.x;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).center.y;
		DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Anything);
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + ((Rect)(ref rect2)).width;
		DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Work);
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + ((Rect)(ref rect2)).width;
		DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Joy);
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + ((Rect)(ref rect2)).width;
		DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Sleep);
		if (ModsConfig.RoyaltyActive)
		{
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + ((Rect)(ref rect2)).width;
			DrawTimeAssignmentSelectorFor(rect2, TimeAssignmentDefOf.Meditate);
		}
	}

	private static void DrawTimeAssignmentSelectorFor(Rect rect, TimeAssignmentDef ta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		rect = rect.ContractedBy(2f);
		GUI.DrawTexture(rect, (Texture)(object)ta.ColorTexture);
		if (Widgets.ButtonInvisible(rect))
		{
			selectedAssignment = ta;
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		using (new TextBlock((TextAnchor)4))
		{
			Widgets.Label(rect, ta.LabelCap);
		}
		if (selectedAssignment == ta)
		{
			Widgets.DrawBox(rect, 2);
		}
		else
		{
			UIHighlighter.HighlightOpportunity(rect, ta.cachedHighlightNotSelectedTag);
		}
	}
}
