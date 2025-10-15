using UnityEngine;

namespace Verse.Steam;

public class Dialog_WorkshopOperationInProgress : Window
{
	public override Vector2 InitialSize => new Vector2(600f, 400f);

	public Dialog_WorkshopOperationInProgress()
	{
		forcePause = true;
		closeOnAccept = false;
		closeOnCancel = false;
		absorbInputAroundWindow = true;
		preventDrawTutor = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Workshop.GetUpdateStatus(out var updateStatus, out var progPercent);
		WorkshopInteractStage curStage = Workshop.CurStage;
		if (curStage == WorkshopInteractStage.None && (int)updateStatus == 0)
		{
			Close();
			return;
		}
		string text = "";
		if (curStage != 0)
		{
			text += curStage.GetLabel();
			text += "\n\n";
		}
		if ((int)updateStatus != 0)
		{
			text += updateStatus.GetLabel();
			if (progPercent > 0f)
			{
				text = text + " (" + progPercent.ToStringPercent() + ")";
			}
			text += GenText.MarchingEllipsis();
		}
		Widgets.Label(inRect, text);
	}

	public static void CloseAll()
	{
		Find.WindowStack.WindowOfType<Dialog_WorkshopOperationInProgress>()?.Close();
	}
}
