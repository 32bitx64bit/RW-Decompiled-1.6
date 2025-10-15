using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

public class Dialog_KeyBindings : Window
{
	protected Vector2 scrollPosition;

	protected float contentHeight;

	protected KeyPrefsData keyPrefsData;

	protected Vector2 WindowSize = new Vector2(700f, 760f);

	protected const float EntryHeight = 34f;

	protected const float CategoryHeadingHeight = 40f;

	private static List<KeyBindingDef> keyBindingsWorkingList = new List<KeyBindingDef>();

	public override Vector2 InitialSize => WindowSize;

	public Dialog_KeyBindings()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		forcePause = true;
		onlyOneOfTypeAllowed = true;
		absorbInputAroundWindow = true;
		scrollPosition = new Vector2(0f, 0f);
		keyPrefsData = KeyPrefs.KeyPrefsData.Clone();
		contentHeight = 0f;
		KeyBindingCategoryDef keyBindingCategoryDef = null;
		foreach (KeyBindingDef allDef in DefDatabase<KeyBindingDef>.AllDefs)
		{
			if (keyBindingCategoryDef != allDef.category)
			{
				keyBindingCategoryDef = allDef.category;
				contentHeight += 44f;
			}
			contentHeight += 34f;
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(120f, 40f);
		float y = val.y;
		Rect rect = GenUI.ContractedBy(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, ((Rect)(ref inRect)).height - (y + 10f)), 10f);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height + 10f, ((Rect)(ref rect)).width, y);
		Widgets.BeginGroup(rect);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, 0f, ((Rect)(ref rect)).width, 40f);
		Text.Font = GameFont.Medium;
		GenUI.SetLabelAlign((TextAnchor)4);
		Widgets.Label(rect3, "KeyboardConfig".Translate());
		GenUI.ResetLabelAlign();
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, ((Rect)(ref rect3)).height, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height - ((Rect)(ref rect3)).height);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, contentHeight);
		Widgets.BeginScrollView(outRect, ref scrollPosition, val2);
		float curY = 0f;
		KeyBindingCategoryDef keyBindingCategoryDef = null;
		keyBindingsWorkingList.Clear();
		keyBindingsWorkingList.AddRange(DefDatabase<KeyBindingDef>.AllDefs);
		keyBindingsWorkingList.SortBy((KeyBindingDef x) => x.category.index, (KeyBindingDef x) => x.index);
		for (int i = 0; i < keyBindingsWorkingList.Count; i++)
		{
			KeyBindingDef keyBindingDef = keyBindingsWorkingList[i];
			if (keyBindingCategoryDef != keyBindingDef.category)
			{
				bool skipDrawing = curY - scrollPosition.y + 40f < 0f || curY - scrollPosition.y > ((Rect)(ref outRect)).height;
				keyBindingCategoryDef = keyBindingDef.category;
				DrawCategoryEntry(keyBindingCategoryDef, ((Rect)(ref val2)).width, ref curY, skipDrawing);
			}
			bool skipDrawing2 = curY - scrollPosition.y + 34f < 0f || curY - scrollPosition.y > ((Rect)(ref outRect)).height;
			DrawKeyEntry(keyBindingDef, val2, ref curY, skipDrawing2);
		}
		Widgets.EndScrollView();
		Widgets.EndGroup();
		Widgets.BeginGroup(rect2);
		Rect rect4 = new Rect(0f, 0f, val.x, val.y);
		Rect rect5 = new Rect((((Rect)(ref rect2)).width - val.x) / 2f, 0f, val.x, val.y);
		Rect rect6 = default(Rect);
		((Rect)(ref rect6))._002Ector(((Rect)(ref rect2)).width - val.x, 0f, val.x, val.y);
		if (Widgets.ButtonText(rect5, "ResetButton".Translate()))
		{
			keyPrefsData.ResetToDefaults();
			keyPrefsData.ErrorCheck();
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			Event.current.Use();
		}
		if (Widgets.ButtonText(rect4, "CancelButton".Translate()))
		{
			Close();
			Event.current.Use();
		}
		if (Widgets.ButtonText(rect6, "OK".Translate()))
		{
			KeyPrefs.KeyPrefsData = keyPrefsData;
			KeyPrefs.Save();
			Close();
			keyPrefsData.ErrorCheck();
			Event.current.Use();
		}
		Widgets.EndGroup();
	}

	private void DrawCategoryEntry(KeyBindingCategoryDef category, float width, ref float curY, bool skipDrawing)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!skipDrawing)
		{
			Rect rect = GenUI.ContractedBy(new Rect(0f, curY, width, 40f), 4f);
			Text.Font = GameFont.Medium;
			Widgets.Label(rect, category.LabelCap);
			Text.Font = GameFont.Small;
			if (Mouse.IsOver(rect) && !category.description.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, new TipSignal(category.description));
			}
		}
		curY += 40f;
		if (!skipDrawing)
		{
			Color color = GUI.color;
			GUI.color = new Color(0.3f, 0.3f, 0.3f);
			Widgets.DrawLineHorizontal(0f, curY, width);
			GUI.color = color;
		}
		curY += 4f;
	}

	private void DrawKeyEntry(KeyBindingDef keyDef, Rect parentRect, ref float curY, bool skipDrawing)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (!skipDrawing)
		{
			Rect rect = GenUI.ContractedBy(new Rect(((Rect)(ref parentRect)).x, ((Rect)(ref parentRect)).y + curY, ((Rect)(ref parentRect)).width, 34f), 3f);
			GenUI.SetLabelAlign((TextAnchor)3);
			Widgets.Label(rect, keyDef.LabelCap);
			GenUI.ResetLabelAlign();
			float num = 4f;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(140f, 28f);
			Rect rect2 = new Rect(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - val.x * 2f - num, ((Rect)(ref rect)).y, val.x, val.y);
			Rect rect3 = default(Rect);
			((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x + ((Rect)(ref rect)).width - val.x, ((Rect)(ref rect)).y, val.x, val.y);
			string key = (SteamDeck.IsSteamDeckInNonKeyboardMode ? "BindingButtonToolTipController" : "BindingButtonToolTip");
			TooltipHandler.TipRegionByKey(rect2, key);
			TooltipHandler.TipRegionByKey(rect3, key);
			if (Widgets.ButtonText(rect2, keyPrefsData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.A).ToStringReadable()))
			{
				SettingButtonClicked(keyDef, KeyPrefs.BindingSlot.A);
			}
			if (Widgets.ButtonText(rect3, keyPrefsData.GetBoundKeyCode(keyDef, KeyPrefs.BindingSlot.B).ToStringReadable()))
			{
				SettingButtonClicked(keyDef, KeyPrefs.BindingSlot.B);
			}
		}
		curY += 34f;
	}

	private void SettingButtonClicked(KeyBindingDef keyDef, KeyPrefs.BindingSlot slot)
	{
		if (Event.current.button == 0)
		{
			Find.WindowStack.Add(new Dialog_DefineBinding(keyPrefsData, keyDef, slot));
			Event.current.Use();
		}
		else if (Event.current.button == 1)
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			list.Add(new FloatMenuOption("ResetBinding".Translate(), delegate
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				KeyCode keyCode = ((slot == KeyPrefs.BindingSlot.A) ? keyDef.defaultKeyCodeA : keyDef.defaultKeyCodeB);
				keyPrefsData.SetBinding(keyDef, slot, keyCode);
			}));
			list.Add(new FloatMenuOption("ClearBinding".Translate(), delegate
			{
				keyPrefsData.SetBinding(keyDef, slot, (KeyCode)0);
			}));
			Find.WindowStack.Add(new FloatMenu(list));
		}
	}
}
