using UnityEngine;
using Verse;

namespace RimWorld;

public static class ScenarioUI
{
	private static float editViewHeight;

	public static void DrawScenarioInfo(Rect rect, Scenario scen, ref Vector2 infoScrollPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		rect = rect.GetInnerRect();
		if (scen != null)
		{
			string fullInformationText = scen.GetFullInformationText();
			float num = ((Rect)(ref rect)).width - 16f;
			float num2 = 30f + Text.CalcHeight(fullInformationText, num) + 100f;
			Rect viewRect = default(Rect);
			((Rect)(ref viewRect))._002Ector(0f, 0f, num, num2);
			Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);
			Text.Font = GameFont.Medium;
			Widgets.Label(new Rect(0f, 0f, ((Rect)(ref viewRect)).width, 30f), scen.name);
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(0f, 30f, ((Rect)(ref viewRect)).width, ((Rect)(ref viewRect)).height - 30f), fullInformationText);
			Widgets.EndScrollView();
		}
	}

	public static void DrawScenarioEditInterface(Rect rect, Scenario scen, ref Vector2 infoScrollPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		rect = rect.GetInnerRect();
		if (scen == null)
		{
			return;
		}
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, editViewHeight);
		Widgets.BeginScrollView(rect, ref infoScrollPosition, viewRect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 0f, ((Rect)(ref viewRect)).width, 99999f);
		Listing_ScenEdit listing_ScenEdit = new Listing_ScenEdit(scen);
		listing_ScenEdit.ColumnWidth = ((Rect)(ref rect2)).width;
		listing_ScenEdit.Begin(rect2);
		listing_ScenEdit.Label("ScenarioTitle".Translate());
		scen.name = listing_ScenEdit.TextEntry(scen.name).TrimmedToLength(55);
		listing_ScenEdit.Label("Summary".Translate());
		scen.summary = listing_ScenEdit.TextEntry(scen.summary, 2).TrimmedToLength(300);
		listing_ScenEdit.Label("Description".Translate());
		scen.description = listing_ScenEdit.TextEntry(scen.description, 4).TrimmedToLength(1000);
		listing_ScenEdit.Gap();
		foreach (ScenPart allPart in scen.AllParts)
		{
			allPart.DoEditInterface(listing_ScenEdit);
		}
		listing_ScenEdit.End();
		editViewHeight = listing_ScenEdit.CurHeight + 100f;
		Widgets.EndScrollView();
	}
}
