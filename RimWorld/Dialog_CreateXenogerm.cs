using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_CreateXenogerm : GeneCreationDialogBase
{
	private Building_GeneAssembler geneAssembler;

	private List<Genepack> libraryGenepacks = new List<Genepack>();

	private List<Genepack> unpoweredGenepacks = new List<Genepack>();

	private List<Genepack> selectedGenepacks = new List<Genepack>();

	private HashSet<Genepack> matchingGenepacks = new HashSet<Genepack>();

	private readonly Color UnpoweredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	private List<GeneDef> tmpGenes = new List<GeneDef>();

	public override Vector2 InitialSize => new Vector2(1016f, (float)UI.screenHeight);

	protected override string Header => "AssembleGenes".Translate();

	protected override string AcceptButtonLabel => "StartCombining".Translate();

	protected override List<GeneDef> SelectedGenes
	{
		get
		{
			tmpGenes.Clear();
			foreach (Genepack selectedGenepack in selectedGenepacks)
			{
				foreach (GeneDef item in selectedGenepack.GeneSet.GenesListForReading)
				{
					tmpGenes.Add(item);
				}
			}
			return tmpGenes;
		}
	}

	public Dialog_CreateXenogerm(Building_GeneAssembler geneAssembler)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		this.geneAssembler = geneAssembler;
		maxGCX = geneAssembler.MaxComplexity();
		libraryGenepacks.AddRange(geneAssembler.GetGenepacks(includePowered: true, includeUnpowered: true));
		unpoweredGenepacks.AddRange(geneAssembler.GetGenepacks(includePowered: false, includeUnpowered: true));
		xenotypeName = string.Empty;
		closeOnAccept = false;
		forcePause = true;
		absorbInputAroundWindow = true;
		searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
		libraryGenepacks.SortGenepacks();
		unpoweredGenepacks.SortGenepacks();
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckBiotech("gene assembly"))
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
	}

	protected override void Accept()
	{
		if (geneAssembler.Working)
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmStartNewXenogerm".Translate(geneAssembler.xenotypeName.Named("XENOGERMNAME")), StartAssembly, destructive: true));
		}
		else
		{
			StartAssembly();
		}
	}

	private void StartAssembly()
	{
		geneAssembler.Start(selectedGenepacks, arc, xenotypeName?.Trim(), iconDef);
		SoundDefOf.StartRecombining.PlayOneShotOnCamera();
		Close(doCloseSound: false);
	}

	protected override void DrawGenes(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		GUI.BeginGroup(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollHeight);
		float curY = 0f;
		Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, val);
		Rect containingRect = val;
		((Rect)(ref containingRect)).y = scrollPosition.y;
		((Rect)(ref containingRect)).height = ((Rect)(ref rect)).height;
		DrawSection(rect, selectedGenepacks, "SelectedGenepacks".Translate(), ref curY, ref selectedHeight, adding: false, containingRect);
		curY += 8f;
		DrawSection(rect, libraryGenepacks, "GenepackLibrary".Translate(), ref curY, ref unselectedHeight, adding: true, containingRect);
		if ((int)Event.current.type == 8)
		{
			scrollHeight = curY;
		}
		Widgets.EndScrollView();
		GUI.EndGroup();
	}

	private void DrawSection(Rect rect, List<Genepack> genepacks, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Invalid comparison between Unknown and I4
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		float curX = 4f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(10f, curY, ((Rect)(ref rect)).width - 16f - 10f, Text.LineHeight);
		Widgets.Label(rect2, label);
		if (!adding)
		{
			Text.Anchor = (TextAnchor)2;
			GUI.color = ColoredText.SubtleGrayColor;
			Widgets.Label(rect2, "ClickToAddOrRemove".Translate());
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
		}
		curY += Text.LineHeight + 3f;
		float num = curY;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, ((Rect)(ref rect)).width, sectionHeight);
		Widgets.DrawRectFast(val, Widgets.MenuSectionBGFillColor);
		curY += 4f;
		if (!genepacks.Any())
		{
			Text.Anchor = (TextAnchor)4;
			GUI.color = ColoredText.SubtleGrayColor;
			Widgets.Label(val, "(" + "NoneLower".Translate() + ")");
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
		}
		else
		{
			for (int i = 0; i < genepacks.Count; i++)
			{
				Genepack genepack = genepacks[i];
				if (quickSearchWidget.filter.Active && (!matchingGenepacks.Contains(genepack) || (adding && selectedGenepacks.Contains(genepack))))
				{
					continue;
				}
				float num2 = 34f + GeneCreationDialogBase.GeneSize.x * (float)genepack.GeneSet.GenesListForReading.Count + 4f * (float)(genepack.GeneSet.GenesListForReading.Count + 2);
				if (curX + num2 > ((Rect)(ref rect)).width - 16f)
				{
					curX = 4f;
					curY += GeneCreationDialogBase.GeneSize.y + 8f + 14f;
				}
				if (adding && selectedGenepacks.Contains(genepack))
				{
					Widgets.DrawLightHighlight(new Rect(curX, curY, num2, GeneCreationDialogBase.GeneSize.y + 8f));
					curX += num2 + 14f;
				}
				else if (DrawGenepack(genepack, ref curX, curY, num2, containingRect))
				{
					if (adding)
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						selectedGenepacks.Add(genepack);
					}
					else
					{
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
						selectedGenepacks.Remove(genepack);
					}
					if (!xenotypeNameLocked)
					{
						xenotypeName = GeneUtility.GenerateXenotypeNameFromGenes(SelectedGenes);
					}
					OnGenesChanged();
					break;
				}
			}
		}
		curY += GeneCreationDialogBase.GeneSize.y + 12f;
		if ((int)Event.current.type == 8)
		{
			sectionHeight = curY - num;
		}
	}

	private bool DrawGenepack(Genepack genepack, ref float curX, float curY, float packWidth, Rect containingRect)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (genepack.GeneSet == null || genepack.GeneSet.GenesListForReading.NullOrEmpty())
		{
			return result;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
		if (!((Rect)(ref containingRect)).Overlaps(val))
		{
			curX = ((Rect)(ref val)).xMax + 14f;
			return false;
		}
		Widgets.DrawHighlight(val);
		GUI.color = GeneCreationDialogBase.OutlineColorUnselected;
		Widgets.DrawBox(val);
		GUI.color = Color.white;
		curX += 4f;
		GeneUIUtility.DrawBiostats(genepack.GeneSet.ComplexityTotal, genepack.GeneSet.MetabolismTotal, genepack.GeneSet.ArchitesTotal, ref curX, curY, 4f);
		List<GeneDef> genesListForReading = genepack.GeneSet.GenesListForReading;
		Rect geneRect = default(Rect);
		for (int i = 0; i < genesListForReading.Count; i++)
		{
			GeneDef gene = genesListForReading[i];
			if (quickSearchWidget.filter.Active && matchingGenes.Contains(gene))
			{
				matchingGenepacks.Contains(genepack);
			}
			else
				_ = 0;
			bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene));
			((Rect)(ref geneRect))._002Ector(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
			string extraTooltip = null;
			if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == gene))
			{
				extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == gene));
			}
			else if (cachedOverriddenGenes.Contains(gene))
			{
				extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene)));
			}
			else if (randomChosenGroups.ContainsKey(gene))
			{
				extraTooltip = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[gene].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
			}
			GeneUIUtility.DrawGeneDef(genesListForReading[i], geneRect, GeneType.Xenogene, () => extraTooltip, doBackground: false, clickable: false, overridden);
			curX += GeneCreationDialogBase.GeneSize.x + 4f;
		}
		Widgets.InfoCardButton(((Rect)(ref val)).xMax - 24f, ((Rect)(ref val)).y + 2f, genepack);
		if (unpoweredGenepacks.Contains(genepack))
		{
			Widgets.DrawBoxSolid(val, UnpoweredColor);
			TooltipHandler.TipRegion(val, "GenepackUnusableGenebankUnpowered".Translate().Colorize(ColorLibrary.RedReadable));
		}
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
		}
		if ((int)Event.current.type == 0 && Mouse.IsOver(val) && Event.current.button == 1)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			list.Add(new FloatMenuOption("EjectGenepackFromGeneBank".Translate(), delegate
			{
				CompGenepackContainer geneBankHoldingPack = geneAssembler.GetGeneBankHoldingPack(genepack);
				if (geneBankHoldingPack != null)
				{
					ThingWithComps parent = geneBankHoldingPack.parent;
					if (geneBankHoldingPack.innerContainer.TryDrop(genepack, parent.def.hasInteractionCell ? parent.InteractionCell : parent.Position, parent.Map, ThingPlaceMode.Near, 1, out var _))
					{
						if (selectedGenepacks.Contains(genepack))
						{
							selectedGenepacks.Remove(genepack);
						}
						tmpGenes.Clear();
						libraryGenepacks.Clear();
						unpoweredGenepacks.Clear();
						matchingGenepacks.Clear();
						libraryGenepacks.AddRange(geneAssembler.GetGenepacks(includePowered: true, includeUnpowered: true));
						unpoweredGenepacks.AddRange(geneAssembler.GetGenepacks(includePowered: false, includeUnpowered: true));
						libraryGenepacks.SortGenepacks();
						unpoweredGenepacks.SortGenepacks();
						OnGenesChanged();
					}
				}
			}));
			Find.WindowStack.Add(new FloatMenu(list));
		}
		else if (Widgets.ButtonInvisible(val))
		{
			result = true;
		}
		curX = Mathf.Max(curX, ((Rect)(ref val)).xMax + 14f);
		return result;
		static string GroupInfo(GeneLeftChosenGroup group)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (group == null)
			{
				return null;
			}
			return ("GeneOneActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
		}
	}

	protected override void DrawSearchRect(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSearchRect(rect);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "LoadXenogermTemplate".Translate()))
		{
			Find.WindowStack.Add(new Dialog_XenogermList_Load(delegate(CustomXenogerm xenogerm)
			{
				xenotypeName = xenogerm.name;
				xenotypeNameLocked = true;
				iconDef = xenogerm.iconDef;
				IEnumerable<Genepack> collection = CustomXenogermUtility.GetMatchingGenepacks(xenogerm.genesets, libraryGenepacks);
				selectedGenepacks.Clear();
				selectedGenepacks.AddRange(collection);
				OnGenesChanged();
				IEnumerable<GeneSet> source = xenogerm.genesets.Where((GeneSet gp) => !selectedGenepacks.Any((Genepack g) => g.GeneSet.Matches(gp)));
				if (source.Any())
				{
					string text = null;
					int num = source.Count();
					if (num == 1)
					{
						text = "MissingGenepackForXenogerm".Translate(xenogerm.name.Named("NAME"));
						text = text + ": " + source.Select((GeneSet g) => g.Label).ToCommaList().CapitalizeFirst();
					}
					else
					{
						text = "MissingGenepacksForXenogerm".Translate(num.Named("COUNT"), xenogerm.name.Named("NAME"));
					}
					Messages.Message(text, null, MessageTypeDefOf.CautionInput, historical: false);
				}
			}));
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - GeneCreationDialogBase.ButSize.x * 2f - 4f, ((Rect)(ref rect)).y, GeneCreationDialogBase.ButSize.x, GeneCreationDialogBase.ButSize.y), "SaveXenogermTemplate".Translate()))
		{
			AcceptanceReport acceptanceReport = CustomXenogermUtility.SaveXenogermTemplate(xenotypeName, iconDef, selectedGenepacks);
			if (!acceptanceReport.Reason.NullOrEmpty())
			{
				Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
	}

	protected override void DoBottomButtons(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		base.DoBottomButtons(rect);
		if (selectedGenepacks.Any())
		{
			int numTicks = Mathf.RoundToInt((float)Mathf.RoundToInt(GeneTuning.ComplexityToCreationHoursCurve.Evaluate(gcx) * 2500f) / geneAssembler.GetStatValue(StatDefOf.AssemblySpeedFactor));
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).center.x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f - GeneCreationDialogBase.ButSize.x - 10f, GeneCreationDialogBase.ButSize.y);
			TaggedString label;
			TaggedString taggedString;
			if (arc > 0 && !ResearchProjectDefOf.Archogenetics.IsFinished)
			{
				label = ("MissingRequiredResearch".Translate() + ": " + ResearchProjectDefOf.Archogenetics.LabelCap).Colorize(ColorLibrary.RedReadable);
				taggedString = "MustResearchProject".Translate(ResearchProjectDefOf.Archogenetics);
			}
			else
			{
				label = "RecombineDuration".Translate() + ": " + numTicks.ToStringTicksToPeriod();
				taggedString = "RecombineDurationDesc".Translate();
			}
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, label);
			Text.Anchor = (TextAnchor)0;
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, taggedString);
			}
		}
	}

	protected override bool CanAccept()
	{
		if (!base.CanAccept())
		{
			return false;
		}
		if (!selectedGenepacks.Any())
		{
			Messages.Message("MessageNoSelectedGenepacks".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (arc > 0 && !ResearchProjectDefOf.Archogenetics.IsFinished)
		{
			Messages.Message("AssemblingRequiresResearch".Translate(ResearchProjectDefOf.Archogenetics), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (gcx > maxGCX)
		{
			Messages.Message("ComplexityTooHighToCreateXenogerm".Translate(gcx.Named("AMOUNT"), maxGCX.Named("MAX")), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		if (!ColonyHasEnoughArchites())
		{
			Messages.Message("NotEnoughArchites".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		return true;
	}

	private bool ColonyHasEnoughArchites()
	{
		if (arc == 0 || geneAssembler.MapHeld == null)
		{
			return true;
		}
		List<Thing> list = geneAssembler.MapHeld.listerThings.ThingsOfDef(ThingDefOf.ArchiteCapsule);
		int num = 0;
		foreach (Thing item in list)
		{
			if (!item.Position.Fogged(geneAssembler.MapHeld))
			{
				num += item.stackCount;
				if (num >= arc)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected override void UpdateSearchResults()
	{
		quickSearchWidget.noResultsMatched = false;
		matchingGenepacks.Clear();
		matchingGenes.Clear();
		if (!quickSearchWidget.filter.Active)
		{
			return;
		}
		foreach (Genepack selectedGenepack in selectedGenepacks)
		{
			List<GeneDef> genesListForReading = selectedGenepack.GeneSet.GenesListForReading;
			for (int i = 0; i < genesListForReading.Count; i++)
			{
				if (quickSearchWidget.filter.Matches(genesListForReading[i].label))
				{
					matchingGenepacks.Add(selectedGenepack);
					matchingGenes.Add(genesListForReading[i]);
				}
			}
		}
		foreach (Genepack libraryGenepack in libraryGenepacks)
		{
			if (selectedGenepacks.Contains(libraryGenepack))
			{
				continue;
			}
			List<GeneDef> genesListForReading2 = libraryGenepack.GeneSet.GenesListForReading;
			for (int j = 0; j < genesListForReading2.Count; j++)
			{
				if (quickSearchWidget.filter.Matches(genesListForReading2[j].label))
				{
					matchingGenepacks.Add(libraryGenepack);
					matchingGenes.Add(genesListForReading2[j]);
				}
			}
		}
		quickSearchWidget.noResultsMatched = !matchingGenepacks.Any();
	}
}
