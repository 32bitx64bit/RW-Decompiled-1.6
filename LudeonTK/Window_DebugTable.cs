using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LudeonTK;

public class Window_DebugTable : Window_Dev
{
	private enum SortMode
	{
		Off,
		Ascending,
		Descending
	}

	private string[,] tableRaw;

	private Vector2 scrollPosition = Vector2.zero;

	private string[,] tableSorted;

	private List<float> colWidths = new List<float>();

	private List<float> rowHeights = new List<float>();

	private int sortColumn = -1;

	private SortMode sortMode;

	private bool[] colVisible;

	private bool[] rowsVisible;

	private float visibleWidth;

	private float visibleHeight;

	private bool focusFilter;

	private readonly QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private const float ColExtraWidth = 4f;

	private const float RowExtraHeight = 2f;

	private const float HiddenColumnWidth = 10f;

	private const float MouseoverOffset = 2f;

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	public Window_DebugTable(string[,] tables)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		tableRaw = tables;
		colVisible = new bool[tableRaw.GetLength(0)];
		rowsVisible = new bool[tableRaw.GetLength(1)];
		for (int i = 0; i < colVisible.Length; i++)
		{
			colVisible[i] = true;
		}
		for (int j = 0; j < rowsVisible.Length; j++)
		{
			rowsVisible[j] = true;
		}
		doCloseButton = true;
		doCloseX = true;
		focusFilter = true;
		Text.Font = GameFont.Tiny;
		BuildTableSorted();
		visibleWidth = colWidths.Sum();
		visibleHeight = rowHeights.Sum() - rowHeights[0];
	}

	private void BuildTableSorted()
	{
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		if (sortMode == SortMode.Off)
		{
			tableSorted = tableRaw;
		}
		else
		{
			List<List<string>> list = new List<List<string>>();
			for (int i = 1; i < tableRaw.GetLength(1); i++)
			{
				list.Add(new List<string>());
				for (int j = 0; j < tableRaw.GetLength(0); j++)
				{
					list[i - 1].Add(tableRaw[j, i]);
				}
			}
			NumericStringComparer comparer = new NumericStringComparer();
			switch (sortMode)
			{
			case SortMode.Ascending:
				list = list.OrderBy((List<string> x) => x[sortColumn], comparer).ToList();
				break;
			case SortMode.Descending:
				list = list.OrderByDescending((List<string> x) => x[sortColumn], comparer).ToList();
				break;
			case SortMode.Off:
				throw new Exception();
			}
			tableSorted = new string[tableRaw.GetLength(0), tableRaw.GetLength(1)];
			for (int k = 0; k < tableRaw.GetLength(1); k++)
			{
				for (int l = 0; l < tableRaw.GetLength(0); l++)
				{
					if (k == 0)
					{
						tableSorted[l, k] = tableRaw[l, k];
					}
					else
					{
						tableSorted[l, k] = list[k - 1][l];
					}
				}
			}
		}
		colWidths.Clear();
		for (int m = 0; m < tableRaw.GetLength(0); m++)
		{
			float item;
			if (colVisible[m])
			{
				float num = 0f;
				for (int n = 0; n < tableRaw.GetLength(1); n++)
				{
					float x2 = Text.CalcSize(tableRaw[m, n]).x;
					if (x2 > num)
					{
						num = x2;
					}
				}
				item = num + 4f;
			}
			else
			{
				item = 10f;
			}
			colWidths.Add(item);
		}
		rowHeights.Clear();
		for (int num2 = 0; num2 < tableSorted.GetLength(1); num2++)
		{
			float num3 = 0f;
			for (int num4 = 0; num4 < tableSorted.GetLength(0); num4++)
			{
				float y = Text.CalcSize(tableSorted[num4, num2]).y;
				if (y > num3)
				{
					num3 = y;
				}
			}
			rowHeights.Add(num3 + 2f);
		}
	}

	private void RefreshSearchFilter()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		visibleHeight = 0f;
		for (int i = 1; i < tableSorted.GetLength(1); i++)
		{
			bool flag = false;
			for (int j = 0; j < tableSorted.GetLength(0); j++)
			{
				if (quickSearchWidget.filter.Matches(tableSorted[j, i]))
				{
					flag = true;
					break;
				}
			}
			if (rowsVisible[i] != flag)
			{
				scrollPosition = Vector2.zero;
			}
			if (flag)
			{
				visibleHeight += rowHeights[i];
			}
			rowsVisible[i] = flag;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		Rect outRect = inRect;
		float num2 = (((Rect)(ref outRect)).y = rowHeights[0] + 2f);
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - (40f + num2);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, colWidths.Sum(), visibleHeight);
		float num3 = 0f;
		Rect rect = default(Rect);
		for (int i = 0; i < tableSorted.GetLength(0); i++)
		{
			((Rect)(ref rect))._002Ector(num3 - scrollPosition.x, 0f, colWidths[i], rowHeights[0]);
			num3 += colWidths[i];
			MouseoverSounds.DoRegion(rect);
			if (!Mouse.IsOver(rect))
			{
				DevGUI.Label(rect, tableSorted[i, 0]);
				continue;
			}
			DevGUI.DrawHighlight(rect);
			if ((int)Event.current.type == 0)
			{
				if (Event.current.button == 0)
				{
					if (i != sortColumn)
					{
						sortMode = SortMode.Off;
					}
					switch (sortMode)
					{
					case SortMode.Off:
						sortMode = SortMode.Descending;
						sortColumn = i;
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						break;
					case SortMode.Descending:
						sortMode = SortMode.Ascending;
						sortColumn = i;
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						break;
					case SortMode.Ascending:
						sortMode = SortMode.Off;
						sortColumn = -1;
						SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
						break;
					}
					BuildTableSorted();
				}
				else if (Event.current.button == 1)
				{
					colVisible[i] = !colVisible[i];
					SoundDefOf.Crunch.PlayOneShotOnCamera();
					BuildTableSorted();
				}
				Event.current.Use();
			}
			DevGUI.Label(rect, tableSorted[i, 0]);
		}
		Rect val = default(Rect);
		((Rect)(ref val)).x = 0f;
		((Rect)(ref val)).width = 300f;
		((Rect)(ref val)).y = ((Rect)(ref inRect)).yMax - 24f - 4f;
		((Rect)(ref val)).height = 24f;
		Rect rect2 = val;
		quickSearchWidget.OnGUI(rect2, RefreshSearchFilter);
		if (focusFilter)
		{
			quickSearchWidget.Focus();
			focusFilter = false;
		}
		DevGUI.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num4 = 0f;
		Rect rect3 = default(Rect);
		for (int j = 1; j < tableSorted.GetLength(1); j++)
		{
			if (!rowsVisible[j])
			{
				continue;
			}
			if (num4 + rowHeights[j] < ((Rect)(ref viewRect)).yMin + scrollPosition.y || num4 > ((Rect)(ref viewRect)).yMax + scrollPosition.y)
			{
				num4 += rowHeights[j];
				continue;
			}
			float num5 = 0f;
			bool flag = Mouse.IsOver(new Rect(num5, num4, visibleWidth, rowHeights[j]));
			for (int k = 0; k < tableSorted.GetLength(0); k++)
			{
				((Rect)(ref rect3))._002Ector(num5, num4, colWidths[k], rowHeights[j]);
				if (flag)
				{
					DevGUI.DrawHighlight(rect3);
				}
				else if (k % 2 == 0)
				{
					DevGUI.DrawLightHighlight(rect3);
				}
				if (colVisible[k])
				{
					DevGUI.Label(rect3, tableSorted[k, j]);
				}
				num5 += colWidths[k];
			}
			num4 += rowHeights[j];
		}
		DevGUI.EndScrollView();
		Text.Font = GameFont.Small;
		if (DevGUI.ButtonText(new Rect(((Rect)(ref inRect)).xMax - 120f, ((Rect)(ref inRect)).yMax - 30f, 120f, 30f), "Copy CSV"))
		{
			CopyCSVToClipboard();
			Messages.Message("Copied table data to clipboard in CSV format.", MessageTypeDefOf.PositiveEvent);
		}
	}

	private void MouseOverHeaderRowCell(Rect cell, int col)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		MouseoverSounds.DoRegion(cell);
		if (!Mouse.IsOver(cell) || (int)Event.current.type != 0)
		{
			return;
		}
		if (Event.current.button == 0)
		{
			if (col != sortColumn)
			{
				sortMode = SortMode.Off;
			}
			switch (sortMode)
			{
			case SortMode.Off:
				sortMode = SortMode.Descending;
				sortColumn = col;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				break;
			case SortMode.Descending:
				sortMode = SortMode.Ascending;
				sortColumn = col;
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				break;
			case SortMode.Ascending:
				sortMode = SortMode.Off;
				sortColumn = -1;
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				break;
			}
			BuildTableSorted();
		}
		else if (Event.current.button == 1)
		{
			colVisible[col] = !colVisible[col];
			SoundDefOf.Crunch.PlayOneShotOnCamera();
			BuildTableSorted();
		}
		Event.current.Use();
	}

	private static void MouseCenterButtonDrag(ref Vector2 scroll)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 3 && Event.current.button == 2)
		{
			Vector2 currentEventDelta = UnityGUIBugsFixer.CurrentEventDelta;
			Event.current.Use();
			if (currentEventDelta != Vector2.zero)
			{
				scroll += -1f * currentEventDelta;
			}
		}
	}

	private static bool IsMouseOverRow(Rect cell)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref cell)).xMin = ((Rect)(ref cell)).xMin - 9999999f;
		((Rect)(ref cell)).xMax = ((Rect)(ref cell)).xMax + 9999999f;
		return Mouse.IsOver(cell);
	}

	private static bool IsMouseOverCol(Rect cell)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref cell)).yMin = ((Rect)(ref cell)).yMin - 9999999f;
		((Rect)(ref cell)).yMax = ((Rect)(ref cell)).yMax + 9999999f;
		return Mouse.IsOver(cell);
	}

	private static void DrawOpaqueLabel(Rect r, string label, bool highlighted = true, bool selectedRow = false, bool selectedCol = false, bool offsetLabel = false)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (offsetLabel)
		{
			((Rect)(ref r)).height = ((Rect)(ref r)).height + 2f;
		}
		Color color = GUI.color;
		GUI.color = DevGUI.WindowBGFillColor;
		GUI.DrawTexture(r, (Texture)(object)BaseContent.WhiteTex);
		GUI.color = color;
		if (selectedRow)
		{
			DevGUI.DrawHighlightSelected(r);
		}
		if (selectedCol)
		{
			DevGUI.DrawHighlightSelected(r);
		}
		if (highlighted)
		{
			DevGUI.DrawHighlight(r);
		}
		else
		{
			DevGUI.DrawLightHighlight(r);
		}
		if (offsetLabel)
		{
			((Rect)(ref r)).y = ((Rect)(ref r)).y + 2f;
		}
		DevGUI.Label(r, label);
	}

	private void CopyCSVToClipboard()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < tableSorted.GetLength(1); i++)
		{
			for (int j = 0; j < tableSorted.GetLength(0); j++)
			{
				if (j != 0)
				{
					stringBuilder.Append(",");
				}
				string text = tableSorted[j, i] ?? "";
				stringBuilder.Append("\"" + text.Replace("\n", " ") + "\"");
			}
			stringBuilder.Append("\n");
		}
		GUIUtility.systemCopyBuffer = stringBuilder.ToString();
	}
}
