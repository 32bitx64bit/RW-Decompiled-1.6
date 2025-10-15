using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Page_ConfigureIdeo : Page
{
	public Ideo ideo;

	protected Vector2 scrollPosition_ideoList;

	protected float scrollViewHeight_ideoList;

	protected Vector2 scrollPosition_ideoDetails;

	protected float scrollViewHeight_ideoDetails;

	public override string PageTitle => "CustomizeIdeoligion".Translate();

	public Page_ConfigureIdeo()
	{
		grayOutIfOtherDialogOpen = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		Find.IdeoManager.RemoveUnusedStartingIdeos();
		if (IdeoUIUtility.selected != null && Find.IdeoManager.IdeosListForReading.Contains(IdeoUIUtility.selected))
		{
			SelectOrMakeNewIdeo(IdeoUIUtility.selected);
		}
		else
		{
			ideo = null;
			IdeoUIUtility.UnselectCurrent();
		}
		TutorSystem.Notify_Event("PageStart-ConfigureIdeo");
	}

	public void SelectOrMakeNewIdeo(Ideo newIdeo = null)
	{
		ideo = newIdeo ?? IdeoUtility.MakeEmptyIdeo();
		if (!Find.IdeoManager.IdeosListForReading.Contains(ideo))
		{
			Find.IdeoManager.Add(ideo);
			Faction.OfPlayer.ideos.SetPrimary(ideo);
		}
		if (!ideo.memes.Any())
		{
			Find.WindowStack.Add(new Dialog_ChooseMemes(ideo, MemeCategory.Structure, initialSelection: true));
		}
		IdeoUIUtility.SetSelected(ideo);
		LessonAutoActivator.TeachOpportunity(ConceptDefOf.EditingMemes, OpportunityType.Critical);
		LessonAutoActivator.TeachOpportunity(ConceptDefOf.EditingPrecepts, OpportunityType.Critical);
	}

	public virtual void DoIdeos(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		IdeoUIUtility.DoIdeoListAndDetails(GetMainRect(rect), ref scrollPosition_ideoList, ref scrollViewHeight_ideoList, ref scrollPosition_ideoDetails, ref scrollViewHeight_ideoDetails, editMode: true, !IdeoUIUtility.PlayerPrimaryIdeoNotShared, null, null, null, forArchonexusRestart: false, null, null, showLoadExistingIdeoBtn: true);
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		DrawPageTitle(rect);
		DoIdeos(rect);
		if (ideo != null)
		{
			string text = null;
			Pair<Precept, Precept> pair = ideo.FirstIncompatiblePreceptPair();
			if (pair != default(Pair<Precept, Precept>))
			{
				string text2 = pair.First.TipLabel;
				string text3 = pair.Second.TipLabel;
				if (text2 == text3)
				{
					text2 = pair.First.UIInfoSecondLine;
					text3 = pair.Second.UIInfoSecondLine;
				}
				text = "MessageIdeoIncompatiblePrecepts".Translate(text2.Named("PRECEPT1"), text3.Named("PRECEPT2")).CapitalizeFirst();
			}
			else
			{
				Tuple<Precept_Ritual, List<string>> tuple = ideo.FirstRitualMissingTarget();
				Precept_Building precept_Building = ideo.FirstConsumableBuildingMissingRitual();
				if (tuple != null)
				{
					text = "MessageRitualMissingTarget".Translate(tuple.Item1.LabelCap.Named("PRECEPT")) + ": " + tuple.Item2.ToCommaList().CapitalizeFirst() + ".";
				}
				else if (precept_Building != null)
				{
					text = "MessageBuildingMissingRitual".Translate(precept_Building.LabelCap.Named("PRECEPT"));
				}
			}
			Rect rect2 = rect;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMax - Page.BottomButSize.x * 2.75f;
			((Rect)(ref rect2)).width = Page.BottomButSize.x * 1.7f;
			((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMax - Page.BottomButSize.y;
			Precept precept = ideo.FirstPreceptWithWarning();
			if (text != null)
			{
				GUI.color = Color.red;
				Text.Font = GameFont.Tiny;
				Text.Anchor = (TextAnchor)2;
				Widgets.Label(rect2, text);
				Text.Font = GameFont.Small;
				Text.Anchor = (TextAnchor)0;
				GUI.color = Color.white;
			}
			else if (precept != null)
			{
				GUI.color = ColorLibrary.Yellow;
				Text.Font = GameFont.Tiny;
				Text.Anchor = (TextAnchor)2;
				precept.GetPlayerWarning(out var shortText, out var description);
				shortText = "Warning".Translate() + ": " + shortText.CapitalizeFirst();
				Widgets.Label(rect2, shortText);
				Text.Font = GameFont.Small;
				Text.Anchor = (TextAnchor)0;
				GUI.color = Color.white;
				Widgets.DrawHighlightIfMouseover(rect2);
				TooltipHandler.TipRegion(rect2, () => description, 37584575);
			}
			else
			{
				IdeoUIUtility.DrawImpactInfo(rect2, ideo.memes);
			}
		}
		if (ideo != null)
		{
			DoBottomButtons(rect, null, "RandomizeAll".Translate(), Randomize);
		}
		else
		{
			DoBottomButtons(rect);
		}
	}

	public virtual void Notify_ClosedChooseMemesDialog()
	{
		if (ideo != null && !ideo.memes.Any((MemeDef x) => x.category == MemeCategory.Normal))
		{
			Faction.OfPlayer.ideos.SetPrimary(null);
			Find.IdeoManager.Remove(ideo);
		}
	}

	private void Randomize()
	{
		if (ideo != null && TutorSystem.AllowAction("ConfiguringIdeo"))
		{
			ideo.foundation.Init(new IdeoGenerationParms(IdeoUIUtility.FactionForRandomization(ideo), forceNoExpansionIdeo: false, null, null, null, classicExtra: false, forceNoWeaponPreference: false, ideo.Fluid));
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
	}

	protected override bool CanDoNext()
	{
		if (!base.CanDoNext())
		{
			return false;
		}
		if (ideo == null)
		{
			Messages.Message("MessageMustChooseIdeo".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (ideo.name.NullOrEmpty())
		{
			Messages.Message("MessageIdeoNameCantBeEmpty".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		Pair<Precept, Precept> pair = ideo.FirstIncompatiblePreceptPair();
		if (pair != default(Pair<Precept, Precept>))
		{
			Messages.Message("MessageIdeoIncompatiblePrecepts".Translate(pair.First.Label.Named("PRECEPT1"), pair.Second.Label.Named("PRECEPT2")).CapitalizeFirst(), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		Tuple<Precept_Ritual, List<string>> tuple = ideo.FirstRitualMissingTarget();
		if (tuple != null)
		{
			Messages.Message("MessageRitualMissingTarget".Translate(tuple.Item1.LabelCap.Named("PRECEPT")) + ": " + tuple.Item2.ToCommaList().CapitalizeFirst() + ".", MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		Precept_Building precept_Building = ideo.FirstConsumableBuildingMissingRitual();
		if (precept_Building != null)
		{
			Messages.Message("MessageBuildingMissingRitual".Translate(precept_Building.LabelCap.Named("PRECEPT")), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		Faction.OfPlayer.ideos.SetPrimary(ideo);
		foreach (Ideo item in Find.IdeoManager.IdeosListForReading)
		{
			item.initialPlayerIdeo = false;
		}
		ideo.initialPlayerIdeo = true;
		Find.Scenario.PostIdeoChosen();
		return true;
	}
}
