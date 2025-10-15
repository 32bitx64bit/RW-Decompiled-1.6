using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Steamworks;
using UnityEngine;

namespace Verse;

public class ModSummaryWindow
{
	private static Vector2 modListScrollPos;

	private static float modListLastHeight;

	private static readonly Vector2 WindowSize = new Vector2(776f, 410f);

	private static readonly Vector2 ListElementSize = new Vector2(238f, 36f);

	private const float WindowHeightCollapsed = 226f;

	private const float ExpansionListHeight = 94f;

	private const float ModListHeight = 224f;

	private const float ModListHeightCollapsed = 40f;

	private const float ListElementIconSize = 32f;

	private static readonly Color DisabledIconTint = new Color(0.35f, 0.35f, 0.35f);

	private static readonly Color ModInfoListBackground = new Color(0.13f, 0.13f, 0.13f);

	private static readonly Color ModInfoListItemBackground = new Color(0.32f, 0.32f, 0.32f);

	private static readonly Color ModInfoListItemBackgroundIncompatible = new Color(0.31f, 0.29f, 0.15f);

	private static readonly Color ModInfoListItemBackgroundDisabled = new Color(0.1f, 0.1f, 0.1f);

	private static bool AnyMods => ModLister.AllInstalledMods.Any((ModMetaData m) => !m.Official && m.Active);

	public static void DrawWindow(Vector2 offset, bool useWindowStack)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(offset.x, offset.y, WindowSize.x, GetEffectiveSize().y);
		if (useWindowStack)
		{
			Find.WindowStack.ImmediateWindow(62893996, rect, WindowLayer.Super, delegate
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				DrawContents(rect.AtZero());
			});
		}
		else
		{
			Widgets.DrawShadowAround(rect);
			Widgets.DrawWindowBackground(rect);
			DrawContents(rect);
		}
	}

	private static void DrawContents(Rect rect)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		float num = 0f;
		float num2 = 17f;
		float itemListInnerMargin = 8f;
		float num3 = num2 + 4f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + num2, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - num2 * 2f, 0f);
		Rect rect2 = rect;
		((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + num3;
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 10f;
		Widgets.Label(rect2, "OfficialContent".Translate());
		num += 10f + Text.LineHeight + 4f;
		Rect rect3 = val;
		((Rect)(ref rect3)).y = ((Rect)(ref rect3)).y + num;
		((Rect)(ref rect3)).height = 94f;
		Widgets.DrawBoxSolid(rect3, ModInfoListBackground);
		num += 104f;
		List<GenUI.AnonymousStackElement> list = new List<GenUI.AnonymousStackElement>();
		Text.Anchor = (TextAnchor)3;
		for (int i = 0; i < ModLister.AllExpansions.Count; i++)
		{
			ExpansionDef exp = ModLister.AllExpansions[i];
			list.Add(new GenUI.AnonymousStackElement
			{
				drawer = delegate(Rect r)
				{
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_006c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0065: Unknown result type (might be due to invalid IL or missing references)
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
					//IL_017e: Unknown result type (might be due to invalid IL or missing references)
					//IL_018f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0199: Unknown result type (might be due to invalid IL or missing references)
					//IL_021e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0150: Unknown result type (might be due to invalid IL or missing references)
					bool flag = exp.Status == ExpansionStatus.Active;
					Widgets.DrawBoxSolid(r, flag ? ModInfoListItemBackground : ModInfoListItemBackgroundDisabled);
					Widgets.DrawHighlightIfMouseover(r);
					if (!exp.isCore && !exp.StoreURL.NullOrEmpty() && Widgets.ButtonInvisible(r))
					{
						SteamUtility.OpenUrl(exp.StoreURL);
					}
					GUI.color = (flag ? Color.white : DisabledIconTint);
					Material material = (flag ? null : TexUI.GrayscaleGUI);
					Rect rect7 = new Rect(((Rect)(ref r)).x + itemListInnerMargin, ((Rect)(ref r)).y + 2f, 32f, 32f);
					float num5 = 42f;
					GenUI.DrawTextureWithMaterial(rect7, (Texture)(object)exp.IconFromStatus, material);
					GUI.color = (flag ? Color.white : Color.grey);
					Rect rect8 = default(Rect);
					((Rect)(ref rect8))._002Ector(((Rect)(ref r)).x + itemListInnerMargin + num5, ((Rect)(ref r)).y, ((Rect)(ref r)).width - num5, ((Rect)(ref r)).height);
					if (exp.Status != 0)
					{
						TaggedString taggedString = ((exp.Status == ExpansionStatus.Installed) ? "DisabledLower" : "ContentNotInstalled").Translate().ToLower();
						Widgets.Label(rect8, exp.label + " (" + taggedString + ")");
					}
					else
					{
						Widgets.Label(rect8, exp.label);
					}
					GUI.color = Color.white;
					if (Mouse.IsOver(r))
					{
						string description2 = exp.label + "\n" + exp.StatusDescription + "\n\n" + exp.description.StripTags();
						TooltipHandler.TipRegion(tip: new TipSignal(() => description2, exp.GetHashCode() * 37), rect: r);
					}
				}
			});
		}
		GenUI.DrawElementStackVertical(new Rect(((Rect)(ref rect3)).x + itemListInnerMargin, ((Rect)(ref rect3)).y + itemListInnerMargin, ((Rect)(ref rect3)).width - itemListInnerMargin * 2f, 94f), ListElementSize.y, list, delegate(Rect r, GenUI.AnonymousStackElement obj)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			obj.drawer(r);
		}, (GenUI.AnonymousStackElement obj) => ListElementSize.x, 6f);
		list.Clear();
		Rect rect4 = rect;
		((Rect)(ref rect4)).x = ((Rect)(ref rect4)).x + num3;
		((Rect)(ref rect4)).y = ((Rect)(ref rect4)).y + num;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(rect4, "Mods".Translate());
		num += Text.LineHeight + 4f;
		Rect val2 = val;
		((Rect)(ref val2)).y = ((Rect)(ref val2)).y + num;
		((Rect)(ref val2)).height = (AnyMods ? 224f : 40f);
		Widgets.DrawBoxSolid(val2, ModInfoListBackground);
		if (AnyMods)
		{
			Text.Anchor = (TextAnchor)3;
			foreach (ModMetaData mod in ModLister.AllInstalledMods.Where((ModMetaData m) => !m.Official && m.Active))
			{
				list.Add(new GenUI.AnonymousStackElement
				{
					drawer = delegate(Rect r)
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_0015: Unknown result type (might be due to invalid IL or missing references)
						//IL_000e: Unknown result type (might be due to invalid IL or missing references)
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0038: Unknown result type (might be due to invalid IL or missing references)
						//IL_003d: Unknown result type (might be due to invalid IL or missing references)
						//IL_011a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0121: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
						//IL_0058: Unknown result type (might be due to invalid IL or missing references)
						//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
						//IL_0184: Unknown result type (might be due to invalid IL or missing references)
						Widgets.DrawBoxSolid(r, mod.VersionCompatible ? ModInfoListItemBackground : ModInfoListItemBackgroundIncompatible);
						Widgets.DrawHighlightIfMouseover(r);
						if (mod.OnSteamWorkshop && mod.GetPublishedFileId() != PublishedFileId_t.Invalid && Widgets.ButtonInvisible(r))
						{
							SteamUtility.OpenWorkshopPage(mod.GetPublishedFileId());
						}
						Rect val4 = default(Rect);
						((Rect)(ref val4))._002Ector(((Rect)(ref r)).x + itemListInnerMargin, ((Rect)(ref r)).y + 2f, 32f, 32f);
						float num4 = 42f;
						if (!mod.Icon.NullOrBad())
						{
							GUI.DrawTexture(val4, (Texture)(object)mod.Icon);
						}
						Rect rect6 = default(Rect);
						((Rect)(ref rect6))._002Ector(((Rect)(ref r)).x + itemListInnerMargin + num4, ((Rect)(ref r)).y, ((Rect)(ref r)).width - num4, ((Rect)(ref r)).height);
						string label = mod.Name.Truncate(((Rect)(ref rect6)).width - itemListInnerMargin - 4f);
						Widgets.Label(rect6, label);
						if (Mouse.IsOver(r))
						{
							string description = mod.Name + "\n\n" + mod.Description.StripTags();
							if (!mod.VersionCompatible)
							{
								description = description + "\n\n" + "ModNotMadeForThisVersionShort".Translate().Colorize(Color.yellow);
							}
							TooltipHandler.TipRegion(tip: new TipSignal(() => description, mod.GetHashCode() * 37), rect: r);
						}
						GUI.color = Color.white;
					}
				});
			}
			Widgets.BeginScrollView(val2, ref modListScrollPos, new Rect(0f, 0f, ((Rect)(ref val2)).width - 16f, modListLastHeight + itemListInnerMargin * 2f));
			Rect val3 = GenUI.DrawElementStack(new Rect(itemListInnerMargin, itemListInnerMargin, ((Rect)(ref val2)).width - itemListInnerMargin * 2f, 99999f), ListElementSize.y, list, delegate(Rect r, GenUI.AnonymousStackElement obj)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				obj.drawer(r);
			}, (GenUI.AnonymousStackElement obj) => ListElementSize.x, 6f);
			modListLastHeight = ((Rect)(ref val3)).height;
			Widgets.EndScrollView();
		}
		else
		{
			Text.Anchor = (TextAnchor)0;
			Rect rect5 = val2;
			((Rect)(ref rect5)).x = ((Rect)(ref rect5)).x + itemListInnerMargin;
			((Rect)(ref rect5)).y = ((Rect)(ref rect5)).y + itemListInnerMargin;
			GUI.color = Color.gray;
			Widgets.Label(rect5, "None".Translate());
			GUI.color = Color.white;
		}
		Text.Anchor = (TextAnchor)0;
	}

	public static Vector2 GetEffectiveSize()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(WindowSize.x, AnyMods ? WindowSize.y : 226f);
	}
}
