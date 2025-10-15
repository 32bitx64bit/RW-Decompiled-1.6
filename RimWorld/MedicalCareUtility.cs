using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class MedicalCareUtility
{
	private static Texture2D[] careTextures;

	public const float CareSetterHeight = 28f;

	public const float CareSetterWidth = 140f;

	private static bool medicalCarePainting;

	public static void Reset()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			careTextures = (Texture2D[])(object)new Texture2D[5];
			careTextures[0] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoCare");
			careTextures[1] = ContentFinder<Texture2D>.Get("UI/Icons/Medical/NoMeds");
			careTextures[2] = ThingDefOf.MedicineHerbal.uiIcon;
			careTextures[3] = ThingDefOf.MedicineIndustrial.uiIcon;
			careTextures[4] = ThingDefOf.MedicineUltratech.uiIcon;
		});
	}

	public static void MedicalCareSetter(Rect rect, ref MedicalCareCategory medCare)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 5f, ((Rect)(ref rect)).height);
		for (int i = 0; i < 5; i++)
		{
			MedicalCareCategory mc = (MedicalCareCategory)i;
			Widgets.DrawHighlightIfMouseover(val);
			MouseoverSounds.DoRegion(val);
			GUI.DrawTexture(val, (Texture)(object)careTextures[i]);
			Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(val);
			if (draggableResult == Widgets.DraggableResult.Dragged)
			{
				medicalCarePainting = true;
			}
			if ((medicalCarePainting && Mouse.IsOver(val) && medCare != mc) || draggableResult.AnyPressed())
			{
				medCare = mc;
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			if (medCare == mc)
			{
				Widgets.DrawBox(val, 2);
			}
			if (Mouse.IsOver(val))
			{
				TooltipHandler.TipRegion(val, () => mc.GetLabel().CapitalizeFirst(), 632165 + i * 17);
			}
			((Rect)(ref val)).x = ((Rect)(ref val)).x + ((Rect)(ref val)).width;
		}
		if (!Input.GetMouseButton(0))
		{
			medicalCarePainting = false;
		}
	}

	public static string GetLabel(this MedicalCareCategory cat)
	{
		return ("MedicalCareCategory_" + cat).Translate();
	}

	public static bool AllowsMedicine(this MedicalCareCategory cat, ThingDef meds)
	{
		return cat switch
		{
			MedicalCareCategory.NoCare => false, 
			MedicalCareCategory.NoMeds => false, 
			MedicalCareCategory.HerbalOrWorse => meds.GetStatValueAbstract(StatDefOf.MedicalPotency) <= ThingDefOf.MedicineHerbal.GetStatValueAbstract(StatDefOf.MedicalPotency), 
			MedicalCareCategory.NormalOrWorse => meds.GetStatValueAbstract(StatDefOf.MedicalPotency) <= ThingDefOf.MedicineIndustrial.GetStatValueAbstract(StatDefOf.MedicalPotency), 
			MedicalCareCategory.Best => true, 
			_ => throw new InvalidOperationException(), 
		};
	}

	public static void MedicalCareSelectButton(Rect rect, Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Widgets.Dropdown(rect, pawn, MedicalCareSelectButton_GetMedicalCare, MedicalCareSelectButton_GenerateMenu, null, careTextures[(uint)pawn.playerSettings.medCare], null, null, null, paintable: true);
	}

	private static MedicalCareCategory MedicalCareSelectButton_GetMedicalCare(Pawn pawn)
	{
		return pawn.playerSettings.medCare;
	}

	private static IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>> MedicalCareSelectButton_GenerateMenu(Pawn p)
	{
		for (int i = 0; i < 5; i++)
		{
			MedicalCareCategory mc = (MedicalCareCategory)i;
			yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
			{
				option = new FloatMenuOption(mc.GetLabel().CapitalizeFirst(), delegate
				{
					p.playerSettings.medCare = mc;
				}, MedicalCareIcon(mc), Color.white),
				payload = mc
			};
		}
		yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
		{
			option = new FloatMenuOption("ChangeDefaults".Translate(), delegate
			{
				Find.WindowStack.Add(new Dialog_MedicalDefaults());
			})
		};
	}

	private static Texture2D MedicalCareIcon(MedicalCareCategory category)
	{
		return (Texture2D)(category switch
		{
			MedicalCareCategory.NoCare => careTextures[0], 
			MedicalCareCategory.NoMeds => careTextures[1], 
			MedicalCareCategory.HerbalOrWorse => careTextures[2], 
			MedicalCareCategory.NormalOrWorse => careTextures[3], 
			MedicalCareCategory.Best => careTextures[4], 
			_ => null, 
		});
	}
}
