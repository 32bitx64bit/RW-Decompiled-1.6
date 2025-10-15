using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

public class WITab_Caravan_Gear : WITab
{
	private Vector2 leftPaneScrollPosition;

	private float leftPaneScrollViewHeight;

	private Vector2 rightPaneScrollPosition;

	private float rightPaneScrollViewHeight;

	private Thing draggedItem;

	private Vector2 draggedItemPosOffset;

	private bool droppedDraggedItem;

	private float leftPaneWidth;

	private float rightPaneWidth;

	private const float PawnRowHeight = 40f;

	private const float ItemRowHeight = 30f;

	private const float PawnLabelHeight = 18f;

	private const float PawnLabelColumnWidth = 100f;

	private const float GearLabelColumnWidth = 250f;

	private const float SpaceAroundIcon = 4f;

	private const float EquippedGearColumnWidth = 250f;

	private const float EquippedGearIconSize = 32f;

	private static List<Apparel> tmpApparel = new List<Apparel>();

	private static List<ThingWithComps> tmpExistingEquipment = new List<ThingWithComps>();

	private static List<Apparel> tmpExistingApparel = new List<Apparel>();

	private List<Pawn> Pawns => base.SelCaravan.PawnsListForReading;

	public WITab_Caravan_Gear()
	{
		labelKey = "TabCaravanGear";
	}

	protected override void UpdateSize()
	{
		base.UpdateSize();
		leftPaneWidth = 469f;
		rightPaneWidth = 345f;
		size.x = leftPaneWidth + rightPaneWidth;
		size.y = Mathf.Min(550f, PaneTopY - 30f);
	}

	public override void OnOpen()
	{
		base.OnOpen();
		draggedItem = null;
	}

	protected override void FillTab()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		CheckDraggedItemStillValid();
		CheckDropDraggedItem();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, leftPaneWidth, size.y);
		Widgets.BeginGroup(rect);
		DoLeftPane();
		Widgets.EndGroup();
		Widgets.BeginGroup(new Rect(((Rect)(ref rect)).xMax, 0f, rightPaneWidth, size.y));
		DoRightPane();
		Widgets.EndGroup();
		if (draggedItem != null && droppedDraggedItem)
		{
			droppedDraggedItem = false;
			draggedItem = null;
		}
	}

	private void DoLeftPane()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, leftPaneWidth, size.y), 10f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, leftPaneScrollViewHeight);
		float curY = 0f;
		Widgets.BeginScrollView(val, ref leftPaneScrollPosition, val2);
		DoPawnRows(ref curY, val2, val);
		if ((int)Event.current.type == 8)
		{
			leftPaneScrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	private void DoRightPane()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, rightPaneWidth, size.y), 10f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, rightPaneScrollViewHeight);
		if (draggedItem != null && ((Rect)(ref val)).Contains(Event.current.mousePosition) && CurrentWearerOf(draggedItem) != null)
		{
			Widgets.DrawHighlight(val);
			if (droppedDraggedItem)
			{
				MoveDraggedItemToInventory();
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			}
		}
		float curY = 0f;
		Widgets.BeginScrollView(val, ref rightPaneScrollPosition, val2);
		DoInventoryRows(ref curY, val2, val);
		if ((int)Event.current.type == 8)
		{
			rightPaneScrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	protected override void ExtraOnGUI()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		base.ExtraOnGUI();
		if (draggedItem != null)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			Rect rect = new Rect(mousePosition.x - draggedItemPosOffset.x, mousePosition.y - draggedItemPosOffset.y, 32f, 32f);
			Find.WindowStack.ImmediateWindow(1283641090, rect, WindowLayer.Super, delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				if (draggedItem != null)
				{
					Widgets.ThingIcon(rect.AtZero(), draggedItem);
				}
			}, doBackground: false, absorbInputAroundWindow: false, 0f);
		}
		CheckDropDraggedItem();
	}

	private void DoPawnRows(ref float curY, Rect scrollViewRect, Rect scrollOutRect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		List<Pawn> pawns = Pawns;
		Text.Font = GameFont.Tiny;
		GUI.color = Color.gray;
		Widgets.Label(new Rect(135f, curY + 6f, 200f, 100f), "DragToRearrange".Translate());
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanColonists".Translate());
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn = pawns[i];
			if (pawn.IsColonist)
			{
				DoPawnRow(ref curY, scrollViewRect, scrollOutRect, pawn);
			}
		}
		bool flag = false;
		for (int j = 0; j < pawns.Count; j++)
		{
			Pawn pawn2 = pawns[j];
			if (pawn2.IsPrisoner)
			{
				if (!flag)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanPrisoners".Translate());
					flag = true;
				}
				DoPawnRow(ref curY, scrollViewRect, scrollOutRect, pawn2);
			}
		}
	}

	private void DoPawnRow(ref float curY, Rect viewRect, Rect scrollOutRect, Pawn p)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = leftPaneScrollPosition.y - 40f;
		float num2 = leftPaneScrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoPawnRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 40f), p);
		}
		curY += 40f;
	}

	private void DoPawnRow(Rect rect, Pawn p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		CaravanThingsTabUtility.DoAbandonButton(val, p, base.SelCaravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, p);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		bool flag = draggedItem != null && ((Rect)(ref val)).Contains(Event.current.mousePosition) && CurrentWearerOf(draggedItem) != p;
		if ((Mouse.IsOver(val) && draggedItem == null) || flag)
		{
			Widgets.DrawHighlight(val);
		}
		if (flag && droppedDraggedItem)
		{
			TryEquipDraggedItem(p);
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect2, p);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(((Rect)(ref rect2)).xMax + 4f, 11f, 100f, 18f);
		GenMapUI.DrawPawnLabel(p, bgRect, 1f, 100f, null, GameFont.Small, alwaysDrawBg: false, alignCenter: false);
		float curX = ((Rect)(ref bgRect)).xMax;
		if (p.equipment != null)
		{
			List<ThingWithComps> allEquipmentListForReading = p.equipment.AllEquipmentListForReading;
			for (int i = 0; i < allEquipmentListForReading.Count; i++)
			{
				DoEquippedGear(allEquipmentListForReading[i], p, ref curX);
			}
		}
		if (p.apparel != null)
		{
			tmpApparel.Clear();
			tmpApparel.AddRange(p.apparel.WornApparel);
			tmpApparel.SortBy((Apparel x) => x.def.apparel.LastLayer.drawOrder, (Apparel x) => 0f - x.def.apparel.HumanBodyCoverage);
			for (int j = 0; j < tmpApparel.Count; j++)
			{
				DoEquippedGear(tmpApparel[j], p, ref curX);
			}
		}
		if (p.Downed && !p.ageTracker.CurLifeStage.alwaysDowned)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	private void DoInventoryRows(ref float curY, Rect scrollViewRect, Rect scrollOutRect)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		List<Thing> list = CaravanInventoryUtility.AllInventoryItems(base.SelCaravan);
		Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanWeaponsAndApparel".Translate());
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			Thing thing = list[i];
			if (IsVisibleWeapon(thing.def))
			{
				if (!flag)
				{
					flag = true;
				}
				DoInventoryRow(ref curY, scrollViewRect, scrollOutRect, thing);
			}
		}
		bool flag2 = false;
		for (int j = 0; j < list.Count; j++)
		{
			Thing thing2 = list[j];
			if (thing2.def.IsApparel)
			{
				if (!flag2)
				{
					flag2 = true;
				}
				DoInventoryRow(ref curY, scrollViewRect, scrollOutRect, thing2);
			}
		}
		if (!flag && !flag2)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref scrollViewRect)).width);
		}
	}

	private void DoInventoryRow(ref float curY, Rect viewRect, Rect scrollOutRect, Thing t)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = rightPaneScrollPosition.y - 30f;
		float num2 = rightPaneScrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoInventoryRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 30f), t);
		}
		curY += 30f;
	}

	private void DoInventoryRow(Rect rect, Thing t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect rect2 = rect.AtZero();
		Widgets.InfoCardButton(((Rect)(ref rect2)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, t);
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - 24f;
		if (draggedItem == null && Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
		}
		float num = ((t == draggedItem) ? 0.5f : 1f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect3, t, num);
		GUI.color = new Color(1f, 1f, 1f, num);
		Rect rect4 = new Rect(((Rect)(ref rect3)).xMax + 4f, 0f, 250f, 30f);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Widgets.Label(rect4, t.LabelCap);
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		GUI.color = Color.white;
		if ((int)Event.current.type == 0 && Event.current.button == 0 && Mouse.IsOver(rect2))
		{
			draggedItem = t;
			droppedDraggedItem = false;
			draggedItemPosOffset = new Vector2(16f, 16f);
			Event.current.Use();
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		Widgets.EndGroup();
	}

	private void DoEquippedGear(Thing t, Pawn p, ref float curX)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, 4f, 32f, 32f);
		bool flag = Mouse.IsOver(rect);
		Widgets.ThingIcon(alpha: (t == draggedItem) ? 0.2f : ((!flag || draggedItem != null) ? 1f : 0.75f), rect: rect, thing: t);
		curX += 32f;
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, t.LabelCap);
		}
		if ((int)Event.current.type == 0 && Event.current.button == 0 && flag)
		{
			draggedItem = t;
			droppedDraggedItem = false;
			draggedItemPosOffset = Event.current.mousePosition - ((Rect)(ref rect)).position;
			Event.current.Use();
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
	}

	private void CheckDraggedItemStillValid()
	{
		if (draggedItem != null)
		{
			if (draggedItem.Destroyed)
			{
				draggedItem = null;
			}
			else if (CurrentWearerOf(draggedItem) == null && !CaravanInventoryUtility.AllInventoryItems(base.SelCaravan).Contains(draggedItem))
			{
				draggedItem = null;
			}
		}
	}

	private void CheckDropDraggedItem()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		if (draggedItem != null && ((int)Event.current.type == 1 || (int)Event.current.rawType == 1))
		{
			droppedDraggedItem = true;
		}
	}

	private bool IsVisibleWeapon(ThingDef t)
	{
		if (t.IsWeapon && t != ThingDefOf.WoodLog)
		{
			return t != ThingDefOf.Beer;
		}
		return false;
	}

	private Pawn CurrentWearerOf(Thing t)
	{
		IThingHolder parentHolder = t.ParentHolder;
		if (parentHolder is Pawn_EquipmentTracker || parentHolder is Pawn_ApparelTracker)
		{
			return (Pawn)parentHolder.ParentHolder;
		}
		return null;
	}

	private void MoveDraggedItemToInventory()
	{
		droppedDraggedItem = false;
		if (draggedItem is Apparel apparel && CurrentWearerOf(apparel) != null && CurrentWearerOf(apparel).apparel.IsLocked(apparel))
		{
			Messages.Message("MessageCantUnequipLockedApparel".Translate(), CurrentWearerOf(apparel), MessageTypeDefOf.RejectInput, historical: false);
			draggedItem = null;
			return;
		}
		Pawn pawn = CaravanInventoryUtility.FindPawnToMoveInventoryTo(draggedItem, Pawns, null);
		if (pawn != null)
		{
			draggedItem.holdingOwner.TryTransferToContainer(draggedItem, pawn.inventory.innerContainer, 1);
		}
		else
		{
			Log.Warning("Could not find any pawn to move " + draggedItem?.ToString() + " to.");
		}
		draggedItem = null;
	}

	private void TryEquipDraggedItem(Pawn p)
	{
		droppedDraggedItem = false;
		if (!EquipmentUtility.CanEquip(draggedItem, p, out var cantReason))
		{
			Messages.Message("MessageCantEquipCustom".Translate(cantReason.CapitalizeFirst()), p, MessageTypeDefOf.RejectInput, historical: false);
			draggedItem = null;
			return;
		}
		if (draggedItem.def.IsWeapon)
		{
			if (p.guest.IsPrisoner)
			{
				Messages.Message("MessageCantEquipCustom".Translate("MessagePrisonerCannotEquipWeapon".Translate(p.Named("PAWN"))), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			if (p.WorkTagIsDisabled(WorkTags.Violent))
			{
				Messages.Message("MessageCantEquipIncapableOfViolence".Translate(p.LabelShort, p), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			if (p.WorkTagIsDisabled(WorkTags.Shooting) && draggedItem.def.IsRangedWeapon)
			{
				Messages.Message("MessageCantEquipIncapableOfShooting".Translate(p.LabelShort, p), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			if (!p.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				Messages.Message("MessageCantEquipIncapableOfManipulation".Translate(), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
		}
		Apparel apparel = draggedItem as Apparel;
		ThingWithComps thingWithComps = draggedItem as ThingWithComps;
		if (apparel != null && p.apparel != null)
		{
			if (!ApparelUtility.HasPartsToWear(p, apparel.def))
			{
				Messages.Message("MessageCantWearApparelMissingBodyParts".Translate(p.LabelShort, p), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			if (CurrentWearerOf(apparel) != null && CurrentWearerOf(apparel).apparel.IsLocked(apparel))
			{
				Messages.Message("MessageCantUnequipLockedApparel".Translate(), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			if (p.apparel.WouldReplaceLockedApparel(apparel))
			{
				Messages.Message("MessageWouldReplaceLockedApparel".Translate(p.LabelShort, p), p, MessageTypeDefOf.RejectInput, historical: false);
				draggedItem = null;
				return;
			}
			tmpExistingApparel.Clear();
			tmpExistingApparel.AddRange(p.apparel.WornApparel);
			for (int i = 0; i < tmpExistingApparel.Count; i++)
			{
				if (!ApparelUtility.CanWearTogether(apparel.def, tmpExistingApparel[i].def, p.RaceProps.body))
				{
					p.apparel.Remove(tmpExistingApparel[i]);
					Pawn pawn = CaravanInventoryUtility.FindPawnToMoveInventoryTo(tmpExistingApparel[i], Pawns, null);
					if (pawn != null)
					{
						pawn.inventory.innerContainer.TryAdd(tmpExistingApparel[i]);
						continue;
					}
					Log.Warning("Could not find any pawn to move " + tmpExistingApparel[i]?.ToString() + " to.");
					tmpExistingApparel[i].Destroy();
				}
			}
			p.apparel.Wear((Apparel)apparel.SplitOff(1), dropReplacedApparel: false);
			if (p.outfits != null)
			{
				p.outfits.forcedHandler.SetForced(apparel, forced: true);
			}
		}
		else if (thingWithComps != null && p.equipment != null)
		{
			string personaWeaponConfirmationText = EquipmentUtility.GetPersonaWeaponConfirmationText(draggedItem, p);
			if (!personaWeaponConfirmationText.NullOrEmpty())
			{
				_ = draggedItem;
				Find.WindowStack.Add(new Dialog_MessageBox(personaWeaponConfirmationText, "Yes".Translate(), delegate
				{
					AddEquipment();
				}, "No".Translate()));
				draggedItem = null;
				return;
			}
			AddEquipment();
		}
		else
		{
			Log.Warning("Could not make " + p?.ToString() + " equip or wear " + draggedItem);
		}
		draggedItem = null;
		void AddEquipment()
		{
			tmpExistingEquipment.Clear();
			tmpExistingEquipment.AddRange(p.equipment.AllEquipmentListForReading);
			for (int j = 0; j < tmpExistingEquipment.Count; j++)
			{
				p.equipment.Remove(tmpExistingEquipment[j]);
				Pawn pawn2 = CaravanInventoryUtility.FindPawnToMoveInventoryTo(tmpExistingEquipment[j], Pawns, null);
				if (pawn2 != null)
				{
					pawn2.inventory.innerContainer.TryAdd(tmpExistingEquipment[j]);
				}
				else
				{
					Log.Warning("Could not find any pawn to move " + tmpExistingEquipment[j]?.ToString() + " to.");
					tmpExistingEquipment[j].Destroy();
				}
			}
			p.equipment.AddEquipment((ThingWithComps)thingWithComps.SplitOff(1));
		}
	}
}
