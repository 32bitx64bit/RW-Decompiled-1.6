using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class QuickSearchWidget
{
	public QuickSearchFilter filter = new QuickSearchFilter();

	public bool noResultsMatched;

	public Color inactiveTextColor = Color.white;

	public int maxSearchTextLength = 30;

	private readonly string controlName;

	public const float WidgetHeight = 24f;

	public const float IconSize = 18f;

	public const float IconMargin = 4f;

	private const int BaseMaxSearchTextLength = 30;

	private static int instanceCounter;

	public QuickSearchWidget()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		controlName = $"QuickSearchWidget_{instanceCounter++}";
	}

	public void OnGUI(Rect rect, Action onFilterChange = null, Action onClear = null)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (CurrentlyFocused() && (int)Event.current.type == 4 && (int)Event.current.keyCode == 27)
		{
			Unfocus();
			Event.current.Use();
		}
		if ((int)OriginalEventUtility.EventType == 0 && !((Rect)(ref rect)).Contains(Event.current.mousePosition))
		{
			Unfocus();
		}
		Color color = GUI.color;
		GUI.color = Color.white;
		float num = Mathf.Min(18f, ((Rect)(ref rect)).height);
		float num2 = num + 8f;
		float num3 = ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - num2) / 2f + 4f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + 4f, num3, num, num);
		GUI.DrawTexture(val, (Texture)(object)TexButton.Search);
		GUI.SetNextControlName(controlName);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).xMax + 4f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - num2, ((Rect)(ref rect)).height);
		if (filter.Active)
		{
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - num2;
		}
		using (new TextBlock(GameFont.Small))
		{
			if (noResultsMatched && filter.Active)
			{
				GUI.color = ColorLibrary.RedReadable;
			}
			else if (!filter.Active && !CurrentlyFocused())
			{
				GUI.color = inactiveTextColor;
			}
			string text = Widgets.TextField(rect2, filter.Text, maxSearchTextLength);
			GUI.color = Color.white;
			if (text != filter.Text)
			{
				filter.Text = text;
				onFilterChange?.Invoke();
			}
		}
		if (filter.Active && Widgets.ButtonImage(new Rect(((Rect)(ref rect2)).xMax + 4f, num3, num, num), TexButton.CloseXSmall))
		{
			filter.Text = "";
			SoundDefOf.CancelMode.PlayOneShotOnCamera();
			onFilterChange?.Invoke();
			onClear?.Invoke();
		}
		GUI.color = color;
	}

	public void Unfocus()
	{
		if (CurrentlyFocused())
		{
			UI.UnfocusCurrentControl();
		}
	}

	public void Focus()
	{
		GUI.FocusControl(controlName);
	}

	public bool CurrentlyFocused()
	{
		return GUI.GetNameOfFocusedControl() == controlName;
	}

	public void Reset()
	{
		filter.Text = "";
		noResultsMatched = false;
	}
}
