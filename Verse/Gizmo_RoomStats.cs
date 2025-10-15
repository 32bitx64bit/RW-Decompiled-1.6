using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Gizmo_RoomStats : Gizmo
{
	private Building building;

	private static readonly Color RoomStatsColor = new Color(0.75f, 0.75f, 0.75f);

	private Room Room => GetRoomToShowStatsFor(building);

	public Gizmo_RoomStats(Building building)
	{
		this.building = building;
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return Mathf.Min(300f, maxWidth);
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		Room room = Room;
		if (room == null)
		{
			return new GizmoResult(GizmoState.Clear);
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Widgets.DrawWindowBackground(val);
		Text.WordWrap = false;
		Widgets.BeginGroup(val);
		Rect val2 = val.AtZero().ContractedBy(10f);
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y - 2f, ((Rect)(ref val2)).width, 100f);
		float stat = room.GetStat(RoomStatDefOf.Impressiveness);
		RoomStatScoreStage scoreStage = RoomStatDefOf.Impressiveness.GetScoreStage(stat);
		string str = room.Role.PostProcessedLabelCap(room) + ", " + scoreStage.label + " (" + RoomStatDefOf.Impressiveness.ScoreToString(stat) + ")";
		Widgets.Label(rect, str.Truncate(((Rect)(ref rect)).width));
		float num = ((Rect)(ref val2)).y + Text.LineHeight + Text.SpaceBetweenLines + 7f;
		GUI.color = RoomStatsColor;
		Text.Font = GameFont.Tiny;
		List<RoomStatDef> allDefsListForReading = DefDatabase<RoomStatDef>.AllDefsListForReading;
		int num2 = 0;
		Rect rect2 = default(Rect);
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			if (!allDefsListForReading[i].isHidden && allDefsListForReading[i] != RoomStatDefOf.Impressiveness)
			{
				float stat2 = room.GetStat(allDefsListForReading[i]);
				RoomStatScoreStage scoreStage2 = allDefsListForReading[i].GetScoreStage(stat2);
				if (num2 % 2 == 0)
				{
					((Rect)(ref rect2))._002Ector(((Rect)(ref val2)).x, num, ((Rect)(ref val2)).width / 2f, 100f);
				}
				else
				{
					((Rect)(ref rect2))._002Ector(((Rect)(ref val2)).x + ((Rect)(ref val2)).width / 2f, num, ((Rect)(ref val2)).width / 2f, 100f);
				}
				string str2 = scoreStage2.label.CapitalizeFirst() + " (" + allDefsListForReading[i].ScoreToString(stat2) + ")";
				Widgets.Label(rect2, str2.Truncate(((Rect)(ref rect2)).width));
				if (num2 % 2 == 1)
				{
					num += Text.LineHeight + Text.SpaceBetweenLines;
				}
				num2++;
			}
		}
		GUI.color = Color.white;
		Text.Font = GameFont.Small;
		Widgets.EndGroup();
		Text.WordWrap = true;
		GenUI.AbsorbClicksInRect(val);
		if (Mouse.IsOver(val))
		{
			Rect windowRect = EnvironmentStatsDrawer.GetWindowRect(shouldShowBeauty: false, shouldShowRoomStats: true);
			Find.WindowStack.ImmediateWindow(74975, windowRect, WindowLayer.Super, delegate
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				float curY = 12f;
				EnvironmentStatsDrawer.DoRoomInfo(room, ref curY, windowRect);
			});
			return new GizmoResult(GizmoState.Mouseover);
		}
		return new GizmoResult(GizmoState.Clear);
	}

	public override void GizmoUpdateOnMouseover()
	{
		base.GizmoUpdateOnMouseover();
		Room?.DrawFieldEdges();
	}

	public static Room GetRoomToShowStatsFor(Building building)
	{
		if (!building.Spawned || building.Fogged())
		{
			return null;
		}
		Room room = null;
		if (building.def.passability != Traversability.Impassable)
		{
			room = building.GetRoom();
		}
		else if (building.def.hasInteractionCell)
		{
			room = building.InteractionCell.GetRoom(building.Map);
		}
		else
		{
			CellRect cellRect = building.OccupiedRect().ExpandedBy(1);
			foreach (IntVec3 item in cellRect)
			{
				if (cellRect.IsOnEdge(item))
				{
					room = item.GetRoom(building.Map);
					if (IsValid(room))
					{
						break;
					}
				}
			}
		}
		if (!IsValid(room))
		{
			return null;
		}
		return room;
		static bool IsValid(Room r)
		{
			if (r != null && !r.Fogged)
			{
				return r.Role != RoomRoleDefOf.None;
			}
			return false;
		}
	}
}
