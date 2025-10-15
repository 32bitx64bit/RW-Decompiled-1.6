using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class ThoughtWorker_WearingColor : ThoughtWorker
{
	public const float RequiredMinPercentage = 0.6f;

	protected abstract Color? Color(Pawn p);

	protected override ThoughtState CurrentStateInternal(Pawn p)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Color? val = Color(p);
		if (!val.HasValue)
		{
			return false;
		}
		int num = 0;
		foreach (Apparel item in p.apparel.WornApparel)
		{
			CompColorable compColorable = item.TryGetComp<CompColorable>();
			if (compColorable != null && compColorable.Active && compColorable.Color.IndistinguishableFrom(val.Value))
			{
				num++;
			}
		}
		return (float)num / (float)p.apparel.WornApparelCount >= 0.6f;
	}
}
