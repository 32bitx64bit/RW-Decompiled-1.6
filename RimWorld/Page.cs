using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Page : Window
{
	public Page prev;

	public Page next;

	public Action nextAct;

	public static readonly Vector2 StandardSize = new Vector2(1020f, 764f);

	public const float TitleAreaHeight = 45f;

	public const float BottomButHeight = 38f;

	protected static readonly Vector2 BottomButSize = new Vector2(150f, 38f);

	public override Vector2 InitialSize => StandardSize;

	public virtual string PageTitle => null;

	public Page()
	{
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		closeOnCancel = false;
		forceCatchAcceptAndCancelEventEvenIfUnfocused = true;
	}

	protected void DrawPageTitle(Rect rect)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref rect)).width, 45f), PageTitle);
		Text.Font = GameFont.Small;
	}

	protected Rect GetMainRect(Rect rect, float extraTopSpace = 0f, bool ignoreTitle = false)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (!ignoreTitle)
		{
			num = 45f + extraTopSpace;
		}
		return new Rect(0f, num, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - 38f - num - 17f);
	}

	protected void DoBottomButtons(Rect rect, string nextLabel = null, string midLabel = null, Action midAct = null, bool showNext = true, bool doNextOnKeypress = true)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - 38f;
		Text.Font = GameFont.Small;
		string label = "Back".Translate();
		if ((Widgets.ButtonText(new Rect(((Rect)(ref rect)).x, num, BottomButSize.x, BottomButSize.y), label) || KeyBindingDefOf.Cancel.KeyDownEvent) && CanDoBack())
		{
			DoBack();
		}
		if (showNext)
		{
			if (nextLabel.NullOrEmpty())
			{
				nextLabel = "Next".Translate();
			}
			Rect rect2 = new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - BottomButSize.x, num, BottomButSize.x, BottomButSize.y);
			if ((Widgets.ButtonText(rect2, nextLabel) || (doNextOnKeypress && KeyBindingDefOf.Accept.KeyDownEvent)) && CanDoNext())
			{
				DoNext();
			}
			UIHighlighter.HighlightOpportunity(rect2, "NextPage");
		}
		if (midAct != null && Widgets.ButtonText(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f - BottomButSize.x / 2f, num, BottomButSize.x, BottomButSize.y), midLabel))
		{
			midAct();
		}
	}

	protected virtual bool CanDoBack()
	{
		if (TutorSystem.TutorialMode)
		{
			return TutorSystem.AllowAction("GotoPrevPage");
		}
		return true;
	}

	protected virtual bool CanDoNext()
	{
		if (TutorSystem.TutorialMode)
		{
			return TutorSystem.AllowAction("GotoNextPage");
		}
		return true;
	}

	protected virtual void DoNext()
	{
		if (next != null)
		{
			Find.WindowStack.Add(next);
		}
		if (nextAct != null)
		{
			nextAct();
		}
		TutorSystem.Notify_Event("PageClosed");
		TutorSystem.Notify_Event("GoToNextPage");
		Close();
	}

	protected virtual void DoBack()
	{
		TutorSystem.Notify_Event("PageClosed");
		TutorSystem.Notify_Event("GoToPrevPage");
		if (prev != null)
		{
			Find.WindowStack.Add(prev);
		}
		Close();
	}

	public override void OnCancelKeyPressed()
	{
		if (closeOnCancel && (Find.World == null || !Find.WorldRoutePlanner.Active))
		{
			if (CanDoBack())
			{
				DoBack();
			}
			else
			{
				Close();
			}
			Event.current.Use();
			base.OnCancelKeyPressed();
		}
	}

	public override void OnAcceptKeyPressed()
	{
		if (closeOnAccept && (Find.World == null || !Find.WorldRoutePlanner.Active))
		{
			if (CanDoNext())
			{
				DoNext();
			}
			Event.current.Use();
		}
	}
}
