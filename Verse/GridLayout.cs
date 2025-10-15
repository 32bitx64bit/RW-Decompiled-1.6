using UnityEngine;

namespace Verse;

public class GridLayout
{
	public Rect container;

	private int cols;

	private float outerPadding;

	private float innerPadding;

	private float colStride;

	private float rowStride;

	private float colWidth;

	private float rowHeight;

	public GridLayout(Rect container, int cols = 1, int rows = 1, float outerPadding = 4f, float innerPadding = 4f)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		this.container = new Rect(container);
		this.cols = cols;
		this.innerPadding = innerPadding;
		this.outerPadding = outerPadding;
		float num = ((Rect)(ref container)).width - outerPadding * 2f - (float)(cols - 1) * innerPadding;
		float num2 = ((Rect)(ref container)).height - outerPadding * 2f - (float)(rows - 1) * innerPadding;
		colWidth = num / (float)cols;
		rowHeight = num2 / (float)rows;
		colStride = colWidth + innerPadding;
		rowStride = rowHeight + innerPadding;
	}

	public GridLayout(float colWidth, float rowHeight, int cols, int rows, float outerPadding = 4f, float innerPadding = 4f)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		this.colWidth = colWidth;
		this.rowHeight = rowHeight;
		this.cols = cols;
		this.innerPadding = innerPadding;
		this.outerPadding = outerPadding;
		colStride = colWidth + innerPadding;
		rowStride = rowHeight + innerPadding;
		container = new Rect(0f, 0f, outerPadding * 2f + colWidth * (float)cols + innerPadding * (float)cols - 1f, outerPadding * 2f + rowHeight * (float)rows + innerPadding * (float)rows - 1f);
	}

	public Rect GetCellRectByIndex(int index, int colspan = 1, int rowspan = 1)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		int col = index % cols;
		int row = index / cols;
		return GetCellRect(col, row, colspan, rowspan);
	}

	public Rect GetCellRect(int col, int row, int colspan = 1, int rowspan = 1)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(Mathf.Floor(((Rect)(ref container)).x + outerPadding + (float)col * colStride), Mathf.Floor(((Rect)(ref container)).y + outerPadding + (float)row * rowStride), Mathf.Ceil(colWidth) * (float)colspan + innerPadding * (float)(colspan - 1), Mathf.Ceil(rowHeight) * (float)rowspan + innerPadding * (float)(rowspan - 1));
	}
}
