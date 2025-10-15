using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public class DragBox
{
	public bool active;

	public Vector3 start;

	private const float DragBoxMinDiagonal = 0.5f;

	public float LeftX => Math.Min(start.x, UI.MouseMapPosition().x);

	public float RightX => Math.Max(start.x, UI.MouseMapPosition().x);

	public float BotZ => Math.Min(start.z, UI.MouseMapPosition().z);

	public float TopZ => Math.Max(start.z, UI.MouseMapPosition().z);

	public Rect ScreenRect
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = start.MapToUIPosition();
			Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
			if (mousePositionOnUIInverted.x < val.x)
			{
				float x = mousePositionOnUIInverted.x;
				mousePositionOnUIInverted.x = val.x;
				val.x = x;
			}
			if (mousePositionOnUIInverted.y < val.y)
			{
				float y = mousePositionOnUIInverted.y;
				mousePositionOnUIInverted.y = val.y;
				val.y = y;
			}
			Rect result = default(Rect);
			((Rect)(ref result)).xMin = val.x;
			((Rect)(ref result)).xMax = mousePositionOnUIInverted.x;
			((Rect)(ref result)).yMin = val.y;
			((Rect)(ref result)).yMax = mousePositionOnUIInverted.y;
			return result;
		}
	}

	public bool IsValid
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = start - UI.MouseMapPosition();
			return ((Vector3)(ref val)).magnitude > 0.5f;
		}
	}

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

	public bool Contains(Thing t)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (t is Pawn)
		{
			return Contains((t as Pawn).Drawer.DrawPos);
		}
		foreach (IntVec3 item in t.OccupiedRect())
		{
			if (Contains(item.ToVector3Shifted()))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (v.x + 0.5f > LeftX && v.x - 0.5f < RightX && v.z + 0.5f > BotZ && v.z - 0.5f < TopZ)
		{
			return true;
		}
		return false;
	}
}
