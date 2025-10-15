using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public static class EnvironmentStatsDrawer
{
	private const float StatLabelColumnWidth = 110f;

	private const float StatGutterColumnWidth = 10f;

	private const float ScoreColumnWidth = 50f;

	private const float ScoreStageLabelColumnWidth = 160f;

	private static readonly Color RelatedStatColor = new Color(0.85f, 0.85f, 0.85f);

	private static readonly Color UnrelatedStatColor = Color.gray;

	private const float DistFromMouse = 26f;

	public const float WindowPadding = 12f;

	private const float LineHeight = 23f;

	private const float FootnoteHeight = 23f;

	private const float TitleHeight = 30f;

	private const float SpaceBetweenLines = 2f;

	private const float SpaceBetweenColumns = 35f;

	private static int DisplayedRoomStatsCount
	{
		get
		{
			int num = 0;
			List<RoomStatDef> allDefsListForReading = DefDatabase<RoomStatDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				if (!allDefsListForReading[i].isHidden || DebugViewSettings.showAllRoomStats)
				{
					num++;
				}
			}
			return num;
		}
	}

	private static bool ShouldShowWindowNow()
	{
		if (!ShouldShowRoomStats() && !ShouldShowBeauty())
		{
			return false;
		}
		if (Mouse.IsInputBlockedNow)
		{
			return false;
		}
		return true;
	}

	private static bool ShouldShowRoomStats()
	{
		if (!Find.PlaySettings.showRoomStats)
		{
			return false;
		}
		if (Find.CurrentMap == null)
		{
			return false;
		}
		if (!UI.MouseCell().InBounds(Find.CurrentMap) || UI.MouseCell().Fogged(Find.CurrentMap))
		{
			return false;
		}
		Room room = UI.MouseCell().GetRoom(Find.CurrentMap);
		if (room != null)
		{
			return room.Role != RoomRoleDefOf.None;
		}
		return false;
	}

	private static bool ShouldShowBeauty()
	{
		if (!Find.PlaySettings.showBeauty)
		{
			return false;
		}
		if (!UI.MouseCell().InBounds(Find.CurrentMap) || UI.MouseCell().Fogged(Find.CurrentMap))
		{
			return false;
		}
		return UI.MouseCell().GetRoom(Find.CurrentMap) != null;
	}

	public static void EnvironmentStatsOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 7 && ShouldShowWindowNow())
		{
			DrawInfoWindow();
		}
	}

	private static void DrawInfoWindow()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect windowRect = GetWindowRect(ShouldShowBeauty(), ShouldShowRoomStats());
		Find.WindowStack.ImmediateWindow(74975, windowRect, WindowLayer.GameUI, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			FillWindow(windowRect);
		});
	}

	public static Rect GetWindowRect(bool shouldShowBeauty, bool shouldShowRoomStats)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		Rect result = default(Rect);
		((Rect)(ref result))._002Ector(Event.current.mousePosition.x, Event.current.mousePosition.y, 424f, 24f);
		int num = 0;
		if (shouldShowBeauty)
		{
			num++;
			((Rect)(ref result)).height = ((Rect)(ref result)).height + 25f;
		}
		if (shouldShowRoomStats)
		{
			num++;
			((Rect)(ref result)).height = ((Rect)(ref result)).height + 23f;
			((Rect)(ref result)).height = ((Rect)(ref result)).height + ((float)DisplayedRoomStatsCount * 25f + 23f);
		}
		((Rect)(ref result)).height = ((Rect)(ref result)).height + 13f * (float)(num - 1);
		((Rect)(ref result)).x = ((Rect)(ref result)).x + 26f;
		((Rect)(ref result)).y = ((Rect)(ref result)).y + 26f;
		if (((Rect)(ref result)).xMax > (float)UI.screenWidth)
		{
			((Rect)(ref result)).x = ((Rect)(ref result)).x - (((Rect)(ref result)).width + 52f);
		}
		if (((Rect)(ref result)).yMax > (float)UI.screenHeight)
		{
			((Rect)(ref result)).y = ((Rect)(ref result)).y - (((Rect)(ref result)).height + 52f);
		}
		return result;
	}

	private static void FillWindow(Rect windowRect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		float curY = 12f;
		int dividingLinesSeen = 0;
		if (ShouldShowBeauty())
		{
			DrawDividingLineIfNecessary();
			float beauty = BeautyUtility.AverageBeautyPerceptible(UI.MouseCell(), Find.CurrentMap);
			Rect rect = new Rect(22f, curY, ((Rect)(ref windowRect)).width - 24f - 10f, 100f);
			GUI.color = BeautyDrawer.BeautyColor(beauty, 40f);
			Widgets.Label(rect, "BeautyHere".Translate() + ": " + beauty.ToString("F1"));
			curY += 25f;
		}
		if (ShouldShowRoomStats())
		{
			DrawDividingLineIfNecessary();
			DoRoomInfo(UI.MouseCell().GetRoom(Find.CurrentMap), ref curY, windowRect);
		}
		GUI.color = Color.white;
		void DrawDividingLineIfNecessary()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			dividingLinesSeen++;
			if (dividingLinesSeen > 1)
			{
				curY += 5f;
				GUI.color = new Color(1f, 1f, 1f, 0.4f);
				Widgets.DrawLineHorizontal(12f, curY, ((Rect)(ref windowRect)).width - 24f);
				GUI.color = Color.white;
				curY += 8f;
			}
		}
	}

	public static void DrawRoomOverlays()
	{
		if (Find.PlaySettings.showBeauty && UI.MouseCell().InBounds(Find.CurrentMap))
		{
			GenUI.RenderMouseoverBracket();
		}
		if (ShouldShowWindowNow() && ShouldShowRoomStats())
		{
			Room room = UI.MouseCell().GetRoom(Find.CurrentMap);
			if (room != null && room.Role != RoomRoleDefOf.None)
			{
				room.DrawFieldEdges();
			}
		}
	}

	public static void DoRoomInfo(Room room, ref float curY, Rect windowRect)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(12f, curY, ((Rect)(ref windowRect)).width - 24f, 100f);
		GUI.color = Color.white;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref val)).x + 10f, curY, ((Rect)(ref val)).width - 10f, ((Rect)(ref val)).height), room.GetRoomRoleLabel().CapitalizeFirst());
		curY += 30f;
		Text.Font = GameFont.Small;
		Text.WordWrap = false;
		int num = 0;
		bool flag = false;
		Rect rect = default(Rect);
		Rect rect2 = default(Rect);
		Rect rect3 = default(Rect);
		Rect rect4 = default(Rect);
		for (int i = 0; i < DefDatabase<RoomStatDef>.AllDefsListForReading.Count; i++)
		{
			RoomStatDef roomStatDef = DefDatabase<RoomStatDef>.AllDefsListForReading[i];
			if (!roomStatDef.isHidden || DebugViewSettings.showAllRoomStats)
			{
				float stat = room.GetStat(roomStatDef);
				RoomStatScoreStage scoreStage = roomStatDef.GetScoreStage(stat);
				GUI.color = Color.white;
				((Rect)(ref rect))._002Ector(((Rect)(ref val)).x, curY, ((Rect)(ref val)).width, 23f);
				if (num % 2 == 1)
				{
					Widgets.DrawLightHighlight(rect);
				}
				((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, curY, 10f, 23f);
				if (room.Role.IsStatRelated(roomStatDef))
				{
					flag = true;
					Widgets.Label(rect2, "*");
					GUI.color = RelatedStatColor;
				}
				else
				{
					GUI.color = UnrelatedStatColor;
				}
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).xMax, curY, 110f, 23f);
				Widgets.Label(rect3, roomStatDef.LabelCap);
				((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).xMax + 35f, curY, 50f, 23f);
				string label = roomStatDef.ScoreToString(stat);
				Widgets.Label(rect4, label);
				Widgets.Label(new Rect(((Rect)(ref rect4)).xMax + 35f, curY, 160f, 23f), (scoreStage == null) ? "" : scoreStage.label.CapitalizeFirst());
				curY += 25f;
				num++;
			}
		}
		if (flag)
		{
			GUI.color = Color.grey;
			Text.Font = GameFont.Tiny;
			Widgets.Label(new Rect(((Rect)(ref val)).x, curY, ((Rect)(ref val)).width, 23f), "* " + "StatRelatesToCurrentRoom".Translate());
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
		}
		Text.WordWrap = true;
	}
}
