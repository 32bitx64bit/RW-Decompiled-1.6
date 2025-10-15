using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace LudeonTK;

public abstract class Dialog_DebugOptionLister : Dialog_OptionLister
{
	protected int currentHighlightIndex;

	protected int prioritizedHighlightedIndex;

	private const float DebugOptionsGap = 7f;

	protected virtual int HighlightedIndex => -1;

	public Dialog_DebugOptionLister()
	{
		forcePause = true;
	}

	public void NewColumn(float columnWidth)
	{
		curY = 0f;
		curX += columnWidth + 17f;
	}

	protected void NewColumnIfNeeded(float columnWidth, float neededHeight)
	{
		if (curY + neededHeight > ((Rect)(ref windowRect)).height)
		{
			NewColumn(columnWidth);
		}
	}

	public bool ButtonDebug(string label, float columnWidth, bool highlight)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(columnWidth, 22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, columnWidth, 22f);
		bool result = false;
		if (!base.BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(base.BoundingRectCached.Value))
		{
			bool wordWrap = Text.WordWrap;
			Text.WordWrap = false;
			result = DevGUI.ButtonText(rect, "  " + label, (TextAnchor)3);
			Text.WordWrap = wordWrap;
			if (highlight)
			{
				GUI.color = Color.yellow;
				DevGUI.DrawBox(rect, 2);
				GUI.color = Color.white;
			}
		}
		curY += 22f + verticalSpacing;
		return result;
	}

	protected void DebugLabel(string label, float columnWidth)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		Text.Font = GameFont.Small;
		float num = Text.CalcHeight(label, columnWidth);
		NewColumnIfNeeded(columnWidth, num);
		DevGUI.Label(new Rect(curX, curY, columnWidth, num), label);
		curY += num + verticalSpacing;
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += 24f;
		}
	}

	protected bool DebugAction(string label, float columnWidth, Action action, bool highlight)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		bool result = false;
		if (!FilterAllows(label))
		{
			return false;
		}
		if (ButtonDebug(label, columnWidth, highlight))
		{
			Close();
			action();
			result = true;
		}
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += 24f;
		}
		return result;
	}

	protected void DebugToolMap(string label, float columnWidth, Action toolAction, bool highlight)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Invalid comparison between Unknown and I4
		if (!WorldRendererUtility.WorldSelected)
		{
			if (!FilterAllows(label))
			{
				GUI.color = new Color(1f, 1f, 1f, 0.3f);
			}
			if (ButtonDebug(label, columnWidth, highlight))
			{
				Close();
				DebugTools.curTool = new DebugTool(label, toolAction);
			}
			GUI.color = Color.white;
			if ((int)Event.current.type == 8)
			{
				totalOptionsHeight += 24f;
			}
		}
	}

	protected virtual void ChangeHighlightedOption()
	{
	}
}
