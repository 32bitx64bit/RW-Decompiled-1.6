using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

public class ITab_Pawn_Gear : ITab
{
	private Vector2 scrollPosition = Vector2.zero;

	private float scrollViewHeight;

	private const float TopPadding = 20f;

	public static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

	public static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

	private const float ThingIconSize = 28f;

	private const float ThingRowHeight = 28f;

	private const float ThingLeftX = 36f;

	private const float StandardLineHeight = 22f;

	private const float InitialHeight = 450f;

	private static List<Thing> workingInvList = new List<Thing>();

	private static List<Thing> workingEquipmentList = new List<Thing>();

	public override bool IsVisible
	{
		get
		{
			Pawn selPawnForGear = SelPawnForGear;
			if (selPawnForGear.ageTracker.CurLifeStage == LifeStageDefOf.HumanlikeBaby || (ModsConfig.AnomalyActive && selPawnForGear.RaceProps.IsAnomalyEntity))
			{
				return false;
			}
			if (ModsConfig.OdysseyActive && selPawnForGear.RaceProps.IsDrone)
			{
				return false;
			}
			if (!ShouldShowInventory(selPawnForGear) && !ShouldShowApparel(selPawnForGear))
			{
				return ShouldShowEquipment(selPawnForGear);
			}
			return true;
		}
	}

	private bool CanControl
	{
		get
		{
			Pawn selPawnForGear = SelPawnForGear;
			if (selPawnForGear.Downed || selPawnForGear.InMentalState || selPawnForGear.CarriedBy != null)
			{
				return false;
			}
			if (selPawnForGear.Faction != Faction.OfPlayer && !selPawnForGear.IsPrisonerOfColony)
			{
				return false;
			}
			if (selPawnForGear.IsPrisonerOfColony && selPawnForGear.Spawned && !selPawnForGear.Map.mapPawns.AnyFreeColonistSpawned)
			{
				return false;
			}
			if (selPawnForGear.IsPrisonerOfColony && (PrisonBreakUtility.IsPrisonBreaking(selPawnForGear) || (selPawnForGear.CurJob != null && selPawnForGear.CurJob.exitMapOnArrival)))
			{
				return false;
			}
			return true;
		}
	}

	private bool CanControlColonist
	{
		get
		{
			if (CanControl)
			{
				return SelPawnForGear.IsColonistPlayerControlled;
			}
			return false;
		}
	}

	private Pawn SelPawnForGear
	{
		get
		{
			if (SelPawn != null)
			{
				return SelPawn;
			}
			if (base.SelThing is Corpse corpse)
			{
				return corpse.InnerPawn;
			}
			throw new InvalidOperationException("Gear tab on non-pawn non-corpse " + base.SelThing);
		}
	}

	public ITab_Pawn_Gear()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(460f, 450f);
		labelKey = "TabGear";
		tutorTag = "Gear";
	}

	protected override void FillTab()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Invalid comparison between Unknown and I4
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 20f, size.x, size.y - 20f);
		Rect val = rect.ContractedBy(10f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height);
		if (Prefs.DevMode && Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - 18f - 125f, 5f, 115f, Text.LineHeight), "Dev tool..."))
		{
			Find.WindowStack.Add(new FloatMenu(DebugToolsPawns.PawnGearDevOptions(SelPawnForGear)));
		}
		Widgets.BeginGroup(rect2);
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		Rect outRect = new Rect(0f, 0f, ((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect2)).width - 16f, scrollViewHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float curY = 0f;
		TryDrawMassInfo(ref curY, ((Rect)(ref viewRect)).width);
		TryDrawComfyTemperatureRange(ref curY, ((Rect)(ref viewRect)).width);
		if (ShouldShowOverallArmor(SelPawnForGear))
		{
			Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "OverallArmor".Translate());
			TryDrawOverallArmor(ref curY, ((Rect)(ref viewRect)).width, StatDefOf.ArmorRating_Sharp, "ArmorSharp".Translate());
			TryDrawOverallArmor(ref curY, ((Rect)(ref viewRect)).width, StatDefOf.ArmorRating_Blunt, "ArmorBlunt".Translate());
			TryDrawOverallArmor(ref curY, ((Rect)(ref viewRect)).width, StatDefOf.ArmorRating_Heat, "ArmorHeat".Translate());
		}
		if (ShouldShowEquipment(SelPawnForGear))
		{
			Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "Equipment".Translate());
			workingEquipmentList.Clear();
			workingEquipmentList.AddRange(SelPawnForGear.equipment.AllEquipmentListForReading);
			workingEquipmentList.AddRange(SelPawnForGear.apparel.WornApparel.Where((Apparel x) => x.def.apparel.layers.Contains(ApparelLayerDefOf.Belt)));
			foreach (Thing workingEquipment in workingEquipmentList)
			{
				DrawThingRow(ref curY, ((Rect)(ref viewRect)).width, workingEquipment);
			}
		}
		if (ShouldShowApparel(SelPawnForGear))
		{
			Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "Apparel".Translate());
			foreach (Apparel item in from x in SelPawnForGear.apparel.WornApparel
				where !x.def.apparel.layers.Contains(ApparelLayerDefOf.Belt)
				select x into ap
				orderby ap.def.apparel.bodyPartGroups[0].listOrder descending
				select ap)
			{
				DrawThingRow(ref curY, ((Rect)(ref viewRect)).width, item);
			}
		}
		if (ShouldShowInventory(SelPawnForGear))
		{
			Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "Inventory".Translate());
			workingInvList.Clear();
			workingInvList.AddRange(SelPawnForGear.inventory.innerContainer);
			for (int i = 0; i < workingInvList.Count; i++)
			{
				DrawThingRow(ref curY, ((Rect)(ref viewRect)).width, workingInvList[i], inventory: true);
			}
			workingInvList.Clear();
		}
		if ((int)Event.current.type == 8)
		{
			if (curY + 70f > 450f)
			{
				size.y = Mathf.Min(curY + 70f, (float)(UI.screenHeight - 35) - 165f - 30f);
			}
			else
			{
				size.y = 450f;
			}
			scrollViewHeight = curY + 20f;
		}
		Widgets.EndScrollView();
		Widgets.EndGroup();
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
	}

	private void DrawThingRow(ref float y, float width, Thing thing, bool inventory = false)
	{
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, y, width, 28f);
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, y, thing);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		bool flag = false;
		if (CanControl && (inventory || CanControlColonist || (SelPawnForGear.Spawned && !SelPawnForGear.Map.IsPlayerHome)))
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref val)).width - 24f, y, 24f, 24f);
			bool flag2 = false;
			if (SelPawnForGear.IsQuestLodger())
			{
				flag2 = inventory || !EquipmentUtility.QuestLodgerCanUnequip(thing, SelPawnForGear);
			}
			bool flag3 = !inventory && SelPawnForGear.kindDef.destroyGearOnDrop;
			bool flag4 = thing is Apparel apparel && SelPawnForGear.apparel != null && SelPawnForGear.apparel.IsLocked(apparel);
			flag = flag2 || flag4 || flag3;
			if (Mouse.IsOver(val2))
			{
				if (flag4)
				{
					TooltipHandler.TipRegion(val2, "DropThingLocked".Translate());
				}
				else if (flag2)
				{
					TooltipHandler.TipRegion(val2, "DropThingLodger".Translate());
				}
				else
				{
					TooltipHandler.TipRegion(val2, "DropThing".Translate());
				}
			}
			Color val3 = (flag ? Color.grey : Color.white);
			Color mouseoverColor = (flag ? val3 : GenUI.MouseoverColor);
			if (Widgets.ButtonImage(val2, TexButton.Drop, val3, mouseoverColor, !flag) && !flag)
			{
				Action action = delegate
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					InterfaceDrop(thing);
				};
				if (!ModsConfig.BiotechActive || !MechanitorUtility.TryConfirmBandwidthLossFromDroppingThing(SelPawnForGear, thing, action))
				{
					action();
				}
			}
			((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		}
		if (CanControlColonist)
		{
			if (FoodUtility.WillIngestFromInventoryNow(SelPawnForGear, thing))
			{
				Rect val4 = new Rect(((Rect)(ref val)).width - 24f, y, 24f, 24f);
				TooltipHandler.TipRegionByKey(val4, "ConsumeThing", thing.LabelNoCount, thing);
				if (Widgets.ButtonImage(val4, TexButton.Ingest))
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					FoodUtility.IngestFromInventoryNow(SelPawnForGear, thing);
				}
			}
			((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		}
		Rect rect = val;
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMax - 60f;
		CaravanThingsTabUtility.DrawMass(thing, rect);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 60f;
		if (Mouse.IsOver(val))
		{
			GUI.color = HighlightColor;
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		if ((Object)(object)thing.def.DrawMatSingle != (Object)null && (Object)(object)thing.def.DrawMatSingle.mainTexture != (Object)null)
		{
			Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing);
		}
		Text.Anchor = (TextAnchor)3;
		GUI.color = ThingLabelColor;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(36f, y, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		string text = thing.LabelCap;
		if (thing is Apparel ap && SelPawnForGear.outfits != null && SelPawnForGear.outfits.forcedHandler.IsForced(ap))
		{
			text += ", " + "ApparelForcedLower".Translate();
		}
		if (flag)
		{
			text += " (" + "ApparelLockedLower".Translate() + ")";
		}
		Text.WordWrap = false;
		Widgets.Label(rect2, text.Truncate(((Rect)(ref rect2)).width));
		Text.WordWrap = true;
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, thing.GetTooltip());
		}
		y += 28f;
	}

	private void TryDrawOverallArmor(ref float curY, float width, StatDef stat, string label)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = Mathf.Clamp01(SelPawnForGear.GetStatValue(stat) / 2f);
		List<BodyPartRecord> allParts = SelPawnForGear.RaceProps.body.AllParts;
		List<Apparel> list = ((SelPawnForGear.apparel != null) ? SelPawnForGear.apparel.WornApparel : null);
		for (int i = 0; i < allParts.Count; i++)
		{
			float num3 = 1f - num2;
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].def.apparel.CoversBodyPart(allParts[i]))
					{
						float num4 = Mathf.Clamp01(list[j].GetStatValue(stat) / 2f);
						num3 *= 1f - num4;
					}
				}
			}
			num += allParts[i].coverageAbs * (1f - num3);
		}
		num = Mathf.Clamp(num * 2f, 0f, 2f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, curY, width, 100f);
		Widgets.Label(rect, label.Truncate(120f));
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 120f;
		Widgets.Label(rect, num.ToStringPercent());
		curY += 22f;
	}

	private void TryDrawMassInfo(ref float curY, float width)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!SelPawnForGear.Dead && ShouldShowInventory(SelPawnForGear))
		{
			Rect rect = new Rect(0f, curY, width, 22f);
			float num = MassUtility.GearAndInventoryMass(SelPawnForGear);
			float num2 = MassUtility.Capacity(SelPawnForGear);
			Widgets.Label(rect, "MassCarried".Translate(num.ToString("0.##"), num2.ToString("0.##")));
			curY += 22f;
		}
	}

	private void TryDrawComfyTemperatureRange(ref float curY, float width)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!SelPawnForGear.Dead)
		{
			Rect rect = new Rect(0f, curY, width, 22f);
			float statValue = SelPawnForGear.GetStatValue(StatDefOf.ComfyTemperatureMin);
			float statValue2 = SelPawnForGear.GetStatValue(StatDefOf.ComfyTemperatureMax);
			Widgets.Label(rect, "ComfyTemperatureRange".Translate() + ": " + statValue.ToStringTemperature("F0") + " ~ " + statValue2.ToStringTemperature("F0"));
			curY += 22f;
		}
	}

	private void InterfaceDrop(Thing t)
	{
		ThingWithComps thingWithComps = t as ThingWithComps;
		if (t is Apparel apparel && SelPawnForGear.apparel != null && SelPawnForGear.apparel.WornApparel.Contains(apparel))
		{
			SelPawnForGear.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.RemoveApparel, apparel), JobTag.Misc);
		}
		else if (thingWithComps != null && SelPawnForGear.equipment != null && SelPawnForGear.equipment.AllEquipmentListForReading.Contains(thingWithComps))
		{
			SelPawnForGear.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.DropEquipment, thingWithComps), JobTag.Misc);
		}
		else if (!t.def.destroyOnDrop)
		{
			SelPawnForGear.inventory.innerContainer.TryDrop(t, SelPawnForGear.Position, SelPawnForGear.Map, ThingPlaceMode.Near, out var _);
		}
	}

	[Obsolete("Will be removed in a future update, use FoodUtility.IngestFromInventoryNow()")]
	private void InterfaceIngest(Thing t)
	{
		FoodUtility.IngestFromInventoryNow(SelPawnForGear, t);
	}

	private bool ShouldShowInventory(Pawn p)
	{
		if (!p.RaceProps.Humanlike)
		{
			return p.inventory.innerContainer.Any;
		}
		return true;
	}

	private bool ShouldShowApparel(Pawn p)
	{
		if (p.apparel == null)
		{
			return false;
		}
		if (!p.RaceProps.Humanlike)
		{
			return p.apparel.WornApparel.Any();
		}
		return true;
	}

	private bool ShouldShowEquipment(Pawn p)
	{
		return p.equipment != null;
	}

	private bool ShouldShowOverallArmor(Pawn p)
	{
		if (!p.RaceProps.Humanlike && !ShouldShowApparel(p) && !(p.GetStatValue(StatDefOf.ArmorRating_Sharp) > 0f) && !(p.GetStatValue(StatDefOf.ArmorRating_Blunt) > 0f))
		{
			return p.GetStatValue(StatDefOf.ArmorRating_Heat) > 0f;
		}
		return true;
	}
}
