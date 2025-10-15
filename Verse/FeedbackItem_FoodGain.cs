using UnityEngine;

namespace Verse;

public class FeedbackItem_FoodGain : FeedbackItem
{
	protected int Amount;

	public FeedbackItem_FoodGain(Vector2 ScreenPos, int Amount)
		: base(ScreenPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		this.Amount = Amount;
	}

	public override void FeedbackOnGUI()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string str = Amount + " food";
		DrawFloatingText(str, Color.yellow);
	}
}
