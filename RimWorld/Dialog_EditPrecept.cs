using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_EditPrecept : Window
{
	private Precept precept;

	private string newPreceptName;

	private string newPreceptNameFemale;

	private float windowHeight = 170f;

	private float attachedRewardDescWidth;

	private float attachedRewardDescHeight;

	private int newTriggerDaysSinceStartOfYear = -1;

	private bool newCanStartAnytime;

	private Quadrum quadrum;

	private int day;

	private StyleCategoryPair selectedStyle;

	private float styleListLastHeight;

	private Vector2 styleListScrollPos;

	private RitualObligationTrigger_Date dateTrigger;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private static readonly float EditFieldHeight = 30f;

	private const float ApparelRequirementThingHeight = 30f;

	private const float ApparelRequirementBtnHeight = 30f;

	private const float WindowBaseHeight = 170f;

	private const float HeaderHeight = 35f;

	private const float ApparelRequirementNoteHeight = 30f;

	private const float BuildingStyleIconSize = 64f;

	private const int MaxPreceptNameLength = 32;

	private static readonly Regex ValidSymbolRegex = new Regex("^[\\p{L}0-9 '\\-]*$");

	private List<PreceptApparelRequirement> apparelRequirements;

	private RitualAttachableOutcomeEffectDef selectedReward;

	private List<RitualAttachableOutcomeEffectDef> attachableOutcomeEffects;

	private List<RitualAttachableOutcomeEffectDef> attachableUsableOutcomeEffects;

	private static List<StyleCategoryPair> stylesForBuildingTmp = new List<StyleCategoryPair>();

	public override Vector2 InitialSize => new Vector2(700f, windowHeight);

	public Dialog_EditPrecept(Precept precept)
	{
		this.precept = precept;
		absorbInputAroundWindow = true;
		newPreceptName = precept.Label;
		Precept_Ritual ritual = precept as Precept_Ritual;
		if (ritual != null)
		{
			dateTrigger = ritual.obligationTriggers.OfType<RitualObligationTrigger_Date>().FirstOrDefault();
			if (dateTrigger != null)
			{
				newTriggerDaysSinceStartOfYear = dateTrigger.triggerDaysSinceStartOfYear;
				RecalculateQuadrumAndDay();
			}
			newCanStartAnytime = ritual.isAnytime;
			attachableOutcomeEffects = DefDatabase<RitualAttachableOutcomeEffectDef>.AllDefs.ToList();
			attachableUsableOutcomeEffects = attachableOutcomeEffects.Where((RitualAttachableOutcomeEffectDef e) => e.CanAttachToRitual(ritual)).ToList();
			selectedReward = ritual.attachableOutcomeEffect;
		}
		if (precept.def.leaderRole)
		{
			newPreceptNameFemale = precept.ideo.leaderTitleFemale;
		}
		apparelRequirements = ((precept.ApparelRequirements != null) ? precept.ApparelRequirements.ToList() : null);
		if (precept is Precept_Building precept_Building)
		{
			selectedStyle = precept_Building.ideo.GetStyleAndCategoryFor(precept_Building.ThingDef);
		}
		UpdateWindowHeight();
	}

	private void UpdateWindowHeight()
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		windowHeight = 170f;
		if (precept.def.leaderRole || dateTrigger != null)
		{
			windowHeight += 10f + EditFieldHeight;
		}
		if (apparelRequirements != null)
		{
			windowHeight += 39f;
			windowHeight += 40f;
			windowHeight += 30f;
			if (!apparelRequirements.NullOrEmpty())
			{
				foreach (PreceptApparelRequirement apparelRequirement in apparelRequirements)
				{
					int num = apparelRequirement.requirement.AllRequiredApparel().EnumerableCount();
					windowHeight += (float)num * 30f;
				}
				windowHeight += (float)apparelRequirements.Count * 10f;
			}
		}
		if (precept is Precept_Ritual)
		{
			windowHeight += 10f + EditFieldHeight;
			windowHeight += 10f + EditFieldHeight;
			windowHeight += 10f + EditFieldHeight;
			if (selectedReward != null)
			{
				float x = Text.CalcSize(selectedReward.LabelCap).x;
				attachedRewardDescWidth = x + ButSize.x + 4f;
				attachedRewardDescHeight = Text.CalcHeight(selectedReward.effectDesc, attachedRewardDescWidth);
				windowHeight += attachedRewardDescHeight;
			}
		}
		if (precept is Precept_Building building && StylesForBuilding(building).Count > 1)
		{
			int val = NumStyleRows(building);
			windowHeight += (float)Math.Min(val, 2) * 74f + 14f;
		}
		if (precept is Precept_Relic precept_Relic && precept_Relic.ThingDef.MadeFromStuff)
		{
			windowHeight += 10f + EditFieldHeight;
		}
		SetInitialSizeAndPosition();
	}

	private int NumStyleRows(Precept_Building building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)(InitialSize.x - Margin * 2f - InitialSize.x / 3f) / 68;
		return StylesForBuilding(building).Count / num + 1;
	}

	public override void OnAcceptKeyPressed()
	{
		ApplyChanges();
		Event.current.Use();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_1341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_100a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1039: Unknown result type (might be due to invalid IL or missing references)
		//IL_103b: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Unknown result type (might be due to invalid IL or missing references)
		//IL_1084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1121: Unknown result type (might be due to invalid IL or missing references)
		//IL_1154: Unknown result type (might be due to invalid IL or missing references)
		//IL_1159: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Unknown result type (might be due to invalid IL or missing references)
		//IL_1176: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1203: Unknown result type (might be due to invalid IL or missing references)
		//IL_1208: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d15: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_150b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c82: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 3f;
		float num2 = ((Rect)(ref rect)).xMax - num;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, 35f), "EditName".Translate());
		Text.Font = GameFont.Small;
		float num3 = ((Rect)(ref rect)).y + 35f + 10f;
		if (this.precept.def.leaderRole)
		{
			Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "LeaderTitle".Translate() + " (" + Gender.Male.GetLabel() + ")");
			newPreceptName = Widgets.TextField(new Rect(num, num3, num2, EditFieldHeight), newPreceptName, 99999, ValidSymbolRegex);
			num3 += EditFieldHeight + 10f;
			if (newPreceptName.Length > 32)
			{
				Messages.Message("PreceptNameTooLong".Translate(32), null, MessageTypeDefOf.RejectInput, historical: false);
				newPreceptName = newPreceptName.Substring(0, 32);
			}
			Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "LeaderTitle".Translate() + " (" + Gender.Female.GetLabel() + ")");
			newPreceptNameFemale = Widgets.TextField(new Rect(num, num3, num2, EditFieldHeight), newPreceptNameFemale, 99999, ValidSymbolRegex);
			num3 += EditFieldHeight + 10f;
			if (newPreceptNameFemale.Length > 32)
			{
				Messages.Message("PreceptNameTooLong".Translate(32), null, MessageTypeDefOf.RejectInput, historical: false);
				newPreceptNameFemale = newPreceptNameFemale.Substring(0, 32);
			}
		}
		else
		{
			Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "Name".Translate());
			newPreceptName = Widgets.TextField(new Rect(num, num3, num2 - EditFieldHeight - 4f, EditFieldHeight), newPreceptName, 99999, ValidSymbolRegex);
			if (newPreceptName.Length > 32)
			{
				Messages.Message("PreceptNameTooLong".Translate(32), null, MessageTypeDefOf.RejectInput, historical: false);
				newPreceptName = newPreceptName.Substring(0, 32);
			}
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref rect)).xMax - 35f, num3, EditFieldHeight, EditFieldHeight);
			if (Widgets.ButtonImage(val, this.precept.nameLocked ? IdeoUIUtility.LockedTex : IdeoUIUtility.UnlockedTex))
			{
				this.precept.nameLocked = !this.precept.nameLocked;
				if (this.precept.nameLocked)
				{
					SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
				}
			}
			GUI.color = Color.white;
			if (Mouse.IsOver(val))
			{
				TaggedString taggedString = "LockPreceptNameButtonDesc".Translate() + "\n\n" + (this.precept.nameLocked ? "LockInOn" : "LockInOff").Translate("PreceptName".Translate(), "PreceptNameLower".Translate());
				TooltipHandler.TipRegion(val, taggedString);
			}
			num3 += EditFieldHeight + 10f;
			Precept_Ritual ritual = this.precept as Precept_Ritual;
			if (ritual != null)
			{
				if (ritual.canBeAnytime && !ritual.sourcePattern.alwaysStartAnytime)
				{
					Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "StartingCondition".Translate());
					bool flag = newCanStartAnytime;
					if (Widgets.RadioButton(num, num3, flag))
					{
						flag = true;
					}
					Widgets.Label(new Rect(num + 10f + 24f, num3, num2, EditFieldHeight), "StartingCondition_Anytime".Translate());
					num3 += EditFieldHeight + 10f;
					if (Widgets.RadioButton(num, num3, !flag))
					{
						flag = false;
					}
					Widgets.Label(new Rect(num + 10f + 24f, num3, num2, EditFieldHeight), "StartingCondition_Date".Translate());
					num3 += EditFieldHeight + 10f;
					if (flag != newCanStartAnytime)
					{
						newCanStartAnytime = flag;
						UpdateWindowHeight();
					}
					if (dateTrigger != null && !newCanStartAnytime)
					{
						Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "Date".Translate());
						if (Widgets.ButtonText(new Rect(num, num3, num2 / 4f, EditFieldHeight), Find.ActiveLanguageWorker.OrdinalNumber(day + 1)))
						{
							List<FloatMenuOption> list = new List<FloatMenuOption>();
							for (int i = 0; i < 15; i++)
							{
								int d = i;
								list.Add(new FloatMenuOption(Find.ActiveLanguageWorker.OrdinalNumber(d + 1), delegate
								{
									day = d;
									newTriggerDaysSinceStartOfYear = (int)quadrum * 15 + day;
								}));
							}
							Find.WindowStack.Add(new FloatMenu(list));
						}
						if (Widgets.ButtonText(new Rect(num + num2 / 4f + 10f, num3, num2 / 4f, EditFieldHeight), quadrum.Label()))
						{
							List<FloatMenuOption> list2 = new List<FloatMenuOption>();
							foreach (Quadrum q in QuadrumUtility.QuadrumsInChronologicalOrder)
							{
								list2.Add(new FloatMenuOption(q.Label(), delegate
								{
									quadrum = q;
									newTriggerDaysSinceStartOfYear = (int)quadrum * 15 + day;
								}));
							}
							Find.WindowStack.Add(new FloatMenu(list2));
						}
						num3 += EditFieldHeight + 10f;
					}
				}
				if (ritual.SupportsAttachableOutcomeEffect)
				{
					Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2 / 2f, EditFieldHeight), "RitualAttachedReward".Translate());
					if (selectedReward != null)
					{
						GUI.color = Color.gray;
						string text = selectedReward.AppliesToOutcomesString(ritual.outcomeEffect.def);
						Widgets.Label(new Rect(((Rect)(ref rect)).x, num3 + EditFieldHeight, num2 / 2f, EditFieldHeight * 2f), (selectedReward.AppliesToSeveralOutcomes(ritual) ? "RitualAttachedApplliesForOutcomes" : "RitualAttachedApplliesForOutcome").Translate(text));
						Widgets.Label(new Rect(num, num3 + EditFieldHeight, attachedRewardDescWidth, attachedRewardDescHeight), selectedReward.effectDesc.CapitalizeFirst());
						GUI.color = Color.white;
					}
					TaggedString taggedString2 = ((selectedReward != null) ? selectedReward.LabelCap : "None".Translate());
					Widgets.Label(new Rect(num, num3, num2, EditFieldHeight), taggedString2);
					float num4 = num + Text.CalcSize(taggedString2).x + 10f;
					if (selectedReward != null)
					{
						TooltipHandler.TipRegion(new Rect(((Rect)(ref rect)).x, num3, num4 - ((Rect)(ref rect)).x, EditFieldHeight), new TipSignal(() => selectedReward.TooltipForRitual(ritual), ritual.Id));
					}
					if (Widgets.ButtonText(new Rect(num4, num3 - 4f, num2 / 4f, EditFieldHeight), "RitualAttachedRewardChoose".Translate() + "...", drawBackground: true, doMouseoverSound: true, attachableOutcomeEffects.Any()))
					{
						List<FloatMenuOption> list3 = new List<FloatMenuOption>();
						if (selectedReward != null)
						{
							list3.Add(new FloatMenuOption("None".Translate(), delegate
							{
								selectedReward = null;
								UpdateWindowHeight();
							}));
						}
						foreach (RitualAttachableOutcomeEffectDef attachableOutcomeEffect in attachableOutcomeEffects)
						{
							RitualAttachableOutcomeEffectDef eff = attachableOutcomeEffect;
							if (eff == selectedReward)
							{
								continue;
							}
							AcceptanceReport acceptanceReport = eff.CanAttachToRitual(ritual);
							if ((bool)acceptanceReport)
							{
								list3.Add(new FloatMenuOption(eff.LabelCap, delegate
								{
									selectedReward = eff;
									UpdateWindowHeight();
								}, MenuOptionPriority.Default, DrawTooltip));
							}
							else
							{
								list3.Add(new FloatMenuOption(eff.LabelCap + " (" + acceptanceReport.Reason + ")", null));
							}
							void DrawTooltip(Rect r)
							{
								//IL_0000: Unknown result type (might be due to invalid IL or missing references)
								TooltipHandler.TipRegion(r, new TipSignal(() => eff.TooltipForRitual(ritual), ritual.Id));
							}
						}
						if (list3.Any())
						{
							Find.WindowStack.Add(new FloatMenu(list3));
						}
					}
				}
			}
		}
		if (apparelRequirements != null)
		{
			string text2 = "EditApparelRequirement".Translate();
			Text.Font = GameFont.Medium;
			Vector2 val2 = Text.CalcSize(text2);
			Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, ((Rect)(ref rect)).width, 35f), text2);
			Text.Font = GameFont.Small;
			num3 += 39f;
			text2 = "NoteRoleApparelRequirementEffects".Translate();
			Rect rect2 = new Rect(((Rect)(ref rect)).x, num3, ((Rect)(ref rect)).width, 30f);
			Color color = GUI.color;
			GUI.color = Color.gray;
			Widgets.Label(rect2, text2);
			GUI.color = color;
			num3 += 40f;
			int num5 = -1;
			Rect rect3 = default(Rect);
			Rect rect4 = default(Rect);
			Rect rect5 = default(Rect);
			for (int j = 0; j < apparelRequirements.Count; j++)
			{
				PreceptApparelRequirement preceptApparelRequirement = apparelRequirements[j];
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - 100f, num3, 100f, 30f);
				if (Widgets.ButtonText(rect3, "Remove".Translate()))
				{
					num5 = j;
				}
				if (preceptApparelRequirement.RequirementOverlapsOther(apparelRequirements, out var compatibilityReason))
				{
					compatibilityReason = compatibilityReason.CapitalizeFirst().Colorize(ColorLibrary.Yellow);
				}
				foreach (ThingDef item in preceptApparelRequirement.requirement.AllRequiredApparel())
				{
					((Rect)(ref rect4))._002Ector(((Rect)(ref rect)).x + 26f + 10f + 24f, num3 + 3f, 24f, 30f);
					((Rect)(ref rect5))._002Ector(((Rect)(ref rect4)).x + 32f, num3, compatibilityReason.NullOrEmpty() ? (((Rect)(ref rect)).width - ((Rect)(ref rect4)).width - ((Rect)(ref rect3)).width - 32f) : 100f, 30f);
					text2 = item.LabelCap;
					val2 = Text.CalcSize(text2);
					((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + (((Rect)(ref rect5)).height - val2.y) / 2f;
					Widgets.Label(rect5, text2.Truncate(((Rect)(ref rect5)).width));
					Widgets.DefIcon(rect4, item);
					if (!compatibilityReason.NullOrEmpty())
					{
						Widgets.Label(new Rect(((Rect)(ref rect5)).xMax + 4f, ((Rect)(ref rect5)).y, ((Rect)(ref rect)).width - ((Rect)(ref rect5)).width - ((Rect)(ref rect4)).width - ((Rect)(ref rect3)).width, 30f), compatibilityReason);
					}
					num3 += ((Rect)(ref rect5)).height;
				}
				num3 += 10f;
			}
			if (num5 != -1)
			{
				apparelRequirements.RemoveAt(num5);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				UpdateWindowHeight();
			}
			if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x + 26f, num3, ((Rect)(ref rect)).width - 26f, 30f), "Add".Translate().CapitalizeFirst() + "..."))
			{
				bool flag2 = false;
				foreach (MemeDef meme in this.precept.ideo.memes)
				{
					if (meme.preventApparelRequirements)
					{
						flag2 = true;
						Messages.Message("CannotNotAddRoleApparelDueToMeme".Translate(meme.LabelCap.Named("MEME")), MessageTypeDefOf.RejectInput, historical: false);
						break;
					}
				}
				if (!flag2)
				{
					List<FloatMenuOption> list4 = new List<FloatMenuOption>();
					foreach (PreceptApparelRequirement item2 in Precept_Role.AllPossibleRequirements(this.precept.ideo, this.precept.def, desperate: true))
					{
						PreceptApparelRequirement localReq = item2;
						List<ThingDef> list5 = item2.requirement.AllRequiredApparel().ToList();
						if (list5.Count > 0)
						{
							FloatMenuOption floatMenuOption = new FloatMenuOption(string.Join(", ", list5.Select((ThingDef ap) => ap.LabelCap)), delegate
							{
								apparelRequirements.Add(localReq);
								UpdateWindowHeight();
							}, list5[0].uiIcon, list5[0].uiIconColor);
							floatMenuOption.Disabled = !item2.CanAddRequirement(this.precept, apparelRequirements, out var cannotAddReason);
							if (floatMenuOption.Disabled)
							{
								floatMenuOption.Label = floatMenuOption.Label + " (" + cannotAddReason + ")";
							}
							list4.Add(floatMenuOption);
						}
					}
					Find.WindowStack.Add(new FloatMenu(list4));
				}
			}
		}
		Precept precept = this.precept;
		Precept_Building building = precept as Precept_Building;
		if (building != null && StylesForBuilding(building).Count > 1)
		{
			_ = building.ThingDef;
			Rect rect6 = default(Rect);
			((Rect)(ref rect6))._002Ector(rect);
			((Rect)(ref rect6)).y = ((Rect)(ref rect6)).y + num3;
			float num6 = 8f;
			Widgets.Label(rect6, "Appearance".Translate() + ":");
			List<StyleCategoryPair> elements = StylesForBuilding(building);
			int val3 = NumStyleRows(building);
			Rect val4 = default(Rect);
			((Rect)(ref val4)).x = ((Rect)(ref rect)).x + InitialSize.x / 3f;
			((Rect)(ref val4)).y = ((Rect)(ref rect)).y + num3;
			((Rect)(ref val4)).width = ((Rect)(ref rect)).width - InitialSize.x / 3f;
			((Rect)(ref val4)).height = (float)Math.Min(val3, 2) * 68f + num6;
			Rect outRect = val4;
			Widgets.BeginScrollView(outRect, ref styleListScrollPos, new Rect(0f, 0f, ((Rect)(ref outRect)).width - 16f, styleListLastHeight + num6));
			val4 = GenUI.DrawElementStack(new Rect(num6, num6, ((Rect)(ref outRect)).width - num6, 99999f), 64f, elements, delegate(Rect r, StyleCategoryPair obj)
			{
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				if (selectedStyle != null && selectedStyle.styleDef == obj.styleDef)
				{
					Widgets.DrawBoxSolid(r, new Color(1f, 0.8f, 0.2f, 0.2f));
				}
				if (Mouse.IsOver(r))
				{
					if (Widgets.ButtonInvisible(r))
					{
						selectedStyle = obj;
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
					TooltipHandler.TipRegion(r, "Origin".Translate() + " " + obj.category.LabelCap + ".");
					Widgets.DrawHighlight(r);
				}
				Widgets.DefIcon(r, building.ThingDef, GenStuff.DefaultStuffFor(building.ThingDef), 1f, obj.styleDef);
			}, (StyleCategoryPair obj) => 64f, 6f);
			styleListLastHeight = ((Rect)(ref val4)).height;
			Widgets.EndScrollView();
		}
		precept = this.precept;
		Precept_Relic relic = precept as Precept_Relic;
		if (relic != null && relic.ThingDef.MadeFromStuff)
		{
			Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "RelicStuff".Translate() + ":");
			Vector2 val5 = Text.CalcSize(relic.stuff.LabelCap);
			Text.Anchor = (TextAnchor)4;
			Rect rect7 = default(Rect);
			((Rect)(ref rect7))._002Ector(num, num3, val5.x, EditFieldHeight);
			Widgets.Label(rect7, relic.stuff.LabelCap);
			Text.Anchor = (TextAnchor)0;
			Rect rect8 = default(Rect);
			((Rect)(ref rect8))._002Ector(((Rect)(ref rect7)).xMax + 4f, num3, EditFieldHeight, EditFieldHeight);
			Widgets.DefIcon(rect8, relic.stuff);
			TaggedString taggedString3 = "ChooseStuffForRelic".Translate() + "...";
			Vector2 val6 = Text.CalcSize(taggedString3);
			if (Widgets.ButtonText(new Rect(((Rect)(ref rect8)).xMax + 4f, num3, val6.x + 20f, EditFieldHeight), taggedString3))
			{
				IEnumerable<ThingDef> enumerable = GenStuff.AllowedStuffsFor(relic.ThingDef);
				if (enumerable.Count() > 0)
				{
					List<FloatMenuOption> list6 = new List<FloatMenuOption>();
					foreach (ThingDef item3 in enumerable)
					{
						ThingDef localStuff = item3;
						list6.Add(new FloatMenuOption(item3.LabelCap, delegate
						{
							relic.stuff = localStuff;
						}, item3));
					}
					Find.WindowStack.Add(new FloatMenu(list6));
				}
			}
			num3 += EditFieldHeight + 10f;
		}
		if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "Back".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f - ButSize.x / 2f, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "Randomize".Translate()))
		{
			newPreceptName = (newPreceptNameFemale = this.precept.GenerateNewName());
			apparelRequirements = this.precept.GenerateNewApparelRequirements();
			if (dateTrigger != null)
			{
				newTriggerDaysSinceStartOfYear = dateTrigger.RandomDate();
				RecalculateQuadrumAndDay();
			}
			if (this.precept is Precept_Ritual precept_Ritual)
			{
				if (precept_Ritual.canBeAnytime && !precept_Ritual.sourcePattern.alwaysStartAnytime)
				{
					newCanStartAnytime = Rand.Bool;
				}
				if (attachableUsableOutcomeEffects.Any() && precept_Ritual.SupportsAttachableOutcomeEffect)
				{
					selectedReward = attachableUsableOutcomeEffects.RandomElement();
				}
			}
			if (this.precept is Precept_Building building2)
			{
				selectedStyle = StylesForBuilding(building2).RandomElement();
			}
			if (this.precept is Precept_Relic precept_Relic)
			{
				precept_Relic.SetRandomStuff();
			}
			SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			UpdateWindowHeight();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - ButSize.x, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "DoneButton".Translate()))
		{
			ApplyChanges();
		}
	}

	private void RecalculateQuadrumAndDay()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Map map = ((Current.ProgramState == ProgramState.Playing) ? Find.AnyPlayerHomeMap : null);
		float longitude = ((map != null) ? Find.WorldGrid.LongLatOf(map.Tile).x : 0f);
		quadrum = GenDate.Quadrum(newTriggerDaysSinceStartOfYear * 60000, longitude);
		day = GenDate.DayOfQuadrum(newTriggerDaysSinceStartOfYear * 60000, longitude);
	}

	private void ApplyChanges()
	{
		if (apparelRequirements != null && apparelRequirements.Any((PreceptApparelRequirement x) => x.RequirementOverlapsOther(apparelRequirements, out var _)))
		{
			Messages.Message("OverlappingRoleApparel".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		precept.SetName(newPreceptName);
		if (precept.def.leaderRole)
		{
			precept.ideo.leaderTitleMale = newPreceptName;
			precept.ideo.leaderTitleFemale = newPreceptNameFemale;
		}
		if (precept is Precept_Ritual precept_Ritual)
		{
			precept_Ritual.isAnytime = newCanStartAnytime;
			precept_Ritual.attachableOutcomeEffect = selectedReward;
		}
		if (dateTrigger != null && newTriggerDaysSinceStartOfYear != dateTrigger.triggerDaysSinceStartOfYear)
		{
			dateTrigger.triggerDaysSinceStartOfYear = newTriggerDaysSinceStartOfYear;
		}
		foreach (Precept item in precept.ideo.PreceptsListForReading)
		{
			item.ClearTipCache();
		}
		if (apparelRequirements != null)
		{
			precept.ApparelRequirements = apparelRequirements;
		}
		if (precept is Precept_Building precept_Building && selectedStyle != null)
		{
			precept.ideo.style.SetStyleForThingDef(precept_Building.ThingDef, selectedStyle);
		}
		Close();
	}

	private List<StyleCategoryPair> StylesForBuilding(Precept_Building building)
	{
		ThingDef thingDef = building.ThingDef;
		if (thingDef != null && thingDef.canEditAnyStyle)
		{
			return Precept_ThingDef.AllPossibleStylesForBuilding(building.ThingDef);
		}
		stylesForBuildingTmp.Clear();
		foreach (ThingStyleCategoryWithPriority thingStyleCategory in precept.ideo.thingStyleCategories)
		{
			foreach (ThingDefStyle thingDefStyle in thingStyleCategory.category.thingDefStyles)
			{
				if (thingDefStyle.ThingDef == building.ThingDef)
				{
					stylesForBuildingTmp.Add(new StyleCategoryPair
					{
						category = thingStyleCategory.category,
						styleDef = thingDefStyle.StyleDef
					});
				}
			}
		}
		return stylesForBuildingTmp;
	}
}
