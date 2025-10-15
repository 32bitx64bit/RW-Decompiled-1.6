using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_FileList : Window
{
	protected string interactButLabel = "Error";

	protected float bottomAreaHeight;

	protected readonly List<SaveFileInfo> files = new List<SaveFileInfo>();

	private readonly QuickSearchWidget search = new QuickSearchWidget();

	private bool focusedSearch;

	private bool focusedNameArea;

	private Vector2 scrollPosition = Vector2.zero;

	protected string typingName = "";

	protected string deleteTipKey = "DeleteThisSavegame";

	protected const float EntryHeight = 40f;

	protected const float FileNameLeftMargin = 8f;

	protected const float FileNameRightMargin = 4f;

	protected const float FileInfoWidth = 94f;

	protected const float InteractButWidth = 100f;

	protected const float InteractButHeight = 36f;

	protected const float DeleteButSize = 36f;

	protected const float NameTextFieldWidth = 400f;

	protected const float NameTextFieldHeight = 35f;

	protected const float NameTextFieldButtonSpace = 20f;

	protected static readonly Color DefaultFileTextColor = new Color(1f, 1f, 0.6f);

	public override Vector2 InitialSize => new Vector2(620f, 700f);

	protected virtual bool ShouldDoTypeInField => false;

	protected virtual bool FocusSearchField => false;

	public Dialog_FileList()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		doCloseButton = true;
		doCloseX = true;
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		ReloadFiles();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref inRect)).width - 16f, 40f);
		float y = val.y;
		float num = (float)FilesMatchingFilter() * y;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, num);
		Rect rect = inRect.LeftHalf();
		((Rect)(ref rect)).height = 24f;
		search.OnGUI(rect);
		if (!focusedSearch && FocusSearchField)
		{
			focusedSearch = true;
			search.Focus();
		}
		Rect outRect = inRect;
		((Rect)(ref outRect)).yMin = ((Rect)(ref rect)).yMax + 10f;
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - (Window.CloseButSize.y + bottomAreaHeight + 10f);
		if (ShouldDoTypeInField)
		{
			((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - 53f;
		}
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num2 = 0f;
		int num3 = 0;
		Rect rect2 = default(Rect);
		Rect val2 = default(Rect);
		Rect rect3 = default(Rect);
		Rect rect4 = default(Rect);
		Rect rect5 = default(Rect);
		foreach (SaveFileInfo file in files)
		{
			if (!search.filter.Matches(file.FileName))
			{
				continue;
			}
			if (num2 + val.y >= scrollPosition.y && num2 <= scrollPosition.y + ((Rect)(ref outRect)).height)
			{
				((Rect)(ref rect2))._002Ector(0f, num2, val.x, val.y);
				if (num3 % 2 == 1)
				{
					Widgets.DrawAltRect(rect2);
				}
				Widgets.BeginGroup(rect2);
				((Rect)(ref val2))._002Ector(((Rect)(ref rect2)).width - 36f, (((Rect)(ref rect2)).height - 36f) / 2f, 36f, 36f);
				if (Widgets.ButtonImage(val2, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
				{
					FileInfo localFile = file.FileInfo;
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(localFile.Name), delegate
					{
						localFile.Delete();
						ReloadFiles();
					}, destructive: true));
				}
				TooltipHandler.TipRegionByKey(val2, deleteTipKey);
				Text.Font = GameFont.Small;
				((Rect)(ref rect3))._002Ector(((Rect)(ref val2)).x - 100f, (((Rect)(ref rect2)).height - 36f) / 2f, 100f, 36f);
				if (Widgets.ButtonText(rect3, interactButLabel))
				{
					DoFileInteraction(Path.GetFileNameWithoutExtension(file.FileName));
				}
				((Rect)(ref rect4))._002Ector(((Rect)(ref rect3)).x - 94f, 0f, 94f, ((Rect)(ref rect2)).height);
				DrawDateAndVersion(file, rect4);
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				GUI.color = FileNameColor(file);
				((Rect)(ref rect5))._002Ector(8f, 0f, ((Rect)(ref rect4)).x - 8f - 4f, ((Rect)(ref rect2)).height);
				Text.Anchor = (TextAnchor)3;
				Text.Font = GameFont.Small;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
				Widgets.Label(rect5, fileNameWithoutExtension.Truncate(((Rect)(ref rect5)).width * 1.8f));
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				Widgets.EndGroup();
			}
			num2 += val.y;
			num3++;
		}
		Widgets.EndScrollView();
		if (ShouldDoTypeInField)
		{
			DoTypeInField(inRect.TopPartPixels(((Rect)(ref inRect)).height - Window.CloseButSize.y - 18f));
		}
	}

	private int FilesMatchingFilter()
	{
		int num = 0;
		for (int i = 0; i < files.Count; i++)
		{
			if (search.filter.Matches(files[i].FileName))
			{
				num++;
			}
		}
		return num;
	}

	protected abstract void DoFileInteraction(string fileName);

	protected abstract void ReloadFiles();

	protected virtual void DoTypeInField(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		bool flag = (int)Event.current.type == 4 && (int)Event.current.keyCode == 13;
		float num = ((Rect)(ref rect)).height - 35f;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		GUI.SetNextControlName("MapNameField");
		string str = Widgets.TextField(new Rect(5f, num, 400f, 35f), typingName);
		if (GenText.IsValidFilename(str))
		{
			typingName = str;
		}
		if (!focusedNameArea)
		{
			UI.FocusControl("MapNameField", this);
			focusedNameArea = true;
		}
		if (Widgets.ButtonText(new Rect(420f, num, ((Rect)(ref rect)).width - 400f - 20f, 35f), "SaveGameButton".Translate()) || flag)
		{
			if (typingName.NullOrEmpty())
			{
				Messages.Message("NeedAName".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				DoFileInteraction(typingName?.Trim());
			}
		}
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
	}

	protected virtual Color FileNameColor(SaveFileInfo sfi)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return DefaultFileTextColor;
	}

	public static void DrawDateAndVersion(SaveFileInfo sfi, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 2f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
		GUI.color = SaveFileInfo.UnimportantTextColor;
		Widgets.Label(rect2, sfi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, ((Rect)(ref rect2)).yMax, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
		GUI.color = sfi.VersionColor;
		Widgets.Label(rect3, sfi.GameVersion);
		if (Mouse.IsOver(rect3))
		{
			TooltipHandler.TipRegion(rect3, sfi.CompatibilityTip);
		}
		Widgets.EndGroup();
	}
}
