using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ScenPart_ConfigPage_ConfigureStartingPawns : ScenPart_ConfigPage_ConfigureStartingPawnsBase
{
	public int pawnCount = 3;

	public DevelopmentalStage allowedDevelopmentalStages = DevelopmentalStage.Baby | DevelopmentalStage.Child | DevelopmentalStage.Adult;

	public List<SkillDef> requiredSkills;

	private string pawnCountBuffer;

	private string pawnCountChoiceBuffer;

	private const int MaxPawnChoiceCount = 10;

	protected override int TotalPawnCount => pawnCount;

	public override void DoEditInterface(Listing_ScenEdit listing)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.DoEditInterface(listing);
		Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
		((Rect)(ref scenPartRect)).height = ScenPart.RowHeight;
		Text.Anchor = (TextAnchor)2;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref scenPartRect)).x - 200f, ((Rect)(ref scenPartRect)).y + ScenPart.RowHeight, 200f, ScenPart.RowHeight);
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 4f;
		Widgets.Label(rect, "ScenPart_StartWithPawns_OutOf".Translate());
		Text.Anchor = (TextAnchor)0;
		Widgets.TextFieldNumeric(scenPartRect, ref pawnCount, ref pawnCountBuffer, 1f, 10f);
		((Rect)(ref scenPartRect)).y = ((Rect)(ref scenPartRect)).y + ScenPart.RowHeight;
		Widgets.TextFieldNumeric(scenPartRect, ref pawnChoiceCount, ref pawnCountChoiceBuffer, pawnCount, 10f);
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref pawnCount, "pawnCount", 0);
	}

	public override string Summary(Scenario scen)
	{
		if (pawnCount == 1)
		{
			return "ScenPart_StartWithPawn".Translate();
		}
		return "ScenPart_StartWithPawns".Translate(pawnCount);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode() ^ pawnCount;
	}

	public override void Randomize()
	{
		pawnCount = Rand.RangeInclusive(1, 6);
		pawnChoiceCount = 10;
	}

	protected override void GenerateStartingPawns()
	{
		int num = 0;
		do
		{
			StartingPawnUtility.ClearAllStartingPawns();
			for (int i = 0; i < pawnCount; i++)
			{
				StartingPawnUtility.AddNewPawn();
			}
			num++;
		}
		while (num <= 20 && !StartingPawnUtility.WorkTypeRequirementsSatisfied());
	}

	public override void PostIdeoChosen()
	{
		Find.GameInitData.allowedDevelopmentalStages = allowedDevelopmentalStages;
		Find.GameInitData.startingSkillsRequired = requiredSkills;
		base.PostIdeoChosen();
	}
}
