using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_PenAnimals : ITab_PenBase
{
	private static readonly Vector2 WinSize = new Vector2(300f, 480f);

	private ThingFilterUI.UIState animalFilterState = new ThingFilterUI.UIState();

	public ITab_PenAnimals()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabPenAnimals";
	}

	public override void OnOpen()
	{
		base.OnOpen();
		animalFilterState.quickSearch.Reset();
	}

	protected override void FillTab()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		CompAnimalPenMarker selectedCompAnimalPenMarker = base.SelectedCompAnimalPenMarker;
		Map map = selectedCompAnimalPenMarker.parent.Map;
		GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f).SplitHorizontally(18f, out var _, out var bottom);
		ThingFilterUI.DoThingFilterConfigWindow(bottom, animalFilterState, selectedCompAnimalPenMarker.AnimalFilter, AnimalPenUtility.GetFixedAnimalFilter(), 1, null, null, forceHideHitPointsConfig: false, forceHideQualityConfig: false, showMentalBreakChanceRange: false, null, map);
	}
}
