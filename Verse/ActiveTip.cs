using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class ActiveTip
{
	public TipSignal signal;

	public double firstTriggerTime;

	public int lastTriggerFrame;

	private const int TipMargin = 4;

	private const float MaxWidth = 260f;

	public static readonly Texture2D TooltipBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/TooltipBG");

	private string FinalText
	{
		get
		{
			string text;
			if (signal.textGetter != null)
			{
				try
				{
					text = signal.textGetter();
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
					text = "Error getting tip text.";
				}
			}
			else
			{
				text = signal.text;
			}
			return text.TrimEnd();
		}
	}

	public Rect TipRect
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			string finalText = FinalText;
			Vector2 val = Text.CalcSize(finalText);
			if (val.x > 260f)
			{
				val.x = 260f;
				val.y = Text.CalcHeight(finalText, val.x);
			}
			return GenUI.ContractedBy(new Rect(0f, 0f, val.x, val.y), -4f).RoundedCeil();
		}
	}

	public ActiveTip(TipSignal signal)
	{
		this.signal = signal;
	}

	public ActiveTip(ActiveTip cloneSource)
	{
		signal = cloneSource.signal;
		firstTriggerTime = cloneSource.firstTriggerTime;
		lastTriggerFrame = cloneSource.lastTriggerFrame;
	}

	public float DrawTooltip(Vector2 pos)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		string finalText = FinalText;
		Rect bgRect = TipRect;
		((Rect)(ref bgRect)).position = pos;
		if (!LongEventHandler.AnyEventWhichDoesntUseStandardWindowNowOrWaiting)
		{
			Find.WindowStack.ImmediateWindow(153 * signal.uniqueId + 62346, bgRect, WindowLayer.Super, delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				DrawInner(bgRect.AtZero(), finalText);
			}, doBackground: false);
		}
		else
		{
			Widgets.DrawShadowAround(bgRect);
			Widgets.DrawWindowBackground(bgRect);
			DrawInner(bgRect, finalText);
		}
		return ((Rect)(ref bgRect)).height;
	}

	private void DrawInner(Rect bgRect, string label)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawAtlas(bgRect, TooltipBGAtlas);
		Text.Font = GameFont.Small;
		Widgets.Label(bgRect.ContractedBy(4f), label);
	}
}
