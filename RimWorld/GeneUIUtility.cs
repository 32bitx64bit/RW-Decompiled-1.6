using System;
using System.Collections.Generic;
using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GeneUIUtility
{
	private static List<GeneDef> geneDefs = new List<GeneDef>();

	private static List<Gene> xenogenes = new List<Gene>();

	private static List<Gene> endogenes = new List<Gene>();

	private static float xenogenesHeight;

	private static float endogenesHeight;

	private static float scrollHeight;

	private static int gcx;

	private static int met;

	private static int arc;

	private static readonly Color CapsuleBoxColor = new Color(0.25f, 0.25f, 0.25f);

	private static readonly Color CapsuleBoxColorOverridden = new Color(0.15f, 0.15f, 0.15f);

	private static readonly CachedTexture GeneBackground_Archite = new CachedTexture("UI/Icons/Genes/GeneBackground_ArchiteGene");

	private static readonly CachedTexture GeneBackground_Xenogene = new CachedTexture("UI/Icons/Genes/GeneBackground_Xenogene");

	private static readonly CachedTexture GeneBackground_Endogene = new CachedTexture("UI/Icons/Genes/GeneBackground_Endogene");

	private const float OverriddenGeneIconAlpha = 0.75f;

	private const float XenogermIconSize = 34f;

	private const float XenotypeLabelWidth = 140f;

	private const float GeneGap = 6f;

	private const float GeneSize = 90f;

	public const float BiostatsWidth = 38f;

	public static void DrawGenesInfo(Rect rect, Thing target, float initialHeight, ref Vector2 size, ref Vector2 scrollPosition, GeneSet pregnancyGenes = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Invalid comparison between Unknown and I4
		Rect rect2 = rect;
		Rect val = rect2.ContractedBy(10f);
		if (Prefs.DevMode)
		{
			DoDebugButton(new Rect(((Rect)(ref rect2)).xMax - 18f - 125f, 5f, 115f, Text.LineHeight), target, pregnancyGenes);
		}
		GUI.BeginGroup(val);
		float num = BiostatsTable.HeightForBiostats(arc);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, 0f, ((Rect)(ref val)).width, ((Rect)(ref val)).height - num - 12f);
		DrawGeneSections(rect3, target, pregnancyGenes, ref scrollPosition);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(0f, ((Rect)(ref rect3)).yMax + 6f, ((Rect)(ref val)).width - 140f - 4f, num);
		((Rect)(ref rect4)).yMax = ((Rect)(ref rect3)).yMax + num + 6f;
		if (!(target is Pawn))
		{
			((Rect)(ref rect4)).width = ((Rect)(ref val)).width;
		}
		BiostatsTable.Draw(rect4, gcx, met, arc, drawMax: false, ignoreLimits: false);
		TryDrawXenotype(target, ((Rect)(ref rect4)).xMax + 4f, ((Rect)(ref rect4)).y + Text.LineHeight / 2f);
		if ((int)Event.current.type == 8)
		{
			float num2 = endogenesHeight + xenogenesHeight + num + 12f + 70f;
			if (num2 > initialHeight)
			{
				size.y = Mathf.Min(num2, (float)(UI.screenHeight - 35) - 165f - 30f);
			}
			else
			{
				size.y = initialHeight;
			}
			xenogenesHeight = 0f;
			endogenesHeight = 0f;
		}
		GUI.EndGroup();
	}

	private static void DrawGeneSections(Rect rect, Thing target, GeneSet genesOverride, ref Vector2 scrollPosition)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Invalid comparison between Unknown and I4
		RecacheGenes(target, genesOverride);
		GUI.BeginGroup(rect);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollHeight);
		float curY = 0f;
		Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, val);
		Rect containingRect = val;
		((Rect)(ref containingRect)).y = scrollPosition.y;
		((Rect)(ref containingRect)).height = ((Rect)(ref rect)).height;
		if (target is Pawn)
		{
			if (endogenes.Any())
			{
				DrawSection(rect, xeno: false, endogenes.Count, ref curY, ref endogenesHeight, delegate(int i, Rect r)
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					DrawGene(endogenes[i], r, GeneType.Endogene);
				}, containingRect);
				curY += 12f;
			}
			DrawSection(rect, xeno: true, xenogenes.Count, ref curY, ref xenogenesHeight, delegate(int i, Rect r)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				DrawGene(xenogenes[i], r, GeneType.Xenogene);
			}, containingRect);
		}
		else
		{
			GeneType geneType = ((genesOverride == null && !(target is HumanEmbryo)) ? GeneType.Xenogene : GeneType.Endogene);
			DrawSection(rect, geneType == GeneType.Xenogene, geneDefs.Count, ref curY, ref xenogenesHeight, delegate(int i, Rect r)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				DrawGeneDef(geneDefs[i], r, geneType);
			}, containingRect);
		}
		if ((int)Event.current.type == 8)
		{
			scrollHeight = curY;
		}
		Widgets.EndScrollView();
		GUI.EndGroup();
	}

	private static void RecacheGenes(Thing target, GeneSet genesOverride)
	{
		geneDefs.Clear();
		xenogenes.Clear();
		endogenes.Clear();
		gcx = 0;
		met = 0;
		arc = 0;
		Pawn pawn = target as Pawn;
		GeneSet geneSet = (target as GeneSetHolderBase)?.GeneSet ?? genesOverride;
		if (pawn != null)
		{
			foreach (Gene xenogene in pawn.genes.Xenogenes)
			{
				if (!xenogene.Overridden)
				{
					AddBiostats(xenogene.def);
				}
				xenogenes.Add(xenogene);
			}
			foreach (Gene endogene in pawn.genes.Endogenes)
			{
				if (endogene.def.endogeneCategory != EndogeneCategory.Melanin || !pawn.genes.Endogenes.Any((Gene x) => x.def.skinColorOverride.HasValue))
				{
					if (!endogene.Overridden)
					{
						AddBiostats(endogene.def);
					}
					endogenes.Add(endogene);
				}
			}
			xenogenes.SortGenes();
			endogenes.SortGenes();
		}
		else
		{
			if (geneSet == null)
			{
				return;
			}
			foreach (GeneDef item in geneSet.GenesListForReading)
			{
				geneDefs.Add(item);
			}
			gcx = geneSet.ComplexityTotal;
			met = geneSet.MetabolismTotal;
			arc = geneSet.ArchitesTotal;
			geneDefs.SortGeneDefs();
		}
		static void AddBiostats(GeneDef gene)
		{
			gcx += gene.biostatCpx;
			met += gene.biostatMet;
			arc += gene.biostatArc;
		}
	}

	private static void DrawSection(Rect rect, bool xeno, int count, ref float curY, ref float sectionHeight, Action<int, Rect> drawer, Rect containingRect)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Invalid comparison between Unknown and I4
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		Widgets.Label(10f, ref curY, ((Rect)(ref rect)).width, (xeno ? "Xenogenes" : "Endogenes").Translate().CapitalizeFirst(), (xeno ? "XenogenesDesc" : "EndogenesDesc").Translate());
		float num = curY;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, ((Rect)(ref rect)).width, sectionHeight);
		if (xeno && count == 0)
		{
			Text.Anchor = (TextAnchor)1;
			GUI.color = ColoredText.SubtleGrayColor;
			((Rect)(ref rect2)).height = Text.LineHeight;
			Widgets.Label(rect2, "(" + "NoXenogermImplanted".Translate() + ")");
			GUI.color = Color.white;
			Text.Anchor = (TextAnchor)0;
			curY += 90f;
		}
		else
		{
			Widgets.DrawMenuSection(rect2);
			float num2 = (((Rect)(ref rect)).width - 12f - 630f - 36f) / 2f;
			curY += num2;
			int num3 = 0;
			int num4 = 0;
			Rect val = default(Rect);
			for (int i = 0; i < count; i++)
			{
				if (num4 >= 6)
				{
					num4 = 0;
					num3++;
				}
				else if (i > 0)
				{
					num4++;
				}
				((Rect)(ref val))._002Ector(num2 + (float)num4 * 90f + (float)num4 * 6f, curY + (float)num3 * 90f + (float)num3 * 6f, 90f, 90f);
				if (((Rect)(ref containingRect)).Overlaps(val))
				{
					drawer(i, val);
				}
			}
			curY += (float)(num3 + 1) * 90f + (float)num3 * 6f + num2;
		}
		if ((int)Event.current.type == 8)
		{
			sectionHeight = curY - num;
		}
	}

	private static void TryDrawXenotype(Thing target, float x, float y)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Pawn sourcePawn = target as Pawn;
		if (sourcePawn == null)
		{
			return;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(x, y, 140f, Text.LineHeight);
		Text.Anchor = (TextAnchor)1;
		Widgets.Label(val, sourcePawn.genes.XenotypeLabelCap);
		Text.Anchor = (TextAnchor)0;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).center.x - 17f, ((Rect)(ref val)).yMax + 4f, 34f, 34f);
		GUI.color = XenotypeDef.IconColor;
		GUI.DrawTexture(val2, (Texture)(object)sourcePawn.genes.XenotypeIcon);
		GUI.color = Color.white;
		((Rect)(ref val)).yMax = ((Rect)(ref val2)).yMax;
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
			TooltipHandler.TipRegion(val, () => ("Xenotype".Translate() + ": " + sourcePawn.genes.XenotypeLabelCap).Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + sourcePawn.genes.XenotypeDescShort, 883938493);
		}
		if (Widgets.ButtonInvisible(val) && !sourcePawn.genes.UniqueXenotype)
		{
			Find.WindowStack.Add(new Dialog_InfoCard(sourcePawn.genes.Xenotype));
		}
	}

	private static void DoDebugButton(Rect buttonRect, Thing target, GeneSet genesOverride)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Pawn sourcePawn = target as Pawn;
		GeneSet geneSet = (target as GeneSetHolderBase)?.GeneSet ?? genesOverride;
		if (!Widgets.ButtonText(buttonRect, "Devtool..."))
		{
			return;
		}
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		string label = ((genesOverride != null || target is HumanEmbryo) ? "Add gene" : "Add xenogene");
		list.Add(new FloatMenuOption(label, delegate
		{
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugToolsPawns.Options_AddGene(delegate(GeneDef geneDef)
			{
				AddGene(geneDef, xeno: true);
			})));
		}));
		if (sourcePawn != null)
		{
			list.Add(new FloatMenuOption("Add endogene", delegate
			{
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(DebugToolsPawns.Options_AddGene(delegate(GeneDef geneDef)
				{
					AddGene(geneDef, xeno: false);
				})));
			}));
			if (xenogenes.Any() || endogenes.Any())
			{
				list.Add(new FloatMenuOption("Remove gene", delegate
				{
					List<DebugMenuOption> list5 = new List<DebugMenuOption>();
					List<Gene> list6 = new List<Gene>();
					list6.AddRange(endogenes);
					list6.AddRange(xenogenes);
					foreach (Gene item in list6)
					{
						Gene gene3 = item;
						list5.Add(new DebugMenuOption(gene3.LabelCap, DebugMenuOptionMode.Action, delegate
						{
							sourcePawn.genes.RemoveGene(gene3);
						}));
					}
					Find.WindowStack.Add(new Dialog_DebugOptionListLister(list5));
				}));
			}
			list.Add(new FloatMenuOption("Add all genes (xenogene)", delegate
			{
				sourcePawn.genes.Debug_AddAllGenes(xenogene: true);
			}));
			list.Add(new FloatMenuOption("Add all genes (endogene)", delegate
			{
				sourcePawn.genes.Debug_AddAllGenes(xenogene: false);
			}));
			list.Add(new FloatMenuOption("Apply xenotype", delegate
			{
				List<DebugMenuOption> list4 = new List<DebugMenuOption>();
				foreach (XenotypeDef allDef in DefDatabase<XenotypeDef>.AllDefs)
				{
					XenotypeDef xenotype = allDef;
					list4.Add(new DebugMenuOption(xenotype.LabelCap, DebugMenuOptionMode.Action, delegate
					{
						sourcePawn.genes.SetXenotype(xenotype);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list4));
			}));
			if (!sourcePawn.genes.UniqueXenotype)
			{
				list.Add(new FloatMenuOption("Reset genes to base xenotype", delegate
				{
					List<Gene> list3 = sourcePawn.genes.Endogenes;
					for (int num = list3.Count - 1; num >= 0; num--)
					{
						Gene gene2 = list3[num];
						if (gene2.def.endogeneCategory != EndogeneCategory.Melanin && gene2.def.endogeneCategory != EndogeneCategory.HairColor)
						{
							sourcePawn.genes.RemoveGene(gene2);
						}
					}
					sourcePawn.genes.SetXenotype(sourcePawn.genes.Xenotype);
				}));
			}
		}
		else if (geneDefs.Any() && geneSet != null)
		{
			list.Add(new FloatMenuOption("Remove gene", delegate
			{
				List<DebugMenuOption> list2 = new List<DebugMenuOption>();
				foreach (GeneDef geneDef in geneDefs)
				{
					GeneDef gene = geneDef;
					list2.Add(new DebugMenuOption(gene.LabelCap, DebugMenuOptionMode.Action, delegate
					{
						geneSet.Debug_RemoveGene(gene);
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
			}));
		}
		Find.WindowStack.Add(new FloatMenu(list));
		void AddGene(GeneDef geneDef, bool xeno)
		{
			if (sourcePawn != null)
			{
				sourcePawn.genes.AddGene(geneDef, xeno);
			}
			else if (geneSet != null)
			{
				geneSet.AddGene(geneDef);
			}
		}
	}

	public static void DrawGene(Gene gene, Rect geneRect, GeneType geneType, bool doBackground = true, bool clickable = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		DrawGeneBasics(gene.def, geneRect, geneType, doBackground, clickable, !gene.Active);
		if (Mouse.IsOver(geneRect))
		{
			string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.DescriptionFull;
			if (gene.Overridden)
			{
				text += "\n\n";
				text = ((gene.overriddenByGene.def != gene.def) ? (text + ("OverriddenByGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)) : (text + ("OverriddenByIdenticalGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary.RedReadable)));
			}
			if (clickable)
			{
				text = text + "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
			}
			TooltipHandler.TipRegion(geneRect, text);
		}
	}

	public static void DrawGeneDef(GeneDef gene, Rect geneRect, GeneType geneType, Func<string> extraTooltip = null, bool doBackground = true, bool clickable = true, bool overridden = false)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		DrawGeneBasics(gene, geneRect, geneType, doBackground, clickable, overridden);
		if (!Mouse.IsOver(geneRect))
		{
			return;
		}
		TooltipHandler.TipRegion(geneRect, delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			string text = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.DescriptionFull;
			if (extraTooltip != null)
			{
				string text2 = extraTooltip();
				if (!text2.NullOrEmpty())
				{
					text = text + "\n\n" + text2.Colorize(ColorLibrary.RedReadable);
				}
			}
			if (clickable)
			{
				text = text + "\n\n" + "ClickForMoreInfo".Translate().ToString().Colorize(ColoredText.SubtleGrayColor);
			}
			return text;
		}, 316238373);
	}

	private static void DrawGeneBasics(GeneDef gene, Rect geneRect, GeneType geneType, bool doBackground, bool clickable, bool overridden)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(geneRect);
		Rect val = geneRect.AtZero();
		if (doBackground)
		{
			Widgets.DrawHighlight(val);
			GUI.color = new Color(1f, 1f, 1f, 0.05f);
			Widgets.DrawBox(val);
			GUI.color = Color.white;
		}
		float num = ((Rect)(ref val)).width - Text.LineHeight;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref geneRect)).width / 2f - num / 2f, 0f, num, num);
		Color iconColor = gene.IconColor;
		if (overridden)
		{
			iconColor.a = 0.75f;
			GUI.color = ColoredText.SubtleGrayColor;
		}
		CachedTexture cachedTexture = GeneBackground_Archite;
		if (gene.biostatArc == 0)
		{
			switch (geneType)
			{
			case GeneType.Endogene:
				cachedTexture = GeneBackground_Endogene;
				break;
			case GeneType.Xenogene:
				cachedTexture = GeneBackground_Xenogene;
				break;
			}
		}
		GUI.DrawTexture(val2, (Texture)(object)cachedTexture.Texture);
		Widgets.DefIcon(val2, gene, null, 0.9f, null, drawPlaceholder: false, iconColor);
		Text.Font = GameFont.Tiny;
		float num2 = Text.CalcHeight(gene.LabelCap, ((Rect)(ref val)).width);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, ((Rect)(ref val)).yMax - num2, ((Rect)(ref val)).width, num2);
		GUI.DrawTexture(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMax - num2, ((Rect)(ref rect)).width, num2), (Texture)(object)TexUI.GrayTextBG);
		Text.Anchor = (TextAnchor)7;
		if (overridden)
		{
			GUI.color = ColoredText.SubtleGrayColor;
		}
		if (doBackground && num2 < (Text.LineHeight - 2f) * 2f)
		{
			((Rect)(ref rect)).y = ((Rect)(ref rect)).y - 3f;
		}
		Widgets.Label(rect, gene.LabelCap);
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		if (clickable)
		{
			if (Widgets.ButtonInvisible(val))
			{
				Find.WindowStack.Add(new Dialog_InfoCard(gene));
			}
			if (Mouse.IsOver(val))
			{
				Widgets.DrawHighlight(val);
			}
		}
		GUI.EndGroup();
	}

	private static void DrawStat(Rect iconRect, CachedTexture icon, string stat, float iconWidth)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(iconRect, (Texture)(object)icon.Texture);
		Text.Anchor = (TextAnchor)5;
		Widgets.LabelFit(new Rect(((Rect)(ref iconRect)).xMax, ((Rect)(ref iconRect)).y, 38f - iconWidth, iconWidth), stat);
		Text.Anchor = (TextAnchor)0;
	}

	public static void DrawBiostats(int gcx, int met, int arc, ref float curX, float curY, float margin = 6f)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		float num = GeneCreationDialogBase.GeneSize.y / 3f;
		float num2 = 0f;
		float num3 = Text.LineHeightOf(GameFont.Small);
		Rect iconRect = default(Rect);
		((Rect)(ref iconRect))._002Ector(curX, curY + margin + num2, num3, num3);
		DrawStat(iconRect, GeneUtility.GCXTex, gcx.ToString(), num3);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, ((Rect)(ref iconRect)).y, 38f, num3);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, "Complexity".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "ComplexityDesc".Translate());
		}
		num2 += num;
		if (met != 0)
		{
			Rect iconRect2 = default(Rect);
			((Rect)(ref iconRect2))._002Ector(curX, curY + margin + num2, num3, num3);
			DrawStat(iconRect2, GeneUtility.METTex, met.ToStringWithSign(), num3);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(curX, ((Rect)(ref iconRect2)).y, 38f, num3);
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, "Metabolism".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "MetabolismDesc".Translate());
			}
			num2 += num;
		}
		if (arc > 0)
		{
			Rect iconRect3 = default(Rect);
			((Rect)(ref iconRect3))._002Ector(curX, curY + margin + num2, num3, num3);
			DrawStat(iconRect3, GeneUtility.ARCTex, arc.ToString(), num3);
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(curX, ((Rect)(ref iconRect3)).y, 38f, num3);
			if (Mouse.IsOver(rect3))
			{
				Widgets.DrawHighlight(rect3);
				TooltipHandler.TipRegion(rect3, "ArchitesRequired".Translate().CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + "ArchitesRequiredDesc".Translate());
			}
		}
		curX += 34f;
	}
}
