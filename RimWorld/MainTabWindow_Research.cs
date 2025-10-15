using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MainTabWindow_Research : MainTabWindow
{
	private class ResearchTabRecord : TabRecord
	{
		public readonly ResearchTabDef def;

		public ResearchProjectDef firstMatch;

		private string cachedTip;

		public bool AnyMatches => firstMatch != null;

		public override string TutorTag => def.tutorTag;

		public ResearchTabRecord(ResearchTabDef def, string label, Action clickedAction, Func<bool> selected)
			: base(label, clickedAction, selected)
		{
			this.def = def;
		}

		public void Reset()
		{
			firstMatch = null;
			labelColor = null;
		}

		public override string GetTip()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (cachedTip == null)
			{
				cachedTip = def.generalTitle.CapitalizeFirst().Colorize(ColoredText.TipSectionTitleColor) ?? "";
				if (!def.generalDescription.NullOrEmpty())
				{
					cachedTip = cachedTip + "\n" + def.generalDescription;
				}
			}
			return cachedTip;
		}
	}

	protected ResearchProjectDef selectedProject;

	private ScrollPositioner scrollPositioner = new ScrollPositioner();

	private Vector2 leftScrollPosition = Vector2.zero;

	private float leftScrollViewHeight;

	private Vector2 rightScrollPosition;

	private float rightViewWidth;

	private ResearchTabDef curTabInt;

	private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private bool editMode;

	private List<ResearchProjectDef> draggingTabs = new List<ResearchProjectDef>();

	private List<ResearchTabRecord> tabs = new List<ResearchTabRecord>();

	private List<ResearchProjectDef> cachedVisibleResearchProjects;

	private Dictionary<ResearchProjectDef, List<Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>>>> cachedUnlockedDefsGroupedByPrerequisites;

	private readonly HashSet<ResearchProjectDef> matchingProjects = new HashSet<ResearchProjectDef>();

	private const float leftAreaWidthPercent = 0.22f;

	private const float LeftAreaWidthMin = 270f;

	private const float ProjectTitleHeight = 50f;

	private const float ProjectTitleLeftMargin = 0f;

	private const int ResearchItemW = 140;

	private const int ResearchItemH = 50;

	private const int ResearchItemPaddingW = 50;

	private const int ResearchItemPaddingH = 50;

	private const int CategoryRectW = 14;

	private const float IndentSpacing = 6f;

	private const float RowHeight = 24f;

	private const float LeftStartButHeight = 55f;

	private const float SearchBoxHeight = 24f;

	private const float SearchBoxWidth = 200f;

	private const float MinTabWidth = 100f;

	private const float MaxTabWidth = 200f;

	private const int SearchHighlightMargin = 4;

	private const KeyCode SelectMultipleKey = 304;

	private const KeyCode DeselectKey = 306;

	private static readonly Texture2D ResearchBarFillTex = SolidColorMaterials.NewSolidColorTexture(TexUI.ResearchMainTabColor);

	private static readonly Texture2D ResearchBarBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f));

	private static readonly CachedTexture BasicBackgroundTex = new CachedTexture("UI/AnomalyResearchCategoryMarkers/AnomalyResearchBackground_Basic");

	private static readonly CachedTexture AdvancedBackgroundTex = new CachedTexture("UI/AnomalyResearchCategoryMarkers/AnomalyResearchBackground_Advanced");

	private static readonly Color FulfilledPrerequisiteColor = Color.green;

	private static readonly Color MissingPrerequisiteColor = ColorLibrary.RedReadable;

	private static readonly Color ProjectWithMissingPrerequisiteLabelColor = Color.gray;

	private static readonly Color HiddenProjectLabelColor = Color.gray;

	private static readonly Color ActiveProjectLabelColor = new ColorInt(219, 201, 126, 255).ToColor;

	private static readonly Color NoMatchTintColor = Widgets.MenuSectionBGFillColor;

	private const float NoMatchTintFactor = 0.4f;

	private static readonly CachedTexture TechprintRequirementTex = new CachedTexture("UI/Icons/Research/Techprint");

	private static readonly CachedTexture StudyRequirementTex = new CachedTexture("UI/Icons/Study");

	private static List<string> lockedReasons = new List<string>();

	private List<(BuildableDef, List<string>)> cachedDefsWithMissingMemes = new List<(BuildableDef, List<string>)>();

	private static Dictionary<string, string> labelsWithNewlineCached = new Dictionary<string, string>();

	private static Dictionary<Pair<int, int>, string> techprintsInfoCached = new Dictionary<Pair<int, int>, string>();

	private List<string> tmpSuffixesForUnlocked = new List<string>();

	private static List<Building> tmpAllBuildings = new List<Building>();

	public ResearchTabDef CurTab
	{
		get
		{
			return curTabInt;
		}
		set
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (value != curTabInt)
			{
				curTabInt = value;
				Vector2 val = ViewSize(CurTab);
				rightViewWidth = val.x;
				rightScrollPosition = Vector2.zero;
			}
		}
	}

	private ResearchTabRecord CurTabRecord
	{
		get
		{
			foreach (ResearchTabRecord tab in tabs)
			{
				if (tab.def == CurTab)
				{
					return tab;
				}
			}
			return null;
		}
	}

	private bool ColonistsHaveResearchBench
	{
		get
		{
			bool result = false;
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].listerBuildings.ColonistsHaveResearchBench())
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	public List<ResearchProjectDef> VisibleResearchProjects
	{
		get
		{
			if (cachedVisibleResearchProjects == null)
			{
				cachedVisibleResearchProjects = new List<ResearchProjectDef>(DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((ResearchProjectDef d) => Find.Storyteller.difficulty.AllowedBy(d.hideWhen) || Find.ResearchManager.IsCurrentProject(d)));
			}
			return cachedVisibleResearchProjects;
		}
	}

	public override Vector2 InitialSize
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			Vector2 initialSize = base.InitialSize;
			float num = UI.screenHeight - 35;
			float num2 = 0f;
			foreach (ResearchTabDef allDef in DefDatabase<ResearchTabDef>.AllDefs)
			{
				num2 = Mathf.Max(num2, ViewSize(allDef).y);
			}
			float num3 = Mathf.Max(270f, (float)UI.screenWidth * 0.22f);
			float overflowTabHeight = TabDrawer.GetOverflowTabHeight(new Rect(num3, 0f, (float)UI.screenWidth - num3 - 200f - 4f, 0f), tabs, 100f, 200f);
			float num4 = Margin + 10f + overflowTabHeight + 10f + num2 + 10f + 10f + Margin;
			float num5 = Mathf.Max(initialSize.y, num4);
			initialSize.y = Mathf.Min(num5, num);
			return initialSize;
		}
	}

	private Vector2 ViewSize(ResearchTabDef tab)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		List<ResearchProjectDef> visibleResearchProjects = VisibleResearchProjects;
		float num = 0f;
		float num2 = 0f;
		Text.Font = GameFont.Small;
		float num3 = 0f;
		if (ModsConfig.AnomalyActive && tab == ResearchTabDefOf.Anomaly)
		{
			num3 = 14f;
		}
		Rect rect = default(Rect);
		for (int i = 0; i < visibleResearchProjects.Count; i++)
		{
			ResearchProjectDef researchProjectDef = visibleResearchProjects[i];
			if (researchProjectDef.tab == tab)
			{
				((Rect)(ref rect))._002Ector(0f, 0f, 140f, 0f);
				Widgets.LabelCacheHeight(ref rect, GetLabelWithNewlineCached(GetLabel(researchProjectDef)), renderLabel: false);
				num = Mathf.Max(num, PosX(researchProjectDef) + 140f + num3);
				num2 = Mathf.Max(num2, PosY(researchProjectDef) + ((Rect)(ref rect)).height);
			}
		}
		return new Vector2(num + 20f + 4f, num2 + 20f + 4f);
	}

	public override void PreOpen()
	{
		base.PreOpen();
		UpdateSelectedProject(Find.ResearchManager);
		scrollPositioner.Arm();
		cachedVisibleResearchProjects = null;
		cachedUnlockedDefsGroupedByPrerequisites = null;
		quickSearchWidget.Reset();
		if (CurTab == null)
		{
			CurTab = ((selectedProject != null) ? selectedProject.tab : ResearchTabDefOf.Main);
		}
		UpdateSearchResults();
	}

	public void Select(ResearchProjectDef project)
	{
		CurTab = project.tab;
		selectedProject = project;
	}

	private void UpdateSelectedProject(ResearchManager researchManager)
	{
		if (ModsConfig.AnomalyActive && curTabInt == ResearchTabDefOf.Anomaly)
		{
			selectedProject = null;
			{
				foreach (ResearchManager.KnowledgeCategoryProject currentAnomalyKnowledgeProject in researchManager.CurrentAnomalyKnowledgeProjects)
				{
					if (currentAnomalyKnowledgeProject.project != null)
					{
						selectedProject = currentAnomalyKnowledgeProject.project;
						break;
					}
				}
				return;
			}
		}
		selectedProject = researchManager.GetProject();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref windowRect)).width = UI.screenWidth;
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Small;
		float num = Mathf.Max(270f, ((Rect)(ref inRect)).width * 0.22f);
		Rect leftOutRect = default(Rect);
		((Rect)(ref leftOutRect))._002Ector(0f, 0f, num, ((Rect)(ref inRect)).height);
		Rect searchRect = default(Rect);
		((Rect)(ref searchRect))._002Ector(((Rect)(ref inRect)).xMax - 200f, 0f, 200f, 24f);
		Rect rightOutRect = default(Rect);
		((Rect)(ref rightOutRect))._002Ector(((Rect)(ref leftOutRect)).xMax + 10f, 0f, ((Rect)(ref inRect)).width - ((Rect)(ref leftOutRect)).width - 10f, ((Rect)(ref inRect)).height);
		DrawSearchRect(searchRect);
		DrawLeftRect(leftOutRect);
		DrawRightRect(rightOutRect, ((Rect)(ref searchRect)).x - 4f);
	}

	private void DrawSearchRect(Rect searchRect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		quickSearchWidget.OnGUI(searchRect, UpdateSearchResults);
	}

	private void DrawLeftRect(Rect leftOutRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(leftOutRect);
		if (selectedProject != null)
		{
			DrawProjectInfo(leftOutRect);
		}
		else
		{
			DrawCurrentTabInfo(leftOutRect);
		}
		Widgets.EndGroup();
	}

	private void DrawCurrentTabInfo(Rect rect)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, leftScrollViewHeight);
		Widgets.BeginScrollView(outRect, ref leftScrollPosition, viewRect);
		float num = 0f;
		Text.Font = GameFont.Medium;
		GenUI.SetLabelAlign((TextAnchor)3);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, num, ((Rect)(ref viewRect)).width - 0f, 50f);
		Widgets.LabelCacheHeight(ref rect2, curTabInt.generalTitle.CapitalizeFirst());
		GenUI.ResetLabelAlign();
		Text.Font = GameFont.Small;
		num += ((Rect)(ref rect2)).height;
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, num, ((Rect)(ref viewRect)).width, 0f);
		Widgets.LabelCacheHeight(ref rect3, curTabInt.generalDescription);
		num += ((Rect)(ref rect3)).height;
		leftScrollViewHeight = num;
		Widgets.EndScrollView();
	}

	private void DrawProjectInfo(Rect rect)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		int num = ((!ModsConfig.AnomalyActive || curTabInt != ResearchTabDefOf.Anomaly) ? 1 : 2);
		float num2 = ((num > 1) ? (75f * (float)num) : 100f);
		Rect val = rect;
		((Rect)(ref val)).yMin = ((Rect)(ref rect)).yMax - num2;
		((Rect)(ref val)).yMax = ((Rect)(ref rect)).yMax;
		Rect rect2 = val;
		Rect rect3 = val;
		((Rect)(ref rect3)).y = ((Rect)(ref val)).y - 30f;
		((Rect)(ref rect3)).height = 28f;
		val = val.ContractedBy(10f);
		((Rect)(ref val)).y = ((Rect)(ref val)).y + 5f;
		Text.Font = GameFont.Medium;
		string key = ((num > 1) ? "ActiveProjectPlural" : "ActiveProject");
		Widgets.Label(rect3, key.Translate());
		Text.Font = GameFont.Small;
		Rect val2 = default(Rect);
		((Rect)(ref val2)).y = ((Rect)(ref rect3)).y - 55f - 10f;
		((Rect)(ref val2)).height = 55f;
		((Rect)(ref val2)).x = ((Rect)(ref rect)).center.x - ((Rect)(ref rect)).width / 4f;
		((Rect)(ref val2)).width = ((Rect)(ref rect)).width / 2f + 20f;
		Rect startButRect = val2;
		Widgets.DrawMenuSection(rect2);
		if (ModsConfig.AnomalyActive && curTabInt == ResearchTabDefOf.Anomaly)
		{
			Rect val3 = val;
			((Rect)(ref val3)).height = ((Rect)(ref val)).height / 2f;
			Rect rect4 = val3;
			((Rect)(ref rect4)).yMin = ((Rect)(ref val)).yMax - ((Rect)(ref val3)).height;
			((Rect)(ref rect4)).yMax = ((Rect)(ref val)).yMax;
			ResearchProjectDef project = Find.ResearchManager.GetProject(KnowledgeCategoryDefOf.Basic);
			ResearchProjectDef project2 = Find.ResearchManager.GetProject(KnowledgeCategoryDefOf.Advanced);
			if (project == null && project2 == null)
			{
				using (new TextBlock((TextAnchor)4))
				{
					Widgets.Label(val, "NoProjectSelected".Translate());
				}
			}
			else
			{
				float prefixWidth = DefDatabase<KnowledgeCategoryDef>.AllDefs.Max((KnowledgeCategoryDef x) => Text.CalcSize(x.LabelCap + ":").x);
				DrawProjectProgress(val3, project, KnowledgeCategoryDefOf.Basic.LabelCap, prefixWidth);
				DrawProjectProgress(rect4, project2, KnowledgeCategoryDefOf.Advanced.LabelCap, prefixWidth);
			}
		}
		else
		{
			ResearchProjectDef project3 = Find.ResearchManager.GetProject();
			if (project3 == null)
			{
				using (new TextBlock((TextAnchor)4))
				{
					Widgets.Label(val, "NoProjectSelected".Translate());
				}
			}
			else
			{
				DrawProjectProgress(val, project3);
			}
		}
		DrawStartButton(startButRect);
		if (Prefs.DevMode && !Find.ResearchManager.IsCurrentProject(selectedProject) && !selectedProject.IsFinished)
		{
			Text.Font = GameFont.Tiny;
			if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - 120f, ((Rect)(ref rect3)).y, 120f, 25f), "Debug: Finish now"))
			{
				Find.ResearchManager.SetCurrentProject(selectedProject);
				Find.ResearchManager.FinishProject(selectedProject);
			}
			Text.Font = GameFont.Small;
		}
		if (Prefs.DevMode && !selectedProject.TechprintRequirementMet)
		{
			Text.Font = GameFont.Tiny;
			if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).xMax - 300f, ((Rect)(ref rect3)).y, 170f, 25f), "Debug: Apply techprint"))
			{
				Find.ResearchManager.ApplyTechprint(selectedProject, null);
				SoundDefOf.TechprintApplied.PlayOneShotOnCamera();
			}
			Text.Font = GameFont.Small;
		}
		float y = 0f;
		DrawProjectPrimaryInfo(rect, ref y);
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(0f, y, ((Rect)(ref rect)).width, 0f);
		((Rect)(ref rect5)).yMax = ((Rect)(ref startButRect)).yMin - 10f;
		DrawProjectScrollView(rect5);
	}

	private void DrawProjectPrimaryInfo(Rect rect, ref float y)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock(GameFont.Medium, (TextAnchor)3))
		{
			Widgets.Label(new Rect(0f, y, ((Rect)(ref rect)).width - 0f, 50f), ref y, selectedProject.LabelCap);
		}
		y += 10f;
		string text = selectedProject.Description;
		if (ModsConfig.AnomalyActive && selectedProject.knowledgeCategory != null)
		{
			text = text + "\n\n" + "AnomalyResearchDescriptionHelpText".Translate().Colorize(ColoredText.SubtleGrayColor);
		}
		Widgets.Label(0f, ref y, ((Rect)(ref rect)).width, text);
		y += 10f;
		Widgets.DrawLineHorizontal(((Rect)(ref rect)).x - 8f, y, ((Rect)(ref rect)).width, Color.gray);
		y += 10f;
		if (ModsConfig.AnomalyActive && selectedProject.knowledgeCategory != null)
		{
			Widgets.Label(0f, ref y, ((Rect)(ref rect)).width, "KnowledgeCategory".Translate() + ": " + selectedProject.knowledgeCategory.LabelCap);
		}
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, y, ((Rect)(ref rect)).width, 500f);
		DrawTechprintInfo(rect2, ref y);
	}

	private void DrawProjectScrollView(Rect rect)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref rect)).width - 16f, leftScrollViewHeight);
		float y = 3f;
		Widgets.BeginScrollView(rect, ref leftScrollPosition, viewRect);
		if ((int)selectedProject.techLevel > (int)Faction.OfPlayer.def.techLevel)
		{
			float num = selectedProject.CostFactor(Faction.OfPlayer.def.techLevel);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 0f);
			string text = "TechLevelTooLow".Translate(Faction.OfPlayer.def.techLevel.ToStringHuman(), selectedProject.techLevel.ToStringHuman(), (1f / num).ToStringPercent());
			if (num != 1f)
			{
				text += " " + "ResearchCostComparison".Translate(selectedProject.Cost.ToString("F0"), selectedProject.CostApparent.ToString("F0"));
			}
			Widgets.LabelCacheHeight(ref rect2, text);
			y += ((Rect)(ref rect2)).height;
		}
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 0f);
		DrawResearchPrerequisites(rect3, ref y, selectedProject);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 500f);
		y += DrawResearchBenchRequirements(selectedProject, rect4);
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 500f);
		y += DrawStudyRequirements(selectedProject, rect5);
		Rect rect6 = default(Rect);
		((Rect)(ref rect6))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 500f);
		y += DrawInspectionRequirements(selectedProject, rect6);
		Rect rect7 = default(Rect);
		((Rect)(ref rect7))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 500f);
		Rect visibleRect = default(Rect);
		((Rect)(ref visibleRect))._002Ector(0f, leftScrollPosition.y, ((Rect)(ref viewRect)).width, ((Rect)(ref rect)).height);
		y += DrawUnlockableHyperlinks(rect7, visibleRect, selectedProject);
		y += DrawCustomUnlockables(new Rect(0f, y, ((Rect)(ref viewRect)).width, 500f), selectedProject);
		Rect rect8 = default(Rect);
		((Rect)(ref rect8))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 500f);
		y += DrawContentSource(rect8, selectedProject);
		y += 3f;
		leftScrollViewHeight = y;
		Widgets.EndScrollView();
	}

	private void DrawStartButton(Rect startButRect)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		if (selectedProject.CanStartNow && !Find.ResearchManager.IsCurrentProject(selectedProject))
		{
			if (Widgets.ButtonText(startButRect, "Research".Translate()))
			{
				AttemptBeginResearch(selectedProject);
			}
			return;
		}
		if (Find.ResearchManager.IsCurrentProject(selectedProject))
		{
			if (Widgets.ButtonText(startButRect, "StopResearch".Translate()))
			{
				Find.ResearchManager.StopProject(selectedProject);
			}
			return;
		}
		lockedReasons.Clear();
		string text;
		if (selectedProject.IsFinished)
		{
			text = "Finished".Translate();
			Text.Anchor = (TextAnchor)4;
		}
		else if (Find.ResearchManager.IsCurrentProject(selectedProject))
		{
			text = "InProgress".Translate();
			Text.Anchor = (TextAnchor)4;
		}
		else
		{
			if (!selectedProject.PrerequisitesCompleted)
			{
				lockedReasons.Add("PrerequisitesNotCompleted".Translate());
			}
			if (!selectedProject.TechprintRequirementMet)
			{
				lockedReasons.Add("InsufficientTechprintsApplied".Translate(selectedProject.TechprintsApplied, selectedProject.TechprintCount));
			}
			if ((!ModsConfig.AnomalyActive || curTabInt != ResearchTabDefOf.Anomaly) && !selectedProject.PlayerHasAnyAppropriateResearchBench)
			{
				lockedReasons.Add("MissingRequiredResearchFacilities".Translate());
			}
			if (!selectedProject.PlayerMechanitorRequirementMet)
			{
				lockedReasons.Add("MissingRequiredMechanitor".Translate());
			}
			if (!selectedProject.InspectionRequirementsMet)
			{
				lockedReasons.Add("MissingGravEngineInspection".Translate());
			}
			if (!selectedProject.AnalyzedThingsRequirementsMet)
			{
				for (int i = 0; i < selectedProject.requiredAnalyzed.Count; i++)
				{
					lockedReasons.Add("NotStudied".Translate(selectedProject.requiredAnalyzed[i].LabelCap));
				}
			}
			if (lockedReasons.NullOrEmpty())
			{
				Log.ErrorOnce("Research " + selectedProject.defName + " locked but no reasons given", selectedProject.GetHashCode() ^ 0x5FE2BD1);
			}
			text = "Locked".Translate();
		}
		Widgets.DrawHighlight(startButRect);
		startButRect = startButRect.ContractedBy(4f);
		string text2 = text;
		if (!lockedReasons.NullOrEmpty())
		{
			text2 = text2 + ":\n" + lockedReasons.ToLineList("  ");
		}
		Vector2 val = Text.CalcSize(text2);
		if (val.x > ((Rect)(ref startButRect)).width || val.y > ((Rect)(ref startButRect)).height)
		{
			TooltipHandler.TipRegion(startButRect.ExpandedBy(4f), text2);
			Text.Anchor = (TextAnchor)4;
		}
		else
		{
			text = text2;
		}
		Widgets.Label(startButRect, text);
		Text.Anchor = (TextAnchor)0;
	}

	private void DrawProjectProgress(Rect rect, ResearchProjectDef project, string prefixTitle = null, float prefixWidth = 75f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		if (!string.IsNullOrEmpty(prefixTitle))
		{
			Rect rect2 = val;
			((Rect)(ref rect2)).width = prefixWidth;
			((Rect)(ref val)).xMin = ((Rect)(ref rect2)).xMax + 10f;
			using (new TextBlock((TextAnchor)3))
			{
				Widgets.Label(rect2, prefixTitle + ":");
			}
		}
		if (project == null)
		{
			using (new TextBlock((TextAnchor)4))
			{
				Widgets.Label(val, "NoProjectSelected".Translate());
				return;
			}
		}
		val = val.ContractedBy(15f);
		Widgets.FillableBar(val, project.ProgressPercent, ResearchBarFillTex, ResearchBarBGTex, doBorder: true);
		Text.Anchor = (TextAnchor)4;
		string label = project.ProgressApparentString + " / " + project.CostApparent.ToString("F0");
		Widgets.Label(val, label);
		Rect rect3 = val;
		((Rect)(ref rect3)).y = ((Rect)(ref val)).y - 22f;
		((Rect)(ref rect3)).height = 22f;
		float x = Text.CalcSize(project.LabelCap).x;
		Widgets.Label(rect3, project.LabelCap.Truncate(((Rect)(ref rect3)).width));
		if (x > ((Rect)(ref rect3)).width)
		{
			TooltipHandler.TipRegion(rect3, project.LabelCap);
			Widgets.DrawHighlightIfMouseover(rect3);
		}
		Text.Anchor = (TextAnchor)0;
	}

	private void AttemptBeginResearch(ResearchProjectDef projectToStart)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		List<(BuildableDef, List<string>)> list = ComputeUnlockedDefsThatHaveMissingMemes(projectToStart);
		if (!list.Any())
		{
			DoBeginResearch(projectToStart);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("ResearchProjectHasDefsWithMissingMemes".Translate(projectToStart.LabelCap)).Append(":");
		stringBuilder.AppendLine();
		foreach (var (buildableDef, items) in list)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  - ").Append(buildableDef.LabelCap.Colorize(ColoredText.NameColor)).Append(" (")
				.Append(items.ToCommaList())
				.Append(")");
		}
		Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(stringBuilder.ToString(), delegate
		{
			DoBeginResearch(projectToStart);
		}));
		SoundDefOf.Tick_Low.PlayOneShotOnCamera();
	}

	private List<(BuildableDef, List<string>)> ComputeUnlockedDefsThatHaveMissingMemes(ResearchProjectDef project)
	{
		cachedDefsWithMissingMemes.Clear();
		if (!ModsConfig.IdeologyActive)
		{
			return cachedDefsWithMissingMemes;
		}
		if (Faction.OfPlayer.ideos?.PrimaryIdeo == null)
		{
			return cachedDefsWithMissingMemes;
		}
		foreach (Def unlockedDef in project.UnlockedDefs)
		{
			if (!(unlockedDef is BuildableDef { canGenerateDefaultDesignator: false } buildableDef))
			{
				continue;
			}
			List<string> list = null;
			foreach (MemeDef item in DefDatabase<MemeDef>.AllDefsListForReading)
			{
				if (!Faction.OfPlayer.ideos.HasAnyIdeoWithMeme(item) && item.AllDesignatorBuildables.Contains(buildableDef))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(item.LabelCap);
				}
			}
			if (list != null)
			{
				cachedDefsWithMissingMemes.Add((buildableDef, list));
			}
		}
		return cachedDefsWithMissingMemes;
	}

	private void DoBeginResearch(ResearchProjectDef projectToStart)
	{
		SoundDefOf.ResearchStart.PlayOneShotOnCamera();
		Find.ResearchManager.SetCurrentProject(projectToStart);
		TutorSystem.Notify_Event("StartResearchProject");
		if ((!ModsConfig.AnomalyActive || projectToStart.knowledgeCategory == null) && !ColonistsHaveResearchBench)
		{
			Messages.Message("MessageResearchMenuWithoutBench".Translate(), MessageTypeDefOf.CautionInput);
		}
	}

	private float CoordToPixelsX(float x)
	{
		return x * 190f;
	}

	private float CoordToPixelsY(float y)
	{
		return y * 100f;
	}

	private float PixelsToCoordX(float x)
	{
		return x / 190f;
	}

	private float PixelsToCoordY(float y)
	{
		return y / 100f;
	}

	private float PosX(ResearchProjectDef d)
	{
		return CoordToPixelsX(d.ResearchViewX);
	}

	private float PosY(ResearchProjectDef d)
	{
		return CoordToPixelsY(d.ResearchViewY);
	}

	public override void PostOpen()
	{
		base.PostOpen();
		tabs.Clear();
		foreach (ResearchTabDef tabDef in DefDatabase<ResearchTabDef>.AllDefs)
		{
			tabs.Add(new ResearchTabRecord(tabDef, tabDef.LabelCap, delegate
			{
				CurTab = tabDef;
				UpdateSelectedProject(Find.ResearchManager);
			}, () => CurTab == tabDef));
		}
	}

	private void DrawRightRect(Rect rightOutRect, float maxTabX)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Invalid comparison between Unknown and I4
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		Rect baseRect = rightOutRect;
		((Rect)(ref baseRect)).xMax = maxTabX;
		((Rect)(ref rightOutRect)).yMin = ((Rect)(ref rightOutRect)).yMin + TabDrawer.GetOverflowTabHeight(baseRect, tabs, 100f, 200f);
		Widgets.DrawMenuSection(rightOutRect);
		TabDrawer.DrawTabsOverflow(baseRect, tabs, 100f, 200f);
		if (Prefs.DevMode)
		{
			Rect rect = rightOutRect;
			((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMin + 20f;
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMax - 80f;
			Rect butRect = rect.RightPartPixels(30f);
			rect = rect.LeftPartPixels(((Rect)(ref rect)).width - 30f);
			Widgets.CheckboxLabeled(rect, "Edit", ref editMode);
			if (Widgets.ButtonImageFitted(butRect, TexButton.Copy))
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ResearchProjectDef item in VisibleResearchProjects.Where((ResearchProjectDef def) => def.Debug_IsPositionModified()))
				{
					stringBuilder.AppendLine(item.defName);
					stringBuilder.AppendLine($"  <researchViewX>{item.ResearchViewX:F2}</researchViewX>");
					stringBuilder.AppendLine($"  <researchViewY>{item.ResearchViewY:F2}</researchViewY>");
					stringBuilder.AppendLine();
				}
				GUIUtility.systemCopyBuffer = stringBuilder.ToString();
				Messages.Message("Modified data copied to clipboard.", MessageTypeDefOf.SituationResolved, historical: false);
			}
		}
		else
		{
			editMode = false;
		}
		bool elementClicked = false;
		Rect val = rightOutRect.ContractedBy(1f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, Mathf.Max(rightViewWidth + 20f, ((Rect)(ref val)).width), ((Rect)(ref rightOutRect)).height - 16f);
		Rect val3 = val2.ContractedBy(20f);
		Rect rect2 = val2;
		((Rect)(ref rect2)).height = ((Rect)(ref rightOutRect)).height;
		scrollPositioner.ClearInterestRects();
		if (Find.ResearchManager.TabInfoVisible(CurTab))
		{
			Widgets.ScrollHorizontal(val, ref rightScrollPosition, val2);
			Widgets.BeginScrollView(val, ref rightScrollPosition, val2);
			if (CurTab == ResearchTabDefOf.Anomaly)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 0.15f);
				GUI.DrawTexture(rect2.LeftPartPixels(830f), (Texture)(object)BasicBackgroundTex.Texture, (ScaleMode)0);
				GUI.DrawTexture(rect2.RightPartPixels(((Rect)(ref rect2)).width - 830f + 1f), (Texture)(object)AdvancedBackgroundTex.Texture, (ScaleMode)0);
				GUI.color = color;
				LessonAutoActivator.TeachOpportunity(ConceptDefOf.AnomalyResearch, OpportunityType.GoodToKnow);
			}
			Widgets.BeginGroup(val3);
			ListProjects(val3, ref elementClicked);
			Widgets.EndGroup();
			Widgets.EndScrollView();
			scrollPositioner.ScrollHorizontally(ref rightScrollPosition, ((Rect)(ref val)).size);
			if (!editMode)
			{
				return;
			}
			if (!elementClicked && Input.GetMouseButtonDown(0))
			{
				draggingTabs.Clear();
			}
			if (draggingTabs.NullOrEmpty())
			{
				return;
			}
			if (Input.GetMouseButtonUp(0))
			{
				for (int i = 0; i < draggingTabs.Count; i++)
				{
					draggingTabs[i].Debug_SnapPositionData();
				}
			}
			else if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0) && (int)Event.current.type == 8)
			{
				for (int j = 0; j < draggingTabs.Count; j++)
				{
					draggingTabs[j].Debug_ApplyPositionDelta(new Vector2(PixelsToCoordX(Event.current.delta.x), PixelsToCoordY(Event.current.delta.y)));
				}
			}
		}
		else
		{
			Widgets.BeginGroup(val);
			using (new TextBlock((TextAnchor)4, ColoredText.SubtleGrayColor))
			{
				Widgets.Label(val.AtZero(), "ResearchNotDiscovered".Translate());
			}
			Widgets.EndGroup();
		}
	}

	private void ListProjects(Rect rightInRect, ref bool elementClicked)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		List<ResearchProjectDef> visibleResearchProjects = VisibleResearchProjects;
		Vector2 start = default(Vector2);
		Vector2 end = default(Vector2);
		for (int i = 0; i < 2; i++)
		{
			foreach (ResearchProjectDef item in visibleResearchProjects)
			{
				if (item.tab != CurTab)
				{
					continue;
				}
				float num = 0f;
				if (ModsConfig.AnomalyActive && item.knowledgeCategory != null)
				{
					num = 14f;
				}
				start.x = PosX(item);
				start.y = PosY(item) + 25f;
				for (int j = 0; j < item.prerequisites.CountAllowNull(); j++)
				{
					ResearchProjectDef researchProjectDef = item.prerequisites[j];
					if (researchProjectDef == null || researchProjectDef.tab != CurTab)
					{
						continue;
					}
					end.x = PosX(researchProjectDef) + 140f + num;
					end.y = PosY(researchProjectDef) + 25f;
					if (selectedProject == item || selectedProject == researchProjectDef)
					{
						if (i == 1)
						{
							Widgets.DrawLine(start, end, TexUI.HighlightLineResearchColor, 4f);
						}
					}
					else if (i == 0)
					{
						Widgets.DrawLine(start, end, TexUI.DefaultLineResearchColor, 2f);
					}
				}
			}
		}
		Rect val = GenUI.ExpandedBy(new Rect(rightScrollPosition.x, rightScrollPosition.y, ((Rect)(ref rightInRect)).width, ((Rect)(ref rightInRect)).height), 10f);
		Rect rect = default(Rect);
		foreach (ResearchProjectDef project in visibleResearchProjects)
		{
			if (project.tab != CurTab)
			{
				continue;
			}
			float num2 = 0f;
			if (ModsConfig.AnomalyActive && project.knowledgeCategory != null)
			{
				num2 = 14f;
			}
			((Rect)(ref rect))._002Ector(PosX(project), PosY(project), 140f, 50f);
			((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax + num2;
			string label = GetLabel(project);
			Rect rect2 = rect;
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + num2;
			Widgets.LabelCacheHeight(ref rect2, label);
			Rect rect3 = rect2;
			((Rect)(ref rect3)).y = ((Rect)(ref rect2)).yMax - 4f;
			Widgets.LabelCacheHeight(ref rect3, " ");
			((Rect)(ref rect)).yMax = ((Rect)(ref rect3)).yMax;
			bool flag = quickSearchWidget.filter.Active && !matchingProjects.Contains(project);
			bool flag2 = quickSearchWidget.filter.Active && matchingProjects.Contains(project);
			if (flag2 || selectedProject == project)
			{
				scrollPositioner.RegisterInterestRect(rect);
			}
			if (project.IsHidden)
			{
				label = string.Format("({0})", "UnknownResearch".Translate());
			}
			if (!((Rect)(ref rect)).Overlaps(val))
			{
				continue;
			}
			Color val2 = Widgets.NormalOptionColor;
			Color val3 = TexUI.OtherActiveResearchColor;
			Color windowBGFillColor = Widgets.WindowBGFillColor;
			Color val4 = default(Color);
			bool flag3 = !project.IsFinished && !project.CanStartNow;
			bool flag4 = false;
			if (project.IsHidden)
			{
				val3 = TexUI.HiddenResearchColor;
				val2 = HiddenProjectLabelColor;
			}
			else if (Find.ResearchManager.IsCurrentProject(project))
			{
				val3 = TexUI.ActiveResearchColor;
				val2 = ActiveProjectLabelColor;
			}
			else if (project.IsFinished)
			{
				val3 = TexUI.FinishedResearchColor;
			}
			else if (flag3)
			{
				val3 = TexUI.LockedResearchColor;
			}
			if (flag3)
			{
				val2 = ProjectWithMissingPrerequisiteLabelColor;
			}
			if (editMode && draggingTabs.Contains(project))
			{
				val4 = Color.yellow;
			}
			else if (selectedProject == project)
			{
				val3 += TexUI.HighlightBgResearchColor;
				val4 = TexUI.BorderResearchSelectedColor;
				val2 = TexUI.BorderResearchSelectedColor;
				flag4 = true;
			}
			else if (Find.ResearchManager.IsCurrentProject(project))
			{
				val4 = TexUI.BorderResearchingColor;
				flag4 = true;
			}
			else
			{
				val4 = TexUI.DefaultBorderResearchColor;
			}
			if (selectedProject != null)
			{
				if (project.prerequisites.NotNullAndContains(selectedProject) || project.hiddenPrerequisites.NotNullAndContains(selectedProject))
				{
					val4 = TexUI.HighlightLineResearchColor;
				}
				if (selectedProject.prerequisites.NotNullAndContains(project) || selectedProject.hiddenPrerequisites.NotNullAndContains(project))
				{
					val4 = (project.IsFinished ? TexUI.HighlightLineResearchColor : TexUI.DependencyOutlineResearchColor);
				}
			}
			Color val5 = (project.TechprintRequirementMet ? FulfilledPrerequisiteColor : MissingPrerequisiteColor);
			Color val6 = (project.AnalyzedThingsRequirementsMet ? FulfilledPrerequisiteColor : MissingPrerequisiteColor);
			if (flag)
			{
				val2 = NoMatchTint(val2);
				val3 = NoMatchTint(val3);
				val4 = NoMatchTint(val4);
				val5 = NoMatchTint(val5);
				val6 = NoMatchTint(val6);
			}
			if (flag2)
			{
				Widgets.DrawStrongHighlight(rect.ExpandedBy(4f));
			}
			int num3 = ((!flag4) ? 1 : 2);
			if (Widgets.CustomButtonText(ref rect, "", val3, val2, val4, windowBGFillColor, cacheHeight: false, num3, doMouseoverSound: true, active: true, project.ProgressPercent) && !project.IsHidden)
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				selectedProject = project;
			}
			Color color = GUI.color;
			TextAnchor anchor = Text.Anchor;
			if (ModsConfig.AnomalyActive && project.knowledgeCategory != null)
			{
				Rect val7 = rect;
				((Rect)(ref val7)).x = ((Rect)(ref val7)).x + 4f;
				((Rect)(ref val7)).width = 14f;
				GUI.color = project.knowledgeCategory.color;
				GUI.DrawTexture(val7, (Texture)(object)project.knowledgeCategory.Tex, (ScaleMode)2);
			}
			GUI.color = val2;
			Text.Anchor = (TextAnchor)1;
			Widgets.Label(rect2, label);
			if (!project.IsHidden)
			{
				DrawBottomRow(rect3, project, val5, val6);
				if (Mouse.IsOver(rect) && !editMode)
				{
					Widgets.DrawLightHighlight(rect);
					TooltipHandler.TipRegion(rect, () => project.GetTip(), project.GetHashCode() ^ 0x1664F);
				}
			}
			GUI.color = color;
			Text.Anchor = anchor;
			if (!editMode || !Mouse.IsOver(rect2) || !Input.GetMouseButtonDown(0))
			{
				continue;
			}
			elementClicked = true;
			if (Input.GetKey((KeyCode)304))
			{
				if (!draggingTabs.Contains(project))
				{
					draggingTabs.Add(project);
				}
			}
			else if (!Input.GetKey((KeyCode)306) && !draggingTabs.Contains(project))
			{
				draggingTabs.Clear();
				draggingTabs.Add(project);
			}
			if (Input.GetKey((KeyCode)306) && draggingTabs.Contains(project))
			{
				draggingTabs.Remove(project);
			}
		}
	}

	private void DrawBottomRow(Rect rect, ResearchProjectDef project, Color techprintColor, Color studiedColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		Color color = GUI.color;
		TextAnchor anchor = Text.Anchor;
		int num = 1;
		if (project.TechprintCount > 0)
		{
			num++;
		}
		if (project.RequiredAnalyzedThingCount > 0)
		{
			num++;
		}
		float num2 = ((Rect)(ref rect)).width / (float)num;
		Rect val = rect;
		((Rect)(ref val)).x = ((Rect)(ref rect)).x;
		((Rect)(ref val)).width = num2;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(val, project.CostApparent.ToString());
		((Rect)(ref val)).x = ((Rect)(ref val)).x + num2;
		if (project.TechprintCount > 0)
		{
			string text = GetTechprintsInfoCached(project.TechprintsApplied, project.TechprintCount);
			Vector2 val2 = Text.CalcSize(text);
			Rect rect2 = val;
			((Rect)(ref rect2)).xMin = ((Rect)(ref val)).xMax - val2.x - 10f;
			Rect rect3 = val;
			((Rect)(ref rect3)).width = ((Rect)(ref rect3)).height;
			((Rect)(ref rect3)).x = ((Rect)(ref rect2)).x - ((Rect)(ref rect3)).width;
			GUI.color = techprintColor;
			Widgets.Label(rect2, text);
			GUI.color = Color.white;
			GUI.DrawTexture(rect3.ContractedBy(4f), (Texture)(object)TechprintRequirementTex.Texture);
			((Rect)(ref val)).x = ((Rect)(ref val)).x + num2;
		}
		if (project.RequiredAnalyzedThingCount > 0)
		{
			string text2 = GetTechprintsInfoCached(project.AnalyzedThingsCompleted, project.RequiredAnalyzedThingCount);
			Vector2 val3 = Text.CalcSize(text2);
			Rect rect4 = val;
			((Rect)(ref rect4)).xMin = ((Rect)(ref val)).xMax - val3.x - 10f;
			Rect rect5 = val;
			((Rect)(ref rect5)).width = ((Rect)(ref rect5)).height;
			((Rect)(ref rect5)).x = ((Rect)(ref rect4)).x - ((Rect)(ref rect5)).width;
			GUI.color = studiedColor;
			Widgets.Label(rect4, text2);
			GUI.color = Color.white;
			GUI.DrawTexture(rect5.ContractedBy(4f), (Texture)(object)StudyRequirementTex.Texture);
		}
		GUI.color = color;
		Text.Anchor = anchor;
	}

	private Color NoMatchTint(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Color.Lerp(color, NoMatchTintColor, 0.4f);
	}

	private void DrawResearchPrerequisites(Rect rect, ref float y, ResearchProjectDef project)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (project.prerequisites.NullOrEmpty() && project.hiddenPrerequisites.NullOrEmpty())
		{
			return;
		}
		Widgets.Label(rect, ref y, string.Format("{0}:", "Prerequisites".Translate()));
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 6f;
		foreach (ResearchProjectDef item in project.prerequisites.OrElseEmptyEnumerable())
		{
			SetPrerequisiteStatusColor(item.IsFinished, project);
			Widgets.Label(rect, ref y, $"- {item.LabelCap}");
		}
		if (project.hiddenPrerequisites != null)
		{
			foreach (ResearchProjectDef hiddenPrerequisite in project.hiddenPrerequisites)
			{
				SetPrerequisiteStatusColor(hiddenPrerequisite.IsFinished, project);
				Widgets.Label(rect, ref y, $"- {hiddenPrerequisite.LabelCap}");
			}
		}
		GUI.color = Color.white;
	}

	private string GetLabelWithNewlineCached(string label)
	{
		if (!labelsWithNewlineCached.ContainsKey(label))
		{
			labelsWithNewlineCached.Add(label, label + "\n");
		}
		return labelsWithNewlineCached[label];
	}

	private string GetTechprintsInfoCached(int applied, int total)
	{
		Pair<int, int> key = new Pair<int, int>(applied, total);
		if (!techprintsInfoCached.ContainsKey(key))
		{
			techprintsInfoCached.Add(key, applied + " / " + total);
		}
		return techprintsInfoCached[key];
	}

	private float DrawResearchBenchRequirements(ResearchProjectDef project, Rect rect)
	{
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		float xMin = ((Rect)(ref rect)).xMin;
		float yMin = ((Rect)(ref rect)).yMin;
		if (project.requiredResearchBuilding != null)
		{
			bool present = false;
			foreach (Map map in Find.Maps)
			{
				if (map.listerBuildings.allBuildingsColonist.Find((Building x) => x.def == project.requiredResearchBuilding) != null)
				{
					present = true;
					break;
				}
			}
			Widgets.LabelCacheHeight(ref rect, "RequiredResearchBench".Translate() + ":");
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 6f;
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			SetPrerequisiteStatusColor(present, project);
			((Rect)(ref rect)).height = Text.CalcHeight(project.requiredResearchBuilding.LabelCap, ((Rect)(ref rect)).width - 24f - 6f);
			Widgets.HyperlinkWithIcon(rect, new Dialog_InfoCard.Hyperlink(project.requiredResearchBuilding));
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + (((Rect)(ref rect)).height + 4f);
			GUI.color = Color.white;
			((Rect)(ref rect)).xMin = xMin;
		}
		if (!project.requiredResearchFacilities.NullOrEmpty())
		{
			Widgets.LabelCacheHeight(ref rect, "RequiredResearchBenchFacilities".Translate() + ":");
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			Building_ResearchBench building_ResearchBench = FindBenchFulfillingMostRequirements(project.requiredResearchBuilding, project.requiredResearchFacilities);
			CompAffectedByFacilities bestMatchingBench = null;
			if (building_ResearchBench != null)
			{
				bestMatchingBench = building_ResearchBench.TryGetComp<CompAffectedByFacilities>();
			}
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 6f;
			foreach (ThingDef requiredResearchFacility in project.requiredResearchFacilities)
			{
				DrawResearchBenchFacilityRequirement(requiredResearchFacility, bestMatchingBench, project, ref rect);
				((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			}
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 4f;
		}
		GUI.color = Color.white;
		((Rect)(ref rect)).xMin = xMin;
		return ((Rect)(ref rect)).yMin - yMin;
	}

	private float DrawStudyRequirements(ResearchProjectDef project, Rect rect)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		float yMin = ((Rect)(ref rect)).yMin;
		if (project.RequiredAnalyzedThingCount > 0)
		{
			Widgets.LabelCacheHeight(ref rect, "StudyRequirements".Translate() + ":");
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 6f;
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			Rect rect2 = default(Rect);
			foreach (ThingDef item in project.requiredAnalyzed)
			{
				((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMin, ((Rect)(ref rect)).width, 24f);
				Color? color = null;
				if (quickSearchWidget.filter.Active)
				{
					if (MatchesUnlockedDef(item))
					{
						Widgets.DrawTextHighlight(rect2);
					}
					else
					{
						color = NoMatchTint(Widgets.NormalOptionColor);
					}
				}
				Widgets.HyperlinkWithIcon(hyperlink: new Dialog_InfoCard.Hyperlink(item), rect: rect2, text: null, iconMargin: 2f, textOffsetX: 6f, color: color, truncateLabel: false, textSuffix: LabelSuffixForUnlocked(item));
				((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 24f;
			}
		}
		return ((Rect)(ref rect)).yMin - yMin;
	}

	private float DrawInspectionRequirements(ResearchProjectDef project, Rect rect)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float yMin = ((Rect)(ref rect)).yMin;
		if (ModsConfig.OdysseyActive && project.requireGravEngineInspected)
		{
			Widgets.LabelCacheHeight(ref rect, "RequiresInspectionof".Translate() + ":");
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 24f;
			((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 6f;
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMin, ((Rect)(ref rect)).width, 24f);
			Widgets.HyperlinkWithIcon(rect2, new Dialog_InfoCard.Hyperlink(ThingDefOf.GravEngine));
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect2)).height;
		}
		return ((Rect)(ref rect)).yMin - yMin;
	}

	private float DrawUnlockableHyperlinks(Rect rect, Rect visibleRect, ResearchProjectDef project)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		List<Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>>> list = UnlockedDefsGroupedByPrerequisites(project);
		if (list.NullOrEmpty())
		{
			return 0f;
		}
		float yMin = ((Rect)(ref rect)).yMin;
		float x = ((Rect)(ref rect)).x;
		Rect val = default(Rect);
		foreach (Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>> item in list)
		{
			ResearchPrerequisitesUtility.UnlockedHeader first = item.First;
			((Rect)(ref rect)).x = x;
			if (!first.unlockedBy.Any())
			{
				Widgets.LabelCacheHeight(ref rect, "Unlocks".Translate() + ":");
			}
			else
			{
				Widgets.LabelCacheHeight(ref rect, string.Concat("UnlockedWith".Translate(), " ", HeaderLabel(first), ":"));
			}
			((Rect)(ref rect)).x = ((Rect)(ref rect)).x + 6f;
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			foreach (Def item2 in item.Second)
			{
				((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMin, ((Rect)(ref rect)).width, 24f);
				if (((Rect)(ref visibleRect)).Overlaps(val))
				{
					Color? color = null;
					if (quickSearchWidget.filter.Active)
					{
						if (MatchesUnlockedDef(item2))
						{
							Widgets.DrawTextHighlight(val);
						}
						else
						{
							color = NoMatchTint(Widgets.NormalOptionColor);
						}
					}
					Widgets.HyperlinkWithIcon(hyperlink: new Dialog_InfoCard.Hyperlink(item2), rect: val, text: null, iconMargin: 2f, textOffsetX: 6f, color: color, truncateLabel: false, textSuffix: LabelSuffixForUnlocked(item2));
				}
				((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 24f;
			}
		}
		return ((Rect)(ref rect)).yMin - yMin;
	}

	private float DrawCustomUnlockables(Rect rect, ResearchProjectDef project)
	{
		if (project.customUnlockTexts.NullOrEmpty())
		{
			return 0f;
		}
		float yMin = ((Rect)(ref rect)).yMin;
		for (int i = 0; i < project.customUnlockTexts.Count; i++)
		{
			if (i == 0)
			{
				Widgets.LabelCacheHeight(ref rect, "Unlocks".Translate() + ":");
				((Rect)(ref rect)).x = ((Rect)(ref rect)).x + 6f;
				((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
			}
			Widgets.LabelCacheHeight(ref rect, project.customUnlockTexts[i]);
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + ((Rect)(ref rect)).height;
		}
		return ((Rect)(ref rect)).yMin - yMin;
	}

	private float DrawContentSource(Rect rect, ResearchProjectDef project)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (project.modContentPack == null || project.modContentPack.IsCoreMod)
		{
			return 0f;
		}
		float yMin = ((Rect)(ref rect)).yMin;
		TaggedString taggedString = "Stat_Source_Label".Translate() + ":  " + project.modContentPack.Name;
		Widgets.LabelCacheHeight(ref rect, taggedString.Colorize(Color.grey));
		ExpansionDef expansionDef = ModLister.AllExpansions.Find((ExpansionDef e) => e.linkedMod == project.modContentPack.PackageId);
		if (expansionDef != null)
		{
			GUI.DrawTexture(new Rect(Text.CalcSize(taggedString).x + 4f, ((Rect)(ref rect)).y, 20f, 20f), (Texture)(object)expansionDef.IconFromStatus);
		}
		return ((Rect)(ref rect)).yMax - yMin;
	}

	private string LabelSuffixForUnlocked(Def unlocked)
	{
		if (!ModLister.IdeologyInstalled)
		{
			return null;
		}
		tmpSuffixesForUnlocked.Clear();
		foreach (MemeDef allDef in DefDatabase<MemeDef>.AllDefs)
		{
			if (allDef.AllDesignatorBuildables.Contains(unlocked))
			{
				tmpSuffixesForUnlocked.AddUnique(allDef.LabelCap);
			}
			if (allDef.thingStyleCategories.NullOrEmpty())
			{
				continue;
			}
			foreach (ThingStyleCategoryWithPriority thingStyleCategory in allDef.thingStyleCategories)
			{
				if (thingStyleCategory.category.AllDesignatorBuildables.Contains(unlocked))
				{
					tmpSuffixesForUnlocked.AddUnique(allDef.LabelCap);
				}
			}
		}
		foreach (CultureDef allDef2 in DefDatabase<CultureDef>.AllDefs)
		{
			if (allDef2.thingStyleCategories.NullOrEmpty())
			{
				continue;
			}
			foreach (ThingStyleCategoryWithPriority thingStyleCategory2 in allDef2.thingStyleCategories)
			{
				if (thingStyleCategory2.category.AllDesignatorBuildables.Contains(unlocked))
				{
					tmpSuffixesForUnlocked.AddUnique(allDef2.LabelCap);
				}
			}
		}
		if (!tmpSuffixesForUnlocked.Any())
		{
			return null;
		}
		return " (" + tmpSuffixesForUnlocked.ToCommaList() + ")";
	}

	private List<Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>>> UnlockedDefsGroupedByPrerequisites(ResearchProjectDef project)
	{
		if (cachedUnlockedDefsGroupedByPrerequisites == null)
		{
			cachedUnlockedDefsGroupedByPrerequisites = new Dictionary<ResearchProjectDef, List<Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>>>>();
		}
		if (!cachedUnlockedDefsGroupedByPrerequisites.TryGetValue(project, out var value))
		{
			value = ResearchPrerequisitesUtility.UnlockedDefsGroupedByPrerequisites(project);
			cachedUnlockedDefsGroupedByPrerequisites.Add(project, value);
		}
		return value;
	}

	private string HeaderLabel(ResearchPrerequisitesUtility.UnlockedHeader headerProject)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		string value = "";
		for (int i = 0; i < headerProject.unlockedBy.Count; i++)
		{
			ResearchProjectDef researchProjectDef = headerProject.unlockedBy[i];
			string text = researchProjectDef.LabelCap;
			if (!researchProjectDef.IsFinished)
			{
				text = text.Colorize(MissingPrerequisiteColor);
			}
			stringBuilder.Append(value).Append(text);
			value = ", ";
		}
		return stringBuilder.ToString();
	}

	private void DrawTechprintInfo(Rect rect, ref float y)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		if (selectedProject.TechprintCount == 0)
		{
			return;
		}
		float xMin = ((Rect)(ref rect)).xMin;
		float yMin = ((Rect)(ref rect)).yMin;
		string text = "ResearchTechprintsFromFactions".Translate();
		float num = Text.CalcHeight(text, ((Rect)(ref rect)).width);
		Widgets.Label(new Rect(((Rect)(ref rect)).x, yMin, ((Rect)(ref rect)).width, num), text);
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + 6f;
		if (selectedProject.heldByFactionCategoryTags != null)
		{
			Rect rect2 = default(Rect);
			foreach (string heldByFactionCategoryTag in selectedProject.heldByFactionCategoryTags)
			{
				foreach (Faction item in Find.FactionManager.AllFactionsInViewOrder)
				{
					if (item.def.categoryTag == heldByFactionCategoryTag)
					{
						string name = item.Name;
						((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, yMin + num, ((Rect)(ref rect)).width, Mathf.Max(24f, Text.CalcHeight(name, ((Rect)(ref rect)).width - 24f - 6f)));
						Widgets.BeginGroup(rect2);
						Rect r = GenUI.ContractedBy(new Rect(0f, 0f, 24f, 24f), 2f);
						FactionUIUtility.DrawFactionIconWithTooltip(r, item);
						Rect rect3 = new Rect(((Rect)(ref r)).xMax + 6f, 0f, ((Rect)(ref rect2)).width - ((Rect)(ref r)).width - 6f, ((Rect)(ref rect2)).height);
						Text.Anchor = (TextAnchor)3;
						Text.WordWrap = false;
						Widgets.Label(rect3, item.Name);
						Text.Anchor = (TextAnchor)0;
						Text.WordWrap = true;
						Widgets.EndGroup();
						num += ((Rect)(ref rect2)).height;
					}
				}
			}
		}
		((Rect)(ref rect)).xMin = xMin;
		y += num;
	}

	private string GetLabel(ResearchProjectDef r)
	{
		return r.LabelCap;
	}

	private void SetPrerequisiteStatusColor(bool present, ResearchProjectDef project)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!project.IsFinished)
		{
			GUI.color = (present ? FulfilledPrerequisiteColor : MissingPrerequisiteColor);
		}
	}

	private void DrawResearchBenchFacilityRequirement(ThingDef requiredFacility, CompAffectedByFacilities bestMatchingBench, ResearchProjectDef project, ref Rect rect)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Thing thing = null;
		Thing thing2 = null;
		if (bestMatchingBench != null)
		{
			thing = bestMatchingBench.LinkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacility);
			thing2 = bestMatchingBench.LinkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacility && bestMatchingBench.IsFacilityActive(x));
		}
		SetPrerequisiteStatusColor(thing2 != null, project);
		string text = requiredFacility.LabelCap;
		if (thing != null && thing2 == null)
		{
			text += " (" + "InactiveFacility".Translate() + ")";
		}
		((Rect)(ref rect)).height = Text.CalcHeight(text, ((Rect)(ref rect)).width - 24f - 6f);
		Widgets.HyperlinkWithIcon(rect, new Dialog_InfoCard.Hyperlink(requiredFacility), text);
	}

	private Building_ResearchBench FindBenchFulfillingMostRequirements(ThingDef requiredResearchBench, List<ThingDef> requiredFacilities)
	{
		tmpAllBuildings.Clear();
		List<Map> maps = Find.Maps;
		for (int i = 0; i < maps.Count; i++)
		{
			tmpAllBuildings.AddRange(maps[i].listerBuildings.allBuildingsColonist);
		}
		float num = 0f;
		Building_ResearchBench building_ResearchBench = null;
		for (int j = 0; j < tmpAllBuildings.Count; j++)
		{
			if (tmpAllBuildings[j] is Building_ResearchBench building_ResearchBench2 && (requiredResearchBench == null || building_ResearchBench2.def == requiredResearchBench))
			{
				float researchBenchRequirementsScore = GetResearchBenchRequirementsScore(building_ResearchBench2, requiredFacilities);
				if (building_ResearchBench == null || researchBenchRequirementsScore > num)
				{
					num = researchBenchRequirementsScore;
					building_ResearchBench = building_ResearchBench2;
				}
			}
		}
		tmpAllBuildings.Clear();
		return building_ResearchBench;
	}

	private float GetResearchBenchRequirementsScore(Building_ResearchBench bench, List<ThingDef> requiredFacilities)
	{
		float num = 0f;
		for (int i = 0; i < requiredFacilities.Count; i++)
		{
			CompAffectedByFacilities benchComp = bench.GetComp<CompAffectedByFacilities>();
			if (benchComp != null)
			{
				List<Thing> linkedFacilitiesListForReading = benchComp.LinkedFacilitiesListForReading;
				if (linkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacilities[i] && benchComp.IsFacilityActive(x)) != null)
				{
					num += 1f;
				}
				else if (linkedFacilitiesListForReading.Find((Thing x) => x.def == requiredFacilities[i]) != null)
				{
					num += 0.6f;
				}
			}
		}
		return num;
	}

	private void UpdateSearchResults()
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		quickSearchWidget.noResultsMatched = false;
		matchingProjects.Clear();
		foreach (ResearchTabRecord tab2 in tabs)
		{
			tab2.Reset();
		}
		if (!quickSearchWidget.filter.Active)
		{
			return;
		}
		foreach (ResearchProjectDef visibleResearchProject in VisibleResearchProjects)
		{
			if (!visibleResearchProject.IsHidden && (quickSearchWidget.filter.Matches(GetLabel(visibleResearchProject)) || MatchesUnlockedDefs(visibleResearchProject)))
			{
				matchingProjects.Add(visibleResearchProject);
			}
		}
		quickSearchWidget.noResultsMatched = !matchingProjects.Any();
		foreach (ResearchTabRecord tab in tabs)
		{
			tab.firstMatch = (from p in matchingProjects
				where tab.def == p.tab
				orderby p.ResearchViewX
				select p).FirstOrDefault();
			if (!tab.AnyMatches)
			{
				tab.labelColor = Color.grey;
			}
		}
		if (!CurTabRecord.AnyMatches)
		{
			foreach (ResearchTabRecord tab3 in tabs)
			{
				if (tab3.AnyMatches)
				{
					CurTab = tab3.def;
					break;
				}
			}
		}
		scrollPositioner.Arm();
		if (CurTabRecord.firstMatch != null)
		{
			selectedProject = CurTabRecord.firstMatch;
		}
		bool MatchesUnlockedDefs(ResearchProjectDef proj)
		{
			foreach (Pair<ResearchPrerequisitesUtility.UnlockedHeader, List<Def>> item in UnlockedDefsGroupedByPrerequisites(proj))
			{
				foreach (Def item2 in item.Second)
				{
					if (MatchesUnlockedDef(item2))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private bool MatchesUnlockedDef(Def unlocked)
	{
		return quickSearchWidget.filter.Matches(unlocked.label);
	}

	public override void Notify_ClickOutsideWindow()
	{
		base.Notify_ClickOutsideWindow();
		quickSearchWidget.Unfocus();
	}
}
