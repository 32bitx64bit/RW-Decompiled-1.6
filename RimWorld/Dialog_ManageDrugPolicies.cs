using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Dialog_ManageDrugPolicies : Dialog_ManagePolicies<DrugPolicy>
{
	private Vector2 scrollPosition;

	private const float DrugEntryRowHeight = 35f;

	private const float CellsPadding = 4f;

	private const float EntryLineYOffset = 2f;

	private const float InfoButtonSize = 16f;

	private const float HeaderHeight = 36f;

	private static readonly Texture2D IconForAddiction = ContentFinder<Texture2D>.Get("UI/Icons/DrugPolicy/Addiction");

	private static readonly Texture2D IconForJoy = ContentFinder<Texture2D>.Get("UI/Icons/DrugPolicy/Joy");

	private static readonly Texture2D IconScheduled = ContentFinder<Texture2D>.Get("UI/Icons/DrugPolicy/Schedule");

	private const float UsageSpacing = 12f;

	protected override string TitleKey => "DrugPolicyTitle";

	protected override string TipKey => "DrugPolicyTip";

	protected override float OffsetHeaderY => 36f;

	public override Vector2 InitialSize => new Vector2((float)Mathf.Min(Screen.width - 50, 1300), 720f);

	public Dialog_ManageDrugPolicies(DrugPolicy policy)
		: base(policy)
	{
	}

	protected override DrugPolicy CreateNewPolicy()
	{
		return Current.Game.drugPolicyDatabase.MakeNewDrugPolicy();
	}

	protected override DrugPolicy GetDefaultPolicy()
	{
		return Current.Game.drugPolicyDatabase.DefaultDrugPolicy();
	}

	protected override void SetDefaultPolicy(DrugPolicy policy)
	{
		Current.Game.drugPolicyDatabase.SetDefault(policy);
	}

	protected override AcceptanceReport TryDeletePolicy(DrugPolicy policy)
	{
		return Current.Game.drugPolicyDatabase.TryDelete(policy);
	}

	protected override List<DrugPolicy> GetPolicies()
	{
		return Current.Game.drugPolicyDatabase.AllPolicies;
	}

	protected override void DoContentsRect(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DoPolicyConfigArea(rect);
	}

	public override void PostOpen()
	{
		base.PostOpen();
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.DrugPolicies, KnowledgeAmount.Total);
	}

	private void DoPolicyConfigArea(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - OffsetHeaderY;
		((Rect)(ref rect2)).height = 36f;
		Rect outRect = rect;
		((Rect)(ref outRect)).yMin = ((Rect)(ref rect2)).yMax;
		DoColumnLabels(rect2);
		Widgets.DrawMenuSection(outRect);
		outRect = outRect.ContractedBy(1f);
		if (base.SelectedPolicy.Count == 0)
		{
			GUI.color = Color.grey;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(outRect, "NoDrugs".Translate());
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
			return;
		}
		float num = (float)base.SelectedPolicy.Count * 35f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width, num);
		Widgets.AdjustRectsForScrollView(rect, ref outRect, ref viewRect);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		DrugPolicy selectedPolicy = base.SelectedPolicy;
		Rect rect3 = default(Rect);
		for (int i = 0; i < selectedPolicy.Count; i++)
		{
			((Rect)(ref rect3))._002Ector(0f, (float)i * 35f, ((Rect)(ref viewRect)).width, 35f);
			DoEntryRow(rect3, selectedPolicy[i], i);
		}
		Widgets.EndScrollView();
	}

	private void CalculateColumnsWidths(Rect rect, out float addictionWidth, out float allowJoyWidth, out float scheduledWidth, out float drugIconWidth, out float drugNameWidth, out float frequencyWidth, out float moodThresholdWidth, out float joyThresholdWidth, out float takeToInventoryWidth)
	{
		float num = ((Rect)(ref rect)).width - 27f - 108f;
		drugIconWidth = 27f;
		drugNameWidth = num * 0.25f;
		addictionWidth = 36f;
		allowJoyWidth = 36f;
		scheduledWidth = 36f;
		frequencyWidth = num * 0.3f;
		moodThresholdWidth = num * 0.15f;
		joyThresholdWidth = num * 0.15f;
		takeToInventoryWidth = num * 0.15f;
	}

	private void DoColumnLabels(Rect rect)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width - 16f;
		CalculateColumnsWidths(rect, out var addictionWidth, out var allowJoyWidth, out var scheduledWidth, out var drugIconWidth, out var drugNameWidth, out var frequencyWidth, out var moodThresholdWidth, out var joyThresholdWidth, out var takeToInventoryWidth);
		float x = ((Rect)(ref rect)).x;
		Text.Anchor = (TextAnchor)7;
		Rect rect2 = new Rect(x + 4f, ((Rect)(ref rect)).y, drugNameWidth + drugIconWidth, ((Rect)(ref rect)).height);
		Widgets.Label(rect2, "DrugColumnLabel".Translate());
		TooltipHandler.TipRegionByKey(rect2, "DrugNameColumnDesc");
		x += drugNameWidth + drugIconWidth;
		Rect rect3 = new Rect(x, ((Rect)(ref rect)).y, takeToInventoryWidth, ((Rect)(ref rect)).height);
		Widgets.Label(rect3, "TakeToInventoryColumnLabel".Translate());
		TooltipHandler.TipRegionByKey(rect3, "TakeToInventoryColumnDesc");
		x += takeToInventoryWidth;
		Rect val = new Rect(x, ((Rect)(ref rect)).yMax - 24f, 24f, 24f);
		GUI.DrawTexture(val, (Texture)(object)IconForAddiction);
		TooltipHandler.TipRegionByKey(val, "DrugUsageTipForAddiction");
		x += addictionWidth;
		Rect val2 = new Rect(x, ((Rect)(ref rect)).yMax - 24f, 24f, 24f);
		GUI.DrawTexture(val2, (Texture)(object)IconForJoy);
		TooltipHandler.TipRegionByKey(val2, "DrugUsageTipForJoy");
		x += allowJoyWidth;
		Rect val3 = new Rect(x, ((Rect)(ref rect)).yMax - 24f, 24f, 24f);
		GUI.DrawTexture(val3, (Texture)(object)IconScheduled);
		TooltipHandler.TipRegionByKey(val3, "DrugUsageTipScheduled");
		x += scheduledWidth;
		Text.Anchor = (TextAnchor)7;
		Rect rect4 = new Rect(x, ((Rect)(ref rect)).y, frequencyWidth, ((Rect)(ref rect)).height);
		Widgets.Label(rect4, "FrequencyColumnLabel".Translate());
		TooltipHandler.TipRegionByKey(rect4, "FrequencyColumnDesc");
		x += frequencyWidth;
		Rect rect5 = new Rect(x, ((Rect)(ref rect)).y, moodThresholdWidth, ((Rect)(ref rect)).height);
		Widgets.Label(rect5, "MoodThresholdColumnLabel".Translate());
		TooltipHandler.TipRegionByKey(rect5, "MoodThresholdColumnDesc");
		x += moodThresholdWidth;
		Rect rect6 = new Rect(x, ((Rect)(ref rect)).y, joyThresholdWidth, ((Rect)(ref rect)).height);
		Widgets.Label(rect6, "JoyThresholdColumnLabel".Translate());
		TooltipHandler.TipRegionByKey(rect6, "JoyThresholdColumnDesc");
		x += joyThresholdWidth;
		Text.Anchor = (TextAnchor)0;
	}

	private void DoEntryRow(Rect rect, DrugPolicyEntry entry, int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		CalculateColumnsWidths(rect, out var addictionWidth, out var allowJoyWidth, out var scheduledWidth, out var drugIconWidth, out var drugNameWidth, out var frequencyWidth, out var moodThresholdWidth, out var joyThresholdWidth, out var takeToInventoryWidth);
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		Text.Anchor = (TextAnchor)3;
		float x = ((Rect)(ref rect)).x;
		float num = (((Rect)(ref rect)).height - drugIconWidth) / 2f;
		Widgets.ThingIcon(new Rect(x + 5f, ((Rect)(ref rect)).y + num, drugIconWidth, drugIconWidth), entry.drug);
		x += drugIconWidth;
		Widgets.Label(GenUI.ContractedBy(new Rect(x, ((Rect)(ref rect)).y, drugNameWidth, ((Rect)(ref rect)).height), 4f), entry.drug.LabelCap);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2)).x = x + Text.CalcSize(entry.drug.LabelCap).x + 5f;
		((Rect)(ref rect2)).y = ((Rect)(ref rect)).y + 8f;
		((Rect)(ref rect2)).width = 16f;
		((Rect)(ref rect2)).height = 16f;
		Widgets.InfoCardButton(rect2, entry.drug);
		x += drugNameWidth;
		Widgets.TextFieldNumeric(GenUI.ContractedBy(new Rect(x, ((Rect)(ref rect)).y, takeToInventoryWidth, ((Rect)(ref rect)).height), 4f), ref entry.takeToInventory, ref entry.takeToInventoryTempBuffer, 0f, PawnUtility.GetMaxAllowedToPickUp(entry.drug));
		x += takeToInventoryWidth;
		if (entry.drug.IsAddictiveDrug)
		{
			Widgets.Checkbox(x, ((Rect)(ref rect)).y + 2f, ref entry.allowedForAddiction, 24f, disabled: false, paintable: true);
		}
		x += addictionWidth;
		if (entry.drug.IsPleasureDrug)
		{
			Widgets.Checkbox(x, ((Rect)(ref rect)).y + 2f, ref entry.allowedForJoy, 24f, disabled: false, paintable: true);
		}
		x += allowJoyWidth;
		Widgets.Checkbox(x, ((Rect)(ref rect)).y + 2f, ref entry.allowScheduled, 24f, disabled: false, paintable: true);
		x += scheduledWidth;
		if (entry.allowScheduled)
		{
			entry.daysFrequency = Widgets.FrequencyHorizontalSlider(GenUI.ContractedBy(new Rect(x, ((Rect)(ref rect)).y + 2f, frequencyWidth, ((Rect)(ref rect)).height), 4f), entry.daysFrequency, 0.1f, 25f, roundToInt: true);
			x += frequencyWidth;
			entry.onlyIfMoodBelow = Widgets.HorizontalSlider(label: (!(entry.onlyIfMoodBelow < 1f)) ? ((string)"NoDrugUseRequirement".Translate()) : entry.onlyIfMoodBelow.ToStringPercent(), rect: GenUI.ContractedBy(new Rect(x, ((Rect)(ref rect)).y + 2f, moodThresholdWidth, ((Rect)(ref rect)).height), 4f), value: entry.onlyIfMoodBelow, min: 0.01f, max: 1f, middleAlignment: true);
			x += moodThresholdWidth;
			entry.onlyIfJoyBelow = Widgets.HorizontalSlider(label: (!(entry.onlyIfJoyBelow < 1f)) ? ((string)"NoDrugUseRequirement".Translate()) : entry.onlyIfJoyBelow.ToStringPercent(), rect: GenUI.ContractedBy(new Rect(x, ((Rect)(ref rect)).y + 2f, joyThresholdWidth, ((Rect)(ref rect)).height), 4f), value: entry.onlyIfJoyBelow, min: 0.01f, max: 1f, middleAlignment: true);
			x += joyThresholdWidth;
		}
		else
		{
			x += frequencyWidth + moodThresholdWidth + joyThresholdWidth;
		}
		Text.Anchor = (TextAnchor)0;
	}
}
