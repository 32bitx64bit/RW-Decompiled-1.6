using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_AutoSlaughter : Window
{
	private struct AnimalCountRecord
	{
		public int total;

		public int male;

		public int maleYoung;

		public int female;

		public int femaleYoung;

		public int pregnant;

		public int bonded;

		public AnimalCountRecord(int total, int male, int maleYoung, int female, int femaleYoung, int pregnant, int bonded)
		{
			this.total = total;
			this.male = male;
			this.maleYoung = maleYoung;
			this.female = female;
			this.femaleYoung = femaleYoung;
			this.pregnant = pregnant;
			this.bonded = bonded;
		}
	}

	private Map map;

	private Vector2 scrollPos;

	private Rect viewRect;

	private Dictionary<ThingDef, AnimalCountRecord> animalCounts = new Dictionary<ThingDef, AnimalCountRecord>();

	private List<AutoSlaughterConfig> configsOrdered = new List<AutoSlaughterConfig>();

	private List<Rect> tmpMouseoverHighlightRects = new List<Rect>();

	private List<Rect> tmpGroupRects = new List<Rect>();

	private const float ColumnWidthCurrent = 60f;

	private const float ColumnWidthMaxNoLabelSpacing = 56f;

	private const float ColumnWidthMax = 60f;

	private const float SizeControlInfinityButton = 48f;

	private const float SizeControlTextArea = 40f;

	private const float ExtraSpacingPregnant = 16f;

	private const int NumColumns = 7;

	public override Vector2 InitialSize => new Vector2(1050f, 600f);

	public Dialog_AutoSlaughter(Map map)
	{
		this.map = map;
		forcePause = true;
		doCloseX = true;
		doCloseButton = true;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		RecalculateAnimals();
	}

	private void RecalculateAnimals()
	{
		animalCounts.Clear();
		foreach (AutoSlaughterConfig config in map.autoSlaughterManager.configs)
		{
			AnimalCountRecord value = default(AnimalCountRecord);
			CountPlayerAnimals(map, config, config.animal, out value.male, out value.maleYoung, out value.female, out value.femaleYoung, out value.total, out value.pregnant, out value.bonded);
			animalCounts.Add(config.animal, value);
		}
		configsOrdered = (from c in map.autoSlaughterManager.configs
			orderby animalCounts[c.animal].total descending, c.animal.label
			select c).ToList();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(inRect);
		((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - Window.CloseButSize.y;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 8f;
		Listing_Standard listing_Standard = new Listing_Standard(val, () => scrollPos);
		listing_Standard.ColumnWidth = ((Rect)(ref inRect)).width - 16f - 4f;
		viewRect = new Rect(0f, 0f, ((Rect)(ref val)).width - 16f, 30f * (float)(configsOrdered.Count + 1));
		Rect val2 = val;
		((Rect)(ref val2)).x = scrollPos.x;
		((Rect)(ref val2)).y = scrollPos.y;
		Widgets.BeginScrollView(val, ref scrollPos, viewRect);
		listing_Standard.Begin(viewRect);
		DoAnimalHeader(listing_Standard.GetRect(24f), listing_Standard.GetRect(24f));
		listing_Standard.Gap(6f);
		int num = 0;
		foreach (AutoSlaughterConfig item in configsOrdered)
		{
			Rect rect = listing_Standard.GetRect(24f);
			if (((Rect)(ref rect)).Overlaps(val2))
			{
				DoAnimalRow(rect, item, num);
			}
			listing_Standard.Gap(6f);
			num++;
		}
		listing_Standard.End();
		Widgets.EndScrollView();
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref inRect)).x + ((Rect)(ref inRect)).width / 2f + Window.CloseButSize.x / 2f + 10f, ((Rect)(ref inRect)).y + ((Rect)(ref inRect)).height - 35f, 395f, 50f);
		((Rect)(ref rect2)).yMax = ((Rect)(ref inRect)).yMax;
		((Rect)(ref rect2)).xMax = ((Rect)(ref inRect)).xMax;
		Color color = GUI.color;
		GameFont font = Text.Font;
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)3;
		GUI.color = Color.gray;
		Text.Font = GameFont.Tiny;
		if (Text.TinyFontSupported)
		{
			Widgets.Label(rect2, "AutoSlaugtherTip".Translate());
		}
		else
		{
			Widgets.Label(rect2, "AutoSlaugtherTip".Translate().Truncate(((Rect)(ref rect2)).width));
			TooltipHandler.TipRegion(rect2, "AutoSlaugtherTip".Translate());
		}
		Text.Font = font;
		Text.Anchor = anchor;
		GUI.color = color;
	}

	private void CountPlayerAnimals(Map map, AutoSlaughterConfig config, ThingDef animal, out int currentMales, out int currentMalesYoung, out int currentFemales, out int currentFemalesYoung, out int currentTotal, out int currentPregnant, out int currentBonded)
	{
		currentMales = (currentMalesYoung = (currentFemales = (currentFemalesYoung = (currentTotal = (currentPregnant = (currentBonded = 0))))));
		foreach (Pawn spawnedColonyAnimal in map.mapPawns.SpawnedColonyAnimals)
		{
			if (spawnedColonyAnimal.def != animal || !AutoSlaughterManager.CanEverAutoSlaughter(spawnedColonyAnimal))
			{
				continue;
			}
			if (spawnedColonyAnimal.relations.GetDirectRelationsCount(PawnRelationDefOf.Bond) > 0)
			{
				currentBonded++;
				if (!config.allowSlaughterBonded)
				{
					continue;
				}
			}
			if (spawnedColonyAnimal.gender == Gender.Male)
			{
				if (spawnedColonyAnimal.ageTracker.CurLifeStage.reproductive)
				{
					currentMales++;
				}
				else
				{
					currentMalesYoung++;
				}
			}
			else if (spawnedColonyAnimal.gender == Gender.Female)
			{
				if (spawnedColonyAnimal.ageTracker.CurLifeStage.reproductive)
				{
					Hediff firstHediffOfDef = spawnedColonyAnimal.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
					if (firstHediffOfDef != null && firstHediffOfDef.Visible)
					{
						currentPregnant++;
						if (!config.allowSlaughterPregnant)
						{
							continue;
						}
						currentFemales++;
					}
					else
					{
						currentFemales++;
					}
				}
				else
				{
					currentFemalesYoung++;
				}
			}
			currentTotal++;
		}
	}

	private float CalculateLabelWidth(Rect rect)
	{
		float num = 64f;
		return ((Rect)(ref rect)).width - 24f - 4f - 4f - num * 7f - 420f - 32f;
	}

	private void DoMaxColumn(WidgetRow row, ref int val, ref string buffer, int current)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		int num = val;
		if (val == -1)
		{
			float num2 = 68f;
			float width = (60f - num2) / 2f;
			row.Gap(width);
			if (row.ButtonIconWithBG(TexButton.Infinity, 48f, "AutoSlaughterTooltipSetLimit".Translate()))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				val = current;
			}
			row.Gap(width);
		}
		else
		{
			row.CellGap = 0f;
			row.Gap(-4f);
			row.TextFieldNumeric<int>(ref val, ref buffer, 40f);
			val = Mathf.Max(0, val);
			if (row.ButtonIcon(TexButton.CloseXSmall, null, Color.white, null, null, doMouseoverSound: true, 16f))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				val = -1;
				buffer = null;
			}
			row.CellGap = 4f;
			row.Gap(4f);
		}
		if (num != val)
		{
			map.autoSlaughterManager.Notify_ConfigChanged();
		}
	}

	private void DoAnimalHeader(Rect rect1, Rect rect2)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		float width = CalculateLabelWidth(rect1);
		Widgets.BeginGroup(new Rect(((Rect)(ref rect1)).x, ((Rect)(ref rect1)).y, ((Rect)(ref rect1)).width, ((Rect)(ref rect1)).height + ((Rect)(ref rect2)).height + 1f));
		int num = 0;
		foreach (Rect tmpGroupRect in tmpGroupRects)
		{
			Rect current = tmpGroupRect;
			if (num % 2 == 1)
			{
				Widgets.DrawLightHighlight(current);
				Widgets.DrawLightHighlight(current);
			}
			else
			{
				Widgets.DrawLightHighlight(current);
			}
			GUI.color = Color.gray;
			if (num > 0)
			{
				Widgets.DrawLineVertical(((Rect)(ref current)).xMin, 0f, ((Rect)(ref rect1)).height + ((Rect)(ref rect2)).height + 1f);
			}
			if (num < tmpGroupRects.Count - 1)
			{
				Widgets.DrawLineVertical(((Rect)(ref current)).xMax, 0f, ((Rect)(ref rect1)).height + ((Rect)(ref rect2)).height + 1f);
			}
			GUI.color = Color.white;
			num++;
		}
		foreach (Rect tmpMouseoverHighlightRect in tmpMouseoverHighlightRects)
		{
			Widgets.DrawHighlightIfMouseover(tmpMouseoverHighlightRect);
		}
		Widgets.EndGroup();
		tmpMouseoverHighlightRects.Clear();
		tmpGroupRects.Clear();
		Widgets.BeginGroup(rect1);
		WidgetRow row = new WidgetRow(0f, 0f);
		TextAnchor anchor = Text.Anchor;
		Text.Anchor = (TextAnchor)4;
		row.Label(string.Empty, 24f);
		float startX = row.FinalX;
		row.Label(string.Empty, width, "AutoSlaugtherHeaderTooltipLabel".Translate());
		Rect item = default(Rect);
		((Rect)(ref item))._002Ector(startX, ((Rect)(ref rect1)).height, row.FinalX - startX, ((Rect)(ref rect2)).height);
		tmpMouseoverHighlightRects.Add(item);
		tmpGroupRects.Add(item);
		AddCurrentAndMaxEntries("AutoSlaugtherHeaderColTotal");
		AddCurrentAndMaxEntries("AnimalMaleAdult");
		AddCurrentAndMaxEntries("AnimalMaleYoung");
		AddCurrentAndMaxEntries("AnimalFemaleAdult");
		AddCurrentAndMaxEntries("AnimalFemaleYoung");
		AddCurrentAndMaxEntries("AnimalPregnant", 0f, 16f);
		AddCurrentAndMaxEntries("AnimalBonded", 0f, 16f);
		Text.Anchor = anchor;
		Widgets.EndGroup();
		Widgets.BeginGroup(rect2);
		WidgetRow widgetRow = new WidgetRow(0f, 0f);
		TextAnchor anchor2 = Text.Anchor;
		Text.Anchor = (TextAnchor)4;
		widgetRow.Label(string.Empty, 24f);
		widgetRow.Label("AutoSlaugtherHeaderColLabel".Translate(), width, "AutoSlaugtherHeaderTooltipLabel".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaugtherHeaderTooltipCurrentTotal".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColMax".Translate(), 56f, "AutoSlaugtherHeaderTooltipMaxTotal".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaugtherHeaderTooltipCurrentMales".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColMax".Translate(), 56f, "AutoSlaugtherHeaderTooltipMaxMales".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaughterHeaderTooltipCurrentMalesYoung".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColMax".Translate(), 56f, "AutoSlaughterHeaderTooltipMaxMalesYoung".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaugtherHeaderTooltipCurrentFemales".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColMax".Translate(), 56f, "AutoSlaugtherHeaderTooltipMaxFemales".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaugtherHeaderTooltipCurrentFemalesYoung".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColMax".Translate(), 56f, "AutoSlaughterHeaderTooltipMaxFemalesYoung".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaughterHeaderTooltipCurrentPregnant".Translate());
		widgetRow.Label("AllowSlaughter".Translate(), 72f, "AutoSlaughterHeaderTooltipAllowSlaughterPregnant".Translate());
		widgetRow.Label("AutoSlaugtherHeaderColCurrent".Translate(), 60f, "AutoSlaughterHeaderTooltipCurrentBonded".Translate());
		widgetRow.Label("AllowSlaughter".Translate(), 72f, "AutoSlaughterHeaderTooltipAllowSlaughterBonded".Translate());
		Text.Anchor = anchor2;
		Widgets.EndGroup();
		GUI.color = Color.gray;
		Widgets.DrawLineHorizontal(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y + ((Rect)(ref rect2)).height + 1f, ((Rect)(ref rect2)).width);
		GUI.color = Color.white;
		void AddCurrentAndMaxEntries(string headerKey, float extraWidthFirst = 0f, float extraWidthSecond = 0f)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			startX = row.FinalX;
			row.Label(string.Empty, 60f + extraWidthFirst);
			tmpMouseoverHighlightRects.Add(new Rect(startX, ((Rect)(ref rect1)).height, row.FinalX - startX, ((Rect)(ref rect2)).height));
			float finalX = row.FinalX;
			row.Label(string.Empty, 56f + extraWidthSecond);
			tmpMouseoverHighlightRects.Add(new Rect(finalX, ((Rect)(ref rect1)).height, row.FinalX - finalX, ((Rect)(ref rect2)).height));
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(startX, 0f, row.FinalX - startX, ((Rect)(ref rect2)).height);
			Widgets.Label(val, headerKey.Translate());
			tmpGroupRects.Add(val);
		}
	}

	private void DoAnimalRow(Rect rect, AutoSlaughterConfig config, int index)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(rect);
		}
		Color color = GUI.color;
		AnimalCountRecord animalCountRecord = animalCounts[config.animal];
		float width = CalculateLabelWidth(rect);
		Widgets.BeginGroup(rect);
		WidgetRow row = new WidgetRow(0f, 0f);
		row.DefIcon(config.animal);
		row.Gap(4f);
		GUI.color = ((animalCountRecord.total == 0) ? Color.gray : color);
		row.Label(config.animal.LabelCap.Truncate(width), width, GetTipForAnimal());
		GUI.color = color;
		DrawCurrentCol(animalCountRecord.total, config.maxTotal);
		DoMaxColumn(row, ref config.maxTotal, ref config.uiMaxTotalBuffer, animalCountRecord.total);
		DrawCurrentCol(animalCountRecord.male, config.maxMales);
		DoMaxColumn(row, ref config.maxMales, ref config.uiMaxMalesBuffer, animalCountRecord.male);
		DrawCurrentCol(animalCountRecord.maleYoung, config.maxMalesYoung);
		DoMaxColumn(row, ref config.maxMalesYoung, ref config.uiMaxMalesYoungBuffer, animalCountRecord.maleYoung);
		DrawCurrentCol(animalCountRecord.female, config.maxFemales);
		DoMaxColumn(row, ref config.maxFemales, ref config.uiMaxFemalesBuffer, animalCountRecord.female);
		DrawCurrentCol(animalCountRecord.femaleYoung, config.maxFemalesYoung);
		DoMaxColumn(row, ref config.maxFemalesYoung, ref config.uiMaxFemalesYoungBuffer, animalCountRecord.femaleYoung);
		Text.Anchor = (TextAnchor)4;
		row.Label(animalCountRecord.pregnant.ToString(), 60f);
		Text.Anchor = (TextAnchor)0;
		bool allowSlaughterPregnant = config.allowSlaughterPregnant;
		row.Gap(26f);
		Widgets.Checkbox(row.FinalX, 0f, ref config.allowSlaughterPregnant, 24f, disabled: false, paintable: true);
		if (allowSlaughterPregnant != config.allowSlaughterPregnant)
		{
			RecalculateAnimals();
		}
		row.Gap(52f);
		Text.Anchor = (TextAnchor)4;
		row.Label(animalCountRecord.bonded.ToString(), 60f);
		Text.Anchor = (TextAnchor)0;
		row.Gap(24f);
		bool allowSlaughterBonded = config.allowSlaughterBonded;
		Widgets.Checkbox(row.FinalX, 0f, ref config.allowSlaughterBonded, 24f, disabled: false, paintable: true);
		if (allowSlaughterBonded != config.allowSlaughterBonded)
		{
			RecalculateAnimals();
		}
		Widgets.EndGroup();
		static string DevTipPartForPawn(Pawn pawn)
		{
			string text = pawn.LabelShortCap + " " + pawn.gender.GetLabel() + " (" + pawn.ageTracker.AgeBiologicalYears + "y)";
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant);
			if (firstHediffOfDef != null)
			{
				text = text + ", pregnant (" + firstHediffOfDef.Severity.ToStringPercent() + ")";
			}
			return text;
		}
		void DrawCurrentCol(int val, int? limit = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Color? val2 = null;
			if (val == 0)
			{
				val2 = Color.gray;
			}
			else if (limit.HasValue && limit != -1 && val > limit)
			{
				val2 = ColorLibrary.RedReadable;
			}
			Color color2 = GUI.color;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = (TextAnchor)4;
			GUI.color = (Color)(((_003F?)val2) ?? Color.white);
			row.Label(val.ToString(), 60f);
			Text.Anchor = anchor;
			GUI.color = color2;
		}
		string GetTipForAnimal()
		{
			TaggedString labelCap = config.animal.LabelCap;
			if (Prefs.DevMode)
			{
				labelCap += "\n\nDEV: Animals to slaughter:\n" + map.autoSlaughterManager.AnimalsToSlaughter.Where((Pawn x) => x.def == config.animal).Select(DevTipPartForPawn).ToLineList("  - ");
			}
			return labelCap;
		}
	}
}
