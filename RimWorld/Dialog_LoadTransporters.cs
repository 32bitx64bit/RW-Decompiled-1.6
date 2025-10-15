using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

public class Dialog_LoadTransporters : Window
{
	private enum Tab
	{
		Pawns,
		Items
	}

	private Map map;

	private List<CompTransporter> transporters;

	private List<TransferableOneWay> transferables;

	private TransferableOneWayWidget pawnsTransfer;

	private TransferableOneWayWidget itemsTransfer;

	private Tab tab;

	private float lastMassFlashTime = -9999f;

	private bool massUsageDirty = true;

	private float cachedMassUsage;

	private bool caravanMassUsageDirty = true;

	private float cachedCaravanMassUsage;

	private bool caravanMassCapacityDirty = true;

	private float cachedCaravanMassCapacity;

	private string cachedCaravanMassCapacityExplanation;

	private bool tilesPerDayDirty = true;

	private float cachedTilesPerDay;

	private string cachedTilesPerDayExplanation;

	private bool daysWorthOfFoodDirty = true;

	private (float days, float tillRot) cachedDaysWorthOfFood;

	private bool foragedFoodPerDayDirty = true;

	private (ThingDef food, float perDay) cachedForagedFoodPerDay;

	private string cachedForagedFoodPerDayExplanation;

	private bool visibilityDirty = true;

	private float cachedVisibility;

	private string cachedVisibilityExplanation;

	private const float TitleRectHeight = 35f;

	private const float BottomAreaHeight = 55f;

	private readonly Vector2 BottomButtonSize = new Vector2(160f, 40f);

	private static List<TabRecord> tabsList = new List<TabRecord>();

	private static List<List<TransferableOneWay>> tmpLeftToLoadCopy = new List<List<TransferableOneWay>>();

	private static Dictionary<TransferableOneWay, int> tmpLeftCountToTransfer = new Dictionary<TransferableOneWay, int>();

	public bool CanChangeAssignedThingsAfterStarting => transporters[0].Props.canChangeAssignedThingsAfterStarting;

	public bool LoadingInProgressOrReadyToLaunch => transporters[0].LoadingInProgressOrReadyToLaunch;

	public override Vector2 InitialSize => new Vector2(1024f, (float)UI.screenHeight);

	protected override float Margin => 0f;

	private float MassCapacity
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < transporters.Count; i++)
			{
				num += transporters[i].MassCapacity;
			}
			return num;
		}
	}

	private float CaravanMassCapacity
	{
		get
		{
			if (caravanMassCapacityDirty)
			{
				caravanMassCapacityDirty = false;
				StringBuilder stringBuilder = new StringBuilder();
				cachedCaravanMassCapacity = CollectionsMassCalculator.CapacityTransferables(transferables, stringBuilder);
				cachedCaravanMassCapacityExplanation = stringBuilder.ToString();
			}
			return cachedCaravanMassCapacity;
		}
	}

	private string TransportersLabel
	{
		get
		{
			if (transporters[0].Props.max1PerGroup)
			{
				return transporters[0].parent.Label;
			}
			return Find.ActiveLanguageWorker.Pluralize(transporters[0].parent.Label);
		}
	}

	private string TransportersLabelCap => TransportersLabel.CapitalizeFirst();

	private BiomeDef Biome => map.Biome;

	private float MassUsage
	{
		get
		{
			if (massUsageDirty)
			{
				massUsageDirty = false;
				CompShuttle compShuttle = transporters.AsShuttle();
				cachedMassUsage = CollectionsMassCalculator.MassUsageTransferables(transferables, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, compShuttle == null || compShuttle.requiredColonistCount == 0);
			}
			return cachedMassUsage;
		}
	}

	public float CaravanMassUsage
	{
		get
		{
			if (caravanMassUsageDirty)
			{
				caravanMassUsageDirty = false;
				cachedCaravanMassUsage = CollectionsMassCalculator.MassUsageTransferables(transferables, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload);
			}
			return cachedCaravanMassUsage;
		}
	}

	private float TilesPerDay
	{
		get
		{
			if (!map.Tile.Valid || !map.Tile.LayerDef.SurfaceTiles)
			{
				return 0f;
			}
			if (tilesPerDayDirty)
			{
				tilesPerDayDirty = false;
				StringBuilder stringBuilder = new StringBuilder();
				cachedTilesPerDay = TilesPerDayCalculator.ApproxTilesPerDay(transferables, MassUsage, MassCapacity, map.Tile, PlanetTile.Invalid, transporters.IsShuttle(), stringBuilder);
				cachedTilesPerDayExplanation = stringBuilder.ToString();
			}
			return cachedTilesPerDay;
		}
	}

	private (float days, float tillRot) DaysWorthOfFood
	{
		get
		{
			if (daysWorthOfFoodDirty)
			{
				daysWorthOfFoodDirty = false;
				float item = DaysWorthOfFoodCalculator.ApproxDaysWorthOfFood(transferables, map.Tile, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, Faction.OfPlayer);
				cachedDaysWorthOfFood = (days: item, tillRot: DaysUntilRotCalculator.ApproxDaysUntilRot(transferables, map.Tile, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload));
			}
			return cachedDaysWorthOfFood;
		}
	}

	private (ThingDef food, float perDay) ForagedFoodPerDay
	{
		get
		{
			if (foragedFoodPerDayDirty)
			{
				foragedFoodPerDayDirty = false;
				StringBuilder stringBuilder = new StringBuilder();
				cachedForagedFoodPerDay = ForagedFoodPerDayCalculator.ForagedFoodPerDay(transferables, Biome, Faction.OfPlayer, stringBuilder);
				cachedForagedFoodPerDayExplanation = stringBuilder.ToString();
			}
			return cachedForagedFoodPerDay;
		}
	}

	private float Visibility
	{
		get
		{
			if (visibilityDirty)
			{
				visibilityDirty = false;
				StringBuilder stringBuilder = new StringBuilder();
				cachedVisibility = CaravanVisibilityCalculator.Visibility(transferables, stringBuilder);
				cachedVisibilityExplanation = stringBuilder.ToString();
			}
			return cachedVisibility;
		}
	}

	public Dialog_LoadTransporters(Map map, List<CompTransporter> transporters)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		this.transporters = new List<CompTransporter>();
		this.transporters.AddRange(transporters);
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		CalculateAndRecacheTransferables();
		if (CanChangeAssignedThingsAfterStarting && LoadingInProgressOrReadyToLaunch)
		{
			SetLoadedItemsToLoad();
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(0f, 0f, ((Rect)(ref inRect)).width, 35f);
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, "LoadTransporters".Translate(TransportersLabel).CapitalizeFirst());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		if (transporters[0].Props.showOverallStats && transporters[0].Map.Tile.Valid)
		{
			CaravanUIUtility.DrawCaravanInfo(new CaravanUIUtility.CaravanInfo(MassUsage, MassCapacity, "", TilesPerDay, cachedTilesPerDayExplanation, DaysWorthOfFood, ForagedFoodPerDay, cachedForagedFoodPerDayExplanation, Visibility, cachedVisibilityExplanation, CaravanMassUsage, CaravanMassCapacity, cachedCaravanMassCapacityExplanation), null, map.Tile, null, lastMassFlashTime, new Rect(12f, 35f, ((Rect)(ref inRect)).width - 24f, 40f), lerpMassColor: false, null, multiline: false, !transporters.IsShuttle());
			((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 52f;
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
		((Rect)(ref inRect)).height = ((Rect)(ref inRect)).height + 17f;
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
		if (anythingChanged)
		{
			CountToTransferChanged();
		}
		Widgets.EndGroup();
	}

	public override bool CausesMessageBackground()
	{
		return true;
	}

	private void AddToTransferables(Thing t)
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

	private bool ErrorsRequireConfirmation(List<Pawn> pawns)
	{
		if (transporters.IsShuttle())
		{
			return false;
		}
		if (CaravanMassUsage > CaravanMassCapacity && CaravanMassCapacity != 0f)
		{
			if (transporters[0].parent != null && transporters[0].parent.def == ThingDefOf.TransportPod)
			{
				return pawns.Any((Pawn x) => x.IsFreeColonist);
			}
			return true;
		}
		return false;
	}

	private void DoBottomButtons(Rect rect)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f - BottomButtonSize.x / 2f, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y), "ResetButton".Translate()))
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			CalculateAndRecacheTransferables();
		}
		if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y), "CancelButton".Translate()))
		{
			Close();
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width - BottomButtonSize.x, ((Rect)(ref rect)).height - 55f - 17f, BottomButtonSize.x, BottomButtonSize.y);
		if (Widgets.ButtonText(rect2, "AcceptButton".Translate()))
		{
			onAcceptButton();
		}
		if (Prefs.DevMode)
		{
			float num = 200f;
			float num2 = BottomButtonSize.y / 2f;
			if (!LoadingInProgressOrReadyToLaunch && Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect2)).yMax + 4f, num, num2), "DEV: Load instantly") && DebugTryLoadInstantly())
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				Close(doCloseSound: false);
			}
			if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect2)).yMax + 4f, num, num2), "DEV: Select everything"))
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				SetToLoadEverything();
			}
		}
	}

	private void onAcceptButton()
	{
		List<Pawn> pawnsFromTransferables = TransferableUtility.GetPawnsFromTransferables(transferables);
		if (ErrorsRequireConfirmation(pawnsFromTransferables))
		{
			if (!CheckForErrors(pawnsFromTransferables))
			{
				return;
			}
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("TransportersCaravanWillBeImmobile".Translate(), delegate
			{
				if (TryAccept())
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					Close(doCloseSound: false);
				}
			}));
		}
		else if (TryAccept())
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			Close(doCloseSound: false);
		}
	}

	private void CalculateAndRecacheTransferables()
	{
		transferables = new List<TransferableOneWay>();
		AddPawnsToTransferables();
		AddItemsToTransferables();
		if (CanChangeAssignedThingsAfterStarting && LoadingInProgressOrReadyToLaunch)
		{
			for (int i = 0; i < transporters.Count; i++)
			{
				for (int j = 0; j < transporters[i].innerContainer.Count; j++)
				{
					AddToTransferables(transporters[i].innerContainer[j]);
				}
			}
			foreach (Thing item in TransporterUtility.ThingsBeingHauledTo(transporters, map))
			{
				AddToTransferables(item);
			}
		}
		pawnsTransfer = new TransferableOneWayWidget(null, null, null, "TransporterColonyThingCountTip".Translate(), drawMass: true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, includePawnsMassInMassUsage: true, () => MassCapacity - MassUsage, 0f, ignoreSpawnedCorpseGearAndInventoryMass: false, map.Tile, drawMarketValue: true, drawEquippedWeapon: true, drawNutritionEatenPerDay: true, drawMechEnergy: false, drawItemNutrition: false, drawForagedFoodPerDay: true);
		CaravanUIUtility.AddPawnsSections(pawnsTransfer, transferables);
		itemsTransfer = new TransferableOneWayWidget(transferables.Where((TransferableOneWay x) => x.ThingDef.category != ThingCategory.Pawn), null, null, "TransporterColonyThingCountTip".Translate(), drawMass: true, IgnorePawnsInventoryMode.IgnoreIfAssignedToUnload, includePawnsMassInMassUsage: true, () => MassCapacity - MassUsage, 0f, ignoreSpawnedCorpseGearAndInventoryMass: false, map.Tile, drawMarketValue: true, drawEquippedWeapon: false, drawNutritionEatenPerDay: false, drawMechEnergy: false, drawItemNutrition: true, drawForagedFoodPerDay: false, drawDaysUntilRot: true);
		CountToTransferChanged();
	}

	private bool DebugTryLoadInstantly()
	{
		TransporterUtility.InitiateLoading(transporters);
		int i;
		for (i = 0; i < transferables.Count; i++)
		{
			TransferableUtility.Transfer(transferables[i].things, transferables[i].CountToTransfer, delegate(Thing splitPiece, IThingHolder originalThing)
			{
				transporters[i % transporters.Count].GetDirectlyHeldThings().TryAdd(splitPiece);
			});
		}
		return true;
	}

	private bool TryAccept()
	{
		List<Pawn> pawnsFromTransferables = TransferableUtility.GetPawnsFromTransferables(transferables);
		if (!CheckForErrors(pawnsFromTransferables))
		{
			return false;
		}
		if (LoadingInProgressOrReadyToLaunch)
		{
			AssignTransferablesToRandomTransporters();
			TransporterUtility.MakeLordsAsAppropriate(pawnsFromTransferables, transporters, map);
			IReadOnlyList<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				if (allPawnsSpawned[i].CurJobDef == JobDefOf.HaulToTransporter && transporters.Contains(((JobDriver_HaulToTransporter)allPawnsSpawned[i].jobs.curDriver).Transporter))
				{
					allPawnsSpawned[i].jobs.EndCurrentJob(JobCondition.InterruptForced);
				}
			}
		}
		else
		{
			TransporterUtility.InitiateLoading(transporters);
			AssignTransferablesToRandomTransporters();
			TransporterUtility.MakeLordsAsAppropriate(pawnsFromTransferables, transporters, map);
			if (transporters.IsShuttle())
			{
				Messages.Message("MessageShuttleLoadingProcessStarted".Translate(), transporters[0].parent, MessageTypeDefOf.TaskCompletion, historical: false);
			}
			else if (transporters[0].Props.max1PerGroup)
			{
				Messages.Message("MessageTransporterSingleLoadingProcessStarted".Translate(), transporters[0].parent, MessageTypeDefOf.TaskCompletion, historical: false);
			}
			else
			{
				Messages.Message("MessageTransportersLoadingProcessStarted".Translate(), transporters[0].parent, MessageTypeDefOf.TaskCompletion, historical: false);
			}
		}
		return true;
	}

	private void SetLoadedItemsToLoad()
	{
		for (int i = 0; i < transporters.Count; i++)
		{
			int j;
			for (j = 0; j < transporters[i].innerContainer.Count; j++)
			{
				TransferableOneWay transferableOneWay = transferables.Find((TransferableOneWay x) => x.things.Contains(transporters[i].innerContainer[j]));
				if (transferableOneWay != null && transferableOneWay.CanAdjustBy(transporters[i].innerContainer[j].stackCount).Accepted)
				{
					transferableOneWay.AdjustBy(transporters[i].innerContainer[j].stackCount);
				}
			}
			if (transporters[i].leftToLoad == null)
			{
				continue;
			}
			for (int k = 0; k < transporters[i].leftToLoad.Count; k++)
			{
				TransferableOneWay transferableOneWay2 = transporters[i].leftToLoad[k];
				if (transferableOneWay2.CountToTransfer != 0 && transferableOneWay2.HasAnyThing)
				{
					TransferableOneWay transferableOneWay3 = TransferableUtility.TransferableMatchingDesperate(transferableOneWay2.AnyThing, transferables, TransferAsOneMode.PodsOrCaravanPacking);
					if (transferableOneWay3 != null && transferableOneWay3.CanAdjustBy(transferableOneWay2.CountToTransferToDestination).Accepted)
					{
						transferableOneWay3.AdjustBy(transferableOneWay2.CountToTransferToDestination);
					}
				}
			}
		}
	}

	private void AssignTransferablesToRandomTransporters()
	{
		tmpLeftToLoadCopy.Clear();
		for (int i = 0; i < transporters.Count; i++)
		{
			tmpLeftToLoadCopy.Add((transporters[i].leftToLoad != null) ? transporters[i].leftToLoad.ToList() : new List<TransferableOneWay>());
			if (transporters[i].leftToLoad != null)
			{
				transporters[i].leftToLoad.Clear();
			}
		}
		tmpLeftCountToTransfer.Clear();
		for (int j = 0; j < transferables.Count; j++)
		{
			tmpLeftCountToTransfer.Add(transferables[j], transferables[j].CountToTransfer);
		}
		if (LoadingInProgressOrReadyToLaunch)
		{
			for (int k = 0; k < transferables.Count; k++)
			{
				if (!transferables[k].HasAnyThing || tmpLeftCountToTransfer[transferables[k]] <= 0)
				{
					continue;
				}
				for (int l = 0; l < tmpLeftToLoadCopy.Count; l++)
				{
					TransferableOneWay transferableOneWay = TransferableUtility.TransferableMatching(transferables[k].AnyThing, tmpLeftToLoadCopy[l], TransferAsOneMode.PodsOrCaravanPacking);
					if (transferableOneWay != null)
					{
						int num = Mathf.Min(tmpLeftCountToTransfer[transferables[k]], transferableOneWay.CountToTransfer);
						if (num > 0)
						{
							transporters[l].AddToTheToLoadList(transferables[k], num);
							tmpLeftCountToTransfer[transferables[k]] -= num;
						}
					}
					for (int m = 0; m < transporters[l].innerContainer.Count; m++)
					{
						Thing thing = transporters[l].innerContainer[m];
						if (TransferableUtility.TransferAsOne(transferables[k].AnyThing, thing, TransferAsOneMode.PodsOrCaravanPacking))
						{
							int num2 = Mathf.Min(tmpLeftCountToTransfer[transferables[k]], thing.stackCount);
							if (num2 > 0)
							{
								transporters[l].AddToTheToLoadList(transferables[k], num2);
								tmpLeftCountToTransfer[transferables[k]] -= num2;
							}
						}
					}
				}
			}
		}
		tmpLeftToLoadCopy.Clear();
		if (transferables.Any())
		{
			transferables.SortByDescending((TransferableOneWay x) => x.AnyThing.GetStatValue(StatDefOf.Mass) * (float)tmpLeftCountToTransfer[x]);
			foreach (TransferableOneWay transferable in transferables)
			{
				if (tmpLeftCountToTransfer[transferable] == 0)
				{
					continue;
				}
				TransferableOneWay transferableOneWay2 = transferable;
				int num3 = tmpLeftCountToTransfer[transferableOneWay2];
				int num4 = Mathf.CeilToInt((float)num3 / (float)transporters.Count);
				int num5 = int.MaxValue;
				int num6 = 0;
				for (int n = 0; n < transporters.Count; n++)
				{
					if (transporters[n].leftToLoad == null)
					{
						num6 = n;
						break;
					}
					if (transporters[n].leftToLoad.Count < num5)
					{
						num5 = transporters[n].leftToLoad.Count;
						num6 = n;
					}
				}
				for (int num7 = 0; num7 < transporters.Count; num7++)
				{
					if (num3 == 0)
					{
						break;
					}
					int num8 = ((num7 == transporters.Count - 1) ? num3 : num4);
					if (num8 > 0)
					{
						transporters[num6].AddToTheToLoadList(transferableOneWay2, num8);
					}
					num3 -= num8;
					num6 = (num6 + 1) % transporters.Count;
				}
				tmpLeftCountToTransfer[transferableOneWay2] = 0;
			}
		}
		tmpLeftCountToTransfer.Clear();
		for (int num9 = 0; num9 < transporters.Count; num9++)
		{
			for (int num10 = transporters[num9].innerContainer.Count - 1; num10 >= 0; num10--)
			{
				Thing thing2 = transporters[num9].innerContainer[num10];
				int num11 = transporters[num9].SubtractFromToLoadList(thing2, thing2.stackCount, sendMessageOnFinished: false);
				if (num11 < thing2.stackCount)
				{
					transporters[num9].Notify_ThingRemoved(thing2);
					if (!transporters[num9].innerContainer.TryDrop(thing2, ThingPlaceMode.Near, thing2.stackCount - num11, out var _))
					{
						Log.Error("Transporter failed to drop " + (thing2.stackCount - num11) + " of " + thing2);
					}
				}
			}
		}
	}

	private bool CheckForErrors(List<Pawn> pawns)
	{
		if (!CanChangeAssignedThingsAfterStarting && !transferables.Any((TransferableOneWay x) => x.CountToTransfer != 0))
		{
			if (transporters[0].Props.max1PerGroup)
			{
				Messages.Message("CantSendEmptyTransporterSingle".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				Messages.Message("CantSendEmptyTransportPods".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			return false;
		}
		if (transporters[0].Props.max1PerGroup)
		{
			CompShuttle compShuttle = transporters.AsShuttle();
			if (compShuttle != null && compShuttle.requiredColonistCount > 0 && pawns.Count > compShuttle.requiredColonistCount)
			{
				Messages.Message("TransporterSingleTooManyColonists".Translate(compShuttle.requiredColonistCount), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
		}
		if (MassUsage > MassCapacity)
		{
			FlashMass();
			if (transporters[0].Props.max1PerGroup)
			{
				Messages.Message("TooBigTransporterSingleMassUsage".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				Messages.Message("TooBigTransportersMassUsage".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			return false;
		}
		Pawn pawn = pawns.Find((Pawn x) => !x.MapHeld.reachability.CanReach(x.PositionHeld, transporters[0].parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors)) && !transporters.Any((CompTransporter y) => y.innerContainer.Contains(x)));
		if (pawn != null)
		{
			if (transporters[0].Props.max1PerGroup)
			{
				Messages.Message("PawnCantReachTransporterSingle".Translate(pawn.LabelShort, pawn).CapitalizeFirst(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				Messages.Message("PawnCantReachTransporters".Translate(pawn.LabelShort, pawn).CapitalizeFirst(), MessageTypeDefOf.RejectInput, historical: false);
			}
			return false;
		}
		Map map = transporters[0].parent.Map;
		for (int i = 0; i < transferables.Count; i++)
		{
			if (transferables[i].ThingDef.category != ThingCategory.Item)
			{
				continue;
			}
			int countToTransfer = transferables[i].CountToTransfer;
			int num = 0;
			if (countToTransfer <= 0)
			{
				continue;
			}
			for (int j = 0; j < transferables[i].things.Count; j++)
			{
				Thing t = transferables[i].things[j];
				Pawn_CarryTracker pawn_CarryTracker = t.ParentHolder as Pawn_CarryTracker;
				if (map.reachability.CanReach(t.PositionHeld, transporters[0].parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors)) || transporters.Any((CompTransporter x) => x.innerContainer.Contains(t)) || (pawn_CarryTracker != null && pawn_CarryTracker.pawn.MapHeld.reachability.CanReach(pawn_CarryTracker.pawn.PositionHeld, transporters[0].parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors))))
				{
					num += t.stackCount;
					if (num >= countToTransfer)
					{
						break;
					}
				}
			}
			if (num >= countToTransfer)
			{
				continue;
			}
			if (countToTransfer == 1)
			{
				if (transporters[0].Props.max1PerGroup)
				{
					Messages.Message("TransporterSingleItemIsUnreachableSingle".Translate(transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput, historical: false);
				}
				else
				{
					Messages.Message("TransporterItemIsUnreachableSingle".Translate(transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			else if (transporters[0].Props.max1PerGroup)
			{
				Messages.Message("TransporterSingleItemIsUnreachableMulti".Translate(countToTransfer, transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				Messages.Message("TransporterItemIsUnreachableMulti".Translate(countToTransfer, transferables[i].ThingDef.label), MessageTypeDefOf.RejectInput, historical: false);
			}
			return false;
		}
		return true;
	}

	private void AddPawnsToTransferables()
	{
		foreach (Pawn item in TransporterUtility.AllSendablePawns(transporters, map))
		{
			AddToTransferables(item);
		}
	}

	private void AddItemsToTransferables()
	{
		foreach (Thing item in TransporterUtility.AllSendableItems(transporters, map))
		{
			AddToTransferables(item);
		}
	}

	private void FlashMass()
	{
		lastMassFlashTime = Time.time;
	}

	private void SetToLoadEverything()
	{
		for (int i = 0; i < transferables.Count; i++)
		{
			transferables[i].AdjustTo(transferables[i].GetMaximumToTransfer());
		}
		CountToTransferChanged();
	}

	private void CountToTransferChanged()
	{
		massUsageDirty = true;
		caravanMassUsageDirty = true;
		caravanMassCapacityDirty = true;
		tilesPerDayDirty = true;
		daysWorthOfFoodDirty = true;
		foragedFoodPerDayDirty = true;
		visibilityDirty = true;
	}

	public override void OnAcceptKeyPressed()
	{
		onAcceptButton();
	}
}
