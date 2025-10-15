using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Fishing : ITab
{
	private string repeatCountEditBuffer;

	private string targetCountEditBuffer;

	private string unpauseAtCountEditBuffer;

	private static readonly Vector2 WinSize = new Vector2(300f, 450f);

	private const float TopSectionHeight = 200f;

	private const float MiddleSectionHeight = 80f;

	public Zone_Fishing SelZone => base.SelObject as Zone_Fishing;

	public ITab_Fishing()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabFishing";
	}

	protected override void FillTab()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		Zone_Fishing zone = SelZone;
		if (zone == null || zone.CellCount == 0)
		{
			return;
		}
		WaterBody waterBody = zone.Cells[0].GetWaterBody(zone.Map);
		Rect rect = base.TabRect.AtZero();
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 22f;
		rect = rect.ContractedBy(10f);
		Listing_Standard listing_Standard = new Listing_Standard(GameFont.Small);
		listing_Standard.Begin(rect);
		Listing_Standard listing_Standard2 = listing_Standard.BeginSection(200f);
		if (listing_Standard2.ButtonText(RepeatModeLabel(zone.repeatMode)))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (FishRepeatMode mode in Enum.GetValues(typeof(FishRepeatMode)))
			{
				list.Add(new FloatMenuOption(RepeatModeLabel(mode), delegate
				{
					zone.targetCount = 1;
					zone.repeatCount = 100;
					zone.pauseWhenSatisfied = false;
					zone.unpauseAtCount = 50;
					zone.repeatMode = mode;
					zone.RecheckPausedDueToResourceCount();
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		listing_Standard2.Gap(10f);
		switch (zone.repeatMode)
		{
		case FishRepeatMode.RepeatCount:
			listing_Standard2.Label(string.Concat("RepeatCount".Translate() + " ", zone.repeatCount.ToString()));
			listing_Standard2.IntEntry(ref zone.repeatCount, ref repeatCountEditBuffer);
			break;
		case FishRepeatMode.TargetCount:
		{
			listing_Standard2.Label(string.Concat("CurrentlyHave".Translate() + ": ", zone.OwnedFishCount.ToString(), " / ", zone.targetCount.ToString()));
			int targetCount = zone.targetCount;
			listing_Standard2.IntEntry(ref zone.targetCount, ref targetCountEditBuffer);
			if (targetCount != zone.targetCount)
			{
				zone.unpauseAtCount = Mathf.Max(0, zone.unpauseAtCount + (zone.targetCount - targetCount));
				unpauseAtCountEditBuffer = zone.unpauseAtCount.ToStringCached();
				zone.RecheckPausedDueToResourceCount();
			}
			listing_Standard2.Gap(10f);
			bool pauseWhenSatisfied = zone.pauseWhenSatisfied;
			listing_Standard2.CheckboxLabeled("PauseWhenSatisfied".Translate(), ref zone.pauseWhenSatisfied);
			if (zone.pauseWhenSatisfied != pauseWhenSatisfied)
			{
				zone.RecheckPausedDueToResourceCount();
			}
			if (zone.pauseWhenSatisfied)
			{
				listing_Standard2.Label("UnpauseWhenYouHave".Translate() + ": " + zone.unpauseAtCount.ToString("F0"));
				listing_Standard2.IntEntry(ref zone.unpauseAtCount, ref unpauseAtCountEditBuffer);
				if (zone.unpauseAtCount >= zone.targetCount)
				{
					zone.unpauseAtCount = Mathf.Max(0, zone.targetCount - 1);
					unpauseAtCountEditBuffer = zone.unpauseAtCount.ToStringCached();
					zone.RecheckPausedDueToResourceCount();
				}
			}
			break;
		}
		}
		listing_Standard.EndSection(listing_Standard2);
		listing_Standard.Gap(10f);
		Listing_Standard listing_Standard3 = listing_Standard.BeginSection(80f);
		int num = (int)waterBody.Population;
		int num2 = (int)waterBody.MaxPopulation;
		float min = 10f / (float)num2;
		string text = FishingUtility.FishPopulationLabel(num);
		listing_Standard3.Label(string.Format("{0}: {1} / {2} ({3})", "FishPopulation".Translate().CapitalizeFirst(), num, num2, text), -1f, (TipSignal?)(TipSignal)"FishPopulationDesc".Translate());
		listing_Standard3.Gap(10f);
		listing_Standard3.Label(string.Concat("MinimumPopulation".Translate() + ": ", Mathf.Round(zone.targetPopulationPct * (float)num2).ToString()), -1f, (TipSignal?)(TipSignal)"MinimumPopulationDesc".Translate());
		zone.targetPopulationPct = listing_Standard3.Slider(zone.targetPopulationPct, min, 1f);
		listing_Standard.EndSection(listing_Standard3);
		listing_Standard.Gap(10f);
		Listing_Standard listing = listing_Standard.BeginSection(((Rect)(ref rect)).height - listing_Standard.CurHeight - 10f);
		foreach (ThingDef commonFishIncludingExtra in waterBody.CommonFishIncludingExtras)
		{
			ListFish(commonFishIncludingExtra, listing, uncommon: false);
		}
		foreach (ThingDef item in waterBody.UncommonFish)
		{
			ListFish(item, listing, uncommon: true);
		}
		listing_Standard.EndSection(listing);
		listing_Standard.End();
	}

	private void ListFish(ThingDef fish, Listing_Standard listing, bool uncommon)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = listing.GetRect(Text.LineHeight);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, Text.LineHeight, Text.LineHeight);
		Widgets.InfoCardButton(rect2, fish);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).xMax + 4f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).height, ((Rect)(ref rect)).height);
		Widgets.ThingIcon(rect3, fish);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).xMax + 4f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).xMax - ((Rect)(ref rect3)).xMax - 4f, ((Rect)(ref rect)).height);
		TaggedString str = fish.LabelCap + (uncommon ? (" (" + "Uncommon".Translate().ToString() + ")") : string.Empty);
		Widgets.Label(rect4, str.Truncate(((Rect)(ref rect4)).width));
		listing.Gap(2f);
	}

	private string RepeatModeLabel(FishRepeatMode mode)
	{
		return mode switch
		{
			FishRepeatMode.RepeatCount => "FishRepeatMode_RepeatCount".Translate().CapitalizeFirst(), 
			FishRepeatMode.TargetCount => "FishRepeatMode_TargetCount".Translate().CapitalizeFirst(), 
			FishRepeatMode.DoForever => "FishRepeatMode_Forever".Translate().CapitalizeFirst(), 
			_ => "Unknown", 
		};
	}
}
