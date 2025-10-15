using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Pawn_Needs : ITab
{
	private Vector2 thoughtScrollPosition;

	public override bool IsVisible
	{
		get
		{
			if (SelPawn.RaceProps.Animal && SelPawn.Faction == null)
			{
				return false;
			}
			if (SelPawn.RaceProps.Insect && SelPawn.Faction != Faction.OfPlayer)
			{
				return false;
			}
			CompHoldingPlatformTarget compHoldingPlatformTarget = SelPawn.TryGetComp<CompHoldingPlatformTarget>();
			if (compHoldingPlatformTarget != null && compHoldingPlatformTarget.CurrentlyHeldOnPlatform)
			{
				return false;
			}
			if (SelPawn.needs != null)
			{
				return SelPawn.needs.AllNeeds.Count > 0;
			}
			return false;
		}
	}

	public ITab_Pawn_Needs()
	{
		labelKey = "TabNeeds";
		tutorTag = "Needs";
	}

	public override void OnOpen()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		thoughtScrollPosition = default(Vector2);
	}

	protected override void FillTab()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		NeedsCardUtility.DoNeedsMoodAndThoughts(new Rect(0f, 0f, size.x, size.y), SelPawn, ref thoughtScrollPosition);
	}

	protected override void UpdateSize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = NeedsCardUtility.GetSize(SelPawn);
	}
}
