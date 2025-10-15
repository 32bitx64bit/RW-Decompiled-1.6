using System;
using UnityEngine;
using Verse;

namespace LudeonTK;

public class DebugTool
{
	private string label;

	private Action clickAction;

	private Action onGUIAction;

	public DebugTool(string label, Action clickAction, Action onGUIAction = null)
	{
		this.label = label;
		this.clickAction = clickAction;
		this.onGUIAction = onGUIAction;
	}

	public DebugTool(string label, Action clickAction, IntVec3 firstRectCorner)
	{
		this.label = label;
		this.clickAction = clickAction;
		onGUIAction = delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			IntVec3 intVec = UI.MouseCell();
			Vector3 val = firstRectCorner.ToVector3Shifted();
			Vector3 val2 = intVec.ToVector3Shifted();
			if (val.x < val2.x)
			{
				val.x -= 0.5f;
				val2.x += 0.5f;
			}
			else
			{
				val.x += 0.5f;
				val2.x -= 0.5f;
			}
			if (val.z < val2.z)
			{
				val.z -= 0.5f;
				val2.z += 0.5f;
			}
			else
			{
				val.z += 0.5f;
				val2.z -= 0.5f;
			}
			Vector2 val3 = val.MapToUIPosition();
			Vector2 val4 = val2.MapToUIPosition();
			DevGUI.DrawBox(new Rect(val3.x, val3.y, val4.x - val3.x, val4.y - val3.y), 3);
		};
	}

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
				DebugTools.curTool = null;
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
