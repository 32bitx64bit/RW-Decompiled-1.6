using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnColumnWorker_Sterilize : PawnColumnWorker_Checkbox
{
	public static readonly CachedTexture AnimalSterilized = new CachedTexture("UI/Icons/Animal/Sterile");

	public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (AnimalSterile(pawn))
		{
			DrawSterile(rect, pawn);
		}
		else
		{
			base.DoCell(rect, pawn, table);
		}
	}

	private void DrawSterile(Rect rect, Pawn animal)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((((Rect)(ref rect)).width - 24f) / 2f);
		int num2 = Mathf.Max(3, 0);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref rect)).x + (float)num, ((Rect)(ref rect)).y + (float)num2);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(val.x, val.y, 24f, 24f);
		GUI.DrawTexture(val2, (Texture)(object)AnimalSterilized.Texture);
		if (Mouse.IsOver(val2))
		{
			string tip = GetTip(animal);
			if (!tip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(val2, tip);
			}
		}
	}

	public override int Compare(Pawn a, Pawn b)
	{
		return GetValueToCompare(a).CompareTo(GetValueToCompare(b));
	}

	private int GetValueToCompare(Pawn pawn)
	{
		if (AnimalSterile(pawn))
		{
			return -1;
		}
		if (SterilizeOperations(pawn).Any())
		{
			return 0;
		}
		return 1;
	}

	protected override string GetTip(Pawn pawn)
	{
		if (AnimalSterile(pawn))
		{
			return "AnimalAlreadySterile".Translate();
		}
		return "SterilizeAnimal".Translate();
	}

	protected override bool GetValue(Pawn pawn)
	{
		return SterilizeOperations(pawn).Any();
	}

	protected override void SetValue(Pawn pawn, bool value, PawnTable table)
	{
		if (AnimalSterile(pawn))
		{
			return;
		}
		bool flag = SterilizeOperations(pawn).Any();
		if (value == flag)
		{
			return;
		}
		if (table.SortingBy == def)
		{
			table.SetDirty();
		}
		if (value)
		{
			if (pawn.HomeFaction != Faction.OfPlayer && pawn.HomeFaction != null)
			{
				TaggedString taggedString = "AnimalSterilizeConfirm".Translate(pawn.Named("PAWN"), pawn.HomeFaction.Named("FACTION"));
				Find.WindowStack.Add(new Dialog_Confirm(taggedString, delegate
				{
					AddSterilizeOperation(pawn);
				}));
			}
			else
			{
				AddSterilizeOperation(pawn);
			}
		}
		else
		{
			CancelSterilizeOperations(pawn);
		}
	}

	private bool AnimalSterile(Pawn animal)
	{
		return animal.health.hediffSet.HasHediff(HediffDefOf.Sterilized);
	}

	private List<Bill> SterilizeOperations(Pawn animal)
	{
		return animal.BillStack.Bills.Where((Bill b) => b.recipe == RecipeDefOf.Sterilize).ToList();
	}

	private void AddSterilizeOperation(Pawn animal)
	{
		HealthCardUtility.CreateSurgeryBill(animal, RecipeDefOf.Sterilize, null);
	}

	private void CancelSterilizeOperations(Pawn animal)
	{
		foreach (Bill item in SterilizeOperations(animal))
		{
			animal.BillStack.Delete(item);
		}
	}
}
