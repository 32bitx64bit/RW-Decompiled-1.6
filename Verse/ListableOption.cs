using System;
using UnityEngine;

namespace Verse;

public class ListableOption
{
	public string label;

	public Action action;

	private string uiHighlightTag;

	public float minHeight = 45f;

	public ListableOption(string label, Action action, string uiHighlightTag = null)
	{
		this.label = label;
		this.action = action;
		this.uiHighlightTag = uiHighlightTag;
	}

	public virtual float DrawOption(Vector2 pos, float width)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float num = Text.CalcHeight(label, width);
		float num2 = Mathf.Max(minHeight, num);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(pos.x, pos.y, width, num2);
		if (Widgets.ButtonText(rect, label))
		{
			action();
		}
		if (uiHighlightTag != null)
		{
			UIHighlighter.HighlightOpportunity(rect, uiHighlightTag);
		}
		return num2;
	}
}
