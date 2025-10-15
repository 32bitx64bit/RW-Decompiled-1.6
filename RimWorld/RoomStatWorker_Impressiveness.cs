using UnityEngine;
using Verse;

namespace RimWorld;

public class RoomStatWorker_Impressiveness : RoomStatWorker
{
	public override float GetScore(Room room)
	{
		float factor = GetFactor(room.GetStat(RoomStatDefOf.Wealth) / 1500f);
		float factor2 = GetFactor(room.GetStat(RoomStatDefOf.Beauty) / 3f);
		float factor3 = GetFactor(room.GetStat(RoomStatDefOf.Space) / 125f);
		float factor4 = GetFactor(1f + Mathf.Min(room.GetStat(RoomStatDefOf.Cleanliness), 0f) / 2.5f);
		float num = (factor + factor2 + factor3 + factor4) / 4f;
		float num2 = Mathf.Min(factor, Mathf.Min(factor2, Mathf.Min(factor3, factor4)));
		float num3 = Mathf.Lerp(num, num2, 0.35f);
		float num4 = factor3 * 5f;
		if (num3 > num4)
		{
			num3 = Mathf.Lerp(num3, num4, 0.75f);
		}
		return num3 * 100f;
	}

	private float GetFactor(float baseFactor)
	{
		if (Mathf.Abs(baseFactor) < 1f)
		{
			return baseFactor;
		}
		if (baseFactor > 0f)
		{
			return 1f + Mathf.Log(baseFactor);
		}
		return -1f - Mathf.Log(0f - baseFactor);
	}
}
