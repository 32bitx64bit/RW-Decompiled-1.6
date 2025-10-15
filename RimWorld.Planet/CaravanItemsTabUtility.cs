using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class CaravanItemsTabUtility
{
	private const float RowHeight = 30f;

	private const float LabelColumnWidth = 300f;

	public static void DoRows(Vector2 size, List<TransferableImmutable> things, Caravan caravan, ref Vector2 scrollPosition, ref float scrollViewHeight)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Invalid comparison between Unknown and I4
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, size.x, size.y), 10f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref val)).width - 16f, scrollViewHeight);
		Widgets.BeginScrollView(val, ref scrollPosition, viewRect);
		float curY = 0f;
		Widgets.ListSeparator(ref curY, ((Rect)(ref viewRect)).width, "CaravanItems".Translate());
		if (things.Any())
		{
			for (int i = 0; i < things.Count; i++)
			{
				DoRow(ref curY, viewRect, val, scrollPosition, things[i], caravan);
			}
		}
		else
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref viewRect)).width);
		}
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = curY + 30f;
		}
		Widgets.EndScrollView();
	}

	public static Vector2 GetSize(List<TransferableImmutable> things, float paneTopY, bool doNeeds = true)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		float num = 300f;
		num += 24f;
		num += 60f;
		Vector2 result = default(Vector2);
		result.x = 103f + num + 16f;
		result.y = Mathf.Min(550f, paneTopY - 30f);
		return result;
	}

	private static void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Vector2 scrollPosition, TransferableImmutable thing, Caravan caravan)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = scrollPosition.y - 30f;
		float num2 = scrollPosition.y + ((Rect)(ref scrollOutRect)).height;
		if (curY > num && curY < num2)
		{
			DoRow(new Rect(0f, curY, ((Rect)(ref viewRect)).width, 30f), thing, caravan);
		}
		curY += 30f;
	}

	private static void DoRow(Rect rect, TransferableImmutable thing, Caravan caravan)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Rect val = rect.AtZero();
		if (thing.TotalStackCount != 1)
		{
			CaravanThingsTabUtility.DoAbandonSpecificCountButton(val, thing, caravan);
		}
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		CaravanThingsTabUtility.DoAbandonButton(val, thing, caravan);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, (((Rect)(ref rect)).height - 24f) / 2f, thing.AnyThing);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		Rect rect2 = val;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMax - 60f;
		CaravanThingsTabUtility.DrawMass(thing, rect2);
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 60f;
		Widgets.DrawHighlightIfMouseover(val);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(4f, (((Rect)(ref rect)).height - 27f) / 2f, 27f, 27f);
		Widgets.ThingIcon(rect3, thing.AnyThing);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).xMax + 4f, 0f, 300f, 30f);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Widgets.Label(rect4, thing.LabelCapWithTotalStackCount.Truncate(((Rect)(ref rect4)).width));
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		Widgets.EndGroup();
	}
}
