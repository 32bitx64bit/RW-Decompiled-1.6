using System;
using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class TransferableUIUtility
{
	public struct ExtraInfo
	{
		public string key;

		public string value;

		public string secondValue;

		public string tip;

		public float lastFlashTime;

		public Color color;

		public Color secondColor;

		public ExtraInfo(string key, string value, Color color, string tip, float lastFlashTime = -9999f)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			this.key = key;
			this.value = value;
			this.color = color;
			this.tip = tip;
			this.lastFlashTime = lastFlashTime;
			secondValue = null;
			secondColor = default(Color);
		}

		public ExtraInfo(string key, string value, Color color, string tip, string secondValue, Color secondColor, float lastFlashTime = -9999f)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			this.key = key;
			this.value = value;
			this.color = color;
			this.tip = tip;
			this.lastFlashTime = lastFlashTime;
			this.secondValue = secondValue;
			this.secondColor = secondColor;
		}
	}

	private static List<TransferableCountToTransferStoppingPoint> stoppingPoints = new List<TransferableCountToTransferStoppingPoint>();

	private const float AmountAreaWidth = 90f;

	private const float AmountAreaHeight = 25f;

	private const float AdjustArrowWidth = 30f;

	public const float ResourceIconSize = 27f;

	public const float SortersHeight = 27f;

	public const float ExtraInfoHeight = 40f;

	public const float ExtraInfoMargin = 12f;

	public static readonly Color ZeroCountColor = new Color(0.5f, 0.5f, 0.5f);

	public static readonly Texture2D FlashTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0f, 0f, 0.4f));

	private static readonly Texture2D TradeArrow = ContentFinder<Texture2D>.Get("UI/Widgets/TradeArrow");

	private static readonly Texture2D DividerTex = ContentFinder<Texture2D>.Get("UI/Widgets/Divider");

	private static readonly Texture2D PregnantIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Pregnant");

	private static readonly Texture2D BondIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Bond");

	private static readonly Texture2D RideableIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Rideable");

	private static readonly Texture2D BonusIcon = ContentFinder<Texture2D>.Get("UI/Icons/MoveSpeedBonus");

	private static readonly Texture2D SickIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Sick");

	private static readonly Rect SortersRect = new Rect(0f, 0f, 350f, 27f);

	private static readonly Rect SearcherRect = new Rect(360f, 0f, 170f, 27f);

	[TweakValue("Interface", 0f, 50f)]
	private static float PregnancyIconWidth = 24f;

	[TweakValue("Interface", 0f, 50f)]
	private static float BondIconWidth = 24f;

	[TweakValue("Interface", 0f, 50f)]
	private static float RideableIconWidth = 24f;

	[TweakValue("Interface", 0f, 50f)]
	private static float SlaveTradeIconWidth = 24f;

	[TweakValue("Interface", 0f, 50f)]
	private static float OverseerIconWidth = 36f;

	[TweakValue("Interface", 0f, 50f)]
	private static float SickIconWidth = 24f;

	public static void DoCountAdjustInterface(Rect rect, Transferable trad, int index, int min, int max, bool flash = false, List<TransferableCountToTransferStoppingPoint> extraStoppingPoints = null, bool readOnly = false)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		stoppingPoints.Clear();
		if (extraStoppingPoints != null)
		{
			stoppingPoints.AddRange(extraStoppingPoints);
		}
		for (int num = stoppingPoints.Count - 1; num >= 0; num--)
		{
			if (stoppingPoints[num].threshold != 0 && (stoppingPoints[num].threshold <= min || stoppingPoints[num].threshold >= max))
			{
				stoppingPoints.RemoveAt(num);
			}
		}
		bool flag = false;
		for (int i = 0; i < stoppingPoints.Count; i++)
		{
			if (stoppingPoints[i].threshold == 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			stoppingPoints.Add(new TransferableCountToTransferStoppingPoint(0, "0", "0"));
		}
		DoCountAdjustInterfaceInternal(rect, trad, index, min, max, flash, readOnly);
	}

	private static void DoCountAdjustInterfaceInternal(Rect rect, Transferable trad, int index, int min, int max, bool flash, bool readOnly)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		rect = rect.Rounded();
		Rect val = GenUI.Rounded(new Rect(((Rect)(ref rect)).center.x - 45f, ((Rect)(ref rect)).center.y - 12.5f, 90f, 25f));
		if (flash)
		{
			GUI.DrawTexture(val, (Texture)(object)FlashTex);
		}
		bool flag = trad is TransferableOneWay { HasAnyThing: not false } transferableOneWay && transferableOneWay.AnyThing is Pawn && transferableOneWay.MaxCount == 1;
		if (!trad.Interactive || readOnly)
		{
			if (flag)
			{
				bool checkOn = trad.CountToTransfer != 0;
				Widgets.Checkbox(((Rect)(ref val)).position, ref checkOn, 24f, disabled: true);
			}
			else
			{
				GUI.color = ((trad.CountToTransfer == 0) ? ZeroCountColor : Color.white);
				Text.Anchor = (TextAnchor)4;
				Widgets.Label(val, trad.CountToTransfer.ToStringCached());
			}
		}
		else if (flag)
		{
			bool flag2 = trad.CountToTransfer != 0;
			bool checkOn2 = flag2;
			Widgets.Checkbox(((Rect)(ref val)).position, ref checkOn2, 24f, disabled: false, paintable: true);
			if (checkOn2 != flag2)
			{
				if (checkOn2)
				{
					trad.AdjustTo(trad.GetMaximumToTransfer());
				}
				else
				{
					trad.AdjustTo(trad.GetMinimumToTransfer());
				}
			}
		}
		else
		{
			Rect rect2 = val.ContractedBy(2f);
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 15f;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 16f;
			int val2 = trad.CountToTransfer;
			string buffer = trad.EditBuffer;
			Widgets.TextFieldNumeric(rect2, ref val2, ref buffer, min, max);
			trad.AdjustTo(val2);
			trad.EditBuffer = buffer;
		}
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
		if (trad.Interactive && !flag && !readOnly)
		{
			TransferablePositiveCountDirection positiveCountDirection = trad.PositiveCountDirection;
			int num = ((positiveCountDirection == TransferablePositiveCountDirection.Source) ? 1 : (-1));
			int num2 = GenUI.CurrentAdjustmentMultiplier();
			bool flag3 = trad.GetRange() == 1;
			if (trad.CanAdjustBy(num * num2).Accepted)
			{
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(((Rect)(ref val)).x - 30f, ((Rect)(ref rect)).y, 30f, ((Rect)(ref rect)).height);
				if (flag3)
				{
					((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x - ((Rect)(ref rect3)).width;
					((Rect)(ref rect3)).width = ((Rect)(ref rect3)).width + ((Rect)(ref rect3)).width;
				}
				if (Widgets.ButtonText(rect3, "<"))
				{
					trad.AdjustBy(num * num2);
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
				if (!flag3)
				{
					string label = "<<";
					int? num3 = null;
					int num4 = 0;
					for (int i = 0; i < stoppingPoints.Count; i++)
					{
						TransferableCountToTransferStoppingPoint transferableCountToTransferStoppingPoint = stoppingPoints[i];
						if (positiveCountDirection == TransferablePositiveCountDirection.Source)
						{
							if (trad.CountToTransfer < transferableCountToTransferStoppingPoint.threshold && (transferableCountToTransferStoppingPoint.threshold < num4 || !num3.HasValue))
							{
								label = transferableCountToTransferStoppingPoint.leftLabel;
								num3 = transferableCountToTransferStoppingPoint.threshold;
							}
						}
						else if (trad.CountToTransfer > transferableCountToTransferStoppingPoint.threshold && (transferableCountToTransferStoppingPoint.threshold > num4 || !num3.HasValue))
						{
							label = transferableCountToTransferStoppingPoint.leftLabel;
							num3 = transferableCountToTransferStoppingPoint.threshold;
						}
					}
					((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x - ((Rect)(ref rect3)).width;
					if (Widgets.ButtonText(rect3, label))
					{
						if (num3.HasValue)
						{
							trad.AdjustTo(num3.Value);
						}
						else if (num == 1)
						{
							trad.AdjustTo(trad.GetMaximumToTransfer());
						}
						else
						{
							trad.AdjustTo(trad.GetMinimumToTransfer());
						}
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
					}
				}
			}
			if (trad.CanAdjustBy(-num * num2).Accepted)
			{
				Rect rect4 = default(Rect);
				((Rect)(ref rect4))._002Ector(((Rect)(ref val)).xMax, ((Rect)(ref rect)).y, 30f, ((Rect)(ref rect)).height);
				if (flag3)
				{
					((Rect)(ref rect4)).width = ((Rect)(ref rect4)).width + ((Rect)(ref rect4)).width;
				}
				if (Widgets.ButtonText(rect4, ">"))
				{
					trad.AdjustBy(-num * num2);
					SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				}
				if (!flag3)
				{
					string label2 = ">>";
					int? num5 = null;
					int num6 = 0;
					for (int j = 0; j < stoppingPoints.Count; j++)
					{
						TransferableCountToTransferStoppingPoint transferableCountToTransferStoppingPoint2 = stoppingPoints[j];
						if (positiveCountDirection == TransferablePositiveCountDirection.Destination)
						{
							if (trad.CountToTransfer < transferableCountToTransferStoppingPoint2.threshold && (transferableCountToTransferStoppingPoint2.threshold < num6 || !num5.HasValue))
							{
								label2 = transferableCountToTransferStoppingPoint2.rightLabel;
								num5 = transferableCountToTransferStoppingPoint2.threshold;
							}
						}
						else if (trad.CountToTransfer > transferableCountToTransferStoppingPoint2.threshold && (transferableCountToTransferStoppingPoint2.threshold > num6 || !num5.HasValue))
						{
							label2 = transferableCountToTransferStoppingPoint2.rightLabel;
							num5 = transferableCountToTransferStoppingPoint2.threshold;
						}
					}
					((Rect)(ref rect4)).x = ((Rect)(ref rect4)).x + ((Rect)(ref rect4)).width;
					if (Widgets.ButtonText(rect4, label2))
					{
						if (num5.HasValue)
						{
							trad.AdjustTo(num5.Value);
						}
						else if (num == 1)
						{
							trad.AdjustTo(trad.GetMinimumToTransfer());
						}
						else
						{
							trad.AdjustTo(trad.GetMaximumToTransfer());
						}
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
			}
		}
		if (trad.CountToTransfer != 0)
		{
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(((Rect)(ref val)).x + ((Rect)(ref val)).width / 2f - (float)(((Texture)TradeArrow).width / 2), ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f - (float)(((Texture)TradeArrow).height / 2), (float)((Texture)TradeArrow).width, (float)((Texture)TradeArrow).height);
			TransferablePositiveCountDirection positiveCountDirection2 = trad.PositiveCountDirection;
			if ((positiveCountDirection2 == TransferablePositiveCountDirection.Source && trad.CountToTransfer > 0) || (positiveCountDirection2 == TransferablePositiveCountDirection.Destination && trad.CountToTransfer < 0))
			{
				((Rect)(ref val3)).x = ((Rect)(ref val3)).x + ((Rect)(ref val3)).width;
				((Rect)(ref val3)).width = ((Rect)(ref val3)).width * -1f;
			}
			GUI.DrawTexture(val3, (Texture)(object)TradeArrow);
		}
	}

	public static void DrawTransferableInfo(Transferable trad, Rect idRect, Color labelColor)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!trad.HasAnyThing && trad.IsThing)
		{
			return;
		}
		if (Mouse.IsOver(idRect))
		{
			Widgets.DrawHighlight(idRect);
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, 27f, 27f);
		if (trad.IsThing)
		{
			try
			{
				Widgets.ThingIcon(val, trad.AnyThing);
			}
			catch (Exception ex)
			{
				Log.Error("Exception drawing thing icon for " + trad.AnyThing.def.defName + ": " + ex.ToString());
			}
		}
		else
		{
			trad.DrawIcon(val);
		}
		if (trad.IsThing)
		{
			Widgets.InfoCardButton(40f, 0f, trad.AnyThing);
		}
		Text.Anchor = (TextAnchor)3;
		Rect rect = new Rect(80f, 0f, ((Rect)(ref idRect)).width - 80f, ((Rect)(ref idRect)).height);
		Text.WordWrap = false;
		GUI.color = labelColor;
		Widgets.Label(rect, trad.LabelCap);
		GUI.color = Color.white;
		Text.WordWrap = true;
		if (!Mouse.IsOver(idRect))
		{
			return;
		}
		Transferable localTrad = trad;
		TooltipHandler.TipRegion(idRect, new TipSignal(delegate
		{
			if (!localTrad.HasAnyThing && localTrad.IsThing)
			{
				return "";
			}
			string text = localTrad.LabelCap;
			string tipDescription = localTrad.TipDescription;
			if (localTrad.AnyThing is Book)
			{
				text = tipDescription;
			}
			else if (!tipDescription.NullOrEmpty())
			{
				text = text + ": " + tipDescription + ContentSourceDescription(localTrad.AnyThing);
			}
			CompIngredients compIngredients = localTrad.AnyThing.TryGetComp<CompIngredients>();
			if (compIngredients != null)
			{
				text = text + "\n\n" + compIngredients.CompInspectStringExtra();
			}
			return text;
		}, localTrad.GetHashCode()));
	}

	public static float DefaultListOrderPriority(Transferable transferable)
	{
		if (!transferable.HasAnyThing)
		{
			return 0f;
		}
		return DefaultListOrderPriority(transferable.ThingDef);
	}

	public static float DefaultArchonexusItemListOrderPriority(ThingDef def)
	{
		if (def == ThingDefOf.MealSurvivalPack)
		{
			return 100.2f;
		}
		if (def == ThingDefOf.Pemmican)
		{
			return 100.1f;
		}
		if (def == ThingDefOf.Luciferium)
		{
			return 90.1f;
		}
		if (def.IsNonMedicalDrug)
		{
			return 90f;
		}
		if (def.IsMedicine)
		{
			return 80f;
		}
		if (MoveColonyUtility.IsDistinctArchonexusItem(def))
		{
			return 75f;
		}
		if (def == ThingDefOf.Silver)
		{
			return 50.2f;
		}
		if (def == ThingDefOf.Gold)
		{
			return 50.1f;
		}
		if (def.thingCategories.Contains(ThingCategoryDefOf.ResourcesRaw))
		{
			return 70f;
		}
		if (def.thingCategories.Contains(ThingCategoryDefOf.Manufactured) || def.thingCategories.Contains(ThingCategoryDefOf.Drugs))
		{
			return 60f;
		}
		if (def.thingCategories.Contains(ThingCategoryDefOf.PlantMatter))
		{
			return -10f;
		}
		if (def.IsEgg || def.IsAnimalProduct)
		{
			return -20f;
		}
		if (def.thingCategories.Contains(ThingCategoryDefOf.Foods) || def.thingCategories.Contains(ThingCategoryDefOf.PlantFoodRaw))
		{
			return -30f;
		}
		if (def.IsLeather || def.IsWool || def.thingCategories.Contains(ThingCategoryDefOf.Textiles))
		{
			return -40f;
		}
		if (def.thingCategories.Contains(ThingCategoryDefOf.StoneBlocks))
		{
			return -50f;
		}
		return 0f;
	}

	public static float DefaultListOrderPriority(ThingDef def)
	{
		if (def == ThingDefOf.Silver)
		{
			return 100f;
		}
		if (def == ThingDefOf.Gold)
		{
			return 99f;
		}
		if (def.Minifiable)
		{
			return 90f;
		}
		if (def.IsApparel)
		{
			return 80f;
		}
		if (def.IsRangedWeapon)
		{
			return 70f;
		}
		if (def.IsMeleeWeapon)
		{
			return 60f;
		}
		if (def.isTechHediff)
		{
			return 50f;
		}
		if (def.CountAsResource)
		{
			return -10f;
		}
		return 20f;
	}

	public static void DoTransferableSorters(TransferableSorterDef sorter1, TransferableSorterDef sorter2, Action<TransferableSorterDef> sorter1Setter, Action<TransferableSorterDef> sorter2Setter)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(SortersRect);
		Text.Font = GameFont.Tiny;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, 60f, 27f);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect, "SortBy".Translate());
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).xMax + 10f, 0f, 130f, 27f);
		if (Widgets.ButtonText(rect2, sorter1.LabelCap.Truncate(((Rect)(ref rect2)).width - 2f)))
		{
			OpenSorterChangeFloatMenu(sorter1Setter);
		}
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).xMax + 10f, 0f, 130f, 27f);
		if (Widgets.ButtonText(rect3, sorter2.LabelCap.Truncate(((Rect)(ref rect3)).width - 2f)))
		{
			OpenSorterChangeFloatMenu(sorter2Setter);
		}
		Widgets.EndGroup();
	}

	public static void DoTransferableSearcher(QuickSearchWidget searchWidget, Action onSearchChange)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Rect searcherRect = SearcherRect;
		Widgets.BeginGroup(searcherRect);
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, (((Rect)(ref searcherRect)).height - 24f) / 2f, 170f, 24f);
		searchWidget.OnGUI(rect, onSearchChange);
		Text.Font = GameFont.Tiny;
		Widgets.EndGroup();
	}

	public static void DoExtraIcons(Transferable trad, Rect rect, ref float curX)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if (!(trad.AnyThing is Pawn pawn))
		{
			return;
		}
		if (pawn.RaceProps.Animal)
		{
			if (pawn.IsCaravanRideable())
			{
				Rect val = default(Rect);
				((Rect)(ref val))._002Ector(curX - RideableIconWidth, (((Rect)(ref rect)).height - RideableIconWidth) / 2f, RideableIconWidth, RideableIconWidth);
				curX -= ((Rect)(ref val)).width;
				GUI.DrawTexture(val, (Texture)(object)RideableIcon);
				if (Mouse.IsOver(val))
				{
					TooltipHandler.TipRegion(val, CaravanRideableUtility.GetIconTooltipText(pawn));
				}
			}
			if (pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) != null)
			{
				DrawBondedIcon(pawn, new Rect(curX - BondIconWidth, (((Rect)(ref rect)).height - BondIconWidth) / 2f, BondIconWidth, BondIconWidth));
				curX -= BondIconWidth;
			}
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant, mustBeVisible: true))
			{
				DrawPregnancyIcon(pawn, new Rect(curX - PregnancyIconWidth, (((Rect)(ref rect)).height - PregnancyIconWidth) / 2f, PregnancyIconWidth, PregnancyIconWidth));
				curX -= PregnancyIconWidth;
			}
			if (pawn.health.hediffSet.AnyHediffMakesSickThought)
			{
				DrawSickIcon(pawn, new Rect(curX - SickIconWidth, (((Rect)(ref rect)).height - SickIconWidth) / 2f, SickIconWidth, SickIconWidth));
				curX -= SickIconWidth;
			}
		}
		else if (ModsConfig.BiotechActive && pawn.IsColonyMech)
		{
			Pawn overseer = pawn.GetOverseer();
			if (overseer != null)
			{
				DrawOverseerIcon(pawn, overseer, new Rect(curX - OverseerIconWidth, (((Rect)(ref rect)).height - OverseerIconWidth) / 2f, OverseerIconWidth, OverseerIconWidth));
				curX -= OverseerIconWidth;
			}
		}
		else if (CaravanBonusUtility.HasCaravanBonus(pawn))
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(curX - RideableIconWidth, (((Rect)(ref rect)).height - RideableIconWidth) / 2f, RideableIconWidth, RideableIconWidth);
			curX -= ((Rect)(ref val2)).width;
			GUI.DrawTexture(val2, (Texture)(object)BonusIcon);
			if (Mouse.IsOver(val2))
			{
				TooltipHandler.TipRegion(val2, CaravanBonusUtility.GetIconTooltipText(pawn));
			}
		}
	}

	public static void DrawBondedIcon(Pawn bondedPawn, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)BondIcon);
		if (Mouse.IsOver(rect))
		{
			string iconTooltipText = TrainableUtility.GetIconTooltipText(bondedPawn);
			if (!iconTooltipText.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, iconTooltipText);
			}
		}
	}

	public static void DrawPregnancyIcon(Pawn pregnantPawn, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, PawnColumnWorker_Pregnant.GetTooltipText(pregnantPawn));
		}
		GUI.DrawTexture(rect, (Texture)(object)PregnantIcon);
	}

	private static void DrawOverseerIcon(Pawn mech, Pawn overseer, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(rect, (Texture)(object)PortraitsCache.Get(overseer, new Vector2(OverseerIconWidth, OverseerIconWidth), Rot4.South));
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "MechOverseer".Translate(overseer));
		}
	}

	private static void DrawSickIcon(Pawn pawn, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.IsOver(rect))
		{
			IEnumerable<string> entries = from h in pawn.health.hediffSet.hediffs
				where h.def.makesSickThought
				select h.LabelCap;
			TooltipHandler.TipRegion(rect, "CaravanAnimalSick".Translate() + ":\n\n" + entries.ToLineList(" - "));
		}
		GUI.DrawTexture(rect, (Texture)(object)SickIcon);
	}

	private static void OpenSorterChangeFloatMenu(Action<TransferableSorterDef> sorterSetter)
	{
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		List<TransferableSorterDef> allDefsListForReading = DefDatabase<TransferableSorterDef>.AllDefsListForReading;
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			TransferableSorterDef def = allDefsListForReading[i];
			list.Add(new FloatMenuOption(def.LabelCap, delegate
			{
				sorterSetter(def);
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list));
	}

	public static void DrawExtraInfo(List<ExtraInfo> info, Rect rect)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		if (((Rect)(ref rect)).width > (float)info.Count * 230f)
		{
			((Rect)(ref rect)).x = ((Rect)(ref rect)).x + Mathf.Floor((((Rect)(ref rect)).width - (float)info.Count * 230f) / 2f);
			((Rect)(ref rect)).width = (float)info.Count * 230f;
		}
		Widgets.BeginGroup(rect);
		float num = Mathf.Floor(((Rect)(ref rect)).width / (float)info.Count);
		float num2 = 0f;
		Rect val = default(Rect);
		Rect val2 = default(Rect);
		Rect val3 = default(Rect);
		Rect val4 = default(Rect);
		for (int i = 0; i < info.Count; i++)
		{
			float num3 = ((i == info.Count - 1) ? (((Rect)(ref rect)).width - num2) : num);
			((Rect)(ref val))._002Ector(num2, 0f, num3, ((Rect)(ref rect)).height);
			((Rect)(ref val2))._002Ector(num2, 0f, num3, ((Rect)(ref rect)).height / 2f);
			((Rect)(ref val3))._002Ector(num2, ((Rect)(ref rect)).height / 2f, num3, ((Rect)(ref rect)).height / 2f);
			if (Time.time - info[i].lastFlashTime < 1f)
			{
				GUI.DrawTexture(val, (Texture)(object)FlashTex);
			}
			Text.Anchor = (TextAnchor)7;
			Text.Font = GameFont.Tiny;
			GUI.color = Color.gray;
			Widgets.Label(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y - 2f, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height - -3f), info[i].key);
			((Rect)(ref val4))._002Ector(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + -3f + 2f, ((Rect)(ref val3)).width, ((Rect)(ref val3)).height - -3f);
			Text.Font = GameFont.Small;
			if (info[i].secondValue.NullOrEmpty())
			{
				Text.Anchor = (TextAnchor)1;
				GUI.color = info[i].color;
				Widgets.Label(val4, info[i].value);
			}
			else
			{
				Rect rect2 = val4;
				((Rect)(ref rect2)).width = Mathf.Floor(((Rect)(ref val4)).width / 2f - 15f);
				Text.Anchor = (TextAnchor)2;
				GUI.color = info[i].color;
				Widgets.Label(rect2, info[i].value);
				Rect rect3 = val4;
				((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMin + Mathf.Ceil(((Rect)(ref val4)).width / 2f + 15f);
				Text.Anchor = (TextAnchor)0;
				GUI.color = info[i].secondColor;
				Widgets.Label(rect3, info[i].secondValue);
				Rect val5 = val4;
				((Rect)(ref val5)).x = Mathf.Floor(((Rect)(ref val4)).x + ((Rect)(ref val4)).width / 2f - 7.5f);
				((Rect)(ref val5)).y = ((Rect)(ref val5)).y + 3f;
				((Rect)(ref val5)).width = 15f;
				((Rect)(ref val5)).height = 15f;
				GUI.color = Color.white;
				GUI.DrawTexture(val5, (Texture)(object)DividerTex);
			}
			GUI.color = Color.white;
			Widgets.DrawHighlightIfMouseover(val);
			TooltipHandler.TipRegion(val, info[i].tip);
			num2 += num3;
		}
		Widgets.EndGroup();
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawCaptiveTradeInfo(Transferable trad, ITrader trader, Rect rect, ref float curX)
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		if (!(trad.AnyThing is Pawn { guest: not null } pawn) || !pawn.RaceProps.Humanlike)
		{
			return;
		}
		if (TransferableIsCaptive(trad) && (pawn.IsSlaveOfColony || pawn.IsPrisonerOfColony))
		{
			if (pawn.HomeFaction == trader.Faction)
			{
				Rect val = default(Rect);
				((Rect)(ref val))._002Ector(curX - SlaveTradeIconWidth, (((Rect)(ref rect)).height - SlaveTradeIconWidth) / 2f, SlaveTradeIconWidth, SlaveTradeIconWidth);
				curX -= SlaveTradeIconWidth;
				GUI.DrawTexture(val, (Texture)(object)GuestUtility.RansomIcon);
				if (Mouse.IsOver(val))
				{
					TooltipHandler.TipRegion(val, "SellingAsRansom".Translate());
				}
			}
			else
			{
				Rect val2 = default(Rect);
				((Rect)(ref val2))._002Ector(curX - SlaveTradeIconWidth, (((Rect)(ref rect)).height - SlaveTradeIconWidth) / 2f, SlaveTradeIconWidth, SlaveTradeIconWidth);
				curX -= SlaveTradeIconWidth;
				GUI.DrawTexture(val2, (Texture)(object)GuestUtility.SlaveIcon);
				if (Mouse.IsOver(val2))
				{
					TooltipHandler.TipRegion(val2, "SellingAsSlave".Translate());
				}
			}
		}
		else
		{
			float num = 140f;
			string label = ((pawn.guest.joinStatus == JoinStatus.JoinAsColonist) ? "JoinsAsColonist" : "JoinsAsSlave").Translate();
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(curX, 0f, num, ((Rect)(ref rect)).height);
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, label);
			Text.Anchor = (TextAnchor)0;
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				string key = ((pawn.guest.joinStatus == JoinStatus.JoinAsColonist) ? "JoinsAsColonistDesc" : "JoinsAsSlaveDesc");
				TooltipHandler.TipRegion(rect2, key.Translate());
			}
		}
	}

	public static bool TransferableIsCaptive(Transferable trad)
	{
		if (trad.AnyThing is Pawn pawn && pawn.RaceProps.Humanlike)
		{
			if (!pawn.IsSlave)
			{
				return pawn.IsPrisoner;
			}
			return true;
		}
		return false;
	}

	public static bool TradeIsPlayerSellingToSlavery(Tradeable trad, Faction traderFaction)
	{
		if (TransferableIsCaptive(trad) && trad.CountHeldBy(Transactor.Colony) > 0)
		{
			return ((Pawn)trad.AnyThing).HomeFaction != traderFaction;
		}
		return false;
	}

	public static string ContentSourceDescription(Thing thing)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (thing?.ContentSource == null || thing.ContentSource.IsCoreMod)
		{
			return "";
		}
		return "\n\n" + ("Stat_Source_Label".Translate() + ": " + thing.ContentSource.Name).Resolve().Colorize(ColoredText.SubtleGrayColor);
	}
}
