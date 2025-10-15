using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class CaravanNeedsTabUtility
{
	private const float RowHeight = 40f;

	private const float NeedExtraSize = 5f;

	private const float LabelHeight = 18f;

	private const float LabelColumnWidth = 100f;

	private const float NeedWidth = 100f;

	private const float NeedMargin = 10f;

	private static List<Need> tmpNeeds = new List<Need>();

	private static List<Thought> thoughtGroupsPresent = new List<Thought>();

	private static List<Thought> thoughtGroup = new List<Thought>();

	public static void DoRows(Vector2 size, List<Pawn> pawns, Caravan caravan, ref Vector2 scrollPosition, ref float scrollViewHeight, ref Pawn specificNeedsTabForPawn, bool doNeeds = true)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Invalid comparison between Unknown and I4
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (specificNeedsTabForPawn != null && (!pawns.Contains(specificNeedsTabForPawn) || specificNeedsTabForPawn.Dead))
		{
			specificNeedsTabForPawn = null;
		}
		Text.Font = GameFont.Small;
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 10f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, scrollViewHeight);
		Widgets.BeginScrollView(val, ref scrollPosition, viewRect);
		float curY = 0f;
		bool flag = false;
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn = pawns[i];
			if (pawn.IsColonist)
			{
				if (!flag)
				{
					Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "CaravanColonists".Translate());
					flag = true;
				}
				DoRow(ref curY, viewRect, val, scrollPosition, pawn, caravan, ref specificNeedsTabForPawn, doNeeds);
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
					Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, ModsConfig.BiotechActive ? "CaravanPrisonersAnimalsAndMechs".Translate() : "CaravanPrisonersAndAnimals".Translate());
					flag2 = true;
				}
				DoRow(ref curY, viewRect, val, scrollPosition, pawn2, caravan, ref specificNeedsTabForPawn, doNeeds);
			}
		}
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	public static Vector2 GetSize(List<Pawn> pawns, float paneTopY, bool doNeeds = true)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float num = 100f;
		if (doNeeds)
		{
			num += (float)MaxNeedsCount(pawns) * 100f;
		}
		num += 24f;
		Vector2 result = default(Vector2);
		result.x = 103f + num + 16f;
		result.y = Mathf.Min(550f, paneTopY - 30f);
		return result;
	}

	private static int MaxNeedsCount(List<Pawn> pawns)
	{
		int num = 0;
		for (int i = 0; i < pawns.Count; i++)
		{
			GetNeedsToDisplay(pawns[i], tmpNeeds);
			num = Mathf.Max(num, tmpNeeds.Count);
		}
		return num;
	}

	private static void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Vector2 scrollPosition, Pawn pawn, Caravan caravan, ref Pawn specificNeedsTabForPawn, bool doNeeds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = scrollPosition.y - 40f;
		float num2 = scrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 40f), pawn, caravan, ref specificNeedsTabForPawn, doNeeds);
		}
		curY += 40f;
	}

	private static void DoRow(Rect rect, Pawn pawn, Caravan caravan, ref Pawn specificNeedsTabForPawn, bool doNeeds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		CaravanThingsTabUtility.DoAbandonButton(val, pawn, caravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, pawn);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		if (!pawn.Dead)
		{
			CaravanThingsTabUtility.DoOpenSpecificTabButton(val, pawn, ref specificNeedsTabForPawn);
			((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
			CaravanThingsTabUtility.DoOpenSpecificTabButtonInvisible(val, pawn, ref specificNeedsTabForPawn);
		}
		Widgets.DrawHighlightIfMouseover(val);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect2, pawn);
		Rect bgRect = default(Rect);
		((Rect)(ref bgRect))._002Ector(((Rect)(ref rect2)).xMax + 4f, 11f, 100f, 18f);
		GenMapUI.DrawPawnLabel(pawn, bgRect, 1f, 100f, null, GameFont.Small, alwaysDrawBg: false, alignCenter: false);
		if (doNeeds)
		{
			GetNeedsToDisplay(pawn, tmpNeeds);
			float xMax = ((Rect)(ref bgRect)).xMax;
			Rect val2 = default(Rect);
			for (int i = 0; i < tmpNeeds.Count; i++)
			{
				Need need = tmpNeeds[i];
				int maxThresholdMarkers = 0;
				bool doTooltip = true;
				((Rect)(ref val2))._002Ector(xMax, 0f, 100f, 40f);
				Need_Mood mood = need as Need_Mood;
				if (mood != null)
				{
					maxThresholdMarkers = 1;
					doTooltip = false;
					if (Mouse.IsOver(val2))
					{
						TooltipHandler.TipRegion(val2, new TipSignal(() => CustomMoodNeedTooltip(mood), ((object)(Rect)(ref val2)).GetHashCode()));
					}
				}
				Rect rect3 = val2;
				((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin - 5f;
				((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax + 5f;
				need.DrawOnGUI(rect3, maxThresholdMarkers, 10f, drawArrows: false, doTooltip, val2);
				xMax = ((Rect)(ref val2)).xMax;
			}
		}
		if (pawn.Downed && !pawn.ageTracker.CurLifeStage.alwaysDowned)
		{
			GUI.color = new Color(1f, 0f, 0f, 0.5f);
			Widgets.DrawLineHorizontal(0f, ((Rect)(ref rect)).height / 2f, ((Rect)(ref rect)).width);
			GUI.color = Color.white;
		}
		Widgets.EndGroup();
	}

	private static void GetNeedsToDisplay(Pawn p, List<Need> outNeeds)
	{
		outNeeds.Clear();
		List<Need> allNeeds = p.needs.AllNeeds;
		for (int i = 0; i < allNeeds.Count; i++)
		{
			Need need = allNeeds[i];
			if (need.def.showForCaravanMembers)
			{
				outNeeds.Add(need);
			}
		}
		PawnNeedsUIUtility.SortInDisplayOrder(outNeeds);
	}

	private static string CustomMoodNeedTooltip(Need_Mood mood)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(mood.GetTipString());
		PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(mood, thoughtGroupsPresent);
		bool flag = false;
		for (int i = 0; i < thoughtGroupsPresent.Count; i++)
		{
			Thought group = thoughtGroupsPresent[i];
			mood.thoughts.GetMoodThoughts(group, thoughtGroup);
			Thought leadingThoughtInGroup = PawnNeedsUIUtility.GetLeadingThoughtInGroup(thoughtGroup);
			if (leadingThoughtInGroup.VisibleInNeedsTab)
			{
				if (!flag)
				{
					flag = true;
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(leadingThoughtInGroup.LabelCap);
				if (thoughtGroup.Count > 1)
				{
					stringBuilder.Append(" x");
					stringBuilder.Append(thoughtGroup.Count);
				}
				stringBuilder.Append(": ");
				stringBuilder.AppendLine(mood.thoughts.MoodOffsetOfGroup(group).ToString("##0"));
			}
		}
		return stringBuilder.ToString();
	}
}
