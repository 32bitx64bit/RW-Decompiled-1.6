using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public class WITab_Caravan_Health : WITab
{
	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private Pawn specificHealthTabForPawn;

	private bool compactMode;

	private static List<PawnCapacityDef> capacitiesToDisplay = new List<PawnCapacityDef>();

	private const float RowHeight = 40f;

	private const float PawnLabelHeight = 18f;

	private const float PawnLabelColumnWidth = 100f;

	private const float SpaceAroundIcon = 4f;

	private const float PawnCapacityColumnWidth = 100f;

	private const float BeCarriedIfSickColumnWidth = 40f;

	private const float BeCarriedIfSickIconSize = 24f;

	private static readonly Texture2D BeCarriedIfSickIcon = ContentFinder<Texture2D>.Get("UI/Icons/CarrySick");

	private List<Pawn> Pawns => base.SelCaravan.PawnsListForReading;

	private List<PawnCapacityDef> CapacitiesToDisplay
	{
		get
		{
			capacitiesToDisplay.Clear();
			List<PawnCapacityDef> allDefsListForReading = DefDatabase<PawnCapacityDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				if (allDefsListForReading[i].showOnCaravanHealthTab)
				{
					capacitiesToDisplay.Add(allDefsListForReading[i]);
				}
			}
			capacitiesToDisplay.SortBy((PawnCapacityDef x) => x.listOrder);
			return capacitiesToDisplay;
		}
	}

	private float SpecificHealthTabWidth
	{
		get
		{
			EnsureSpecificHealthTabForPawnValid();
			if (specificHealthTabForPawn.DestroyedOrNull())
			{
				return 0f;
			}
			return 630f;
		}
	}

	public WITab_Caravan_Health()
	{
		labelKey = "TabCaravanHealth";
	}

	protected override void FillTab()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		EnsureSpecificHealthTabForPawnValid();
		Text.Font = GameFont.Small;
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 10f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, scrollViewHeight);
		float curY = 0f;
		Widgets.BeginScrollView(val, ref scrollPosition, val2);
		DoColumnHeaders(ref curY);
		DoRows(ref curY, val2, val);
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	protected override void UpdateSize()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificHealthTabForPawnValid();
		base.UpdateSize();
		size = GetRawSize(compactMode: false);
		if (size.x + SpecificHealthTabWidth > (float)UI.screenWidth)
		{
			compactMode = true;
			size = GetRawSize(compactMode: true);
		}
		else
		{
			compactMode = false;
		}
	}

	protected override void ExtraOnGUI()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificHealthTabForPawnValid();
		base.ExtraOnGUI();
		Pawn localSpecificHealthTabForPawn = specificHealthTabForPawn;
		if (localSpecificHealthTabForPawn == null)
		{
			return;
		}
		Rect tabRect = base.TabRect;
		float specificHealthTabWidth = SpecificHealthTabWidth;
		Rect rect = new Rect(((Rect)(ref tabRect)).xMax - 1f, ((Rect)(ref tabRect)).yMin, specificHealthTabWidth, ((Rect)(ref tabRect)).height);
		Find.WindowStack.ImmediateWindow(1439870015, rect, WindowLayer.GameUI, delegate
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (!localSpecificHealthTabForPawn.DestroyedOrNull())
			{
				HealthCardUtility.DrawPawnHealthCard(new Rect(Vector2.zero, ((Rect)(ref rect)).size), localSpecificHealthTabForPawn, allowOperations: false, showBloodLoss: true, localSpecificHealthTabForPawn);
				if (Widgets.CloseButtonFor(rect.AtZero()))
				{
					specificHealthTabForPawn = null;
					SoundDefOf.TabClose.PlayOneShotOnCamera();
				}
			}
		});
	}

	private void DoColumnHeaders(ref float curY)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (!compactMode)
		{
			float num = 135f;
			Text.Anchor = (TextAnchor)1;
			GUI.color = Widgets.SeparatorLabelColor;
			Widgets.Label(new Rect(num, 3f, 100f, 100f), "Pain".Translate());
			num += 100f;
			List<PawnCapacityDef> list = CapacitiesToDisplay;
			for (int i = 0; i < list.Count; i++)
			{
				Widgets.Label(new Rect(num, 3f, 100f, 100f), list[i].LabelCap.Truncate(100f));
				num += 100f;
			}
			Rect val = new Rect(num + 8f, 0f, 24f, 24f);
			GUI.DrawTexture(val, (Texture)(object)BeCarriedIfSickIcon);
			TooltipHandler.TipRegionByKey(val, "BeCarriedIfSickTip");
			num += 40f;
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
	}

	private void DoRows(ref float curY, Rect scrollViewRect, Rect scrollOutRect)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		List<Pawn> pawns = Pawns;
		if (specificHealthTabForPawn != null && !pawns.Contains(specificHealthTabForPawn))
		{
			specificHealthTabForPawn = null;
		}
		bool flag = false;
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn = pawns[i];
			if (pawn.IsColonist)
			{
				if (!flag)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, "CaravanColonists".Translate());
					flag = true;
				}
				DoRow(ref curY, scrollViewRect, scrollOutRect, pawn);
			}
		}
		bool flag2 = false;
		for (int j = 0; j < pawns.Count; j++)
		{
			Pawn pawn2 = pawns[j];
			if (!pawn2.IsColonist)
			{
				if (!flag2)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref scrollViewRect)).width, ModsConfig.BiotechActive ? "CaravanPrisonersAnimalsAndMechs".Translate() : "CaravanPrisonersAndAnimals".Translate());
					flag2 = true;
				}
				DoRow(ref curY, scrollViewRect, scrollOutRect, pawn2);
			}
		}
	}

	private Vector2 GetRawSize(bool compactMode)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		float num = 100f;
		if (!compactMode)
		{
			num += 100f;
			num += (float)CapacitiesToDisplay.Count * 100f;
			num += 40f;
		}
		Vector2 result = default(Vector2);
		result.x = 127f + num + 16f;
		result.y = Mathf.Min(550f, PaneTopY - 30f);
		return result;
	}

	private void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Pawn p)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float num = scrollPosition.y - 40f;
		float num2 = scrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 40f), p);
		}
		curY += 40f;
	}

	private void DoRow(Rect rect, Pawn p)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		CaravanThingsTabUtility.DoAbandonButton(val, p, base.SelCaravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, p);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoOpenSpecificTabButton(val, p, ref specificHealthTabForPawn);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect2, p);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(((Rect)(ref rect2)).xMax + 4f, 11f, 100f, 18f);
		GenMapUI.DrawPawnLabel(p, bgRect, 1f, 100f, null, GameFont.Small, alwaysDrawBg: false, alignCenter: false);
		float num = ((Rect)(ref bgRect)).xMax;
		if (!compactMode)
		{
			if (p.RaceProps.IsFlesh)
			{
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(num, 0f, 100f, 40f);
				DoPain(rect3, p);
			}
			num += 100f;
			List<PawnCapacityDef> list = CapacitiesToDisplay;
			Rect rect4 = default(Rect);
			for (int i = 0; i < list.Count; i++)
			{
				((Rect)(ref rect4))._002Ector(num, 0f, 100f, 40f);
				if ((p.RaceProps.Humanlike && !list[i].showOnHumanlikes) || (p.RaceProps.Animal && !list[i].showOnAnimals) || (p.RaceProps.IsAnomalyEntity && !list[i].showOnAnomalyEntities) || (p.RaceProps.IsMechanoid && !list[i].showOnMechanoids) || (p.RaceProps.IsDrone && !list[i].showOnDrones) || !PawnCapacityUtility.BodyCanEverDoCapacity(p.RaceProps.body, list[i]))
				{
					num += 100f;
					continue;
				}
				DoCapacity(rect4, p, list[i]);
				num += 100f;
			}
		}
		if (!compactMode)
		{
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(num + 8f, 8f);
			Widgets.Checkbox(val2, ref p.health.beCarriedByCaravanIfSick, 24f, disabled: false, paintable: true);
			TooltipHandler.TipRegionByKey(new Rect(val2, new Vector2(24f, 24f)), "BeCarriedIfSickTip");
			num += 40f;
		}
		if (p.Downed && !p.ageTracker.CurLifeStage.alwaysDowned)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	private void DoPain(Rect rect, Pawn pawn)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Pair<string, Color> painLabel = HealthCardUtility.GetPainLabel(pawn);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		GUI.color = painLabel.Second;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, painLabel.First);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect))
		{
			string painTip = HealthCardUtility.GetPainTip(pawn);
			TooltipHandler.TipRegion(rect, painTip);
		}
	}

	private void DoCapacity(Rect rect, Pawn pawn, PawnCapacityDef capacity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Pair<string, Color> efficiencyLabel = HealthCardUtility.GetEfficiencyLabel(pawn, capacity);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		GUI.color = efficiencyLabel.Second;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, efficiencyLabel.First);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect))
		{
			string pawnCapacityTip = HealthCardUtility.GetPawnCapacityTip(pawn, capacity);
			TooltipHandler.TipRegion(rect, pawnCapacityTip);
		}
	}

	public override void Notify_ClearingAllMapsMemory()
	{
		base.Notify_ClearingAllMapsMemory();
		specificHealthTabForPawn = null;
	}

	private void EnsureSpecificHealthTabForPawnValid()
	{
		if (specificHealthTabForPawn != null && (specificHealthTabForPawn.Destroyed || !base.SelCaravan.ContainsPawn(specificHealthTabForPawn)))
		{
			specificHealthTabForPawn = null;
		}
	}
}
