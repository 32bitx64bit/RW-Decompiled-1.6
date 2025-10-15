using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_ContentsGenepackHolder : ITab_ContentsBase
{
	private static readonly CachedTexture DropTex = new CachedTexture("UI/Buttons/Drop");

	private const float MiniGeneIconSize = 22f;

	public override IList<Thing> container => ContainerThing.innerContainer;

	public CompGenepackContainer ContainerThing => base.SelThing.TryGetComp<CompGenepackContainer>();

	public ITab_ContentsGenepackHolder()
	{
		labelKey = "TabCasketContents";
		containedItemsKey = "TabCasketContents";
	}

	public override void OnOpen()
	{
		if (!ModLister.CheckBiotech("genepack container"))
		{
			CloseTab();
		}
	}

	protected override void DoItemsLists(Rect inRect, ref float curY)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		CompGenepackContainer containerThing = ContainerThing;
		bool autoLoad = containerThing.autoLoad;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 24f);
		Widgets.CheckboxLabeled(rect, "AllowAllGenepacks".Translate(), ref containerThing.autoLoad);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegionByKey(rect, "AllowAllGenepacksDesc");
		}
		if (autoLoad != containerThing.autoLoad)
		{
			containerThing.leftToLoad.Clear();
		}
		curY += 28f;
		ListContainedGenepacks(inRect, containerThing, ref curY);
		if (!containerThing.autoLoad)
		{
			ListGenepacksToLoad(inRect, containerThing, ref curY);
			ListGenepacksOnMap(inRect, containerThing, ref curY);
		}
	}

	private void ListContainedGenepacks(Rect inRect, CompGenepackContainer container, ref float curY)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		GUI.BeginGroup(inRect);
		float num = curY;
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, containedItemsKey.Translate());
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref inRect)).width, curY - num - 3f);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegionByKey(rect, "ContainedGenepacksDesc");
		}
		List<Genepack> containedGenepacks = container.ContainedGenepacks;
		bool flag = false;
		for (int i = 0; i < containedGenepacks.Count; i++)
		{
			Genepack genepack = containedGenepacks[i];
			if (genepack != null)
			{
				flag = true;
				DoRow(genepack, container, ((Rect)(ref inRect)).width, ref curY, insideContainer: true);
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		GUI.EndGroup();
	}

	private void ListGenepacksToLoad(Rect inRect, CompGenepackContainer container, ref float curY)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		GUI.BeginGroup(inRect);
		float num = curY;
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, "GenepacksToLoad".Translate());
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref inRect)).width, curY - num - 3f);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegionByKey(rect, "GenepacksToLoadDesc");
		}
		if (container.leftToLoad != null)
		{
			for (int num2 = container.leftToLoad.Count - 1; num2 >= 0; num2--)
			{
				if (!(container.leftToLoad[num2] is Genepack { Destroyed: false } genepack) || genepack.MapHeld != container.parent.Map || !genepack.AutoLoad)
				{
					container.leftToLoad.RemoveAt(num2);
				}
				else
				{
					DoRow(genepack, container, ((Rect)(ref inRect)).width, ref curY, insideContainer: false);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		GUI.EndGroup();
	}

	private void ListGenepacksOnMap(Rect inRect, CompGenepackContainer container, ref float curY)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		GUI.BeginGroup(inRect);
		float num = curY;
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, "GenepacksToIgnore".Translate());
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref inRect)).width, curY - num - 3f);
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegionByKey(rect, "GenepacksIgnoredDesc");
		}
		List<Thing> list = container.parent.Map.listerThings.ThingsOfDef(ThingDefOf.Genepack);
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			Thing thing = list[num2];
			if (thing != null)
			{
				Genepack genepack = (Genepack)thing;
				if (genepack.targetContainer == null && genepack.AutoLoad)
				{
					DoRow((Genepack)thing, container, ((Rect)(ref inRect)).width, ref curY, insideContainer: false);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY, ((Rect)(ref inRect)).width);
		}
		GUI.EndGroup();
	}

	private void DoRow(Genepack genepack, CompGenepackContainer container, float width, ref float curY, bool insideContainer)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		bool checkOn = container.leftToLoad.Contains(genepack);
		bool flag = checkOn;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, width, 28f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).width - 24f, curY, 24f, 24f);
		if (insideContainer)
		{
			if (Widgets.ButtonImage(val2, DropTex.Texture))
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmRemoveGenepack".Translate(genepack.LabelNoCount), delegate
				{
					OnDropThing(genepack, genepack.stackCount);
				}));
			}
			TooltipHandler.TipRegionByKey(val2, "EjectGenepackDesc");
		}
		else
		{
			Widgets.Checkbox(((Rect)(ref val)).width - 24f, curY, ref checkOn);
			string key = (checkOn ? "RemoveFromLoadingListDesc" : "AddToLoadingListDesc");
			TooltipHandler.TipRegionByKey(val2, key);
		}
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		for (int num = Mathf.Min(genepack.GeneSet.GenesListForReading.Count, 5) - 1; num >= 0; num--)
		{
			GeneDef geneDef = genepack.GeneSet.GenesListForReading[num];
			Rect val3 = new Rect(((Rect)(ref val)).xMax - 22f, ((Rect)(ref val)).yMax - ((Rect)(ref val)).height / 2f - 11f, 22f, 22f);
			Widgets.DefIcon(val3, geneDef, null, 0.75f);
			Rect rect = val3;
			((Rect)(ref rect)).yMin = ((Rect)(ref val)).yMin;
			((Rect)(ref rect)).yMax = ((Rect)(ref val)).yMax;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, geneDef.LabelCap + "\n\n" + geneDef.DescriptionFull);
			}
			((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - 22f;
		}
		Widgets.InfoCardButton(0f, curY, genepack);
		if (Mouse.IsOver(val))
		{
			GUI.color = ITab_ContentsBase.ThingHighlightColor;
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		Widgets.ThingIcon(new Rect(24f, curY, 28f, 28f), genepack);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(60f, curY, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, genepack.LabelCap.Truncate(((Rect)(ref rect2)).width));
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(val))
		{
			TargetHighlighter.Highlight(genepack, arrow: true, colonistBar: false);
			TooltipHandler.TipRegion(val, genepack.LabelCap);
		}
		curY += 28f;
		if (flag != checkOn)
		{
			if (!checkOn)
			{
				genepack.targetContainer = null;
				container.leftToLoad.Remove(genepack);
			}
			else if (!container.CanLoadMore)
			{
				Messages.Message("CanOnlyStoreNumGenepacks".Translate(container.parent, container.Props.maxCapacity).CapitalizeFirst(), container.parent, MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				genepack.targetContainer = container.parent;
				container.leftToLoad.Add(genepack);
			}
		}
	}
}
