using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MainTabWindow_Quests : MainTabWindow
{
	private enum QuestsTab : byte
	{
		Available,
		Active,
		Historical
	}

	private Quest selected;

	private QuestsTab curTab;

	private List<TabRecord> tabs = new List<TabRecord>();

	private Vector2 scrollPosition_available;

	private Vector2 scrollPosition_active;

	private Vector2 scrollPosition_historical;

	private Vector2 selectedQuestScrollPosition;

	private float selectedQuestLastHeight;

	private bool showDebugInfo;

	private List<QuestPart> tmpQuestParts = new List<QuestPart>();

	private string debugSendSignalTextField;

	private bool showAll;

	private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private const float LeftRectWidthFraction = 0.36f;

	private const float RowHeight = 32f;

	private const float CheckBoxHeight = 24f;

	private const float ShowDebugInfoToggleWidth = 110f;

	private const float DismisIconWidth = 32f;

	private const float TimeInfoWidth = 35f;

	private const float CharityIconWidth = 32f;

	private static readonly Color TimeLimitColor = new Color(1f, 1f, 1f, 0.7f);

	private static readonly Color AcceptanceRequirementsColor = new Color(1f, 0.25f, 0.25f);

	private static readonly Color AcceptanceRequirementsBoxColor = new Color(0.62f, 0.18f, 0.18f);

	private static readonly Color acceptanceRequirementsBoxBgColor = new Color(0.13f, 0.13f, 0.13f);

	private static readonly Color IdeoCharityTextColor = Color32.op_Implicit(new Color32(byte.MaxValue, (byte)237, (byte)38, byte.MaxValue));

	private static readonly Color IdeoCharityBoxBorderColor = Color32.op_Implicit(new Color32((byte)205, (byte)207, (byte)18, byte.MaxValue));

	private static readonly Color IdeoCharityBoxBackgroundColor = new Color(0.13f, 0.13f, 0.13f);

	private static readonly Color QuestCompletedColor = GenColor.FromHex("1e591a");

	private static readonly Color QuestFailedColor = GenColor.FromHex("5e2f2f");

	private static readonly Color QuestExpiredColor = GenColor.FromHex("916e2d");

	private const int RowIconSize = 15;

	private const float RatingWidth = 60f;

	private const float RewardsConfigButtonHeight = 40f;

	private static Texture2D RatingIcon = null;

	private static readonly Texture2D DismissIcon = ContentFinder<Texture2D>.Get("UI/Buttons/Dismiss");

	private static readonly Texture2D ResumeQuestIcon = ContentFinder<Texture2D>.Get("UI/Buttons/UnDismiss");

	private static readonly Texture2D QuestDismissedIcon = ContentFinder<Texture2D>.Get("UI/Icons/DismissedQuestIcon");

	private static readonly Texture2D CharityQuestIcon = ContentFinder<Texture2D>.Get("UI/Icons/CharityQuestIcon");

	private const float IndentWidth = 10f;

	private const float SearchBoxHeight = 24f;

	private static List<Quest> tmpQuestsToShow = new List<Quest>();

	private static HashSet<Quest> tmpQuestsVisited = new HashSet<Quest>();

	private static List<Thing> tmpColonistsForIdeo = new List<Thing>();

	private static readonly HashSet<string> uniqueEntries = new HashSet<string>();

	private static List<GenUI.AnonymousStackElement> tmpStackElements = new List<GenUI.AnonymousStackElement>();

	private static List<QuestPart> tmpRemainingQuestParts = new List<QuestPart>();

	private static List<GlobalTargetInfo> tmpLookTargets = new List<GlobalTargetInfo>();

	private static List<GlobalTargetInfo> tmpSelectTargets = new List<GlobalTargetInfo>();

	public override Vector2 RequestedTabSize => new Vector2(1010f, 640f);

	public override void PreOpen()
	{
		base.PreOpen();
		quickSearchWidget.Reset();
		if ((Object)(object)RatingIcon == (Object)null)
		{
			RatingIcon = ContentFinder<Texture2D>.Get("UI/Icons/ChallengeRatingIcon");
		}
		tabs.Clear();
		tabs.Add(new TabRecord("AvailableQuests".Translate(), delegate
		{
			curTab = QuestsTab.Available;
			selected = null;
		}, () => curTab == QuestsTab.Available));
		tabs.Add(new TabRecord("ActiveQuests".Translate(), delegate
		{
			curTab = QuestsTab.Active;
			selected = null;
		}, () => curTab == QuestsTab.Active));
		tabs.Add(new TabRecord("HistoricalQuests".Translate(), delegate
		{
			curTab = QuestsTab.Historical;
			selected = null;
		}, () => curTab == QuestsTab.Historical));
		Select(selected);
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin + 4f;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).width * 0.36f;
		float yMax = ((Rect)(ref rect2)).yMax;
		Rect val = DoRewardsPrefsButton(rect2);
		((Rect)(ref rect2)).yMax = yMax - (((Rect)(ref val)).height + 4f);
		DoQuestsList(rect2);
		Rect rect3 = rect;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 4f;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax + 17f;
		DoSelectedQuestInfo(rect3);
	}

	public Rect DoRewardsPrefsButton(Rect rect)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMax - 40f;
		Text.Font = GameFont.Small;
		if (Widgets.ButtonText(rect, "ChooseRewards".Translate()))
		{
			Find.WindowStack.Add(new Dialog_RewardPrefsConfig());
		}
		return rect;
	}

	public void Select(Quest quest)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (quest != selected)
		{
			selected = quest;
			selectedQuestScrollPosition = default(Vector2);
			selectedQuestLastHeight = 300f;
		}
		if (quest != null)
		{
			if (quest.dismissed)
			{
				curTab = QuestsTab.Historical;
			}
			else if (quest.State == QuestState.NotYetAccepted)
			{
				curTab = QuestsTab.Available;
			}
			else if (quest.State == QuestState.Ongoing)
			{
				curTab = QuestsTab.Active;
			}
			else
			{
				curTab = QuestsTab.Historical;
			}
		}
	}

	private void DoQuestsList(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 32f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, ((Rect)(ref val)).yMax - 24f, ((Rect)(ref val)).width, 24f);
		quickSearchWidget.OnGUI(rect2);
		((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - 28f;
		Widgets.DrawMenuSection(val);
		TabDrawer.DrawTabs(val, tabs);
		if (DebugSettings.godMode)
		{
			showAll = true;
		}
		else if (Prefs.DevMode)
		{
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref rect)).width - 135f, ((Rect)(ref val)).yMax - 24f, 120f, 24f), "DEV: Show all", ref showAll);
		}
		else
		{
			showAll = false;
		}
		SortQuestsByTab();
		Rect scrollOutRect;
		Rect scrollViewRect;
		Vector2 scrollPosition;
		float curY;
		if (tmpQuestsToShow.Count != 0)
		{
			scrollOutRect = val;
			scrollOutRect = scrollOutRect.ContractedBy(10f);
			ref Rect reference = ref scrollOutRect;
			((Rect)(ref reference)).xMax = ((Rect)(ref reference)).xMax + 6f;
			scrollViewRect = new Rect(0f, 0f, ((Rect)(ref scrollOutRect)).width - 16f, (float)tmpQuestsToShow.Count * 32f);
			scrollPosition = default(Vector2);
			switch (curTab)
			{
			case QuestsTab.Available:
				Widgets.BeginScrollView(scrollOutRect, ref scrollPosition_available, scrollViewRect);
				scrollPosition = scrollPosition_available;
				break;
			case QuestsTab.Active:
				Widgets.BeginScrollView(scrollOutRect, ref scrollPosition_active, scrollViewRect);
				scrollPosition = scrollPosition_active;
				break;
			case QuestsTab.Historical:
				Widgets.BeginScrollView(scrollOutRect, ref scrollPosition_historical, scrollViewRect);
				scrollPosition = scrollPosition_historical;
				break;
			}
			curY = 0f;
			foreach (Quest item in tmpQuestsToShow)
			{
				DrawQuest(item);
			}
			tmpQuestsVisited.Clear();
			tmpQuestsToShow.Clear();
			Widgets.EndScrollView();
		}
		else
		{
			Widgets.NoneLabel(((Rect)(ref val)).y + 17f, ((Rect)(ref val)).width);
		}
		void DrawQuest(Quest quest, int indent = 0)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			if (tmpQuestsVisited.Contains(quest) || (quest.parent != null && tmpQuestsToShow.Contains(quest.parent) && !tmpQuestsVisited.Contains(quest.parent)))
			{
				return;
			}
			float num = scrollPosition.y - 32f;
			float num2 = scrollPosition.y + ((Rect)(ref scrollOutRect)).height;
			if (curY > num && curY < num2)
			{
				float num3 = (float)indent * 10f;
				DoRow(new Rect(num3, curY, ((Rect)(ref scrollViewRect)).width - 4f - num3, 32f), quest);
			}
			curY += 32f;
			tmpQuestsVisited.Add(quest);
			indent++;
			foreach (Quest subquest in quest.GetSubquests())
			{
				if (tmpQuestsToShow.Contains(subquest))
				{
					DrawQuest(subquest, indent);
				}
			}
		}
	}

	private void SortQuestsByTab()
	{
		List<Quest> questsInDisplayOrder = Find.QuestManager.questsInDisplayOrder;
		tmpQuestsToShow.Clear();
		for (int i = 0; i < questsInDisplayOrder.Count; i++)
		{
			if (ShouldListNow(questsInDisplayOrder[i]) && quickSearchWidget.filter.Matches(questsInDisplayOrder[i].name))
			{
				tmpQuestsToShow.Add(questsInDisplayOrder[i]);
			}
		}
		switch (curTab)
		{
		case QuestsTab.Available:
			tmpQuestsToShow.SortBy((Quest q) => q.TicksUntilExpiry);
			break;
		case QuestsTab.Active:
			tmpQuestsToShow.SortBy((Quest q) => q.TicksSinceAccepted);
			break;
		case QuestsTab.Historical:
			tmpQuestsToShow.SortBy((Quest q) => q.TicksSinceCleanup);
			break;
		}
	}

	private void DoRow(Rect rect, Quest quest)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 95f;
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 4f;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMax - 35f;
		Rect rect3 = rect;
		((Rect)(ref rect3)).xMax = ((Rect)(ref rect2)).xMin;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMax - 60f;
		if (quest.Historical)
		{
			Rect position = rect.ContractedBy(2f);
			switch (quest.State)
			{
			case QuestState.EndedSuccess:
				Widgets.DrawRectFast(position, QuestCompletedColor);
				break;
			case QuestState.EndedFailed:
				Widgets.DrawRectFast(position, QuestFailedColor);
				break;
			default:
				Widgets.DrawRectFast(position, QuestExpiredColor);
				break;
			}
		}
		if (selected == quest)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Text.Anchor = (TextAnchor)3;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y, ((Rect)(ref val)).width - 4f, ((Rect)(ref val)).height);
		Widgets.Label(rect4, quest.name.Truncate(((Rect)(ref rect4)).width));
		string timeTip;
		Color color;
		string shortTimeInfo = GetShortTimeInfo(quest, out timeTip, out color);
		if (!shortTimeInfo.NullOrEmpty())
		{
			GUI.color = color;
			Text.Anchor = (TextAnchor)5;
			Widgets.Label(rect2, shortTimeInfo);
			GUI.color = Color.white;
			if (Mouse.IsOver(rect2))
			{
				TooltipHandler.TipRegion(rect2, () => quest.name + (timeTip.NullOrEmpty() ? "" : ("\n" + timeTip)), quest.id ^ 0x343115E2);
				Widgets.DrawHighlight(rect2);
			}
		}
		if (quest.dismissed && !quest.Historical)
		{
			((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x - 25f;
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref rect3)).xMax + 5f, ((Rect)(ref rect3)).y + ((Rect)(ref rect3)).height / 2f - 7f, 15f, 15f);
			GUI.DrawTexture(val2, (Texture)(object)QuestDismissedIcon);
			((Rect)(ref val2)).height = ((Rect)(ref rect4)).height;
			((Rect)(ref val2)).y = ((Rect)(ref rect4)).y;
			if (Mouse.IsOver(val2))
			{
				TooltipHandler.TipRegion(val2, "QuestDismissed".Translate());
				Widgets.DrawHighlight(val2);
			}
		}
		if (ModsConfig.IdeologyActive && quest.charity && !quest.Historical && !quest.dismissed)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(((Rect)(ref rect3)).x - 15f, ((Rect)(ref rect3)).y + ((Rect)(ref rect3)).height / 2f - 7f, 15f, 15f);
			GUI.DrawTexture(val3, (Texture)(object)CharityQuestIcon);
			((Rect)(ref val3)).height = ((Rect)(ref rect4)).height;
			((Rect)(ref val3)).y = ((Rect)(ref rect4)).y;
			if (Mouse.IsOver(val3))
			{
				TooltipHandler.TipRegion(val3, "CharityQuestTip".Translate());
				Widgets.DrawHighlight(val3);
			}
		}
		int num = Mathf.Max(quest.challengeRating, 1);
		for (int i = 0; i < num; i++)
		{
			GUI.DrawTexture(new Rect(((Rect)(ref rect3)).xMax - (float)(15 * (i + 1)), ((Rect)(ref rect3)).y + ((Rect)(ref rect3)).height / 2f - 7f, 15f, 15f), (Texture)(object)RatingIcon);
		}
		if (Mouse.IsOver(rect3))
		{
			TooltipHandler.TipRegion(rect3, "QuestChallengeRatingTip".Translate());
			Widgets.DrawHighlight(rect3);
		}
		GenUI.ResetLabelAlign();
		if (Widgets.ButtonInvisible(rect))
		{
			Select(quest);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
	}

	private string GetShortTimeInfo(Quest quest, out string tip, out Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		color = Color.gray;
		if (quest.State == QuestState.NotYetAccepted)
		{
			if (quest.TicksUntilExpiry >= 0)
			{
				color = ColorLibrary.RedReadable;
				tip = "QuestExpiresIn".Translate(quest.TicksUntilExpiry.ToStringTicksToPeriod());
				return quest.TicksUntilExpiry.ToStringTicksToPeriod(allowSeconds: true, shortForm: true);
			}
		}
		else
		{
			if (quest.Historical)
			{
				tip = "QuestFinishedAgo".Translate(quest.TicksSinceCleanup.ToStringTicksToPeriod());
				return quest.TicksSinceCleanup.ToStringTicksToPeriod(allowSeconds: false, shortForm: true);
			}
			if (quest.EverAccepted)
			{
				foreach (QuestPart item in quest.PartsListForReading)
				{
					if (item is QuestPart_Delay { State: QuestPartState.Enabled, isBad: not false } questPart_Delay && !questPart_Delay.expiryInfoPart.NullOrEmpty())
					{
						color = ColorLibrary.RedReadable;
						tip = "QuestExpiresIn".Translate(questPart_Delay.TicksLeft.ToStringTicksToPeriod());
						return questPart_Delay.TicksLeft.ToStringTicksToPeriod(allowSeconds: false, shortForm: true, canUseDecimals: false);
					}
				}
				tip = GetAcceptedAgoByString(quest);
				return quest.TicksSinceAccepted.ToStringTicksToPeriod(allowSeconds: false, shortForm: true);
			}
		}
		tip = null;
		return null;
	}

	private void DoSelectedQuestInfo(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		if (selected == null)
		{
			Widgets.NoneLabelCenteredVertically(rect, "(" + "NoQuestSelected".Translate() + ")");
			return;
		}
		Rect val = rect.ContractedBy(17f);
		Rect outRect = val;
		Rect innerRect = default(Rect);
		((Rect)(ref innerRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width, selectedQuestLastHeight);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, selectedQuestLastHeight);
		Rect rect2 = val2;
		bool flag = ((Rect)(ref val2)).height > ((Rect)(ref val)).height;
		if (flag)
		{
			((Rect)(ref val2)).width = ((Rect)(ref val2)).width - 4f;
			((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - 16f;
		}
		Widgets.BeginScrollView(outRect, ref selectedQuestScrollPosition, val2);
		float curY = 0f;
		DoTitle(val2, ref curY);
		DoDismissButton(val2, ref curY);
		DoCharityIcon(val2);
		if (selected != null)
		{
			float curYBeforeAcceptButton = curY;
			DoAcceptButton(val2, ref curY);
			DoRightAlignedInfo(val2, ref curY, curYBeforeAcceptButton);
			DoOutcomeInfo(val2, ref curY);
			DoDescription(val2, ref curY);
			DoAcceptanceRequirementInfo(innerRect, flag, ref curY);
			DoIdeoCharityInfo(innerRect, flag, ref curY);
			DoRewards(val2, ref curY);
			DoLookTargets(val2, ref curY);
			DoSelectTargets(val2, ref curY);
			float num = curY;
			DoDefHyperlinks(val2, ref curY);
			float num2 = curY;
			curY = num;
			if (selected.root != null && !selected.root.hideInvolvedFactionsInfo)
			{
				DoFactionInfo(rect2, ref curY);
			}
			DoDebugInfoToggle(val2, ref curY);
			if (num2 > curY)
			{
				curY = num2;
			}
			DoDebugInfo(val2, ref curY);
			selectedQuestLastHeight = curY;
		}
		Widgets.EndScrollView();
	}

	private void DoTitle(Rect innerRect, ref float curY)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 100f);
		Widgets.Label(rect, selected.name.Truncate(((Rect)(ref rect)).width));
		Text.Font = GameFont.Small;
		curY += Text.LineHeight;
		curY += 17f;
	}

	private void DoDismissButton(Rect innerRect, ref float curY)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref innerRect)).xMax - 32f - 4f, ((Rect)(ref innerRect)).y, 32f, 32f);
		Texture2D tex = ((!selected.Historical && selected.dismissed) ? ResumeQuestIcon : DismissIcon);
		if (Widgets.ButtonImage(val, tex))
		{
			if (selected.Historical)
			{
				selected.hiddenInUI = true;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				Select(null);
				return;
			}
			selected.dismissed = !selected.dismissed;
			foreach (Quest subquest in selected.GetSubquests())
			{
				subquest.dismissed = selected.dismissed;
			}
			if (selected.dismissed)
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				SortQuestsByTab();
				selected = tmpQuestsToShow.FirstOrDefault((Quest x) => ShouldListNow(x));
				tmpQuestsToShow.Clear();
				return;
			}
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			Select(selected);
		}
		if (Mouse.IsOver(val))
		{
			string key = (selected.Historical ? "DeleteQuest" : (selected.dismissed ? "UnDismissQuest" : "DismissQuest"));
			TooltipHandler.TipRegion(val, key.Translate());
		}
	}

	private void DoCharityIcon(Rect innerRect)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (selected != null && selected.charity && ModsConfig.IdeologyActive)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref innerRect)).xMax - 32f - 26f - 32f - 4f, ((Rect)(ref innerRect)).y, 32f, 32f);
			GUI.DrawTexture(val, (Texture)(object)CharityQuestIcon);
			if (Mouse.IsOver(val))
			{
				TooltipHandler.TipRegion(val, "CharityQuestTip".Translate());
			}
		}
	}

	private void DoDebugInfoToggle(Rect innerRect, ref float curY)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!Prefs.DevMode)
		{
			showDebugInfo = false;
			return;
		}
		if (DebugSettings.godMode)
		{
			showDebugInfo = true;
			return;
		}
		Widgets.CheckboxLabeled(new Rect(((Rect)(ref innerRect)).xMax - 110f, curY, 110f, 30f), "DEV: Show debug info", ref showDebugInfo);
		curY += 30f;
	}

	private void DoAcceptButton(Rect innerRect, ref float curY)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		QuestPart_Choice questPart_Choice = null;
		List<QuestPart> partsListForReading = selected.PartsListForReading;
		for (int i = 0; i < partsListForReading.Count; i++)
		{
			questPart_Choice = partsListForReading[i] as QuestPart_Choice;
			if (questPart_Choice != null)
			{
				break;
			}
		}
		if (questPart_Choice != null && !Prefs.DevMode)
		{
			return;
		}
		curY += 17f;
		if (selected.State != 0)
		{
			return;
		}
		float num = ((Rect)(ref innerRect)).x;
		if (questPart_Choice == null)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(num, curY, 180f, 40f);
			AcceptanceReport acceptanceReport = QuestUtility.CanAcceptQuest(selected);
			if (!acceptanceReport.Accepted)
			{
				GUI.color = Color.grey;
				TooltipHandler.TipRegion(rect, acceptanceReport.Reason.Colorize(ColoredText.WarningColor));
			}
			if (Widgets.ButtonText(rect, "AcceptQuest".Translate()))
			{
				AcceptQuestByInterface(null, selected.RequiresAccepter);
			}
			num += ((Rect)(ref rect)).width + 10f;
			GUI.color = Color.white;
		}
		if (Prefs.DevMode && Widgets.ButtonText(new Rect(num, curY, 180f, 40f), "DEV: Accept instantly"))
		{
			SoundDefOf.Quest_Accepted.PlayOneShotOnCamera();
			if (questPart_Choice != null && questPart_Choice.choices.Any())
			{
				questPart_Choice.Choose(questPart_Choice.choices.RandomElement());
			}
			selected.Accept(PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoSuspended.Where((Pawn p) => QuestUtility.CanPawnAcceptQuest(p, selected)).RandomElementWithFallback());
			selected.dismissed = false;
			Select(selected);
		}
		curY += 44f;
	}

	private void DoRightAlignedInfo(Rect innerRect, ref float curY, float curYBeforeAcceptButton)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector2 locForDates = QuestUtility.GetLocForDates();
		float num = curYBeforeAcceptButton;
		if (!selected.initiallyAccepted && selected.EverAccepted)
		{
			if (!flag)
			{
				num += 17f;
				flag = true;
			}
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref innerRect)).x, num, ((Rect)(ref innerRect)).width, 25f);
			GUI.color = TimeLimitColor;
			Text.Anchor = (TextAnchor)5;
			string text = (selected.Historical ? GetAcceptedOnByString(selected) : GetAcceptedAgoByString(selected));
			Widgets.Label(val, text);
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
			((Rect)(ref val)).xMin = ((Rect)(ref val)).xMax - Text.CalcSize(text).x;
			if (Mouse.IsOver(val))
			{
				TooltipHandler.TipRegion(val, selected.Historical ? GetAcceptedAgoByString(selected) : GetAcceptedOnByString(selected));
			}
			if (selected.AccepterPawn != null && CameraJumper.CanJump(selected.AccepterPawn))
			{
				Widgets.DrawHighlightIfMouseover(val);
				if (Widgets.ButtonInvisible(val))
				{
					CameraJumper.TryJumpAndSelect(selected.AccepterPawn);
					Find.MainTabsRoot.EscapeCurrentTab();
				}
			}
			num += Text.LineHeight;
		}
		else if (selected.Historical)
		{
			if (!flag)
			{
				num += 17f;
				flag = true;
			}
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, num, ((Rect)(ref innerRect)).width, 25f);
			GUI.color = TimeLimitColor;
			Text.Anchor = (TextAnchor)5;
			TaggedString taggedString = "AppearedOn".Translate(GenDate.DateFullStringWithHourAt(GenDate.TickGameToAbs(selected.appearanceTick), locForDates));
			Widgets.Label(rect, taggedString);
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMax - Text.CalcSize(taggedString).x;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "AppearedDaysAgo".Translate(((float)selected.TicksSinceAppeared / 60000f).ToString("0.#")));
			}
			num += Text.LineHeight;
		}
		if (selected.State == QuestState.NotYetAccepted && selected.TicksUntilExpiry > 0)
		{
			if (!flag)
			{
				num += 17f;
				flag = true;
			}
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref innerRect)).x, num, ((Rect)(ref innerRect)).width, 25f);
			GUI.color = TimeLimitColor;
			Text.Anchor = (TextAnchor)5;
			string text2 = "QuestExpiresIn".Translate(selected.TicksUntilExpiry.ToStringTicksToPeriod());
			Widgets.Label(rect2, text2);
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMax - Text.CalcSize(text2).x;
			if (Mouse.IsOver(rect2))
			{
				TooltipHandler.TipRegion(rect2, "QuestExpiresOn".Translate(GenDate.DateFullStringWithHourAt(Find.TickManager.TicksAbs + selected.TicksUntilExpiry, locForDates)));
			}
			num += Text.LineHeight;
		}
		if (selected.State == QuestState.Ongoing)
		{
			tmpQuestParts.Clear();
			tmpQuestParts.AddRange(selected.PartsListForReading);
			tmpQuestParts.SortBy((QuestPart x) => (x is QuestPartActivable) ? ((QuestPartActivable)x).EnableTick : 0);
			Rect rect3 = default(Rect);
			for (int i = 0; i < tmpQuestParts.Count; i++)
			{
				if (!(tmpQuestParts[i] is QuestPartActivable { State: QuestPartState.Enabled, ExpiryInfoPart: var expiryInfoPart } questPartActivable) || expiryInfoPart.NullOrEmpty())
				{
					continue;
				}
				if (!flag)
				{
					num += 17f;
					flag = true;
				}
				((Rect)(ref rect3))._002Ector(((Rect)(ref innerRect)).x, num, ((Rect)(ref innerRect)).width, 25f);
				GUI.color = TimeLimitColor;
				Text.Anchor = (TextAnchor)5;
				Widgets.Label(rect3, expiryInfoPart);
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMax - Text.CalcSize(expiryInfoPart).x;
				if (Mouse.IsOver(rect3))
				{
					string expiryInfoPartTip = questPartActivable.ExpiryInfoPartTip;
					if (!expiryInfoPartTip.NullOrEmpty())
					{
						TooltipHandler.TipRegion(rect3, expiryInfoPartTip);
					}
				}
				num += Text.LineHeight;
			}
			tmpQuestParts.Clear();
		}
		curY = Mathf.Max(curY, num);
	}

	private void DoAcceptanceRequirementInfo(Rect innerRect, bool scrollBarVisible, ref float curY)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!selected.EverAccepted && !selected.Historical)
		{
			IEnumerable<string> enumerable = ListUnmetAcceptRequirements();
			int num = enumerable.Count();
			if (num != 0)
			{
				bool flag = num > 1;
				string text = "QuestAcceptanceRequirementsDescription".Translate() + (flag ? ": " : " ") + (flag ? ("\n" + enumerable.ToLineList("  - ", capitalizeItems: true)) : (enumerable.First() + "."));
				curY += 17f;
				DrawInfoBox(innerRect, scrollBarVisible, ref curY, text, acceptanceRequirementsBoxBgColor, AcceptanceRequirementsBoxColor, AcceptanceRequirementsColor);
				new LookTargets(ListUnmetAcceptRequirementCulprits()).TryHighlight(arrow: true, colonistBar: true, circleOverlay: true);
			}
		}
	}

	private void DoIdeoCharityInfo(Rect innerRect, bool scrollBarVisible, ref float curY)
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (!selected.charity || !ModsConfig.IdeologyActive)
		{
			return;
		}
		List<Pawn> allMaps_FreeColonistsSpawned = PawnsFinder.AllMaps_FreeColonistsSpawned;
		List<Ideo> ideosListForReading = Find.IdeoManager.IdeosListForReading;
		string text = "";
		for (int i = 0; i < ideosListForReading.Count; i++)
		{
			Ideo ideo = ideosListForReading[i];
			List<Precept> preceptsListForReading = ideo.PreceptsListForReading;
			bool flag = false;
			for (int j = 0; j < preceptsListForReading.Count; j++)
			{
				if (preceptsListForReading[j].def.issue == IssueDefOf.Charity)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			tmpColonistsForIdeo.Clear();
			for (int k = 0; k < allMaps_FreeColonistsSpawned.Count; k++)
			{
				Pawn pawn = allMaps_FreeColonistsSpawned[k];
				if (pawn != null && pawn.Ideo == ideo && !pawn.IsQuestReward(selected))
				{
					tmpColonistsForIdeo.Add(pawn);
				}
			}
			if (tmpColonistsForIdeo.Count != 0)
			{
				if (!text.NullOrEmpty())
				{
					text += "\n\n";
				}
				text += "IdeoCharityQuestInfo".Translate(ideo.name, GenThing.ThingsToCommaList(tmpColonistsForIdeo));
			}
		}
		if (!text.NullOrEmpty())
		{
			curY += 17f;
			DrawInfoBox(innerRect, scrollBarVisible, ref curY, text, IdeoCharityBoxBackgroundColor, IdeoCharityBoxBorderColor, IdeoCharityTextColor);
		}
	}

	private void DrawInfoBox(Rect innerRect, bool scrollBarVisible, ref float curY, string text, Color boxBackground, Color boxBorder, Color textColor)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = ((Rect)(ref innerRect)).x + 8f;
		float num3 = ((Rect)(ref innerRect)).width - 16f;
		if (scrollBarVisible)
		{
			num3 -= 31f;
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(num2, curY, num3, 10000f);
		num += Text.CalcHeight(text, ((Rect)(ref rect)).width);
		Rect rect2 = GenUI.ExpandedBy(new Rect(num2, curY, num3, num), 8f);
		Widgets.DrawBoxSolid(rect2, boxBackground);
		GUI.color = textColor;
		Widgets.Label(rect, text);
		GUI.color = boxBorder;
		Widgets.DrawBox(rect2, 2);
		curY += num;
		GUI.color = Color.white;
	}

	private IEnumerable<string> ListUnmetAcceptRequirements()
	{
		uniqueEntries.Clear();
		for (int i = 0; i < selected.PartsListForReading.Count; i++)
		{
			if (selected.PartsListForReading[i] is QuestPart_RequirementsToAccept { ShowInRequirementBox: not false } questPart_RequirementsToAccept)
			{
				AcceptanceReport acceptanceReport = questPart_RequirementsToAccept.CanAccept();
				if (!acceptanceReport.Accepted)
				{
					uniqueEntries.Add(acceptanceReport.Reason);
				}
			}
		}
		foreach (string uniqueEntry in uniqueEntries)
		{
			yield return uniqueEntry;
		}
	}

	private IEnumerable<GlobalTargetInfo> ListUnmetAcceptRequirementCulprits()
	{
		for (int i = 0; i < selected.PartsListForReading.Count; i++)
		{
			if (!(selected.PartsListForReading[i] is QuestPart_RequirementsToAccept questPart_RequirementsToAccept))
			{
				continue;
			}
			foreach (GlobalTargetInfo culprit in questPart_RequirementsToAccept.Culprits)
			{
				yield return culprit;
			}
		}
	}

	private void DoOutcomeInfo(Rect innerRect, ref float curY)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (selected.Historical)
		{
			string text = ((selected.State == QuestState.EndedOfferExpired) ? ((string)"QuestOutcomeInfo_OfferExpired".Translate()) : ((selected.State == QuestState.EndedUnknownOutcome || selected.State == QuestState.EndedSuccess) ? ((string)"QuestOutcomeInfo_UnknownOrSuccess".Translate()) : ((selected.State == QuestState.EndedFailed) ? ((string)"QuestOutcomeInfo_Failed".Translate()) : ((selected.State != QuestState.EndedInvalid) ? null : ((string)"QuestOutcomeInfo_Invalid".Translate())))));
			if (!text.NullOrEmpty())
			{
				curY += 17f;
				Widgets.Label(new Rect(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 25f), text);
				curY += Text.LineHeight;
			}
		}
	}

	private void DoDescription(Rect innerRect, ref float curY)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		if (!selected.description.RawText.NullOrEmpty())
		{
			string value = selected.description.Resolve();
			stringBuilder.Append(value);
		}
		tmpQuestParts.Clear();
		tmpQuestParts.AddRange(selected.PartsListForReading);
		tmpQuestParts.SortBy((QuestPart x) => (x is QuestPartActivable) ? ((QuestPartActivable)x).EnableTick : 0);
		for (int i = 0; i < tmpQuestParts.Count; i++)
		{
			if (tmpQuestParts[i] is QuestPartActivable { State: not QuestPartState.Enabled })
			{
				continue;
			}
			string descriptionPart = tmpQuestParts[i].DescriptionPart;
			if (!descriptionPart.NullOrEmpty())
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
				}
				stringBuilder.Append(descriptionPart);
			}
		}
		tmpQuestParts.Clear();
		if (stringBuilder.Length != 0)
		{
			curY += 17f;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 10000f);
			Widgets.Label(rect, stringBuilder.ToString());
			curY += Text.CalcHeight(stringBuilder.ToString(), ((Rect)(ref rect)).width);
		}
	}

	private void DoRewards(Rect innerRect, ref float curY)
	{
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		QuestPart_Choice choice = null;
		List<QuestPart> partsListForReading = selected.PartsListForReading;
		for (int i = 0; i < partsListForReading.Count; i++)
		{
			choice = partsListForReading[i] as QuestPart_Choice;
			if (choice != null)
			{
				break;
			}
		}
		if (choice == null)
		{
			return;
		}
		bool flag = selected.State == QuestState.NotYetAccepted;
		bool flag2 = true;
		Rect rect = default(Rect);
		Rect rect3 = default(Rect);
		for (int j = 0; j < choice.choices.Count; j++)
		{
			tmpStackElements.Clear();
			float num = 0f;
			for (int k = 0; k < choice.choices[j].rewards.Count; k++)
			{
				tmpStackElements.AddRange(choice.choices[j].rewards[k].StackElements);
				num += choice.choices[j].rewards[k].TotalMarketValue;
			}
			if (!tmpStackElements.Any())
			{
				continue;
			}
			if (num > 0f && (choice.choices[j].rewards.Count != 1 || !(choice.choices[j].rewards[0] is Reward_Items { items: not null } reward_Items) || reward_Items.items.Count != 1 || !(reward_Items.items[0].StyleSourcePrecept is Precept_Relic)))
			{
				TaggedString totalValueStr = "TotalValue".Translate(num.ToStringMoney("F0"));
				tmpStackElements.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0041: Unknown result type (might be due to invalid IL or missing references)
						//IL_0051: Unknown result type (might be due to invalid IL or missing references)
						GUI.color = new Color(0.7f, 0.7f, 0.7f);
						Widgets.Label(new Rect(((Rect)(ref r)).x + 5f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 10f, ((Rect)(ref r)).height), totalValueStr);
						GUI.color = Color.white;
					},
					width = Text.CalcSize(totalValueStr).x + 10f
				});
			}
			if (flag2)
			{
				curY += 17f;
				flag2 = false;
			}
			else
			{
				curY += 10f;
			}
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 10000f);
			Rect rect2 = rect.ContractedBy(10f);
			if (flag)
			{
				((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 100f;
			}
			Rect val = GenUI.DrawElementStack(rect2, 24f, tmpStackElements, null, (GenUI.AnonymousStackElement obj) => obj.width, 4f, 5f, allowOrderOptimization: false);
			((Rect)(ref rect)).height = ((Rect)(ref val)).height + 20f;
			Widgets.DrawBoxSolid(rect, new Color(0.13f, 0.13f, 0.13f));
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			Widgets.DrawHighlightIfMouseover(rect);
			GUI.color = Color.white;
			GenUI.DrawElementStack(rect2, 24f, tmpStackElements, delegate(Rect r, GenUI.AnonymousStackElement obj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				obj.drawer(r);
			}, (GenUI.AnonymousStackElement obj) => obj.width, 4f, 5f, allowOrderOptimization: false);
			if (flag)
			{
				AcceptanceReport acceptanceReport = QuestUtility.CanAcceptQuest(selected);
				if (!acceptanceReport)
				{
					GUI.color = Color.grey;
				}
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, 100f, ((Rect)(ref rect)).height);
				if (Widgets.ButtonText(rect3, "AcceptQuestFor".Translate() + ":"))
				{
					tmpRemainingQuestParts.Clear();
					tmpRemainingQuestParts.AddRange(selected.PartsListForReading);
					for (int l = 0; l < choice.choices.Count; l++)
					{
						if (j == l)
						{
							continue;
						}
						for (int m = 0; m < choice.choices[l].questParts.Count; m++)
						{
							QuestPart item = choice.choices[l].questParts[m];
							if (!choice.choices[j].questParts.Contains(item))
							{
								tmpRemainingQuestParts.Remove(item);
							}
						}
					}
					bool requiresAccepter = false;
					for (int n = 0; n < tmpRemainingQuestParts.Count; n++)
					{
						if (tmpRemainingQuestParts[n].RequiresAccepter)
						{
							requiresAccepter = true;
							break;
						}
					}
					tmpRemainingQuestParts.Clear();
					QuestPart_Choice.Choice localChoice = choice.choices[j];
					AcceptQuestByInterface(delegate
					{
						choice.Choose(localChoice);
					}, requiresAccepter);
				}
				TaggedString taggedString = "AcceptQuestForTip".Translate();
				if (!acceptanceReport && !acceptanceReport.Reason.NullOrEmpty())
				{
					taggedString += "\n\n" + acceptanceReport.Reason.Colorize(ColorLibrary.RedReadable);
				}
				TooltipHandler.TipRegion(rect3, taggedString);
				GUI.color = Color.white;
			}
			curY += ((Rect)(ref rect)).height;
		}
		tmpStackElements.Clear();
	}

	private void DoLookTargets(Rect innerRect, ref float curY)
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		List<Map> maps = Find.Maps;
		int num = 0;
		for (int i = 0; i < maps.Count; i++)
		{
			if (maps[i].IsPlayerHome)
			{
				num++;
			}
		}
		tmpLookTargets.Clear();
		tmpLookTargets.AddRange(selected.QuestLookTargets);
		tmpLookTargets.SortBy(delegate(GlobalTargetInfo x)
		{
			if (x.Thing is Pawn)
			{
				return 0;
			}
			if (x.HasThing)
			{
				return 1;
			}
			if (!x.IsWorldTarget)
			{
				return 2;
			}
			return (!(x.WorldObject is Settlement) || ((Settlement)x.WorldObject).Faction != Faction.OfPlayer) ? 3 : 4;
		}, (GlobalTargetInfo x) => x.Label);
		bool flag = false;
		for (int j = 0; j < tmpLookTargets.Count; j++)
		{
			GlobalTargetInfo globalTargetInfo = tmpLookTargets[j];
			if (globalTargetInfo.HasWorldObject && globalTargetInfo.WorldObject is MapParent mapParent && (!mapParent.HasMap || !mapParent.Map.IsPlayerHome))
			{
				flag = true;
				break;
			}
		}
		bool flag2 = false;
		for (int k = 0; k < tmpLookTargets.Count; k++)
		{
			GlobalTargetInfo globalTargetInfo2 = tmpLookTargets[k];
			if (!CameraJumper.CanJump(globalTargetInfo2))
			{
				continue;
			}
			Map anyPlayerHomeMap = Find.AnyPlayerHomeMap;
			if (num != 1 || anyPlayerHomeMap == null || !(globalTargetInfo2 == anyPlayerHomeMap.Parent) || flag)
			{
				if (!flag2)
				{
					flag2 = true;
					curY += 17f;
				}
				if (Widgets.ButtonText(new Rect(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 25f), "JumpToTargetCustom".Translate(globalTargetInfo2.Label), drawBackground: false))
				{
					CameraJumper.TryJumpAndSelect(globalTargetInfo2);
					Find.MainTabsRoot.EscapeCurrentTab();
				}
				curY += 25f;
			}
		}
	}

	private void DoSelectTargets(Rect innerRect, ref float curY)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		for (int i = 0; i < selected.PartsListForReading.Count; i++)
		{
			QuestPart questPart = selected.PartsListForReading[i];
			tmpSelectTargets.Clear();
			tmpSelectTargets.AddRange(questPart.QuestSelectTargets);
			if (tmpSelectTargets.Count == 0)
			{
				continue;
			}
			if (!flag)
			{
				flag = true;
				curY += 4f;
			}
			if (Widgets.ButtonText(new Rect(((Rect)(ref innerRect)).x, curY, ((Rect)(ref innerRect)).width, 25f), questPart.QuestSelectTargetsLabel, drawBackground: false))
			{
				Map map = null;
				int num = 0;
				Vector3 val = Vector3.zero;
				Find.Selector.ClearSelection();
				for (int j = 0; j < tmpSelectTargets.Count; j++)
				{
					GlobalTargetInfo target = tmpSelectTargets[j];
					if (CameraJumper.CanJump(target) && target.HasThing)
					{
						Find.Selector.Select(target.Thing);
						if (map == null)
						{
							map = target.Map;
						}
						else if (target.Map != map)
						{
							num = 0;
							break;
						}
						val += target.Cell.ToVector3();
						num++;
					}
				}
				if (num > 0)
				{
					CameraJumper.TryJump(new IntVec3(val / (float)num), map);
				}
				Find.MainTabsRoot.EscapeCurrentTab();
			}
			curY += 25f;
		}
	}

	private void DoFactionInfo(Rect rect, ref float curY)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		curY += 15f;
		foreach (Faction involvedFaction in selected.InvolvedFactions)
		{
			if (involvedFaction != null && !involvedFaction.Hidden && !involvedFaction.IsPlayer)
			{
				FactionUIUtility.DrawRelatedFactionInfo(rect, involvedFaction, ref curY);
			}
		}
	}

	private void DoDefHyperlinks(Rect rect, ref float curY)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		curY += 25f;
		Rect rect2 = default(Rect);
		foreach (Dialog_InfoCard.Hyperlink hyperlink in selected.Hyperlinks)
		{
			float num = Text.CalcHeight(hyperlink.Label, ((Rect)(ref rect)).width);
			float num2 = ((Rect)(ref rect)).width / 2f;
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, num2, num);
			Color value = Widgets.NormalOptionColor;
			if (hyperlink.quest != null && (hyperlink.quest.IsSubquestOf(selected) || selected.IsSubquestOf(hyperlink.quest)))
			{
				if (!selected.hidden && !hyperlink.quest.hidden)
				{
					string text = "";
					if (hyperlink.quest.Historical)
					{
						text += "(" + "Finished".Translate().ToLower() + ") ";
						value = Color.gray;
					}
					text += (hyperlink.quest.IsSubquestOf(selected) ? "HasSubquest".Translate() : "SubquestOf".Translate());
					text = text + ": " + hyperlink.Label;
					Widgets.HyperlinkWithIcon(rect2, hyperlink, text, 2f, 6f, value, truncateLabel: true);
				}
			}
			else
			{
				Widgets.HyperlinkWithIcon(rect2, hyperlink, "ViewHyperlink".Translate(hyperlink.Label), 2f, 6f, value);
			}
			curY += num;
		}
	}

	private void DoDebugInfo(Rect innerRect, ref float curY)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		if (!showDebugInfo)
		{
			return;
		}
		curY += 17f;
		List<QuestPart> partsListForReading = selected.PartsListForReading;
		if (selected.State == QuestState.Ongoing)
		{
			for (int i = 0; i < partsListForReading.Count; i++)
			{
				partsListForReading[i].DoDebugWindowContents(innerRect, ref curY);
			}
		}
		if (selected.State == QuestState.Ongoing || selected.State == QuestState.NotYetAccepted)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(((Rect)(ref innerRect)).x, curY, 210f, 25f);
			debugSendSignalTextField = Widgets.TextField(rect, debugSendSignalTextField);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref innerRect)).x + ((Rect)(ref rect)).width + 4f, curY, 117f, 25f);
			if (Widgets.ButtonText(rect2, "Send signal"))
			{
				Find.SignalManager.SendSignal(new Signal(debugSendSignalTextField));
				debugSendSignalTextField = "";
			}
			if (Widgets.ButtonText(new Rect(((Rect)(ref rect2)).xMax + 4f, curY, 165f, 25f), "Send defined signal..."))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (string item in from x in DebugPossibleSignals(selected).Distinct()
					orderby x
					select x)
				{
					string signalLocal = item;
					list.Add(new FloatMenuOption(signalLocal, delegate
					{
						Find.SignalManager.SendSignal(new Signal(signalLocal));
						debugSendSignalTextField = "";
					}));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
			curY += ((Rect)(ref rect)).height + 4f;
		}
		string text = "-----------------";
		text = text + "\nId: " + selected.id;
		text = text + "\nState: " + selected.State;
		text += "\nData:";
		text = text + "\n" + Scribe.saver.DebugOutputFor(selected);
		text += "\n";
		text += "\nActive QuestParts:";
		bool flag = false;
		for (int j = 0; j < partsListForReading.Count; j++)
		{
			if (partsListForReading[j] is QuestPartActivable { State: QuestPartState.Enabled } questPartActivable)
			{
				text = text + "\n" + questPartActivable.ToString();
				flag = true;
			}
		}
		if (!flag)
		{
			text += "\nNone";
		}
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref innerRect)).x, curY, 180f, 40f);
		if (Widgets.ButtonText(rect3, "Copy debug to clipboard"))
		{
			GUIUtility.systemCopyBuffer = text;
		}
		curY += ((Rect)(ref rect3)).height + 4f;
		Widgets.LongLabel(((Rect)(ref innerRect)).x, ((Rect)(ref innerRect)).width, text, ref curY);
	}

	private bool ShouldListNow(Quest quest)
	{
		if (quest.hidden && !showAll)
		{
			return false;
		}
		switch (curTab)
		{
		case QuestsTab.Available:
			if (quest.State == QuestState.NotYetAccepted && !quest.dismissed)
			{
				return !quest.hiddenInUI;
			}
			return false;
		case QuestsTab.Active:
			if (quest.State == QuestState.Ongoing && !quest.dismissed)
			{
				return !quest.hiddenInUI;
			}
			return false;
		case QuestsTab.Historical:
			if (!quest.hiddenInUI)
			{
				if (!quest.Historical)
				{
					return quest.dismissed;
				}
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	private IEnumerable<string> DebugPossibleSignals(Quest quest)
	{
		string input = Scribe.saver.DebugOutputFor(selected);
		foreach (Match item in Regex.Matches(input, ">(Quest" + quest.id + "\\.[a-zA-Z0-9/\\-\\.]*)<"))
		{
			yield return item.Groups[1].Value;
		}
	}

	private string GetAcceptedAgoByString(Quest quest)
	{
		string text = quest.TicksSinceAccepted.ToStringTicksToPeriod();
		if (!quest.AccepterPawnLabelCap.NullOrEmpty())
		{
			return "AcceptedAgoBy".Translate(text, quest.AccepterPawnLabelCap);
		}
		return "AcceptedAgo".Translate(text);
	}

	private string GetAcceptedOnByString(Quest quest)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector2 locForDates = QuestUtility.GetLocForDates();
		string text = GenDate.DateFullStringWithHourAt(GenDate.TickGameToAbs(quest.acceptanceTick), locForDates);
		if (!quest.AccepterPawnLabelCap.NullOrEmpty())
		{
			return "AcceptedOnBy".Translate(text, quest.AccepterPawnLabelCap);
		}
		return "AcceptedOn".Translate(text);
	}

	private void AcceptQuestByInterface(Action preAcceptAction = null, bool requiresAccepter = false)
	{
		if (!QuestUtility.CanAcceptQuest(selected))
		{
			Messages.Message("MessageCannotAcceptQuest".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		if (!requiresAccepter)
		{
			SoundDefOf.Quest_Accepted.PlayOneShotOnCamera();
			if (preAcceptAction != null)
			{
				preAcceptAction();
			}
			selected.Accept(null);
			Select(selected);
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		foreach (Pawn p in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoSuspended)
		{
			if (!QuestUtility.CanPawnAcceptQuest(p, selected))
			{
				continue;
			}
			Pawn pLocal = p;
			string text = "AcceptWith".Translate(p);
			if (p.royalty != null && p.royalty.AllTitlesInEffectForReading.Any())
			{
				text = text + " (" + p.royalty.MostSeniorTitle.def.GetLabelFor(pLocal) + ")";
			}
			list.Add(new FloatMenuOption(text, delegate
			{
				if (QuestUtility.CanPawnAcceptQuest(pLocal, selected))
				{
					QuestPart_GiveRoyalFavor questPart_GiveRoyalFavor = selected.PartsListForReading.OfType<QuestPart_GiveRoyalFavor>().FirstOrDefault();
					if (questPart_GiveRoyalFavor != null && questPart_GiveRoyalFavor.giveToAccepter)
					{
						IEnumerable<Trait> conceitedTraits = RoyalTitleUtility.GetConceitedTraits(p);
						IEnumerable<Trait> traitsAffectingPsylinkNegatively = RoyalTitleUtility.GetTraitsAffectingPsylinkNegatively(p);
						bool totallyDisabled = p.skills.GetSkill(SkillDefOf.Social).TotallyDisabled;
						bool flag = conceitedTraits.Any();
						bool flag2 = !p.HasPsylink && traitsAffectingPsylinkNegatively.Any();
						if (totallyDisabled || flag || flag2)
						{
							NamedArgument arg = p.Named("PAWN");
							NamedArgument arg2 = questPart_GiveRoyalFavor.faction.Named("FACTION");
							TaggedString taggedString = null;
							if (totallyDisabled)
							{
								taggedString = "RoyalIncapableOfSocial".Translate(arg, arg2);
							}
							TaggedString taggedString2 = null;
							if (flag)
							{
								taggedString2 = "RoyalWithConceitedTrait".Translate(arg, arg2, conceitedTraits.Select((Trait t) => t.Label).ToCommaList(useAnd: true));
							}
							TaggedString taggedString3 = null;
							if (flag2)
							{
								taggedString3 = "RoyalWithTraitAffectingPsylinkNegatively".Translate(arg, arg2, traitsAffectingPsylinkNegatively.Select((Trait t) => t.Label).ToCommaList(useAnd: true));
							}
							TaggedString text2 = "QuestGivesRoyalFavor".Translate(arg, arg2);
							if (totallyDisabled)
							{
								text2 += "\n\n" + taggedString;
							}
							if (flag)
							{
								text2 += "\n\n" + taggedString2;
							}
							if (flag2)
							{
								text2 += "\n\n" + taggedString3;
							}
							text2 += "\n\n" + "WantToContinue".Translate();
							Find.WindowStack.Add(new Dialog_MessageBox(text2, "Confirm".Translate(), AcceptAction, "GoBack".Translate()));
						}
						else
						{
							AcceptAction();
						}
					}
					else
					{
						AcceptAction();
					}
				}
			}));
			void AcceptAction()
			{
				SoundDefOf.Quest_Accepted.PlayOneShotOnCamera();
				if (preAcceptAction != null)
				{
					preAcceptAction();
				}
				selected.Accept(pLocal);
				Select(selected);
				Messages.Message("MessageQuestAccepted".Translate(pLocal, selected.name), pLocal, MessageTypeDefOf.TaskCompletion, historical: false);
			}
		}
		if (list.Count > 0)
		{
			Find.WindowStack.Add(new FloatMenu(list));
		}
		else
		{
			Messages.Message("MessageNoColonistCanAcceptQuest".Translate(Faction.OfPlayer.def.pawnsPlural), MessageTypeDefOf.RejectInput, historical: false);
		}
	}
}
