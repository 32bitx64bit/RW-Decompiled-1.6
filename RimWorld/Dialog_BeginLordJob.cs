using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public abstract class Dialog_BeginLordJob : Window
{
	protected IPawnRoleSelectionWidget participantsDrawer;

	private Vector2 scrollPositionQualityDesc;

	private float qualityDescHeight;

	private static readonly Texture2D questionMark = ContentFinder<Texture2D>.Get("UI/Overlays/QuestionMark");

	private static readonly Texture2D QualityOffsetCheckOn = Resources.Load<Texture2D>("Textures/UI/Widgets/RitualQualityCheck_On");

	private static readonly Texture2D QualityOffsetCheckOff = Resources.Load<Texture2D>("Textures/UI/Widgets/RitualQualityCheck_Off");

	protected const float CategoryCaptionHeight = 32f;

	protected const float EntryHeight = 28f;

	protected const float ListWidth = 320f;

	protected const float QualityOffsetListWidth = 402f;

	private const int ContextHash = 798775645;

	private List<QualityFactor> tmpExpectedOutcomeEffects = new List<QualityFactor>();

	private static List<ILordJobOutcomePossibility> tmpOutcomes = new List<ILordJobOutcomePossibility>();

	public override Vector2 InitialSize => new Vector2(845f, 740f);

	protected virtual Vector2 ButtonSize => new Vector2(200f, 40f);

	protected string WarningText
	{
		get
		{
			if (BlockingIssues() == null)
			{
				return "";
			}
			string result = "";
			using (IEnumerator<string> enumerator = BlockingIssues().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					result = enumerator.Current;
				}
			}
			return result;
		}
	}

	public virtual bool CanBegin
	{
		get
		{
			IEnumerable<string> enumerable = BlockingIssues();
			if (enumerable == null)
			{
				return true;
			}
			return !enumerable.Any();
		}
	}

	public virtual TaggedString HeaderLabel => "";

	public virtual TaggedString DescriptionLabel => "";

	public virtual TaggedString ExtraExplanationLabel => "";

	public virtual TaggedString ExpectedQualityLabel => "ExpectedLordJobQuality".Translate();

	public virtual TaggedString OkButtonLabel => "OK".Translate();

	public virtual TaggedString CancelButtonLabel => "CancelButton".Translate();

	public virtual TaggedString QualityFactorsLabel => "QualityFactors".Translate();

	public virtual Texture2D Icon => null;

	protected virtual IEnumerable<string> BlockingIssues()
	{
		return null;
	}

	public virtual TaggedString ExpectedDurationLabel(FloatRange qualityRange)
	{
		return TaggedString.Empty;
	}

	public virtual string OutcomeChancesLabel(string qualityNumber)
	{
		return "LordJobOutcomeChances".Translate(qualityNumber);
	}

	public virtual string OutcomeToolTip(ILordJobOutcomePossibility possibility)
	{
		return possibility.ToolTip;
	}

	public virtual Color QualitySummaryColor(FloatRange qualityRange)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Color.white;
	}

	protected virtual void Start()
	{
		Close();
	}

	protected virtual void Cancel()
	{
		Close();
	}

	public virtual void DoExtraHeaderInfo(ref RectDivider layout, ref RectDivider headerLabelRow)
	{
	}

	public virtual void DrawExtraOutcomeDescriptions(Rect viewRect, FloatRange qualityRange, string qualityNumber, ref float curY, ref float totalInfoHeight)
	{
	}

	public Dialog_BeginLordJob(IPawnRoleSelectionWidget participantsDrawer)
	{
		if (ModLister.CheckAnyExpansion("Ritual"))
		{
			this.participantsDrawer = participantsDrawer;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = true;
			forcePause = true;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		using (TextBlock.Default())
		{
			RectDivider layout = new RectDivider(inRect, 798775645);
			layout.NewRow(0f, VerticalJustification.Bottom, 1f);
			DoHeader(ref layout);
			layout.NewRow(0f);
			DoDescription(ref layout);
			DoButtonRow(ref layout, CanBegin);
			layout.NewRow(0f, VerticalJustification.Top, 20f);
			layout.NewCol(20f, HorizontalJustification.Left, 0f);
			RectDivider layout2 = layout.NewCol(320f, HorizontalJustification.Left, 24f);
			RectDivider layout3 = layout;
			DoLeftColumn(ref layout2);
			DoRightColumn(ref layout3);
		}
	}

	public virtual void DoHeader(ref RectDivider layout)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		RectDivider headerLabelRow;
		using (new TextBlock(GameFont.Medium))
		{
			Vector2 val = Text.CalcSize(HeaderLabel);
			headerLabelRow = layout.NewRow(val.y);
			if ((Object)(object)Icon != (Object)null)
			{
				Widgets.DrawTextureFitted(headerLabelRow.NewCol(val.y, HorizontalJustification.Left, 5f), (Texture)(object)Icon, 1f);
			}
			Widgets.Label(headerLabelRow.NewCol(val.x), HeaderLabel);
		}
		DoExtraHeaderInfo(ref layout, ref headerLabelRow);
	}

	public virtual void DoDescription(ref RectDivider layout)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("DoDescription"))
		{
			string text = DescriptionLabel;
			Rect rect;
			if (!text.NullOrEmpty())
			{
				rect = layout.Rect;
				float num = Text.CalcHeight(text, ((Rect)(ref rect)).width - 30f);
				RectDivider rectDivider = layout.NewRow(num + 17f, VerticalJustification.Top, 10f);
				rectDivider.NewRow(0f, VerticalJustification.Top, 10f);
				rectDivider.NewCol(10f, HorizontalJustification.Left, 0f);
				rectDivider.NewCol(20f, HorizontalJustification.Right, 0f);
				Widgets.Label(rectDivider, text);
			}
			string text2 = ExtraExplanationLabel;
			if (!text2.NullOrEmpty() || !text2.NullOrEmpty())
			{
				rect = layout.Rect;
				float num2 = Text.CalcHeight(text2, ((Rect)(ref rect)).width - 30f);
				RectDivider rectDivider2 = layout.NewRow(num2 + 17f, VerticalJustification.Top, 0f);
				rectDivider2.NewRow(0f, VerticalJustification.Top, 10f);
				rectDivider2.NewCol(10f, HorizontalJustification.Left, 0f);
				rectDivider2.NewCol(20f, HorizontalJustification.Right, 0f);
				Widgets.Label(rectDivider2, text2);
			}
		}
	}

	public virtual void DoButtonRow(ref RectDivider layout, bool canBegin)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		RectDivider rectDivider = layout.NewRow(ButtonSize.y, VerticalJustification.Bottom, 0f);
		RectDivider rectDivider2 = rectDivider.NewCol(ButtonSize.x, HorizontalJustification.Right, 10f);
		RectDivider rectDivider3 = rectDivider.NewCol(ButtonSize.x);
		Rect rect = rectDivider.Rect;
		RectDivider rectDivider4 = rectDivider.NewCol(((Rect)(ref rect)).width, HorizontalJustification.Right);
		TextBlock textBlock = new TextBlock(canBegin ? Color.white : Color.gray);
		try
		{
			if (Widgets.ButtonText(rectDivider2, OkButtonLabel, drawBackground: true, doMouseoverSound: true, canBegin))
			{
				Start();
			}
		}
		finally
		{
			((IDisposable)textBlock).Dispose();
		}
		if (Widgets.ButtonText(rectDivider3, CancelButtonLabel))
		{
			Cancel();
		}
		using (new TextBlock((TextAnchor)5, ColorLibrary.RedReadable))
		{
			Widgets.Label(rectDivider4, WarningText);
		}
	}

	public virtual void DoLeftColumn(ref RectDivider layout)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		layout.NewRow(10f, VerticalJustification.Bottom, 0f);
		using (new ProfilerBlock("DrawPawnList"))
		{
			participantsDrawer.DrawPawnList(layout);
		}
	}

	public virtual void DoRightColumn(ref RectDivider layout)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		DrawQualityFactors(layout.NewCol(402f));
	}

	private float DrawQualityFactor(Rect viewRect, bool even, QualityFactor qualityFactor, float y)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		if (qualityFactor == null)
		{
			return 0f;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref viewRect)).x, y, ((Rect)(ref viewRect)).width, 25f);
		Rect val2 = default(Rect);
		((Rect)(ref val2)).x = ((Rect)(ref viewRect)).x;
		((Rect)(ref val2)).width = ((Rect)(ref viewRect)).width + 10f;
		((Rect)(ref val2)).y = y - 3f;
		((Rect)(ref val2)).height = 28f;
		Rect rect = val2;
		if (even)
		{
			Widgets.DrawLightHighlight(rect);
		}
		GUI.color = (qualityFactor.uncertainOutcome ? ColorLibrary.Yellow : (qualityFactor.positive ? ColorLibrary.Green : ColorLibrary.RedReadable));
		Rect rect2 = val;
		((Rect)(ref rect2)).width = 205f;
		Widgets.LabelEllipses(rect2, "  " + qualityFactor.label);
		using (new TextBlock((TextAnchor)2))
		{
			Widgets.Label(val, qualityFactor.qualityChange);
		}
		if (!qualityFactor.noMiddleColumnInfo)
		{
			if (!qualityFactor.count.NullOrEmpty())
			{
				float x = Text.CalcSize(qualityFactor.count).x;
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(val);
				((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMin + (220f - x / 2f);
				((Rect)(ref rect3)).width = x;
				Widgets.Label(rect3, qualityFactor.count);
			}
			else
			{
				GUI.color = Color.white;
				Texture2D val3 = (qualityFactor.uncertainOutcome ? questionMark : ((!qualityFactor.present) ? QualityOffsetCheckOff : QualityOffsetCheckOn));
				Rect val4 = default(Rect);
				((Rect)(ref val4))._002Ector(val);
				((Rect)(ref val4)).x = ((Rect)(ref val4)).x + 208f;
				((Rect)(ref val4)).y = ((Rect)(ref val4)).y - 1f;
				((Rect)(ref val4)).width = 24f;
				((Rect)(ref val4)).height = 24f;
				if (!qualityFactor.present)
				{
					if (qualityFactor.uncertainOutcome)
					{
						TooltipHandler.TipRegion(val4, () => "QualityFactorTooltipUncertain".Translate(), 238934347);
					}
					else
					{
						TooltipHandler.TipRegion(val4, () => "QualityFactorTooltipNotFulfilled".Translate(), 238934347);
					}
				}
				GUI.DrawTexture(val4, (Texture)(object)val3);
			}
		}
		GUI.color = Color.white;
		if (qualityFactor.toolTip != null && Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(val, () => qualityFactor.toolTip, 976091152);
		}
		return 28f;
	}

	public static string QualityNumberToString(FloatRange qualityRange)
	{
		if (qualityRange.min == qualityRange.max)
		{
			return qualityRange.min.ToStringPercent("F0");
		}
		return qualityRange.min.ToStringPercent("F0") + "-" + qualityRange.max.ToStringPercent("F0");
	}

	protected virtual List<QualityFactor> PopulateQualityFactors(out FloatRange qualityRange)
	{
		tmpExpectedOutcomeEffects.Clear();
		qualityRange = new FloatRange(0f, 0f);
		return tmpExpectedOutcomeEffects;
	}

	protected virtual List<ILordJobOutcomePossibility> PopulateOutcomePossibilities()
	{
		tmpOutcomes.Clear();
		return tmpOutcomes;
	}

	public virtual void DrawOutcomeChances(Rect viewRect, FloatRange qualityRange, string qualityNumber, ref float curY, ref float totalInfoHeight)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		List<ILordJobOutcomePossibility> list = PopulateOutcomePossibilities();
		if (list.NullOrEmpty())
		{
			return;
		}
		Widgets.Label(new Rect(((Rect)(ref viewRect)).x, curY, ((Rect)(ref viewRect)).width, 32f), OutcomeChancesLabel(qualityNumber) + ": ");
		curY += 28f;
		totalInfoHeight += 28f;
		float num = 0f;
		foreach (ILordJobOutcomePossibility item in list)
		{
			num += item.Weight(qualityRange);
		}
		Rect val = default(Rect);
		foreach (ILordJobOutcomePossibility item2 in list)
		{
			float f = item2.Weight(qualityRange) / num;
			TaggedString taggedString = "  - " + item2.Label + ": " + f.ToStringPercent();
			((Rect)(ref val))._002Ector(((Rect)(ref viewRect)).x, curY, Text.CalcSize(taggedString).x + 4f, 32f);
			Rect val2 = new Rect(val);
			((Rect)(ref val2)).width = ((Rect)(ref val)).width + 8f;
			((Rect)(ref val2)).height = 22f;
			Rect rect = val2;
			if (Mouse.IsOver(rect))
			{
				string desc = OutcomeToolTip(item2);
				Widgets.DrawLightHighlight(rect);
				if (!desc.NullOrEmpty())
				{
					TooltipHandler.TipRegion(rect, () => desc, 231134347);
				}
			}
			Widgets.Label(val, taggedString);
			curY += Text.LineHeight;
			totalInfoHeight += Text.LineHeight;
		}
	}

	protected virtual void DrawQualityDescription(Rect outRectQualityDesc, FloatRange qualityRange, string qualityNumber, float totalInfoHeight)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRectQualityDesc)).width, qualityDescHeight);
		bool flag = qualityDescHeight > ((Rect)(ref outRectQualityDesc)).height;
		if (flag)
		{
			((Rect)(ref viewRect)).width = ((Rect)(ref viewRect)).width - 16f;
		}
		float curY = 0f;
		Widgets.BeginScrollView(outRectQualityDesc, ref scrollPositionQualityDesc, viewRect, flag);
		TaggedString taggedString = ExpectedDurationLabel(qualityRange);
		if (!taggedString.NullOrEmpty())
		{
			float num = Text.CalcHeight(taggedString, ((Rect)(ref viewRect)).width);
			Widgets.Label(new Rect(((Rect)(ref viewRect)).x, curY - 4f, ((Rect)(ref viewRect)).width, num), taggedString);
			curY += 17f + (num - 22f);
			totalInfoHeight += 17f + (num - 22f);
		}
		DrawOutcomeChances(viewRect, qualityRange, qualityNumber, ref curY, ref totalInfoHeight);
		DrawExtraOutcomeDescriptions(viewRect, qualityRange, qualityNumber, ref curY, ref totalInfoHeight);
		GUI.color = Color.white;
		qualityDescHeight = curY;
		Widgets.EndScrollView();
	}

	protected virtual void DrawQualityFactors(Rect viewRect)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		float y = ((Rect)(ref viewRect)).y;
		float num = 0f;
		bool flag = true;
		FloatRange qualityRange;
		List<QualityFactor> list = PopulateQualityFactors(out qualityRange);
		if (list.NullOrEmpty())
		{
			return;
		}
		Widgets.Label(new Rect(((Rect)(ref viewRect)).x, y + 3f, ((Rect)(ref viewRect)).width, 32f), QualityFactorsLabel);
		y += 32f;
		num += 32f;
		foreach (QualityFactor item in list.OrderByDescending((QualityFactor e) => e.priority))
		{
			float num2 = DrawQualityFactor(viewRect, flag, item, y);
			y += num2;
			num += num2;
			flag = !flag;
		}
		y += 2f;
		string text = QualityNumberToString(qualityRange);
		using (new TextBlock(QualitySummaryColor(qualityRange)))
		{
			Widgets.Label(new Rect(((Rect)(ref viewRect)).x, y + 4f, ((Rect)(ref viewRect)).width, 25f), ExpectedQualityLabel + ":");
			using (new TextBlock(GameFont.Medium))
			{
				float x = Text.CalcSize(text).x;
				Widgets.Label(new Rect(((Rect)(ref viewRect)).xMax - x, y - 2f, ((Rect)(ref viewRect)).width, 32f), text);
			}
			y += 28f;
			num += 28f;
		}
		Rect rect = viewRect;
		((Rect)(ref rect)).width = ((Rect)(ref rect)).width + 10f;
		((Rect)(ref rect)).height = num;
		rect = rect.ExpandedBy(9f);
		using (new TextBlock(new Color(0.25f, 0.25f, 0.25f)))
		{
			Widgets.DrawBox(rect, 2);
		}
		y += 10f;
		num += 10f;
		Rect outRectQualityDesc = default(Rect);
		((Rect)(ref outRectQualityDesc))._002Ector(((Rect)(ref viewRect)).x, y, ((Rect)(ref viewRect)).width, ((Rect)(ref viewRect)).height - num);
		DrawQualityDescription(outRectQualityDesc, qualityRange, text, num);
	}

	public override void WindowUpdate()
	{
		base.WindowUpdate();
		participantsDrawer.WindowUpdate();
	}

	public override void OnAcceptKeyPressed()
	{
		Start();
	}
}
