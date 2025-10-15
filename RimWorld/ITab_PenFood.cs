using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_PenFood : ITab_PenBase
{
	private static readonly Vector2 WinSize = new Vector2(500f, 500f);

	private const int StatLineIndent = 8;

	private const int StatLabelColumnWidth = 210;

	private const float AboveTableMargin = 10f;

	private Vector2 animalPaneScrollPos;

	private readonly List<PenFoodCalculator.PenAnimalInfo> tmpAnimalInfos = new List<PenFoodCalculator.PenAnimalInfo>();

	public ITab_PenFood()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabPenFood";
	}

	public override void OnOpen()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.OnOpen();
		animalPaneScrollPos = Vector2.zero;
	}

	protected override void FillTab()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		CompAnimalPenMarker selectedCompAnimalPenMarker = base.SelectedCompAnimalPenMarker;
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		if (selectedCompAnimalPenMarker.PenState.Unenclosed)
		{
			Widgets.NoneLabelCenteredVertically(rect, "(" + "PenFoodTab_NotEnclosed".Translate() + ")");
			return;
		}
		PenFoodCalculator penFoodCalculator = selectedCompAnimalPenMarker.PenFoodCalculator;
		Widgets.BeginGroup(rect);
		float curY = 0f;
		DrawTopPane(ref curY, ((Rect)(ref rect)).width, penFoodCalculator);
		float height = ((Rect)(ref rect)).height - curY;
		DrawAnimalPane(ref curY, ((Rect)(ref rect)).width, height, penFoodCalculator, selectedCompAnimalPenMarker.parent.Map);
		Widgets.EndGroup();
	}

	private void DrawTopPane(ref float curY, float width, PenFoodCalculator calc)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		float num = calc.SumNutritionConsumptionPerDay - calc.NutritionPerDayToday;
		bool flag = num > 0f;
		DrawStatLine("PenSizeLabel".Translate(), calc.PenSizeDescription(), ref curY, width);
		DrawStatLine("PenFoodTab_NaturalNutritionGrowthRate".Translate(), PenFoodCalculator.NutritionPerDayToString(calc.NutritionPerDayToday), ref curY, width, calc.NaturalGrowthRateTooltip, flag ? new Color?(Color.red) : null);
		DrawStatLine("PenFoodTab_TotalNutritionConsumptionRate".Translate(), PenFoodCalculator.NutritionPerDayToString(calc.SumNutritionConsumptionPerDay), ref curY, width, calc.TotalConsumedToolTop, flag ? new Color?(Color.red) : null);
		if (!(calc.sumStockpiledNutritionAvailableNow > 0f))
		{
			return;
		}
		DrawStatLine("PenFoodTab_StockpileTotal".Translate(), PenFoodCalculator.NutritionToString(calc.sumStockpiledNutritionAvailableNow, withUnits: false), ref curY, width, calc.StockpileToolTip);
		if (flag)
		{
			int num2 = Mathf.FloorToInt(calc.sumStockpiledNutritionAvailableNow / num);
			DrawStatLine("PenFoodTab_StockpileEmptyDays".Translate(), num2.ToString(), ref curY, width, () => "PenFoodTab_StockpileEmptyDaysDescription".Translate(), Color.red);
		}
	}

	private void DrawStatLine(string label, string value, ref float curY, float width, Func<string> toolipGetter = null, Color? valueColor = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		float lineHeight = Text.LineHeight;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(8f, curY, width, lineHeight);
		rect.SplitVertically(210f, out var left, out var right);
		Widgets.Label(left, label);
		GUI.color = (Color)(((_003F?)valueColor) ?? Color.white);
		Widgets.Label(right, value);
		GUI.color = Color.white;
		if (Mouse.IsOver(rect) && toolipGetter != null)
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, toolipGetter, Gen.HashCombineInt(10192384, label.GetHashCode()));
		}
		curY += lineHeight;
	}

	private void DrawAnimalPane(ref float curYOuter, float width, float height, PenFoodCalculator calc, Map map)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		float cellWidth2 = width - 328f;
		float curY = curYOuter;
		float num = curY;
		float num2 = Mathf.Max(Text.LineHeight, 27f);
		float num3 = Text.LineHeightOf(GameFont.Small) + 10f;
		float num4 = num2;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, curY, width, height - (curY - num) - num4);
		rect.SplitHorizontally(num3, out var top, out var bottom);
		float x2 = ((Rect)(ref top)).x;
		curY = ((Rect)(ref top)).y;
		DrawIconCell(null, ref x2, 53f, num3);
		DrawCell("PenFoodTab_AnimalType".Translate(), ref x2, cellWidth2, num3, (TextAnchor)6);
		DrawCell("PenFoodTab_Count".Translate(), ref x2, 100f, num3, (TextAnchor)7);
		DrawCell("PenFoodTab_NutritionConsumedPerDay_ColumLabel".Translate(), ref x2, 120f, num3, (TextAnchor)7, null, () => "PenFoodTab_NutritionConsumedPerDay_ColumnTooltip".Translate());
		GUI.color = Widgets.SeparatorLineColor;
		Widgets.DrawLineHorizontal(0f, ((Rect)(ref top)).yMax - 1f, width);
		GUI.color = Color.white;
		tmpAnimalInfos.Clear();
		tmpAnimalInfos.AddRange(calc.ActualAnimalInfos);
		tmpAnimalInfos.AddRange(calc.ComputeExampleAnimals(base.SelectedCompAnimalPenMarker.ForceDisplayedAnimalDefs));
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(((Rect)(ref bottom)).x, ((Rect)(ref bottom)).y, ((Rect)(ref bottom)).width - 16f, (float)tmpAnimalInfos.Count * num2);
		Widgets.BeginScrollView(bottom, ref animalPaneScrollPos, viewRect);
		curY = ((Rect)(ref viewRect)).y;
		int num5 = 0;
		Rect rect2 = default(Rect);
		foreach (PenFoodCalculator.PenAnimalInfo info2 in tmpAnimalInfos)
		{
			float x3 = ((Rect)(ref viewRect)).x;
			((Rect)(ref rect2))._002Ector(x3, curY, ((Rect)(ref viewRect)).width, num2);
			if (num5 % 2 == 1)
			{
				Widgets.DrawLightHighlight(rect2);
			}
			DrawIconCell(info2.animalDef, ref x3, 53f, num2);
			DrawCell(info2.animalDef.LabelCap, ref x3, cellWidth2, num2, (TextAnchor)3);
			if (!info2.example)
			{
				DrawCell(info2.TotalCount.ToString(), ref x3, 100f, num2, (TextAnchor)4);
				DrawCell(PenFoodCalculator.NutritionPerDayToString(info2.TotalNutritionConsumptionPerDay, withUnits: false), ref x3, 120f, num2, (TextAnchor)4);
			}
			else
			{
				float num6 = SimplifiedPastureNutritionSimulator.NutritionConsumedPerDay(info2.animalDef);
				int num7 = Mathf.FloorToInt(calc.NutritionPerDayToday / num6);
				DrawCell("max".Translate() + " " + num7.ToString(), ref x3, 100f, num2, (TextAnchor)4, Color.grey);
				DrawCell("PenFoodTab_NutritionConsumedEachAnimalLabel".Translate(PenFoodCalculator.NutritionPerDayToString(num6, withUnits: false).Named("CONSUMEDAMOUNT")), ref x3, 120f, num2, (TextAnchor)4, Color.grey);
				DrawExampleAnimalControls(info2, ref x3, 27f, num2);
			}
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, () => info2.ToolTip(calc), 9477435);
			}
			curY += ((Rect)(ref rect2)).height;
			num5++;
		}
		Widgets.EndScrollView();
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x, Mathf.Min(((Rect)(ref rect)).yMax, curY), ((Rect)(ref rect)).width, num4);
		Widgets.Dropdown(rect3.LeftPart(0.35f), calc, (PenFoodCalculator calculator) => (ThingDef)null, MenuGenerator, "PenFoodTab_AddAnimal".Translate());
		curY = ((Rect)(ref rect3)).yMax;
		curYOuter = curY;
		void DrawCell(string label, ref float x, float cellWidth, float cellHeight, TextAnchor anchor = 0, Color? color = null, Func<string> tooltip = null)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (label != null)
			{
				Rect rect4 = default(Rect);
				((Rect)(ref rect4))._002Ector(x, curY, cellWidth, cellHeight);
				Text.Anchor = anchor;
				if (color.HasValue)
				{
					GUI.color = color.Value;
				}
				Widgets.Label(rect4, label);
				Text.Anchor = (TextAnchor)0;
				GUI.color = Color.white;
				if (tooltip != null && Mouse.IsOver(rect4))
				{
					Widgets.DrawHighlight(rect4);
					TooltipHandler.TipRegion(rect4, tooltip, 7578334);
				}
			}
			x += cellWidth + 4f;
		}
		void DrawExampleAnimalControls(PenFoodCalculator.PenAnimalInfo info, ref float x, float cellWidth, float cellHeight)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (Widgets.ButtonImage(new Rect(x, curY, cellWidth, cellHeight), TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
			{
				RemoveAnimal(calc, info);
			}
			x += cellWidth + 4f;
		}
		void DrawIconCell(ThingDef thingDef, ref float x, float cellWidth, float cellHeight)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (thingDef != null)
			{
				Rect rect5 = default(Rect);
				((Rect)(ref rect5))._002Ector(x, curY, 27f, 27f);
				Widgets.ThingIcon(rect5, thingDef);
				((Rect)(ref rect5)).x = ((Rect)(ref rect5)).x + 29f;
				Widgets.InfoCardButton(((Rect)(ref rect5)).x, ((Rect)(ref rect5)).y + 2f, thingDef);
			}
			x += cellWidth + 4f;
		}
		IEnumerable<Widgets.DropdownMenuElement<ThingDef>> MenuGenerator(PenFoodCalculator calculator)
		{
			foreach (ThingDef animal in map.plantGrowthRateCalculator.GrazingAnimals)
			{
				if (!base.SelectedCompAnimalPenMarker.ForceDisplayedAnimalDefs.Contains(animal))
				{
					Widgets.DropdownMenuElement<ThingDef> dropdownMenuElement = default(Widgets.DropdownMenuElement<ThingDef>);
					dropdownMenuElement.option = new FloatMenuOption(animal.LabelCap, delegate
					{
						AddExampleAnimal(calculator, animal);
					}, animal);
					dropdownMenuElement.payload = animal;
					yield return dropdownMenuElement;
				}
			}
		}
	}

	private void RemoveAnimal(PenFoodCalculator calc, PenFoodCalculator.PenAnimalInfo info)
	{
		base.SelectedCompAnimalPenMarker.RemoveForceDisplayedAnimal(info.animalDef);
	}

	private void AddExampleAnimal(PenFoodCalculator calc, ThingDef animal)
	{
		base.SelectedCompAnimalPenMarker.AddForceDisplayedAnimal(animal);
	}
}
