using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_EntityCodex : Window
{
	private Vector2 leftScrollPos;

	private Vector2 rightScrollPos;

	private float leftScrollHeight;

	private float rightScrollHeight;

	private EntityCodexEntryDef selectedEntry;

	private List<EntityCategoryDef> categoriesInOrder;

	private Dictionary<EntityCategoryDef, List<EntityCodexEntryDef>> entriesByCategory = new Dictionary<EntityCategoryDef, List<EntityCodexEntryDef>>();

	private Dictionary<EntityCategoryDef, float> categoryRectSizes = new Dictionary<EntityCategoryDef, float>();

	private bool devShowAll;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private const float HeaderHeight = 30f;

	private const float LeftRectWidthPct = 0.35f;

	private const float EntrySize = 74f;

	private const float EntryGap = 10f;

	private const int MaxEntriesPerRow = 7;

	public override Vector2 InitialSize => new Vector2(980f, 724f);

	public Dialog_EntityCodex(EntityCodexEntryDef selectedEntry = null)
	{
		doCloseX = true;
		doCloseButton = true;
		forcePause = true;
		categoriesInOrder = (from x in DefDatabase<EntityCategoryDef>.AllDefsListForReading
			where DefDatabase<EntityCodexEntryDef>.AllDefs.Any((EntityCodexEntryDef y) => y.category == x && y.Visible)
			orderby x.listOrder
			select x).ToList();
		foreach (EntityCategoryDef item in categoriesInOrder)
		{
			entriesByCategory.Add(item, new List<EntityCodexEntryDef>());
			categoryRectSizes.Add(item, 0f);
		}
		foreach (EntityCodexEntryDef item2 in DefDatabase<EntityCodexEntryDef>.AllDefsListForReading)
		{
			if (item2.Visible)
			{
				entriesByCategory[item2.category].Add(item2);
			}
		}
		foreach (KeyValuePair<EntityCategoryDef, List<EntityCodexEntryDef>> item3 in entriesByCategory)
		{
			item3.Deconstruct(out var _, out var value);
			value.SortBy((EntityCodexEntryDef e) => e.orderInCategory, (EntityCodexEntryDef e) => e.label);
		}
		this.selectedEntry = selectedEntry ?? DefDatabase<EntityCodexEntryDef>.AllDefs.OrderBy((EntityCodexEntryDef x) => x.label).FirstOrDefault((EntityCodexEntryDef x) => x.Discovered);
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect rect = inRect;
		((Rect)(ref rect)).height = ((Rect)(ref rect)).height - (ButSize.y + 10f);
		using (new TextBlock(GameFont.Medium))
		{
			Widgets.Label(new Rect(0f, 0f, ((Rect)(ref rect)).width, 30f), "EntityCodex".Translate());
		}
		if (Prefs.DevMode && DebugSettings.godMode)
		{
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref rect)).xMax - 150f, 0f, 150f, 30f), "DEV: Show all", ref devShowAll, disabled: false, null, null, placeCheckboxNearText: true);
		}
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 40f;
		TaggedString taggedString = "EntityCodexDesc".Translate();
		float num = Text.CalcHeight(taggedString, ((Rect)(ref rect)).width);
		Widgets.Label(new Rect(0f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, num), taggedString);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + (num + 10f);
		Rect inRect2 = rect.LeftPart(0.35f);
		Rect rect2 = rect.RightPart(0.65f);
		LeftRect(inRect2);
		RightRect(rect2);
	}

	private void LeftRect(Rect inRect)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, leftScrollHeight);
		Widgets.BeginScrollView(inRect, ref leftScrollPos, viewRect);
		if (selectedEntry != null)
		{
			float num = 0f;
			bool flag = devShowAll || selectedEntry.Discovered;
			using (new TextBlock(GameFont.Medium))
			{
				Widgets.Label(new Rect(0f, num, ((Rect)(ref viewRect)).width, 30f), flag ? selectedEntry.LabelCap : "UndiscoveredEntity".Translate());
				num += 40f;
			}
			using (new TextBlock(newWordWrap: true))
			{
				string text = (flag ? selectedEntry.Description : ((string)"UndiscoveredEntityDesc".Translate()));
				float num2 = Text.CalcHeight(text, ((Rect)(ref viewRect)).width);
				Widgets.Label(new Rect(0f, num, ((Rect)(ref viewRect)).width, num2), text);
				num += num2 + 10f;
			}
			if (flag)
			{
				if (selectedEntry.linkedThings.Count > 0)
				{
					Rect rect = default(Rect);
					foreach (ThingDef linkedThing in selectedEntry.linkedThings)
					{
						((Rect)(ref rect))._002Ector(0f, num, ((Rect)(ref viewRect)).width, Text.LineHeight);
						if (devShowAll || Find.EntityCodex.Discovered(linkedThing))
						{
							Widgets.HyperlinkWithIcon(rect, new Dialog_InfoCard.Hyperlink(linkedThing));
						}
						else
						{
							((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + ((Rect)(ref rect)).height;
							using (new TextBlock(ColoredText.SubtleGrayColor))
							{
								Widgets.Label(rect, "Undiscovered".Translate());
							}
						}
						num += ((Rect)(ref rect)).height;
					}
					num += 10f;
				}
				if (selectedEntry.discoveredResearchProjects.Count > 0)
				{
					Widgets.Label(new Rect(0f, num, ((Rect)(ref viewRect)).width, Text.LineHeight), "ResearchUnlocks".Translate() + ":");
					num += Text.LineHeight;
					Rect rect2 = default(Rect);
					foreach (ResearchProjectDef discoveredResearchProject in selectedEntry.discoveredResearchProjects)
					{
						((Rect)(ref rect2))._002Ector(0f, num, ((Rect)(ref viewRect)).width, Text.LineHeight);
						if (Widgets.ButtonText(rect2, "ViewHyperlink".Translate(discoveredResearchProject.LabelCap), drawBackground: false))
						{
							Close();
							Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Research);
							((MainTabWindow_Research)MainButtonDefOf.Research.TabWindow).Select(discoveredResearchProject);
						}
						num += ((Rect)(ref rect2)).height;
					}
				}
			}
			else if (Prefs.DevMode && DebugSettings.godMode)
			{
				if (Widgets.ButtonText(new Rect(0f, num, ((Rect)(ref viewRect)).width, ButSize.y), "DEV: Discover"))
				{
					if (!selectedEntry.linkedThings.NullOrEmpty())
					{
						for (int i = 0; i < selectedEntry.linkedThings.Count; i++)
						{
							Find.EntityCodex.SetDiscovered(selectedEntry, selectedEntry.linkedThings[i]);
						}
					}
					else
					{
						Find.EntityCodex.SetDiscovered(selectedEntry);
					}
				}
				num += ButSize.y;
			}
			leftScrollHeight = num;
		}
		Widgets.EndScrollView();
	}

	private void RightRect(Rect rect)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, rightScrollHeight);
		Widgets.BeginScrollView(rect, ref rightScrollPos, viewRect);
		float num = 0f;
		Rect rect2 = default(Rect);
		Rect rect3 = default(Rect);
		foreach (EntityCategoryDef item in categoriesInOrder)
		{
			float num2 = num;
			float num3 = categoryRectSizes[item];
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Widgets.DrawHighlight(new Rect(0f, num, ((Rect)(ref rect)).width, num3));
			GUI.color = Color.white;
			Widgets.Label(new Rect(10f, num, ((Rect)(ref rect)).width, Text.LineHeight), item.LabelCap);
			num += Text.LineHeight + 4f;
			List<EntityCodexEntryDef> list = entriesByCategory[item];
			int num4 = Mathf.Min(Mathf.CeilToInt(Mathf.Sqrt((float)list.Count)) + 1, 7);
			int num5 = Mathf.CeilToInt((float)list.Count / (float)num4);
			for (int i = 0; i < list.Count; i++)
			{
				EntityCodexEntryDef entityCodexEntryDef = list[i];
				int num6 = i / num4;
				int num7 = i % num4;
				int num8 = ((i >= list.Count - list.Count % num4) ? (list.Count % num4) : num4);
				float num9 = (((Rect)(ref viewRect)).width - (float)num8 * 74f - (float)(num8 - 1) * 10f) / 2f;
				((Rect)(ref rect2))._002Ector(num9 + (float)num7 * 74f + (float)num7 * 10f, num + (float)num6 * 74f + (float)num6 * 10f, 74f, 74f);
				bool flag = devShowAll || entityCodexEntryDef.Discovered;
				DrawEntry(rect2, entityCodexEntryDef, flag);
				if (flag)
				{
					Text.Font = GameFont.Tiny;
					float num10 = Text.CalcHeight(entityCodexEntryDef.LabelCap, ((Rect)(ref rect2)).width);
					((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).yMax - num10, ((Rect)(ref rect2)).width, num10);
					Widgets.DrawBoxSolid(rect3, new Color(0f, 0f, 0f, 0.3f));
					using (new TextBlock((TextAnchor)4))
					{
						Widgets.Label(rect3, entityCodexEntryDef.LabelCap);
					}
					Text.Font = GameFont.Small;
				}
			}
			num += 10f + (float)num5 * 74f + (float)(num5 - 1) * 10f;
			categoryRectSizes[item] = num - num2;
			num += 10f;
		}
		rightScrollHeight = num;
		Widgets.EndScrollView();
	}

	private void DrawEntry(Rect rect, EntityCodexEntryDef entry, bool discovered)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawOptionBackground(rect, entry == selectedEntry);
		GUI.DrawTexture(rect.ContractedBy(2f), (Texture)(object)(discovered ? entry.icon : entry.silhouette));
		if (Widgets.ButtonInvisible(rect))
		{
			selectedEntry = entry;
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
		}
	}
}
