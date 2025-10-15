using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class PawnColumnWorker_Icon : PawnColumnWorker
{
	protected virtual int Width => 26;

	protected virtual int Padding => 2;

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Texture2D iconFor = GetIconFor(pawn);
		if (!((Object)(object)iconFor != (Object)null))
		{
			return;
		}
		Vector2 iconSize = GetIconSize(pawn);
		int num = (int)((((Rect)(ref rect)).width - iconSize.x) / 2f);
		int num2 = Mathf.Max((int)((30f - iconSize.y) / 2f), 0);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + (float)num, ((Rect)(ref rect)).y + (float)num2, iconSize.x, iconSize.y);
		GUI.color = GetIconColor(pawn);
		GUI.DrawTexture(val.ContractedBy(Padding), (Texture)(object)iconFor);
		GUI.color = Color.white;
		if (Mouse.IsOver(val))
		{
			string iconTip = GetIconTip(pawn);
			if (!iconTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val, iconTip);
			}
		}
		if (Widgets.ButtonInvisible(val, doMouseoverSound: false))
		{
			ClickedIcon(pawn);
		}
		if (Mouse.IsOver(val) && Input.GetMouseButton(0))
		{
			PaintedIcon(pawn);
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), Width);
	}

	public override int GetMaxWidth(PawnTable table)
	{
		return Mathf.Min(base.GetMaxWidth(table), GetMinWidth(table));
	}

	public override int GetMinCellHeight(Pawn pawn)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Max(base.GetMinCellHeight(pawn), Mathf.CeilToInt(GetIconSize(pawn).y));
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
	}

	private int GetValueToCompare(Pawn pawn)
	{
		Texture2D iconFor = GetIconFor(pawn);
		if (!((Object)(object)iconFor != (Object)null))
		{
			return int.MinValue;
		}
		return ((Object)iconFor).GetInstanceID();
	}

	protected abstract Texture2D GetIconFor(Pawn pawn);

	protected virtual string GetIconTip(Pawn pawn)
	{
		return null;
	}

	protected virtual Color GetIconColor(Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Color.white;
	}

	protected virtual void ClickedIcon(Pawn pawn)
	{
	}

	protected virtual void PaintedIcon(Pawn pawn)
	{
	}

	protected virtual Vector2 GetIconSize(Pawn pawn)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetIconFor(pawn) == (Object)null)
		{
			return Vector2.zero;
		}
		return new Vector2((float)Width, (float)Width);
	}
}
