using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class NeedsCardUtility
{
	private static List<Need> displayNeeds = new List<Need>();

	public static readonly Color MoodColor = new Color(0.1f, 1f, 0.1f);

	public static readonly Color MoodColorNegative = new Color(0.8f, 0.4f, 0.4f);

	private static readonly Color NoEffectColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

	private const float ThoughtHeight = 20f;

	private const float ThoughtSpacing = 4f;

	private const float ThoughtIntervalY = 24f;

	private const float MoodX = 235f;

	private const float MoodNumberWidth = 32f;

	private const float NeedsColumnWidth = 225f;

	public static readonly Vector2 FullSize = new Vector2(580f, 520f);

	private static List<Thought> thoughtGroupsPresent = new List<Thought>();

	private static List<Thought> thoughtGroup = new List<Thought>();

	public static Vector2 GetSize(Pawn pawn)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		UpdateDisplayNeeds(pawn);
		if (pawn.needs.mood != null)
		{
			return FullSize;
		}
		return new Vector2(225f, (float)displayNeeds.Count * Mathf.Min(70f, FullSize.y / (float)displayNeeds.Count));
	}

	public static void DoNeedsMoodAndThoughts(Rect rect, Pawn pawn, ref Vector2 thoughtScrollPosition)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, 225f, ((Rect)(ref rect)).height);
		DoNeeds(rect2, pawn);
		if (pawn.needs.mood != null)
		{
			DoMoodAndThoughts(new Rect(((Rect)(ref rect2)).xMax, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - ((Rect)(ref rect2)).width, ((Rect)(ref rect)).height), pawn, ref thoughtScrollPosition);
		}
	}

	public static void DoNeeds(Rect rect, Pawn pawn)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		UpdateDisplayNeeds(pawn);
		float num = 0f;
		Rect rect2 = default(Rect);
		for (int i = 0; i < displayNeeds.Count; i++)
		{
			Need need = displayNeeds[i];
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + num, ((Rect)(ref rect)).width, Mathf.Min(70f, ((Rect)(ref rect)).height / (float)displayNeeds.Count));
			if (!need.def.major)
			{
				if (i > 0 && displayNeeds[i - 1].def.major)
				{
					((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 10f;
				}
				((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width * 0.73f;
				((Rect)(ref rect2)).height = Mathf.Max(((Rect)(ref rect2)).height * 0.666f, 30f);
			}
			need.DrawOnGUI(rect2);
			num = ((Rect)(ref rect2)).yMax;
		}
	}

	private static void DoMoodAndThoughts(Rect rect, Pawn pawn, ref Vector2 thoughtScrollPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 0f, ((Rect)(ref rect)).width * 0.8f, 70f);
		pawn.needs.mood.DrawOnGUI(rect2);
		DrawThoughtListing(GenUI.ContractedBy(new Rect(0f, 80f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - 70f - 10f), 10f), pawn, ref thoughtScrollPosition);
		Widgets.EndGroup();
	}

	private static void UpdateDisplayNeeds(Pawn pawn)
	{
		displayNeeds.Clear();
		List<Need> allNeeds = pawn.needs.AllNeeds;
		for (int i = 0; i < allNeeds.Count; i++)
		{
			if (allNeeds[i].ShowOnNeedList)
			{
				displayNeeds.Add(allNeeds[i]);
			}
		}
		PawnNeedsUIUtility.SortInDisplayOrder(displayNeeds);
	}

	private static void DrawThoughtListing(Rect listingRect, Pawn pawn, ref Vector2 thoughtScrollPosition)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 8)
		{
			return;
		}
		Text.Font = GameFont.Small;
		IdeoUIUtility.DrawExtraThoughtInfoFromIdeo(pawn, ref listingRect);
		PawnNeedsUIUtility.GetThoughtGroupsInDisplayOrder(pawn.needs.mood, thoughtGroupsPresent);
		float num = (float)thoughtGroupsPresent.Count * 24f;
		Widgets.BeginScrollView(listingRect, ref thoughtScrollPosition, new Rect(0f, 0f, ((Rect)(ref listingRect)).width - 16f, num));
		Text.Anchor = (TextAnchor)3;
		float num2 = 0f;
		for (int i = 0; i < thoughtGroupsPresent.Count; i++)
		{
			if (DrawThoughtGroup(new Rect(0f, num2, ((Rect)(ref listingRect)).width - 16f, 20f), thoughtGroupsPresent[i], pawn))
			{
				num2 += 24f;
			}
		}
		Widgets.EndScrollView();
		Text.Anchor = (TextAnchor)0;
	}

	private static bool DrawThoughtGroup(Rect rect, Thought group, Pawn pawn)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			pawn.needs.mood.thoughts.GetMoodThoughts(group, thoughtGroup);
			if (thoughtGroup.Count == 0)
			{
				return false;
			}
			Thought leadingThought = PawnNeedsUIUtility.GetLeadingThoughtInGroup(thoughtGroup);
			if (!leadingThought.VisibleInNeedsTab)
			{
				thoughtGroup.Clear();
				return false;
			}
			if (leadingThought != thoughtGroup[0])
			{
				thoughtGroup.Remove(leadingThought);
				thoughtGroup.Insert(0, leadingThought);
			}
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
			}
			if (Mouse.IsOver(rect))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(leadingThought.LabelCap.AsTipTitle()).AppendLine().AppendLine();
				if (pawn.DevelopmentalStage.Baby())
				{
					stringBuilder.AppendLine(leadingThought.BabyTalk);
					stringBuilder.AppendLine();
					stringBuilder.AppendTagged(("Translation".Translate() + ": " + leadingThought.Description).Colorize(ColoredText.SubtleGrayColor));
				}
				else
				{
					stringBuilder.Append(leadingThought.Description);
				}
				int durationTicks = group.DurationTicks;
				if (durationTicks > 5)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					if (leadingThought is Thought_Memory thought_Memory)
					{
						if (thoughtGroup.Count == 1)
						{
							stringBuilder.AppendTagged("ThoughtExpiresIn".Translate((durationTicks - thought_Memory.age).ToStringTicksToPeriod()));
						}
						else
						{
							int num = int.MaxValue;
							int num2 = int.MinValue;
							foreach (Thought_Memory item in thoughtGroup)
							{
								num = Mathf.Min(num, item.age);
								num2 = Mathf.Max(num2, item.age);
							}
							stringBuilder.AppendTagged("ThoughtStartsExpiringIn".Translate((durationTicks - num2).ToStringTicksToPeriod()));
							stringBuilder.AppendLine();
							stringBuilder.AppendTagged("ThoughtFinishesExpiringIn".Translate((durationTicks - num).ToStringTicksToPeriod()));
						}
					}
				}
				if (thoughtGroup.Count > 1)
				{
					bool flag = false;
					for (int i = 1; i < thoughtGroup.Count; i++)
					{
						bool flag2 = false;
						for (int j = 0; j < i; j++)
						{
							if (thoughtGroup[i].LabelCap == thoughtGroup[j].LabelCap)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							if (!flag)
							{
								stringBuilder.AppendLine();
								stringBuilder.AppendLine();
								flag = true;
							}
							stringBuilder.AppendLine("+ " + thoughtGroup[i].LabelCap);
						}
					}
				}
				TooltipHandler.TipRegion(rect, new TipSignal(stringBuilder.ToString(), 7291));
			}
			Text.WordWrap = false;
			Text.Anchor = (TextAnchor)3;
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + 10f, ((Rect)(ref rect)).y, 225f, ((Rect)(ref rect)).height);
			((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - 3f;
			((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax + 3f;
			string text = leadingThought.LabelCap;
			if (thoughtGroup.Count > 1)
			{
				text = text + " x" + thoughtGroup.Count;
			}
			Widgets.Label(rect2, text);
			Text.Anchor = (TextAnchor)4;
			float num3 = pawn.needs.mood.thoughts.MoodOffsetOfGroup(group);
			if (num3 == 0f)
			{
				GUI.color = NoEffectColor;
			}
			else if (num3 > 0f)
			{
				GUI.color = MoodColor;
			}
			else
			{
				GUI.color = MoodColorNegative;
			}
			Widgets.Label(new Rect(((Rect)(ref rect)).x + 235f, ((Rect)(ref rect)).y, 32f, ((Rect)(ref rect)).height), num3.ToString("##0"));
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
			Text.WordWrap = true;
			if (ModsConfig.IdeologyActive && leadingThought.sourcePrecept != null && !Find.IdeoManager.classicMode)
			{
				IdeoUIUtility.DoIdeoIcon(new Rect(((Rect)(ref rect)).x + 235f + 32f + 10f, ((Rect)(ref rect)).y, 20f, 20f), leadingThought.sourcePrecept.ideo, doTooltip: false, delegate
				{
					IdeoUIUtility.OpenIdeoInfo(leadingThought.sourcePrecept.ideo);
				});
			}
		}
		catch (Exception ex)
		{
			Log.ErrorOnce("Exception in DrawThoughtGroup for " + group.def?.ToString() + " on " + pawn?.ToString() + ": " + ex, 3452698);
		}
		return true;
	}
}
