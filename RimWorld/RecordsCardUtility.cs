using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class RecordsCardUtility
{
	private static Vector2 scrollPosition;

	private static float listHeight;

	private const float RecordsLeftPadding = 8f;

	private static List<RecordDef> allRecordsSorted;

	private static List<RecordDef> AllRecordsSorted
	{
		get
		{
			if (allRecordsSorted == null)
			{
				allRecordsSorted = DefDatabase<RecordDef>.AllDefsListForReading.OrderBy((RecordDef x) => x.displayOrder).ToList();
			}
			return allRecordsSorted;
		}
	}

	public static void DrawRecordsCard(Rect rect, Pawn pawn)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, listHeight);
		Widgets.BeginScrollView(rect, ref scrollPosition, val);
		Rect leftRect = val;
		((Rect)(ref leftRect)).width = ((Rect)(ref leftRect)).width * 0.5f;
		Rect rightRect = val;
		((Rect)(ref rightRect)).x = ((Rect)(ref leftRect)).xMax;
		((Rect)(ref rightRect)).width = ((Rect)(ref val)).width - ((Rect)(ref rightRect)).x;
		((Rect)(ref leftRect)).xMax = ((Rect)(ref leftRect)).xMax - 6f;
		((Rect)(ref rightRect)).xMin = ((Rect)(ref rightRect)).xMin + 6f;
		float num = DrawTimeRecords(leftRect, pawn);
		float num2 = DrawMiscRecords(rightRect, pawn);
		listHeight = Mathf.Max(num, num2) + 100f;
		Widgets.EndScrollView();
	}

	private static float DrawTimeRecords(Rect leftRect, Pawn pawn)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<RecordDef> list = AllRecordsSorted;
		float curY = 0f;
		Widgets.BeginGroup(leftRect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref leftRect)).width, "TimeRecordsCategory".Translate());
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].type == RecordType.Time)
			{
				curY += DrawRecord(8f, curY, ((Rect)(ref leftRect)).width - 8f, list[i], pawn);
			}
		}
		Widgets.EndGroup();
		return curY;
	}

	private static float DrawMiscRecords(Rect rightRect, Pawn pawn)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<RecordDef> list = AllRecordsSorted;
		float curY = 0f;
		Widgets.BeginGroup(rightRect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref rightRect)).width, "MiscRecordsCategory".Translate());
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].type == RecordType.Int || list[i].type == RecordType.Float)
			{
				curY += DrawRecord(8f, curY, ((Rect)(ref rightRect)).width - 8f, list[i], pawn);
			}
		}
		Widgets.EndGroup();
		return curY;
	}

	private static float DrawRecord(float x, float y, float width, RecordDef record, Pawn pawn)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		float num = width * 0.45f;
		string text = ((record.type != 0) ? pawn.records.GetValue(record).ToString("0.##") : pawn.records.GetAsInt(record).ToStringTicksToPeriod());
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(8f, y, width, Text.CalcHeight(text, num));
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		Rect rect = val;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - num;
		Widgets.Label(rect, record.LabelCap);
		Rect rect2 = val;
		((Rect)(ref rect2)).x = ((Rect)(ref rect)).xMax;
		((Rect)(ref rect2)).width = num;
		Widgets.Label(rect2, text);
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, new TipSignal(() => record.description, record.GetHashCode()));
		}
		return ((Rect)(ref val)).height;
	}
}
