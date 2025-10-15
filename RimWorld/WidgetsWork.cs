using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class WidgetsWork
{
	public const float WorkBoxSize = 25f;

	public static readonly Texture2D WorkBoxBGTex_Awful = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Awful");

	public static readonly Texture2D WorkBoxBGTex_Bad = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Bad");

	private const int AwfulBGMax = 4;

	public static readonly Texture2D WorkBoxBGTex_Mid = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Mid");

	private const int BadBGMax = 14;

	public static readonly Texture2D WorkBoxBGTex_Excellent = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_Excellent");

	public static readonly Texture2D WorkBoxCheckTex = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxCheck");

	public static readonly Texture2D PassionWorkboxMinorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinorGray");

	public static readonly Texture2D PassionWorkboxMajorIcon = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajorGray");

	public static readonly Texture2D WorkBoxOverlay_Warning = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxOverlay_Warning");

	public static readonly Texture2D WorkBoxOverlay_PreceptWarning = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxOverlay_PreceptWarning");

	private const int WarnIfSelectedMax = 2;

	private static Texture2D workBoxBGTex_AgeDisabled;

	private const float PassionOpacity = 0.4f;

	private static Texture2D WorkBoxBGTex_AgeDisabled
	{
		get
		{
			if ((Object)(object)workBoxBGTex_AgeDisabled == (Object)null)
			{
				workBoxBGTex_AgeDisabled = ContentFinder<Texture2D>.Get("UI/Widgets/WorkBoxBG_AgeDisabled");
			}
			return workBoxBGTex_AgeDisabled;
		}
	}

	public static Color ColorOfPriority(int prio)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(prio switch
		{
			1 => new Color(0f, 1f, 0f), 
			2 => new Color(1f, 0.9f, 0.5f), 
			3 => new Color(0.8f, 0.7f, 0.5f), 
			4 => new Color(0.74f, 0.74f, 0.74f), 
			_ => Color.grey, 
		});
	}

	public static void DrawWorkBoxFor(float x, float y, Pawn p, WorkTypeDef wType, bool incapableBecauseOfCapacities)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (p.WorkTypeIsDisabled(wType))
		{
			if (p.IsWorkTypeDisabledByAge(wType, out var minAgeRequired))
			{
				Rect val = default(Rect);
				((Rect)(ref val))._002Ector(x, y, 25f, 25f);
				if ((int)Event.current.type == 0 && Mouse.IsOver(val))
				{
					Messages.Message("MessageWorkTypeDisabledAge".Translate(p, p.ageTracker.AgeBiologicalYears, wType.labelShort, minAgeRequired), p, MessageTypeDefOf.RejectInput, historical: false);
					SoundDefOf.ClickReject.PlayOneShotOnCamera();
				}
				GUI.DrawTexture(val, (Texture)(object)WorkBoxBGTex_AgeDisabled);
			}
			return;
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(x, y, 25f, 25f);
		if (incapableBecauseOfCapacities)
		{
			GUI.color = new Color(1f, 0.3f, 0.3f);
		}
		DrawWorkBoxBackground(val2, p, wType);
		GUI.color = Color.white;
		if (Find.PlaySettings.useWorkPriorities)
		{
			int priority = p.workSettings.GetPriority(wType);
			if (priority > 0)
			{
				Text.Anchor = (TextAnchor)4;
				GUI.color = ColorOfPriority(priority);
				Widgets.Label(val2.ContractedBy(-3f), priority.ToStringCached());
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
			}
			if ((int)Event.current.type != 0 || !Mouse.IsOver(val2))
			{
				return;
			}
			bool num = p.workSettings.WorkIsActive(wType);
			if (Event.current.button == 0)
			{
				int num2 = p.workSettings.GetPriority(wType) - 1;
				if (num2 < 0)
				{
					num2 = 4;
				}
				p.workSettings.SetPriority(wType, num2);
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			if (Event.current.button == 1)
			{
				int num3 = p.workSettings.GetPriority(wType) + 1;
				if (num3 > 4)
				{
					num3 = 0;
				}
				p.workSettings.SetPriority(wType, num3);
				SoundDefOf.DragSlider.PlayOneShotOnCamera();
			}
			if (!num && p.workSettings.WorkIsActive(wType) && wType.relevantSkills.Any() && p.skills.AverageOfRelevantSkillsFor(wType) <= 2f)
			{
				SoundDefOf.Crunch.PlayOneShotOnCamera();
			}
			if (!num && p.workSettings.WorkIsActive(wType) && p.Ideo != null && p.Ideo.IsWorkTypeConsideredDangerous(wType))
			{
				Messages.Message("MessageIdeoOpposedWorkTypeSelected".Translate(p, wType.gerundLabel), p, MessageTypeDefOf.CautionInput, historical: false);
				SoundDefOf.DislikedWorkTypeActivated.PlayOneShotOnCamera();
			}
			Event.current.Use();
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ManualWorkPriorities, KnowledgeAmount.SmallInteraction);
			return;
		}
		if (p.workSettings.GetPriority(wType) > 0)
		{
			GUI.DrawTexture(val2, (Texture)(object)WorkBoxCheckTex);
		}
		if (!Widgets.ButtonInvisible(val2))
		{
			return;
		}
		if (p.workSettings.GetPriority(wType) > 0)
		{
			p.workSettings.SetPriority(wType, 0);
			SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
		}
		else
		{
			p.workSettings.SetPriority(wType, 3);
			SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			if (wType.relevantSkills.Any() && p.skills.AverageOfRelevantSkillsFor(wType) <= 2f)
			{
				SoundDefOf.Crunch.PlayOneShotOnCamera();
			}
			if (p.Ideo != null && p.Ideo.IsWorkTypeConsideredDangerous(wType))
			{
				Messages.Message("MessageIdeoOpposedWorkTypeSelected".Translate(p, wType.gerundLabel), p, MessageTypeDefOf.CautionInput, historical: false);
				SoundDefOf.DislikedWorkTypeActivated.PlayOneShotOnCamera();
			}
		}
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorkTab, KnowledgeAmount.SpecificInteraction);
	}

	public static string TipForPawnWorker(Pawn p, WorkTypeDef wDef, bool incapableBecauseOfCapacities)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		string text = wDef.gerundLabel.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor);
		int priority = p.workSettings.GetPriority(wDef);
		text = text + ": " + ((string)("Priority" + priority).Translate()).Colorize(ColorOfPriority(priority));
		stringBuilder.AppendLine(text);
		if (p.WorkTypeIsDisabled(wDef))
		{
			string text2 = "CannotDoThisWork".Translate(p.LabelShort, p);
			List<string> reasonsForDisabledWorkType = p.GetReasonsForDisabledWorkType(wDef);
			if (!reasonsForDisabledWorkType.NullOrEmpty())
			{
				string text3 = "\n\n" + string.Join(". ", reasonsForDisabledWorkType);
				if (reasonsForDisabledWorkType.Count == 1)
				{
					text3 += ".";
				}
				text2 += text3.Colorize(ColorLibrary.RedReadable);
			}
			stringBuilder.Append(text2);
		}
		else
		{
			float num = p.skills.AverageOfRelevantSkillsFor(wDef);
			if (wDef.relevantSkills.Any())
			{
				string text4 = "";
				foreach (SkillDef relevantSkill in wDef.relevantSkills)
				{
					text4 = text4 + relevantSkill.skillLabel.CapitalizeFirst() + ", ";
				}
				text4 = text4.Substring(0, text4.Length - 2);
				stringBuilder.AppendLine("RelevantSkills".Translate(text4, num.ToString("0.#"), 20));
			}
			if (p.Ideo != null && p.Ideo.IsWorkTypeConsideredDangerous(wDef))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(((string)"SelectedWorkTypeOpposedByIdeo".Translate(p)).Colorize(ColorOfPriority(2)));
			}
			if (wDef.relevantSkills.Any() && num <= 2f && p.workSettings.WorkIsActive(wDef))
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(((string)"SelectedWorkTypeWithVeryBadSkill".Translate()).Colorize(ColorOfPriority(2)));
			}
			stringBuilder.AppendLine();
			stringBuilder.Append(wDef.description);
			if (incapableBecauseOfCapacities)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.Append("IncapableOfWorkTypeBecauseOfCapacities".Translate());
			}
		}
		return stringBuilder.ToString();
	}

	private static void DrawWorkBoxBackground(Rect rect, Pawn p, WorkTypeDef workDef)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		float num = p.skills.AverageOfRelevantSkillsFor(workDef);
		Texture2D val;
		Texture2D val2;
		float num2;
		if (num < 4f)
		{
			val = WorkBoxBGTex_Awful;
			val2 = WorkBoxBGTex_Bad;
			num2 = num / 4f;
		}
		else if (num <= 14f)
		{
			val = WorkBoxBGTex_Bad;
			val2 = WorkBoxBGTex_Mid;
			num2 = (num - 4f) / 10f;
		}
		else
		{
			val = WorkBoxBGTex_Mid;
			val2 = WorkBoxBGTex_Excellent;
			num2 = (num - 14f) / 6f;
		}
		GUI.DrawTexture(rect, (Texture)(object)val);
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, num2);
		GUI.DrawTexture(rect, (Texture)(object)val2);
		if (p.Ideo != null && p.Ideo.IsWorkTypeConsideredDangerous(workDef))
		{
			GUI.color = Color.white;
			GUI.DrawTexture(rect, (Texture)(object)WorkBoxOverlay_PreceptWarning);
		}
		if (workDef.relevantSkills.Any() && num <= 2f && p.workSettings.WorkIsActive(workDef))
		{
			GUI.color = Color.white;
			GUI.DrawTexture(rect.ContractedBy(-2f), (Texture)(object)WorkBoxOverlay_Warning);
		}
		Passion passion = p.skills.MaxPassionOfRelevantSkillsFor(workDef);
		if ((int)passion > 0)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.4f);
			Rect val3 = rect;
			((Rect)(ref val3)).xMin = ((Rect)(ref rect)).center.x;
			((Rect)(ref val3)).yMin = ((Rect)(ref rect)).center.y;
			switch (passion)
			{
			case Passion.Minor:
				GUI.DrawTexture(val3, (Texture)(object)PassionWorkboxMinorIcon);
				break;
			case Passion.Major:
				GUI.DrawTexture(val3, (Texture)(object)PassionWorkboxMajorIcon);
				break;
			}
		}
		GUI.color = Color.white;
	}
}
