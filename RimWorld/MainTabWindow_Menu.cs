using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class MainTabWindow_Menu : MainTabWindow
{
	private bool anyGameFiles;

	public override Vector2 RequestedTabSize => new Vector2(450f, 390f);

	public override MainTabWindowAnchor Anchor => MainTabWindowAnchor.Right;

	public MainTabWindow_Menu()
	{
		forcePause = true;
		layer = WindowLayer.Super;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		PlayerKnowledgeDatabase.Save();
		ShipCountdown.CancelCountdown();
		if (ModsConfig.IdeologyActive)
		{
			ArchonexusCountdown.CancelCountdown();
		}
		anyGameFiles = GenFilePaths.AllSavedGameFiles.Any();
	}

	public override void ExtraOnGUI()
	{
		base.ExtraOnGUI();
		VersionControl.DrawInfoInCorner();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MainMenuDrawer.DoMainMenuControls(rect, anyGameFiles);
	}
}
