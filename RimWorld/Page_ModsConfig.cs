using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Steamworks;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Page_ModsConfig : Page
{
	private ModMetaData primarySelectedMod;

	private int lastSelectedIndex = -1;

	private int lastSelectedGroupId = -1;

	private List<ModMetaData> selectedMods = new List<ModMetaData>();

	private string filter = "";

	private Vector2 activeModListScrollPosition = Vector2.zero;

	private Vector2 inactiveModListScrollPosition = Vector2.zero;

	private Vector2 modDescriptionScrollPosition = Vector2.zero;

	private bool displayFullfilledRequirements;

	private int activeModsHash = -1;

	private int activeModsWhenOpenedHash = -1;

	private int activeGroupId = -1;

	private int inactiveGroupId = -1;

	private Queue<string> pendingReorders = new Queue<string>();

	private int pendingReorderIndex;

	private ModMetaData pendingScrollToMod;

	private double lastClickTime;

	private ModMetaData lastClickedMod;

	private ModMetaData draggedMod;

	private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();

	private bool discardChanges;

	private bool saveChanges;

	private static List<ModMetaData> originalModList;

	private bool modListsDirty;

	private static List<ModMetaData> activeModListOrderCached = new List<ModMetaData>();

	private static List<ModMetaData> inactiveModListOrderCached = new List<ModMetaData>();

	private static List<ModMetaData> filteredActiveModListOrderCached = new List<ModMetaData>();

	private static List<ModMetaData> filteredInactiveModListOrderCached = new List<ModMetaData>();

	private static Dictionary<string, string> modWarningsCached = new Dictionary<string, string>();

	private List<ModRequirement> visibleReqsCached = new List<ModRequirement>();

	private bool anyReqsCached;

	private bool anyReqsInfoToShowCached;

	private bool anyUnfulfilledReqsCached;

	private bool anyOrderingIssuesCached;

	private float modRequirementsHeightCached;

	private Dictionary<string, string> truncatedStringCache = new Dictionary<string, string>();

	private Mod primaryModHandle;

	private const float ModListWidth = 250f;

	private const float ColumnPadding = 4f;

	private const float ModRowHeight = 26f;

	private const float MetaRowHeight = 25f;

	private const float ButtonWidth = 150f;

	private const float ButtonHeight = 30f;

	private const float RequirementBoxInnerOffset = 10f;

	private const float DoubleClickTime = 0.5f;

	private const int DraggedModsLimit = 30;

	private static readonly Color RequirementBoxOutlineColor = new Color(0.25f, 0.25f, 0.25f);

	private static readonly Color UnmetRequirementBoxOutlineColor = new Color(0.62f, 0.18f, 0.18f);

	private static readonly Color UnmetRequirementBoxBGColor = new Color(0.1f, 0.065f, 0.072f);

	private static readonly Color RequirementRowColor = new Color(0.13f, 0.13f, 0.13f);

	private static readonly Color UnmetRequirementRowColor = new Color(0.23f, 0.15f, 0.15f);

	private static readonly Color UnmetRequirementRowColorHighlighted = new Color(0.27f, 0.18f, 0.18f);

	private static readonly Texture2D WarningIcon = Resources.Load<Texture2D>("Textures/UI/Widgets/YellowWarning");

	private static readonly Texture2D ErrorIcon = Resources.Load<Texture2D>("Textures/UI/Widgets/Error");

	private bool ControlIsHeld
	{
		get
		{
			if (!Input.GetKey((KeyCode)306))
			{
				return Input.GetKey((KeyCode)305);
			}
			return true;
		}
	}

	private bool ShiftIsHeld
	{
		get
		{
			if (!Input.GetKey((KeyCode)304))
			{
				return Input.GetKey((KeyCode)303);
			}
			return true;
		}
	}

	private List<ModMetaData> DraggedMods
	{
		get
		{
			if (!ReorderableWidget.Dragging)
			{
				return null;
			}
			if (selectedMods.Contains(draggedMod))
			{
				return selectedMods;
			}
			return new List<ModMetaData> { draggedMod };
		}
	}

	protected override float Margin => 25f;

	public Page_ModsConfig()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		doCloseX = true;
		closeOnCancel = true;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		ModLister.RebuildModList();
		quickSearchWidget.Reset();
		modListsDirty = true;
		primarySelectedMod = ModListsInOrder().Item1.FirstOrDefault();
		activeModsWhenOpenedHash = ModLister.InstalledModsListHash(activeOnly: true);
		originalModList = ModsConfig.ActiveModsInLoadOrder.ToList();
		RecacheSelectedModInfo();
	}

	public override void PostClose()
	{
		if (saveChanges)
		{
			ModsConfig.Save();
		}
		else if (discardChanges)
		{
			DiscardChanges();
		}
		else
		{
			Log.Warning("Page_ModsConfig closed without saving or discarding, discarding anyway...");
			DiscardChanges();
		}
		foreach (ModMetaData item in ModsConfig.ActiveModsInLoadOrder)
		{
			item.UnsetPreviewImage();
		}
		Resources.UnloadUnusedAssets();
		if (saveChanges && activeModsWhenOpenedHash != ModLister.InstalledModsListHash(activeOnly: true))
		{
			ModsConfig.RestartFromChangedMods();
		}
	}

	public override bool OnCloseRequest()
	{
		if (activeModsWhenOpenedHash == ModLister.InstalledModsListHash(activeOnly: true))
		{
			discardChanges = true;
		}
		if (saveChanges || discardChanges)
		{
			return true;
		}
		Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("DiscardModChanges".Translate(), delegate
		{
			discardChanges = true;
			Close();
		}));
		return false;
	}

	private (List<ModMetaData>, List<ModMetaData>) ModListsInOrder()
	{
		if (modListsDirty)
		{
			activeModListOrderCached.Clear();
			inactiveModListOrderCached.Clear();
			filteredActiveModListOrderCached.Clear();
			filteredInactiveModListOrderCached.Clear();
			activeModListOrderCached.AddRange(ModsConfig.ActiveModsInLoadOrder);
			inactiveModListOrderCached.AddRange(from mod in ModLister.AllInstalledMods
				where !mod.Active
				orderby mod.Official descending, mod.ShortName
				select mod);
			filteredActiveModListOrderCached.AddRange(activeModListOrderCached.Where((ModMetaData mod) => mod.Name.ToLower().Contains(filter.ToLower())).ToList());
			filteredInactiveModListOrderCached.AddRange(inactiveModListOrderCached.Where((ModMetaData mod) => mod.Name.ToLower().Contains(filter.ToLower())).ToList());
			modListsDirty = false;
		}
		return (filteredActiveModListOrderCached, filteredInactiveModListOrderCached);
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		if (pendingReorders.Count > 0)
		{
			DoPendingReorder();
		}
		var (list, list2) = ModListsInOrder();
		if ((int)Event.current.type == 4)
		{
			if ((int)Event.current.keyCode == 274)
			{
				OnDownKey(primarySelectedMod.Active ? list : list2);
				Event.current.Use();
			}
			if ((int)Event.current.keyCode == 273)
			{
				OnUpKey(primarySelectedMod.Active ? list : list2);
				Event.current.Use();
			}
		}
		float num = 0f;
		Widgets.BeginGroup(rect);
		quickSearchWidget.OnGUI(new Rect(0f, num, 354f, 30f));
		if (quickSearchWidget.filter.Text != filter)
		{
			modListsDirty = true;
		}
		filter = quickSearchWidget.filter.Text;
		if (Widgets.ButtonText(new Rect(358f, num, 150f, 30f), "ResolveModOrder".Translate()))
		{
			ModsConfig.TrySortMods();
			modListsDirty = true;
		}
		num += 40f;
		Widgets.Label(new Rect(0f, num, 250f, 30f), "Disabled".Translate());
		Widgets.Label(new Rect(254f, num, 250f, 30f), "Enabled".Translate());
		num += 30f;
		float num2 = ((Rect)(ref rect)).height - num - 30f - 10f;
		Rect modListArea = default(Rect);
		((Rect)(ref modListArea))._002Ector(254f, num, 250f, num2);
		Rect modListArea2 = default(Rect);
		((Rect)(ref modListArea2))._002Ector(0f, num, 250f, num2);
		DoModList(modListArea2, list2, ref inactiveModListScrollPosition, ref inactiveGroupId, delegate
		{
		});
		DoModList(modListArea, filteredActiveModListOrderCached, ref activeModListScrollPosition, ref activeGroupId, onReorderedAction);
		ReorderableWidget.NewMultiGroup(new List<int> { activeGroupId, inactiveGroupId }, onMultiReorderedAction);
		DoBottomButtons(new Rect(0f, ((Rect)(ref modListArea)).yMax + 10f, 508f, 30f));
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(525f, 0f, ((Rect)(ref rect)).width - 508f - 17f, ((Rect)(ref rect)).height);
		if (primarySelectedMod != null)
		{
			DoModInfo(r, primarySelectedMod);
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref r)).xMax - 200f, ((Rect)(ref r)).yMax - 30f, 200f, 30f), "SaveModChanges".Translate()))
		{
			saveChanges = true;
			Close();
		}
		Widgets.EndGroup();
		draggedMod = null;
		if (ReorderableWidget.GetDraggedIndex >= 0)
		{
			List<ModMetaData> list3 = ((ReorderableWidget.GetDraggedFromGroupID == activeGroupId) ? list : list2);
			int getDraggedIndex = ReorderableWidget.GetDraggedIndex;
			if (!list3.NullOrEmpty() && list3.Count > getDraggedIndex)
			{
				draggedMod = list3[getDraggedIndex];
			}
		}
		if (!DraggedMods.NullOrEmpty())
		{
			Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
			Find.WindowStack.ImmediateWindow(12312541, new Rect(mousePositionOnUIInverted.x - 125f, mousePositionOnUIInverted.y - 13f, 250f, 26f * (float)Math.Min(DraggedMods.Count, 30)), WindowLayer.Super, DoDraggedMods, doBackground: false, absorbInputAroundWindow: false, 0f);
		}
		if (Widgets.ButtonInvisible(rect, doMouseoverSound: false) && !ShiftIsHeld && !ControlIsHeld)
		{
			selectedMods.Clear();
			if (primarySelectedMod != null)
			{
				lastSelectedIndex = GetModIndex(primarySelectedMod);
				lastSelectedGroupId = (primarySelectedMod.Active ? activeGroupId : inactiveGroupId);
			}
			else
			{
				lastSelectedIndex = -1;
				lastSelectedGroupId = -1;
			}
		}
		int num3 = ModLister.InstalledModsListHash(activeOnly: true);
		if (activeModsHash == -1 || activeModsHash != num3)
		{
			modWarningsCached = ModsConfig.GetModWarnings();
			RecacheSelectedModInfo();
			activeModsHash = num3;
			modListsDirty = true;
		}
	}

	private void DoModList(Rect modListArea, List<ModMetaData> modList, ref Vector2 scrollPosition, ref int reorderableGroup, Action<int, int> reorderedAction)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Invalid comparison between Unknown and I4
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		bool flag = reorderableGroup == inactiveGroupId;
		float num = (float)modList.Count * 26f + 8f;
		if (flag)
		{
			num += (float)WorkshopItems.DownloadingItemsCount * 26f;
		}
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref modListArea)).width - 16f, num);
		Widgets.BeginScrollView(modListArea, ref scrollPosition, viewRect);
		float num2 = scrollPosition.y - 26f;
		float num3 = scrollPosition.y + ((Rect)(ref modListArea)).height;
		bool flag2 = (float)modList.Count * 26f > ((Rect)(ref modListArea)).height;
		if ((int)Event.current.type == 7)
		{
			reorderableGroup = ReorderableWidget.NewGroup(reorderedAction, ReorderableDirection.Vertical, modListArea);
		}
		int num4 = 0;
		Rect val = default(Rect);
		foreach (ModMetaData mod in modList)
		{
			((Rect)(ref val))._002Ector(0f, (float)num4 * 26f, 250f, 26f);
			if (flag2)
			{
				((Rect)(ref val)).width = ((Rect)(ref val)).width - 16f;
			}
			ReorderableWidget.Reorderable(reorderableGroup, val, useRightButton: false, highlightDragged: false);
			if (((Rect)(ref val)).y >= num2 && ((Rect)(ref val)).y <= num3)
			{
				DoModRow(val, mod, modList, num4);
			}
			if (mod == pendingScrollToMod)
			{
				int num5 = ((!(((Rect)(ref val)).y <= scrollPosition.y)) ? 1 : (-1));
				while (((Rect)(ref val)).y <= num2 || ((Rect)(ref val)).y + 26f > num3)
				{
					scrollPosition.y += 26f * (float)num5;
					num2 = scrollPosition.y;
					num3 = scrollPosition.y + ((Rect)(ref modListArea)).height;
				}
				pendingScrollToMod = null;
			}
			num4++;
		}
		if (flag)
		{
			Rect r = default(Rect);
			foreach (WorkshopItem_Downloading allDownloadingItem in WorkshopItems.AllDownloadingItems)
			{
				((Rect)(ref r))._002Ector(0f, (float)num4 * 26f, 250f, 26f);
				if (flag2)
				{
					((Rect)(ref r)).width = ((Rect)(ref r)).width - 16f;
				}
				if (((Rect)(ref r)).y >= num2 && ((Rect)(ref r)).y <= num3)
				{
					DoModRowDownloading(r, num4, allDownloadingItem);
				}
				num4++;
			}
		}
		Widgets.EndScrollView();
	}

	private void DoModRow(Rect r, ModMetaData mod, List<ModMetaData> list, int index, bool isDragged = false)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Invalid comparison between Unknown and I4
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		if (!DraggedMods.NullOrEmpty() && DraggedMods.Contains(mod) && !isDragged)
		{
			return;
		}
		if (index % 2 == 1)
		{
			Widgets.DrawLightHighlight(r);
		}
		if (selectedMods.Contains(mod) && !ReorderableWidget.Dragging)
		{
			Widgets.DrawHighlightSelected(r);
		}
		Action clickAction = null;
		if (mod.Source == ContentSource.SteamWorkshop)
		{
			clickAction = delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				SteamUtility.OpenWorkshopPage(mod.GetPublishedFileId());
			};
		}
		if (mod.Source == ContentSource.ModsFolder && ((int)Application.platform == 2 || (int)Application.platform == 7))
		{
			clickAction = delegate
			{
				Application.OpenURL(GenFilePaths.ModsFolderPath + "/" + mod.FolderName);
			};
		}
		ContentSourceUtility.DrawContentSource(r, mod.Source, clickAction);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(r);
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 28f;
		Texture2D val2 = null;
		string text = "";
		Text.Font = GameFont.Small;
		GUI.color = Color.white;
		if (!mod.Active)
		{
			GUI.color = Color.grey;
		}
		if (!mod.VersionCompatible)
		{
			GUI.color = Color.yellow;
			val2 = WarningIcon;
			if (!text.NullOrEmpty())
			{
				text += "\n";
			}
			text = ((!mod.MadeForNewerVersion) ? ((string)(text + "ModNotMadeForThisVersion".Translate())) : ((string)(text + "ModNotMadeForThisVersion_Newer".Translate())));
		}
		if (mod.Active && !modWarningsCached.NullOrEmpty() && modWarningsCached.ContainsKey(mod.PackageId) && !modWarningsCached[mod.PackageId].NullOrEmpty())
		{
			GUI.color = Color.red;
			val2 = ErrorIcon;
			if (!text.NullOrEmpty())
			{
				text += "\n\n";
			}
			text += modWarningsCached[mod.PackageId];
		}
		Text.WordWrap = false;
		Text.Anchor = (TextAnchor)3;
		if ((Object)(object)val2 != (Object)null)
		{
			float num = ((Rect)(ref val)).width - 32f;
			if (Text.CalcSize(mod.ShortName).x > num)
			{
				text = mod.ShortName.Colorize(Color.yellow) + "\n\n" + text;
			}
			string label = mod.ShortName.Truncate(num, truncatedStringCache);
			Widgets.LabelWithIcon(val, label, val2, 0.75f);
		}
		else
		{
			if (Text.CalcSize(mod.ShortName).x > ((Rect)(ref val)).width)
			{
				text = mod.ShortName.Colorize(Color.yellow) + "\n\n" + text;
			}
			string label2 = mod.ShortName.Truncate(((Rect)(ref val)).width, truncatedStringCache);
			Widgets.Label(val, label2);
		}
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		GUI.color = Color.white;
		if (isDragged)
		{
			return;
		}
		if (!ReorderableWidget.Dragging)
		{
			Widgets.DrawHighlightIfMouseover(val);
		}
		if (Widgets.ButtonInvisible(val))
		{
			if (ControlIsHeld)
			{
				if (selectedMods.Count > 0 && selectedMods[0].Active != mod.Active)
				{
					selectedMods.Clear();
				}
				if (ShiftIsHeld)
				{
					for (int i = lastSelectedIndex; i != index; i += Math.Sign(index - lastSelectedIndex))
					{
						selectedMods.Add(list[i]);
					}
					selectedMods.Add(mod);
					primarySelectedMod = mod;
					RecacheSelectedModInfo();
				}
				else if (selectedMods.Count > 1 && selectedMods.Contains(mod))
				{
					selectedMods.Remove(mod);
					if (mod == primarySelectedMod)
					{
						primarySelectedMod = selectedMods[0];
						RecacheSelectedModInfo();
					}
					lastSelectedIndex = GetModIndex(primarySelectedMod);
					lastSelectedGroupId = (primarySelectedMod.Active ? activeGroupId : inactiveGroupId);
				}
				else
				{
					primarySelectedMod = mod;
					RecacheSelectedModInfo();
					selectedMods.Add(primarySelectedMod);
					lastSelectedIndex = index;
					lastSelectedGroupId = (primarySelectedMod.Active ? activeGroupId : inactiveGroupId);
				}
			}
			else
			{
				primarySelectedMod = mod;
				int num2 = (mod.Active ? activeGroupId : inactiveGroupId);
				RecacheSelectedModInfo();
				if (ShiftIsHeld && lastSelectedIndex >= 0 && lastSelectedGroupId == num2)
				{
					selectedMods.Clear();
					for (int j = lastSelectedIndex; j != index; j += Math.Sign(index - lastSelectedIndex))
					{
						selectedMods.Add(list[j]);
					}
					selectedMods.Add(mod);
				}
				else
				{
					selectedMods.Clear();
					selectedMods.Add(primarySelectedMod);
					lastSelectedIndex = index;
					lastSelectedGroupId = (primarySelectedMod.Active ? activeGroupId : inactiveGroupId);
				}
			}
			if (lastClickedMod == mod && (double)RealTime.LastRealTime < lastClickTime + 0.5)
			{
				foreach (ModMetaData selectedMod in selectedMods)
				{
					if (selectedMod.Active)
					{
						TrySetModInactive(selectedMod);
					}
					else
					{
						TrySetModActive(selectedMod);
					}
				}
			}
			else
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
			}
			lastClickTime = RealTime.LastRealTime;
			lastClickedMod = mod;
		}
		if (!text.NullOrEmpty())
		{
			TooltipHandler.TipRegion(val, new TipSignal(text, mod.GetHashCode() * 3311));
		}
	}

	private void DoModRowDownloading(Rect r, int index, WorkshopItem_Downloading item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		ContentSourceUtility.DrawContentSource(r, ContentSource.SteamWorkshop);
		((Rect)(ref r)).xMin = ((Rect)(ref r)).xMin + 28f;
		float height = ((Rect)(ref r)).height;
		((Rect)(ref r)).xMax = ((Rect)(ref r)).xMax - height;
		Text.WordWrap = false;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(r, "Downloading".Translate() + GenText.MarchingEllipsis());
		Text.Anchor = (TextAnchor)0;
		Text.WordWrap = true;
		Rect val = new Rect(((Rect)(ref r)).xMax, ((Rect)(ref r)).y, height, height);
		GUI.DrawTexture(val, (Texture)(object)Widgets.CheckboxOffTex);
		Widgets.DrawHighlightIfMouseover(val);
		if (Widgets.ButtonInvisible(val))
		{
			Workshop.Unsubscribe(item.PublishedFileId);
		}
		Rect rect = r;
		object arg = "WorkshopId".Translate();
		PublishedFileId_t publishedFileId = item.PublishedFileId;
		TooltipHandler.TipRegion(rect, $"{arg}: {((object)(PublishedFileId_t)(ref publishedFileId)).ToString()}");
	}

	private void DoModInfo(Rect r, ModMetaData mod)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Invalid comparison between Unknown and I4
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Invalid comparison between Unknown and I4
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Invalid comparison between Unknown and I4
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Invalid comparison between Unknown and I4
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(r);
		float num = ((Rect)(ref r)).y;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, num, 0f, 20f);
		Texture2D previewImage = mod.PreviewImage;
		if ((Object)(object)previewImage != (Object)null)
		{
			((Rect)(ref val)).width = Mathf.Min((float)((Texture)previewImage).width, ((Rect)(ref r)).width);
			((Rect)(ref val)).height = (float)((Texture)previewImage).height * (((Rect)(ref val)).width / (float)((Texture)previewImage).width);
			float num2 = Mathf.Ceil(((Rect)(ref r)).height * 0.35f);
			if (((Rect)(ref val)).height > num2)
			{
				float height = ((Rect)(ref val)).height;
				((Rect)(ref val)).height = num2;
				((Rect)(ref val)).width = ((Rect)(ref val)).width * (((Rect)(ref val)).height / height);
			}
			((Rect)(ref val)).x = ((Rect)(ref r)).width / 2f - ((Rect)(ref val)).width / 2f;
			GUI.DrawTexture(val, (Texture)(object)previewImage, (ScaleMode)2);
			num += ((Rect)(ref val)).height + 10f;
		}
		float num3 = num;
		Text.Font = GameFont.Medium;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num3, ((Rect)(ref r)).width, 40f);
		Widgets.Label(rect, mod.Name.Truncate(((Rect)(ref rect)).width));
		Text.Font = GameFont.Small;
		num3 += ((Rect)(ref rect)).height;
		if (!mod.AuthorsString.NullOrEmpty())
		{
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(0f, num3, ((Rect)(ref r)).width / 2f, Text.LineHeight);
			string text = ("Author".Translate() + ": ").Colorize(Color.gray) + mod.AuthorsString;
			Widgets.Label(rect2, text.Truncate(((Rect)(ref r)).width / 2f));
			if (Text.CalcSize(text).x > ((Rect)(ref rect2)).width)
			{
				Widgets.DrawHighlightIfMouseover(rect2);
				TooltipHandler.TipRegion(rect2, new TipSignal(text));
			}
			num3 += ((Rect)(ref rect2)).height;
		}
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, num3, ((Rect)(ref r)).width / 2f, Text.LineHeight);
		string text2 = ("ModPackageId".Translate() + ": ").Colorize(Color.gray) + mod.packageIdLowerCase;
		Widgets.Label(rect3, text2.Truncate(((Rect)(ref r)).width / 2f));
		if (Text.CalcSize(text2).x > ((Rect)(ref rect3)).width)
		{
			Widgets.DrawHighlightIfMouseover(rect3);
			TooltipHandler.TipRegion(rect3, new TipSignal(text2));
		}
		num3 += ((Rect)(ref rect3)).height;
		if (!mod.IsCoreMod)
		{
			Rect rect4 = default(Rect);
			((Rect)(ref rect4))._002Ector(0f, num3, ((Rect)(ref r)).width / 2f, Text.LineHeight);
			Widgets.Label(rect4, ("ModTargetVersion".Translate() + ": ").Colorize(Color.gray) + mod.SupportedVersionsReadOnly.Select(delegate(Version v)
			{
				string text3 = (VersionControl.IsCompatible(v) ? "<color=green>" : "<color=red>");
				string text4 = "</color>";
				return (v.Build > 0) ? $"{text3}{v.Major.ToString()}.{v.Minor.ToString()}.{v.Build.ToString()}{text4}" : $"{text3}{v.Major.ToString()}.{v.Minor.ToString()}{text4}";
			}).ToCommaList());
			num3 += ((Rect)(ref rect4)).height;
		}
		if (!mod.ModVersion.NullOrEmpty())
		{
			Rect rect5 = default(Rect);
			((Rect)(ref rect5))._002Ector(0f, num3, ((Rect)(ref r)).width / 2f, Text.LineHeight);
			Widgets.Label(rect5, ("ModVersion".Translate() + ": ").Colorize(Color.gray) + mod.ModVersion);
			num3 += ((Rect)(ref rect5)).height;
		}
		float yMax = ((Rect)(ref rect)).yMax;
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		if (SteamManager.Initialized && mod.OnSteamWorkshop)
		{
			list.Add(new FloatMenuOption("Unsubscribe".Translate(), delegate
			{
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmUnsubscribeFrom".Translate(mod.Name), delegate
				{
					mod.enabled = false;
					Workshop.Unsubscribe((WorkshopUploadable)mod);
				}, destructive: true));
			}));
			list.Add(new FloatMenuOption("WorkshopPage".Translate(), delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				SteamUtility.OpenWorkshopPage(mod.GetPublishedFileId());
			}));
			if (!mod.Official && ((int)Application.platform == 2 || (int)Application.platform == 7))
			{
				list.Add(new FloatMenuOption("ModFolder".Translate(), delegate
				{
					Application.OpenURL(mod.RootDir.FullName);
				}));
			}
		}
		else
		{
			if (!mod.Url.NullOrEmpty())
			{
				list.Add(new FloatMenuOption("ModWebsite".Translate(), delegate
				{
					Application.OpenURL(mod.Url);
				}));
			}
			if (!mod.Official && ((int)Application.platform == 2 || (int)Application.platform == 7))
			{
				list.Add(new FloatMenuOption("ModFolder".Translate(), delegate
				{
					Application.OpenURL(GenFilePaths.ModsFolderPath + "/" + mod.FolderName);
				}));
			}
		}
		if (primaryModHandle != null && !primaryModHandle.SettingsCategory().NullOrEmpty())
		{
			list.Add(new FloatMenuOption("ModOptions".Translate(), delegate
			{
				Find.WindowStack.Add(new Dialog_ModSettings(primaryModHandle));
			}));
		}
		if (Prefs.DevMode && SteamManager.Initialized && mod.CanToUploadToWorkshop())
		{
			list.Add(new FloatMenuOption(Workshop.UploadButtonLabel(mod.GetPublishedFileId()), delegate
			{
				List<string> list2 = mod.loadFolders?.GetIssueList(mod);
				if (mod.HadIncorrectlyFormattedVersionInMetadata)
				{
					Messages.Message("MessageModNeedsWellFormattedTargetVersion".Translate(VersionControl.CurrentMajor + "." + VersionControl.CurrentMinor), MessageTypeDefOf.RejectInput, historical: false);
				}
				else if (mod.HadIncorrectlyFormattedPackageId)
				{
					Find.WindowStack.Add(new Dialog_MessageBox("MessageModNeedsWellFormattedPackageId".Translate()));
				}
				else if (!list2.NullOrEmpty())
				{
					Find.WindowStack.Add(new Dialog_MessageBox("ModHadLoadFolderIssues".Translate() + "\n" + list2.ToLineList("  - ")));
				}
				else
				{
					Find.WindowStack.Add(new Dialog_ConfirmModUpload(mod, delegate
					{
						SoundDefOf.Tick_High.PlayOneShotOnCamera();
						Dialog_MessageBox dialog_MessageBox = Dialog_MessageBox.CreateConfirmation("ConfirmContentAuthor".Translate(), delegate
						{
							SoundDefOf.Tick_High.PlayOneShotOnCamera();
							Workshop.Upload(mod);
						}, destructive: true);
						dialog_MessageBox.buttonAText = "Yes".Translate();
						dialog_MessageBox.buttonBText = "No".Translate();
						dialog_MessageBox.interactionDelay = 6f;
						Find.WindowStack.Add(dialog_MessageBox);
					}));
				}
			}));
		}
		WidgetRow widgetRow = new WidgetRow(((Rect)(ref r)).width, yMax, UIDirection.LeftThenDown, ((Rect)(ref r)).width / 2f);
		if (list.Count > 0 && widgetRow.ButtonText("MoreActions".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			Find.WindowStack.Add(new FloatMenu(list));
		}
		if (widgetRow.ButtonText(mod.Active ? "Disable".Translate() : "Enable".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			if (mod.Active)
			{
				TrySetModInactive(mod);
			}
			else
			{
				TrySetModActive(mod);
			}
			selectedMods.Clear();
			selectedMods.Add(primarySelectedMod);
		}
		yMax = widgetRow.FinalY + 25f;
		if (anyReqsCached)
		{
			Text.Anchor = (TextAnchor)5;
			TaggedString taggedString = "ModDisplayFulfilledRequirements".Translate();
			float num4 = Text.CalcSize(taggedString).x + 24f + 4f;
			Rect rect6 = new Rect(((Rect)(ref r)).width - num4, yMax, num4, 24f);
			bool flag = displayFullfilledRequirements;
			Widgets.CheckboxLabeled(rect6, taggedString, ref displayFullfilledRequirements);
			if (flag != displayFullfilledRequirements)
			{
				RecacheSelectedModInfo();
			}
			yMax += 34f;
			Text.Anchor = (TextAnchor)0;
		}
		num = Math.Max(num3, yMax) + 10f;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(0f, num, ((Rect)(ref r)).width, ((Rect)(ref r)).height - num - 30f - 10f);
		float num5 = ((Rect)(ref outRect)).width - 16f;
		float num6 = Text.CalcHeight(mod.Description, num5);
		num6 = Mathf.Min(num6 * 1.25f, num6 + 200f);
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, num5, num6 + modRequirementsHeightCached + (anyReqsInfoToShowCached ? 10f : 0f));
		float num7 = ((((Rect)(ref viewRect)).height > ((Rect)(ref outRect)).height) ? 16f : 0f);
		Widgets.BeginScrollView(outRect, ref modDescriptionScrollPosition, viewRect);
		float num8 = 0f;
		if (anyReqsInfoToShowCached)
		{
			num8 = DoRequirementSection(((Rect)(ref r)).width - num7);
			num8 += 10f;
		}
		Widgets.Label(new Rect(0f, num8, ((Rect)(ref viewRect)).width - num7, ((Rect)(ref viewRect)).height - num8), WebUtility.HtmlDecode(WebUtility.HtmlDecode(mod.Description)));
		Widgets.EndScrollView();
		Widgets.EndGroup();
	}

	private void DoDraggedMods()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (DraggedMods.NullOrEmpty())
		{
			return;
		}
		float num = 0f;
		int num2 = 0;
		Widgets.DrawWindowBackground(new Rect(0f, 0f, 250f, 26f * (float)Math.Min(DraggedMods.Count, 30)));
		Rect r = default(Rect);
		foreach (ModMetaData draggedMod in DraggedMods)
		{
			if (num2 > 30)
			{
				break;
			}
			((Rect)(ref r))._002Ector(0f, num, 250f, 26f);
			DoModRow(r, draggedMod, null, num2, isDragged: true);
			num += ((Rect)(ref r)).height;
			num2++;
		}
	}

	private void DoBottomButtons(Rect r)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		List<ModMetaData> filteredInactiveMods = ModListsInOrder().Item2;
		Widgets.BeginGroup(r);
		WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenDown);
		if (widgetRow.ButtonText("GetMods".Translate(), null, drawBackground: true, doMouseoverSound: true, active: true, 150f))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			List<FloatMenuOption> options = new List<FloatMenuOption>
			{
				new FloatMenuOption("FromWorkshop".Translate(), delegate
				{
					SteamUtility.OpenSteamWorkshopPage();
				}),
				new FloatMenuOption("FromForum".Translate(), delegate
				{
					Application.OpenURL("http://rimworldgame.com/getmods");
				})
			};
			Find.WindowStack.Add(new FloatMenu(options));
		}
		if (widgetRow.ButtonText("UnsubscribeMultiple".Translate(), null, drawBackground: true, doMouseoverSound: true, active: true, 150f))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			List<FloatMenuOption> options2 = new List<FloatMenuOption>
			{
				new FloatMenuOption("AllSelectedMods".Translate(), delegate
				{
					UnsubscribeMods(selectedMods);
				}),
				new FloatMenuOption("AllIncompatibleMods".Translate(), delegate
				{
					UnsubscribeMods(filteredInactiveMods.Where((ModMetaData mod) => !mod.Official && !mod.VersionCompatible));
				}),
				new FloatMenuOption("AllDisabledMods".Translate(), delegate
				{
					UnsubscribeMods(filteredInactiveMods.Where((ModMetaData mod) => !mod.Official));
				})
			};
			Find.WindowStack.Add(new FloatMenu(options2));
		}
		if (widgetRow.ButtonText("SaveLoadList".Translate(), null, drawBackground: true, doMouseoverSound: true, active: true, 150f))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			List<FloatMenuOption> options3 = new List<FloatMenuOption>
			{
				new FloatMenuOption("SaveModList".Translate(), delegate
				{
					SaveModList(activeModListOrderCached);
				}),
				new FloatMenuOption("LoadModList".Translate(), delegate
				{
					Find.WindowStack.Add(new Dialog_ModList_Load(delegate(ModList modList)
					{
						LoadModList(modList);
					}));
				})
			};
			Find.WindowStack.Add(new FloatMenu(options3));
		}
		Widgets.EndGroup();
	}

	private float DoRequirementSection(float width)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (visibleReqsCached.Count > 0 || anyOrderingIssuesCached)
		{
			bool num2 = anyUnfulfilledReqsCached || anyOrderingIssuesCached;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(0f, 0f, width, modRequirementsHeightCached);
			if (num2)
			{
				Widgets.DrawBoxSolid(rect, UnmetRequirementBoxBGColor);
			}
			GUI.color = (num2 ? UnmetRequirementBoxOutlineColor : RequirementBoxOutlineColor);
			Widgets.DrawBox(rect);
			GUI.color = Color.white;
			num += 10f;
			Text.Anchor = (TextAnchor)3;
			Rect entryRect = default(Rect);
			for (int i = 0; i < visibleReqsCached.Count; i++)
			{
				((Rect)(ref entryRect))._002Ector(11f, num, width - 20f - 1f, 26f);
				DrawRequirementEntry(visibleReqsCached[i], entryRect, ref num);
				if (i < visibleReqsCached.Count - 1)
				{
					num += 4f;
				}
			}
			if (anyOrderingIssuesCached)
			{
				num += 4f;
				Widgets.Label(new Rect(10f, num, width - 20f, Text.LineHeight * 2f), "ModOrderingWarning".Translate());
				num += Text.LineHeight * 2f;
			}
			num += 10f;
		}
		Text.Anchor = (TextAnchor)0;
		return num;
	}

	private void DrawRequirementEntry(ModRequirement entry, Rect entryRect, ref float y)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		Widgets.DrawBoxSolid(entryRect, entry.IsSatisfied ? RequirementRowColor : (Mouse.IsOver(entryRect) ? UnmetRequirementRowColorHighlighted : UnmetRequirementRowColor));
		Rect rect = entryRect;
		((Rect)(ref rect)).x = ((Rect)(ref rect)).x + 4f;
		((Rect)(ref rect)).width = 150f;
		Widgets.Label(rect, entry.RequirementTypeLabel.Truncate(((Rect)(ref rect)).width, truncatedStringCache));
		Rect rect2 = entryRect;
		((Rect)(ref rect2)).x = ((Rect)(ref rect)).xMax + 4f;
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - (((Rect)(ref rect2)).x + 24f + 8f);
		Text.WordWrap = false;
		Widgets.Label(rect2, entry.displayName.Truncate(((Rect)(ref rect2)).width, truncatedStringCache));
		Text.WordWrap = true;
		if (Widgets.ButtonInvisible(entryRect))
		{
			entry.OnClicked(this);
		}
		Rect val = default(Rect);
		((Rect)(ref val)).xMin = ((Rect)(ref entryRect)).xMax - 24f - 4f;
		((Rect)(ref val)).y = ((Rect)(ref entryRect)).y + 1f;
		((Rect)(ref val)).width = 24f;
		((Rect)(ref val)).height = 24f;
		GUI.DrawTexture(val, (Texture)(object)entry.StatusIcon);
		TooltipHandler.TipRegion(entryRect, new TipSignal(entry.Tooltip));
		y += 26f;
	}

	private void RecacheSelectedModInfo()
	{
		anyReqsCached = false;
		anyReqsInfoToShowCached = false;
		anyUnfulfilledReqsCached = false;
		anyOrderingIssuesCached = false;
		visibleReqsCached.Clear();
		primaryModHandle = null;
		if (primarySelectedMod == null)
		{
			return;
		}
		if (!primarySelectedMod.Official)
		{
			primaryModHandle = LoadedModManager.ModHandles.FirstOrDefault((Mod p) => primarySelectedMod.SamePackageId(p.Content.PackageId));
		}
		foreach (ModRequirement item in (from r in primarySelectedMod.GetRequirements()
			orderby r.IsSatisfied, r.RequirementTypeLabel
			select r).ToList())
		{
			bool isSatisfied = item.IsSatisfied;
			if (!isSatisfied || displayFullfilledRequirements)
			{
				visibleReqsCached.Add(item);
				if (!isSatisfied)
				{
					anyUnfulfilledReqsCached = true;
				}
			}
			anyReqsCached = true;
			anyReqsInfoToShowCached = true;
		}
		anyOrderingIssuesCached = ModsConfig.ModHasAnyOrderingIssues(primarySelectedMod);
		if (visibleReqsCached.Any() || anyOrderingIssuesCached)
		{
			anyReqsInfoToShowCached = true;
			modRequirementsHeightCached = (float)visibleReqsCached.Count * 26f + (float)(visibleReqsCached.Count - 1) * 4f + 20f + 1f;
			if (anyOrderingIssuesCached)
			{
				modRequirementsHeightCached += Text.LineHeight * 2f + 4f;
			}
		}
		else
		{
			modRequirementsHeightCached = 0f;
		}
	}

	public void SelectMod(ModMetaData mod)
	{
		primarySelectedMod = mod;
		lastSelectedIndex = GetModIndex(mod);
		lastSelectedGroupId = (mod.Active ? activeGroupId : inactiveGroupId);
		selectedMods.Clear();
		selectedMods.Add(primarySelectedMod);
		RecacheSelectedModInfo();
		pendingScrollToMod = mod;
	}

	private void DoPendingReorder()
	{
		ModMetaData placeAfter = null;
		if (pendingReorderIndex > 0)
		{
			placeAfter = filteredActiveModListOrderCached[pendingReorderIndex - 1];
		}
		IEnumerable<ModMetaData> activeModsInLoadOrder = ModsConfig.ActiveModsInLoadOrder;
		while (pendingReorders.Count > 0)
		{
			string modId = pendingReorders.Dequeue();
			int modIndex = activeModsInLoadOrder.FirstIndexOf((ModMetaData mod) => mod.PackageId == modId);
			if (pendingReorderIndex > 0 && placeAfter != null)
			{
				pendingReorderIndex = activeModsInLoadOrder.FirstIndexOf((ModMetaData m) => m.PackageId == placeAfter.PackageId) + 1;
			}
			if (filteredActiveModListOrderCached.Count() == 0)
			{
				pendingReorderIndex = activeModsInLoadOrder.Count();
			}
			if (!ModsConfig.TryReorder(modIndex, pendingReorderIndex, out var errorMessage) && !string.IsNullOrWhiteSpace(errorMessage))
			{
				Messages.Message(errorMessage, null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
	}

	private void OnDownKey(List<ModMetaData> currentList)
	{
		if (lastSelectedIndex + 1 < currentList.Count)
		{
			primarySelectedMod = currentList[lastSelectedIndex + 1];
			lastSelectedIndex++;
			if (!ShiftIsHeld)
			{
				selectedMods.Clear();
			}
			selectedMods.Add(primarySelectedMod);
			RecacheSelectedModInfo();
			pendingScrollToMod = primarySelectedMod;
		}
	}

	private void OnUpKey(List<ModMetaData> currentList)
	{
		if (lastSelectedIndex - 1 >= 0)
		{
			primarySelectedMod = currentList[lastSelectedIndex - 1];
			lastSelectedIndex--;
			if (!ShiftIsHeld)
			{
				selectedMods.Clear();
			}
			selectedMods.Add(primarySelectedMod);
			RecacheSelectedModInfo();
			pendingScrollToMod = primarySelectedMod;
		}
	}

	private void onReorderedAction(int from, int to)
	{
		List<ModMetaData> item = ModListsInOrder().Item1;
		ModMetaData item2 = item[from];
		if (!selectedMods.Contains(item2))
		{
			selectedMods.Clear();
			primarySelectedMod = item2;
			selectedMods.Add(primarySelectedMod);
			RecacheSelectedModInfo();
		}
		List<ModMetaData> list = new List<ModMetaData>();
		foreach (ModMetaData item3 in item)
		{
			if (selectedMods.Contains(item3))
			{
				list.Add(item3);
			}
		}
		list.Reverse();
		ModMetaData placeAfter = null;
		if (to > 0)
		{
			placeAfter = item[to - 1];
		}
		if (to == item.Count)
		{
			to = activeModListOrderCached.Count;
			placeAfter = null;
		}
		IEnumerable<ModMetaData> activeModsInLoadOrder = ModsConfig.ActiveModsInLoadOrder;
		foreach (ModMetaData mod in list)
		{
			from = activeModsInLoadOrder.FirstIndexOf((ModMetaData m) => m.PackageId == mod.PackageId);
			if (to > 0 && placeAfter != null)
			{
				to = activeModsInLoadOrder.FirstIndexOf((ModMetaData m) => m.PackageId == placeAfter.PackageId) + 1;
			}
			if (!ModsConfig.TryReorder(from, to, out var errorMessage) && !string.IsNullOrWhiteSpace(errorMessage))
			{
				Messages.Message(errorMessage, null, MessageTypeDefOf.RejectInput, historical: false);
			}
		}
		modListsDirty = true;
	}

	private void onMultiReorderedAction(int from, int fromGroup, int to, int toGroup)
	{
		var (list, list2) = ModListsInOrder();
		if (fromGroup == 0 && toGroup == 1)
		{
			ModMetaData modMetaData = list2[from];
			pendingReorderIndex = to;
			if (!selectedMods.Contains(modMetaData))
			{
				if (TrySetModActive(modMetaData) && !modMetaData.Official)
				{
					pendingReorders.Enqueue(modMetaData.PackageId);
				}
				selectedMods.Clear();
				primarySelectedMod = modMetaData;
				selectedMods.Add(primarySelectedMod);
				RecacheSelectedModInfo();
			}
			else
			{
				foreach (ModMetaData selectedMod in selectedMods)
				{
					if (TrySetModActive(selectedMod) && !selectedMod.Official)
					{
						pendingReorders.Enqueue(selectedMod.PackageId);
					}
				}
				primarySelectedMod = modMetaData;
				RecacheSelectedModInfo();
			}
			lastSelectedIndex = GetModIndex(primarySelectedMod);
			lastSelectedGroupId = activeGroupId;
		}
		if (fromGroup != 1 || toGroup != 0)
		{
			return;
		}
		ModMetaData modMetaData2 = list[from];
		if (!selectedMods.Contains(modMetaData2))
		{
			TrySetModInactive(modMetaData2);
		}
		else
		{
			foreach (ModMetaData selectedMod2 in selectedMods)
			{
				TrySetModInactive(selectedMod2);
			}
		}
		primarySelectedMod = modMetaData2;
		lastSelectedIndex = GetModIndex(primarySelectedMod);
		lastSelectedGroupId = inactiveGroupId;
		RecacheSelectedModInfo();
	}

	private int GetModIndex(ModMetaData mod)
	{
		if (mod == null)
		{
			return -1;
		}
		return (mod.Active ? filteredActiveModListOrderCached : inactiveModListOrderCached).IndexOf(mod);
	}

	private bool TrySetModActive(ModMetaData mod)
	{
		ModMetaData activeModWithIdentifier = ModLister.GetActiveModWithIdentifier(mod.packageIdLowerCase, ignorePostfix: true);
		if (activeModWithIdentifier != null)
		{
			Find.WindowStack.Add(new Dialog_MessageBox("MessageModWithPackageIdAlreadyEnabled".Translate(mod.PackageIdPlayerFacing, activeModWithIdentifier.Name)));
			return false;
		}
		mod.Active = true;
		modListsDirty = true;
		truncatedStringCache.Remove(mod.Name);
		return true;
	}

	private void TrySetModInactive(ModMetaData mod)
	{
		if (mod.IsCoreMod)
		{
			Find.WindowStack.Add(new Dialog_MessageBox("ConfirmDisableCoreMod".Translate(), "Confirm".Translate(), delegate
			{
				mod.Active = false;
				modListsDirty = true;
			}, "GoBack".Translate(), delegate
			{
				selectedMods.Remove(mod);
			}));
		}
		else
		{
			mod.Active = false;
			modListsDirty = true;
			truncatedStringCache.Remove(mod.Name);
		}
	}

	private void SaveModList(List<ModMetaData> activeMods)
	{
		ModList modList = new ModList();
		modList.ids = activeMods.Select((ModMetaData mod) => mod.PackageId).ToList();
		modList.names = activeMods.Select((ModMetaData mod) => mod.Name).ToList();
		Find.WindowStack.Add(new Dialog_ModList_Save(modList));
	}

	private void LoadModList(ModList modList)
	{
		selectedMods.Clear();
		selectedMods.Add(primarySelectedMod);
		List<int> list = new List<int>();
		foreach (ModMetaData allInstalledMod in ModLister.AllInstalledMods)
		{
			allInstalledMod.Active = false;
		}
		int j;
		for (j = 0; j < modList.ids.Count; j++)
		{
			ModMetaData modMetaData = ModLister.AllInstalledMods.FirstOrDefault((ModMetaData mod) => mod.PackageId == modList.ids[j]);
			if (modMetaData != null)
			{
				modMetaData.Active = true;
			}
			else
			{
				list.Add(j);
			}
		}
		IEnumerable<string> enumerable = list.Select((int i) => " - " + modList.names[i] + " (" + modList.ids[i] + ")");
		if (enumerable.Any())
		{
			Find.WindowStack.Add(new Dialog_MessageBox(string.Format("{0}:\n\n{1}", "MissingMods".Translate(), enumerable.ToLineList()), "OK".Translate()));
		}
		modListsDirty = true;
	}

	private void DiscardChanges()
	{
		selectedMods.Clear();
		foreach (ModMetaData allInstalledMod in ModLister.AllInstalledMods)
		{
			allInstalledMod.Active = false;
		}
		foreach (ModMetaData originalMod in originalModList)
		{
			originalMod.Active = true;
		}
		modListsDirty = true;
	}

	private void UnsubscribeMods(IEnumerable<ModMetaData> mods)
	{
		Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmUnsubscribeFromMultiple".Translate(mods.Select((ModMetaData mod) => mod.Name).ToLineList()), delegate
		{
			foreach (ModMetaData mod in mods)
			{
				mod.enabled = false;
				Workshop.Unsubscribe((WorkshopUploadable)mod);
			}
		}, destructive: true));
	}

	internal void Notify_SteamItemSubscribed(PublishedFileId_t pfid)
	{
		modListsDirty = true;
	}

	internal void Notify_SteamItemInstalled(PublishedFileId_t pfid)
	{
		modListsDirty = true;
	}

	internal void Notify_SteamItemUnsubscribed(PublishedFileId_t pfid)
	{
		if (primarySelectedMod != null && primarySelectedMod.FolderName == ((object)(PublishedFileId_t)(ref pfid)).ToString())
		{
			primarySelectedMod = null;
		}
		RecacheSelectedModInfo();
		modListsDirty = true;
	}
}
