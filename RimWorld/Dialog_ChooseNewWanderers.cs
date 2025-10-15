using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_ChooseNewWanderers : Window
{
	private int curPawnIndex;

	private int generationIndex;

	private const float TitleAreaHeight = 45f;

	private const float PawnEntryHeight = 60f;

	private const float SkillSummaryHeight = 127f;

	private const float CrossIconSize = 15f;

	private const float TabAreaWidth = 140f;

	private static readonly Vector2 PawnSelectorPortraitSize = new Vector2(70f, 110f);

	private static readonly Vector2 ConfirmButtonSize = new Vector2(150f, 38f);

	private const int DefaultPawnCount = 3;

	private const int MinPawnCount = 1;

	private const int MaxPawnCount = 6;

	private static readonly FloatRange ExcludeBiologicalAgeRange = new FloatRange(12.1f, 13f);

	public override Vector2 InitialSize => new Vector2(1020f, 764f);

	private static List<Pawn> StartingAndOptionalPawns => Find.GameInitData.startingAndOptionalPawns;

	private static PawnGenerationRequest DefaultStartingPawnRequest
	{
		get
		{
			PawnKindDef basicMemberKind = Faction.OfPlayer.def.basicMemberKind;
			Faction ofPlayer = Faction.OfPlayer;
			FloatRange? excludeBiologicalAgeRange = (ModsConfig.BiotechActive ? new FloatRange?(ExcludeBiologicalAgeRange) : null);
			return new PawnGenerationRequest(basicMemberKind, ofPlayer, PawnGenerationContext.PlayerStarter, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 50f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: true, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, forceNoIdeo: false, forceNoBackstory: false, forbidAnyTitle: false, forceDead: false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, excludeBiologicalAgeRange);
		}
	}

	public Dialog_ChooseNewWanderers()
	{
		doCloseX = true;
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		Current.Game.InitData = new GameInitData();
		Find.GameInitData.startingPawnCount = 3;
		StartingPawnUtility.ClearAllStartingPawns();
		generationIndex = 0;
		while (StartingAndOptionalPawns.Count < 3)
		{
			StartingPawnUtility.SetGenerationRequest(generationIndex, DefaultStartingPawnRequest);
			StartingPawnUtility.AddNewPawn(generationIndex);
			generationIndex++;
		}
		curPawnIndex = 0;
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsOpen)
		{
			return;
		}
		Rect val = rect;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref val)).width, 45f), "ChooseNewWanderers".Translate());
		Text.Font = GameFont.Small;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 45f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width - ConfirmButtonSize.x, ((Rect)(ref rect)).height - ConfirmButtonSize.y, ConfirmButtonSize.x, ConfirmButtonSize.y);
		if (Widgets.ButtonText(rect2, "Confirm".Translate()))
		{
			IncidentParms incidentParms = new IncidentParms();
			incidentParms.target = Find.AnyPlayerHomeMap;
			incidentParms.forced = true;
			Find.Storyteller.TryFire(new FiringIncident(IncidentDefOf.GameEndedWanderersJoin, null, incidentParms));
			Find.LetterStack.RemoveLetter(Find.LetterStack.LettersListForReading.Find((Letter letter) => letter.def == LetterDefOf.GameEnded));
			Find.GameEnder.gameEnding = false;
			Find.GameEnder.newWanderersCreatedTick = Find.TickManager.TicksGame;
			Close();
			Current.Game.InitData = null;
		}
		else
		{
			((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - (((Rect)(ref rect2)).height + 10f);
			Rect rect3 = val;
			((Rect)(ref rect3)).width = 140f;
			DrawPawnList(rect3);
			Rect rect4 = val;
			((Rect)(ref rect4)).xMin = ((Rect)(ref rect4)).xMin + 150f;
			Rect rect5 = rect4.BottomPartPixels(127f);
			((Rect)(ref rect4)).yMax = ((Rect)(ref rect5)).yMin - 10f;
			StartingPawnUtility.DrawPortraitArea(rect4, curPawnIndex, renderClothes: true, renderHeadgear: true);
			StartingPawnUtility.DrawSkillSummaries(rect5);
		}
	}

	private void DrawPawnList(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).height = 60f;
		rect2 = rect2.ContractedBy(4f);
		for (int i = 0; i < StartingAndOptionalPawns.Count; i++)
		{
			DoPawnRow(rect2, i);
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 60f;
		}
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 4f;
		if (StartingAndOptionalPawns.Count < 6 && Widgets.ButtonText(new Rect(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width, 25f), "+"))
		{
			Find.GameInitData.startingPawnCount++;
			StartingPawnUtility.AddNewPawn(generationIndex);
			generationIndex++;
		}
	}

	private void DoPawnRow(Rect rect, int index)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		Pawn pawn = StartingAndOptionalPawns[index];
		Widgets.DrawOptionBackground(rect, curPawnIndex == index);
		MouseoverSounds.DoRegion(rect);
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero().ContractedBy(4f);
		GUI.color = new Color(1f, 1f, 1f, 0.2f);
		GUI.DrawTexture(new Rect(110f - PawnSelectorPortraitSize.x / 2f, 40f - PawnSelectorPortraitSize.y / 2f, PawnSelectorPortraitSize.x, PawnSelectorPortraitSize.y), (Texture)(object)PortraitsCache.Get(pawn, PawnSelectorPortraitSize, Rot4.South));
		GUI.color = Color.white;
		Widgets.Label(label: (!(pawn.Name is NameTriple nameTriple)) ? pawn.LabelShort : (string.IsNullOrEmpty(nameTriple.Nick) ? nameTriple.First : nameTriple.Nick), rect: val.TopPart(0.5f).Rounded());
		if (Text.CalcSize(pawn.story.TitleCap).x > ((Rect)(ref val)).width)
		{
			Widgets.Label(val.BottomPart(0.5f).Rounded(), pawn.story.TitleShortCap);
		}
		else
		{
			Widgets.Label(val.BottomPart(0.5f).Rounded(), pawn.story.TitleCap);
		}
		if (Mouse.IsOver(val) && StartingAndOptionalPawns.Count > 1 && Widgets.ButtonImage(new Rect(((Rect)(ref val)).xMax - 15f, ((Rect)(ref val)).y, 15f, 15f), TexButton.Delete))
		{
			Find.GameInitData.startingPawnCount--;
			StartingAndOptionalPawns.Remove(pawn);
			curPawnIndex = Math.Min(StartingAndOptionalPawns.Count - 1, curPawnIndex);
		}
		if (Widgets.ButtonInvisible(val))
		{
			curPawnIndex = index;
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
		}
		Widgets.EndGroup();
	}

	public override void PostClose()
	{
		base.PostClose();
		if (Find.GameEnder.gameEnding)
		{
			StartingPawnUtility.ClearAllStartingPawns();
		}
	}
}
