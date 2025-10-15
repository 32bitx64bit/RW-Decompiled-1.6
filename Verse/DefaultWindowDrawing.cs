using UnityEngine;

namespace Verse;

public class DefaultWindowDrawing : IWindowDrawing
{
	public GUIStyle EmptyStyle => Widgets.EmptyStyle;

	public bool DoCloseButton(Rect rect, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Widgets.ButtonText(rect, text);
	}

	public bool DoClostButtonSmall(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Widgets.CloseButtonFor(rect);
	}

	public void DoGrayOut(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawRectFast(rect, new Color(0f, 0f, 0f, 0.5f));
	}

	public void DoWindowBackground(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawWindowBackground(rect);
	}

	public void BeginGroup(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
	}

	public void EndGroup()
	{
		Widgets.EndGroup();
	}
}
