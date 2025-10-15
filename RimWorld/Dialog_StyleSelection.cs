using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_StyleSelection : Window
{
	private Color baseColor = Color.white;

	private static readonly List<ThingStyleCategoryWithPriority> tmpStyles = new List<ThingStyleCategoryWithPriority>();

	private const float FadeStartMouseDist = 5f;

	private const float FadeFinishMouseDist = 150f;

	private const int MaxButtonCount = 3;

	public override Vector2 InitialSize
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(Text.CalcSize("StylesInUse".Translate()).x, 84f) + 10f + Margin * 2f;
			float num2 = Text.LineHeightOf(GameFont.Small) + 20f + 24f + Margin * 2f;
			return new Vector2(num, num2);
		}
	}

	protected override float Margin => 4f;

	public Dialog_StyleSelection()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		closeOnAccept = false;
		closeOnCancel = false;
		doWindowBackground = false;
		drawShadow = false;
		closeOnClickedOutside = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		UpdateBaseColor();
		GUI.color = baseColor;
		Text.Font = GameFont.Small;
		Widgets.DrawWindowBackground(inRect, baseColor);
		GUI.BeginGroup(inRect.ContractedBy(Margin));
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref inRect)).width, Text.LineHeight), "StylesInUse".Translate());
		float num = 0f;
		Rect val = default(Rect);
		for (int i = 0; i < Find.IdeoManager.selectedStyleCategories.Count; i++)
		{
			StyleCategoryDef cat2 = Find.IdeoManager.selectedStyleCategories[i];
			((Rect)(ref val))._002Ector(num, Text.LineHeight + 10f, 24f, 24f);
			if (Widgets.ButtonImage(val, cat2.Icon, baseColor))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (StyleCategoryDef allDef in DefDatabase<StyleCategoryDef>.AllDefs)
				{
					StyleCategoryDef category = allDef;
					if (category != cat2 && !Find.IdeoManager.selectedStyleCategories.Contains(allDef))
					{
						list.Add(new FloatMenuOption(category.LabelCap, delegate
						{
							Find.IdeoManager.selectedStyleCategories.Replace(cat2, category);
							ClearCaches();
						}, category.Icon, Color.white));
					}
				}
				list.Add(new FloatMenuOption("Remove".Translate().CapitalizeFirst(), delegate
				{
					Find.IdeoManager.selectedStyleCategories.Remove(cat2);
					ClearCaches();
				}));
				Find.WindowStack.Add(new FloatMenu(list));
			}
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
				TooltipHandler.TipRegion(val, IdeoUIUtility.StyleTooltip(cat2, IdeoEditMode.GameStart, null, tmpStyles, skipDominanceDesc: true));
			}
			num += 28f;
		}
		if (Find.IdeoManager.selectedStyleCategories.Count < 3)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(num, Text.LineHeight + 10f, 24f, 24f);
			if (Widgets.ButtonImage(val2, TexButton.Plus, baseColor))
			{
				List<FloatMenuOption> list2 = new List<FloatMenuOption>();
				foreach (StyleCategoryDef allDef2 in DefDatabase<StyleCategoryDef>.AllDefs)
				{
					StyleCategoryDef cat = allDef2;
					if (!Find.IdeoManager.selectedStyleCategories.Contains(cat))
					{
						list2.Add(new FloatMenuOption(cat.LabelCap, delegate
						{
							Find.IdeoManager.selectedStyleCategories.Add(cat);
							ClearCaches();
						}, cat.Icon, Color.white));
					}
				}
				if (list2.Any())
				{
					Find.WindowStack.Add(new FloatMenu(list2));
				}
			}
			if (Mouse.IsOver(val2))
			{
				Widgets.DrawHighlight(val2);
				TooltipHandler.TipRegion(val2, "AddStyleCategory".Translate().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "StyleCategoryDescriptionAbstract".Translate().Colorize(ColoredText.SubtleGrayColor) + "\n\n" + "ClickToEdit".Translate().CapitalizeFirst().Colorize(ColorLibrary.Green));
			}
		}
		GUI.EndGroup();
		GUI.color = Color.white;
	}

	private void ClearCaches()
	{
		Faction.OfPlayer.ideos.PrimaryIdeo.style.ResetStylesForThingDef();
		Faction.OfPlayer.ideos.PrimaryIdeo.RecachePossibleBuildables();
		foreach (DesignationCategoryDef allDef in DefDatabase<DesignationCategoryDef>.AllDefs)
		{
			allDef.DirtyCache();
		}
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialSize = InitialSize;
		Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
		windowRect = GenUI.Rounded(new Rect(mousePositionOnUIInverted.x, mousePositionOnUIInverted.y - initialSize.y, initialSize.x, initialSize.y));
	}

	private void UpdateBaseColor()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (Find.WindowStack.FloatMenu != null)
		{
			baseColor = Color.white;
			return;
		}
		Rect r = GenUI.ExpandedBy(new Rect(0f, 0f, InitialSize.x, InitialSize.y), 5f);
		if (!((Rect)(ref r)).Contains(Event.current.mousePosition))
		{
			float num = GenUI.DistFromRect(r, Event.current.mousePosition);
			baseColor = new Color(1f, 1f, 1f, 1f - num / 145f);
			if (num > 145f)
			{
				Close(doCloseSound: false);
			}
		}
	}
}
