using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class PawnColumnWorker
{
	public PawnColumnDef def;

	protected const int DefaultCellHeight = 30;

	private static readonly Texture2D SortingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");

	private static readonly Texture2D SortingDescendingIcon = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");

	private const int IconMargin = 2;

	protected virtual Color DefaultHeaderColor => Color.white;

	protected virtual GameFont DefaultHeaderFont => GameFont.Small;

	protected virtual TextAnchor DefaultHeaderAlignment => (TextAnchor)7;

	public virtual bool VisibleCurrently => true;

	public virtual void DoHeader(Rect rect, PawnTable table)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if (!def.label.NullOrEmpty())
		{
			Text.Font = DefaultHeaderFont;
			GUI.color = DefaultHeaderColor;
			Text.Anchor = DefaultHeaderAlignment;
			Rect rect2 = rect;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + GetHeaderOffsetX(rect);
			Widgets.Label(rect2, def.LabelCap.Resolve().Truncate(((Rect)(ref rect)).width));
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
		}
		else if ((Object)(object)def.HeaderIcon != (Object)null)
		{
			Vector2 headerIconSize = def.HeaderIconSize;
			int num = (int)((((Rect)(ref rect)).width - headerIconSize.x) / 2f);
			GUI.DrawTexture(GenUI.ContractedBy(new Rect(((Rect)(ref rect)).x + (float)num, ((Rect)(ref rect)).yMax - headerIconSize.y, headerIconSize.x, headerIconSize.y), 2f), (Texture)(object)def.HeaderIcon);
		}
		if (table.SortingBy == def)
		{
			Texture2D val = (table.SortingDescending ? SortingDescendingIcon : SortingIcon);
			GUI.DrawTexture(new Rect(((Rect)(ref rect)).xMax - (float)((Texture)val).width - 1f, ((Rect)(ref rect)).yMax - (float)((Texture)val).height - 1f, (float)((Texture)val).width, (float)((Texture)val).height), (Texture)(object)val);
		}
		if (!def.HeaderInteractable)
		{
			return;
		}
		Rect interactableHeaderRect = GetInteractableHeaderRect(rect, table);
		if (Mouse.IsOver(interactableHeaderRect))
		{
			Widgets.DrawHighlight(interactableHeaderRect);
			string headerTip = GetHeaderTip(table);
			if (!headerTip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(interactableHeaderRect, headerTip);
			}
		}
		if (Widgets.ButtonInvisible(interactableHeaderRect))
		{
			HeaderClicked(rect, table);
		}
	}

	protected virtual float GetHeaderOffsetX(Rect rect)
	{
		return 0f;
	}

	public abstract void DoCell(Rect rect, Pawn pawn, PawnTable table);

	public virtual bool CanGroupWith(Pawn pawn, Pawn other)
	{
		return false;
	}

	public virtual void Recache()
	{
	}

	public virtual int GetMinWidth(PawnTable table)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!def.label.NullOrEmpty())
		{
			Text.Font = DefaultHeaderFont;
			int result = Mathf.CeilToInt(Text.CalcSize(def.LabelCap).x);
			Text.Font = GameFont.Small;
			return result;
		}
		if ((Object)(object)def.HeaderIcon != (Object)null)
		{
			return Mathf.CeilToInt(def.HeaderIconSize.x);
		}
		return 1;
	}

	public virtual int GetMaxWidth(PawnTable table)
	{
		return 1000000;
	}

	public virtual int GetOptimalWidth(PawnTable table)
	{
		return GetMinWidth(table);
	}

	public virtual int GetMinCellHeight(Pawn pawn)
	{
		return 30;
	}

	public virtual int GetMinHeaderHeight(PawnTable table)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!def.label.NullOrEmpty())
		{
			Text.Font = DefaultHeaderFont;
			int result = Mathf.CeilToInt(Text.CalcSize(def.LabelCap).y);
			Text.Font = GameFont.Small;
			return result;
		}
		if ((Object)(object)def.HeaderIcon != (Object)null)
		{
			return Mathf.CeilToInt(def.HeaderIconSize.y);
		}
		return 0;
	}

	public virtual int Compare(Pawn a, Pawn b)
	{
		return 0;
	}

	protected virtual Rect GetInteractableHeaderRect(Rect headerRect, PawnTable table)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Min(25f, ((Rect)(ref headerRect)).height);
		return new Rect(((Rect)(ref headerRect)).x, ((Rect)(ref headerRect)).yMax - num, ((Rect)(ref headerRect)).width, num);
	}

	protected virtual void HeaderClicked(Rect headerRect, PawnTable table)
	{
		if (!def.sortable || Event.current.shift)
		{
			return;
		}
		if (Event.current.button == 0)
		{
			if (table.SortingBy != def)
			{
				table.SortBy(def, descending: true);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else if (table.SortingDescending)
			{
				table.SortBy(def, descending: false);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				table.SortBy(null, descending: false);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}
		else if (Event.current.button == 1)
		{
			if (table.SortingBy != def)
			{
				table.SortBy(def, descending: false);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else if (table.SortingDescending)
			{
				table.SortBy(null, descending: false);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
			else
			{
				table.SortBy(def, descending: true);
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
		}
	}

	protected virtual string GetHeaderTip(PawnTable table)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!def.headerTip.NullOrEmpty())
		{
			stringBuilder.Append(def.headerTip);
		}
		if (def.sortable)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("ClickToSortByThisColumn".Translate());
		}
		return stringBuilder.ToString();
	}
}
