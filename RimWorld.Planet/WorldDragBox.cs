using System;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldDragBox
{
	public bool active;

	public Vector2 start;

	private const float DragBoxMinDiagonal = 7f;

	public float LeftX => Math.Min(start.x, UI.MousePositionOnUIInverted.x);

	public float RightX => Math.Max(start.x, UI.MousePositionOnUIInverted.x);

	public float BotZ => Math.Min(start.y, UI.MousePositionOnUIInverted.y);

	public float TopZ => Math.Max(start.y, UI.MousePositionOnUIInverted.y);

	public Rect ScreenRect => new Rect(LeftX, BotZ, RightX - LeftX, TopZ - BotZ);

	public float Diagonal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = start - new Vector2(UI.MousePositionOnUIInverted.x, UI.MousePositionOnUIInverted.y);
			return ((Vector2)(ref val)).magnitude;
		}
	}

	public bool IsValid => Diagonal > 7f;

	public bool IsValidAndActive
	{
		get
		{
			if (active)
			{
				return IsValid;
			}
			return false;
		}
	}

	public void DragBoxOnGUI()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (IsValidAndActive)
		{
			Widgets.DrawBox(ScreenRect, 2);
		}
	}

	public bool Contains(WorldObject o)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Contains(o.ScreenPos());
	}

	public bool Contains(Vector2 screenPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (screenPoint.x + 0.5f > LeftX && screenPoint.x - 0.5f < RightX && screenPoint.y + 0.5f > BotZ && screenPoint.y - 0.5f < TopZ)
		{
			return true;
		}
		return false;
	}
}
