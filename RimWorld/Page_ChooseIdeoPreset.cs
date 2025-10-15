using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Page_ChooseIdeoPreset : Page
{
	private enum PresetSelection
	{
		Classic,
		CustomFluid,
		CustomFixed,
		Load,
		Preset
	}

	private IdeoPresetDef selectedIdeo;

	private Ideo classicIdeo;

	private MemeDef selectedStructure;

	private List<StyleCategoryDef> selectedStyles = new List<StyleCategoryDef> { null };

	private List<ThingStyleCategoryWithPriority> selectedStylesWithPriority = new List<ThingStyleCategoryWithPriority> { null };

	private PresetSelection presetSelection;

	private Vector2 leftScrollPosition;

	private float totalCategoryListHeight;

	private string lastCategoryGroupLabel;

	private Texture2D randomStructureIcon;

	private const float IdeosRowWidth = 620f;

	private const float CategoryDescRowWidth = 300f;

	private const float CategoryDescRowWidthSmall = 500f;

	private const float CategoryRowWidth = 937f;

	private const float MemeIconSize = 50f;

	private const float IdeoBoxMargin = 5f;

	private const float IdeoBoxWidthMin = 110f;

	private static readonly Vector2 ButtonSize = new Vector2(160f, 60f);

	private static readonly Vector2 ButtonSizeSmall = new Vector2(120f, 40f);

	private static readonly Texture2D PlusTex = ContentFinder<Texture2D>.Get("UI/Buttons/Plus");

	private List<int> ideosPerRow = new List<int>();

	public override string PageTitle => "ChooseYourIdeoligion".Translate();

	public Texture2D RandomIcon
	{
		get
		{
			if ((Object)(object)randomStructureIcon == (Object)null)
			{
				randomStructureIcon = ContentFinder<Texture2D>.Get("UI/Structures/Random");
			}
			return randomStructureIcon;
		}
	}

	public override void PostOpen()
	{
		base.PostOpen();
		IdeoGenerationParms genParms = new IdeoGenerationParms(Find.FactionManager.OfPlayer.def);
		if (!DefDatabase<CultureDef>.AllDefs.Where((CultureDef x) => Find.FactionManager.OfPlayer.def.allowedCultures.Contains(x)).TryRandomElement(out var result))
		{
			result = DefDatabase<CultureDef>.AllDefs.RandomElement();
		}
		classicIdeo = IdeoGenerator.GenerateClassicIdeo(result, genParms, noExpansionIdeo: false);
		Find.IdeoManager.classicMode = false;
		foreach (Faction allFaction in Find.FactionManager.AllFactions)
		{
			if (allFaction != Faction.OfPlayer && allFaction.ideos != null && allFaction.ideos.PrimaryIdeo.memes.NullOrEmpty())
			{
				FactionDef def = allFaction.def;
				if (allFaction.def.fixedIdeo)
				{
					IdeoGenerationParms parms = new IdeoGenerationParms(def, forceNoExpansionIdeo: false, null, null, name: def.ideoName, styles: def.styles, deities: def.deityPresets, hidden: def.hiddenIdeo, description: def.ideoDescription, forcedMemes: def.forcedMemes, classicExtra: false, forceNoWeaponPreference: false, forNewFluidIdeo: false, fixedIdeo: true, requiredPreceptsOnly: def.requiredPreceptsOnly);
					allFaction.ideos.ChooseOrGenerateIdeo(parms);
				}
				else
				{
					allFaction.ideos.ChooseOrGenerateIdeo(new IdeoGenerationParms(allFaction.def));
				}
			}
		}
		AssignIdeoToPlayer(classicIdeo);
		Faction.OfPlayer.ideos.SetPrimary(classicIdeo);
		Find.IdeoManager.RemoveUnusedStartingIdeos();
		if (Find.Storyteller.def.tutorialMode)
		{
			DoNext();
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		DrawPageTitle(inRect);
		float num = 0f;
		Rect mainRect = GetMainRect(inRect);
		TaggedString taggedString = "ChooseYourIdeoligionDesc".Translate();
		float num2 = Text.CalcHeight(taggedString, ((Rect)(ref mainRect)).width);
		Rect rect = mainRect;
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + num;
		Widgets.Label(rect, taggedString);
		num += num2 + 10f;
		DrawStructureAndStyleSelection(inRect);
		Rect outRect = mainRect;
		((Rect)(ref outRect)).width = 954f;
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + num;
		float num3 = (InitialSize.x - 937f) / 2f;
		float num4 = (((Rect)(ref inRect)).width - ButtonSize.x - 10f - 500f - 16f) / 2f - num3;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f - num3, 0f, 921f, totalCategoryListHeight + 100f);
		Widgets.BeginScrollView(outRect, ref leftScrollPosition, viewRect);
		num = 0f;
		lastCategoryGroupLabel = "";
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(num4, num + Text.LineHeight, ButtonSize.x, ButtonSize.y);
		DrawSplitCategoryInfo(IdeoPresetCategoryDefOf.Classic, val);
		DrawSelectable(val, "PlayClassic".Translate(), null, (TextAnchor)4, presetSelection == PresetSelection.Classic, tutorAllows: true, null, delegate
		{
			selectedIdeo = null;
			presetSelection = PresetSelection.Classic;
		});
		num = Mathf.Max(0f, ((Rect)(ref val)).yMax) + Text.LineHeight;
		Widgets.Label(new Rect(0f, num, 300f, Text.LineHeight), "CustomIdeoligions".Translate());
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		Widgets.DrawLineHorizontal(0f, num + Text.LineHeight + 2f, 901f);
		GUI.color = Color.white;
		num += 12f;
		float num5 = num;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(num4, num + Text.LineHeight, ButtonSize.x, ButtonSize.y);
		DrawSplitCategoryInfo(IdeoPresetCategoryDefOf.Fluid, val2);
		DrawSelectable(val2, "CreateCustomFluid".Translate(), null, (TextAnchor)4, presetSelection == PresetSelection.CustomFluid, tutorAllows: true, null, delegate
		{
			selectedIdeo = null;
			presetSelection = PresetSelection.CustomFluid;
		});
		float num6 = Mathf.Max(num5, ((Rect)(ref val2)).yMax);
		num = num6 + 10f;
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(num4, num + Text.LineHeight, ButtonSize.x, ButtonSize.y);
		DrawSplitCategoryInfo(IdeoPresetCategoryDefOf.Custom, val3);
		DrawSelectable(val3, "CreateCustomFixed".Translate(), null, (TextAnchor)4, presetSelection == PresetSelection.CustomFixed, tutorAllows: true, null, delegate
		{
			selectedIdeo = null;
			presetSelection = PresetSelection.CustomFixed;
		});
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val3)).xMax - ButtonSizeSmall.x, ((Rect)(ref val3)).yMax + 10f, ButtonSizeSmall.x, ButtonSizeSmall.y);
		DrawSelectable(rect2, "LoadSaved".Translate() + "...", null, (TextAnchor)4, presetSelection == PresetSelection.Load, tutorAllows: true, null, delegate
		{
			selectedIdeo = null;
			presetSelection = PresetSelection.Load;
		});
		num = Mathf.Max(num6, ((Rect)(ref rect2)).yMax) + 10f;
		foreach (IdeoPresetCategoryDef item in DefDatabase<IdeoPresetCategoryDef>.AllDefsListForReading.Where((IdeoPresetCategoryDef c) => c != IdeoPresetCategoryDefOf.Classic && c != IdeoPresetCategoryDefOf.Custom && c != IdeoPresetCategoryDefOf.Fluid))
		{
			DrawCategory(item, ref num);
		}
		totalCategoryListHeight = num;
		Widgets.EndScrollView();
		DoBottomButtons(inRect);
		static void DrawSplitCategoryInfo(IdeoPresetCategoryDef cat, Rect buttonRect)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Rect rect3 = new Rect(((Rect)(ref buttonRect)).xMax + 10f, ((Rect)(ref buttonRect)).y, 500f, ((Rect)(ref buttonRect)).height);
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect3, cat.description);
			Text.Anchor = (TextAnchor)0;
		}
	}

	private void DrawCategory(IdeoPresetCategoryDef category, ref float curY)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (category.groupLabel != lastCategoryGroupLabel)
		{
			curY += 16f;
			Widgets.Label(new Rect(0f, curY, 300f, Text.LineHeight), category.groupLabel);
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Widgets.DrawLineHorizontal(0f, curY + Text.LineHeight + 2f, 901f);
			GUI.color = Color.white;
			num += Text.LineHeight + 12f;
		}
		if (category.LabelCap != category.groupLabel)
		{
			Rect rect = new Rect(0f, curY + num, 300f, Text.LineHeight);
			num += Text.LineHeight;
			Widgets.Label(rect, category.LabelCap);
		}
		float num2 = Text.CalcHeight(category.description, 300f);
		Widgets.Label(new Rect(0f, curY + num, 300f, num2), category.description);
		curY += num;
		ideosPerRow.Clear();
		int num3 = 0;
		float num4 = 0f;
		ideosPerRow.Add(0);
		foreach (IdeoPresetDef item in DefDatabase<IdeoPresetDef>.AllDefs.Where((IdeoPresetDef i) => i.categoryDef == category))
		{
			Rect val = RectForIdeo(item);
			float num5 = ((Rect)(ref val)).width + 17f;
			if (num4 + num5 <= 620f)
			{
				ideosPerRow[num3]++;
				num4 += num5;
			}
			else
			{
				num3++;
				ideosPerRow.Add(1);
				num4 = num5;
			}
		}
		num3 = 0;
		num4 = 0f;
		int num6 = 0;
		foreach (IdeoPresetDef item2 in DefDatabase<IdeoPresetDef>.AllDefs.Where((IdeoPresetDef i) => i.categoryDef == category))
		{
			Rect val2 = RectForIdeo(item2);
			float num7 = ((Rect)(ref val2)).width + 10f;
			DrawIdeo(new Vector2(num4 + 300f + 17f, curY), item2);
			num4 += num7;
			num6++;
			if (num3 < ideosPerRow.Count && num6 >= ideosPerRow[num3])
			{
				num3++;
				num4 = 0f;
				num6 = 0;
				curY += ((Rect)(ref val2)).height + 10f;
			}
		}
		curY += 8f;
		lastCategoryGroupLabel = category.groupLabel;
	}

	private void DrawSelectable(Rect rect, string label, string tooltip, TextAnchor textAnchor, bool active, bool tutorAllows, Action iconDrawer, Action onSelect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		TextAnchor anchor = Text.Anchor;
		Widgets.DrawOptionBackground(rect, active);
		Rect rect2 = rect.ContractedBy(2f);
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 4f;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect2)).xMax - 4f;
		Text.Anchor = textAnchor;
		Widgets.Label(rect2, label);
		Text.Anchor = anchor;
		iconDrawer?.Invoke();
		if (Mouse.IsOver(rect) && !tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, new TipSignal(tooltip, 351941375, TooltipPriority.Ideo));
		}
		if (Widgets.ButtonInvisible(rect) && tutorAllows)
		{
			onSelect?.Invoke();
			SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
		}
	}

	private void DrawIdeo(Vector2 offset, IdeoPresetDef ideo)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = RectForIdeo(ideo);
		ref Rect reference = ref rect;
		((Rect)(ref reference)).x = ((Rect)(ref reference)).x + offset.x;
		ref Rect reference2 = ref rect;
		((Rect)(ref reference2)).y = ((Rect)(ref reference2)).y + offset.y;
		DrawSelectable(rect, ideo.LabelCap, ideo.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + ideo.description, Text.Anchor, selectedIdeo == ideo, !TutorSystem.TutorialMode || TutorSystem.AllowAction("IdeoPresetSelectIdeo"), delegate
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			float num = 5f;
			float num2 = 55f * (float)(((Object)(object)ideo.Icon != (Object)null) ? 1 : ideo.memes.Count) + 5f;
			float num3 = ((Rect)(ref rect)).width / 2f - num2 / 2f;
			if ((Object)(object)ideo.Icon != (Object)null)
			{
				GUI.DrawTexture(new Rect(((Rect)(ref rect)).x + num + num3, ((Rect)(ref rect)).y + Text.LineHeight + 5f, 50f, 50f), (Texture)(object)ideo.Icon);
				return;
			}
			Rect rect2 = default(Rect);
			foreach (MemeDef meme in ideo.memes)
			{
				((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x + num + num3, ((Rect)(ref rect)).y + Text.LineHeight + 5f, 50f, 50f);
				DrawMeme(rect2, meme);
				num += 55f;
			}
		}, delegate
		{
			presetSelection = PresetSelection.Preset;
			selectedIdeo = ideo;
		});
	}

	private Rect RectForIdeo(IdeoPresetDef ideo)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		float num = Text.CalcSize(ideo.LabelCap).x + 5f + 2f;
		float num2 = (float)(((Object)(object)ideo.Icon != (Object)null) ? 1 : ideo.memes.Count) * 55f;
		Rect result = default(Rect);
		((Rect)(ref result)).width = Mathf.Max(Mathf.Max(num, num2) + 5f, 110f);
		((Rect)(ref result)).height = Text.LineHeight + 50f + 10f;
		return result;
	}

	private void DrawMeme(Rect rect, MemeDef meme)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawLightHighlight(rect);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, meme.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + meme.description);
		}
		GUI.DrawTexture(rect, (Texture)(object)meme.Icon);
	}

	private void DrawStructureAndStyleSelection(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		int numStyles = selectedStyles.Count;
		bool num = numStyles < 3;
		float curX = 10f;
		if (num)
		{
			Rect val = GetRect();
			GUI.DrawTexture(val.ContractedBy(4f), (Texture)(object)PlusTex);
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
				TooltipHandler.TipRegion(val, "AddStyleCategory".Translate() + "\n\n" + "StyleCategoryDescriptionAbstract".Translate().Resolve().Colorize(ColoredText.SubtleGrayColor));
			}
			if (Widgets.ButtonInvisible(val) && (!TutorSystem.TutorialMode || TutorSystem.AllowAction("IdeoPresetEditStyle")))
			{
				List<FloatMenuOption> opts2 = new List<FloatMenuOption>();
				FillAllAvailableStyles(ref opts2);
				Find.WindowStack.Add(new FloatMenu(opts2));
			}
		}
		for (int num2 = selectedStyles.Count - 1; num2 >= 0; num2--)
		{
			StyleCategoryDef styleCategoryDef = selectedStyles[num2];
			Rect val2 = GetRect();
			GUI.DrawTexture(val2.ContractedBy(2f), (Texture)(object)((styleCategoryDef != null) ? styleCategoryDef.Icon : RandomIcon));
			if (Mouse.IsOver(val2))
			{
				Widgets.DrawHighlight(val2);
				TooltipHandler.TipRegion(val2, (styleCategoryDef != null) ? IdeoUIUtility.StyleTooltip(styleCategoryDef, IdeoEditMode.None, null, selectedStylesWithPriority) : "RandomStyleTip".Translate());
			}
			if (Widgets.ButtonInvisible(val2) && (!TutorSystem.TutorialMode || TutorSystem.AllowAction("IdeoPresetEditStyle")))
			{
				List<FloatMenuOption> opts3 = new List<FloatMenuOption>();
				FillAllAvailableStyles(ref opts3, num2);
				Find.WindowStack.Add(new FloatMenu(opts3));
			}
		}
		curX += 4f;
		TaggedString taggedString = "Styles".Translate();
		float x2 = Text.CalcSize(taggedString).x;
		curX += x2;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).xMax - curX, ((Rect)(ref rect)).yMin + 6f, x2, Text.LineHeight);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2.ExpandedBy(4f, 2f));
			TooltipHandler.TipRegion(rect2, "StyleCategoryDescriptionAbstract".Translate());
		}
		Widgets.Label(rect2, taggedString);
		curX += 17f;
		Rect val3 = GetRect();
		GUI.DrawTexture(val3.ContractedBy(2f), (Texture)(object)((selectedStructure != null) ? selectedStructure.Icon : RandomIcon));
		if (Mouse.IsOver(val3))
		{
			Widgets.DrawHighlight(val3);
			TooltipHandler.TipRegion(val3, (selectedStructure != null) ? IdeoUIUtility.StructureTooltip(selectedStructure, IdeoEditMode.None) : "RandomStructureTip".Translate());
		}
		if (Widgets.ButtonInvisible(val3) && (!TutorSystem.TutorialMode || TutorSystem.AllowAction("IdeoPresetEditStructure")))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (selectedStructure != null)
			{
				list.Add(new FloatMenuOption("Random".Translate(), delegate
				{
					selectedStructure = null;
				}, RandomIcon, Color.white));
			}
			foreach (MemeDef meme in DefDatabase<MemeDef>.AllDefsListForReading)
			{
				if (selectedStructure != meme && meme.category == MemeCategory.Structure && IdeoUtility.IsMemeAllowedFor(meme, Find.Scenario.playerFaction.factionDef))
				{
					list.Add(new FloatMenuOption(meme.LabelCap, delegate
					{
						selectedStructure = meme;
					}, meme.Icon, Color.white));
				}
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		curX += 4f;
		TaggedString taggedString2 = "Structure".Translate();
		float x3 = Text.CalcSize(taggedString2).x;
		curX += x3;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).xMax - curX, ((Rect)(ref rect)).yMin + 6f, x3, Text.LineHeight);
		if (Mouse.IsOver(rect3))
		{
			Widgets.DrawHighlight(rect3.ExpandedBy(4f, 2f));
			TooltipHandler.TipRegion(rect3, "StructureMemeTip".Translate());
		}
		Widgets.Label(rect3, taggedString2);
		void FillAllAvailableStyles(ref List<FloatMenuOption> opts, int forIndex = -1)
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			if (forIndex == -1 || selectedStyles[forIndex] != null)
			{
				opts.Add(new FloatMenuOption("Random".Translate(), delegate
				{
					if (forIndex == -1)
					{
						selectedStyles.Add(null);
					}
					else
					{
						selectedStyles[forIndex] = null;
					}
					RecacheStyleCategoriesWithPriority();
				}, RandomIcon, Color.white));
			}
			foreach (StyleCategoryDef s in DefDatabase<StyleCategoryDef>.AllDefs.Where((StyleCategoryDef x) => !x.fixedIdeoOnly && !selectedStyles.Contains(x)))
			{
				if (forIndex == -1 || selectedStyles[forIndex] != s)
				{
					opts.Add(new FloatMenuOption(s.LabelCap, delegate
					{
						if (forIndex == -1)
						{
							selectedStyles.Add(s);
						}
						else
						{
							selectedStyles[forIndex] = s;
						}
						RecacheStyleCategoriesWithPriority();
					}, s.Icon, Color.white));
				}
			}
			if (forIndex != -1 && numStyles > 1)
			{
				opts.Add(new FloatMenuOption("Remove".Translate(), delegate
				{
					selectedStyles.RemoveAt(forIndex);
					RecacheStyleCategoriesWithPriority();
				}));
			}
		}
		Rect GetRect()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			Rect result = rect;
			((Rect)(ref result)).xMax = ((Rect)(ref result)).xMax - curX;
			((Rect)(ref result)).xMin = ((Rect)(ref result)).xMax - 35f;
			((Rect)(ref result)).height = 35f;
			curX += 35f;
			return result;
		}
	}

	private void DoClassic()
	{
		foreach (Faction allFaction in Find.FactionManager.AllFactions)
		{
			if (allFaction.ideos != null)
			{
				allFaction.ideos.RemoveAll();
				allFaction.ideos.SetPrimary(classicIdeo);
			}
		}
		Find.IdeoManager.RemoveUnusedStartingIdeos();
		Find.Scenario.PostIdeoChosen();
		if (Find.Storyteller.def.tutorialMode)
		{
			next.prev = prev;
		}
		else
		{
			next.prev = this;
		}
		base.DoNext();
	}

	private void DoCustomize(bool fluid = false)
	{
		if (TutorSystem.TutorialMode && !TutorSystem.AllowAction("IdeoPresetCustomizeIdeo"))
		{
			return;
		}
		foreach (Ideo item in Find.IdeoManager.IdeosListForReading)
		{
			item.initialPlayerIdeo = false;
		}
		Faction.OfPlayer.ideos.RemoveAll();
		Find.IdeoManager.RemoveUnusedStartingIdeos();
		Page_ConfigureIdeo page_ConfigureIdeo;
		if (fluid)
		{
			page_ConfigureIdeo = new Page_ConfigureFluidIdeo();
			page_ConfigureIdeo.SelectOrMakeNewIdeo();
			page_ConfigureIdeo.ideo.Fluid = true;
		}
		else
		{
			page_ConfigureIdeo = new Page_ConfigureIdeo();
		}
		page_ConfigureIdeo.prev = this;
		page_ConfigureIdeo.next = next;
		next.prev = page_ConfigureIdeo;
		ResetSelection();
		Find.WindowStack.Add(page_ConfigureIdeo);
		Close();
	}

	private void DoLoad()
	{
		Dialog_IdeoList_Load window = new Dialog_IdeoList_Load(delegate(Ideo ideo)
		{
			AssignIdeoToPlayer(ideo);
			Find.IdeoManager.RemoveUnusedStartingIdeos();
			Find.Scenario.PostIdeoChosen();
			next.prev = this;
			ResetSelection();
			base.DoNext();
		});
		Find.WindowStack.Add(window);
	}

	private void DoPreset()
	{
		List<MemeDef> list = selectedIdeo.memes.ToList();
		MemeDef result;
		if (selectedStructure != null)
		{
			list.RemoveAll((MemeDef x) => x.category == MemeCategory.Structure);
			list.Add(selectedStructure);
		}
		else if (!list.Any((MemeDef x) => x.category == MemeCategory.Structure) && DefDatabase<MemeDef>.AllDefsListForReading.Where((MemeDef m) => m.category == MemeCategory.Structure && IdeoUtility.IsMemeAllowedFor(m, Find.Scenario.playerFaction.factionDef)).TryRandomElement(out result))
		{
			list.Add(result);
		}
		Ideo ideo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms(Find.FactionManager.OfPlayer.def, forceNoExpansionIdeo: false, null, null, list, selectedIdeo.classicPlus, forceNoWeaponPreference: true));
		ApplySelectedStylesToIdeo(ideo);
		AssignIdeoToPlayer(ideo);
		Find.IdeoManager.RemoveUnusedStartingIdeos();
		Find.Scenario.PostIdeoChosen();
		base.DoNext();
	}

	protected override void DoNext()
	{
		Find.IdeoManager.classicMode = presetSelection == PresetSelection.Classic;
		switch (presetSelection)
		{
		case PresetSelection.Classic:
			DoClassic();
			break;
		case PresetSelection.CustomFixed:
			DoCustomize();
			break;
		case PresetSelection.CustomFluid:
			DoCustomize(fluid: true);
			break;
		case PresetSelection.Load:
			DoLoad();
			break;
		case PresetSelection.Preset:
			DoPreset();
			break;
		}
	}

	private void ApplySelectedStylesToIdeo(Ideo ideo)
	{
		if (selectedStyles.Count == 1 && selectedStyles[0] == null)
		{
			ideo.foundation.RandomizeStyles();
			return;
		}
		List<StyleCategoryDef> finalizedStyles = selectedStyles.ToList();
		for (int num = finalizedStyles.Count - 1; num >= 0; num--)
		{
			if (finalizedStyles[num] == null)
			{
				if (DefDatabase<StyleCategoryDef>.AllDefs.Where((StyleCategoryDef x) => !x.fixedIdeoOnly && !finalizedStyles.Contains(x)).TryRandomElement(out var result))
				{
					finalizedStyles[num] = result;
				}
				else
				{
					finalizedStyles.RemoveAt(num);
				}
			}
		}
		ideo.thingStyleCategories.Clear();
		for (int i = 0; i < finalizedStyles.Count; i++)
		{
			StyleCategoryDef category = finalizedStyles[i];
			ideo.thingStyleCategories.Add(new ThingStyleCategoryWithPriority(category, 3 - i));
		}
		ideo.SortStyleCategories();
	}

	private void AssignIdeoToPlayer(Ideo ideo)
	{
		Faction.OfPlayer.ideos.SetPrimary(ideo);
		foreach (Ideo item in Find.IdeoManager.IdeosListForReading)
		{
			item.initialPlayerIdeo = false;
		}
		ideo.initialPlayerIdeo = true;
		Find.IdeoManager.Add(ideo);
	}

	private void RecacheStyleCategoriesWithPriority()
	{
		selectedStylesWithPriority.Clear();
		for (int i = 0; i < selectedStyles.Count; i++)
		{
			StyleCategoryDef styleCategoryDef = selectedStyles[i];
			if (styleCategoryDef != null)
			{
				selectedStylesWithPriority.Add(new ThingStyleCategoryWithPriority(styleCategoryDef, 3 - i));
			}
		}
	}

	private void ResetSelection()
	{
		selectedIdeo = null;
		selectedStructure = null;
		selectedStyles = new List<StyleCategoryDef> { null };
	}
}
