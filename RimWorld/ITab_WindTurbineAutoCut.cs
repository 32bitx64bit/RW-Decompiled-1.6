using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class ITab_WindTurbineAutoCut : ITab
{
	private static readonly Vector2 WinSize = new Vector2(300f, 480f);

	private const int CutNowButtonWidth = 150;

	private const int CutNowButtonHeight = 27;

	private const float AutoCutRowHeight = 24f;

	private ThingFilterUI.UIState plantFilterState = new ThingFilterUI.UIState();

	public CompAutoCut AutoCut => base.SelThing?.TryGetComp<CompAutoCut>();

	public override bool IsVisible => AutoCut != null;

	public ITab_WindTurbineAutoCut()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabWindTurbineAutoCut";
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
		CompAutoCut autoCut = AutoCut;
		Rect rect = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		Widgets.BeginGroup(rect);
		float curY = 0f;
		DrawAutoCutOptions(ref curY, ((Rect)(ref rect)).width, autoCut);
		curY += 4f;
		DrawPlantFilter(ref curY, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - curY, autoCut);
		Widgets.EndGroup();
	}

	private void DrawPlantFilter(ref float curY, float width, float height, CompAutoCut autoCut)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ThingFilterUI.DoThingFilterConfigWindow(new Rect(0f, curY, width, height), plantFilterState, autoCut.AutoCutFilter, autoCut.GetFixedAutoCutFilter(), 1, null, map: autoCut.parent.Map, forceHiddenFilters: HiddenSpecialThingFilters(), forceHideHitPointsConfig: true);
	}

	private void DrawAutoCutOptions(ref float curY, float width, CompAutoCut autoCut)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Designator_PlantsCut designator_PlantsCut = Find.ReverseDesignatorDatabase.Get<Designator_PlantsCut>();
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, 24f, 24f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref val)).xMax + 4f, curY, width, 24f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, ((Rect)(ref rect)).yMax + 4f, 150f, 27f);
		GUI.DrawTexture(val, designator_PlantsCut.icon);
		Text.Font = GameFont.Tiny;
		Widgets.CheckboxLabeled(rect, "WindTurbineAutoCut_EnabledCheckbox".Translate(), ref autoCut.autoCut, disabled: false, null, null, placeCheckboxNearText: true);
		Text.Font = GameFont.Small;
		if (Widgets.ButtonText(rect2, "AutoCutNow".Translate()))
		{
			autoCut.DesignatePlantsToCut();
			designator_PlantsCut.soundSucceeded?.PlayOneShotOnCamera();
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
