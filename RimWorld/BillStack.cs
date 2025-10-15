using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class BillStack : IExposable
{
	[Unsaved(false)]
	public IBillGiver billGiver;

	private List<Bill> bills = new List<Bill>();

	public const int MaxCount = 15;

	private const float TopAreaHeight = 35f;

	private const float BillInterfaceSpacing = 6f;

	private const float ExtraViewHeight = 60f;

	public List<Bill> Bills => bills;

	public Bill this[int index] => bills[index];

	public int Count => bills.Count;

	public Bill FirstShouldDoNow
	{
		get
		{
			for (int i = 0; i < Count; i++)
			{
				if (bills[i].ShouldDoNow())
				{
					return bills[i];
				}
			}
			return null;
		}
	}

	public bool AnyShouldDoNow
	{
		get
		{
			for (int i = 0; i < Count; i++)
			{
				if (bills[i].ShouldDoNow())
				{
					return true;
				}
			}
			return false;
		}
	}

	public IEnumerator<Bill> GetEnumerator()
	{
		return bills.GetEnumerator();
	}

	public BillStack(IBillGiver giver)
	{
		billGiver = giver;
	}

	public void AddBill(Bill bill)
	{
		bill.billStack = this;
		bills.Add(bill);
	}

	public void Delete(Bill bill)
	{
		bill.deleted = true;
		bills.Remove(bill);
		billGiver.Notify_BillDeleted(bill);
	}

	public void Clear()
	{
		bills.Clear();
	}

	public void Reorder(Bill bill, int offset)
	{
		int num = bills.IndexOf(bill);
		num += offset;
		if (num >= 0)
		{
			bills.Remove(bill);
			bills.Insert(num, bill);
		}
	}

	public void RemoveIncompletableBills()
	{
		for (int num = bills.Count - 1; num >= 0; num--)
		{
			if (!bills[num].CompletableEver)
			{
				Bill bill = bills[num];
				bills.Remove(bill);
				billGiver.Notify_BillDeleted(bill);
			}
		}
	}

	public int IndexOf(Bill bill)
	{
		return bills.IndexOf(bill);
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref bills, "bills", LookMode.Deep);
		if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
		{
			if (bills.RemoveAll((Bill x) => x == null) != 0)
			{
				Log.Error("Some bills were null after loading.");
			}
			if (bills.RemoveAll((Bill x) => x.recipe == null) != 0)
			{
				Log.Error("Some bills had null recipe after loading.");
			}
			for (int i = 0; i < bills.Count; i++)
			{
				bills[i].billStack = this;
			}
		}
	}

	public Bill DoListing(Rect rect, Func<List<FloatMenuOption>> recipeOptionsMaker, ref Vector2 scrollPosition, ref float viewHeight)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Invalid comparison between Unknown and I4
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		Bill result = null;
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Small;
		if (Count < 15)
		{
			Rect rect2 = new Rect(0f, 0f, 150f, 29f);
			if (Widgets.ButtonText(rect2, "AddBill".Translate()))
			{
				Find.WindowStack.Add(new FloatMenu(recipeOptionsMaker()));
			}
			UIHighlighter.HighlightOpportunity(rect2, "AddBill");
		}
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 35f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - 35f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, viewHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num = 0f;
		for (int i = 0; i < Count; i++)
		{
			Bill bill = bills[i];
			Rect rect3 = bill.DoInterface(0f, num, ((Rect)(ref viewRect)).width, i);
			if (!bill.DeletedOrDereferenced && Mouse.IsOver(rect3))
			{
				result = bill;
			}
			num += ((Rect)(ref rect3)).height + 6f;
		}
		if ((int)Event.current.type == 8)
		{
			viewHeight = num + 60f;
		}
		Widgets.EndScrollView();
		Widgets.EndGroup();
		return result;
	}
}
