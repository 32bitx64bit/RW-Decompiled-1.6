using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Dialog_PawnTableTest : Window
{
	private PawnColumnDef singleColumn;

	private PawnTable pawnTableMin;

	private PawnTable pawnTableOptimal;

	private PawnTable pawnTableMax;

	private const int TableTitleHeight = 30;

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	private List<Pawn> Pawns => Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer);

	public Dialog_PawnTableTest(PawnColumnDef singleColumn)
	{
		this.singleColumn = singleColumn;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		int num = ((int)((Rect)(ref inRect)).height - 90) / 3;
		PawnTableDef pawnTableDef = new PawnTableDef();
		pawnTableDef.columns = new List<PawnColumnDef> { singleColumn };
		pawnTableDef.minWidth = 0;
		if (pawnTableMin == null)
		{
			pawnTableMin = new PawnTable(pawnTableDef, () => Pawns, 0, 0);
			pawnTableMin.SetMinMaxSize(Mathf.Min(singleColumn.Worker.GetMinWidth(pawnTableMin) + 16, (int)((Rect)(ref inRect)).width), Mathf.Min(singleColumn.Worker.GetMinWidth(pawnTableMin) + 16, (int)((Rect)(ref inRect)).width), 0, num);
		}
		if (pawnTableOptimal == null)
		{
			pawnTableOptimal = new PawnTable(pawnTableDef, () => Pawns, 0, 0);
			pawnTableOptimal.SetMinMaxSize(Mathf.Min(singleColumn.Worker.GetOptimalWidth(pawnTableOptimal) + 16, (int)((Rect)(ref inRect)).width), Mathf.Min(singleColumn.Worker.GetOptimalWidth(pawnTableOptimal) + 16, (int)((Rect)(ref inRect)).width), 0, num);
		}
		if (pawnTableMax == null)
		{
			pawnTableMax = new PawnTable(pawnTableDef, () => Pawns, 0, 0);
			pawnTableMax.SetMinMaxSize(Mathf.Min(singleColumn.Worker.GetMaxWidth(pawnTableMax) + 16, (int)((Rect)(ref inRect)).width), Mathf.Min(singleColumn.Worker.GetMaxWidth(pawnTableMax) + 16, (int)((Rect)(ref inRect)).width), 0, num);
		}
		int num2 = 0;
		Text.Font = GameFont.Small;
		GUI.color = Color.gray;
		Widgets.Label(new Rect(0f, (float)num2, ((Rect)(ref inRect)).width, 30f), "Min size");
		GUI.color = Color.white;
		num2 += 30;
		pawnTableMin.PawnTableOnGUI(new Vector2(0f, (float)num2));
		num2 += num;
		GUI.color = Color.gray;
		Widgets.Label(new Rect(0f, (float)num2, ((Rect)(ref inRect)).width, 30f), "Optimal size");
		GUI.color = Color.white;
		num2 += 30;
		pawnTableOptimal.PawnTableOnGUI(new Vector2(0f, (float)num2));
		num2 += num;
		GUI.color = Color.gray;
		Widgets.Label(new Rect(0f, (float)num2, ((Rect)(ref inRect)).width, 30f), "Max size");
		GUI.color = Color.white;
		num2 += 30;
		pawnTableMax.PawnTableOnGUI(new Vector2(0f, (float)num2));
		num2 += num;
	}

	[DebugOutput("UI", false)]
	private static void PawnColumnTest()
	{
		List<DebugMenuOption> list = new List<DebugMenuOption>();
		List<PawnColumnDef> allDefsListForReading = DefDatabase<PawnColumnDef>.AllDefsListForReading;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			PawnColumnDef localDef = allDefsListForReading[i];
			list.Add(new DebugMenuOption(localDef.defName, DebugMenuOptionMode.Action, delegate
			{
				Find.WindowStack.Add(new Dialog_PawnTableTest(localDef));
			}));
		}
		Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
	}
}
