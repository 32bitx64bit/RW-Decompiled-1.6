using UnityEngine;

namespace Verse;

public class CreditRecord_Text : CreditsEntry
{
	public string text;

	public TextAnchor anchor;

	public CreditRecord_Text()
	{
	}

	public CreditRecord_Text(string text, TextAnchor anchor = 0)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		this.text = text;
		this.anchor = anchor;
	}

	public override float DrawHeight(float width)
	{
		return Text.CalcHeight(text, width);
	}

	public override void Draw(Rect r)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Text.Anchor = anchor;
		Text.Font = GameFont.Medium;
		Widgets.Label(r, text);
		Text.Anchor = (TextAnchor)0;
	}
}
