using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class MainTabWindow_PawnTable : MainTabWindow
{
	private PawnTable table;

	protected virtual float ExtraBottomSpace => 53f;

	protected virtual float ExtraTopSpace => 0f;

	protected abstract PawnTableDef PawnTableDef { get; }

	protected override float Margin => 6f;

	public override Vector2 RequestedTabSize
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (table == null)
			{
				return Vector2.zero;
			}
			return new Vector2(table.Size.x + Margin * 2f, table.Size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);
		}
	}

	protected virtual IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.FreeColonists;

	public override void PostOpen()
	{
		base.PostOpen();
		if (table == null)
		{
			table = CreateTable();
		}
		SetDirty();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		table.PawnTableOnGUI(new Vector2(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ExtraTopSpace));
	}

	public void Notify_PawnsChanged()
	{
		SetDirty();
	}

	public override void Notify_ResolutionChanged()
	{
		table = CreateTable();
		base.Notify_ResolutionChanged();
	}

	private PawnTable CreateTable()
	{
		return (PawnTable)Activator.CreateInstance(PawnTableDef.workerClass, PawnTableDef, (Func<IEnumerable<Pawn>>)(() => Pawns), UI.screenWidth - (int)(Margin * 2f), (int)((float)(UI.screenHeight - 35) - ExtraBottomSpace - ExtraTopSpace - Margin * 2f));
	}

	protected void SetDirty()
	{
		table.SetDirty();
		SetInitialSizeAndPosition();
	}
}
