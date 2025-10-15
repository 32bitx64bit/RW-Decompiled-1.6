using System;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Dialog_NodeTree : Window
{
	private Vector2 scrollPosition;

	private Vector2 optsScrollPosition;

	protected string title;

	protected DiaNode curNode;

	public Action closeAction;

	private float makeInteractiveAtTime;

	public Color screenFillColor = Color.clear;

	protected float minOptionsAreaHeight;

	private const float InteractivityDelay = 1f;

	private const float TitleHeight = 36f;

	protected const float OptHorMargin = 15f;

	protected const float OptVerticalSpace = 7f;

	private const int ResizeIfMoreOptionsThan = 5;

	private const float MinSpaceLeftForTextAfterOptionsResizing = 100f;

	private float optTotalHeight;

	public override Vector2 InitialSize
	{
		get
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int num = 480;
			if (curNode.options.Count > 5)
			{
				Text.Font = GameFont.Small;
				num += (curNode.options.Count - 5) * (int)(Text.LineHeight + 7f);
			}
			return new Vector2(620f, (float)Mathf.Min(num, UI.screenHeight));
		}
	}

	private bool InteractiveNow => Time.realtimeSinceStartup >= makeInteractiveAtTime;

	public Dialog_NodeTree(DiaNode nodeRoot, bool delayInteractivity = false, bool radioMode = false, string title = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this.title = title;
		GotoNode(nodeRoot);
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		closeOnCancel = false;
		if (delayInteractivity)
		{
			makeInteractiveAtTime = RealTime.LastRealTime + 1f;
		}
		soundAppear = SoundDefOf.CommsWindow_Open;
		soundClose = SoundDefOf.CommsWindow_Close;
		if (radioMode)
		{
			soundAmbient = SoundDefOf.RadioComms_Ambience;
		}
	}

	public override void PreClose()
	{
		base.PreClose();
		curNode.PreClose();
	}

	public override void PostClose()
	{
		base.PostClose();
		if (closeAction != null)
		{
			closeAction();
		}
	}

	public override void WindowOnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (screenFillColor != Color.clear)
		{
			GUI.color = screenFillColor;
			GUI.DrawTexture(new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight), (Texture)(object)BaseContent.WhiteTex);
			GUI.color = Color.white;
		}
		base.WindowOnGUI();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Rect val = inRect.AtZero();
		if (title != null)
		{
			Text.Font = GameFont.Small;
			Rect rect = val;
			((Rect)(ref rect)).height = 36f;
			((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 53f;
			Widgets.DrawTitleBG(rect);
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 9f;
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 5f;
			Widgets.Label(rect, title);
		}
		DrawNode(val);
	}

	protected void DrawNode(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Invalid comparison between Unknown and I4
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Small;
		float num = Mathf.Min(optTotalHeight, ((Rect)(ref rect)).height - 100f - Margin * 2f);
		Rect outRect = new Rect(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - Mathf.Max(num, minOptionsAreaHeight));
		float num2 = ((Rect)(ref rect)).width - 16f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, num2, Text.CalcHeight(curNode.text, num2));
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		Widgets.Label(val, curNode.text.Resolve());
		Widgets.EndScrollView();
		Widgets.BeginScrollView(new Rect(0f, ((Rect)(ref rect)).height - num, ((Rect)(ref rect)).width, num), ref optsScrollPosition, new Rect(0f, 0f, ((Rect)(ref rect)).width - 16f, optTotalHeight));
		float num3 = 0f;
		float num4 = 0f;
		Rect rect2 = default(Rect);
		for (int i = 0; i < curNode.options.Count; i++)
		{
			((Rect)(ref rect2))._002Ector(15f, num3, ((Rect)(ref rect)).width - 30f, 999f);
			float num5 = curNode.options[i].OptOnGUI(rect2, InteractiveNow);
			num3 += num5 + 7f;
			num4 += num5 + 7f;
		}
		if ((int)Event.current.type == 8)
		{
			optTotalHeight = num4;
		}
		Widgets.EndScrollView();
		Widgets.EndGroup();
	}

	public void GotoNode(DiaNode node)
	{
		foreach (DiaOption option in node.options)
		{
			option.dialog = this;
		}
		curNode = node;
	}
}
