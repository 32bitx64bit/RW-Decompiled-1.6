using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class CaravanThingsTabUtility
{
	public const float MassColumnWidth = 60f;

	public const float SpaceAroundIcon = 4f;

	public const float SpecificTabButtonSize = 24f;

	public const float AbandonButtonSize = 24f;

	public const float AbandonSpecificCountButtonSize = 24f;

	public static readonly Texture2D AbandonButtonTex = ContentFinder<Texture2D>.Get("UI/Buttons/Abandon");

	public static readonly Texture2D AbandonSpecificCountButtonTex = ContentFinder<Texture2D>.Get("UI/Buttons/AbandonSpecificCount");

	public static readonly Texture2D SpecificTabButtonTex = ContentFinder<Texture2D>.Get("UI/Buttons/OpenSpecificTab");

	public static readonly Color OpenedSpecificTabButtonColor = new Color(0f, 0.8f, 0f);

	public static readonly Color OpenedSpecificTabButtonMouseoverColor = new Color(0f, 0.5f, 0f);

	public static void DoAbandonButton(Rect rowRect, Thing t, Caravan caravan)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rowRect)).width - 24f, (((Rect)(ref rowRect)).height - 24f) / 2f, 24f, 24f);
		if (Widgets.ButtonImage(val, AbandonButtonTex))
		{
			CaravanAbandonOrBanishUtility.TryAbandonOrBanishViaInterface(t, caravan);
		}
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, () => CaravanAbandonOrBanishUtility.GetAbandonOrBanishButtonTooltip(t, abandonSpecificCount: false), Gen.HashCombineInt(t.GetHashCode(), 1383004931));
		}
	}

	public static void DoAbandonButton(Rect rowRect, TransferableImmutable t, Caravan caravan)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rowRect)).width - 24f, (((Rect)(ref rowRect)).height - 24f) / 2f, 24f, 24f);
		if (Widgets.ButtonImage(val, AbandonButtonTex))
		{
			CaravanAbandonOrBanishUtility.TryAbandonOrBanishViaInterface(t, caravan);
		}
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, () => CaravanAbandonOrBanishUtility.GetAbandonOrBanishButtonTooltip(t, abandonSpecificCount: false), Gen.HashCombineInt(t.GetHashCode(), 8476546));
		}
	}

	public static void DoAbandonSpecificCountButton(Rect rowRect, Thing t, Caravan caravan)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rowRect)).width - 24f, (((Rect)(ref rowRect)).height - 24f) / 2f, 24f, 24f);
		if (Widgets.ButtonImage(val, AbandonSpecificCountButtonTex))
		{
			CaravanAbandonOrBanishUtility.TryAbandonSpecificCountViaInterface(t, caravan);
		}
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, () => CaravanAbandonOrBanishUtility.GetAbandonOrBanishButtonTooltip(t, abandonSpecificCount: true), Gen.HashCombineInt(t.GetHashCode(), 1163428609));
		}
	}

	public static void DoAbandonSpecificCountButton(Rect rowRect, TransferableImmutable t, Caravan caravan)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rowRect)).width - 24f, (((Rect)(ref rowRect)).height - 24f) / 2f, 24f, 24f);
		if (Widgets.ButtonImage(val, AbandonSpecificCountButtonTex))
		{
			CaravanAbandonOrBanishUtility.TryAbandonSpecificCountViaInterface(t, caravan);
		}
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, () => CaravanAbandonOrBanishUtility.GetAbandonOrBanishButtonTooltip(t, abandonSpecificCount: true), Gen.HashCombineInt(t.GetHashCode(), 1163428609));
		}
	}

	public static void DoOpenSpecificTabButton(Rect rowRect, Pawn p, ref Pawn specificTabForPawn)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Color baseColor = ((p == specificTabForPawn) ? OpenedSpecificTabButtonColor : Color.white);
		Color mouseoverColor = ((p == specificTabForPawn) ? OpenedSpecificTabButtonMouseoverColor : GenUI.MouseoverColor);
		Rect val = new Rect(((Rect)(ref rowRect)).width - 24f, (((Rect)(ref rowRect)).height - 24f) / 2f, 24f, 24f);
		if (Widgets.ButtonImage(val, SpecificTabButtonTex, baseColor, mouseoverColor))
		{
			if (p == specificTabForPawn)
			{
				specificTabForPawn = null;
				SoundDefOf.TabClose.PlayOneShotOnCamera();
			}
			else
			{
				specificTabForPawn = p;
				SoundDefOf.TabOpen.PlayOneShotOnCamera();
			}
		}
		TooltipHandler.TipRegionByKey(val, "OpenSpecificTabButtonTip");
		GUI.color = Color.white;
	}

	public static void DoOpenSpecificTabButtonInvisible(Rect rect, Pawn pawn, ref Pawn specificTabForPawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonInvisible(rect))
		{
			if (pawn == specificTabForPawn)
			{
				specificTabForPawn = null;
			}
			else
			{
				specificTabForPawn = pawn;
			}
			SoundDefOf.TabClose.PlayOneShotOnCamera();
		}
	}

	public static void DrawMass(TransferableImmutable transferable, Rect rect)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < transferable.things.Count; i++)
		{
			num += transferable.things[i].GetStatValue(StatDefOf.Mass) * (float)transferable.things[i].stackCount;
		}
		DrawMass(num, rect);
	}

	public static void DrawMass(Thing thing, Rect rect)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		DrawMass(thing.GetStatValue(StatDefOf.Mass) * (float)thing.stackCount, rect);
	}

	private static void DrawMass(float mass, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = TransferableOneWayWidget.ItemMassColor;
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
		Widgets.Label(rect, mass.ToStringMass());
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
	}
}
