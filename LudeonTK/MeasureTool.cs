using System;
using UnityEngine;
using Verse;

namespace LudeonTK;

public class MeasureTool
{
	public string label;

	public Action clickAction;

	public Action onGUIAction;

	public void DebugToolOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0)
		{
			if (Event.current.button == 0)
			{
				clickAction();
			}
			if (Event.current.button == 1)
			{
				DebugTools.curMeasureTool = null;
			}
			Event.current.Use();
		}
		Vector2 val = Event.current.mousePosition + new Vector2(15f, 15f);
		Rect rect = new Rect(val.x, val.y, 999f, 999f);
		Text.Font = GameFont.Small;
		DevGUI.Label(rect, label);
		if (onGUIAction != null)
		{
			onGUIAction();
		}
	}
}
