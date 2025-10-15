using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_ConfirmModUpload : Dialog_MessageBox
{
	private ModMetaData mod;

	public Dialog_ConfirmModUpload(ModMetaData mod, Action acceptAction)
		: base("ConfirmSteamWorkshopUpload".Translate(), "Confirm".Translate(), acceptAction, "GoBack".Translate(), null, null, buttonADestructive: true, acceptAction)
	{
		this.mod = mod;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.DoWindowContents(inRect);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref inRect)).x + 10f, ((Rect)(ref inRect)).height - 35f - 24f - 10f);
		Widgets.Checkbox(val, ref mod.translationMod);
		Widgets.Label(new Rect(val.x + 24f + 10f, val.y + (24f - Text.LineHeight) / 2f, ((Rect)(ref inRect)).width / 2f, 24f), "TagAsTranslation".Translate());
	}
}
