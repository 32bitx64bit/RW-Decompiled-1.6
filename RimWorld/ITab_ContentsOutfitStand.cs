using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_ContentsOutfitStand : ITab_ContentsBase
{
	private static readonly CachedTexture DropTex = new CachedTexture("UI/Buttons/Drop");

	public override IList<Thing> container => OutfitStand.HeldItems.ToList();

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

	public Building_OutfitStand OutfitStand => base.SelThing as Building_OutfitStand;

	public override bool VisibleInBlueprintMode => false;

	public ITab_ContentsOutfitStand()
	{
		labelKey = "TabCasketContents";
		containedItemsKey = "TabCasketContents";
	}

	protected override void DoItemsLists(Rect inRect, ref float curY)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ListContainedApparel(inRect, container, ref curY);
	}

	private void ListContainedApparel(Rect inRect, IList<Thing> apparel, ref float curY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(inRect);
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, containedItemsKey.Translate());
		bool flag = false;
		for (int i = 0; i < apparel.Count; i++)
		{
			Thing thing = apparel[i];
			if (thing != null)
			{
				flag = true;
				DoRow(thing, ((Rect)(ref inRect)).width, i, ref curY);
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		GUI.EndGroup();
	}

	private void DoRow(Thing thing, float width, int i, ref float curY)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, width, 28f);
		Widgets.InfoCardButton(0f, curY, thing);
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
			if (!OutfitStand.OccupiedRect().AdjacentCells.Where((IntVec3 x) => x.Walkable(OutfitStand.Map)).TryRandomElement(out var result))
			{
				result = OutfitStand.Position;
			}
			OutfitStand.TryDrop(thing, result, ThingPlaceMode.Near, 1, out var dropped);
			if (dropped.TryGetComp(out CompForbiddable comp))
			{
				comp.Forbidden = true;
			}
		}
		else if (Widgets.ButtonInvisible(val))
		{
			Find.Selector.ClearSelection();
			Find.Selector.Select(thing);
		}
		TooltipHandler.TipRegionByKey(val2, "EjectApparelTooltip");
		Widgets.ThingIcon(new Rect(24f, curY, 28f, 28f), thing);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(60f, curY, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		((Rect)(ref rect)).xMax = ((Rect)(ref val2)).xMin;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect, thing.LabelCap.Truncate(((Rect)(ref rect)).width));
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(val))
		{
			TargetHighlighter.Highlight(thing, arrow: true, colonistBar: false);
			TooltipHandler.TipRegion(val, thing.DescriptionDetailed);
		}
		curY += 28f;
	}
}
