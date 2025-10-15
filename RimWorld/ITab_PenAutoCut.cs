using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class ITab_PenAutoCut : ITab_PenBase
{
	private static readonly Vector2 WinSize = new Vector2(300f, 480f);

	private const float AutoCutRowHeight = 24f;

	private const int CutNowButtonWidth = 150;

	private const int CutNowButtonHeight = 27;

	private ThingFilterUI.UIState plantFilterState = new ThingFilterUI.UIState();

	public ITab_PenAutoCut()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabPenAutoCut";
	}

	public override void OnOpen()
	{
		base.OnOpen();
		plantFilterState.quickSearch.Reset();
	}

	protected override void FillTab()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		CompAnimalPenMarker selectedCompAnimalPenMarker = base.SelectedCompAnimalPenMarker;
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		Widgets.BeginGroup(rect);
		float curY = 0f;
		DrawAutoCutOptions(ref curY, ((Rect)(ref rect)).width, selectedCompAnimalPenMarker);
		curY += 4f;
		DrawPlantFilter(ref curY, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - curY, selectedCompAnimalPenMarker);
		Widgets.EndGroup();
	}

	private void DrawPlantFilter(ref float curY, float width, float height, CompAnimalPenMarker marker)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ThingFilterUI.DoThingFilterConfigWindow(new Rect(0f, curY, width, height), plantFilterState, marker.AutoCutFilter, marker.parent.Map.animalPenManager.GetFixedAutoCutFilter(), 1, null, map: marker.parent.Map, forceHiddenFilters: HiddenSpecialThingFilters(), forceHideHitPointsConfig: true);
	}

	private void DrawAutoCutOptions(ref float curY, float width, CompAnimalPenMarker marker)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		bool enclosed = marker.PenState.Enclosed;
		Designator_PlantsCut designator_PlantsCut = Find.ReverseDesignatorDatabase.Get<Designator_PlantsCut>();
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, 24f, 24f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref val)).xMax + 4f, curY, width, 24f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, ((Rect)(ref rect)).yMax + 4f, 150f, 27f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, curY, width - 18f, 55f);
		Widgets.DrawHighlightIfMouseover(rect3);
		if (!enclosed)
		{
			GUI.color = Widgets.InactiveColor;
		}
		GUI.DrawTexture(val, designator_PlantsCut.icon);
		Widgets.CheckboxLabeled(rect, "PenAutoCut_EnabledCheckbox".Translate(), ref marker.autoCut, disabled: false, null, null, placeCheckboxNearText: true);
		GUI.color = Color.white;
		if (enclosed)
		{
			if (Widgets.ButtonText(rect2, "AutoCutNow".Translate()))
			{
				marker.DesignatePlantsToCut();
				designator_PlantsCut.soundSucceeded?.PlayOneShotOnCamera();
			}
		}
		else
		{
			GUI.color = ColorLibrary.RedReadable;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect2, "AutocutUnenclosedPen".Translate());
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
		}
		if (Mouse.IsOver(rect3))
		{
			TaggedString tooltip = "PenAutoCut_EnabledCheckboxTip".Translate();
			if (!enclosed)
			{
				tooltip += "\n\n" + "AutocutUnenclosedPenTip".Translate().Colorize(ColorLibrary.RedReadable);
			}
			TooltipHandler.TipRegion(rect, () => tooltip.Resolve(), 19727181);
		}
		curY = ((Rect)(ref rect2)).yMax;
	}

	private IEnumerable<SpecialThingFilterDef> HiddenSpecialThingFilters()
	{
		yield return SpecialThingFilterDefOf.AllowFresh;
		if (ModsConfig.IdeologyActive)
		{
			yield return SpecialThingFilterDefOf.AllowVegetarian;
			yield return SpecialThingFilterDefOf.AllowCarnivore;
			yield return SpecialThingFilterDefOf.AllowCannibal;
			yield return SpecialThingFilterDefOf.AllowInsectMeat;
		}
	}
}
