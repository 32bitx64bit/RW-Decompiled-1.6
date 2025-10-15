using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class ResourceReadout
{
	private Vector2 scrollPosition;

	private float lastDrawnHeight;

	private readonly List<ThingCategoryDef> RootThingCategories;

	private const float LineHeightSimple = 24f;

	private const float LineHeightCategorized = 24f;

	private const float DistFromScreenBottom = 200f;

	public ResourceReadout()
	{
		RootThingCategories = DefDatabase<ThingCategoryDef>.AllDefs.Where((ThingCategoryDef cat) => cat.resourceReadoutRoot).ToList();
	}

	public void ResourceReadoutOnGUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type != 8 && Current.ProgramState == ProgramState.Playing && Find.MainTabsRoot.OpenTab != MainButtonDefOf.Menu)
		{
			GenUI.DrawTextWinterShadow(new Rect(256f, 512f, -256f, -512f));
			Text.Font = GameFont.Small;
			Rect val = (Prefs.ResourceReadoutCategorized ? new Rect(2f, 7f, 124f, (float)(UI.screenHeight - 7) - 200f) : new Rect(7f, 7f, 110f, (float)(UI.screenHeight - 7) - 200f));
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width, lastDrawnHeight);
			bool num = ((Rect)(ref val2)).height > ((Rect)(ref val)).height;
			if (num)
			{
				Widgets.BeginScrollView(val, ref scrollPosition, val2, showScrollbars: false);
			}
			else
			{
				scrollPosition = Vector2.zero;
				Widgets.BeginGroup(val);
			}
			if (!Prefs.ResourceReadoutCategorized)
			{
				DoReadoutSimple(val2, ((Rect)(ref val)).height);
			}
			else
			{
				DoReadoutCategorized(val2);
			}
			if (num)
			{
				Widgets.EndScrollView();
			}
			else
			{
				Widgets.EndGroup();
			}
		}
	}

	private void DoReadoutCategorized(Rect rect)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Listing_ResourceReadout listing_ResourceReadout = new Listing_ResourceReadout(Find.CurrentMap);
		listing_ResourceReadout.Begin(rect);
		listing_ResourceReadout.nestIndentWidth = 7f;
		listing_ResourceReadout.lineHeight = 24f;
		listing_ResourceReadout.verticalSpacing = 0f;
		for (int i = 0; i < RootThingCategories.Count; i++)
		{
			listing_ResourceReadout.DoCategory(RootThingCategories[i].treeNode, 0, 32);
		}
		listing_ResourceReadout.End();
		lastDrawnHeight = listing_ResourceReadout.CurHeight;
	}

	private void DoReadoutSimple(Rect rect, float outRectHeight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Text.Anchor = (TextAnchor)3;
		float num = 0f;
		Rect rect2 = default(Rect);
		foreach (KeyValuePair<ThingDef, int> allCountedAmount in Find.CurrentMap.resourceCounter.AllCountedAmounts)
		{
			if (allCountedAmount.Value > 0 || allCountedAmount.Key.resourceReadoutAlwaysShow)
			{
				((Rect)(ref rect2))._002Ector(0f, num, 999f, 24f);
				if (((Rect)(ref rect2)).yMax >= scrollPosition.y && ((Rect)(ref rect2)).y <= scrollPosition.y + outRectHeight)
				{
					DrawResourceSimple(rect2, allCountedAmount.Key);
				}
				num += 24f;
			}
		}
		Text.Anchor = (TextAnchor)0;
		lastDrawnHeight = num;
		Widgets.EndGroup();
	}

	public void DrawResourceSimple(Rect rect, ThingDef thingDef)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		DrawIcon(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, thingDef);
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + 2f;
		int count = Find.CurrentMap.resourceCounter.GetCount(thingDef);
		Widgets.Label(new Rect(34f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 34f, ((Rect)(ref rect)).height), count.ToStringCached());
	}

	private void DrawIcon(float x, float y, ThingDef thingDef)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, 27f, 27f);
		Color color = GUI.color;
		Widgets.ThingIcon(rect, thingDef);
		GUI.color = color;
		if (Mouse.IsOver(rect))
		{
			TaggedString taggedString = thingDef.LabelCap + ": " + thingDef.description.CapitalizeFirst();
			TooltipHandler.TipRegion(rect, taggedString);
		}
	}
}
