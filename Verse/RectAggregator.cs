using System;
using UnityEngine;

namespace Verse;

public struct RectAggregator
{
	private Rect currentRect;

	private Vector2 margin;

	private int contextHash;

	public Rect Rect => currentRect;

	public RectAggregator(Rect parent, int contextHash, Vector2? margin = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		currentRect = parent;
		this.margin = (Vector2)(((_003F?)margin) ?? new Vector2(10f, 4f));
		this.contextHash = contextHash;
	}

	public static implicit operator Rect(RectAggregator rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return rect.currentRect;
	}

	public RectDivider NewRow(float height, VerticalJustification addAt = VerticalJustification.Bottom)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Rect parent = default(Rect);
		switch (addAt)
		{
		case VerticalJustification.Top:
		{
			ref Rect reference2 = ref currentRect;
			((Rect)(ref reference2)).yMin = ((Rect)(ref reference2)).yMin - (margin.y + height);
			((Rect)(ref parent))._002Ector(((Rect)(ref currentRect)).x, ((Rect)(ref currentRect)).y, ((Rect)(ref currentRect)).width, height);
			break;
		}
		case VerticalJustification.Bottom:
		{
			ref Rect reference = ref currentRect;
			((Rect)(ref reference)).yMax = ((Rect)(ref reference)).yMax + (margin.y + height);
			((Rect)(ref parent))._002Ector(((Rect)(ref currentRect)).x, ((Rect)(ref currentRect)).yMax - height, ((Rect)(ref currentRect)).width, height);
			break;
		}
		default:
			throw new InvalidOperationException();
		}
		return new RectDivider(parent, contextHash, margin);
	}

	public RectDivider NewCol(float width, HorizontalJustification addAt = HorizontalJustification.Right)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Rect parent = default(Rect);
		switch (addAt)
		{
		case HorizontalJustification.Left:
		{
			ref Rect reference2 = ref currentRect;
			((Rect)(ref reference2)).xMin = ((Rect)(ref reference2)).xMin - (margin.x + width);
			((Rect)(ref parent))._002Ector(((Rect)(ref currentRect)).x, ((Rect)(ref currentRect)).y, width, ((Rect)(ref currentRect)).height);
			break;
		}
		case HorizontalJustification.Right:
		{
			ref Rect reference = ref currentRect;
			((Rect)(ref reference)).xMax = ((Rect)(ref reference)).xMax + (margin.x + width);
			((Rect)(ref parent))._002Ector(((Rect)(ref currentRect)).xMax - width, ((Rect)(ref currentRect)).y, width, ((Rect)(ref currentRect)).height);
			break;
		}
		default:
			throw new InvalidOperationException();
		}
		return new RectDivider(parent, contextHash, margin);
	}
}
