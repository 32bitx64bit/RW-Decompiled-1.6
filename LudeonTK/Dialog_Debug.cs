using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LudeonTK;

public class Dialog_Debug : Window_Dev
{
	public static DebugActionNode rootNode;

	private DebugActionNode currentNode;

	private Dictionary<DebugTabMenuDef, DebugTabMenu> menus = new Dictionary<DebugTabMenuDef, DebugTabMenu>();

	private static Dictionary<DebugTabMenuDef, DebugActionNode> roots = new Dictionary<DebugTabMenuDef, DebugActionNode>();

	private List<DebugTabMenuDef> menuDefsSorted = new List<DebugTabMenuDef>();

	private DebugTabMenu currentTabMenu;

	private float totalOptionsHeight;

	private string filter;

	private bool focusFilter;

	private int currentHighlightIndex;

	private int prioritizedHighlightedIndex;

	private Vector2 scrollPosition;

	protected float curY;

	protected float curX;

	private int boundingRectCachedForFrame = -1;

	private Rect? boundingRectCached;

	private Rect? boundingRect;

	public float verticalSpacing = 2f;

	private float heightPerColumn;

	private const string FilterControlName = "DebugFilter";

	private const float DebugOptionsGap = 7f;

	private static readonly Color DisallowedColor = new Color(1f, 1f, 1f, 0.3f);

	private static readonly Vector2 FilterInputSize = new Vector2(200f, 30f);

	private const float AssumedBiggestElementHeight = 50f;

	private const float BackButtonWidth = 120f;

	private const float PinnableActionHeight = 22f;

	private const float TabHeight = 32f;

	private const float MaxTabWidth = 200f;

	public override bool IsDebug => true;

	public override Vector2 InitialSize => new Vector2((float)UI.screenWidth, (float)UI.screenHeight);

	public string Filter => filter;

	private float FilterX
	{
		get
		{
			if (currentNode?.parent == null || !currentNode.parent.IsRoot)
			{
				return 130f;
			}
			return 0f;
		}
	}

	private int HighlightedIndex => currentTabMenu.HighlightedIndex(currentHighlightIndex, prioritizedHighlightedIndex);

	public Rect? BoundingRectCached
	{
		get
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (boundingRectCachedForFrame != Time.frameCount)
			{
				if (boundingRect.HasValue)
				{
					Rect value = boundingRect.Value;
					Vector2 val = scrollPosition;
					((Rect)(ref value)).x = ((Rect)(ref value)).x + val.x;
					((Rect)(ref value)).y = ((Rect)(ref value)).y + val.y;
					boundingRectCached = value;
				}
				boundingRectCachedForFrame = Time.frameCount;
			}
			return boundingRectCached;
		}
	}

	public DebugActionNode CurrentNode => currentNode;

	public Dialog_Debug()
	{
		Setup();
		SwitchTab(DebugTabMenuDefOf.Actions);
	}

	public Dialog_Debug(DebugTabMenuDef def)
	{
		Setup();
		SwitchTab(def);
	}

	public void NewColumn(float columnWidth)
	{
		curY = 0f;
		curX += columnWidth + 17f;
	}

	protected void NewColumnIfNeeded(float columnWidth, float neededHeight)
	{
		if (curY + neededHeight > heightPerColumn)
		{
			NewColumn(columnWidth);
		}
	}

	public Rect GetRect(float height, float columnWidth, float widthPct = 1f)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		NewColumnIfNeeded(columnWidth, height);
		Rect result = new Rect(curX, curY, columnWidth * widthPct, height);
		curY += height;
		return result;
	}

	private void Setup()
	{
		forcePause = true;
		doCloseX = true;
		onlyOneOfTypeAllowed = true;
		absorbInputAroundWindow = true;
		focusFilter = true;
		menuDefsSorted.AddRange(DefDatabase<DebugTabMenuDef>.AllDefs.ToList());
		menuDefsSorted.SortBy((DebugTabMenuDef x) => x.displayOrder, (DebugTabMenuDef y) => y.label);
		currentTabMenu?.Recache();
	}

	public void SwitchTab(DebugTabMenuDef def)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		TrySetupNodeGraph();
		scrollPosition = Vector2.zero;
		currentHighlightIndex = 0;
		prioritizedHighlightedIndex = 0;
		currentTabMenu = (menus.ContainsKey(def) ? menus[def] : DebugTabMenu.CreateMenu(def, this, rootNode));
		currentTabMenu.Enter(roots[def]);
		SoundDefOf.RowTabSelect.PlayOneShotOnCamera();
	}

	public static void TrySetupNodeGraph()
	{
		if (rootNode != null)
		{
			return;
		}
		rootNode = new DebugActionNode("Root");
		foreach (DebugTabMenuDef allDef in DefDatabase<DebugTabMenuDef>.AllDefs)
		{
			roots.Add(allDef, DebugTabMenu.CreateMenu(allDef, null, rootNode).InitActions(rootNode));
		}
	}

	private void DrawTabs(Rect rect)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Min(((Rect)(ref rect)).width / (float)menuDefsSorted.Count, 200f);
		for (int i = 0; i < menuDefsSorted.Count; i++)
		{
			DebugTabMenuDef debugTabMenuDef = menuDefsSorted[i];
			Rect val = GenUI.ContractedBy(new Rect(((Rect)(ref rect)).x + (float)i * num, ((Rect)(ref rect)).y, num, ((Rect)(ref rect)).height), 1f);
			if (debugTabMenuDef == currentTabMenu.def)
			{
				GUI.DrawTexture(val, (Texture)(object)DevGUI.ButtonBackgroundClick);
				Text.Anchor = (TextAnchor)4;
				DevGUI.Label(val, debugTabMenuDef.LabelCap);
				Text.Anchor = (TextAnchor)0;
			}
			else if (DevGUI.ButtonText(val, debugTabMenuDef.LabelCap))
			{
				SwitchTab(debugTabMenuDef);
			}
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		if (KeyBindingDefOf.Dev_ChangeSelectedDebugAction.KeyDownEvent)
		{
			int highlightedIndex = HighlightedIndex;
			if (highlightedIndex >= 0)
			{
				for (int i = 0; i < currentTabMenu.Count; i++)
				{
					int index = (highlightedIndex + i + 1) % currentTabMenu.Count;
					if (FilterAllows(currentTabMenu.LabelAtIndex(index)))
					{
						prioritizedHighlightedIndex = index;
						break;
					}
				}
			}
		}
		GUI.SetNextControlName("DebugFilter");
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(FilterX, 0f, FilterInputSize.x, FilterInputSize.y);
		filter = DevGUI.TextField(rect, filter);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).xMax + 10f, ((Rect)(ref rect)).y, ((Rect)(ref inRect)).width - ((Rect)(ref rect)).width - 10f, 32f);
		DrawTabs(rect2);
		if (((int)Event.current.type == 4 || (int)Event.current.type == 7) && focusFilter)
		{
			GUI.FocusControl("DebugFilter");
			filter = string.Empty;
			focusFilter = false;
		}
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight = 0f;
		}
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(inRect);
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 42f;
		int num = (int)(InitialSize.x / 200f);
		heightPerColumn = Mathf.Max(((Rect)(ref outRect)).height, (totalOptionsHeight + 50f * (float)(num - 1)) / (float)num);
		curX = 0f;
		curY = 0f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, heightPerColumn);
		DevGUI.BeginScrollView(outRect, ref scrollPosition, val);
		DevGUI.BeginGroup(val);
		float columnWidth = (((Rect)(ref val)).width - 17f * (float)(num - 1)) / (float)num;
		currentTabMenu.ListOptions(HighlightedIndex, columnWidth);
		DevGUI.EndGroup();
		DevGUI.EndScrollView();
		if (currentNode.parent != null && !currentNode.parent.IsRoot)
		{
			GameFont font = Text.Font;
			Text.Font = GameFont.Small;
			if (DevGUI.ButtonText(new Rect(0f, 0f, 120f, 32f), "Back"))
			{
				currentNode.parent.Enter(this);
			}
			if (!currentNode.IsRoot)
			{
				Text.Anchor = (TextAnchor)2;
				Text.Font = GameFont.Tiny;
				DevGUI.Label(new Rect(0f, 0f, ((Rect)(ref outRect)).width - 24f - 10f, 32f), currentNode.Path.Colorize(ColoredText.SubtleGrayColor));
				Text.Anchor = (TextAnchor)0;
			}
			Text.Font = font;
		}
	}

	public override void OnAcceptKeyPressed()
	{
		if (GUI.GetNameOfFocusedControl() == "DebugFilter")
		{
			int highlightedIndex = HighlightedIndex;
			currentTabMenu.OnAcceptKeyPressed(highlightedIndex);
			Event.current.Use();
		}
	}

	public override void OnCancelKeyPressed()
	{
		if (currentNode.parent != null && !currentNode.parent.IsRoot)
		{
			currentNode.parent.Enter(this);
			Event.current.Use();
		}
		else
		{
			base.OnCancelKeyPressed();
		}
	}

	public static DebugActionNode GetNode(string path)
	{
		TrySetupNodeGraph();
		DebugActionNode debugActionNode = rootNode;
		string[] s = path.Split('\\');
		int i;
		for (i = 0; i < s.Length; i++)
		{
			DebugActionNode debugActionNode2 = debugActionNode.children.FirstOrDefault((DebugActionNode x) => x.label == s[i]);
			if (debugActionNode2 == null)
			{
				return null;
			}
			debugActionNode = debugActionNode2;
			debugActionNode.TrySetupChildren();
		}
		return debugActionNode;
	}

	public void SetCurrentNode(DebugActionNode node)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		currentNode = node;
		foreach (DebugActionNode child in currentNode.children)
		{
			child.DirtyLabelCache();
		}
		scrollPosition = Vector2.zero;
		filter = string.Empty;
		currentHighlightIndex = 0;
		prioritizedHighlightedIndex = 0;
		currentTabMenu?.Recache();
	}

	public void DrawNode(DebugActionNode node, float columnWidth, bool highlight)
	{
		if (node.settingsField != null)
		{
			DoCheckbox(node, columnWidth, highlight);
		}
		else
		{
			DoButton(node, columnWidth, highlight);
		}
	}

	public DebugActionButtonResult ButtonDebugPinnable(string label, float columnWidth, bool highlight, bool pinned)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(columnWidth, 22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, columnWidth - 22f, 22f);
		DebugActionButtonResult result = DebugActionButtonResult.None;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			result = DevGUI.ButtonDebugPinnable(rect, label, highlight, pinned);
		}
		curY += 22f + verticalSpacing;
		return result;
	}

	public DebugActionButtonResult CheckboxPinnable(string label, float columnWidth, ref bool checkOn, bool highlight, bool pinned)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		NewColumnIfNeeded(columnWidth, 22f);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curX, curY, columnWidth - 22f, 22f);
		DebugActionButtonResult result = DebugActionButtonResult.None;
		if (!BoundingRectCached.HasValue || ((Rect)(ref rect)).Overlaps(BoundingRectCached.Value))
		{
			result = DevGUI.CheckboxPinnable(rect, label, ref checkOn, highlight, pinned);
		}
		curY += 22f + verticalSpacing;
		return result;
	}

	private void DoButton(DebugActionNode node, float columnWidth, bool highlight)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		string labelNow = node.LabelNow;
		if (!FilterAllows(labelNow))
		{
			GUI.color = DisallowedColor;
		}
		switch (ButtonDebugPinnable(labelNow, columnWidth, highlight, Prefs.DebugActionsPalette.Contains(node.Path)))
		{
		case DebugActionButtonResult.ButtonPressed:
			node.Enter(this);
			break;
		case DebugActionButtonResult.PinPressed:
			Dialog_DevPalette.ToggleAction(node.Path);
			break;
		}
		GUI.color = Color.white;
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += 22f + verticalSpacing;
		}
	}

	private void DoCheckbox(DebugActionNode node, float columnWidth, bool highlight)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Invalid comparison between Unknown and I4
		string labelNow = node.LabelNow;
		FieldInfo settingsField = node.settingsField;
		bool checkOn = (bool)settingsField.GetValue(null);
		bool flag = checkOn;
		if (!FilterAllows(labelNow))
		{
			GUI.color = DisallowedColor;
		}
		switch (CheckboxPinnable(labelNow, columnWidth, ref checkOn, highlight, Prefs.DebugActionsPalette.Contains(node.Path)))
		{
		case DebugActionButtonResult.ButtonPressed:
			node.Enter(this);
			break;
		case DebugActionButtonResult.PinPressed:
			Dialog_DevPalette.ToggleAction(node.Path);
			break;
		}
		GUI.color = Color.white;
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += Text.LineHeight;
		}
		if (checkOn != flag)
		{
			settingsField.SetValue(null, checkOn);
			MethodInfo method = settingsField.DeclaringType.GetMethod(settingsField.Name + "Toggled", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				method.Invoke(null, null);
			}
		}
	}

	public void DoLabel(string label, float columnWidth)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		Text.Font = GameFont.Small;
		float num = Text.CalcHeight(label, columnWidth);
		DevGUI.Label(new Rect(curX, curY, columnWidth, num), label);
		curY += num;
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += num;
		}
	}

	public void DoGap(float gapSize = 7f)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		curY += gapSize;
		if ((int)Event.current.type == 8)
		{
			totalOptionsHeight += gapSize;
		}
	}

	public bool FilterAllows(string label)
	{
		if (filter.NullOrEmpty())
		{
			return true;
		}
		if (label.NullOrEmpty())
		{
			return true;
		}
		return label.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	public static void ResetStaticData()
	{
		rootNode = null;
		roots.Clear();
	}
}
