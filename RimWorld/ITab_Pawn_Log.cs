using System;
using System.Collections.Generic;
using System.Text;
using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Pawn_Log : ITab
{
	public const float Width = 630f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowAllX = 60f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowAllWidth = 100f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowCombatX = 445f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowCombatWidth = 115f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowSocialX = 330f;

	[TweakValue("Interface", 0f, 1000f)]
	private static float ShowSocialWidth = 105f;

	[TweakValue("Interface", 0f, 20f)]
	private static float ToolbarHeight = 2f;

	[TweakValue("Interface", 0f, 100f)]
	private static float ButtonOffset = 60f;

	private const int MaxLogLines = 300;

	public bool showAll;

	public bool showCombat = true;

	public bool showSocial = true;

	public LogEntry logSeek;

	public ITab_Pawn_Log_Utility.LogDrawData data = new ITab_Pawn_Log_Utility.LogDrawData();

	public List<ITab_Pawn_Log_Utility.LogLineDisplayable> cachedLogDisplay;

	public int cachedLogDisplayLastTick = -1;

	public int cachedLogPlayLastTick = -1;

	private Pawn cachedLogForPawn;

	private Vector2 scrollPosition;

	private Pawn SelPawnForCombatInfo
	{
		get
		{
			if (SelPawn != null)
			{
				return SelPawn;
			}
			if (base.SelThing is Corpse corpse)
			{
				return corpse.InnerPawn;
			}
			throw new InvalidOperationException("Social tab on non-pawn non-corpse " + base.SelThing);
		}
	}

	public ITab_Pawn_Log()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(630f, 510f);
		labelKey = "TabLog";
	}

	protected override void FillTab()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		Pawn selPawnForCombatInfo = SelPawnForCombatInfo;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, size.x, size.y);
		GameFont font = Text.Font;
		Text.Font = GameFont.Small;
		Rect rect = new Rect(ShowAllX, ToolbarHeight, ShowAllWidth, 24f);
		bool flag = showAll;
		Widgets.CheckboxLabeled(rect, "ShowAll".Translate(), ref showAll);
		if (flag != showAll)
		{
			cachedLogDisplay = null;
		}
		Rect rect2 = new Rect(ShowCombatX, ToolbarHeight, ShowCombatWidth, 24f);
		bool flag2 = showCombat;
		Widgets.CheckboxLabeled(rect2, "ShowCombat".Translate(), ref showCombat);
		if (flag2 != showCombat)
		{
			cachedLogDisplay = null;
		}
		Rect rect3 = new Rect(ShowSocialX, ToolbarHeight, ShowSocialWidth, 24f);
		bool flag3 = showSocial;
		Widgets.CheckboxLabeled(rect3, "ShowSocial".Translate(), ref showSocial);
		if (flag3 != showSocial)
		{
			cachedLogDisplay = null;
		}
		Text.Font = font;
		if (cachedLogDisplay == null || cachedLogDisplayLastTick != selPawnForCombatInfo.records.LastBattleTick || cachedLogPlayLastTick != Find.PlayLog.LastTick || cachedLogForPawn != selPawnForCombatInfo)
		{
			cachedLogDisplay = ITab_Pawn_Log_Utility.GenerateLogLinesFor(selPawnForCombatInfo, showAll, showCombat, showSocial, 300);
			cachedLogDisplayLastTick = selPawnForCombatInfo.records.LastBattleTick;
			cachedLogPlayLastTick = Find.PlayLog.LastTick;
			cachedLogForPawn = selPawnForCombatInfo;
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).width - ButtonOffset, 0f, 18f, 24f);
		if (Widgets.ButtonImage(val2, TexButton.Copy))
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ITab_Pawn_Log_Utility.LogLineDisplayable item in cachedLogDisplay)
			{
				item.AppendTo(stringBuilder);
			}
			GUIUtility.systemCopyBuffer = stringBuilder.ToString();
		}
		TooltipHandler.TipRegionByKey(val2, "CopyLogTip");
		((Rect)(ref val)).yMin = 24f;
		val = val.ContractedBy(10f);
		float num = ((Rect)(ref val)).width - 16f - 10f;
		float num2 = 0f;
		foreach (ITab_Pawn_Log_Utility.LogLineDisplayable item2 in cachedLogDisplay)
		{
			if (item2.Matches(logSeek))
			{
				scrollPosition.y = num2 - (item2.GetHeight(num) + ((Rect)(ref val)).height) / 2f;
			}
			num2 += item2.GetHeight(num);
		}
		logSeek = null;
		if (num2 > 0f)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, num2);
			data.StartNewDraw();
			Rect val4 = val3;
			((Rect)(ref val4)).yMin = ((Rect)(ref val4)).yMin + scrollPosition.y;
			((Rect)(ref val4)).height = ((Rect)(ref val)).height;
			Widgets.BeginScrollView(val, ref scrollPosition, val3);
			float num3 = 0f;
			foreach (ITab_Pawn_Log_Utility.LogLineDisplayable item3 in cachedLogDisplay)
			{
				float height = item3.GetHeight(num);
				if (((Rect)(ref val4)).Overlaps(new Rect(0f, num3, num, height)))
				{
					item3.Draw(num3, num, data);
				}
				else
				{
					data.alternatingBackground = !data.alternatingBackground;
				}
				num3 += height;
			}
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = (TextAnchor)4;
			GUI.color = Color.grey;
			Widgets.Label(new Rect(0f, 0f, size.x, size.y), "(" + "NoRecentEntries".Translate() + ")");
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
	}

	public void SeekTo(LogEntry entry)
	{
		logSeek = entry;
	}

	public void Highlight(LogEntry entry)
	{
		data.highlightEntry = entry;
		data.highlightIntensity = 1f;
	}

	public override void Notify_ClearingAllMapsMemory()
	{
		base.Notify_ClearingAllMapsMemory();
		cachedLogForPawn = null;
	}
}
