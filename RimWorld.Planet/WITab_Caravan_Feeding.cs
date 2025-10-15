using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

public class WITab_Caravan_Feeding : WITab
{
	private Vector2 scrollPosition;

	private Pawn selectedPawn;

	private Vector2 scrollPositionAutoBreastfeed;

	private Vector2 scrollPositionBabyConsumables;

	private const int contextHash = 1384745000;

	private const float RowHeight = 34f;

	private const float ScrollViewTopMargin = 15f;

	private const float PawnLabelHeight = 18f;

	private const float PawnLabelColumnWidth = 100f;

	private const float SpaceAroundIcon = 4f;

	private const float BabyPickWindowWidth = 250f;

	private List<Pawn> Pawns => base.SelCaravan.PawnsListForReading;

	public override bool IsVisible
	{
		get
		{
			ChildcareUtility.BreastfeedFailReason? reason;
			return Pawns.Any((Pawn pawn) => ChildcareUtility.CanSuckle(pawn, out reason));
		}
	}

	public WITab_Caravan_Feeding()
	{
		labelKey = "TabCaravanFeeding";
	}

	protected override void UpdateSize()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(250f, Mathf.Min(550f, PaneTopY - 30f));
		base.UpdateSize();
	}

	protected override void FillTab()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		RectDivider divider = new RectDivider(GenUI.ContractedBy(new Rect(0f, 15f, size.x, size.y - 15f), 10f), 1384745000);
		Widgets.ListSeparator(ref divider, "Babies".Translate().CapitalizeFirst());
		ChildcareUtility.BreastfeedFailReason? reason;
		RectDivider scrollViewRect = divider.CreateViewRect(Pawns.Count((Pawn pawn) => ChildcareUtility.CanSuckle(pawn, out reason)), 34f);
		Widgets.BeginScrollView(divider, ref scrollPosition, scrollViewRect);
		DoRows(ref scrollViewRect);
		Widgets.EndScrollView();
	}

	private void DoRows(ref RectDivider scrollViewRect)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (selectedPawn != null && (selectedPawn.Destroyed || !base.SelCaravan.ContainsPawn(selectedPawn)))
		{
			selectedPawn = null;
		}
		foreach (Pawn pawn in Pawns)
		{
			if (ChildcareUtility.CanSuckle(pawn, out var _))
			{
				DoRow(scrollViewRect.NewRow(34f), pawn);
			}
		}
	}

	private void DoRow(Rect rect, Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		CaravanThingsTabUtility.DoAbandonButton(val, pawn, base.SelCaravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, pawn);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoOpenSpecificTabButton(val, pawn, ref selectedPawn);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoOpenSpecificTabButtonInvisible(val, pawn, ref selectedPawn);
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect2, pawn);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(((Rect)(ref rect2)).xMax + 4f, 8f, 100f, 18f);
		GenMapUI.DrawPawnLabel(pawn, bgRect, 1f, 100f, null, GameFont.Small, alwaysDrawBg: false, alignCenter: false);
		if (pawn.Downed && !pawn.ageTracker.CurLifeStage.alwaysDowned)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	protected override void ExtraOnGUI()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		base.ExtraOnGUI();
		if (selectedPawn != null && (selectedPawn.Destroyed || !base.SelCaravan.ContainsPawn(selectedPawn)))
		{
			selectedPawn = null;
		}
		Pawn captureSelectedPawn = selectedPawn;
		if (selectedPawn == null)
		{
			return;
		}
		Rect tabRect = base.TabRect;
		float num = 500f;
		Rect rect = new Rect(((Rect)(ref tabRect)).xMax - 1f, ((Rect)(ref tabRect)).yMin, num, ((Rect)(ref tabRect)).height);
		Find.WindowStack.ImmediateWindow(323826479, rect, WindowLayer.GameUI, delegate
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (!captureSelectedPawn.DestroyedOrNull() && base.SelCaravan != null)
			{
				ITab_Pawn_Feeding.FillTab(captureSelectedPawn, rect.AtZero(), ref scrollPositionAutoBreastfeed, ref scrollPositionBabyConsumables, Pawns);
				if (Widgets.CloseButtonFor(rect.AtZero()))
				{
					captureSelectedPawn = null;
					SoundDefOf.TabClose.PlayOneShotOnCamera();
				}
			}
		});
	}

	public override void Notify_ClearingAllMapsMemory()
	{
		base.Notify_ClearingAllMapsMemory();
		selectedPawn = null;
	}

	public override void OnOpen()
	{
		base.OnOpen();
		if (selectedPawn == null || !Pawns.Contains(selectedPawn))
		{
			selectedPawn = Pawns.FirstOrFallback((Pawn pawn) => ChildcareUtility.CanSuckle(pawn, out var _));
		}
	}
}
