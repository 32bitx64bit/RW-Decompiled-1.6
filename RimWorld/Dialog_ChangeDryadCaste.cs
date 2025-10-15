using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_ChangeDryadCaste : Window
{
	private CompTreeConnection treeConnection;

	private Pawn connectedPawn;

	private Vector2 scrollPosition;

	private GauranlenTreeModeDef selectedMode;

	private GauranlenTreeModeDef currentMode;

	private float rightViewWidth;

	private List<GauranlenTreeModeDef> allDryadModes;

	private const float HeaderHeight = 35f;

	private const float LeftRectWidth = 400f;

	private const float OptionSpacing = 52f;

	private const float ChangeFormButtonHeight = 55f;

	private static readonly Vector2 OptionSize = new Vector2(190f, 46f);

	private static readonly Vector2 ButSize = new Vector2(200f, 40f);

	public override Vector2 InitialSize => new Vector2((float)Mathf.Min(900, UI.screenWidth), 650f);

	private PawnKindDef SelectedKind => selectedMode.pawnKindDef;

	public Dialog_ChangeDryadCaste(Thing tree)
	{
		treeConnection = tree.TryGetComp<CompTreeConnection>();
		currentMode = treeConnection.desiredMode;
		selectedMode = currentMode;
		connectedPawn = treeConnection.ConnectedPawn;
		forcePause = true;
		closeOnAccept = false;
		doCloseX = true;
		doCloseButton = true;
		allDryadModes = DefDatabase<GauranlenTreeModeDef>.AllDefs.ToList();
	}

	public override void PreOpen()
	{
		if (!ModLister.CheckIdeology("Dryad upgrades"))
		{
			Close();
		}
		base.PreOpen();
		SetupView();
	}

	private void SetupView()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		foreach (GauranlenTreeModeDef allDryadMode in allDryadModes)
		{
			rightViewWidth = Mathf.Max(rightViewWidth, GetPosition(allDryadMode, InitialSize.y).x + OptionSize.x);
		}
		rightViewWidth += 20f;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		string label = ((selectedMode != null) ? selectedMode.LabelCap : "ChangeMode".Translate());
		Widgets.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 35f), label);
		Text.Font = GameFont.Small;
		float num = ((Rect)(ref inRect)).y + 35f + 10f;
		float curY = num;
		float num2 = ((Rect)(ref inRect)).height - num;
		num2 -= ButSize.y + 10f;
		DrawLeftRect(new Rect(((Rect)(ref inRect)).xMin, num, 400f, num2), ref curY);
		DrawRightRect(new Rect(((Rect)(ref inRect)).x + 400f + 17f, num, ((Rect)(ref inRect)).width - 400f - 17f, num2));
	}

	private void DrawLeftRect(Rect rect, ref float curY)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect)).yMax;
		Rect rect3 = rect2.ContractedBy(4f);
		if (selectedMode == null)
		{
			Widgets.Label(rect3, "ChooseProductionModeInitialDesc".Translate(connectedPawn.Named("PAWN"), treeConnection.parent.Named("TREE"), ThingDefOf.DryadCocoon.GetCompProperties<CompProperties_DryadCocoon>().daysToComplete.Named("UPGRADEDURATION")));
			return;
		}
		Widgets.Label(((Rect)(ref rect3)).x, ref curY, ((Rect)(ref rect3)).width, selectedMode.Description);
		curY += 10f;
		if (!Find.IdeoManager.classicMode && !selectedMode.requiredMemes.NullOrEmpty())
		{
			Widgets.Label(((Rect)(ref rect3)).x, ref curY, ((Rect)(ref rect3)).width, "RequiredMemes".Translate() + ":");
			string text = "";
			for (int i = 0; i < selectedMode.requiredMemes.Count; i++)
			{
				MemeDef memeDef = selectedMode.requiredMemes[i];
				if (!text.NullOrEmpty())
				{
					text += "\n";
				}
				text = text + "  - " + memeDef.LabelCap.ToString().Colorize(connectedPawn.Ideo.HasMeme(memeDef) ? Color.white : ColorLibrary.RedReadable);
			}
			Widgets.Label(((Rect)(ref rect3)).x, ref curY, ((Rect)(ref rect3)).width, text);
			curY += 10f;
		}
		if (selectedMode.previousStage != null)
		{
			Widgets.Label(((Rect)(ref rect3)).x, ref curY, ((Rect)(ref rect3)).width, string.Concat("RequiredStage".Translate(), ": ", selectedMode.previousStage.pawnKindDef.LabelCap.ToString().Colorize(Color.white)));
			curY += 10f;
		}
		if (selectedMode.displayedStats != null)
		{
			for (int j = 0; j < selectedMode.displayedStats.Count; j++)
			{
				StatDef statDef = selectedMode.displayedStats[j];
				Widgets.Label(((Rect)(ref rect3)).x, ref curY, ((Rect)(ref rect3)).width, statDef.LabelCap + ": " + statDef.ValueToString(SelectedKind.race.GetStatValueAbstract(statDef), statDef.toStringNumberSense));
			}
			curY += 10f;
		}
		if (selectedMode.hyperlinks != null)
		{
			foreach (Dialog_InfoCard.Hyperlink item in Dialog_InfoCard.DefsToHyperlinks(selectedMode.hyperlinks))
			{
				Widgets.HyperlinkWithIcon(new Rect(((Rect)(ref rect3)).x, curY, ((Rect)(ref rect3)).width, Text.LineHeight), item);
				curY += Text.LineHeight;
			}
			curY += 10f;
		}
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).x, ((Rect)(ref rect3)).yMax - 55f, ((Rect)(ref rect3)).width, 55f);
		if (MeetsRequirements(selectedMode) && selectedMode != currentMode)
		{
			if (Widgets.ButtonText(rect4, "Accept".Translate()))
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("GauranlenModeChangeDescFull".Translate(treeConnection.parent.Named("TREE"), connectedPawn.Named("CONNECTEDPAWN"), ThingDefOf.DryadCocoon.GetCompProperties<CompProperties_DryadCocoon>().daysToComplete.Named("DURATION")), delegate
				{
					StartChange();
				});
				Find.WindowStack.Add(window);
			}
		}
		else
		{
			string label = ((selectedMode == currentMode) ? ((string)"AlreadySelected".Translate()) : ((!MeetsMemeRequirements(selectedMode)) ? ((string)"MissingRequiredMemes".Translate()) : ((selectedMode.previousStage == null || currentMode == selectedMode.previousStage) ? ((string)"Locked".Translate()) : ((string)("Locked".Translate() + ": " + "MissingRequiredCaste".Translate())))));
			Text.Anchor = (TextAnchor)4;
			Widgets.DrawHighlight(rect4);
			Widgets.Label(rect4.ContractedBy(5f), label);
			Text.Anchor = (TextAnchor)0;
		}
	}

	private void StartChange()
	{
		treeConnection.desiredMode = selectedMode;
		SoundDefOf.GauranlenProductionModeSet.PlayOneShotOnCamera();
		Close(doCloseSound: false);
	}

	private void DrawRightRect(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, rightViewWidth, ((Rect)(ref rect)).height - 16f);
		Rect val2 = val.ContractedBy(10f);
		Widgets.ScrollHorizontal(rect, ref scrollPosition, val);
		Widgets.BeginScrollView(rect, ref scrollPosition, val);
		Widgets.BeginGroup(val2);
		DrawDependencyLines(val2);
		foreach (GauranlenTreeModeDef allDryadMode in allDryadModes)
		{
			DrawDryadStage(val2, allDryadMode);
		}
		Widgets.EndGroup();
		Widgets.EndScrollView();
	}

	private bool MeetsMemeRequirements(GauranlenTreeModeDef stage)
	{
		if (!Find.IdeoManager.classicMode && !stage.requiredMemes.NullOrEmpty())
		{
			foreach (MemeDef requiredMeme in stage.requiredMemes)
			{
				if (!connectedPawn.Ideo.HasMeme(requiredMeme))
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool MeetsRequirements(GauranlenTreeModeDef mode)
	{
		if (mode.previousStage != null && currentMode != mode.previousStage)
		{
			return false;
		}
		return MeetsMemeRequirements(mode);
	}

	private Color GetBoxColor(GauranlenTreeModeDef mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Color val = TexUI.AvailResearchColor;
		if (mode == currentMode)
		{
			val = TexUI.OldActiveResearchColor;
		}
		else if (!MeetsRequirements(mode))
		{
			val = TexUI.LockedResearchColor;
		}
		if (selectedMode == mode)
		{
			val += TexUI.HighlightBgResearchColor;
		}
		return val;
	}

	private Color GetBoxOutlineColor(GauranlenTreeModeDef mode)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (selectedMode != null && selectedMode == mode)
		{
			return TexUI.HighlightBorderResearchColor;
		}
		return TexUI.DefaultBorderResearchColor;
	}

	private Color GetTextColor(GauranlenTreeModeDef mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (!MeetsRequirements(mode))
		{
			return ColorLibrary.RedReadable;
		}
		return Color.white;
	}

	private void DrawDependencyLines(Rect fullRect)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		foreach (GauranlenTreeModeDef allDryadMode in allDryadModes)
		{
			if (allDryadMode.previousStage != null)
			{
				DrawLineBetween(allDryadMode, allDryadMode.previousStage, ((Rect)(ref fullRect)).height, TexUI.DefaultLineResearchColor);
			}
		}
		foreach (GauranlenTreeModeDef allDryadMode2 in allDryadModes)
		{
			if (allDryadMode2.previousStage != null && (allDryadMode2.previousStage == selectedMode || selectedMode == allDryadMode2))
			{
				DrawLineBetween(allDryadMode2, allDryadMode2.previousStage, ((Rect)(ref fullRect)).height, TexUI.HighlightLineResearchColor, 3f);
			}
		}
	}

	private void DrawDryadStage(Rect rect, GauranlenTreeModeDef stage)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = GetPosition(stage, ((Rect)(ref rect)).height);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(position.x, position.y, OptionSize.x, OptionSize.y);
		Widgets.DrawBoxSolidWithOutline(val, GetBoxColor(stage), GetBoxOutlineColor(stage));
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).height, ((Rect)(ref val)).height);
		Widgets.DefIcon(rect2.ContractedBy(4f), stage.pawnKindDef);
		GUI.color = GetTextColor(stage);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(GenUI.ContractedBy(new Rect(((Rect)(ref rect2)).xMax, ((Rect)(ref val)).y, ((Rect)(ref val)).width - ((Rect)(ref rect2)).width, ((Rect)(ref val)).height), 4f), stage.LabelCap);
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
		if (Widgets.ButtonInvisible(val))
		{
			selectedMode = stage;
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
	}

	private void DrawLineBetween(GauranlenTreeModeDef left, GauranlenTreeModeDef right, float height, Color color, float width = 2f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector2 start = GetPosition(left, height) + new Vector2(5f, OptionSize.y / 2f);
		Vector2 end = GetPosition(right, height) + OptionSize / 2f;
		Widgets.DrawLine(start, end, color, width);
	}

	private Vector2 GetPosition(GauranlenTreeModeDef stage, float height)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(stage.drawPosition.x * OptionSize.x + stage.drawPosition.x * 52f, (height - OptionSize.y) * stage.drawPosition.y);
	}
}
