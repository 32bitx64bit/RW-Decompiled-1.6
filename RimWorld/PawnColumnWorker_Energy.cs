using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class PawnColumnWorker_Energy : PawnColumnWorker
{
	private const int Width = 120;

	private const int BarPadding = 4;

	public static readonly Texture2D EnergyBarTex = SolidColorMaterials.NewSolidColorTexture(Color32.op_Implicit(new Color32((byte)252, byte.MaxValue, byte.MaxValue, (byte)65)));

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!pawn.IsGestating())
		{
			Widgets.FillableBar(rect.ContractedBy(4f), pawn.needs.energy.CurLevelPercentage, EnergyBarTex, BaseContent.ClearTex, doBorder: false);
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(rect, Mathf.RoundToInt(pawn.needs.energy.CurLevel) + " / " + Mathf.RoundToInt(pawn.needs.energy.MaxLevel));
			Text.Anchor = (TextAnchor)0;
			Text.Font = GameFont.Small;
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), 120);
	}

	public override int GetMaxWidth(PawnTable table)
	{
		return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
	}
}
