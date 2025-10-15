using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_Search<T> : Window where T : class
{
	protected QuickSearchWidget quickSearchWidget;

	protected SortedList<string, T> searchResults;

	protected HashSet<T> searchResultsSet;

	protected T highlightedElement;

	protected Vector2 scrollPos;

	protected float scrollHeight;

	protected List<T> allElements;

	protected int searchIndex;

	protected bool triedToFocus;

	private int openFrames;

	protected const float ElementHeight = 26f;

	private const int MaxSearchesPerFrame = 500;

	public override Vector2 InitialSize => new Vector2(350f, 100f);

	public override QuickSearchWidget CommonSearchWidget => quickSearchWidget;

	protected bool Searching
	{
		get
		{
			if (!quickSearchWidget.filter.Text.NullOrEmpty() && allElements.Any())
			{
				return searchIndex < allElements.Count;
			}
			return false;
		}
	}

	protected abstract List<T> SearchSet { get; }

	protected abstract bool ShouldClose { get; }

	protected abstract TaggedString SearchLabel { get; }

	public T Highlighted => highlightedElement;

	protected abstract void TryAddElement(T element);

	protected abstract void TryRemoveElement(T element);

	protected abstract void DoIcon(T element, Rect iconRect);

	protected abstract void DoLabel(T element, Rect labelRect);

	protected virtual void DoExtraIcon(T element, Rect iconRect)
	{
	}

	protected abstract void ClikedOnElement(T element);

	protected abstract bool ShouldSkipElement(T element);

	protected abstract void OnHighlightUpdate(T element);

	protected virtual void CheckAnyElementRemoved()
	{
	}

	public bool IsListed(T element)
	{
		return searchResultsSet.Contains(element);
	}

	protected override Rect QuickSearchWidgetRect(Rect winRect, Rect inRect)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - 24f, ((Rect)(ref inRect)).width, 24f);
	}

	public Dialog_Search()
	{
		doCloseX = true;
		closeOnAccept = false;
		preventCameraMotion = false;
		quickSearchWidget = new QuickSearchWidget();
		searchResults = new SortedList<string, T>(new DuplicateKeyComparer<string>());
		searchResultsSet = new HashSet<T>();
		allElements = new List<T>();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		highlightedElement = null;
		float num = Text.CalcHeight(SearchLabel, ((Rect)(ref inRect)).width);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, ((Rect)(ref inRect)).yMax - 24f - num, ((Rect)(ref inRect)).width, num);
		using (new TextBlock(ColoredText.SubtleGrayColor, (TextAnchor)3, newWordWrap: true))
		{
			Widgets.Label(label: Searching ? QuickSearchUtility.CurrentSearchText : ((quickSearchWidget.filter.Text.Length <= 0) ? ((string)SearchLabel) : ((string)((searchResults.Count == 1) ? "MapSearchResultSingular".Translate() : "MapSearchResults".Translate(searchResults.Count)))), rect: rect);
		}
		if (searchResults.Count > 0)
		{
			Rect outRect = default(Rect);
			((Rect)(ref outRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height);
			((Rect)(ref outRect)).yMax = ((Rect)(ref rect)).yMin;
			Rect viewRect = default(Rect);
			((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, scrollHeight);
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, scrollPos.y, ((Rect)(ref outRect)).width, ((Rect)(ref outRect)).height);
			bool flag = scrollHeight >= ((Rect)(ref outRect)).height;
			Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
			using (new ProfilerBlock("DrawSearchResults"))
			{
				Rect val2 = default(Rect);
				for (int i = 0; i < searchResults.Count; i++)
				{
					((Rect)(ref val2))._002Ector(0f, 26f * (float)i, ((Rect)(ref inRect)).width, 26f);
					if (!((Rect)(ref val)).Overlaps(val2))
					{
						continue;
					}
					if (i % 2 == 1)
					{
						Widgets.DrawLightHighlight(val2);
					}
					T val3 = searchResults.Values[i];
					if (val3 != null && !ShouldSkipElement(val3))
					{
						Rect iconRect = val2;
						((Rect)(ref iconRect)).xMax = 26f;
						DoIcon(val3, iconRect);
						Rect iconRect2 = val2;
						if (flag)
						{
							((Rect)(ref iconRect2)).xMin = ((Rect)(ref val2)).xMax - 26f - 16f;
							((Rect)(ref iconRect2)).width = 26f;
						}
						else
						{
							((Rect)(ref iconRect2)).xMin = ((Rect)(ref val2)).xMax - 26f;
						}
						DoExtraIcon(val3, iconRect2);
						Rect labelRect = val2;
						((Rect)(ref labelRect)).xMin = ((Rect)(ref iconRect)).xMax + 4f;
						((Rect)(ref labelRect)).xMax = ((Rect)(ref iconRect2)).xMin - 4f;
						DoLabel(val3, labelRect);
						if (Mouse.IsOver(val2))
						{
							Widgets.DrawHighlight(val2);
							highlightedElement = val3;
						}
						if (Widgets.ButtonInvisible(val2))
						{
							ClikedOnElement(val3);
						}
					}
				}
			}
			Widgets.EndScrollView();
		}
		if (!triedToFocus && openFrames == 2)
		{
			quickSearchWidget.Focus();
			triedToFocus = true;
		}
	}

	public override void WindowUpdate()
	{
		base.WindowUpdate();
		openFrames++;
		if (ShouldClose)
		{
			Close();
			return;
		}
		if (highlightedElement != null)
		{
			OnHighlightUpdate(highlightedElement);
		}
		if (Searching)
		{
			using (new ProfilerBlock("Searching"))
			{
				for (int i = 0; i < 500; i++)
				{
					searchIndex++;
					if (searchIndex >= allElements.Count)
					{
						allElements.Clear();
						break;
					}
					TryAddElement(allElements[searchIndex]);
				}
				return;
			}
		}
		if (Time.frameCount % 20 == 0)
		{
			CheckAnyElementRemoved();
		}
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		scrollHeight = (float)searchResults.Count * 26f;
		Vector2 initialSize = InitialSize;
		initialSize.y = Mathf.Clamp(initialSize.y + scrollHeight, InitialSize.y, (float)UI.screenHeight / 2f);
		windowRect = GenUI.Rounded(new Rect((float)UI.screenWidth - initialSize.x, (float)UI.screenHeight - initialSize.y - 35f, initialSize.x, initialSize.y));
	}

	public override void Notify_CommonSearchChanged()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		scrollPos = Vector2.zero;
		searchIndex = 0;
		searchResults.Clear();
		searchResultsSet.Clear();
		allElements.Clear();
		allElements.AddRange(SearchSet);
		SetInitialSizeAndPosition();
	}

	protected bool TextMatch(string text)
	{
		if (text.NullOrEmpty())
		{
			return false;
		}
		return text.IndexOf(quickSearchWidget.filter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
	}
}
