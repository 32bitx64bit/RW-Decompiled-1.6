using System;
using UnityEngine;

namespace Verse;

public struct RectDivider
{
	private Rect currentRect;

	private Vector2 margin;

	private int contextHash;

	private int ErrorHash => contextHash ^ 0x3BFEED84;

	public Rect Rect => currentRect;

	public RectDivider(Rect parent, int contextHash, Vector2? margin = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		currentRect = parent;
		this.margin = (Vector2)(((_003F?)margin) ?? new Vector2(17f, 4f));
		this.contextHash = contextHash;
	}

	public static implicit operator Rect(RectDivider rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return rect.currentRect;
	}

	public RectDivider NewRow(float height, VerticalJustification justification = VerticalJustification.Top, float? marginOverride = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		switch (justification)
		{
		case VerticalJustification.Top:
		{
			if (!currentRect.SplitHorizontallyWithMargin(out var top2, out var bottom2, out var overflow2, marginOverride ?? margin.y, height))
			{
				Log.ErrorOnce($"Rect height was too small by {overflow2} for the requested row height.", ErrorHash);
			}
			currentRect = bottom2;
			return new RectDivider(top2, contextHash, margin);
		}
		case VerticalJustification.Bottom:
		{
			Rect rect = currentRect;
			float compressibleMargin = marginOverride ?? margin.y;
			float? bottomHeight = height;
			if (!rect.SplitHorizontallyWithMargin(out var top, out var bottom, out var overflow, compressibleMargin, null, bottomHeight))
			{
				Log.ErrorOnce($"Rect height was too small by {overflow} for the requested rows height.", ErrorHash);
			}
			currentRect = top;
			return new RectDivider(bottom, contextHash, margin);
		}
		default:
			throw new InvalidOperationException();
		}
	}

	public RectDivider NewCol(float width, HorizontalJustification justification = HorizontalJustification.Left, float? marginOverride = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		switch (justification)
		{
		case HorizontalJustification.Left:
		{
			if (!currentRect.SplitVerticallyWithMargin(out var left2, out var right2, out var overflow2, marginOverride ?? margin.x, width))
			{
				Log.ErrorOnce($"Rect width was too small by {overflow2} for the requested column width.", ErrorHash);
			}
			currentRect = right2;
			return new RectDivider(left2, contextHash, margin);
		}
		case HorizontalJustification.Right:
		{
			Rect rect = currentRect;
			float compressibleMargin = marginOverride ?? margin.x;
			float? rightWidth = width;
			if (!rect.SplitVerticallyWithMargin(out var left, out var right, out var overflow, compressibleMargin, null, rightWidth))
			{
				Log.ErrorOnce($"Rect width was too small by {overflow} for the requested column width.", ErrorHash);
			}
			currentRect = left;
			return new RectDivider(right, contextHash, margin);
		}
		default:
			throw new InvalidOperationException();
		}
	}

	public RectDivider CreateViewRect(int count, float rowHeight)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (count > 0)
		{
			num = (float)count * rowHeight + margin.y * (float)(count - 1);
		}
		Rect rect = Rect;
		float num2 = ((Rect)(ref rect)).width;
		float num3 = num;
		rect = Rect;
		if (num3 > ((Rect)(ref rect)).height)
		{
			num2 -= 16f;
		}
		return new RectDivider(new Rect(0f, 0f, num2, num), contextHash);
	}
}
