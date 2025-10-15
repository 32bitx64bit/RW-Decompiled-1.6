using System;
using System.Collections.Generic;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class QuestPartUtility
{
	public const int RewardStackElementIconSize = 22;

	public const int RewardStackElementMarginHorizontal = 6;

	public const int RewardStackElementMarginVertical = 1;

	public const int RewardStackElementIconGap = 2;

	private static List<Pair<Thing, int>> tmpThings = new List<Pair<Thing, int>>();

	private static List<Reward_Items.RememberedItem> tmpThingDefs = new List<Reward_Items.RememberedItem>();

	public static T MakeAndAddEndCondition<T>(Quest quest, string inSignalActivate, QuestEndOutcome outcome, Letter letter = null) where T : QuestPartActivable, new()
	{
		T val = new T
		{
			inSignalEnable = inSignalActivate
		};
		quest.AddPart(val);
		if (letter != null)
		{
			QuestPart_Letter questPart_Letter = new QuestPart_Letter();
			questPart_Letter.letter = letter;
			questPart_Letter.inSignal = val.OutSignalCompleted;
			quest.AddPart(questPart_Letter);
		}
		QuestPart_QuestEnd questPart_QuestEnd = new QuestPart_QuestEnd();
		questPart_QuestEnd.inSignal = val.OutSignalCompleted;
		questPart_QuestEnd.outcome = outcome;
		quest.AddPart(questPart_QuestEnd);
		return val;
	}

	public static QuestPart_QuestEnd MakeAndAddEndNodeWithLetter(Quest quest, string inSignalActivate, QuestEndOutcome outcome, Letter letter)
	{
		QuestPart_Letter questPart_Letter = new QuestPart_Letter();
		questPart_Letter.letter = letter;
		questPart_Letter.inSignal = inSignalActivate;
		quest.AddPart(questPart_Letter);
		QuestPart_QuestEnd questPart_QuestEnd = new QuestPart_QuestEnd();
		questPart_QuestEnd.inSignal = inSignalActivate;
		questPart_QuestEnd.outcome = outcome;
		quest.AddPart(questPart_QuestEnd);
		return questPart_QuestEnd;
	}

	public static QuestPart_Delay MakeAndAddQuestTimeoutDelay(Quest quest, int delayTicks, WorldObject worldObject)
	{
		QuestPart_WorldObjectTimeout questPart_WorldObjectTimeout = new QuestPart_WorldObjectTimeout();
		questPart_WorldObjectTimeout.delayTicks = delayTicks;
		questPart_WorldObjectTimeout.expiryInfoPart = "QuestExpiresIn".Translate();
		questPart_WorldObjectTimeout.expiryInfoPartTip = "QuestExpiresOn".Translate();
		questPart_WorldObjectTimeout.isBad = true;
		questPart_WorldObjectTimeout.outcomeCompletedSignalArg = QuestEndOutcome.Fail;
		questPart_WorldObjectTimeout.inSignalEnable = quest.InitiateSignal;
		quest.AddPart(questPart_WorldObjectTimeout);
		string text = "Quest" + quest.id + ".DelayingWorldObject";
		QuestUtility.AddQuestTag(ref worldObject.questTags, text);
		questPart_WorldObjectTimeout.inSignalDisable = text + ".MapGenerated";
		QuestPart_QuestEnd questPart_QuestEnd = new QuestPart_QuestEnd();
		questPart_QuestEnd.inSignal = questPart_WorldObjectTimeout.OutSignalCompleted;
		quest.AddPart(questPart_QuestEnd);
		return questPart_WorldObjectTimeout;
	}

	public static IEnumerable<GenUI.AnonymousStackElement> GetRewardStackElementsForThings(IEnumerable<Thing> things, bool detailsHidden = false)
	{
		tmpThings.Clear();
		foreach (Thing thing2 in things)
		{
			bool flag = false;
			for (int j = 0; j < tmpThings.Count; j++)
			{
				if (tmpThings[j].First.CanStackWith(thing2))
				{
					tmpThings[j] = new Pair<Thing, int>(tmpThings[j].First, tmpThings[j].Second + thing2.stackCount);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				tmpThings.Add(new Pair<Thing, int>(thing2, thing2.stackCount));
			}
		}
		for (int i = 0; i < tmpThings.Count; i++)
		{
			Thing thing = tmpThings[i].First.GetInnerIfMinified();
			int second = tmpThings[i].Second;
			string label;
			if (thing is Pawn pawn)
			{
				label = "PawnQuestReward".Translate(pawn);
			}
			else
			{
				label = thing.LabelCapNoCount + ((second > 1) ? (" x" + second) : "");
			}
			yield return new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect rect)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0041: Unknown result type (might be due to invalid IL or missing references)
					//IL_0220: Unknown result type (might be due to invalid IL or missing references)
					//IL_0246: Unknown result type (might be due to invalid IL or missing references)
					//IL_0247: Unknown result type (might be due to invalid IL or missing references)
					//IL_025b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0267: Unknown result type (might be due to invalid IL or missing references)
					//IL_004c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0077: Unknown result type (might be due to invalid IL or missing references)
					//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
					//IL_0146: Unknown result type (might be due to invalid IL or missing references)
					//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
					Widgets.DrawHighlight(rect);
					Rect val = default(Rect);
					((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + 6f, ((Rect)(ref rect)).y + 1f, ((Rect)(ref rect)).width - 12f, ((Rect)(ref rect)).height - 2f);
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						TaggedString ts = (detailsHidden ? "NoMoreInfoAvailable".Translate() : "ClickForMoreInfo".Translate());
						ts = ts.Colorize(ColoredText.SubtleGrayColor);
						if (thing is Pawn pawn2 && pawn2.RaceProps.Humanlike)
						{
							string text = pawn2.LabelCap;
							if (ModsConfig.BiotechActive && pawn2.genes != null)
							{
								text += (" (" + pawn2.genes.XenotypeLabel + ")").Colorize(ColoredText.SubtleGrayColor);
								text = text + "\n\n" + pawn2.genes.XenotypeDescShort;
							}
							SkillRecord skillRecord = pawn2.skills.skills.MaxBy((SkillRecord s) => (!s.TotallyDisabled) ? s.Level : (-1));
							string arg = new StringBuilder().Append("BestSkillLabel".Translate().Colorize(ColoredText.TipSectionTitleColor)).Append(": ").Append(skillRecord.def.LabelCap.ToString())
								.Append(" (")
								.Append("BestSkillInfoLevel".Translate(skillRecord.Level).ToString())
								.Append(')')
								.ToString();
							TooltipHandler.TipRegion(rect, $"{text}\n\n{arg}\n\n{ts}");
						}
						else
						{
							TooltipHandler.TipRegion(rect, thing.DescriptionDetailed + "\n\n" + ts);
						}
					}
					Widgets.ThingIcon(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, 22f, 22f), thing);
					Rect rect2 = val;
					((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 24f;
					Widgets.Label(rect2, label);
					if (Widgets.ButtonInvisible(rect))
					{
						if (detailsHidden)
						{
							Messages.Message("NoMoreInfoAvailable".Translate(), MessageTypeDefOf.RejectInput, historical: false);
						}
						else
						{
							Find.WindowStack.Add(new Dialog_InfoCard(thing));
						}
					}
				},
				width = Text.CalcSize(label).x + 12f + 22f + 2f
			};
		}
	}

	public static IEnumerable<GenUI.AnonymousStackElement> GetRewardStackElementsForThings(IEnumerable<Reward_Items.RememberedItem> thingDefs)
	{
		tmpThingDefs.Clear();
		foreach (Reward_Items.RememberedItem thingDef in thingDefs)
		{
			bool flag = false;
			for (int j = 0; j < tmpThingDefs.Count; j++)
			{
				if (tmpThingDefs[j].thing == thingDef.thing && tmpThingDefs[j].label == thingDef.label)
				{
					tmpThingDefs[j] = new Reward_Items.RememberedItem(tmpThingDefs[j].thing, tmpThingDefs[j].stackCount + thingDef.stackCount, tmpThingDefs[j].label);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				tmpThingDefs.Add(thingDef);
			}
		}
		for (int i = 0; i < tmpThingDefs.Count; i++)
		{
			ThingStuffPairWithQuality thing = tmpThingDefs[i].thing;
			int stackCount = tmpThingDefs[i].stackCount;
			string label = tmpThingDefs[i].label.CapitalizeFirst() + ((stackCount > 1) ? (" x" + stackCount) : "");
			yield return new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect rect)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0041: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_006e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					Widgets.DrawHighlight(rect);
					Rect val = default(Rect);
					((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + 6f, ((Rect)(ref rect)).y + 1f, ((Rect)(ref rect)).width - 12f, ((Rect)(ref rect)).height - 2f);
					if (Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(rect);
						TaggedString taggedString = thing.thing.DescriptionDetailed + "\n\n" + "ClickForMoreInfo".Translate().Colorize(ColoredText.SubtleGrayColor);
						TooltipHandler.TipRegion(rect, taggedString);
					}
					Widgets.ThingIcon(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, 22f, 22f), thing.thing, thing.stuff);
					Rect rect2 = val;
					((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 24f;
					Widgets.Label(rect2, label);
					if (Widgets.ButtonInvisible(rect))
					{
						Find.WindowStack.Add(new Dialog_InfoCard(thing.thing, thing.stuff));
					}
				},
				width = Text.CalcSize(label).x + 12f + 22f + 2f
			};
		}
	}

	public static GenUI.AnonymousStackElement GetStandardRewardStackElement(string label, Texture2D icon, Func<string> tipGetter, Action onClick = null)
	{
		return GetStandardRewardStackElement(label, (Action<Rect>)delegate(Rect r)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUI.DrawTexture(r, (Texture)(object)icon);
		}, tipGetter, onClick);
	}

	public static GenUI.AnonymousStackElement GetStandardRewardStackElement(string label, Action<Rect> iconDrawer, Func<string> tipGetter, Action onClick = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		return new GenUI.AnonymousStackElement
		{
			drawer = delegate(Rect rect)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				Widgets.DrawHighlight(rect);
				Rect val = default(Rect);
				((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + 6f, ((Rect)(ref rect)).y + 1f, ((Rect)(ref rect)).width - 12f, ((Rect)(ref rect)).height - 2f);
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
					if (tipGetter != null)
					{
						TooltipHandler.TipRegion(rect, new TipSignal(tipGetter, 0x56AAA7E ^ (int)((Rect)(ref rect)).x ^ (int)((Rect)(ref rect)).y));
					}
				}
				Rect obj = default(Rect);
				((Rect)(ref obj))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, 22f, 22f);
				iconDrawer(obj);
				Rect rect2 = val;
				((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 24f;
				Widgets.Label(rect2, label);
				if (onClick != null && Widgets.ButtonInvisible(rect))
				{
					onClick();
				}
			},
			width = Text.CalcSize(label).x + 12f + 22f + 2f
		};
	}
}
