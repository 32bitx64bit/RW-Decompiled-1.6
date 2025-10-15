using System;
using UnityEngine;

namespace Verse;

public abstract class InspectTabBase
{
	public string labelKey;

	protected Vector2 size;

	public string tutorTag;

	private string cachedTutorHighlightTagClosed;

	protected abstract float PaneTopY { get; }

	protected abstract bool StillValid { get; }

	public virtual bool IsVisible => true;

	public virtual bool Hidden => false;

	public virtual bool VisibleInBlueprintMode { get; } = true;


	public string TutorHighlightTagClosed
	{
		get
		{
			if (tutorTag == null)
			{
				return null;
			}
			if (cachedTutorHighlightTagClosed == null)
			{
				cachedTutorHighlightTagClosed = "ITab-" + tutorTag + "-Closed";
			}
			return cachedTutorHighlightTagClosed;
		}
	}

	protected Rect TabRect
	{
		get
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			UpdateSize();
			float num = PaneTopY - 30f - size.y;
			return new Rect(0f, num, size.x, size.y);
		}
	}

	public void DoTabGUI()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = TabRect;
		Find.WindowStack.ImmediateWindow(235086, rect, WindowLayer.GameUI, delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (!StillValid || !IsVisible)
			{
				return;
			}
			if (Widgets.CloseButtonFor(rect.AtZero()))
			{
				CloseTab();
			}
			try
			{
				FillTab();
			}
			catch (Exception ex)
			{
				Log.ErrorOnce("Exception filling tab " + GetType()?.ToString() + ": " + ex, 49827);
			}
		}, doBackground: true, absorbInputAroundWindow: false, 1f, Notify_ClickOutsideWindow);
		ExtraOnGUI();
	}

	protected abstract void CloseTab();

	protected abstract void FillTab();

	protected virtual void ExtraOnGUI()
	{
	}

	protected virtual void UpdateSize()
	{
	}

	public virtual void OnOpen()
	{
	}

	public virtual void TabTick()
	{
	}

	public virtual void TabUpdate()
	{
	}

	public virtual void Notify_ClearingAllMapsMemory()
	{
	}

	public virtual void Notify_ClickOutsideWindow()
	{
	}
}
