using UnityEngine;

namespace RimWorld;

public class MainTabWindow_Factions : MainTabWindow
{
	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private Faction scrollToFaction;

	public override void PreOpen()
	{
		base.PreOpen();
		scrollToFaction = null;
	}

	public void ScrollToFaction(Faction faction)
	{
		scrollToFaction = faction;
	}

	public override void DoWindowContents(Rect fillRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		FactionUIUtility.DoWindowContents(fillRect, ref scrollPosition, ref scrollViewHeight, scrollToFaction);
		if (scrollToFaction != null)
		{
			scrollToFaction = null;
		}
	}
}
