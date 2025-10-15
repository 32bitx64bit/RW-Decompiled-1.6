using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class PawnColumnWorker_DrugPolicy : PawnColumnWorker
{
	private const int TopAreaHeight = 65;

	public const int ManageDrugPoliciesButtonHeight = 32;

	public override void DoHeader(Rect rect, PawnTable table)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		base.DoHeader(rect, table);
		MouseoverSounds.DoRegion(rect);
		Rect rect2 = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + (((Rect)(ref rect)).height - 65f), Mathf.Min(((Rect)(ref rect)).width, 360f), 32f);
		if (Widgets.ButtonText(rect2, "ManageDrugPolicies".Translate()))
		{
			Find.WindowStack.Add(new Dialog_ManageDrugPolicies(null));
		}
		UIHighlighter.HighlightOpportunity(rect2, "ManageDrugPolicies");
		UIHighlighter.HighlightOpportunity(rect2, "ButtonAssignDrugs");
	}

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.drugs != null)
		{
			DrugPolicyUIUtility.DoAssignDrugPolicyButtons(rect, pawn);
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), Mathf.CeilToInt(194f));
	}

	public override int GetOptimalWidth(PawnTable table)
	{
		return Mathf.Clamp(Mathf.CeilToInt(251f), GetMinWidth(table), GetMaxWidth(table));
	}

	public override int GetMinHeaderHeight(PawnTable table)
	{
		return Mathf.Max(base.GetMinHeaderHeight(table), 65);
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
	}

	private int GetValueToCompare(Pawn pawn)
	{
		if (pawn.drugs != null && pawn.drugs.CurrentPolicy != null)
		{
			return pawn.drugs.CurrentPolicy.id;
		}
		return int.MinValue;
	}
}
