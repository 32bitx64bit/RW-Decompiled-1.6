using System;
using UnityEngine;
using Verse;

namespace LudeonTK;

public class MeasureWorldDistanceTool : MeasureTool
{
	public MeasureWorldDistanceTool(string label, Action clickAction, Action onGUIAction = null)
	{
		base.label = label;
		base.clickAction = clickAction;
		base.onGUIAction = onGUIAction;
	}

	public MeasureWorldDistanceTool(string label, Action clickAction, Vector3 firstRectCorner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.label = label;
		base.clickAction = clickAction;
		onGUIAction = delegate
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Vector3 v = UI.MouseMapPosition();
			Vector2 start = firstRectCorner.MapToUIPosition();
			Vector2 end = v.MapToUIPosition();
			DevGUI.DrawLine(start, end, Color.white, 0.25f);
		};
	}
}
