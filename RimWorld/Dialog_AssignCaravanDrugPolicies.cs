using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_AssignCaravanDrugPolicies : Window
{
	private Caravan caravan;

	private Vector2 scrollPos;

	private float lastHeight;

	private const float RowHeight = 30f;

	private const float AssignDrugPolicyButtonsTotalWidth = 354f;

	private const int ManageDrugPoliciesButtonHeight = 32;

	public override Vector2 InitialSize => new Vector2(550f, 500f);

	public Dialog_AssignCaravanDrugPolicies(Caravan caravan)
	{
		this.caravan = caravan;
		doCloseButton = true;
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).height = ((Rect)(ref rect)).height - Window.CloseButSize.y;
		float num = 0f;
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - 354f - 16f, num, 354f, 32f), "ManageDrugPolicies".Translate()))
		{
			Find.WindowStack.Add(new Dialog_ManageDrugPolicies(null));
		}
		num += 42f;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, num, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - num);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, lastHeight);
		Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);
		float num2 = 0f;
		foreach (Pawn pawn in caravan.pawns)
		{
			if (pawn.drugs != null && !pawn.DevelopmentalStage.Baby())
			{
				if (num2 + 30f >= scrollPos.y && num2 <= scrollPos.y + ((Rect)(ref outRect)).height)
				{
					DoRow(new Rect(0f, num2, ((Rect)(ref viewRect)).width, 30f), pawn);
				}
				num2 += 30f;
			}
		}
		lastHeight = num2;
		Widgets.EndScrollView();
	}

	private void DoRow(Rect rect, Pawn pawn)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 354f, 30f);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Widgets.Label(rect2, pawn.LabelCap);
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		GUI.color = Color.white;
		DrugPolicyUIUtility.DoAssignDrugPolicyButtons(new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 354f, ((Rect)(ref rect)).y, 354f, 30f), pawn);
	}
}
