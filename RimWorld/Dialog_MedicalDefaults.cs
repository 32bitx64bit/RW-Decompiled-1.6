using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_MedicalDefaults : Window
{
	private const float MedicalCareLabelWidth = 230f;

	private const float VerticalGap = 6f;

	public override Vector2 InitialSize => new Vector2(406f, 640f);

	public override string CloseButtonText => "OK".Translate();

	public Dialog_MedicalDefaults()
	{
		forcePause = true;
		doCloseX = true;
		doCloseButton = true;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		float y = 0f;
		using (new TextBlock(GameFont.Medium))
		{
			Widgets.Label(inRect, ref y, "DefaultMedicineSettings".Translate());
		}
		Text.Font = GameFont.Small;
		Widgets.Label(inRect, ref y, "DefaultMedicineSettingsDesc".Translate());
		y += 10f;
		Text.Anchor = (TextAnchor)3;
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForColonist, "MedGroupColonists", "MedGroupColonistsDesc");
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForPrisoner, "MedGroupPrisoners", "MedGroupPrisonersDesc");
		if (ModsConfig.IdeologyActive)
		{
			DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForSlave, "MedGroupSlaves", "MedGroupSlavesDesc");
		}
		if (ModsConfig.AnomalyActive)
		{
			DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForGhouls, "MedGroupGhouls", "MedGroupGhoulsDesc");
		}
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForTamedAnimal, "MedGroupTamedAnimals", "MedGroupTamedAnimalsDesc");
		y += 17f;
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForFriendlyFaction, "MedGroupFriendlyFaction", "MedGroupFriendlyFactionDesc");
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForNeutralFaction, "MedGroupNeutralFaction", "MedGroupNeutralFactionDesc");
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForHostileFaction, "MedGroupHostileFaction", "MedGroupHostileFactionDesc");
		y += 17f;
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForNoFaction, "MedGroupNoFaction", "MedGroupNoFactionDesc");
		DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForWildlife, "MedGroupWildlife", "MedGroupWildlifeDesc");
		if (ModsConfig.AnomalyActive)
		{
			DoRow(inRect, ref y, ref Find.PlaySettings.defaultCareForEntities, "MedGroupEntities", "MedGroupEntitiesDesc");
		}
		Text.Anchor = (TextAnchor)0;
	}

	private void DoRow(Rect rect, ref float y, ref MedicalCareCategory category, string labelKey, string tipKey)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, y, ((Rect)(ref rect)).width, 28f);
		Rect rect3 = new Rect(((Rect)(ref rect)).x, y, 230f, 28f);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(230f, y, 140f, 28f);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawLightHighlight(rect2);
		}
		TooltipHandler.TipRegionByKey(rect2, tipKey);
		Widgets.LabelFit(rect3, labelKey.Translate());
		MedicalCareUtility.MedicalCareSetter(rect4, ref category);
		y += 34f;
	}
}
