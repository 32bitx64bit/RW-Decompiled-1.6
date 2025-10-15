using UnityEngine;
using Verse;

namespace RimWorld;

public class Command_AbilityTrial : Command_AbilitySpeech
{
	public Command_AbilityTrial(Ability ability, Pawn pawn)
		: base(ability, pawn)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		defaultLabel = "Accuse".Translate();
		defaultIconColor = Color.white;
	}
}
