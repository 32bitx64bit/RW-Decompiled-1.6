using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Listing_ScenEdit : Listing_Standard
{
	private Scenario scen;

	public Listing_ScenEdit(Scenario scen)
	{
		this.scen = scen;
	}

	public Rect GetScenPartRect(ScenPart part, float height)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		string label = part.Label;
		Rect rect = GetRect(height);
		Widgets.DrawBoxSolid(rect, new Color(1f, 1f, 1f, 0.08f));
		WidgetRow widgetRow = new WidgetRow(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, UIDirection.RightThenDown, 72f, 0f);
		if (part.def.PlayerAddRemovable && widgetRow.ButtonIcon(TexButton.Delete, null, GenUI.SubtleMouseoverColor))
		{
			scen.RemovePart(part);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		if (scen.CanReorder(part, ReorderDirection.Up) && widgetRow.ButtonIcon(TexButton.ReorderUp))
		{
			scen.Reorder(part, ReorderDirection.Up);
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
		if (scen.CanReorder(part, ReorderDirection.Down) && widgetRow.ButtonIcon(TexButton.ReorderDown))
		{
			scen.Reorder(part, ReorderDirection.Down);
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
		}
		Text.Anchor = (TextAnchor)2;
		Rect rect2 = rect.LeftPart(0.5f).Rounded();
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 4f;
		Widgets.Label(rect2, label);
		Text.Anchor = (TextAnchor)0;
		Gap(4f);
		return rect.RightPart(0.5f).Rounded();
	}
}
