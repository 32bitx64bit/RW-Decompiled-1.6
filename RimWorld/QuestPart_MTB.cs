using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class QuestPart_MTB : QuestPartActivable
{
	private const int CheckIntervalTicks = 10;

	protected abstract float MTBDays { get; }

	public override void QuestPartTick()
	{
		float mTBDays = MTBDays;
		if (mTBDays > 0f && Find.TickManager.TicksGame % 10 == 0 && Rand.MTBEventOccurs(mTBDays, 60000f, 10f))
		{
			Complete();
		}
	}

	public override void DoDebugWindowContents(Rect innerRect, ref float curY)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (base.State == QuestPartState.Enabled)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, 500f, 25f);
			if (Widgets.ButtonText(rect, "MTB occurs " + ToString()))
			{
				Complete();
			}
			curY += ((Rect)(ref rect)).height + 4f;
		}
	}
}
