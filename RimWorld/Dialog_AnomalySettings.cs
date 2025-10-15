using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_AnomalySettings : Window
{
	private Difficulty difficulty;

	private float anomalyThreatsInactiveFraction;

	private float anomalyThreatsActiveFraction;

	private float studyEfficiencyFactor;

	private float overrideAnomalyThreatsFraction;

	private AnomalyPlaystyleDef anomalyPlaystyleDef;

	private Vector2 scrollPosition;

	private float scrollHeight;

	private Listing_Standard listing;

	private static readonly Vector2 AcceptButtonSize = new Vector2(100f, 35f);

	private const float DefaultOverrideThreatFraction = 0.15f;

	private const float MaxStudyEfficiencyFactor = 5f;

	private static (float, string)[] FrequencyLabels = new(float, string)[6]
	{
		(0f, "AnomalyFrequency_None"),
		(0.05f, "AnomalyFrequency_VeryRare"),
		(0.2f, "AnomalyFrequency_Rare"),
		(0.5f, "AnomalyFrequency_Balanced"),
		(0.75f, "AnomalyFrequency_Intense"),
		(1f, "AnomalyFrequency_Overwhelming")
	};

	public override Vector2 InitialSize => new Vector2(500f, 475f);

	public Dialog_AnomalySettings(Difficulty difficulty)
	{
		doCloseX = true;
		absorbInputAroundWindow = true;
		listing = new Listing_Standard();
		listing.maxOneColumn = true;
		this.difficulty = difficulty;
		anomalyThreatsInactiveFraction = difficulty.anomalyThreatsInactiveFraction;
		anomalyThreatsActiveFraction = difficulty.anomalyThreatsActiveFraction;
		overrideAnomalyThreatsFraction = difficulty.overrideAnomalyThreatsFraction.GetValueOrDefault();
		studyEfficiencyFactor = difficulty.studyEfficiencyFactor;
		anomalyPlaystyleDef = difficulty.AnomalyPlaystyleDef;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock(GameFont.Medium))
		{
			Widgets.Label(inRect, "AnomalySettings".Translate());
			((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + (Text.LineHeight + 10f);
		}
		Text.Font = GameFont.Small;
		if (Widgets.ButtonText(new Rect((((Rect)(ref inRect)).x + ((Rect)(ref inRect)).width - AcceptButtonSize.x) / 2f, ((Rect)(ref inRect)).yMax - AcceptButtonSize.y, AcceptButtonSize.x, AcceptButtonSize.y), "Accept".Translate()))
		{
			if (anomalyPlaystyleDef.overrideThreatFraction)
			{
				difficulty.overrideAnomalyThreatsFraction = overrideAnomalyThreatsFraction;
			}
			else
			{
				difficulty.overrideAnomalyThreatsFraction = null;
			}
			difficulty.anomalyThreatsInactiveFraction = anomalyThreatsInactiveFraction;
			difficulty.anomalyThreatsActiveFraction = anomalyThreatsActiveFraction;
			difficulty.studyEfficiencyFactor = studyEfficiencyFactor;
			difficulty.AnomalyPlaystyleDef = anomalyPlaystyleDef;
			Close();
		}
		((Rect)(ref inRect)).yMax = ((Rect)(ref inRect)).yMax - (AcceptButtonSize.y + 17f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - Text.LineHeight, ((Rect)(ref inRect)).width, Text.LineHeight);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 10f;
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 10f;
		((Rect)(ref rect)).height = Text.LineHeight + 5f;
		if (Widgets.ButtonText(rect, "SetToStandardPlaystyle".Translate() + "..."))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (DifficultyDef d in DefDatabase<DifficultyDef>.AllDefs)
			{
				if (!d.isCustom)
				{
					list.Add(new FloatMenuOption(d.LabelCap, delegate
					{
						anomalyThreatsInactiveFraction = d.anomalyThreatsInactiveFraction;
						anomalyThreatsActiveFraction = d.anomalyThreatsActiveFraction;
						studyEfficiencyFactor = d.studyEfficiencyFactor;
						anomalyPlaystyleDef = AnomalyPlaystyleDefOf.Standard;
					}));
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		((Rect)(ref inRect)).yMax = ((Rect)(ref inRect)).yMax - (((Rect)(ref rect)).height + 4f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, scrollHeight);
		Widgets.BeginScrollView(inRect, ref scrollPosition, val);
		Rect rect2 = val;
		((Rect)(ref rect2)).height = 99999f;
		listing.Begin(rect2);
		DrawPlaystyles();
		DrawExtraSettings();
		scrollHeight = listing.CurHeight;
		listing.End();
		Widgets.EndScrollView();
	}

	private void DrawPlaystyles()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		listing.Label("ChooseAnomalyPlaystyle".Translate());
		listing.Gap();
		foreach (AnomalyPlaystyleDef allDef in DefDatabase<AnomalyPlaystyleDef>.AllDefs)
		{
			bool flag = !Find.Scenario.standardAnomalyPlaystyleOnly || allDef == AnomalyPlaystyleDefOf.Standard;
			string text = allDef.LabelCap.AsTipTitle() + "\n" + allDef.description;
			if (!flag)
			{
				text = text + "\n\n" + ("DisabledByScenario".Translate() + ": " + Find.Scenario.name).Colorize(ColorLibrary.RedReadable);
			}
			if (listing.RadioButton(allDef.LabelCap, anomalyPlaystyleDef == allDef, 30f, 80f, text, 0f, !flag))
			{
				if (flag)
				{
					if (!anomalyPlaystyleDef.overrideThreatFraction && allDef.overrideThreatFraction)
					{
						overrideAnomalyThreatsFraction = 0.15f;
					}
					anomalyPlaystyleDef = allDef;
				}
				else
				{
					Messages.Message("DisabledByScenario".Translate() + ": " + Find.Scenario.name, MessageTypeDefOf.RejectInput, historical: false);
				}
			}
			listing.Gap(3f);
		}
		listing.Gap();
	}

	private void DrawExtraSettings()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		if (anomalyPlaystyleDef.displayThreatFractionSliders || anomalyPlaystyleDef.overrideThreatFraction || anomalyPlaystyleDef.displayThreatFractionSliders)
		{
			listing.Label("CanBeEditedInStorytellerSettings".Translate() + ":");
		}
		if (anomalyPlaystyleDef.displayThreatFractionSliders)
		{
			TaggedString taggedString = "Difficulty_AnomalyThreatsInactive_Info".Translate();
			listing.Label("Difficulty_AnomalyThreatsInactive_Label".Translate() + ": " + anomalyThreatsInactiveFraction.ToStringPercent() + " - " + GetFrequencyLabel(anomalyThreatsInactiveFraction), -1f, taggedString);
			anomalyThreatsInactiveFraction = listing.Slider(anomalyThreatsInactiveFraction, 0f, 1f);
			TaggedString taggedString2 = "Difficulty_AnomalyThreatsActive_Info".Translate(Mathf.Clamp01(anomalyThreatsActiveFraction).ToStringPercent(), Mathf.Clamp01(anomalyThreatsActiveFraction * 1.5f).ToStringPercent());
			listing.Label("Difficulty_AnomalyThreatsActive_Label".Translate() + ": " + anomalyThreatsActiveFraction.ToStringPercent() + " - " + GetFrequencyLabel(anomalyThreatsActiveFraction), -1f, taggedString2);
			anomalyThreatsActiveFraction = listing.Slider(anomalyThreatsActiveFraction, 0f, 1f);
		}
		else if (anomalyPlaystyleDef.overrideThreatFraction)
		{
			TaggedString taggedString3 = "Difficulty_AnomalyThreats_Info".Translate();
			listing.Label("Difficulty_AnomalyThreats_Label".Translate() + ": " + overrideAnomalyThreatsFraction.ToStringPercent() + " - " + GetFrequencyLabel(overrideAnomalyThreatsFraction), -1f, taggedString3);
			overrideAnomalyThreatsFraction = listing.Slider(overrideAnomalyThreatsFraction, 0f, 1f);
		}
		if (anomalyPlaystyleDef.displayStudyFactorSlider)
		{
			listing.Label("Difficulty_StudyEfficiency_Label".Translate() + ": " + studyEfficiencyFactor.ToStringPercent(), -1f, "Difficulty_StudyEfficiency_Info".Translate());
			studyEfficiencyFactor = listing.Slider(studyEfficiencyFactor, 0f, 5f);
		}
	}

	public static string GetFrequencyLabel(float freq)
	{
		for (int i = 0; i < FrequencyLabels.Length; i++)
		{
			if (freq <= FrequencyLabels[i].Item1)
			{
				return FrequencyLabels[i].Item2.Translate();
			}
		}
		return FrequencyLabels[FrequencyLabels.Length - 1].Item2.Translate();
	}
}
