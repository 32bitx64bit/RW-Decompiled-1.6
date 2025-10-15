using UnityEngine;
using Verse;

namespace RimWorld;

public class MainTabWindow_Ideos : MainTabWindow
{
	private Vector2 scrollPosition_ideoList;

	private float scrollViewHeight_ideoList;

	private Vector2 scrollPosition_ideoDetails;

	private float scrollViewHeight_ideoDetails;

	public override Vector2 InitialSize => new Vector2(base.InitialSize.x, (float)(UI.screenHeight - 35));

	public override void PreOpen()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.PreOpen();
		scrollPosition_ideoDetails = Vector2.zero;
	}

	public override void PostClose()
	{
		base.PostClose();
		IdeoUIUtility.UnselectCurrent();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		IdeoUIUtility.DoIdeoListAndDetails(rect, ref scrollPosition_ideoList, ref scrollViewHeight_ideoList, ref scrollPosition_ideoDetails, ref scrollViewHeight_ideoDetails, editMode: false, showCreateIdeoButton: false, null, null, null, forArchonexusRestart: false, null, null, showLoadExistingIdeoBtn: false, allowLoad: false);
	}
}
