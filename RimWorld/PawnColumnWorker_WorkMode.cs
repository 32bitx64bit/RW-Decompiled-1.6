using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_WorkMode : PawnColumnWorker
{
	private const int Width = 160;

	private const int LeftMargin = 3;

	private static List<FloatMenuOption> tmpFloatMenuOptions = new List<FloatMenuOption>();

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		MechanitorControlGroup mechControlGroup = pawn.GetMechControlGroup();
		if (mechControlGroup == null)
		{
			return;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 3f;
		Widgets.Label(rect2, mechControlGroup.WorkMode.LabelCap);
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect))
		{
			AcceptanceReport canControlMechs = mechControlGroup.Tracker.CanControlMechs;
			TipSignal tooltip = pawn.GetTooltip();
			tooltip.text = "ClickToChangeWorkMode".Translate();
			if (!canControlMechs && !canControlMechs.Reason.NullOrEmpty())
			{
				ref string text = ref tooltip.text;
				text = text + "\n\n" + ("DisabledCommand".Translate() + ": " + canControlMechs.Reason).Colorize(ColorLibrary.RedReadable);
			}
			TooltipHandler.TipRegion(rect, tooltip);
			if ((bool)canControlMechs && Widgets.ButtonInvisible(rect))
			{
				tmpFloatMenuOptions.Clear();
				tmpFloatMenuOptions.AddRange(MechanitorControlGroupGizmo.GetWorkModeOptions(mechControlGroup));
				Find.WindowStack.Add(new FloatMenu(tmpFloatMenuOptions));
				tmpFloatMenuOptions.Clear();
			}
			Widgets.DrawHighlight(rect);
		}
	}

	public override bool CanGroupWith(Pawn pawn, Pawn other)
	{
		MechanitorControlGroup mechControlGroup = pawn.GetMechControlGroup();
		if (mechControlGroup != null)
		{
			return other.GetMechControlGroup() == mechControlGroup;
		}
		return false;
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), 160);
	}

	public override int GetMaxWidth(PawnTable table)
	{
		return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
	}
}
