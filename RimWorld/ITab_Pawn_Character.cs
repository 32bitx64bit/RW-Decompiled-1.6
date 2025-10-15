using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Pawn_Character : ITab
{
	private Pawn PawnToShowInfoAbout
	{
		get
		{
			Pawn pawn = null;
			if (SelPawn != null)
			{
				pawn = SelPawn;
			}
			else if (base.SelThing is Corpse corpse)
			{
				pawn = corpse.InnerPawn;
			}
			if (pawn == null)
			{
				Log.Error("Character tab found no selected pawn to display.");
				return null;
			}
			return pawn;
		}
	}

	public override bool IsVisible => PawnToShowInfoAbout.story != null;

	public ITab_Pawn_Character()
	{
		labelKey = "TabCharacter";
		tutorTag = "Character";
	}

	protected override void UpdateSize()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateSize();
		size = CharacterCardUtility.PawnCardSize(PawnToShowInfoAbout) + new Vector2(17f, 17f) * 2f;
	}

	protected override void FillTab()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		UpdateSize();
		Vector2 val = CharacterCardUtility.PawnCardSize(PawnToShowInfoAbout);
		CharacterCardUtility.DrawCharacterCard(new Rect(17f, 17f, val.x, val.y), PawnToShowInfoAbout);
	}
}
