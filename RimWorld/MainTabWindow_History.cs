using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MainTabWindow_History : MainTabWindow
{
	private enum HistoryTab : byte
	{
		Graph,
		Messages,
		Statistics
	}

	private HistoryAutoRecorderGroup historyAutoRecorderGroup;

	private FloatRange graphSection;

	private Vector2 messagesScrollPos;

	private float messagesLastHeight;

	private List<TabRecord> tabs = new List<TabRecord>();

	private int displayedMessageIndex;

	private static QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private static HistoryTab curTab = HistoryTab.Graph;

	private static bool showLetters = true;

	private static bool showMessages;

	private const float MessagesRowHeight = 30f;

	private const float PinColumnSize = 30f;

	private const float PinSize = 22f;

	private const float IconColumnSize = 30f;

	private const float DateSize = 90f;

	private const float SpaceBetweenColumns = 5f;

	private static readonly Vector2 SearchBarOffset = new Vector2(720f, 8f);

	private static readonly Texture2D PinTex = ContentFinder<Texture2D>.Get("UI/Icons/Pin");

	private static readonly Texture2D PinOutlineTex = ContentFinder<Texture2D>.Get("UI/Icons/Pin-Outline");

	private static readonly Color PinOutlineColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);

	private Dictionary<string, string> truncationCache = new Dictionary<string, string>();

	private static List<CurveMark> marks = new List<CurveMark>();

	public override Vector2 RequestedTabSize => new Vector2(1010f, 640f);

	public override void PreOpen()
	{
		base.PreOpen();
		tabs.Clear();
		tabs.Add(new TabRecord("Graph".Translate(), delegate
		{
			curTab = HistoryTab.Graph;
		}, () => curTab == HistoryTab.Graph));
		tabs.Add(new TabRecord("Messages".Translate(), delegate
		{
			curTab = HistoryTab.Messages;
		}, () => curTab == HistoryTab.Messages));
		tabs.Add(new TabRecord("Statistics".Translate(), delegate
		{
			curTab = HistoryTab.Statistics;
		}, () => curTab == HistoryTab.Statistics));
		historyAutoRecorderGroup = Find.History.Groups().FirstOrDefault();
		if (historyAutoRecorderGroup != null)
		{
			graphSection = new FloatRange(0f, (float)Find.TickManager.TicksGame / 60000f);
		}
		List<Map> maps = Find.Maps;
		for (int i = 0; i < maps.Count; i++)
		{
			maps[i].wealthWatcher.ForceRecount();
		}
		quickSearchWidget.Reset();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 45f;
		TabDrawer.DrawTabs(val, tabs);
		switch (curTab)
		{
		case HistoryTab.Graph:
			DoGraphPage(val);
			break;
		case HistoryTab.Messages:
			DoMessagesPage(val);
			break;
		case HistoryTab.Statistics:
			DoStatisticsPage(val);
			break;
		}
	}

	private void DoStatisticsPage(Rect rect)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 17f;
		Widgets.BeginGroup(rect);
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = new TimeSpan(0, 0, (int)Find.GameInfo.RealPlayTimeInteracting);
		stringBuilder.AppendLine(string.Concat(string.Concat(string.Concat(string.Concat("Playtime".Translate() + ": ", timeSpan.Days.ToString()) + "LetterDay".Translate() + " ", timeSpan.Hours.ToString()) + "LetterHour".Translate() + " ", timeSpan.Minutes.ToString()) + "LetterMinute".Translate() + " ", timeSpan.Seconds.ToString()) + "LetterSecond".Translate());
		stringBuilder.AppendLine("Storyteller".Translate() + ": " + Find.Storyteller.def.LabelCap);
		stringBuilder.AppendLine("Difficulty".Translate() + ": " + Find.Storyteller.difficultyDef.LabelCap);
		if (Find.CurrentMap != null)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("ThisMapColonyWealthTotal".Translate() + ": " + Find.CurrentMap.wealthWatcher.WealthTotal.ToString("F0"));
			stringBuilder.AppendLine("ThisMapColonyWealthItems".Translate() + ": " + Find.CurrentMap.wealthWatcher.WealthItems.ToString("F0"));
			stringBuilder.AppendLine("ThisMapColonyWealthBuildings".Translate() + ": " + Find.CurrentMap.wealthWatcher.WealthBuildings.ToString("F0"));
			stringBuilder.AppendLine("ThisMapColonyWealthColonistsAndTameAnimals".Translate() + ": " + Find.CurrentMap.wealthWatcher.WealthPawns.ToString("F0"));
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(string.Concat("NumThreatBigs".Translate() + ": ", Find.StoryWatcher.statsRecord.numThreatBigs.ToString()));
		stringBuilder.AppendLine(string.Concat("NumEnemyRaids".Translate() + ": ", Find.StoryWatcher.statsRecord.numRaidsEnemy.ToString()));
		stringBuilder.AppendLine();
		if (Find.CurrentMap != null)
		{
			stringBuilder.AppendLine(string.Concat("ThisMapDamageTaken".Translate() + ": ", Find.CurrentMap.damageWatcher.DamageTakenEver.ToString()));
		}
		stringBuilder.AppendLine(string.Concat("ColonistsKilled".Translate() + ": ", Find.StoryWatcher.statsRecord.colonistsKilled.ToString()));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(string.Concat("ColonistsLaunched".Translate() + ": ", Find.StoryWatcher.statsRecord.colonistsLaunched.ToString()));
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(new Rect(0f, 0f, 400f, 400f), stringBuilder.ToString());
		Widgets.EndGroup();
	}

	private void DoMessagesPage(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 10f;
		Widgets.CheckboxLabeled(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, 200f, 30f), "ShowLetters".Translate(), ref showLetters, disabled: false, null, null, placeCheckboxNearText: true);
		Widgets.CheckboxLabeled(new Rect(((Rect)(ref rect)).x + 200f, ((Rect)(ref rect)).y, 200f, 30f), "ShowMessages".Translate(), ref showMessages, disabled: false, null, null, placeCheckboxNearText: true);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 40f;
		bool flag = false;
		Rect outRect = rect;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width / 2f - 16f, messagesLastHeight);
		List<IArchivable> archivablesListForReading = Find.Archive.ArchivablesListForReading;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f + 10f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f - 10f - 16f, ((Rect)(ref rect)).height);
		displayedMessageIndex = -1;
		quickSearchWidget.noResultsMatched = !archivablesListForReading.Any();
		Widgets.BeginScrollView(outRect, ref messagesScrollPos, viewRect);
		float num = 0f;
		for (int num2 = archivablesListForReading.Count - 1; num2 >= 0; num2--)
		{
			if ((showLetters || (!(archivablesListForReading[num2] is Letter) && !(archivablesListForReading[num2] is ArchivedDialog))) && (showMessages || !(archivablesListForReading[num2] is Message)))
			{
				flag = true;
				if (num2 > displayedMessageIndex)
				{
					displayedMessageIndex = num2;
				}
				if (num + 30f >= messagesScrollPos.y && num <= messagesScrollPos.y + ((Rect)(ref outRect)).height)
				{
					DoArchivableRow(new Rect(0f, num, ((Rect)(ref viewRect)).width - 5f, 30f), archivablesListForReading[num2], num2);
				}
				num += 30f;
			}
		}
		messagesLastHeight = num;
		Widgets.EndScrollView();
		if (flag)
		{
			if (displayedMessageIndex >= 0)
			{
				TaggedString label = archivablesListForReading[displayedMessageIndex].ArchivedTooltip.TruncateHeight(((Rect)(ref rect2)).width - 10f, ((Rect)(ref rect2)).height - 10f, truncationCache);
				Widgets.Label(rect2.ContractedBy(5f), label);
			}
		}
		else
		{
			Widgets.NoneLabel(((Rect)(ref rect)).yMin + 3f, ((Rect)(ref rect)).width, "(" + "NoMessages".Translate() + ")");
		}
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref val)).x + SearchBarOffset.x, ((Rect)(ref val)).y + SearchBarOffset.y - Window.QuickSearchSize.y - 10f, Window.QuickSearchSize.x, Window.QuickSearchSize.y);
		quickSearchWidget.OnGUI(rect3, Notify_CommonSearchChanged);
	}

	private void DoArchivableRow(Rect rect, IArchivable archivable, int index)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Rect val = rect;
		bool flag = quickSearchWidget.filter.Active && quickSearchWidget.filter.Matches(archivable.ArchivedLabel);
		if (flag)
		{
			Widgets.DrawTextHighlight(rect, 0f);
			if (quickSearchWidget.filter.Active && quickSearchWidget.CurrentlyFocused())
			{
				displayedMessageIndex = index;
			}
		}
		else if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		Widgets.DrawHighlightIfMouseover(rect);
		Rect val2 = val;
		((Rect)(ref val2)).width = 30f;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 35f;
		float num = (Find.Archive.IsPinned(archivable) ? 1f : ((!Mouse.IsOver(val2)) ? 0f : 0.25f));
		Rect val3 = GenUI.Rounded(new Rect(((Rect)(ref val2)).x + (((Rect)(ref val2)).width - 22f) / 2f, ((Rect)(ref val2)).y + (((Rect)(ref val2)).height - 22f) / 2f, 22f, 22f));
		if (num > 0f)
		{
			GUI.color = new Color(1f, 1f, 1f, num);
			GUI.DrawTexture(val3, (Texture)(object)PinTex);
		}
		else
		{
			GUI.color = PinOutlineColor;
			GUI.DrawTexture(val3, (Texture)(object)PinOutlineTex);
		}
		GUI.color = Color.white;
		Rect val4 = val;
		Rect outerRect = val;
		((Rect)(ref outerRect)).width = 30f;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 35f;
		Texture archivedIcon = archivable.ArchivedIcon;
		if ((Object)(object)archivedIcon != (Object)null)
		{
			GUI.color = archivable.ArchivedIconColor;
			Widgets.DrawTextureFitted(outerRect, archivedIcon, 0.8f);
			GUI.color = Color.white;
		}
		Rect rect2 = val;
		((Rect)(ref rect2)).width = 90f;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 95f;
		Vector2 location = (Vector2)((Find.CurrentMap != null) ? Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile) : default(Vector2));
		GUI.color = new Color(0.75f, 0.75f, 0.75f);
		Widgets.Label(label: GenDate.DateShortStringAt(GenDate.TickGameToAbs(archivable.CreatedTicksGame), location).Truncate(((Rect)(ref rect2)).width), rect: rect2);
		GUI.color = Color.white;
		Rect rect3 = val;
		if (!flag)
		{
			GUI.color = Color.gray;
		}
		Widgets.Label(rect3, archivable.ArchivedLabel.Truncate(((Rect)(ref rect3)).width));
		GenUI.ResetLabelAlign();
		Text.WordWrap = true;
		GUI.color = Color.white;
		TooltipHandler.TipRegionByKey(val2, "PinArchivableTip", 200);
		if (Mouse.IsOver(val4))
		{
			displayedMessageIndex = index;
		}
		if (Widgets.ButtonInvisible(val2))
		{
			if (Find.Archive.IsPinned(archivable))
			{
				Find.Archive.Unpin(archivable);
				SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
			}
			else
			{
				Find.Archive.Pin(archivable);
				SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
			}
		}
		if (!Widgets.ButtonInvisible(val4))
		{
			return;
		}
		if (Event.current.button == 1)
		{
			LookTargets lookTargets = archivable.LookTargets;
			if (CameraJumper.CanJump(lookTargets.TryGetPrimaryTarget()))
			{
				CameraJumper.TryJumpAndSelect(lookTargets.TryGetPrimaryTarget());
				Find.MainTabsRoot.EscapeCurrentTab();
			}
		}
		else
		{
			archivable.OpenArchived();
		}
	}

	private void DoGraphPage(Rect rect)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 17f;
		Widgets.BeginGroup(rect);
		Rect graphRect = default(Rect);
		((Rect)(ref graphRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width, 450f);
		Rect legendRect = default(Rect);
		((Rect)(ref legendRect))._002Ector(0f, ((Rect)(ref graphRect)).yMax, ((Rect)(ref rect)).width / 2f, 40f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, ((Rect)(ref legendRect)).yMax, ((Rect)(ref rect)).width, 40f);
		if (historyAutoRecorderGroup != null)
		{
			marks.Clear();
			List<Tale> allTalesListForReading = Find.TaleManager.AllTalesListForReading;
			for (int i = 0; i < allTalesListForReading.Count; i++)
			{
				Tale tale = allTalesListForReading[i];
				if (tale.def.type == TaleType.PermanentHistorical && !tale.hidden)
				{
					float x = (float)GenDate.TickAbsToGame(tale.date) / 60000f;
					marks.Add(new CurveMark(x, tale.ShortSummary, tale.def.historyGraphColor));
				}
			}
			historyAutoRecorderGroup.DrawGraph(graphRect, legendRect, graphSection, marks);
		}
		Text.Font = GameFont.Small;
		float num = (float)Find.TickManager.TicksGame / 60000f;
		if (Widgets.ButtonText(new Rect(((Rect)(ref legendRect)).xMin + ((Rect)(ref legendRect)).width, ((Rect)(ref legendRect)).yMin, 110f, 40f), "Last30Days".Translate()))
		{
			graphSection = new FloatRange(Mathf.Max(0f, num - 30f), num);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref legendRect)).xMin + ((Rect)(ref legendRect)).width + 110f + 4f, ((Rect)(ref legendRect)).yMin, 110f, 40f), "Last100Days".Translate()))
		{
			graphSection = new FloatRange(Mathf.Max(0f, num - 100f), num);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref legendRect)).xMin + ((Rect)(ref legendRect)).width + 228f, ((Rect)(ref legendRect)).yMin, 110f, 40f), "Last300Days".Translate()))
		{
			graphSection = new FloatRange(Mathf.Max(0f, num - 300f), num);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref legendRect)).xMin + ((Rect)(ref legendRect)).width + 342f, ((Rect)(ref legendRect)).yMin, 110f, 40f), "AllDays".Translate()))
		{
			graphSection = new FloatRange(0f, num);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, 110f, 40f), "SelectGraph".Translate()))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			List<HistoryAutoRecorderGroup> list2 = Find.History.Groups();
			for (int j = 0; j < list2.Count; j++)
			{
				HistoryAutoRecorderGroup groupLocal = list2[j];
				if (!groupLocal.def.devModeOnly || Prefs.DevMode)
				{
					list.Add(new FloatMenuOption(groupLocal.def.LabelCap, delegate
					{
						historyAutoRecorderGroup = groupLocal;
					}));
				}
			}
			FloatMenu window = new FloatMenu(list, "SelectGraph".Translate());
			Find.WindowStack.Add(window);
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.HistoryTab, KnowledgeAmount.Total);
		}
		Widgets.EndGroup();
	}

	public override void Notify_ClickOutsideWindow()
	{
		quickSearchWidget.Unfocus();
	}
}
