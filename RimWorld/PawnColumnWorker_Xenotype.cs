using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_Xenotype : PawnColumnWorker_Icon
{
	protected override Texture2D GetIconFor(Pawn pawn)
	{
		return pawn.genes?.XenotypeIcon;
	}

	protected override Color GetIconColor(Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return XenotypeDef.IconColor;
	}

	protected override string GetIconTip(Pawn pawn)
	{
		if (pawn.genes != null)
		{
			return pawn.genes.XenotypeLabelCap;
		}
		return null;
	}
}
