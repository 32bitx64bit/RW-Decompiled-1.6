using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Pawn_Training : ITab
{
	public override bool IsVisible
	{
		get
		{
			if (SelPawn.training != null && SelPawn.Faction == Faction.OfPlayer && !SelPawn.RaceProps.hideTrainingTab)
			{
				if (SelPawn.IsMutant)
				{
					return SelPawn.mutant.Def.tameable;
				}
				return true;
			}
			return false;
		}
	}

	public ITab_Pawn_Training()
	{
		labelKey = "TabTraining";
		tutorTag = "Training";
	}

	protected override void FillTab()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 17f);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 10f;
		TrainingCardUtility.DrawTrainingCard(rect, SelPawn);
	}

	protected override void UpdateSize()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateSize();
		size = new Vector2(300f, TrainingCardUtility.TotalHeightForPawn(SelPawn));
	}
}
