using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public abstract class PawnColumnWorker_Text : PawnColumnWorker
{
	private static NumericStringComparer comparer = new NumericStringComparer();

	protected virtual int Width => def.width;

	protected virtual TextAnchor Anchor => (TextAnchor)3;

	public override void DoHeader(Rect rect, PawnTable table)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base.DoHeader(rect, table);
		MouseoverSounds.DoRegion(rect);
	}

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, Mathf.Min(((Rect)(ref rect)).height, 30f));
		string textFor = GetTextFor(pawn);
		if (textFor == null)
		{
			return;
		}
		Text.Font = GameFont.Small;
		Text.Anchor = Anchor;
		Text.WordWrap = false;
		Widgets.Label(rect2, textFor);
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect2))
		{
			string tip = GetTip(pawn);
			if (!tip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect2, tip);
			}
		}
	}

	public override int GetMinWidth(PawnTable table)
	{
		return Mathf.Max(base.GetMinWidth(table), Width);
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return comparer.Compare(GetTextFor(a), GetTextFor(b));
	}

	protected abstract string GetTextFor(Pawn pawn);

	protected virtual string GetTip(Pawn pawn)
	{
		return null;
	}
}
