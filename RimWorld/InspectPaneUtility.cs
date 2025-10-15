using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class InspectPaneUtility
{
	private static Dictionary<string, string> truncatedLabelsCached = new Dictionary<string, string>();

	public const float TabWidth = 72f;

	public const float TabHeight = 30f;

	private static readonly Texture2D InspectTabButtonFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.07450981f, 0.08627451f, 9f / 85f, 1f));

	public const float CornerButtonsSize = 24f;

	public const float PaneInnerMargin = 12f;

	public const float PaneHeight = 165f;

	private const int TabMinimum = 6;

	private static List<Thing> selectedThings = new List<Thing>();

	public static void Reset()
	{
		truncatedLabelsCached.Clear();
	}

	public static float PaneWidthFor(IInspectPane pane)
	{
		if (pane == null)
		{
			return 432f;
		}
		int visible = 0;
		if (pane.CurTabs != null)
		{
			if (pane.CurTabs is IList list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Process((InspectTabBase)list[i]);
				}
			}
			else
			{
				foreach (InspectTabBase curTab in pane.CurTabs)
				{
					Process(curTab);
				}
			}
		}
		return 72f * (float)Mathf.Max(6, visible);
		void Process(InspectTabBase tab)
		{
			if (tab.IsVisible && !tab.Hidden)
			{
				visible++;
			}
		}
	}

	public static Vector2 PaneSizeFor(IInspectPane pane)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(PaneWidthFor(pane), 165f);
	}

	public static bool CanInspectTogether(object A, object B)
	{
		Thing thing = A as Thing;
		Thing thing2 = B as Thing;
		if (thing == null || thing2 == null)
		{
			return false;
		}
		if (thing.def.category == ThingCategory.Pawn)
		{
			return false;
		}
		return thing.def == thing2.def;
	}

	public static string AdjustedLabelFor(List<object> selected, Rect rect)
	{
		string text = "";
		if (selected[0] is IRenameable renameable)
		{
			if (selected.Count == 1)
			{
				text = renameable.InspectLabel;
			}
			else
			{
				string baseLabel = renameable.BaseLabel;
				bool flag = true;
				for (int i = 1; i < selected.Count; i++)
				{
					if (selected[i] is IRenameable renameable2 && renameable2.BaseLabel != baseLabel)
					{
						flag = false;
						break;
					}
				}
				text = ((!flag) ? ((string)"VariousLabel".Translate()) : renameable.BaseLabel);
				text = text + " x" + selected.Count;
			}
		}
		else
		{
			selectedThings.Clear();
			for (int j = 0; j < selected.Count; j++)
			{
				if (selected[j] is Thing outerThing)
				{
					selectedThings.Add(outerThing.GetInnerIfMinified());
				}
			}
			if (selectedThings.Count == 1)
			{
				text = selectedThings[0].LabelCap;
			}
			else if (selectedThings.Count > 1)
			{
				string text2 = PluralLabelFor(selectedThings[0]);
				bool flag2 = true;
				for (int k = 1; k < selectedThings.Count; k++)
				{
					if (PluralLabelFor(selectedThings[k]) != text2)
					{
						flag2 = false;
						break;
					}
				}
				text = ((!flag2) ? ((string)"VariousLabel".Translate()) : PluralLabelFor(selectedThings[0]).CapitalizeFirst());
				int num = 0;
				for (int l = 0; l < selectedThings.Count; l++)
				{
					num += selectedThings[l].stackCount;
				}
				text = text + " x" + num.ToStringCached();
			}
			else
			{
				text = "?";
			}
			selectedThings.Clear();
		}
		using (new TextBlock(GameFont.Medium))
		{
			return text.Truncate(((Rect)(ref rect)).width, truncatedLabelsCached);
		}
	}

	private static string PluralLabelFor(Thing thing)
	{
		if (thing is Pawn { IsMutant: not false } pawn)
		{
			return pawn.mutant.Def.label;
		}
		return thing.def.label;
	}

	public static void ExtraOnGUI(IInspectPane pane)
	{
		if (pane.AnythingSelected)
		{
			if (KeyBindingDefOf.SelectNextInCell.KeyDownEvent)
			{
				pane.SelectNextInCell();
			}
			DoTabs(pane);
		}
	}

	public static void UpdateTabs(IInspectPane pane)
	{
		bool tabUpdated = false;
		if (pane.CurTabs != null)
		{
			if (pane.CurTabs is IList list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					Update((InspectTabBase)list[i]);
				}
			}
			else
			{
				foreach (InspectTabBase curTab in pane.CurTabs)
				{
					Update(curTab);
				}
			}
		}
		if (!tabUpdated)
		{
			pane.CloseOpenTab();
		}
		void Update(InspectTabBase tab)
		{
			if (tab.IsVisible && tab.GetType() == pane.OpenTabType)
			{
				tab.TabUpdate();
				tabUpdated = true;
			}
		}
	}

	public static void InspectPaneOnGUI(Rect inRect, IInspectPane pane)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		pane.RecentHeight = 165f;
		if (!pane.AnythingSelected)
		{
			return;
		}
		try
		{
			Rect rect = inRect.ContractedBy(12f);
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin - 4f;
			((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax + 6f;
			Widgets.BeginGroup(rect);
			float lineEndWidth = 0f;
			if (pane.ShouldShowSelectNextInCellButton)
			{
				Rect val = default(Rect);
				((Rect)(ref val))._002Ector(((Rect)(ref rect)).width - 24f, 0f, 24f, 24f);
				MouseoverSounds.DoRegion(val);
				if (Widgets.ButtonImage(val, TexButton.SelectOverlappingNext))
				{
					pane.SelectNextInCell();
				}
				lineEndWidth += 24f;
				if (SteamDeck.IsSteamDeckInNonKeyboardMode)
				{
					TooltipHandler.TipRegionByKey(val, "SelectNextInSquareTipController");
				}
				else
				{
					TooltipHandler.TipRegionByKey(val, "SelectNextInSquareTip", KeyBindingDefOf.SelectNextInCell.MainKeyLabel);
				}
			}
			pane.DoInspectPaneButtons(rect, ref lineEndWidth);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(0f, 0f, ((Rect)(ref rect)).width - lineEndWidth, 50f);
			string label = pane.GetLabel(rect2);
			((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width + 300f;
			using (new TextBlock(GameFont.Medium, (TextAnchor)0))
			{
				Widgets.Label(rect2, label);
			}
			if (pane.ShouldShowPaneContents)
			{
				Rect rect3 = rect.AtZero();
				((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 26f;
				pane.DoPaneContents(rect3);
			}
		}
		catch (Exception ex)
		{
			Log.Error("Exception doing inspect pane: " + ex.ToString());
		}
		finally
		{
			Widgets.EndGroup();
		}
	}

	private static void DoTabs(IInspectPane pane)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			float tabsTopY = pane.PaneTopY - 30f;
			float curTabX = PaneWidthFor(pane) - 72f;
			float leftEdge = 0f;
			bool drewOpen = false;
			if (pane.CurTabs != null)
			{
				if (pane.CurTabs is IList list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Do((InspectTabBase)list[i]);
					}
				}
				else
				{
					foreach (InspectTabBase curTab in pane.CurTabs)
					{
						Do(curTab);
					}
				}
			}
			if (drewOpen)
			{
				GUI.DrawTexture(new Rect(0f, tabsTopY, leftEdge, 30f), (Texture)(object)InspectTabButtonFillTex);
			}
			void Do(InspectTabBase tab)
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				if (tab.IsVisible)
				{
					bool flag = tab.GetType() == pane.OpenTabType;
					if (!tab.Hidden)
					{
						Rect rect = default(Rect);
						((Rect)(ref rect))._002Ector(curTabX, tabsTopY, 72f, 30f);
						leftEdge = curTabX;
						Text.Font = GameFont.Small;
						if (Widgets.ButtonText(rect, tab.labelKey.Translate()))
						{
							InterfaceToggleTab(tab, pane);
						}
						if (!flag && !tab.TutorHighlightTagClosed.NullOrEmpty())
						{
							UIHighlighter.HighlightOpportunity(rect, tab.TutorHighlightTagClosed);
						}
						curTabX -= 72f;
					}
					if (flag)
					{
						tab.DoTabGUI();
						pane.RecentHeight = 700f;
						drewOpen = true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.ErrorOnce(ex.ToString(), 742783);
		}
	}

	private static bool IsOpen(InspectTabBase tab, IInspectPane pane)
	{
		return tab.GetType() == pane.OpenTabType;
	}

	private static void ToggleTab(InspectTabBase tab, IInspectPane pane)
	{
		if (IsOpen(tab, pane) || (tab == null && pane.OpenTabType == null))
		{
			pane.OpenTabType = null;
			SoundDefOf.TabClose.PlayOneShotOnCamera();
		}
		else
		{
			tab.OnOpen();
			pane.OpenTabType = tab.GetType();
			SoundDefOf.TabOpen.PlayOneShotOnCamera();
		}
	}

	public static InspectTabBase OpenTab(Type inspectTabType)
	{
		MainTabWindow_Inspect mainTabWindow_Inspect = (MainTabWindow_Inspect)MainButtonDefOf.Inspect.TabWindow;
		InspectTabBase tab = null;
		if (mainTabWindow_Inspect.CurTabs != null)
		{
			if (mainTabWindow_Inspect.CurTabs is IList list)
			{
				for (int i = 0; i < list.Count && !Find((InspectTabBase)list[i]); i++)
				{
				}
			}
			else
			{
				using IEnumerator<InspectTabBase> enumerator = mainTabWindow_Inspect.CurTabs.GetEnumerator();
				while (enumerator.MoveNext() && !Find(enumerator.Current))
				{
				}
			}
		}
		if (tab != null)
		{
			if (Find.MainTabsRoot.OpenTab != MainButtonDefOf.Inspect)
			{
				Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Inspect);
			}
			if (!IsOpen(tab, mainTabWindow_Inspect))
			{
				ToggleTab(tab, mainTabWindow_Inspect);
			}
		}
		return tab;
		bool Find(InspectTabBase t)
		{
			if (inspectTabType.IsAssignableFrom(t.GetType()))
			{
				tab = t;
				return true;
			}
			return false;
		}
	}

	private static void InterfaceToggleTab(InspectTabBase tab, IInspectPane pane)
	{
		if (!TutorSystem.TutorialMode || IsOpen(tab, pane) || TutorSystem.AllowAction("ITab-" + tab.tutorTag + "-Open"))
		{
			ToggleTab(tab, pane);
		}
	}
}
