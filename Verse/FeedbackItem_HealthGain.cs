using UnityEngine;

namespace Verse;

public class FeedbackItem_HealthGain : FeedbackItem
{
	protected Pawn Healer;

	protected int Amount;

	public FeedbackItem_HealthGain(Vector2 ScreenPos, int Amount, Pawn Healer)
		: base(ScreenPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		this.Amount = Amount;
		this.Healer = Healer;
	}

	public override void FeedbackOnGUI()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		text = ((Amount < 0) ? "-" : "+");
		text += Amount;
		DrawFloatingText(text, Color.red);
	}
}
