using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LudeonTK;

[StaticConstructorOnStartup]
public class Dialog_DevInfectionPathways : Window_Dev
{
	private Vector2 windowPosition;

	private Vector2 scroll;

	private float lastHeight;

	private const string Title = "Infection Pathway Debugger";

	private const float ButtonHeight = 30f;

	public override bool IsDebug => true;

	protected override float Margin => 4f;

	public override Vector2 InitialSize => new Vector2(230f, 330f);

	public Dialog_DevInfectionPathways()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		draggable = true;
		focusWhenOpened = false;
		drawShadow = false;
		closeOnAccept = false;
		closeOnCancel = false;
		preventCameraMotion = false;
		drawInScreenshotMode = false;
		windowPosition = Prefs.DevPalettePosition;
		onlyDrawInDevMode = true;
		doCloseX = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 24f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).height / 2f - 12f, ((Rect)(ref inRect)).width, 24f);
		DevGUI.Label(rect, "Infection Pathway Debugger");
		Text.Font = GameFont.Tiny;
		List<Pawn> selectedPawns = Find.Selector.SelectedPawns;
		if (selectedPawns.Count == 0 || !selectedPawns[0].RaceProps.Humanlike)
		{
			DevGUI.Label(rect2, "No valid humanlike selected");
			return;
		}
		Pawn pawn = Find.Selector.SelectedPawns[0];
		if (pawn.infectionVectors.PathwaysCount == 0)
		{
			DevGUI.Label(rect2, "No vectors");
			return;
		}
		Rect outRect = inRect;
		((Rect)(ref outRect)).yMin = ((Rect)(ref rect)).height + 6f;
		Rect val = inRect;
		((Rect)(ref val)).y = 0f;
		((Rect)(ref val)).height = lastHeight;
		Widgets.BeginScrollView(outRect, ref scroll, val);
		float y = 0f;
		int num = 0;
		PrintLabel("Def name", "Age", 1, val, ref y);
		foreach (InfectionPathway pathway in pawn.infectionVectors.Pathways)
		{
			PrintLabel(pathway, num++, val, ref y);
		}
		lastHeight = y;
		Widgets.EndScrollView();
	}

	private void PrintLabel(InfectionPathway pathway, int row, Rect container, ref float y)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		PrintLabel(pathway.Def.label, pathway.AgeTicks.ToStringSecondsFromTicks(), row, container, ref y);
	}

	private void PrintLabel(string key, string value, int row, Rect container, ref float y)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref container)).x, y, ((Rect)(ref container)).width, 20f);
		float num = ((Rect)(ref container)).width * 0.6f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref container)).x, y, num, 20f);
		Rect rect3 = new Rect(((Rect)(ref container)).x + num, y, num, 20f);
		if (row % 2 == 0)
		{
			DevGUI.DrawLightHighlight(rect);
		}
		DevGUI.Label(rect2, key);
		DevGUI.Label(rect3, value);
		y += 20f;
	}
}
