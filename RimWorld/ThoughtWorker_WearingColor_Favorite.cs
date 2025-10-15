using UnityEngine;
using Verse;

namespace RimWorld;

public class ThoughtWorker_WearingColor_Favorite : ThoughtWorker_WearingColor
{
	protected override Color? Color(Pawn p)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (p.DevelopmentalStage.Baby())
		{
			return null;
		}
		return p.story.favoriteColor?.color;
	}
}
