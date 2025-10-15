using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_IdeosDuringLanding : Window
{
	private Vector2 scrollPosition_ideoList;

	private float scrollViewHeight_ideoList;

	private Vector2 scrollPosition_ideoDetails;

	private float scrollViewHeight_ideoDetails;

	public override Vector2 InitialSize => new Vector2(1010f, Mathf.Min(1000f, (float)UI.screenHeight));

	public Dialog_IdeosDuringLanding()
	{
		doCloseButton = true;
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		IdeoUIUtility.DoIdeoListAndDetails(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - Window.CloseButSize.y), ref scrollPosition_ideoList, ref scrollViewHeight_ideoList, ref scrollPosition_ideoDetails, ref scrollViewHeight_ideoDetails, editMode: false, showCreateIdeoButton: false, null, null, null, forArchonexusRestart: false, null, null, showLoadExistingIdeoBtn: false, allowLoad: false);
	}
}
