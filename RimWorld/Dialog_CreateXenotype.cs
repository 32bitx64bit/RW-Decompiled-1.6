using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_CreateXenotype : GeneCreationDialogBase
{
	private int generationRequestIndex;

	private Action callback;

	private List<GeneDef> selectedGenes = new List<GeneDef>();

	private bool inheritable;

	private bool? selectedCollapsed = false;

	private HashSet<GeneCategoryDef> matchingCategories = new HashSet<GeneCategoryDef>();

	private Dictionary<GeneCategoryDef, bool> collapsedCategories = new Dictionary<GeneCategoryDef, bool>();

	private bool hoveredAnyGene;

	private GeneDef hoveredGene;

	private static bool ignoreRestrictionsConfirmationSent;

	private Vector2 selectedScrollPosition;

	private const int MaxCustomXenotypes = 200;

	private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);

	public override Vector2 InitialSize => new Vector2((float)Mathf.Min(UI.screenWidth, 1036), (float)(UI.screenHeight - 4));

	protected override List<GeneDef> SelectedGenes => selectedGenes;

	protected override string Header => "CreateXenotype".Translate().CapitalizeFirst();

	protected override string AcceptButtonLabel => "SaveAndApply".Translate().CapitalizeFirst();

	public Dialog_CreateXenotype(int index, Action callback)
	{
		generationRequestIndex = index;
		this.callback = callback;
		xenotypeName = string.Empty;
		closeOnAccept = false;
		absorbInputAroundWindow = true;
		alwaysUseFullBiostatsTableHeight = true;
		searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
		foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
		{
			collapsedCategories.Add(allDef, value: false);
		}
		OnGenesChanged();
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckBiotech("xenotype creation"))
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
	}

	protected override void DrawGenes(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Invalid comparison between Unknown and I4
		hoveredAnyGene = false;
		GUI.BeginGroup(rect);
		float curY = 0f;
		DrawSection(new Rect(0f, 0f, ((Rect)(ref rect)).width, selectedHeight), selectedGenes, "SelectedGenes".Translate(), ref curY, ref selectedHeight, adding: false, rect, ref selectedCollapsed);
		if (!selectedCollapsed.Value)
		{
			curY += 10f;
		}
		float num = curY;
		Widgets.Label(0f, ref curY, ((Rect)(ref rect)).width, "Genes".Translate().CapitalizeFirst());
		curY += 10f;
		float num2 = curY - num - 4f;
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - 150f - 16f, num, 150f, num2), "CollapseAllCategories".Translate()))
		{
			SoundDefOf.TabClose.PlayOneShotOnCamera();
			foreach (GeneCategoryDef allDef in DefDatabase<GeneCategoryDef>.AllDefs)
			{
				collapsedCategories[allDef] = true;
			}
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - 300f - 4f - 16f, num, 150f, num2), "ExpandAllCategories".Translate()))
		{
			SoundDefOf.TabOpen.PlayOneShotOnCamera();
			foreach (GeneCategoryDef allDef2 in DefDatabase<GeneCategoryDef>.AllDefs)
			{
				collapsedCategories[allDef2] = false;
			}
		}
		float num3 = curY;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref rect)).width - 16f, scrollHeight);
		Widgets.BeginScrollView(new Rect(0f, curY, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - curY), ref scrollPosition, val);
		Rect containingRect = val;
		((Rect)(ref containingRect)).y = curY + scrollPosition.y;
		((Rect)(ref containingRect)).height = ((Rect)(ref rect)).height;
		bool? collapsed = null;
		DrawSection(rect, GeneUtility.GenesInOrder, null, ref curY, ref unselectedHeight, adding: true, containingRect, ref collapsed);
		if ((int)Event.current.type == 8)
		{
			scrollHeight = curY - num3;
		}
		Widgets.EndScrollView();
		GUI.EndGroup();
		if (!hoveredAnyGene)
		{
			hoveredGene = null;
		}
	}

	private void DrawSection(Rect rect, List<GeneDef> genes, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect, ref bool? collapsed)
	{
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Invalid comparison between Unknown and I4
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Invalid comparison between Unknown and I4
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		float curX = 4f;
		if (!label.NullOrEmpty())
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref rect)).width, Text.LineHeight);
			((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - (adding ? 16f : (Text.CalcSize("ClickToAddOrRemove".Translate()).x + 4f));
			if (collapsed.HasValue)
			{
				Rect val2 = default(Rect);
				((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y + (((Rect)(ref val)).height - 18f) / 2f, 18f, 18f);
				GUI.DrawTexture(val2, (Texture)(object)(collapsed.Value ? TexButton.Reveal : TexButton.Collapse));
				if (Widgets.ButtonInvisible(val))
				{
					collapsed = !collapsed;
					if (collapsed.Value)
					{
						SoundDefOf.TabClose.PlayOneShotOnCamera();
					}
					else
					{
						SoundDefOf.TabOpen.PlayOneShotOnCamera();
					}
				}
				if (Mouse.IsOver(val))
				{
					Widgets.DrawHighlight(val);
				}
				((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + ((Rect)(ref val2)).width;
			}
			Widgets.Label(val, label);
			if (!adding)
			{
				Text.Anchor = (TextAnchor)2;
				GUI.color = ColoredText.SubtleGrayColor;
				Widgets.Label(new Rect(((Rect)(ref val)).xMax - 18f, curY, ((Rect)(ref rect)).width - ((Rect)(ref val)).width, Text.LineHeight), "ClickToAddOrRemove".Translate());
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
			}
			curY += Text.LineHeight + 3f;
		}
		if (collapsed == true)
		{
			if ((int)Event.current.type == 8)
			{
				sectionHeight = 0f;
			}
			return;
		}
		float num = curY;
		bool flag = false;
		float num2 = 34f + GeneCreationDialogBase.GeneSize.x + 8f;
		float num3 = GeneCreationDialogBase.GeneSize.y + 8f + 4f;
		float num4 = ((Rect)(ref rect)).width - 16f;
		float num5 = num2 + 4f;
		float num6 = (num4 - num5 * Mathf.Floor(num4 / num5)) / 2f;
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, curY, ((Rect)(ref rect)).width, sectionHeight);
		if (!adding)
		{
			Widgets.DrawRectFast(val3, Widgets.MenuSectionBGFillColor);
		}
		curY += 4f;
		if (!genes.Any())
		{
			Text.Anchor = (TextAnchor)4;
			GUI.color = ColoredText.SubtleGrayColor;
			Widgets.Label(val3, "(" + "NoneLower".Translate() + ")");
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
			curY += num3;
		}
		else
		{
			float num7 = ((Rect)(ref rect)).width - 16f;
			int num8 = Mathf.FloorToInt(num7 / (num2 + 1f));
			int num9 = Mathf.CeilToInt((float)genes.Count / (float)num8);
			float num10 = Mathf.Max(new float[1] { num3 * (float)Mathf.Min(num9, 3) });
			float num11 = num3 * (float)num9;
			bool flag2 = !adding && collapsed == false && genes.Count > 0 && num11 > num10;
			float num12 = curY;
			if (flag2)
			{
				num12 = 0f;
				Rect outRect = new Rect(0f, curY, ((Rect)(ref rect)).width, num10);
				Rect viewRect = default(Rect);
				((Rect)(ref viewRect))._002Ector(0f, 0f, num7, num11);
				Widgets.BeginScrollView(outRect, ref selectedScrollPosition, viewRect);
			}
			GeneCategoryDef geneCategoryDef = null;
			int num13 = 0;
			Rect val4 = default(Rect);
			Rect val5 = default(Rect);
			for (int i = 0; i < genes.Count; i++)
			{
				GeneDef geneDef = genes[i];
				if ((adding && quickSearchWidget.filter.Active && (!matchingGenes.Contains(geneDef) || selectedGenes.Contains(geneDef)) && !matchingCategories.Contains(geneDef.displayCategory)) || (!ignoreRestrictions && geneDef.biostatArc > 0))
				{
					continue;
				}
				bool flag3 = false;
				if (curX + num2 > num4)
				{
					curX = 4f;
					num12 += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					flag3 = true;
				}
				bool flag4 = quickSearchWidget.filter.Active && (matchingGenes.Contains(geneDef) || matchingCategories.Contains(geneDef.displayCategory));
				bool flag5 = collapsedCategories[geneDef.displayCategory] && !flag4;
				if (adding && geneCategoryDef != geneDef.displayCategory)
				{
					if (!flag3 && flag)
					{
						curX = 4f;
						num12 += GeneCreationDialogBase.GeneSize.y + 8f + 4f;
					}
					geneCategoryDef = geneDef.displayCategory;
					((Rect)(ref val4))._002Ector(curX, num12, ((Rect)(ref rect)).width - 8f, Text.LineHeight);
					if (!flag4)
					{
						((Rect)(ref val5))._002Ector(((Rect)(ref val4)).x, ((Rect)(ref val4)).y + (((Rect)(ref val4)).height - 18f) / 2f, 18f, 18f);
						GUI.DrawTexture(val5, (Texture)(object)(flag5 ? TexButton.Reveal : TexButton.Collapse));
						if (Widgets.ButtonInvisible(val4))
						{
							collapsedCategories[geneDef.displayCategory] = !collapsedCategories[geneDef.displayCategory];
							if (collapsedCategories[geneDef.displayCategory])
							{
								SoundDefOf.TabClose.PlayOneShotOnCamera();
							}
							else
							{
								SoundDefOf.TabOpen.PlayOneShotOnCamera();
							}
						}
						if (num13 % 2 == 1)
						{
							Widgets.DrawLightHighlight(val4);
						}
						if (Mouse.IsOver(val4))
						{
							Widgets.DrawHighlight(val4);
						}
						((Rect)(ref val4)).xMin = ((Rect)(ref val4)).xMin + ((Rect)(ref val5)).width;
					}
					Widgets.Label(val4, geneCategoryDef.LabelCap);
					num12 += ((Rect)(ref val4)).height;
					if (!flag5)
					{
						GUI.color = Color.grey;
						Widgets.DrawLineHorizontal(curX, num12, ((Rect)(ref rect)).width - 8f);
						GUI.color = Color.white;
						num12 += 10f;
					}
					num13++;
				}
				if (adding && flag5)
				{
					flag = false;
					if ((int)Event.current.type == 8)
					{
						sectionHeight = num12 - num;
					}
					continue;
				}
				curX = Mathf.Max(curX, num6);
				flag = true;
				if (DrawGene(geneDef, !adding, ref curX, num12, num2, containingRect, flag4))
				{
					if (selectedGenes.Contains(geneDef))
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						selectedGenes.Remove(geneDef);
					}
					else
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						selectedGenes.Add(geneDef);
					}
					if (!xenotypeNameLocked)
					{
						xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
					}
					OnGenesChanged();
					break;
				}
			}
			num12 += num3;
			if (flag2)
			{
				Widgets.EndScrollView();
				curY += num10;
			}
			else
			{
				curY = num12;
			}
		}
		if ((int)Event.current.type == 8)
		{
			sectionHeight = curY - num;
		}
	}

	private bool DrawGene(GeneDef geneDef, bool selectedSection, ref float curX, float curY, float packWidth, Rect containingRect, bool isMatch)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
		if (!((Rect)(ref containingRect)).Overlaps(val))
		{
			curX = ((Rect)(ref val)).xMax + 4f;
			return false;
		}
		bool selected = !selectedSection && selectedGenes.Contains(geneDef);
		bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef));
		Widgets.DrawOptionBackground(val, selected);
		curX += 4f;
		GeneUIUtility.DrawBiostats(geneDef.biostatCpx, geneDef.biostatMet, geneDef.biostatArc, ref curX, curY, 4f);
		Rect geneRect = default(Rect);
		((Rect)(ref geneRect))._002Ector(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
		GeneUIUtility.DrawGeneDef(geneDef, geneRect, (!inheritable) ? GeneType.Xenogene : GeneType.Endogene, () => GeneTip(geneDef, selectedSection), doBackground: false, clickable: false, overridden);
		curX += GeneCreationDialogBase.GeneSize.x + 4f;
		if (Mouse.IsOver(val))
		{
			hoveredGene = geneDef;
			hoveredAnyGene = true;
		}
		else if (hoveredGene != null && geneDef.ConflictsWith(hoveredGene))
		{
			Widgets.DrawLightHighlight(val);
		}
		if (Widgets.ButtonInvisible(val))
		{
			result = true;
		}
		curX = Mathf.Max(curX, ((Rect)(ref val)).xMax + 4f);
		return result;
	}

	private string GeneTip(GeneDef geneDef, bool selectedSection)
	{
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		if (selectedSection)
		{
			if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == geneDef))
			{
				text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == geneDef));
			}
			else if (cachedOverriddenGenes.Contains(geneDef))
			{
				text = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(geneDef)));
			}
			else if (randomChosenGroups.ContainsKey(geneDef))
			{
				text = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[geneDef].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
			}
		}
		if (selectedGenes.Contains(geneDef) && geneDef.prerequisite != null && !selectedGenes.Contains(geneDef.prerequisite))
		{
			if (!text.NullOrEmpty())
			{
				text += "\n\n";
			}
			text += ("MessageGeneMissingPrerequisite".Translate(geneDef.label).CapitalizeFirst() + ": " + geneDef.prerequisite.LabelCap).Colorize(ColorLibrary.RedReadable);
		}
		if (!text.NullOrEmpty())
		{
			text += "\n\n";
		}
		return text + (selectedGenes.Contains(geneDef) ? "ClickToRemove" : "ClickToAdd").Translate().Colorize(ColoredText.SubtleGrayColor);
		static string GroupInfo(GeneLeftChosenGroup group)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (group == null)
			{
				return null;
			}
			return ("GeneLeftmostActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
		}
	}

	protected override void PostXenotypeOnGUI(float curX, float curY)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		TaggedString taggedString = "GenesAreInheritable".Translate();
		TaggedString taggedString2 = "IgnoreRestrictions".Translate();
		float num = Mathf.Max(Text.CalcSize(taggedString).x, Text.CalcSize(taggedString2).x) + 4f + 24f;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, num, Text.LineHeight);
		Widgets.CheckboxLabeled(rect, taggedString, ref inheritable);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "GenesAreInheritableDesc".Translate());
		}
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + Text.LineHeight;
		bool num2 = ignoreRestrictions;
		Widgets.CheckboxLabeled(rect, taggedString2, ref ignoreRestrictions);
		if (num2 != ignoreRestrictions)
		{
			if (ignoreRestrictions)
			{
				if (!ignoreRestrictionsConfirmationSent)
				{
					ignoreRestrictionsConfirmationSent = true;
					Find.WindowStack.Add(new Dialog_MessageBox("IgnoreRestrictionsConfirmation".Translate(), "Yes".Translate(), delegate
					{
					}, "No".Translate(), delegate
					{
						ignoreRestrictions = false;
					}));
				}
			}
			else
			{
				selectedGenes.RemoveAll((GeneDef x) => x.biostatArc > 0);
				OnGenesChanged();
			}
		}
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "IgnoreRestrictionsDesc".Translate());
		}
		postXenotypeHeight += ((Rect)(ref rect)).yMax - curY;
	}

	protected override void OnGenesChanged()
	{
		selectedGenes.SortGeneDefs();
		base.OnGenesChanged();
	}

	protected override void DrawSearchRect(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSearchRect(rect);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadCustom".Translate()))
		{
			Find.WindowStack.Add(new Dialog_XenotypeList_Load(delegate(CustomXenotype xenotype)
			{
				xenotypeName = xenotype.name;
				xenotypeNameLocked = true;
				selectedGenes.Clear();
				selectedGenes.AddRange(xenotype.genes);
				inheritable = xenotype.inheritable;
				iconDef = xenotype.IconDef;
				OnGenesChanged();
				ignoreRestrictions = selectedGenes.Any((GeneDef x) => x.biostatArc > 0) || !WithinAcceptableBiostatLimits(showMessage: false);
			}));
		}
		if (!Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x * 2f - 4f, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadPremade".Translate()))
		{
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		foreach (XenotypeDef item in DefDatabase<XenotypeDef>.AllDefs.OrderBy((XenotypeDef c) => 0f - c.displayPriority))
		{
			XenotypeDef xenotype2 = item;
			list.Add(new FloatMenuOption(xenotype2.LabelCap, delegate
			{
				xenotypeName = xenotype2.label;
				selectedGenes.Clear();
				selectedGenes.AddRange(xenotype2.genes);
				inheritable = xenotype2.inheritable;
				OnGenesChanged();
				ignoreRestrictions = selectedGenes.Any((GeneDef g) => g.biostatArc > 0) || !WithinAcceptableBiostatLimits(showMessage: false);
			}, xenotype2.Icon, XenotypeDef.IconColor, MenuOptionPriority.Default, delegate(Rect r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				TooltipHandler.TipRegion(r, xenotype2.descriptionShort ?? xenotype2.description);
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list));
	}

	protected override void DoBottomButtons(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		base.DoBottomButtons(rect);
		if (leftChosenGroups.Any())
		{
			int num = leftChosenGroups.Sum((GeneLeftChosenGroup x) => x.overriddenGenes.Count);
			GeneLeftChosenGroup geneLeftChosenGroup = leftChosenGroups[0];
			string text = "GenesConflict".Translate() + ": " + "GenesConflictDesc".Translate(geneLeftChosenGroup.leftChosen.Named("FIRST"), geneLeftChosenGroup.overriddenGenes[0].Named("SECOND")).CapitalizeFirst() + ((num > 1) ? (" +" + (num - 1)) : string.Empty);
			float x2 = Text.CalcSize(text).x;
			GUI.color = ColorLibrary.RedReadable;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x - x2 - 4f, ((Rect)(ref rect)).y, x2, ((Rect)(ref rect)).height), text);
			Text.Anchor = (TextAnchor)0;
			GUI.color = Color.white;
		}
	}

	protected override bool CanAccept()
	{
		if (!base.CanAccept())
		{
			return false;
		}
		if (!selectedGenes.Any())
		{
			Messages.Message("MessageNoSelectedGenes".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (GenFilePaths.AllCustomXenotypeFiles.EnumerableCount() >= 200)
		{
			Messages.Message("MessageTooManyCustomXenotypes", null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (!ignoreRestrictions && leftChosenGroups.Any())
		{
			Messages.Message("MessageConflictingGenesPresent".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (generationRequestIndex >= 0)
		{
			PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(generationRequestIndex);
			if (TutorSystem.TutorialMode && generationRequest.MustBeCapableOfViolence && selectedGenes.Any((GeneDef g) => g.disabledWorkTags.HasFlag(WorkTags.Violent)))
			{
				Messages.Message("MessageStartingPawnCapableOfViolence".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
		}
		return true;
	}

	protected override void Accept()
	{
		IEnumerable<string> warnings = GetWarnings();
		if (warnings.Any())
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("XenotypeBreaksLimits".Translate() + ":\n" + warnings.ToLineList("  - ", capitalizeItems: true) + "\n\n" + "SaveAnyway".Translate(), AcceptInner));
		}
		else
		{
			AcceptInner();
		}
	}

	private void AcceptInner()
	{
		CustomXenotype customXenotype = new CustomXenotype();
		customXenotype.name = xenotypeName?.Trim();
		customXenotype.genes.AddRange(selectedGenes);
		customXenotype.inheritable = inheritable;
		customXenotype.iconDef = iconDef;
		string text = GenFile.SanitizedFileName(customXenotype.name);
		string absPath = GenFilePaths.AbsFilePathForXenotype(text);
		LongEventHandler.QueueLongEvent(delegate
		{
			GameDataSaveLoader.SaveXenotype(customXenotype, absPath);
		}, "SavingLongEvent", doAsynchronously: false, null);
		if (generationRequestIndex >= 0)
		{
			PawnGenerationRequest generationRequest = StartingPawnUtility.GetGenerationRequest(generationRequestIndex);
			generationRequest.ForcedXenotype = null;
			generationRequest.ForcedCustomXenotype = customXenotype;
			StartingPawnUtility.SetGenerationRequest(generationRequestIndex, generationRequest);
		}
		callback?.Invoke();
		Close();
	}

	private IEnumerable<string> GetWarnings()
	{
		if (ignoreRestrictions)
		{
			if (arc > 0 && inheritable)
			{
				yield return "XenotypeBreaksLimits_Archites".Translate();
			}
			if (met > GeneTuning.BiostatRange.TrueMax)
			{
				yield return "XenotypeBreaksLimits_Exceeds".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMax.Named("MAX")).CapitalizeFirst();
			}
			else if (met < GeneTuning.BiostatRange.TrueMin)
			{
				yield return "XenotypeBreaksLimits_Below".Translate("Metabolism".Translate().Named("STAT"), met.Named("VALUE"), GeneTuning.BiostatRange.TrueMin.Named("MIN")).CapitalizeFirst();
			}
		}
	}

	protected override void UpdateSearchResults()
	{
		quickSearchWidget.noResultsMatched = false;
		matchingGenes.Clear();
		matchingCategories.Clear();
		if (!quickSearchWidget.filter.Active)
		{
			return;
		}
		foreach (GeneDef item in GeneUtility.GenesInOrder)
		{
			if (!selectedGenes.Contains(item))
			{
				if (quickSearchWidget.filter.Matches(item.label))
				{
					matchingGenes.Add(item);
				}
				if (quickSearchWidget.filter.Matches(item.displayCategory.label))
				{
					matchingCategories.Add(item.displayCategory);
				}
			}
		}
		quickSearchWidget.noResultsMatched = !matchingGenes.Any() && !matchingCategories.Any();
	}
}
