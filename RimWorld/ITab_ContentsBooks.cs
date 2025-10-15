using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_ContentsBooks : ITab_ContentsBase
{
	private static readonly CachedTexture DropTex = new CachedTexture("UI/Buttons/Drop");

	public override IList<Thing> container => Bookcase.GetDirectlyHeldThings().ToList();

	public override bool IsVisible
	{
		get
		{
			if (base.SelThing != null)
			{
				return base.IsVisible;
			}
			return false;
		}
	}

	public Building_Bookcase Bookcase => base.SelThing as Building_Bookcase;

	public override bool VisibleInBlueprintMode => false;

	public ITab_ContentsBooks()
	{
		labelKey = "TabCasketContents";
		containedItemsKey = "TabCasketContents";
	}

	protected override void DoItemsLists(Rect inRect, ref float curY)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ListContainedBooks(inRect, container, ref curY);
	}

	private void ListContainedBooks(Rect inRect, IList<Thing> books, ref float curY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(inRect);
		float num = curY;
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, containedItemsKey.Translate());
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref inRect)).width, curY - num - 3f);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegionByKey(rect, "ContainedBooksDesc");
		}
		bool flag = false;
		for (int i = 0; i < books.Count; i++)
		{
			if (books[i] is Book book)
			{
				flag = true;
				DoRow(book, ((Rect)(ref inRect)).width, i, ref curY);
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		GUI.EndGroup();
	}

	private void DoRow(Book book, float width, int i, ref float curY)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, width, 28f);
		Widgets.InfoCardButton(0f, curY, book);
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlightSelected(val);
		}
		else if (i % 2 == 1)
		{
			Widgets.DrawLightHighlight(val);
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).width - 24f, curY, 24f, 24f);
		if (Widgets.ButtonImage(val2, DropTex.Texture))
		{
			if (!Bookcase.OccupiedRect().AdjacentCells.Where((IntVec3 x) => x.Walkable(Bookcase.Map)).TryRandomElement(out var result))
			{
				result = Bookcase.Position;
			}
			Bookcase.GetDirectlyHeldThings().TryDrop(book, result, Bookcase.Map, ThingPlaceMode.Near, 1, out var resultingThing);
			if (resultingThing.TryGetComp(out CompForbiddable comp))
			{
				comp.Forbidden = true;
			}
		}
		else if (Widgets.ButtonInvisible(val))
		{
			Find.Selector.ClearSelection();
			Find.Selector.Select(book);
			InspectPaneUtility.OpenTab(typeof(ITab_Book));
		}
		TooltipHandler.TipRegionByKey(val2, "EjectBookTooltip");
		Widgets.ThingIcon(new Rect(24f, curY, 28f, 28f), book);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(60f, curY, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		((Rect)(ref rect)).xMax = ((Rect)(ref val2)).xMin;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect, book.LabelCap.Truncate(((Rect)(ref rect)).width));
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(val))
		{
			TargetHighlighter.Highlight(book, arrow: true, colonistBar: false);
			TooltipHandler.TipRegion(val, book.DescriptionDetailed);
		}
		curY += 28f;
	}
}
