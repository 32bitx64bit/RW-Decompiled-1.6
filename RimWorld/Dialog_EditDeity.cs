using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_EditDeity : Window
{
	private IdeoFoundation_Deity.Deity deity;

	private Ideo ideo;

	private string newDeityName;

	private string newDeityTitle;

	private Gender newDeityGender;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private static readonly float EditFieldHeight = 30f;

	public override Vector2 InitialSize => new Vector2(700f, 250f);

	public Dialog_EditDeity(IdeoFoundation_Deity.Deity deity, Ideo ideo)
	{
		this.deity = deity;
		this.ideo = ideo;
		absorbInputAroundWindow = true;
		newDeityName = deity.name;
		newDeityTitle = deity.type;
		newDeityGender = deity.gender;
	}

	public override void OnAcceptKeyPressed()
	{
		ApplyChanges();
		Event.current.Use();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 3f;
		float num2 = ((Rect)(ref rect)).xMax - num;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, 35f), "EditDeity".Translate());
		Text.Font = GameFont.Small;
		float num3 = ((Rect)(ref rect)).y + 35f + 10f;
		Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "DeityName".Translate());
		newDeityName = Widgets.TextField(new Rect(num, num3, num2, EditFieldHeight), newDeityName);
		num3 += EditFieldHeight + 10f;
		Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "DeityTitle".Translate());
		newDeityTitle = Widgets.TextField(new Rect(num, num3, num2, EditFieldHeight), newDeityTitle);
		num3 += EditFieldHeight + 10f;
		Widgets.Label(new Rect(((Rect)(ref rect)).x, num3, num2, EditFieldHeight), "DeityGender".Translate());
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(num, num3, EditFieldHeight + 8f + Text.CalcSize(newDeityGender.GetLabel().CapitalizeFirst()).x, EditFieldHeight);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, num3, EditFieldHeight, EditFieldHeight);
		GUI.DrawTexture(rect2.ContractedBy(2f), (Texture)(object)newDeityGender.GetIcon());
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(new Rect(((Rect)(ref rect2)).xMax + 4f, num3, ((Rect)(ref val)).width - ((Rect)(ref rect2)).width, EditFieldHeight), newDeityGender.GetLabel().CapitalizeFirst());
		Text.Anchor = (TextAnchor)0;
		Widgets.DrawHighlightIfMouseover(val);
		if (Widgets.ButtonInvisible(val))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			Gender[] array = (Gender[])Enum.GetValues(typeof(Gender));
			foreach (Gender g in array)
			{
				list.Add(new FloatMenuOption(g.GetLabel().CapitalizeFirst(), delegate
				{
					newDeityGender = g;
				}, g.GetIcon(), Color.white));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		num3 += EditFieldHeight + 10f;
		if (Widgets.ButtonText(new Rect(0f, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "Back".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - ButSize.x, ((Rect)(ref rect)).height - ButSize.y, ButSize.x, ButSize.y), "DoneButton".Translate()))
		{
			ApplyChanges();
		}
	}

	private void ApplyChanges()
	{
		deity.name = newDeityName;
		deity.type = newDeityTitle;
		deity.gender = newDeityGender;
		ideo.RegenerateAllPreceptNames();
		ideo.RegenerateDescription();
		Close();
	}
}
