using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_MechBillConfig : Dialog_BillConfig
{
	private static float formingInfoHeight;

	public Dialog_MechBillConfig(Bill_Mech bill, IntVec3 billGiverPos)
		: base(bill, billGiverPos)
	{
		ModLister.CheckBiotech("BillWithFormingCycleDialog");
	}

	protected override void DoIngredientConfigPane(float x, ref float y, float width, float height)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		float y2 = y;
		base.DoIngredientConfigPane(x, ref y2, width, height - formingInfoHeight);
		if (bill.billStack.billGiver is Building_MechGestator building_MechGestator && building_MechGestator.ActiveBill == bill)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(x, y2, width, 9999f);
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(rect);
			StringBuilder stringBuilder = new StringBuilder();
			listing_Standard.Label("FormerIngredients".Translate() + ":");
			building_MechGestator.ActiveBill.AppendCurrentIngredientCount(stringBuilder);
			listing_Standard.Label(stringBuilder.ToString());
			Bill_Mech bill_Mech = (Bill_Mech)bill;
			listing_Standard.Label(string.Concat("GestationCyclesCompleted".Translate() + ": ", bill_Mech.GestationCyclesCompleted.ToString(), " / ", bill_Mech.recipe.gestationCycles.ToString()));
			listing_Standard.Gap();
			listing_Standard.End();
			formingInfoHeight = listing_Standard.CurHeight;
		}
	}
}
