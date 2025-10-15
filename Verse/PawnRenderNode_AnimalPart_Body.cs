using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNode_AnimalPart_Body : PawnRenderNode_AnimalPart
{
	public PawnRenderNode_AnimalPart_Body(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
		: base(pawn, props, tree)
	{
	}

	protected override IEnumerable<(GraphicStateDef state, Graphic graphic)> StateGraphicsFor(Pawn pawn)
	{
		foreach (var item in base.StateGraphicsFor(pawn))
		{
			yield return item;
		}
		(GraphicStateDef, Graphic)? swimmingGraphic = GetSwimmingGraphic(pawn);
		if (swimmingGraphic.HasValue)
		{
			yield return swimmingGraphic.Value;
		}
		(GraphicStateDef, Graphic)? stationaryGraphic = GetStationaryGraphic(pawn);
		if (stationaryGraphic.HasValue)
		{
			yield return stationaryGraphic.Value;
		}
	}

	private (GraphicStateDef state, Graphic graphic)? GetSwimmingGraphic(Pawn pawn)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
		if (curKindLifeStage.swimmingGraphicData == null)
		{
			return null;
		}
		Graphic graphic = curKindLifeStage.swimmingGraphicData.Graphic;
		if (pawn.gender == Gender.Female && curKindLifeStage.femaleSwimmingGraphicData != null)
		{
			graphic = curKindLifeStage.femaleSwimmingGraphicData.Graphic;
		}
		if (pawn.TryGetAlternate(out var ag, out var _))
		{
			graphic = ag.GetSwimmingGraphic(graphic);
		}
		Color baseColor = graphic.Color;
		Color baseColor2 = graphic.ColorTwo;
		if (pawn.IsMutant)
		{
			baseColor = MutantUtility.GetMutantSkinColor(pawn, baseColor);
			baseColor2 = MutantUtility.GetMutantSkinColor(pawn, baseColor2);
		}
		baseColor = pawn.health.hediffSet.GetSkinColor(baseColor);
		baseColor2 = pawn.health.hediffSet.GetSkinColor(baseColor2);
		graphic = graphic.GetColoredVersion(graphic.Shader, baseColor, baseColor2);
		return (GraphicStateDefOf.Swimming, graphic);
	}

	private (GraphicStateDef state, Graphic graphic)? GetStationaryGraphic(Pawn pawn)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		PawnKindLifeStage curKindLifeStage = pawn.ageTracker.CurKindLifeStage;
		if (curKindLifeStage.stationaryGraphicData == null)
		{
			return null;
		}
		Graphic graphic = curKindLifeStage.stationaryGraphicData.Graphic;
		if (pawn.gender == Gender.Female && curKindLifeStage.femaleStationaryGraphicData != null)
		{
			graphic = curKindLifeStage.femaleStationaryGraphicData.Graphic;
		}
		if (pawn.TryGetAlternate(out var ag, out var _))
		{
			graphic = ag.GetStationaryGraphic(graphic);
		}
		Color baseColor = graphic.Color;
		Color baseColor2 = graphic.ColorTwo;
		if (pawn.IsMutant)
		{
			baseColor = MutantUtility.GetMutantSkinColor(pawn, baseColor);
			baseColor2 = MutantUtility.GetMutantSkinColor(pawn, baseColor2);
		}
		baseColor = pawn.health.hediffSet.GetSkinColor(baseColor);
		baseColor2 = pawn.health.hediffSet.GetSkinColor(baseColor2);
		graphic = graphic.GetColoredVersion(graphic.Shader, baseColor, baseColor2);
		return (GraphicStateDefOf.Stationary, graphic);
	}
}
