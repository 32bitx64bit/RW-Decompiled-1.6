using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public static class CopyPasteUI
{
	public const float CopyPasteIconHeight = 24f;

	public const float CopyPasteIconWidth = 18f;

	public const float CopyPasteColumnWidth = 36f;

	public static void DoCopyPasteButtons(Rect rect, Action copyAction, Action pasteAction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height / 2f - 12f), 18f, 24f);
		if (Widgets.ButtonImage(val, TexButton.Copy))
		{
			copyAction();
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
		TooltipHandler.TipRegionByKey(val, "Copy");
		if (pasteAction != null)
		{
			Rect val2 = val;
			((Rect)(ref val2)).x = ((Rect)(ref val)).xMax;
			if (Widgets.ButtonImage(val2, TexButton.Paste))
			{
				pasteAction();
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			TooltipHandler.TipRegionByKey(val2, "Paste");
		}
	}
}
