using System;
using UnityEngine;

namespace Verse;

public class Dialog_MessageBox : Window
{
	public TaggedString text;

	public string title;

	public string buttonAText;

	public Action buttonAAction;

	public bool buttonADestructive;

	public string buttonBText;

	public Action buttonBAction;

	public string buttonCText;

	public Action buttonCAction;

	public bool buttonCClose = true;

	public float interactionDelay;

	public Action acceptAction;

	public Action cancelAction;

	public Texture2D image;

	private Vector2 scrollPosition = Vector2.zero;

	private float creationRealTime = -1f;

	private const float TitleHeight = 42f;

	protected const float ButtonHeight = 35f;

	public override Vector2 InitialSize => new Vector2(640f, 460f);

	private float TimeUntilInteractive => interactionDelay - (Time.realtimeSinceStartup - creationRealTime);

	private bool InteractionDelayExpired => TimeUntilInteractive <= 0f;

	public static Dialog_MessageBox CreateConfirmation(TaggedString text, Action confirmedAct, bool destructive = false, string title = null, WindowLayer layer = WindowLayer.Dialog)
	{
		return new Dialog_MessageBox(text, "Confirm".Translate(), confirmedAct, "GoBack".Translate(), null, title, destructive, confirmedAct, delegate
		{
		}, layer);
	}

	public static Dialog_MessageBox CreateConfirmation(TaggedString text, Action confirmedAct, Action cancelAct, bool destructive = false, string title = null, WindowLayer layer = WindowLayer.Dialog)
	{
		return new Dialog_MessageBox(text, "Confirm".Translate(), confirmedAct, "GoBack".Translate(), cancelAct, title, destructive, confirmedAct, cancelAct, layer);
	}

	public Dialog_MessageBox(TaggedString text, string buttonAText = null, Action buttonAAction = null, string buttonBText = null, Action buttonBAction = null, string title = null, bool buttonADestructive = false, Action acceptAction = null, Action cancelAction = null, WindowLayer layer = WindowLayer.Dialog)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		this.text = text;
		this.buttonAText = buttonAText;
		this.buttonAAction = buttonAAction;
		this.buttonADestructive = buttonADestructive;
		this.buttonBText = buttonBText;
		this.buttonBAction = buttonBAction;
		this.title = title;
		this.acceptAction = acceptAction;
		this.cancelAction = cancelAction;
		base.layer = layer;
		if (buttonAText.NullOrEmpty())
		{
			this.buttonAText = "OK".Translate();
		}
		forcePause = true;
		absorbInputAroundWindow = true;
		creationRealTime = RealTime.LastRealTime;
		onlyOneOfTypeAllowed = false;
		bool flag = buttonAAction == null && buttonBAction == null && buttonCAction == null;
		forceCatchAcceptAndCancelEventEvenIfUnfocused = acceptAction != null || cancelAction != null || flag;
		closeOnAccept = flag;
		closeOnCancel = flag;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref inRect)).y;
		if (!title.NullOrEmpty())
		{
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, num, ((Rect)(ref inRect)).width, 42f), title);
			num += 42f;
		}
		if ((Object)(object)image != (Object)null)
		{
			float num2 = (float)((Texture)image).width / (float)((Texture)image).height;
			float num3 = 270f * num2;
			GUI.DrawTexture(new Rect(((Rect)(ref inRect)).x + (((Rect)(ref inRect)).width - num3) / 2f, num, num3, 270f), (Texture)(object)image);
			num += 280f;
		}
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(((Rect)(ref inRect)).x, num, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - 35f - 5f - num);
		float num4 = ((Rect)(ref outRect)).width - 16f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, num4, Text.CalcHeight(text, num4));
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref viewRect)).width, ((Rect)(ref viewRect)).height), text);
		Widgets.EndScrollView();
		int num5 = (buttonCText.NullOrEmpty() ? 2 : 3);
		float num6 = ((Rect)(ref inRect)).width / (float)num5;
		float num7 = num6 - 10f;
		if (buttonADestructive)
		{
			GUI.color = new Color(1f, 0.3f, 0.35f);
		}
		string label = (InteractionDelayExpired ? buttonAText : (buttonAText + "(" + Mathf.Ceil(TimeUntilInteractive).ToString("F0") + ")"));
		if (Widgets.ButtonText(new Rect(num6 * (float)(num5 - 1) + 10f, ((Rect)(ref inRect)).height - 35f, num7, 35f), label) && InteractionDelayExpired)
		{
			if (buttonAAction != null)
			{
				buttonAAction();
			}
			Close();
		}
		GUI.color = Color.white;
		if (buttonBText != null && Widgets.ButtonText(new Rect(0f, ((Rect)(ref inRect)).height - 35f, num7, 35f), buttonBText))
		{
			if (buttonBAction != null)
			{
				buttonBAction();
			}
			Close();
		}
		if (buttonCText != null && Widgets.ButtonText(new Rect(num6, ((Rect)(ref inRect)).height - 35f, num7, 35f), buttonCText))
		{
			if (buttonCAction != null)
			{
				buttonCAction();
			}
			if (buttonCClose)
			{
				Close();
			}
		}
	}

	public override void OnCancelKeyPressed()
	{
		if (cancelAction != null)
		{
			cancelAction();
			Close();
			Event.current.Use();
		}
		else
		{
			base.OnCancelKeyPressed();
		}
	}

	public override void OnAcceptKeyPressed()
	{
		if (acceptAction != null)
		{
			acceptAction();
			Close();
			Event.current.Use();
		}
		else
		{
			base.OnAcceptKeyPressed();
		}
	}
}
