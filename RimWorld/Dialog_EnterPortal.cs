using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_EnterPortal : Window
{
	private enum Tab
	{
		Pawns,
		Items
	}

	private const float TitleRectHeight = 35f;

	private const float BottomAreaHeight = 55f;

	private readonly Vector2 BottomButtonSize = new Vector2(160f, 40f);

	private MapPortal portal;

	private List<TransferableOneWay> transferables;

	private TransferableOneWayWidget pawnsTransfer;

	private TransferableOneWayWidget itemsTransfer;

	private Tab tab;

	private static List<TabRecord> tabsList = new List<TabRecord>();

	public override Vector2 InitialSize => new Vector2(1024f, (float)UI.screenHeight);

	protected override float Margin => 0f;

	public Dialog_EnterPortal(MapPortal portal)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.portal = portal;
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		CalculateAndRecacheTransferables();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width, 35f);
		using (new TextBlock(GameFont.Medium, (TextAnchor)4))
		{
			Widgets.Label(rect, portal.EnterString);
		}
		tabsList.Clear();
		tabsList.Add(new TabRecord("PawnsTab".Translate(), delegate
		{
			tab = Tab.Pawns;
		}, tab == Tab.Pawns));
		tabsList.Add(new TabRecord("ItemsTab".Translate(), delegate
		{
			tab = Tab.Items;
		}, tab == Tab.Items));
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 67f;
		Widgets.DrawMenuSection(inRect);
		TabDrawer.DrawTabs(inRect, tabsList);
		inRect = inRect.ContractedBy(17f);
		Widgets.BeginGroup(inRect);
		Rect val = inRect.AtZero();
		DoBottomButtons(val);
		Rect inRect2 = val;
		((Rect)(ref inRect2)).yMax = ((Rect)(ref inRect2)).yMax - 76f;
		bool anythingChanged = false;
		switch (tab)
		{
		case Tab.Pawns:
			pawnsTransfer.OnGUI(inRect2, out anythingChanged);
			break;
		case Tab.Items:
			itemsTransfer.OnGUI(inRect2, out anythingChanged);
			break;
		}
		Widgets.EndGroup();
	}

	private void DoBottomButtons(Rect rect)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f - BottomButtonSize.x / 2f, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y), "ResetButton".Translate()))
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			CalculateAndRecacheTransferables();
		}
		if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y), "CancelButton".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - BottomButtonSize.x, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y), "AcceptButton".Translate()) && TryAccept())
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}
	}

	private bool TryAccept()
	{
		List<Pawn> pawnsFromTransferables = TransferableUtility.GetPawnsFromTransferables(transferables);
		portal.leftToLoad = new List<TransferableOneWay>();
		foreach (TransferableOneWay transferable in transferables)
		{
			portal.AddToTheToLoadList(transferable, transferable.CountToTransfer);
		}
		EnterPortalUtility.MakeLordsAsAppropriate(pawnsFromTransferables, portal);
		return true;
	}

	private void CalculateAndRecacheTransferables()
	{
		transferables = new List<TransferableOneWay>();
		if (portal.LoadInProgress)
		{
			foreach (TransferableOneWay item in portal.leftToLoad)
			{
				transferables.Add(item);
			}
		}
		AddPawnsToTransferables();
		AddItemsToTransferables();
		foreach (Thing item2 in EnterPortalUtility.ThingsBeingHauledTo(portal))
		{
			AddToTransferables(item2);
		}
		pawnsTransfer = new TransferableOneWayWidget(null, null, null, "TransferMapPortalColonyThingCountTip".Translate(), drawMass: true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, includePawnsMassInMassUsage: true, () => float.MaxValue, 0f, ignoreSpawnedCorpseGearAndInventoryMass: false, portal.Map.Tile, drawMarketValue: false, drawEquippedWeapon: true);
		CaravanUIUtility.AddPawnsSections(pawnsTransfer, transferables);
		itemsTransfer = new TransferableOneWayWidget(transferables.Where((TransferableOneWay x) => x.ThingDef.category != ThingCategory.Pawn), null, null, "TransferMapPortalColonyThingCountTip".Translate(), drawMass: true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, includePawnsMassInMassUsage: true, () => float.MaxValue, 0f, ignoreSpawnedCorpseGearAndInventoryMass: false, portal.Map.Tile);
	}

	private void AddToTransferables(Thing t)
	{
		if (!portal.LoadInProgress || !portal.leftToLoad.Any((TransferableOneWay trans) => trans.things.Contains(t)))
		{
			TransferableOneWay transferableOneWay = TransferableUtility.TransferableMatching(t, transferables, TransferAsOneMode.PodsOrCaravanPacking);
			if (transferableOneWay == null)
			{
				transferableOneWay = new TransferableOneWay();
				transferables.Add(transferableOneWay);
			}
			if (transferableOneWay.things.Contains(t))
			{
				Log.Error("Tried to add the same thing twice to TransferableOneWay: " + t);
			}
			else
			{
				transferableOneWay.things.Add(t);
			}
		}
	}

	private void AddPawnsToTransferables()
	{
		foreach (Pawn item in CaravanFormingUtility.AllSendablePawns(portal.Map, allowEvenIfDowned: true, allowEvenIfInMentalState: false, allowEvenIfPrisonerNotSecure: false, allowCapturableDownedPawns: false, allowLodgers: true))
		{
			AddToTransferables(item);
		}
	}

	private void AddItemsToTransferables()
	{
		bool isPocketMap = portal.Map.IsPocketMap;
		foreach (Thing item in CaravanFormingUtility.AllReachableColonyItems(portal.Map, isPocketMap, isPocketMap))
		{
			AddToTransferables(item);
		}
	}

	public override void OnAcceptKeyPressed()
	{
		if (TryAccept())
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}
	}
}
