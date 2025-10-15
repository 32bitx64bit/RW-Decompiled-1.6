using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_GiveName : Window
{
	protected Pawn suggestingPawn;

	protected string curName;

	protected Func<string> nameGenerator;

	protected string nameMessageKey;

	protected string gainedNameMessageKey;

	protected string invalidNameMessageKey;

	protected bool useSecondName;

	protected string curSecondName;

	protected Func<string> secondNameGenerator;

	protected string secondNameMessageKey;

	protected string invalidSecondNameMessageKey;

	private float Height
	{
		get
		{
			if (!useSecondName)
			{
				return 200f;
			}
			return 300f;
		}
	}

	public override Vector2 InitialSize => new Vector2(640f, Height);

	protected virtual int FirstCharLimit => 64;

	protected virtual int SecondCharLimit => 64;

	public Dialog_GiveName()
	{
		if (Find.AnyPlayerHomeMap != null && Find.AnyPlayerHomeMap.mapPawns.FreeColonistsCount != 0)
		{
			if (Find.AnyPlayerHomeMap.mapPawns.FreeColonistsSpawnedCount != 0)
			{
				suggestingPawn = Find.AnyPlayerHomeMap.mapPawns.FreeColonistsSpawned.RandomElement();
			}
			else
			{
				suggestingPawn = Find.AnyPlayerHomeMap.mapPawns.FreeColonists.RandomElement();
			}
		}
		else
		{
			suggestingPawn = PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists_NoSuspended.RandomElement();
		}
		forcePause = true;
		closeOnAccept = false;
		closeOnCancel = false;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		bool flag = false;
		if ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271))
		{
			flag = true;
			Event.current.Use();
		}
		Rect rect2 = default(Rect);
		if (!useSecondName)
		{
			Widgets.Label(new Rect(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height), nameMessageKey.Translate(suggestingPawn.LabelShort, suggestingPawn).CapitalizeFirst());
			if (nameGenerator != null && Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f + 90f, 80f, ((Rect)(ref rect)).width / 2f - 90f, 35f), "Randomize".Translate()))
			{
				curName = nameGenerator();
			}
			curName = Widgets.TextField(new Rect(0f, 80f, ((Rect)(ref rect)).width / 2f + 70f, 35f), curName, FirstCharLimit);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width / 2f + 90f, ((Rect)(ref rect)).height - 35f, ((Rect)(ref rect)).width / 2f - 90f, 35f);
		}
		else
		{
			float num = 0f;
			string text = nameMessageKey.Translate(suggestingPawn.LabelShort, suggestingPawn).CapitalizeFirst();
			Widgets.Label(new Rect(0f, num, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height), text);
			num += Text.CalcHeight(text, ((Rect)(ref rect)).width) + 10f;
			if (nameGenerator != null && Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f + 90f, num, ((Rect)(ref rect)).width / 2f - 90f, 35f), "Randomize".Translate()))
			{
				curName = nameGenerator();
			}
			curName = Widgets.TextField(new Rect(0f, num, ((Rect)(ref rect)).width / 2f + 70f, 35f), curName, FirstCharLimit);
			num += 60f;
			text = secondNameMessageKey.Translate(suggestingPawn.LabelShort, suggestingPawn);
			Widgets.Label(new Rect(0f, num, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height), text);
			num += Text.CalcHeight(text, ((Rect)(ref rect)).width) + 10f;
			if (secondNameGenerator != null && Widgets.ButtonText(new Rect(((Rect)(ref rect)).width / 2f + 90f, num, ((Rect)(ref rect)).width / 2f - 90f, 35f), "Randomize".Translate()))
			{
				curSecondName = secondNameGenerator();
			}
			curSecondName = Widgets.TextField(new Rect(0f, num, ((Rect)(ref rect)).width / 2f + 70f, 35f), curSecondName, SecondCharLimit);
			num += 45f;
			float num2 = ((Rect)(ref rect)).width / 2f - 90f;
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).width / 2f - num2 / 2f, ((Rect)(ref rect)).height - 35f, num2, 35f);
		}
		if (!(Widgets.ButtonText(rect2, "OK".Translate()) || flag))
		{
			return;
		}
		string text2 = curName?.Trim();
		string text3 = curSecondName?.Trim();
		if (IsValidName(text2) && (!useSecondName || IsValidSecondName(text3)))
		{
			if (useSecondName)
			{
				Named(text2);
				NamedSecond(text3);
				Messages.Message(gainedNameMessageKey.Translate(text2, text3), MessageTypeDefOf.TaskCompletion, historical: false);
			}
			else
			{
				Named(text2);
				Messages.Message(gainedNameMessageKey.Translate(text2), MessageTypeDefOf.TaskCompletion, historical: false);
			}
			Find.WindowStack.TryRemove(this);
		}
		else
		{
			Messages.Message(invalidNameMessageKey.Translate(), MessageTypeDefOf.RejectInput, historical: false);
		}
		Event.current.Use();
	}

	protected abstract bool IsValidName(string s);

	protected abstract void Named(string s);

	protected virtual bool IsValidSecondName(string s)
	{
		return true;
	}

	protected virtual void NamedSecond(string s)
	{
	}
}
