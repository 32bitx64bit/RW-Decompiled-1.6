using System;
using UnityEngine;

namespace Verse;

public class Dialog_Slider : Window
{
	public Func<int, string> textGetter;

	public int from;

	public int to;

	public float roundTo = 1f;

	public float extraBottomSpace;

	private Action<int> confirmAction;

	private int curValue;

	private const float BotAreaHeight = 30f;

	private const float NumberYOffset = 10f;

	public override Vector2 InitialSize => new Vector2(300f, 130f + extraBottomSpace);

	protected override float Margin => 10f;

	public Dialog_Slider(Func<int, string> textGetter, int from, int to, Action<int> confirmAction, int startingValue = int.MinValue, float roundTo = 1f)
	{
		this.textGetter = textGetter;
		this.from = from;
		this.to = to;
		this.confirmAction = confirmAction;
		this.roundTo = roundTo;
		forcePause = true;
		closeOnClickedOutside = true;
		if (startingValue == int.MinValue)
		{
			curValue = from;
		}
		else
		{
			curValue = startingValue;
		}
	}

	public Dialog_Slider(string text, int from, int to, Action<int> confirmAction, int startingValue = int.MinValue, float roundTo = 1f)
		: this((int val) => string.Format(text, val), from, to, confirmAction, startingValue, roundTo)
	{
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		string text = textGetter(curValue);
		float num = Text.CalcHeight(text, ((Rect)(ref inRect)).width);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, num);
		Text.Anchor = (TextAnchor)1;
		Widgets.Label(rect, text);
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y + ((Rect)(ref rect)).height + 10f, ((Rect)(ref inRect)).width, 30f);
		curValue = (int)Widgets.HorizontalSlider(rect2, curValue, from, to, middleAlignment: true, null, null, null, roundTo);
		GUI.color = ColoredText.SubtleGrayColor;
		Text.Font = GameFont.Tiny;
		Widgets.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref rect2)).yMax - 10f, ((Rect)(ref inRect)).width / 2f, Text.LineHeight), from.ToString());
		Text.Anchor = (TextAnchor)2;
		Widgets.Label(new Rect(((Rect)(ref inRect)).x + ((Rect)(ref inRect)).width / 2f, ((Rect)(ref rect2)).yMax - 10f, ((Rect)(ref inRect)).width / 2f, Text.LineHeight), to.ToString());
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		float num2 = (((Rect)(ref inRect)).width - 10f) / 2f;
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - 30f, num2, 30f), "CancelButton".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x + num2 + 10f, ((Rect)(ref inRect)).yMax - 30f, num2, 30f), "OK".Translate()))
		{
			Close();
			confirmAction(curValue);
		}
	}
}
