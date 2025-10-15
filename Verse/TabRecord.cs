using System;
using LudeonTK;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class TabRecord
{
	public string label = "Tab";

	public Action clickedAction;

	public bool selected;

	public Func<bool> selectedGetter;

	public Color? labelColor;

	private const float TabEndWidth = 30f;

	private const float TabMiddleGraphicWidth = 4f;

	private static readonly Texture2D TabAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TabAtlas");

	public bool Selected
	{
		get
		{
			if (selectedGetter == null)
			{
				return selected;
			}
			return selectedGetter();
		}
	}

	public virtual string TutorTag => null;

	public TabRecord(string label, Action clickedAction, bool selected)
	{
		this.label = label;
		this.clickedAction = clickedAction;
		this.selected = selected;
	}

	public TabRecord(string label, Action clickedAction, Func<bool> selected)
	{
		this.label = label;
		this.clickedAction = clickedAction;
		selectedGetter = selected;
	}

	public virtual string GetTip()
	{
		return null;
	}

	public void Draw(Rect rect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		Rect drawRect = default(Rect);
		((Rect)(ref drawRect))._002Ector(rect);
		((Rect)(ref drawRect)).width = 30f;
		Rect drawRect2 = default(Rect);
		((Rect)(ref drawRect2))._002Ector(rect);
		((Rect)(ref drawRect2)).width = 30f;
		((Rect)(ref drawRect2)).x = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 30f;
		Rect uvRect = default(Rect);
		((Rect)(ref uvRect))._002Ector(17f / 32f, 0f, 15f / 32f, 1f);
		Rect drawRect3 = default(Rect);
		((Rect)(ref drawRect3))._002Ector(rect);
		((Rect)(ref drawRect3)).x = ((Rect)(ref drawRect3)).x + ((Rect)(ref drawRect)).width;
		((Rect)(ref drawRect3)).width = ((Rect)(ref drawRect3)).width - 60f;
		((Rect)(ref drawRect3)).xMin = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref drawRect3)).xMin);
		((Rect)(ref drawRect3)).xMax = UIScaling.AdjustCoordToUIScalingCeil(((Rect)(ref drawRect3)).xMax);
		Rect uvRect2 = Widgets.ToUVRect(new Rect(30f, 0f, 4f, (float)((Texture)TabAtlas).height), new Vector2((float)((Texture)TabAtlas).width, (float)((Texture)TabAtlas).height));
		Widgets.DrawTexturePart(drawRect, new Rect(0f, 0f, 15f / 32f, 1f), TabAtlas);
		Widgets.DrawTexturePart(drawRect3, uvRect2, TabAtlas);
		Widgets.DrawTexturePart(drawRect2, uvRect, TabAtlas);
		GUI.color = (Color)(((_003F?)labelColor) ?? Color.white);
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - 10f;
		if (Mouse.IsOver(rect2))
		{
			GUI.color = Color.yellow;
			((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 2f;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - 2f;
		}
		if (!TutorTag.NullOrEmpty())
		{
			UIHighlighter.HighlightOpportunity(rect2, TutorTag);
		}
		Text.WordWrap = false;
		Widgets.Label(rect, label);
		Text.WordWrap = true;
		GUI.color = Color.white;
		if (!Selected)
		{
			Rect drawRect4 = default(Rect);
			((Rect)(ref drawRect4))._002Ector(rect);
			((Rect)(ref drawRect4)).y = ((Rect)(ref drawRect4)).y + ((Rect)(ref rect)).height;
			((Rect)(ref drawRect4)).y = ((Rect)(ref drawRect4)).y - 1f;
			((Rect)(ref drawRect4)).height = 1f;
			Rect uvRect3 = default(Rect);
			((Rect)(ref uvRect3))._002Ector(0.5f, 0.01f, 0.01f, 0.01f);
			Widgets.DrawTexturePart(drawRect4, uvRect3, TabAtlas);
		}
	}
}
