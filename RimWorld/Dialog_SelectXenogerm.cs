using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_SelectXenogerm : Window
{
	private Pawn pawn;

	private List<Xenogerm> xenogerms = new List<Xenogerm>();

	private Xenogerm selected;

	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private Action<Xenogerm> onSelect;

	private const float HeaderHeight = 35f;

	public const float MiniGeneIconSize = 22f;

	private const float XenogermElementHeight = 32f;

	private const int MaxDisplayedGenes = 10;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private Dictionary<string, string> truncateCache = new Dictionary<string, string>();

	public override Vector2 InitialSize => new Vector2(500f, 600f);

	public Dialog_SelectXenogerm(Pawn pawn, Map map, Xenogerm initialSelected, Action<Xenogerm> onSelect)
	{
		this.pawn = pawn;
		this.onSelect = onSelect;
		foreach (Thing item in map.listerThings.ThingsOfDef(ThingDefOf.Xenogerm))
		{
			if (!item.PositionHeld.Fogged(map))
			{
				xenogerms.Add((Xenogerm)item);
			}
		}
		if (initialSelected != null && xenogerms.Contains(initialSelected))
		{
			selected = initialSelected;
		}
		closeOnAccept = false;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckBiotech("xenogerm"))
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax - (ButSize.y + 4f);
		Text.Font = GameFont.Medium;
		Widgets.Label(rect2, "SelectXenogerm".Translate());
		Text.Font = GameFont.Small;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin + 39f;
		DisplayXenogerms(rect2);
		Rect val = rect;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMax - ButSize.y;
		if (selected != null)
		{
			if (Widgets.ButtonText(new Rect(((Rect)(ref val)).xMax - ButSize.x, ((Rect)(ref val)).y, ButSize.x, ButSize.y), "Accept".Translate()))
			{
				Accept();
			}
			if (Widgets.ButtonText(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ButSize.x, ButSize.y), "Close".Translate()))
			{
				Close();
			}
		}
		else if (Widgets.ButtonText(new Rect((((Rect)(ref val)).width - ButSize.x) / 2f, ((Rect)(ref val)).y, ButSize.x, ButSize.y), "Close".Translate()))
		{
			Close();
		}
	}

	private void DisplayXenogerms(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawMenuSection(rect);
		rect = rect.ContractedBy(4f);
		GUI.BeginGroup(rect);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollViewHeight);
		float num = 0f;
		Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, viewRect);
		for (int i = 0; i < xenogerms.Count; i++)
		{
			float num2 = ((Rect)(ref rect)).width;
			if (scrollViewHeight > ((Rect)(ref rect)).height)
			{
				num2 -= 16f;
			}
			DrawXenogerm(new Rect(0f, num, num2, 32f), i);
			num += 32f;
		}
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = num;
		}
		Widgets.EndScrollView();
		GUI.EndGroup();
	}

	private void DrawXenogerm(Rect rect, int index)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		Xenogerm xenogerm = xenogerms[index];
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
		}
		if (selected == xenogerm)
		{
			Widgets.DrawHighlightSelected(rect);
		}
		Widgets.InfoCardButton(((Rect)(ref rect)).xMax - 24f, ((Rect)(ref rect)).y + 4f, xenogerm);
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 36f;
		for (int num = Mathf.Min(xenogerm.GeneSet.GenesListForReading.Count, 10) - 1; num >= 0; num--)
		{
			GeneDef geneDef = xenogerm.GeneSet.GenesListForReading[num];
			Rect val = new Rect(((Rect)(ref rect)).xMax - 11f, ((Rect)(ref rect)).yMax - ((Rect)(ref rect)).height / 2f - 11f, 22f, 22f);
			Widgets.DefIcon(val, geneDef, null, 1.25f);
			Rect rect2 = val;
			((Rect)(ref rect2)).yMin = ((Rect)(ref rect)).yMin;
			((Rect)(ref rect2)).yMax = ((Rect)(ref rect)).yMax;
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, geneDef.LabelCap + "\n\n" + geneDef.DescriptionFull);
			}
			((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 22f;
		}
		if (Mouse.IsOver(rect))
		{
			TooltipHandler.TipRegion(rect, () => xenogerm.LabelCap + "\n\n" + "Genes".Translate().CapitalizeFirst() + ":\n" + xenogerm.GeneSet.GenesListForReading.Select((GeneDef x) => x.LabelCap.ToString()).ToLineList("  - "), 128921381);
		}
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 4f;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect, xenogerm.LabelCap.Truncate(((Rect)(ref rect)).width, truncateCache));
		Text.Anchor = (TextAnchor)0;
		if (Widgets.ButtonInvisible(rect))
		{
			selected = xenogerm;
		}
	}

	private void Accept()
	{
		if (pawn != null)
		{
			int num = GeneUtility.MetabolismAfterImplanting(pawn, selected.GeneSet);
			if (num < GeneTuning.BiostatRange.TrueMin)
			{
				Messages.Message(string.Concat("OrderImplantationIntoPawn".Translate(pawn.Named("PAWN")).Resolve().UncapitalizeFirst() + ": " + "ResultingMetTooLow".Translate() + " (", num.ToString(), ")"), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
			if (selected.PawnIdeoDisallowsImplanting(pawn))
			{
				Messages.Message("CannotGenericWorkCustom".Translate("OrderImplantationIntoPawn".Translate(pawn.Named("PAWN")).Resolve().UncapitalizeFirst() + ": " + "IdeoligionForbids".Translate()), pawn, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
		}
		if (onSelect != null)
		{
			onSelect(selected);
		}
		Close();
	}
}
