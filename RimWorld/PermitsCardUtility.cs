using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class PermitsCardUtility
{
	private static Vector2 rightScrollPosition;

	public static RoyalTitlePermitDef selectedPermit;

	public static Faction selectedFaction;

	private const float LeftRectPercent = 0.33f;

	private const float TitleHeight = 55f;

	private const float ReturnButtonWidth = 180f;

	private const float PermitOptionWidth = 200f;

	private const float PermitOptionHeight = 50f;

	private const float AcceptButtonHeight = 50f;

	private const float SwitchFactionsButtonSize = 32f;

	private const float LineWidthNotSelected = 2f;

	private const float LineWidthSelected = 4f;

	private const int ReturnPermitsCost = 8;

	private static readonly Vector2 PermitOptionSpacing = new Vector2(0.25f, 0.35f);

	private static readonly Texture2D SwitchFactionIcon = ContentFinder<Texture2D>.Get("UI/Icons/SwitchFaction");

	private static bool ShowSwitchFactionButton
	{
		get
		{
			int num = 0;
			foreach (Faction item in Find.FactionManager.AllFactionsVisible)
			{
				if (item.IsPlayer || item.def.permanentEnemy || item.temporary)
				{
					continue;
				}
				foreach (RoyalTitlePermitDef allDef in DefDatabase<RoyalTitlePermitDef>.AllDefs)
				{
					if (allDef.faction == item.def)
					{
						num++;
						break;
					}
				}
			}
			return num > 1;
		}
	}

	private static int TotalReturnPermitsCost(Pawn pawn)
	{
		int num = 8;
		List<FactionPermit> allFactionPermits = pawn.royalty.AllFactionPermits;
		for (int i = 0; i < allFactionPermits.Count; i++)
		{
			if (allFactionPermits[i].OnCooldown && allFactionPermits[i].Permit.royalAid != null)
			{
				num += allFactionPermits[i].Permit.royalAid.favorCost;
			}
		}
		return num;
	}

	public static void DrawRecordsCard(Rect rect, Pawn pawn)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckRoyalty("Permit"))
		{
			return;
		}
		((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 4f;
		if (ShowSwitchFactionButton)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, 32f, 32f);
			if (Widgets.ButtonImage(val, SwitchFactionIcon))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Faction item in Find.FactionManager.AllFactionsVisibleInViewOrder)
				{
					if (!item.IsPlayer && !item.def.permanentEnemy)
					{
						Faction localFaction = item;
						list.Add(new FloatMenuOption(localFaction.Name, delegate
						{
							selectedFaction = localFaction;
							selectedPermit = null;
						}, localFaction.def.FactionIcon, localFaction.Color));
					}
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
			TooltipHandler.TipRegion(val, "SwitchFaction_Desc".Translate());
		}
		if (selectedFaction.def.HasRoyalTitles)
		{
			string label = "ReturnAllPermits".Translate();
			Rect rect2 = new Rect(((Rect)(ref rect)).xMax - 180f, ((Rect)(ref rect)).y - 4f, 180f, 51f);
			int num = TotalReturnPermitsCost(pawn);
			if (Widgets.ButtonText(rect2, label))
			{
				if (!pawn.royalty.PermitsFromFaction(selectedFaction).Any())
				{
					Messages.Message("NoPermitsToReturn".Translate(pawn.Named("PAWN")), new LookTargets(pawn), MessageTypeDefOf.RejectInput, historical: false);
				}
				else if (pawn.royalty.GetFavor(selectedFaction) < num)
				{
					Messages.Message("NotEnoughFavor".Translate(num.Named("FAVORCOST"), selectedFaction.def.royalFavorLabel.Named("FAVOR"), pawn.Named("PAWN"), pawn.royalty.GetFavor(selectedFaction).Named("CURFAVOR")), MessageTypeDefOf.RejectInput);
				}
				else
				{
					string text = "ReturnAllPermits_Confirm".Translate(8.Named("BASEFAVORCOST"), num.Named("FAVORCOST"), selectedFaction.def.royalFavorLabel.Named("FAVOR"), selectedFaction.Named("FACTION"));
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, delegate
					{
						pawn.royalty.RefundPermits(8, selectedFaction);
					}, destructive: true));
				}
			}
			TooltipHandler.TipRegion(rect2, "ReturnAllPermits_Desc".Translate(8.Named("BASEFAVORCOST"), num.Named("FAVORCOST"), selectedFaction.def.royalFavorLabel.Named("FAVOR")));
		}
		RoyalTitleDef currentTitle = pawn.royalty.GetCurrentTitle(selectedFaction);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).xMax - 360f - 4f, ((Rect)(ref rect)).y - 4f, 360f, 55f);
		string text2 = string.Concat("CurrentTitle".Translate() + ": " + ((currentTitle != null) ? currentTitle.GetLabelFor(pawn).CapitalizeFirst() : ((string)"None".Translate())) + "\n" + "UnusedPermits".Translate() + ": ", pawn.royalty.GetPermitPoints(selectedFaction).ToString());
		if (!selectedFaction.def.royalFavorLabel.NullOrEmpty())
		{
			text2 = text2 + "\n" + selectedFaction.def.royalFavorLabel.CapitalizeFirst() + ": " + pawn.royalty.GetFavor(selectedFaction);
		}
		Widgets.Label(rect3, text2);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 55f;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(rect);
		((Rect)(ref rect4)).width = ((Rect)(ref rect4)).width * 0.33f;
		DoLeftRect(rect4, pawn);
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(rect);
		((Rect)(ref rect5)).xMin = ((Rect)(ref rect4)).xMax + 10f;
		DoRightRect(rect5, pawn);
	}

	private static void DoLeftRect(Rect rect, Pawn pawn)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		RoyalTitleDef currentTitle = pawn.royalty.GetCurrentTitle(selectedFaction);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		Widgets.BeginGroup(rect2);
		if (selectedPermit != null)
		{
			Text.Font = GameFont.Medium;
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(0f, num, ((Rect)(ref rect2)).width, 0f);
			Widgets.LabelCacheHeight(ref rect3, selectedPermit.LabelCap);
			Text.Font = GameFont.Small;
			num += ((Rect)(ref rect3)).height;
			if (!selectedPermit.description.NullOrEmpty())
			{
				Rect rect4 = default(Rect);
				((Rect)(ref rect4))._002Ector(0f, num, ((Rect)(ref rect2)).width, 0f);
				Widgets.LabelCacheHeight(ref rect4, selectedPermit.description);
				num += ((Rect)(ref rect4)).height + 16f;
			}
			Rect rect5 = default(Rect);
			((Rect)(ref rect5))._002Ector(0f, num, ((Rect)(ref rect2)).width, 0f);
			string text = "Cooldown".Translate() + ": " + "PeriodDays".Translate(selectedPermit.cooldownDays);
			if (selectedPermit.royalAid != null && selectedPermit.royalAid.favorCost > 0 && !selectedFaction.def.royalFavorLabel.NullOrEmpty())
			{
				text = string.Concat(text, "\n" + "CooldownUseFavorCost".Translate(selectedFaction.def.royalFavorLabel.Named("HONOR")).CapitalizeFirst() + ": ", selectedPermit.royalAid.favorCost.ToString());
			}
			if (selectedPermit.minTitle != null)
			{
				text = text + "\n" + "RequiresTitle".Translate(selectedPermit.minTitle.GetLabelForBothGenders()).Resolve().Colorize((currentTitle != null && currentTitle.seniority >= selectedPermit.minTitle.seniority) ? Color.white : ColorLibrary.RedReadable);
			}
			if (selectedPermit.prerequisite != null)
			{
				text = text + "\n" + "UpgradeFrom".Translate(selectedPermit.prerequisite.LabelCap).Resolve().Colorize(PermitUnlocked(selectedPermit.prerequisite, pawn) ? Color.white : ColorLibrary.RedReadable);
			}
			Widgets.LabelCacheHeight(ref rect5, text);
			num += ((Rect)(ref rect5)).height + 4f;
			Rect rect6 = default(Rect);
			((Rect)(ref rect6))._002Ector(0f, ((Rect)(ref rect2)).height - 50f, ((Rect)(ref rect2)).width, 50f);
			if (selectedPermit.AvailableForPawn(pawn, selectedFaction) && !PermitUnlocked(selectedPermit, pawn) && Widgets.ButtonText(rect6, "AcceptPermit".Translate()))
			{
				SoundDefOf.Quest_Accepted.PlayOneShotOnCamera();
				pawn.royalty.AddPermit(selectedPermit, selectedFaction);
			}
		}
		Widgets.EndGroup();
	}

	private static void DoRightRect(Rect rect, Pawn pawn)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		if (selectedFaction == null)
		{
			return;
		}
		List<RoyalTitlePermitDef> allDefsListForReading = DefDatabase<RoyalTitlePermitDef>.AllDefsListForReading;
		Rect outRect = rect.ContractedBy(10f);
		Rect val = default(Rect);
		for (int i = 0; i < allDefsListForReading.Count; i++)
		{
			RoyalTitlePermitDef permit = allDefsListForReading[i];
			if (CanDrawPermit(permit))
			{
				((Rect)(ref val)).width = Mathf.Max(((Rect)(ref val)).width, DrawPosition(permit).x + 200f + 26f);
				((Rect)(ref val)).height = Mathf.Max(((Rect)(ref val)).height, DrawPosition(permit).y + 50f + 26f);
			}
		}
		Widgets.BeginScrollView(outRect, ref rightScrollPosition, val);
		Widgets.BeginGroup(val.ContractedBy(10f));
		DrawLines();
		Rect rect2 = default(Rect);
		for (int j = 0; j < allDefsListForReading.Count; j++)
		{
			RoyalTitlePermitDef royalTitlePermitDef = allDefsListForReading[j];
			if (CanDrawPermit(royalTitlePermitDef))
			{
				Vector2 val2 = DrawPosition(royalTitlePermitDef);
				((Rect)(ref rect2))._002Ector(val2.x, val2.y, 200f, 50f);
				Color val3 = Widgets.NormalOptionColor;
				Color val4 = (PermitUnlocked(royalTitlePermitDef, pawn) ? TexUI.OldFinishedResearchColor : TexUI.AvailResearchColor);
				Color borderColor;
				if (selectedPermit == royalTitlePermitDef)
				{
					borderColor = TexUI.HighlightBorderResearchColor;
					val4 += TexUI.HighlightBgResearchColor;
				}
				else
				{
					borderColor = TexUI.DefaultBorderResearchColor;
				}
				if (!royalTitlePermitDef.AvailableForPawn(pawn, selectedFaction) && !PermitUnlocked(royalTitlePermitDef, pawn))
				{
					val3 = Color.red;
				}
				if (Widgets.CustomButtonText(ref rect2, string.Empty, val4, val3, borderColor))
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
					selectedPermit = royalTitlePermitDef;
				}
				TextAnchor anchor = Text.Anchor;
				Color color = GUI.color;
				GUI.color = val3;
				Text.Anchor = (TextAnchor)4;
				Widgets.Label(rect2, royalTitlePermitDef.LabelCap);
				GUI.color = color;
				Text.Anchor = anchor;
			}
		}
		Widgets.EndGroup();
		Widgets.EndScrollView();
	}

	private static void DrawLines()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 start = default(Vector2);
		Vector2 end = default(Vector2);
		List<RoyalTitlePermitDef> allDefsListForReading = DefDatabase<RoyalTitlePermitDef>.AllDefsListForReading;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < allDefsListForReading.Count; j++)
			{
				RoyalTitlePermitDef royalTitlePermitDef = allDefsListForReading[j];
				if (!CanDrawPermit(royalTitlePermitDef))
				{
					continue;
				}
				Vector2 val = DrawPosition(royalTitlePermitDef);
				start.x = val.x;
				start.y = val.y + 25f;
				RoyalTitlePermitDef prerequisite = royalTitlePermitDef.prerequisite;
				if (prerequisite != null)
				{
					Vector2 val2 = DrawPosition(prerequisite);
					end.x = val2.x + 200f;
					end.y = val2.y + 25f;
					if ((i == 1 && selectedPermit == royalTitlePermitDef) || selectedPermit == prerequisite)
					{
						Widgets.DrawLine(start, end, TexUI.HighlightLineResearchColor, 4f);
					}
					else if (i == 0)
					{
						Widgets.DrawLine(start, end, TexUI.DefaultLineResearchColor, 2f);
					}
				}
			}
		}
	}

	private static bool PermitUnlocked(RoyalTitlePermitDef permit, Pawn pawn)
	{
		if (pawn.royalty.HasPermit(permit, selectedFaction))
		{
			return true;
		}
		List<FactionPermit> allFactionPermits = pawn.royalty.AllFactionPermits;
		for (int i = 0; i < allFactionPermits.Count; i++)
		{
			if (allFactionPermits[i].Permit.prerequisite == permit && allFactionPermits[i].Faction == selectedFaction)
			{
				return true;
			}
		}
		return false;
	}

	private static Vector2 DrawPosition(RoyalTitlePermitDef permit)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2(permit.uiPosition.x * 200f, permit.uiPosition.y * 50f);
		return val + val * PermitOptionSpacing;
	}

	private static bool CanDrawPermit(RoyalTitlePermitDef permit)
	{
		if (permit.permitPointCost > 0)
		{
			if (permit.faction != null)
			{
				return permit.faction == selectedFaction.def;
			}
			return true;
		}
		return false;
	}
}
