using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Dialog_Options : Window
{
	public OptionCategoryDef selectedCategory = OptionCategoryDefOf.General;

	public Mod selectedMod;

	private bool hasModSettings;

	private Vector2 categoryScrollPosition;

	private Vector2 optionsScrollPosition;

	private float optionsViewRectHeight;

	private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private string modFilter = "";

	private IEnumerable<Mod> cachedModsWithSettings;

	private const float CategoryListWidth = 160f;

	private const float CategoryRowHeight = 48f;

	private const float CategoryRowSpacing = 2f;

	private const float CategoryIconSize = 20f;

	private const float BottomRowHeight = 50f;

	private const float ListingVerticalSpacing = 5f;

	private const float SearchBarHeight = 30f;

	private const float ModIconSize = 32f;

	private static readonly Vector2 OkayButtonSize = new Vector2(160f, 40f);

	public static readonly float[] UIScales = new float[9] { 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 3.5f, 4f };

	public static readonly Texture2D RandomBackgroundTex = ContentFinder<Texture2D>.Get("UI/Icons/RandomBackground");

	public static readonly Texture2D LanguageIconTex = ContentFinder<Texture2D>.Get("UI/Icons/Language");

	private Dictionary<string, string> modOptionTruncationCache = new Dictionary<string, string>();

	public override Vector2 InitialSize => new Vector2(650f, 600f);

	public Dialog_Options(OptionCategoryDef initialCategory)
		: this()
	{
		selectedCategory = initialCategory;
		selectedMod = null;
	}

	public Dialog_Options(Mod initialMod)
		: this()
	{
		selectedCategory = null;
		selectedMod = initialMod;
	}

	public Dialog_Options()
	{
		doCloseX = true;
		forcePause = true;
		absorbInputAroundWindow = true;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		quickSearchWidget.Reset();
		cachedModsWithSettings = from mod in LoadedModManager.ModHandles
			where !mod.SettingsCategory().NullOrEmpty()
			orderby mod.SettingsCategory()
			select mod;
		foreach (OptionCategoryDef allDef in DefDatabase<OptionCategoryDef>.AllDefs)
		{
			if (!allDef.modContentPack.IsOfficialMod)
			{
				Log.Error("Unofficial OptionCategoryDef: " + allDef.label + " found, ignoring");
			}
		}
		hasModSettings = LoadedModManager.ModHandles.Any((Mod mod) => !mod.SettingsCategory().NullOrEmpty());
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		IEnumerable<OptionCategoryDef> allDefs = DefDatabase<OptionCategoryDef>.AllDefs;
		Text.Anchor = (TextAnchor)3;
		int num = 0;
		Rect rect = default(Rect);
		foreach (OptionCategoryDef item in allDefs)
		{
			if ((Prefs.DevMode || !item.isDev) && item.modContentPack.IsOfficialMod && (item != OptionCategoryDefOf.Mods || hasModSettings))
			{
				((Rect)(ref rect))._002Ector(0f, (float)num * 50f, 160f, 48f);
				DoCategoryRow(rect.ContractedBy(4f), item);
				num++;
			}
		}
		Text.Anchor = (TextAnchor)0;
		float num2 = 60f;
		Rect inRect2 = default(Rect);
		((Rect)(ref inRect2))._002Ector(177f, 0f, ((Rect)(ref inRect)).width - 160f - 17f, ((Rect)(ref inRect)).height - num2);
		DoOptions(selectedCategory, inRect2);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).width / 2f - OkayButtonSize.x / 2f, ((Rect)(ref inRect)).yMax - OkayButtonSize.y, OkayButtonSize.x, OkayButtonSize.y), "OK".Translate()))
		{
			Close();
		}
		Text.Anchor = (TextAnchor)0;
	}

	private void DoCategoryRow(Rect r, OptionCategoryDef optionCategory)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawOptionBackground(r, optionCategory == selectedCategory);
		if (Widgets.ButtonInvisible(r))
		{
			selectedCategory = optionCategory;
			selectedMod = null;
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		float num = ((Rect)(ref r)).x + 10f;
		Rect val = new Rect(num, ((Rect)(ref r)).y + (((Rect)(ref r)).height - 20f) / 2f, 20f, 20f);
		Texture2D val2 = ContentFinder<Texture2D>.Get(optionCategory.texPath);
		GUI.DrawTexture(val, (Texture)(object)val2);
		num += 30f;
		Widgets.Label(new Rect(num, ((Rect)(ref r)).y, ((Rect)(ref r)).width - num, ((Rect)(ref r)).height), optionCategory.label.CapitalizeFirst());
	}

	private void DoOptions(OptionCategoryDef category, Rect inRect)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = optionsViewRectHeight > ((Rect)(ref inRect)).height;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(inRect);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width - (flag ? 26f : 0f), optionsViewRectHeight);
		Widgets.BeginScrollView(outRect, ref optionsScrollPosition, viewRect);
		Listing_Standard listing_Standard = new Listing_Standard();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref viewRect)).x, ((Rect)(ref viewRect)).y, ((Rect)(ref viewRect)).width, 999999f);
		listing_Standard.Begin(rect);
		listing_Standard.verticalSpacing = 5f;
		listing_Standard.Gap();
		if (category == OptionCategoryDefOf.General)
		{
			DoGeneralOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Graphics)
		{
			DoVideoOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Audio)
		{
			DoAudioOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Gameplay)
		{
			DoGameplayOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Interface)
		{
			DoUIOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Controls)
		{
			DoControlsOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Dev)
		{
			DoDevOptions(listing_Standard);
		}
		else if (category == OptionCategoryDefOf.Mods)
		{
			DoModOptions(listing_Standard);
		}
		if (selectedMod != null)
		{
			float num = 15f;
			Rect inRect2 = default(Rect);
			((Rect)(ref inRect2))._002Ector(0f, num, ((Rect)(ref viewRect)).width, ((Rect)(ref inRect)).height - num);
			selectedMod?.DoSettingsWindowContents(inRect2);
		}
		optionsViewRectHeight = listing_Standard.CurHeight;
		listing_Standard.End();
		Widgets.EndScrollView();
	}

	private void DoGeneralOptions(Listing_Standard listing)
	{
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Invalid comparison between Unknown and I4
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Invalid comparison between Unknown and I4
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		if (listing.ButtonTextLabeledPct("ChooseLanguage".Translate(), LanguageDatabase.activeLanguage.DisplayName, 0.6f, (TextAnchor)3, null, null, LanguageIconTex))
		{
			if (Current.ProgramState == ProgramState.Playing)
			{
				Messages.Message("ChangeLanguageFromMainMenu".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
			else
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (LoadedLanguage allLoadedLanguage in LanguageDatabase.AllLoadedLanguages)
				{
					LoadedLanguage localLang = allLoadedLanguage;
					list.Add(new FloatMenuOption(localLang.DisplayName, delegate
					{
						LanguageDatabase.SelectLanguage(localLang);
					}));
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}
		float autosaveIntervalDays = Prefs.AutosaveIntervalDays;
		string text = "Days".Translate();
		string text2 = "day".Translate();
		if (listing.ButtonTextLabeledPct("AutosaveInterval".Translate(), autosaveIntervalDays + " " + ((autosaveIntervalDays == 1f) ? text2 : text), 0.6f, (TextAnchor)3, null, "AutosaveIntervalTooltip".Translate()))
		{
			List<FloatMenuOption> list2 = new List<FloatMenuOption>();
			if (Prefs.DevMode)
			{
				list2.Add(new FloatMenuOption("0.05 " + text + "(debug)", delegate
				{
					Prefs.AutosaveIntervalDays = 0.05f;
				}));
				list2.Add(new FloatMenuOption("0.075 " + text + "(debug)", delegate
				{
					Prefs.AutosaveIntervalDays = 0.075f;
				}));
				list2.Add(new FloatMenuOption("0.1 " + text + "(debug)", delegate
				{
					Prefs.AutosaveIntervalDays = 0.1f;
				}));
				list2.Add(new FloatMenuOption("0.125 " + text + "(debug)", delegate
				{
					Prefs.AutosaveIntervalDays = 0.125f;
				}));
				list2.Add(new FloatMenuOption("0.25 " + text + "(debug)", delegate
				{
					Prefs.AutosaveIntervalDays = 0.25f;
				}));
			}
			list2.Add(new FloatMenuOption("0.5 " + text, delegate
			{
				Prefs.AutosaveIntervalDays = 0.5f;
			}));
			list2.Add(new FloatMenuOption(1 + " " + text2, delegate
			{
				Prefs.AutosaveIntervalDays = 1f;
			}));
			list2.Add(new FloatMenuOption(3 + " " + text, delegate
			{
				Prefs.AutosaveIntervalDays = 3f;
			}));
			list2.Add(new FloatMenuOption(7 + " " + text, delegate
			{
				Prefs.AutosaveIntervalDays = 7f;
			}));
			list2.Add(new FloatMenuOption(14 + " " + text, delegate
			{
				Prefs.AutosaveIntervalDays = 14f;
			}));
			Find.WindowStack.Add(new FloatMenu(list2));
		}
		if (Current.ProgramState == ProgramState.Playing && Current.Game.Info.permadeathMode && Prefs.AutosaveIntervalDays > 1f)
		{
			GUI.color = Color.red;
			listing.Label("MaxPermadeathAutosaveIntervalInfo".Translate(1f));
			GUI.color = Color.white;
		}
		int autosavesCount = Prefs.AutosavesCount;
		Prefs.AutosavesCount = Mathf.RoundToInt(listing.SliderLabeled("AutosavesCount".Translate(autosavesCount), autosavesCount, 1f, 25f, 0.6f));
		bool checkOn = Prefs.RunInBackground;
		listing.CheckboxLabeled("RunInBackground".Translate(), ref checkOn, null, 30f, 0.6f);
		Prefs.RunInBackground = checkOn;
		if (!DevModePermanentlyDisabledUtility.Disabled || Prefs.DevMode)
		{
			bool checkOn2 = Prefs.DevMode;
			listing.CheckboxLabeled("DevelopmentMode".Translate(), ref checkOn2, null, 30f, 0.6f);
			Prefs.DevMode = checkOn2;
		}
		if ((int)Application.platform == 2 || (int)Application.platform == 7)
		{
			listing.verticalSpacing = 0f;
			if (listing.ButtonTextLabeledPct("SaveGameDataFolder".Translate(), "OpenFolder".Translate(), 0.6f, (TextAnchor)3))
			{
				Application.OpenURL(GenFilePaths.SavedGamesFolderPath);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
			listing.SubLabel(GenFilePaths.SavedGamesFolderPath, 0.6f);
			listing.Gap();
			if (listing.ButtonTextLabeledPct("LogFileFolder".Translate(), "OpenFolder".Translate(), 0.6f, (TextAnchor)3))
			{
				Application.OpenURL(Application.persistentDataPath);
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
			listing.SubLabel(Application.persistentDataPath, 0.6f);
			listing.Gap();
			listing.verticalSpacing = 5f;
		}
		else
		{
			if (listing.ButtonTextLabeledPct("SaveGameDataLocation".Translate(), "ShowFolder".Translate(), 0.6f, (TextAnchor)3))
			{
				Find.WindowStack.Add(new Dialog_MessageBox(Path.GetFullPath(GenFilePaths.SaveDataFolderPath)));
			}
			if (listing.ButtonTextLabeledPct("LogFileLocation".Translate(), "ShowFolder".Translate(), 0.6f, (TextAnchor)3))
			{
				Find.WindowStack.Add(new Dialog_MessageBox(Path.GetFullPath(Application.consoleLogPath)));
			}
		}
		if (listing.ButtonTextLabeledPct("RestoreToDefaultSettingsLabel".Translate(), "RestoreToDefaultSettings".Translate(), 0.6f, (TextAnchor)3))
		{
			Find.WindowStack.Add(new Dialog_MessageBox("ResetAndRestartConfirmationDialog".Translate(), buttonAAction: RestoreToDefaultSettings, buttonAText: "Yes".Translate(), buttonBText: "No".Translate()));
		}
	}

	private void DoVideoOptions(Listing_Standard listing)
	{
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (listing.ButtonTextLabeledPct("Resolution".Translate(), ResToString(Screen.width, Screen.height), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (Resolution item in from x in UnityGUIBugsFixer.ScreenResolutionsWithoutDuplicates
				where ((Resolution)(ref x)).width >= 1024 && ((Resolution)(ref x)).height >= 768
				orderby ((Resolution)(ref x)).width, ((Resolution)(ref x)).height
				select x)
			{
				Resolution res = item;
				list.Add(new FloatMenuOption(ResToString(((Resolution)(ref res)).width, ((Resolution)(ref res)).height), delegate
				{
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					if (!ResolutionUtility.UIScaleSafeWithResolution(Prefs.UIScale, ((Resolution)(ref res)).width, ((Resolution)(ref res)).height))
					{
						Messages.Message("MessageScreenResTooSmallForUIScale".Translate(), MessageTypeDefOf.RejectInput, historical: false);
					}
					else
					{
						ResolutionUtility.SafeSetResolution(res);
					}
				}));
			}
			if (!list.Any())
			{
				list.Add(new FloatMenuOption("NoneBrackets".Translate(), null));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		if (ResolutionUtility.BorderlessFullscreen)
		{
			listing.Label("BorderlessFullscreen".Translate());
		}
		else
		{
			bool checkOn = Screen.fullScreen;
			bool flag = checkOn;
			listing.CheckboxLabeled("Fullscreen".Translate(), ref checkOn, null, 30f, 0.6f);
			if (checkOn != flag)
			{
				ResolutionUtility.SafeSetFullscreen(checkOn);
			}
			if (!SteamDeck.IsSteamDeck && listing.ButtonText("BorderlessFullscreen".Translate()))
			{
				Find.WindowStack.Add(new Dialog_MessageBox("BorderlessFullscreenInstructions".Translate()));
			}
		}
		bool textureCompression = Prefs.TextureCompression;
		listing.CheckboxLabeled("TextureCompression".Translate(), ref textureCompression, "TextureCompression_Tooltip".Translate(), 30f, 0.6f);
		if (textureCompression != Prefs.TextureCompression)
		{
			Find.WindowStack.Add(new Dialog_MessageBox("ChangedTextureCompressionRestart".Translate(), "Yes".Translate(), delegate
			{
				Prefs.TextureCompression = textureCompression;
				Prefs.Save();
				GenCommandLine.Restart();
			}, "No".Translate()));
		}
		Prefs.TextureCompression = textureCompression;
		bool checkOn2 = Prefs.PlantWindSway;
		listing.CheckboxLabeled("PlantWindSway".Translate(), ref checkOn2, null, 30f, 0.6f);
		Prefs.PlantWindSway = checkOn2;
		float screenShakeIntensity = Prefs.ScreenShakeIntensity;
		Prefs.ScreenShakeIntensity = (float)Math.Round(listing.SliderLabeled(string.Format("{0}: {1}", "ScreenShakeIntensity".Translate(), screenShakeIntensity.ToStringPercent("N0")), screenShakeIntensity, 0f, 2f, 0.6f), 1);
		bool checkOn3 = Prefs.SmoothCameraJumps;
		listing.CheckboxLabeled("SmoothCameraJumps".Translate(), ref checkOn3, "SmoothCameraJumpsDesc".Translate(), 30f, 0.6f);
		Prefs.SmoothCameraJumps = checkOn3;
		bool checkOn4 = Prefs.GravshipCutscenes;
		listing.CheckboxLabeled("GravshipCutscenes".Translate(), ref checkOn4, "GravshipCutscenesDesc".Translate(), 30f, 0.6f);
		Prefs.GravshipCutscenes = checkOn4;
	}

	private void DoAudioOptions(Listing_Standard listing)
	{
		Prefs.VolumeMaster = listing.SliderLabeled(string.Format("{0}: {1}", "MasterVolume".Translate(), Prefs.VolumeMaster.ToStringPercent()), Prefs.VolumeMaster, 0f, 1f, 0.6f, "MasterVolumeTooltip".Translate());
		Prefs.VolumeGame = listing.SliderLabeled(string.Format("{0}: {1}", "GameVolume".Translate(), Prefs.VolumeGame.ToStringPercent()), Prefs.VolumeGame, 0f, 1f, 0.6f, "GameVolumeTooltip".Translate());
		Prefs.VolumeMusic = listing.SliderLabeled(string.Format("{0}: {1}", "MusicVolume".Translate(), Prefs.VolumeMusic.ToStringPercent()), Prefs.VolumeMusic, 0f, 1f, 0.6f, "MusicVolumeTooltip".Translate());
		Prefs.VolumeAmbient = listing.SliderLabeled(string.Format("{0}: {1}", "AmbientVolume".Translate(), Prefs.VolumeAmbient.ToStringPercent()), Prefs.VolumeAmbient, 0f, 1f, 0.6f, "AmbientVolumeTooltip".Translate());
		Prefs.VolumeUI = listing.SliderLabeled(string.Format("{0}: {1}", "UIVolume".Translate(), Prefs.VolumeUI.ToStringPercent()), Prefs.VolumeUI, 0f, 1f, 0.6f, "UIVolumeTooltip".Translate());
	}

	private void DoGameplayOptions(Listing_Standard listing)
	{
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		if (Current.ProgramState == ProgramState.Playing && listing.ButtonTextLabeledPct("ChangeStoryteller".Translate(), "Modify".Translate(), 0.6f, (TextAnchor)0, "OptionsButton-ChooseStoryteller") && TutorSystem.AllowAction("ChooseStoryteller"))
		{
			Find.WindowStack.Add(new Page_SelectStorytellerInGame());
		}
		int maxNumberOfPlayerSettlements = Prefs.MaxNumberOfPlayerSettlements;
		int num2 = (Prefs.MaxNumberOfPlayerSettlements = Mathf.RoundToInt(listing.SliderLabeled("MaxNumberOfPlayerSettlements".Translate(maxNumberOfPlayerSettlements), maxNumberOfPlayerSettlements, 1f, 5f, 0.6f)));
		if (maxNumberOfPlayerSettlements != num2 && num2 > 1)
		{
			TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.MaxNumberOfPlayerSettlements);
		}
		bool checkOn = Prefs.PauseOnLoad;
		listing.CheckboxLabeled("PauseOnLoad".Translate(), ref checkOn, null, 30f, 0.6f);
		Prefs.PauseOnLoad = checkOn;
		AutomaticPauseMode automaticPauseMode = Prefs.AutomaticPauseMode;
		if (listing.ButtonTextLabeledPct("AutomaticPauseModeSetting".Translate(), Prefs.AutomaticPauseMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			foreach (AutomaticPauseMode value in Enum.GetValues(typeof(AutomaticPauseMode)))
			{
				AutomaticPauseMode localPmode = value;
				list.Add(new FloatMenuOption(localPmode.ToStringHuman(), delegate
				{
					Prefs.AutomaticPauseMode = localPmode;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		Prefs.AutomaticPauseMode = automaticPauseMode;
		bool checkOn2 = Prefs.AdaptiveTrainingEnabled;
		listing.CheckboxLabeled("LearningHelper".Translate(), ref checkOn2, "LearningHelperTooltip".Translate(), 30f, 0.6f);
		Prefs.AdaptiveTrainingEnabled = checkOn2;
		if (listing.ButtonTextLabeledPct("ResetAdaptiveTutor".Translate(), "Reset".Translate(), 0.6f, (TextAnchor)3))
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmResetLearningHelper".Translate(), delegate
			{
				Messages.Message("AdaptiveTutorIsReset".Translate(), MessageTypeDefOf.TaskCompletion, historical: false);
				PlayerKnowledgeDatabase.ResetPersistent();
			}, destructive: true));
		}
		listing.verticalSpacing = 0f;
		if (Prefs.PreferredNames.Count < 6)
		{
			if (listing.ButtonTextLabeledPct("NamesYouWantToSee".Translate(), "AddName".Translate() + "...", 0.6f, (TextAnchor)3))
			{
				Find.WindowStack.Add(new Dialog_AddPreferredName());
			}
		}
		else
		{
			listing.Label("NamesYouWantToSee".Translate());
		}
		listing.SubLabel("NamesYouWantToSeeSubText".Translate(), 0.6f);
		listing.verticalSpacing = 5f;
		Prefs.PreferredNames.RemoveAll((string n) => n.NullOrEmpty());
		float num3 = 20f;
		listing.Indent(num3);
		for (int i = 0; i < Prefs.PreferredNames.Count; i++)
		{
			string name = Prefs.PreferredNames[i];
			PawnBio pawnBio = SolidBioDatabase.allBios.Where((PawnBio b) => b.name.ToString() == name).FirstOrDefault();
			if (pawnBio == null)
			{
				name += " [N]";
			}
			else
			{
				switch (pawnBio.BioType)
				{
				case PawnBioType.BackstoryInGame:
					name += " [B]";
					break;
				case PawnBioType.PirateKing:
					name += " [PK]";
					break;
				}
			}
			Rect rect = listing.GetRect(24f);
			((Rect)(ref rect)).width = ((Rect)(ref rect)).width - num3;
			if (i % 2 == 0)
			{
				Widgets.DrawLightHighlight(rect);
			}
			Widgets.Label(new Rect(((Rect)(ref rect)).x + 4f, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 4f, ((Rect)(ref rect)).height), name);
			if (Widgets.ButtonImage(new Rect(((Rect)(ref rect)).xMax - 24f, ((Rect)(ref rect)).y, 24f, 24f), TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
			{
				Prefs.PreferredNames.RemoveAt(i);
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}
		listing.Outdent(num3);
		listing.Gap();
	}

	private void DoUIOptions(Listing_Standard listing)
	{
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		if (listing.ButtonTextLabeledPct("UIScale".Translate(), Prefs.UIScale + "x", 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			for (int i = 0; i < UIScales.Length; i++)
			{
				float scale = UIScales[i];
				list.Add(new FloatMenuOption(UIScales[i] + "x", delegate
				{
					if (scale != 1f && !ResolutionUtility.UIScaleSafeWithResolution(scale, Screen.width, Screen.height))
					{
						Messages.Message("MessageScreenResTooSmallForUIScale".Translate(), MessageTypeDefOf.RejectInput, historical: false);
					}
					else
					{
						ResolutionUtility.SafeSetUIScale(scale);
					}
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list));
		}
		if (listing.ButtonTextLabeledPct("TemperatureMode".Translate(), Prefs.TemperatureMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list2 = new List<FloatMenuOption>();
			foreach (TemperatureDisplayMode value in Enum.GetValues(typeof(TemperatureDisplayMode)))
			{
				TemperatureDisplayMode localTmode = value;
				list2.Add(new FloatMenuOption(localTmode.ToStringHuman(), delegate
				{
					Prefs.TemperatureMode = localTmode;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list2));
		}
		if (listing.ButtonTextLabeledPct("ShowWeaponsUnderPortrait".Translate(), Prefs.ShowWeaponsUnderPortraitMode.ToStringHuman(), 0.6f, (TextAnchor)3, null, "ShowWeaponsUnderPortraitTooltip".Translate()))
		{
			List<FloatMenuOption> list3 = new List<FloatMenuOption>();
			foreach (ShowWeaponsUnderPortraitMode mode in Enum.GetValues(typeof(ShowWeaponsUnderPortraitMode)))
			{
				list3.Add(new FloatMenuOption(mode.ToStringHuman(), delegate
				{
					Prefs.ShowWeaponsUnderPortraitMode = mode;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list3));
		}
		if (listing.ButtonTextLabeledPct("ShowAnimalNames".Translate(), Prefs.AnimalNameMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list4 = new List<FloatMenuOption>();
			foreach (AnimalNameDisplayMode value2 in Enum.GetValues(typeof(AnimalNameDisplayMode)))
			{
				AnimalNameDisplayMode localMode4 = value2;
				list4.Add(new FloatMenuOption(localMode4.ToStringHuman(), delegate
				{
					Prefs.AnimalNameMode = localMode4;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list4));
		}
		if (ModsConfig.BiotechActive && listing.ButtonTextLabeledPct("ShowMechNames".Translate(), Prefs.MechNameMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list5 = new List<FloatMenuOption>();
			foreach (MechNameDisplayMode value3 in Enum.GetValues(typeof(MechNameDisplayMode)))
			{
				MechNameDisplayMode localMode3 = value3;
				list5.Add(new FloatMenuOption(localMode3.ToStringHuman(), delegate
				{
					Prefs.MechNameMode = localMode3;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list5));
		}
		if (listing.ButtonTextLabeledPct("DotHighlightDisplayMode".Translate(), Prefs.DotHighlightDisplayMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list6 = new List<FloatMenuOption>();
			foreach (DotHighlightDisplayMode value4 in Enum.GetValues(typeof(DotHighlightDisplayMode)))
			{
				DotHighlightDisplayMode localMode2 = value4;
				list6.Add(new FloatMenuOption(localMode2.ToStringHuman(), delegate
				{
					Prefs.DotHighlightDisplayMode = localMode2;
				}));
			}
			Find.WindowStack.Add(new FloatMenu(list6));
		}
		bool flag = Prefs.DotHighlightDisplayMode != DotHighlightDisplayMode.None;
		if (!flag)
		{
			GUI.color = Color.gray;
		}
		if (listing.ButtonTextLabeledPct("HighlightStyleMode".Translate(), Prefs.HighlightStyleMode.ToStringHuman(), 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list7 = new List<FloatMenuOption>();
			foreach (HighlightStyleMode value5 in Enum.GetValues(typeof(HighlightStyleMode)))
			{
				HighlightStyleMode localMode = value5;
				list7.Add(new FloatMenuOption(localMode.ToStringHuman(), delegate
				{
					Prefs.HighlightStyleMode = localMode;
				}));
			}
			if (flag)
			{
				Find.WindowStack.Add(new FloatMenu(list7));
			}
		}
		if (!flag)
		{
			GUI.color = Color.white;
		}
		TaggedString taggedString = (Prefs.RandomBackgroundImage ? "Random".Translate() : Prefs.BackgroundImageExpansion.LabelCap);
		if (ModLister.AllExpansions.Where((ExpansionDef e) => !e.isCore && e.Status != ExpansionStatus.NotInstalled).Any() && listing.ButtonTextLabeledPct("SetBackgroundImage".Translate(), taggedString, 0.6f, (TextAnchor)3))
		{
			List<FloatMenuOption> list8 = new List<FloatMenuOption>();
			foreach (ExpansionDef allExpansion in ModLister.AllExpansions)
			{
				if (allExpansion.Status != ExpansionStatus.NotInstalled)
				{
					ExpansionDef localExpansion = allExpansion;
					list8.Add(new FloatMenuOption(allExpansion.label, delegate
					{
						Prefs.BackgroundImageExpansion = localExpansion;
						Prefs.RandomBackgroundImage = false;
					}, allExpansion.Icon, Color.white));
				}
			}
			list8.Add(new FloatMenuOption("Random".Translate(), delegate
			{
				Prefs.RandomBackgroundImage = true;
				((UI_BackgroundMain)UIMenuBackgroundManager.background).overrideBGImage = ModLister.AllExpansions.Where((ExpansionDef exp) => exp.Status == ExpansionStatus.Active).RandomElement().BackgroundImage;
			}, RandomBackgroundTex, Color.white));
			Find.WindowStack.Add(new FloatMenu(list8));
		}
		bool checkOn = Prefs.ShowRealtimeClock;
		listing.CheckboxLabeled("ShowRealtimeClock".Translate(), ref checkOn, null, 30f, 0.6f);
		Prefs.ShowRealtimeClock = checkOn;
		bool checkOn2 = Prefs.TwelveHourClockMode;
		listing.CheckboxLabeled("TwelveHourClockMode".Translate(), ref checkOn2, null, 30f, 0.6f);
		Prefs.TwelveHourClockMode = checkOn2;
		bool checkOn3 = Prefs.HatsOnlyOnMap;
		listing.CheckboxLabeled("HatsShownOnlyOnMap".Translate(), ref checkOn3, null, 30f, 0.6f);
		if (checkOn3 != Prefs.HatsOnlyOnMap)
		{
			PortraitsCache.Clear();
		}
		Prefs.HatsOnlyOnMap = checkOn3;
		if (!SteamDeck.IsSteamDeck)
		{
			bool checkOn4 = Prefs.DisableTinyText;
			listing.CheckboxLabeled("DisableTinyText".Translate(), ref checkOn4, null, 30f, 0.6f);
			if (Prefs.DisableTinyText != checkOn4)
			{
				Prefs.DisableTinyText = checkOn4;
				Widgets.ClearLabelCache();
				GenUI.ClearLabelWidthCache();
				if (Current.ProgramState == ProgramState.Playing)
				{
					Find.ColonistBar.drawer.ClearLabelCache();
				}
			}
		}
		bool checkOn5 = !Prefs.CustomCursorEnabled;
		listing.CheckboxLabeled("CustomCursor".Translate(), ref checkOn5, null, 30f, 0.6f);
		Prefs.CustomCursorEnabled = !checkOn5;
		bool checkOn6 = Prefs.VisibleMood;
		listing.CheckboxLabeled("VisibleMood".Translate(), ref checkOn6, "VisibleMoodDesc".Translate(), 30f, 0.6f);
		Prefs.VisibleMood = checkOn6;
	}

	private void DoControlsOptions(Listing_Standard listing)
	{
		if (!SteamDeck.IsSteamDeckInNonKeyboardMode && listing.ButtonTextLabeledPct("KeyboardConfig".Translate(), "ModifyConfig".Translate(), 0.6f, (TextAnchor)3))
		{
			Find.WindowStack.Add(new Dialog_KeyBindings());
		}
		if (SteamDeck.IsSteamDeck && listing.ButtonTextLabeledPct("ControllerConfig".Translate(), "ModifyConfig".Translate(), 0.6f, (TextAnchor)3))
		{
			SteamDeck.ShowConfigPage();
		}
		if (SteamDeck.IsSteamDeck)
		{
			bool checkOn = Prefs.SteamDeckKeyboardMode;
			listing.CheckboxLabeled("SteamDeckKeyboardMode".Translate(), ref checkOn, null, 30f, 0.6f);
			Prefs.SteamDeckKeyboardMode = checkOn;
		}
		float mapDragSensitivity = Prefs.MapDragSensitivity;
		Prefs.MapDragSensitivity = (float)Math.Round(listing.SliderLabeled("MapDragSensitivity".Translate() + ": " + mapDragSensitivity.ToStringPercent("F0"), mapDragSensitivity, 0.8f, 2.5f, 0.6f), 2);
		bool checkOn2 = Prefs.EdgeScreenScroll;
		listing.CheckboxLabeled("EdgeScreenScroll".Translate(), ref checkOn2, null, 30f, 0.6f);
		Prefs.EdgeScreenScroll = checkOn2;
		bool checkOn3 = Prefs.ZoomToMouse;
		listing.CheckboxLabeled("ZoomToMouse".Translate(), ref checkOn3, null, 30f, 0.6f);
		Prefs.ZoomToMouse = checkOn3;
		bool checkOn4 = Prefs.ZoomSwitchWorldLayer;
		listing.CheckboxLabeled("ZoomSwitchLayer".Translate(), ref checkOn4, "ZoomSwitchLayer_Tooltip".Translate(), 30f, 0.6f);
		Prefs.ZoomSwitchWorldLayer = checkOn4;
		bool checkOn5 = Prefs.RememberDrawStlyes;
		listing.CheckboxLabeled("RememberDrawStyle".Translate(), ref checkOn5, "RememberDrawStyle_Tooltip".Translate(), 30f, 0.6f);
		Prefs.RememberDrawStlyes = checkOn5;
	}

	private void DoDevOptions(Listing_Standard listing)
	{
		bool checkOn = Prefs.TestMapSizes;
		listing.CheckboxLabeled("EnableTestMapSizes".Translate(), ref checkOn, null, 30f, 0.6f);
		Prefs.TestMapSizes = checkOn;
		bool checkOn2 = Prefs.LogVerbose;
		listing.CheckboxLabeled("LogVerbose".Translate(), ref checkOn2, null, 30f, 0.6f);
		Prefs.LogVerbose = checkOn2;
		bool checkOn3 = Prefs.ResetModsConfigOnCrash;
		listing.CheckboxLabeled("ResetModsConfigOnCrash".Translate(), ref checkOn3, null, 30f, 0.6f);
		Prefs.ResetModsConfigOnCrash = checkOn3;
		bool checkOn4 = Prefs.DisableQuickStartCryptoSickness;
		listing.CheckboxLabeled("DisableQuickStartCryptoSickness".Translate(), ref checkOn4, null, 30f, 0.6f);
		Prefs.DisableQuickStartCryptoSickness = checkOn4;
		bool checkOn5 = Prefs.StartDevPaletteOn;
		listing.CheckboxLabeled("StartDevPaletteOn".Translate(), ref checkOn5, null, 30f, 0.6f);
		Prefs.StartDevPaletteOn = checkOn5;
		bool checkOn6 = Prefs.OpenLogOnWarnings;
		listing.CheckboxLabeled("OpenLogOnWarnings".Translate(), ref checkOn6, null, 30f, 0.6f);
		Prefs.OpenLogOnWarnings = checkOn6;
		bool checkOn7 = Prefs.CloseLogWindowOnEscape;
		listing.CheckboxLabeled("CloseLogWindowOnEscape".Translate(), ref checkOn7, null, 30f, 0.6f);
		Prefs.CloseLogWindowOnEscape = checkOn7;
		if (!DevModePermanentlyDisabledUtility.Disabled && listing.ButtonTextLabeledPct("PermanentlyDisableDevMode".Translate(), "Disable".Translate(), 0.6f, (TextAnchor)3))
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmPermanentlyDisableDevMode".Translate(), DevModePermanentlyDisabledUtility.Disable, destructive: true));
		}
	}

	private void DoModOptions(Listing_Standard listing)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = listing.GetRect(30f);
		quickSearchWidget.OnGUI(rect);
		modFilter = quickSearchWidget.filter.Text;
		listing.Gap();
		Rect rect2 = default(Rect);
		int num = 0;
		foreach (Mod cachedModsWithSetting in cachedModsWithSettings)
		{
			if (cachedModsWithSetting.SettingsCategory().ToLower().Contains(modFilter.ToLower()) || cachedModsWithSetting.Content.Name.ToLower().Contains(modFilter.ToLower()))
			{
				if (num % 2 == 0)
				{
					rect2 = listing.GetRect(40f);
				}
				Rect val = ((num % 2 == 0) ? rect2.LeftHalf() : rect2.RightHalf()).ContractedBy(2f);
				Text.Anchor = (TextAnchor)3;
				Widgets.DrawOptionBackground(val, selected: false);
				if (Widgets.ButtonInvisible(val))
				{
					Find.WindowStack.Add(new Dialog_ModSettings(cachedModsWithSetting));
				}
				if (Mouse.IsOver(val) && Text.CalcSize(cachedModsWithSetting.SettingsCategory()).x > ((Rect)(ref val)).width - ((Rect)(ref val)).height)
				{
					TooltipHandler.TipRegion(val, cachedModsWithSetting.SettingsCategory());
				}
				if (!cachedModsWithSetting.Content.ModMetaData.Icon.NullOrBad())
				{
					GUI.DrawTexture(GenUI.ContractedBy(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).height, ((Rect)(ref val)).height), 2f), (Texture)(object)cachedModsWithSetting.Content.ModMetaData.Icon);
				}
				((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + ((Rect)(ref val)).height;
				Widgets.Label(val, cachedModsWithSetting.SettingsCategory().Truncate(((Rect)(ref val)).width, modOptionTruncationCache));
				Text.Anchor = (TextAnchor)0;
				num++;
			}
		}
	}

	public override void PreClose()
	{
		base.PreClose();
		Prefs.Save();
		if (selectedMod != null)
		{
			selectedMod.WriteSettings();
		}
	}

	public void RestoreToDefaultSettings()
	{
		FileInfo[] files = new DirectoryInfo(GenFilePaths.ConfigFolderPath).GetFiles("*.xml");
		foreach (FileInfo fileInfo in files)
		{
			try
			{
				fileInfo.Delete();
			}
			catch (SystemException)
			{
			}
		}
		Find.WindowStack.Add(new Dialog_MessageBox("ResetAndRestart".Translate(), null, GenCommandLine.Restart));
	}

	public static string ResToString(int width, int height)
	{
		string text = width + "x" + height;
		if (width == 1280 && height == 720)
		{
			text += " (720p)";
		}
		if (width == 1920 && height == 1080)
		{
			text += " (1080p)";
		}
		return text;
	}
}
