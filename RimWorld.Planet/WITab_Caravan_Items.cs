using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WITab_Caravan_Items : WITab
{
	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private TransferableSorterDef sorter1;

	private TransferableSorterDef sorter2;

	private List<TransferableImmutable> cachedItems = new List<TransferableImmutable>();

	private int cachedItemsHash;

	private int cachedItemsCount;

	private const float SortersSpace = 25f;

	private const float AssignDrugPoliciesButtonHeight = 27f;

	public WITab_Caravan_Items()
	{
		labelKey = "TabCaravanItems";
	}

	protected override void FillTab()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		CheckCreateSorters();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, size.x, size.y);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x + 10f, ((Rect)(ref rect)).y + 10f, 200f, 27f), "AssignDrugPolicies".Translate()))
		{
			Find.WindowStack.Add(new Dialog_AssignCaravanDrugPolicies(base.SelCaravan));
		}
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 37f;
		Widgets.BeginGroup(rect.ContractedBy(10f));
		TransferableUIUtility.DoTransferableSorters(sorter1, sorter2, delegate(TransferableSorterDef x)
		{
			sorter1 = x;
			CacheItems();
		}, delegate(TransferableSorterDef x)
		{
			sorter2 = x;
			CacheItems();
		});
		Widgets.EndGroup();
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 25f;
		Widgets.BeginGroup(rect);
		CheckCacheItems();
		CaravanItemsTabUtility.DoRows(((Rect)(ref rect)).size, cachedItems, base.SelCaravan, ref scrollPosition, ref scrollViewHeight);
		Widgets.EndGroup();
	}

	protected override void UpdateSize()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateSize();
		CheckCacheItems();
		size = CaravanItemsTabUtility.GetSize(cachedItems, PaneTopY);
	}

	private void CheckCacheItems()
	{
		List<Thing> list = CaravanInventoryUtility.AllInventoryItems(base.SelCaravan);
		if (list.Count != cachedItemsCount)
		{
			CacheItems();
			return;
		}
		int num = base.SelCaravan.GetHashCode();
		for (int i = 0; i < list.Count; i++)
		{
			num = Gen.HashCombineInt(num, list[i].GetHashCode());
		}
		if (num != cachedItemsHash)
		{
			CacheItems();
		}
	}

	private void CacheItems()
	{
		CheckCreateSorters();
		cachedItems.Clear();
		List<Thing> list = CaravanInventoryUtility.AllInventoryItems(base.SelCaravan);
		int seed = 0;
		for (int i = 0; i < list.Count; i++)
		{
			TransferableImmutable transferableImmutable = TransferableUtility.TransferableMatching(list[i], cachedItems, TransferAsOneMode.Normal);
			if (transferableImmutable == null)
			{
				transferableImmutable = new TransferableImmutable();
				cachedItems.Add(transferableImmutable);
			}
			transferableImmutable.things.Add(list[i]);
			seed = Gen.HashCombineInt(seed, list[i].GetHashCode());
		}
		cachedItems = cachedItems.OrderBy((TransferableImmutable tr) => tr, sorter1.Comparer).ThenBy((TransferableImmutable tr) => tr, sorter2.Comparer).ThenBy((TransferableImmutable tr) => TransferableUIUtility.DefaultListOrderPriority(tr))
			.ToList();
		cachedItemsCount = list.Count;
		cachedItemsHash = seed;
	}

	private void CheckCreateSorters()
	{
		if (sorter1 == null)
		{
			sorter1 = TransferableSorterDefOf.Category;
		}
		if (sorter2 == null)
		{
			sorter2 = TransferableSorterDefOf.MarketValue;
		}
	}
}
