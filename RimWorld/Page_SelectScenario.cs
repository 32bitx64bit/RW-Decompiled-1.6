using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Page_SelectScenario : Page
{
	private Scenario curScen;

	private Vector2 infoScrollPosition = Vector2.zero;

	private const float ScenarioEntryHeight = 68f;

	private static readonly Texture2D CanUploadIcon = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/CanUpload");

	private static readonly Texture2D WarningIcon = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");

	private Vector2 scenariosScrollPosition = Vector2.zero;

	private float totalScenarioListHeight;

	public override string PageTitle => "ChooseScenario".Translate();

	public override void PreOpen()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.PreOpen();
		infoScrollPosition = Vector2.zero;
		ScenarioLister.MarkDirty();
		EnsureValidSelection();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		DrawPageTitle(rect);
		Rect mainRect = GetMainRect(rect);
		Widgets.BeginGroup(mainRect);
		Rect rect2 = GenUI.Rounded(new Rect(0f, 0f, ((Rect)(ref mainRect)).width * 0.35f, ((Rect)(ref mainRect)).height));
		DoScenarioSelectionList(rect2);
		ScenarioUI.DrawScenarioInfo(GenUI.Rounded(new Rect(((Rect)(ref rect2)).xMax + 17f, 0f, ((Rect)(ref mainRect)).width - ((Rect)(ref rect2)).width - 17f, ((Rect)(ref mainRect)).height)), curScen, ref infoScrollPosition);
		Widgets.EndGroup();
		DoBottomButtons(rect, null, "ScenarioEditor".Translate(), GoToScenarioEditor);
	}

	private bool CanEditScenario(Scenario scen)
	{
		if (scen.Category == ScenarioCategory.CustomLocal)
		{
			return true;
		}
		if (scen.CanToUploadToWorkshop())
		{
			return true;
		}
		return false;
	}

	private void GoToScenarioEditor()
	{
		Page_ScenarioEditor page_ScenarioEditor = new Page_ScenarioEditor(CanEditScenario(curScen) ? curScen : curScen.CopyForEditing());
		page_ScenarioEditor.prev = this;
		Find.WindowStack.Add(page_ScenarioEditor);
		Close();
	}

	private void DoScenarioSelectionList(Rect rect)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax + 2f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f - 2f, totalScenarioListHeight + 250f);
		Widgets.BeginScrollView(rect, ref scenariosScrollPosition, val);
		Rect rect2 = val.AtZero();
		((Rect)(ref rect2)).height = 999999f;
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.ColumnWidth = ((Rect)(ref val)).width;
		listing_Standard.Begin(rect2);
		Text.Font = GameFont.Small;
		ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef));
		listing_Standard.Gap();
		Text.Font = GameFont.Small;
		listing_Standard.Label("ScenariosCustom".Translate());
		ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.CustomLocal));
		listing_Standard.Gap();
		Text.Font = GameFont.Small;
		listing_Standard.Label("ScenariosSteamWorkshop".Translate());
		if (listing_Standard.ButtonText("OpenSteamWorkshop".Translate()))
		{
			SteamUtility.OpenSteamWorkshopPage();
		}
		ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.SteamWorkshop));
		listing_Standard.End();
		totalScenarioListHeight = listing_Standard.CurHeight;
		Widgets.EndScrollView();
	}

	private void ListScenariosOnListing(Listing_Standard listing, IEnumerable<Scenario> scenarios)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (Scenario scenario in scenarios)
		{
			if (scenario.showInUI)
			{
				if (flag)
				{
					listing.Gap(6f);
				}
				Scenario scen = scenario;
				Rect rect = listing.GetRect(68f).ContractedBy(4f);
				DoScenarioListEntry(rect, scen);
				flag = true;
			}
		}
		if (!flag)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			listing.Label("(" + "NoneLower".Translate() + ")");
			GUI.color = Color.white;
		}
	}

	private void DoScenarioListEntry(Rect rect, Scenario scen)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		bool flag = curScen == scen;
		if (!scen.valid)
		{
			TooltipHandler.TipRegion(rect, "ScenPart_Error".Translate().Colorize(ColorLibrary.RedReadable));
		}
		Widgets.DrawOptionBackground(rect, flag);
		MouseoverSounds.DoRegion(rect);
		Rect val = rect.ContractedBy(4f);
		Text.Font = GameFont.Small;
		Rect rect2 = val;
		((Rect)(ref rect2)).height = Text.CalcHeight(scen.name, ((Rect)(ref rect2)).width);
		Widgets.Label(rect2, scen.name);
		Text.Font = GameFont.Tiny;
		Rect rect3 = val;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect2)).yMax;
		if (!Text.TinyFontSupported)
		{
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin - 6f;
			((Rect)(ref rect3)).height = ((Rect)(ref rect3)).height + 6f;
		}
		Widgets.Label(rect3, scen.GetSummary());
		if (!scen.enabled)
		{
			return;
		}
		WidgetRow widgetRow = new WidgetRow(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).y, UIDirection.LeftThenDown);
		if (scen.Category == ScenarioCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.Delete, "Delete".Translate(), GenUI.SubtleMouseoverColor))
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(scen.File.Name), delegate
			{
				scen.File.Delete();
				ScenarioLister.MarkDirty();
			}, destructive: true));
		}
		if (scen.Category == ScenarioCategory.SteamWorkshop && widgetRow.ButtonIcon(TexButton.Delete, "Unsubscribe".Translate(), GenUI.SubtleMouseoverColor))
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmUnsubscribeFrom".Translate(scen.File.Name), delegate
			{
				scen.enabled = false;
				if (curScen == scen)
				{
					curScen = null;
					EnsureValidSelection();
				}
				Workshop.Unsubscribe((WorkshopUploadable)scen);
			}, destructive: true));
		}
		if (scen.GetPublishedFileId() != PublishedFileId_t.Invalid)
		{
			if (widgetRow.ButtonIcon(ContentSource.SteamWorkshop.GetIcon(), "WorkshopPage".Translate()))
			{
				SteamUtility.OpenWorkshopPage(scen.GetPublishedFileId());
			}
			if (scen.CanToUploadToWorkshop())
			{
				widgetRow.Icon((Texture)(object)CanUploadIcon, "CanBeUpdatedOnWorkshop".Translate());
			}
		}
		if (!scen.valid)
		{
			widgetRow.Icon((Texture)(object)WarningIcon, null, 3f);
		}
		if (flag || !Widgets.ButtonInvisible(rect))
		{
			return;
		}
		SoundDefOf.Click.PlayOneShotOnCamera();
		if (!scen.valid)
		{
			PreLoadUtility.CheckVersionAndLoad(scen.File.FullName, ScribeMetaHeaderUtility.ScribeHeaderMode.Scenario, delegate
			{
				curScen = scen;
			});
		}
		else
		{
			curScen = scen;
		}
	}

	protected override bool CanDoNext()
	{
		if (!base.CanDoNext())
		{
			return false;
		}
		if (curScen == null)
		{
			return false;
		}
		BeginScenarioConfiguration(curScen, this);
		return true;
	}

	public static void BeginScenarioConfiguration(Scenario scen, Page originPage)
	{
		Game.ClearCaches();
		Current.Game = new Game();
		Current.Game.InitData = new GameInitData();
		Current.Game.Scenario = scen;
		Current.Game.Scenario.PreConfigure();
		Find.GameInitData.startedFromEntry = true;
		Page firstConfigPage = Current.Game.Scenario.GetFirstConfigPage();
		if (firstConfigPage == null)
		{
			PageUtility.InitGameStart();
			return;
		}
		originPage.next = firstConfigPage;
		firstConfigPage.prev = originPage;
	}

	private void EnsureValidSelection()
	{
		if (curScen == null || !ScenarioLister.ScenarioIsListedAnywhere(curScen))
		{
			curScen = ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef).FirstOrDefault();
		}
	}

	internal void Notify_ScenarioListChanged()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		PublishedFileId_t selModId = curScen.GetPublishedFileId();
		curScen = ScenarioLister.AllScenarios().FirstOrDefault((Scenario sc) => sc.GetPublishedFileId() == selModId);
		EnsureValidSelection();
	}

	internal void Notify_SteamItemUnsubscribed(PublishedFileId_t pfid)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (curScen != null && curScen.GetPublishedFileId() == pfid)
		{
			curScen = null;
		}
		EnsureValidSelection();
	}
}
