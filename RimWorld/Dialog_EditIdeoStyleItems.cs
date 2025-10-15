using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Dialog_EditIdeoStyleItems : Window
{
	public enum ItemType
	{
		Hair,
		Beard,
		FaceTattoo,
		BodyTattoo
	}

	public class ExpandedInfo
	{
		public StyleItemCategoryDef categoryDef;

		public ItemType itemType;

		public bool any;

		public bool expanded;

		public ExpandedInfo(StyleItemCategoryDef categoryDef, ItemType itemType, bool any)
		{
			this.categoryDef = categoryDef;
			this.itemType = itemType;
			this.any = any;
			expanded = false;
		}
	}

	private Ideo ideo;

	private StyleItemDef hover;

	private StyleItemTab curTab;

	private DefMap<HairDef, StyleItemSpawningProperties> hairFrequencies;

	private DefMap<BeardDef, StyleItemSpawningProperties> beardFrequencies;

	private DefMap<TattooDef, StyleItemSpawningProperties> tattooFrequencies;

	private List<ExpandedInfo> expandedInfos;

	private List<TabRecord> tabs = new List<TabRecord>();

	private Vector2 scrollPositionLeft;

	private Vector2 scrollPositionRight;

	private float scrollViewHeightLeft;

	private float scrollViewHeightRight;

	private bool painting;

	private StyleItemFrequency[] allFrequencies;

	private IdeoEditMode editMode;

	private static readonly Vector2 ButSize = new Vector2(150f, 38f);

	private static readonly Color HairColor = PawnHairColors.ReddishBrown;

	private static readonly Color SemiSelectedColor = new Color(1f, 1f, 1f, 0.3f);

	private const float HeaderHeight = 35f;

	private const float TabsSpacing = 45f;

	private const float ItemHeight = 28f;

	private const float ItemLabelWidth = 110f;

	private static readonly Texture2D Plus = ContentFinder<Texture2D>.Get("UI/Buttons/Plus");

	private static readonly Texture2D Minus = ContentFinder<Texture2D>.Get("UI/Buttons/Minus");

	private static readonly Texture2D RadioButSemiOn = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButSemiOn");

	private Dictionary<string, string> labelCache = new Dictionary<string, string>();

	public override Vector2 InitialSize => new Vector2(Mathf.Min(1200f, (float)UI.screenWidth), 750f);

	protected override float Margin => 0f;

	public Dialog_EditIdeoStyleItems(Ideo ideo, StyleItemTab category, IdeoEditMode editMode)
	{
		this.ideo = ideo;
		curTab = category;
		this.editMode = editMode;
		absorbInputAroundWindow = true;
		allFrequencies = (StyleItemFrequency[])Enum.GetValues(typeof(StyleItemFrequency));
		expandedInfos = new List<ExpandedInfo>();
		ItemType[] array = (ItemType[])Enum.GetValues(typeof(ItemType));
		foreach (ItemType itemType in array)
		{
			foreach (StyleItemCategoryDef allDef in DefDatabase<StyleItemCategoryDef>.AllDefs)
			{
				expandedInfos.Add(new ExpandedInfo(allDef, itemType, CanList(allDef, itemType)));
			}
		}
		Reset();
	}

	private void Reset()
	{
		hairFrequencies = new DefMap<HairDef, StyleItemSpawningProperties>();
		foreach (KeyValuePair<HairDef, StyleItemSpawningProperties> hairFrequency in hairFrequencies)
		{
			hairFrequencies[hairFrequency.Key].frequency = ideo.style.GetFrequency(hairFrequency.Key);
			hairFrequencies[hairFrequency.Key].gender = ideo.style.GetGender(hairFrequency.Key);
		}
		beardFrequencies = new DefMap<BeardDef, StyleItemSpawningProperties>();
		foreach (KeyValuePair<BeardDef, StyleItemSpawningProperties> beardFrequency in beardFrequencies)
		{
			beardFrequencies[beardFrequency.Key].frequency = ideo.style.GetFrequency(beardFrequency.Key);
			beardFrequencies[beardFrequency.Key].gender = ideo.style.GetGender(beardFrequency.Key);
		}
		tattooFrequencies = new DefMap<TattooDef, StyleItemSpawningProperties>();
		foreach (KeyValuePair<TattooDef, StyleItemSpawningProperties> tattooFrequency in tattooFrequencies)
		{
			tattooFrequencies[tattooFrequency.Key].frequency = ideo.style.GetFrequency(tattooFrequency.Key);
			tattooFrequencies[tattooFrequency.Key].gender = ideo.style.GetGender(tattooFrequency.Key);
		}
	}

	public override void PostOpen()
	{
		base.PostOpen();
		if (!ModLister.CheckIdeology("Appearance editing"))
		{
			Close();
		}
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		hover = null;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 18f;
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin + 10f;
		((Rect)(ref rect2)).height = 35f;
		Text.Font = GameFont.Medium;
		Widgets.Label(rect2, "EditAppearanceItems".Translate());
		Text.Font = GameFont.Small;
		Rect rect3 = rect;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect2)).yMax + 35f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - (ButSize.y + 10f);
		FillDialog(rect3);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref rect)).xMax - ButSize.x - 10f, ((Rect)(ref rect)).y + 10f, ButSize.x, 30f);
		if (Widgets.ButtonText(rect4, "ExpandAllCategories".Translate().CapitalizeFirst()))
		{
			SoundDefOf.Tick_High.PlayOneShotOnCamera();
			foreach (ExpandedInfo expandedInfo in expandedInfos)
			{
				expandedInfo.expanded = true;
			}
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect4)).x, ((Rect)(ref rect4)).yMax + 4f, ButSize.x, 30f), "CollapseAllCategories".Translate().CapitalizeFirst()))
		{
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			foreach (ExpandedInfo expandedInfo2 in expandedInfos)
			{
				expandedInfo2.expanded = false;
			}
		}
		rect = rect.ContractedBy(18f);
		float num = ((Rect)(ref rect)).height - ButSize.y + 18f;
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width / 2f - ButSize.x / 2f, num, ButSize.x, ButSize.y);
		if (editMode == IdeoEditMode.None)
		{
			if (Widgets.ButtonText(rect5, "Back".Translate()))
			{
				Close();
			}
			return;
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x, num, ButSize.x, ButSize.y), "Cancel".Translate()))
		{
			Close();
		}
		TooltipHandler.TipRegion(rect5, "ResetButtonDesc".Translate());
		if (Widgets.ButtonText(rect5, "Reset".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			Reset();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).width - ButSize.x + 18f, num, ButSize.x, ButSize.y), "DoneButton".Translate()))
		{
			Done();
		}
	}

	public override void OnAcceptKeyPressed()
	{
		Done();
		Event.current.Use();
	}

	private void FillDialog(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		DrawTabs(rect);
		rect = rect.ContractedBy(18f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f - 5f, ((Rect)(ref rect)).height);
		DrawSection(rect2, ref scrollPositionLeft, ref scrollViewHeightLeft, (curTab != 0) ? ItemType.FaceTattoo : ItemType.Hair);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).xMax + 10f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width / 2f - 10f, ((Rect)(ref rect)).height);
		DrawSection(rect3, ref scrollPositionRight, ref scrollViewHeightRight, (curTab == StyleItemTab.HairAndBeard) ? ItemType.Beard : ItemType.BodyTattoo);
		if (!Input.GetMouseButton(0))
		{
			painting = false;
		}
	}

	private void DrawTabs(Rect rect)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		tabs.Clear();
		tabs.Add(new TabRecord("HairAndBeards".Translate(), delegate
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			curTab = StyleItemTab.HairAndBeard;
			scrollPositionLeft = (scrollPositionRight = Vector2.zero);
		}, curTab == StyleItemTab.HairAndBeard));
		tabs.Add(new TabRecord("Tattoos".Translate(), delegate
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			curTab = StyleItemTab.Tattoo;
			scrollPositionLeft = (scrollPositionRight = Vector2.zero);
		}, curTab == StyleItemTab.Tattoo));
		TabDrawer.DrawTabs(rect, tabs);
	}

	private void DrawSection(Rect rect, ref Vector2 scrollPosition, ref float scrollViewHeight, ItemType itemType)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Invalid comparison between Unknown and I4
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(new Rect(((Rect)(ref rect)).x + 10f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, 30f), GetSectionLabel(itemType).CapitalizeFirst());
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 30f;
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		for (int i = 0; i < allFrequencies.Length; i++)
		{
			Rect rect2 = FrequencyPosition(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width, Text.LineHeight), i);
			string label = allFrequencies[i].GetLabel();
			string text = label.Truncate(((Rect)(ref rect2)).width);
			Widgets.Label(rect2, text);
			if (label != text)
			{
				TooltipHandler.TipRegion(rect2, label);
			}
		}
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + Text.LineHeight;
		Widgets.BeginGroup(rect);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, scrollViewHeight);
		float curY = 0f;
		float num = 28f;
		Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, viewRect);
		foreach (StyleItemCategoryDef c in DefDatabase<StyleItemCategoryDef>.AllDefs)
		{
			ExpandedInfo expandedInfo = expandedInfos.FirstOrDefault((ExpandedInfo x) => x.categoryDef == c && itemType == x.itemType);
			if (expandedInfo != null && expandedInfo.any)
			{
				ListStyleItemCategory(c, ref curY, viewRect, expandedInfo, itemType);
				num += 28f + 28f * (float)c.ItemsInCategory.Where((StyleItemDef x) => CanList(x, itemType)).Count();
			}
		}
		if ((int)Event.current.type == 8)
		{
			scrollViewHeight = num;
		}
		Widgets.EndScrollView();
		Widgets.EndGroup();
	}

	private void ListStyleItemCategory(StyleItemCategoryDef category, ref float curY, Rect viewRect, ExpandedInfo expandedInfo, ItemType itemType)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref viewRect)).x, ((Rect)(ref viewRect)).y + curY, ((Rect)(ref viewRect)).width, 28f);
		Widgets.DrawHighlightSelected(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref viewRect)).x, curY, 28f, 28f);
		GUI.DrawTexture(rect2.ContractedBy(4f), (Texture)(object)(expandedInfo.expanded ? Minus : Plus));
		Widgets.DrawHighlightIfMouseover(rect);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).xMax + 4f, curY, 110f, 28f);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect3, category.LabelCap);
		Text.Anchor = (TextAnchor)0;
		if (Widgets.ButtonInvisible(new Rect(((Rect)(ref rect2)).x, ((Rect)(ref rect2)).y, ((Rect)(ref rect2)).width + ((Rect)(ref rect3)).width + 4f, 28f)))
		{
			expandedInfo.expanded = !expandedInfo.expanded;
			if (expandedInfo.expanded)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}
		Gender? gender = null;
		if (itemType != ItemType.Beard)
		{
			gender = TryGetGender(category, itemType);
		}
		DrawConfigInfo(viewRect, curY, delegate(Gender g)
		{
			foreach (StyleItemDef item in category.ItemsInCategory)
			{
				if (CanList(item, itemType))
				{
					ChangeGender(item, g);
				}
			}
		}, delegate(StyleItemFrequency f)
		{
			foreach (StyleItemDef item2 in category.ItemsInCategory)
			{
				if (CanList(item2, itemType))
				{
					ChangeFrequency(item2, f);
				}
			}
		}, gender, TryGetFrequency(category, itemType), (StyleItemFrequency f) => category.ItemsInCategory.Any((StyleItemDef x) => CanList(x, itemType) && GetFrequency(x) == f));
		curY += ((Rect)(ref rect)).height;
		if (expandedInfo.expanded)
		{
			int num = 0;
			foreach (StyleItemDef item3 in category.ItemsInCategory)
			{
				if (CanList(item3, itemType))
				{
					ListStyleItem(item3, ref curY, num, viewRect);
					num++;
				}
			}
		}
		curY += 4f;
	}

	private void ListStyleItem(StyleItemDef styleItem, ref float curY, int index, Rect viewRect)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref viewRect)).x + 17f, ((Rect)(ref viewRect)).y + curY, ((Rect)(ref viewRect)).width - 17f, 28f);
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(new Rect(((Rect)(ref rect)).x + 28f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 28f, ((Rect)(ref rect)).height));
		}
		if (Mouse.IsOver(rect))
		{
			hover = styleItem;
			Rect r = new Rect(UI.MousePositionOnUI.x + 10f, UI.MousePositionOnUIInverted.y, 100f, 100f + Text.LineHeight);
			Find.WindowStack.ImmediateWindow(12918217, r, WindowLayer.Super, delegate
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				Rect val = r.AtZero();
				((Rect)(ref val)).height = ((Rect)(ref val)).height - Text.LineHeight;
				Widgets.DrawHighlight(val);
				if (hover != null)
				{
					Text.Anchor = (TextAnchor)1;
					Widgets.LabelFit(new Rect(0f, ((Rect)(ref val)).yMax, ((Rect)(ref val)).width, Text.LineHeight), hover.LabelCap);
					Text.Anchor = (TextAnchor)0;
					float scale = 1.1f;
					if (hover is HairDef)
					{
						GUI.color = HairColor;
						((Rect)(ref val)).y = ((Rect)(ref val)).y + 10f;
					}
					else if (hover is BeardDef)
					{
						GUI.color = HairColor;
						((Rect)(ref val)).y = ((Rect)(ref val)).y - 10f;
					}
					else if (hover is TattooDef)
					{
						Widgets.DrawRectFast(val, SemiSelectedColor);
						scale = 1.25f;
					}
					Widgets.DefIcon(val, hover, null, scale);
					GUI.color = Color.white;
				}
			});
		}
		Widgets.DrawHighlightIfMouseover(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, 28f, 28f);
		Rect rect3 = rect2.ContractedBy(2f);
		Widgets.DrawHighlight(rect3);
		if (styleItem is HairDef || styleItem is BeardDef)
		{
			GUI.color = HairColor;
		}
		else if (styleItem is TattooDef)
		{
			Widgets.DrawHighlight(rect3);
		}
		Widgets.DefIcon(rect2, styleItem, null, 1.25f);
		GUI.color = Color.white;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref rect2)).xMax + 4f, curY, 110f, 28f);
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect4, GenText.Truncate(styleItem.LabelCap, ((Rect)(ref rect4)).width, labelCache));
		Text.Anchor = (TextAnchor)0;
		DrawConfigInfo(viewRect, curY, delegate(Gender g)
		{
			ChangeGender(styleItem, g);
		}, delegate(StyleItemFrequency f)
		{
			ChangeFrequency(styleItem, f);
		}, GetGender(styleItem), GetFrequency(styleItem));
		curY += 28f;
	}

	private void DrawConfigInfo(Rect viewRect, float curY, Action<Gender> changeGenderAction, Action<StyleItemFrequency> changeFrequencyAction, Gender? gender = null, StyleItemFrequency? curFrequency = null, Func<StyleItemFrequency, bool> semiSelectedValidator = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		Rect inRect = default(Rect);
		((Rect)(ref inRect))._002Ector(((Rect)(ref viewRect)).x, curY, ((Rect)(ref viewRect)).width + 16f, 28f);
		Rect val = FrequencyPosition(inRect, 0);
		if (gender.HasValue)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref val)).xMin - ((Rect)(ref val)).width / 2f, curY, 28f, 28f);
			if (Mouse.IsOver(val2))
			{
				Widgets.DrawHighlight(val2);
				if (gender.HasValue)
				{
					TooltipHandler.TipRegion(val2, GenderLabel(gender.Value).CapitalizeFirst());
				}
			}
			GUI.DrawTexture(val2.ContractedBy(4f), (Texture)(object)gender.Value.GetIcon());
			if (Widgets.ButtonInvisible(val2))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (Gender g2 in Enum.GetValues(typeof(Gender)))
				{
					list.Add(new FloatMenuOption(GenderLabel(g2).CapitalizeFirst(), delegate
					{
						changeGenderAction(g2);
					}, g2.GetIcon(), Color.white));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}
		for (int i = 0; i < allFrequencies.Length; i++)
		{
			StyleItemFrequency styleItemFrequency = allFrequencies[i];
			bool flag = curFrequency == styleItemFrequency;
			Rect val3 = FrequencyPosition(inRect, i);
			Widgets.DrawHighlightIfMouseover(val3);
			Vector2 val4 = ((Rect)(ref val3)).center - new Vector2(12f, 12f);
			if (editMode != 0)
			{
				Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(val3);
				if (draggableResult == Widgets.DraggableResult.Dragged)
				{
					painting = true;
				}
				Widgets.RadioButton(val4, flag);
				if (!flag && semiSelectedValidator != null && semiSelectedValidator(styleItemFrequency))
				{
					GUI.DrawTexture(new Rect(val4.x, val4.y, 24f, 24f), (Texture)(object)RadioButSemiOn);
				}
				if (!flag && ((painting && Mouse.IsOver(val3)) || draggableResult.AnyPressed()))
				{
					SoundDefOf.Designate_DragStandard_Changed_NoCam.PlayOneShotOnCamera();
					changeFrequencyAction(styleItemFrequency);
				}
			}
			else if (flag)
			{
				Widgets.RadioButton(val4, flag);
			}
		}
		static string GenderLabel(Gender g)
		{
			if (g == Gender.None)
			{
				return "MaleAndFemale".Translate();
			}
			return g.GetLabel();
		}
	}

	private StyleItemFrequency GetFrequency(StyleItemDef def)
	{
		if (def is HairDef def2)
		{
			return hairFrequencies[def2].frequency;
		}
		if (def is BeardDef def3)
		{
			return beardFrequencies[def3].frequency;
		}
		if (def is TattooDef def4)
		{
			return tattooFrequencies[def4].frequency;
		}
		return StyleItemFrequency.Never;
	}

	private Gender? GetGender(StyleItemDef def)
	{
		StyleGender styleGender = StyleGender.Any;
		if (def is HairDef def2)
		{
			styleGender = hairFrequencies[def2].gender;
		}
		else
		{
			if (def is BeardDef)
			{
				return null;
			}
			if (def is TattooDef def3)
			{
				styleGender = tattooFrequencies[def3].gender;
			}
		}
		switch (styleGender)
		{
		case StyleGender.Male:
		case StyleGender.MaleUsually:
			return Gender.Male;
		case StyleGender.FemaleUsually:
		case StyleGender.Female:
			return Gender.Female;
		default:
			return Gender.None;
		}
	}

	private void ChangeFrequency(StyleItemDef def, StyleItemFrequency freq)
	{
		if (def is HairDef def2)
		{
			hairFrequencies[def2].frequency = freq;
		}
		else if (def is BeardDef def3)
		{
			beardFrequencies[def3].frequency = freq;
		}
		else if (def is TattooDef def4)
		{
			tattooFrequencies[def4].frequency = freq;
		}
	}

	private void ChangeGender(StyleItemDef def, StyleGender gender)
	{
		if (def is HairDef def2)
		{
			hairFrequencies[def2].gender = gender;
		}
		else if (def is BeardDef def3)
		{
			beardFrequencies[def3].gender = gender;
		}
		else if (def is TattooDef def4)
		{
			tattooFrequencies[def4].gender = gender;
		}
	}

	private void ChangeGender(StyleItemDef def, Gender gender)
	{
		StyleGender gender2 = StyleGender.Any;
		switch (gender)
		{
		case Gender.Male:
			gender2 = StyleGender.Male;
			break;
		case Gender.Female:
			gender2 = StyleGender.Female;
			break;
		}
		ChangeGender(def, gender2);
	}

	private bool CanList(StyleItemDef s, ItemType itemType)
	{
		switch (itemType)
		{
		case ItemType.Hair:
			return s is HairDef;
		case ItemType.Beard:
			return s is BeardDef;
		case ItemType.FaceTattoo:
			if (s is TattooDef tattooDef2)
			{
				return tattooDef2.tattooType == TattooType.Face;
			}
			return false;
		case ItemType.BodyTattoo:
			if (s is TattooDef tattooDef)
			{
				return tattooDef.tattooType == TattooType.Body;
			}
			return false;
		default:
			return false;
		}
	}

	private ItemType GetItemType(StyleItemDef s)
	{
		if (s is HairDef)
		{
			return ItemType.Hair;
		}
		if (s is BeardDef)
		{
			return ItemType.Beard;
		}
		if (s is TattooDef tattooDef)
		{
			if (tattooDef.tattooType == TattooType.Body)
			{
				return ItemType.BodyTattoo;
			}
			return ItemType.FaceTattoo;
		}
		return ItemType.Hair;
	}

	private bool CanList(StyleItemCategoryDef category, ItemType itemType)
	{
		foreach (StyleItemDef item in category.ItemsInCategory)
		{
			if (GetItemType(item) == itemType)
			{
				return true;
			}
		}
		return false;
	}

	private string GetSectionLabel(ItemType itemType)
	{
		return itemType switch
		{
			ItemType.Hair => "Hair".Translate(), 
			ItemType.Beard => "Beard".Translate(), 
			ItemType.FaceTattoo => "TattooFace".Translate(), 
			ItemType.BodyTattoo => "TattooBody".Translate(), 
			_ => string.Empty, 
		};
	}

	private Gender TryGetGender(StyleItemCategoryDef def, ItemType itemType)
	{
		Gender? gender = null;
		foreach (StyleItemDef item in def.ItemsInCategory)
		{
			if (CanList(item, itemType))
			{
				if (!gender.HasValue)
				{
					gender = GetGender(item);
				}
				else if (GetGender(item) != gender)
				{
					return Gender.None;
				}
			}
		}
		return gender.GetValueOrDefault();
	}

	private StyleItemFrequency? TryGetFrequency(StyleItemCategoryDef def, ItemType itemType)
	{
		StyleItemFrequency? styleItemFrequency = null;
		foreach (StyleItemDef item in def.ItemsInCategory)
		{
			if (CanList(item, itemType))
			{
				if (!styleItemFrequency.HasValue)
				{
					styleItemFrequency = GetFrequency(item);
				}
				else if (GetFrequency(item) != styleItemFrequency)
				{
					return null;
				}
			}
		}
		return styleItemFrequency;
	}

	private Rect FrequencyPosition(Rect inRect, int index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Rect result = inRect;
		((Rect)(ref result)).width = (((Rect)(ref inRect)).width - 178f) / (float)allFrequencies.Length - 4f;
		((Rect)(ref result)).x = ((Rect)(ref inRect)).x + 178f + ((Rect)(ref result)).width * (float)index;
		return result;
	}

	private void Done()
	{
		if (editMode != 0)
		{
			foreach (KeyValuePair<HairDef, StyleItemSpawningProperties> hairFrequency in hairFrequencies)
			{
				ideo.style.SetFrequency(hairFrequency.Key, hairFrequency.Value.frequency);
				ideo.style.SetGender(hairFrequency.Key, hairFrequency.Value.gender);
			}
			foreach (KeyValuePair<BeardDef, StyleItemSpawningProperties> beardFrequency in beardFrequencies)
			{
				ideo.style.SetFrequency(beardFrequency.Key, beardFrequency.Value.frequency);
				ideo.style.SetGender(beardFrequency.Key, beardFrequency.Value.gender);
			}
			foreach (KeyValuePair<TattooDef, StyleItemSpawningProperties> tattooFrequency in tattooFrequencies)
			{
				ideo.style.SetFrequency(tattooFrequency.Key, tattooFrequency.Value.frequency);
				ideo.style.SetGender(tattooFrequency.Key, tattooFrequency.Value.gender);
			}
			ideo.style.EnsureAtLeastOneStyleItemAvailable();
		}
		Close();
	}
}
