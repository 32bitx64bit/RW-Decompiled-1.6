using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_FactionDuringLanding : Window
{
	private Vector2 scrollPosition = Vector2.zero;

	private float scrollViewHeight;

	public override Vector2 InitialSize => new Vector2(1010f, 684f);

	public Dialog_FactionDuringLanding()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		doCloseButton = true;
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		FactionUIUtility.DoWindowContents(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - Window.CloseButSize.y), ref scrollPosition, ref scrollViewHeight);
	}
}
