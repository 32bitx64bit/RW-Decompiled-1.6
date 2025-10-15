using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_ModMismatch : Window
{
	private Action loadAction;

	private List<string> loadedModIdsList;

	public List<string> loadedModNamesList;

	private Vector2 addedModListScrollPosition = Vector2.zero;

	private Vector2 missingModListScrollPosition = Vector2.zero;

	private Vector2 sharedModListScrollPosition = Vector2.zero;

	private List<string> runningModIdsList;

	private List<string> runningModNamesList;

	private List<string> addedModsList;

	private List<string> missingModsList;

	private List<string> sharedModsList;

	private static float ButtonWidth = 200f;

	private static float ButtonHeight = 30f;

	private static float ModRowHeight = 24f;

	public override Vector2 InitialSize => new Vector2(900f, 750f);

	public Dialog_ModMismatch(Action loadAction, List<string> loadedModIdsList, List<string> loadedModNamesList)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		this.loadAction = loadAction;
		this.loadedModIdsList = loadedModIdsList.Select((string id) => id.ToLower()).ToList();
		this.loadedModNamesList = loadedModNamesList;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		List<string> source = LoadedModManager.RunningMods.Select((ModContentPack mod) => mod.PackageId).ToList();
		runningModIdsList = source.Select((string id) => ModLister.GetModWithIdentifier(id).PackageId.ToLower()).ToList();
		runningModNamesList = source.Select((string id) => ModLister.GetModWithIdentifier(id).Name).ToList();
		addedModsList = (from modId in runningModIdsList
			where !loadedModIdsList.Contains(modId)
			select runningModNamesList[runningModIdsList.IndexOf(modId)]).ToList();
		missingModsList = (from modId in loadedModIdsList
			where !runningModIdsList.Contains(modId)
			select loadedModNamesList[loadedModIdsList.IndexOf(modId)]).ToList();
		sharedModsList = (from modId in runningModIdsList
			where loadedModIdsList.Contains(modId)
			select runningModNamesList[runningModIdsList.IndexOf(modId)]).ToList();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		float num = (((Rect)(ref inRect)).width - 20f) / 3f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = num + 10f;
		float num5 = (num + 10f) * 2f;
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(0f, num2, ((Rect)(ref inRect)).width, Text.LineHeight), "ModsMismatchWarningTitle".Translate());
		num2 += Text.LineHeight + 10f;
		Text.Font = GameFont.Small;
		float num6 = Text.CalcHeight("ModsMismatchWarningText".Translate(), ((Rect)(ref inRect)).width);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, num2, ((Rect)(ref inRect)).width, num6);
		Widgets.Label(rect, "ModsMismatchWarningText".Translate());
		num2 += ((Rect)(ref rect)).height + 17f;
		if (!addedModsList.Any() && !missingModsList.Any())
		{
			float num7 = Text.CalcHeight("ModsMismatchOrderChanged".Translate(), ((Rect)(ref inRect)).width);
			Widgets.Label(new Rect(0f, num2, ((Rect)(ref inRect)).width, num7), "ModsMismatchOrderChanged".Translate());
		}
		else
		{
			float num8 = num2;
			Widgets.Label(new Rect(num3, num2, num, Text.LineHeight), "AddedModsList".Translate());
			num2 += Text.LineHeight + 10f;
			float num9 = ((Rect)(ref inRect)).height - num2 - ButtonHeight - 10f;
			DoModList(new Rect(num3, num2, num, num9), addedModsList, ref addedModListScrollPosition, (Color?)new Color(0.27f, 0.4f, 0.1f));
			num2 = num8;
			Widgets.Label(new Rect(num4, num2, num, Text.LineHeight), "MissingModsList".Translate());
			num2 += Text.LineHeight + 10f;
			DoModList(new Rect(num4, num2, num, num9), missingModsList, ref missingModListScrollPosition, (Color?)new Color(0.38f, 0.07f, 0.09f));
			num2 = num8;
			Widgets.Label(new Rect(num5, num2, num, Text.LineHeight), "SharedModsList".Translate());
			num2 += Text.LineHeight + 10f;
			DoModList(new Rect(num5, num2, num, num9), sharedModsList, ref sharedModListScrollPosition);
		}
		float num10 = ((Rect)(ref inRect)).height - ButtonHeight;
		Rect rect2 = new Rect(0f, num10, ButtonWidth, ButtonHeight);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref inRect)).width / 2f - ButtonWidth - 4f, num10, ButtonWidth, ButtonHeight);
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(((Rect)(ref inRect)).width / 2f + 4f, num10, ButtonWidth, ButtonHeight);
		Rect rect5 = default(Rect);
		((Rect)(ref rect5))._002Ector(((Rect)(ref inRect)).width - ButtonWidth, num10, ButtonWidth, ButtonHeight);
		if (Widgets.ButtonText(rect2, "GoBack".Translate()))
		{
			HandleGoBackClicked();
		}
		if (Widgets.ButtonText(rect3, "SaveModList".Translate()))
		{
			HandleSaveCurrentModList();
		}
		if (Widgets.ButtonText(rect4, "ChangeLoadedMods".Translate()))
		{
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmLoadSaveList".Translate(), delegate
			{
				HandleChangeLoadedModClicked();
			}, destructive: true));
		}
		if (Widgets.ButtonText(rect5, "LoadAnyway".Translate()))
		{
			HandleLoadAnywayClicked();
		}
	}

	private void DoModList(Rect r, List<string> modList, ref Vector2 scrollPos, Color? rowColor = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref r)).width - 16f, (float)modList.Count * ModRowHeight);
		Widgets.BeginScrollView(r, ref scrollPos, viewRect);
		Rect val = default(Rect);
		foreach (string mod in modList)
		{
			((Rect)(ref val))._002Ector(0f, (float)num * ModRowHeight, ((Rect)(ref r)).width, ModRowHeight);
			if (rowColor.HasValue)
			{
				Widgets.DrawBoxSolid(val, rowColor.Value);
			}
			DoModRow(val, mod, num);
			num++;
		}
		Widgets.EndScrollView();
	}

	private void DoModRow(Rect r, string modName, int index)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (index % 2 == 0)
		{
			Widgets.DrawLightHighlight(r);
		}
		((Rect)(ref r)).xMin = ((Rect)(ref r)).xMin + 4f;
		((Rect)(ref r)).xMax = ((Rect)(ref r)).xMax - 4f;
		Widgets.Label(r, modName);
	}

	private void HandleGoBackClicked()
	{
		SoundDefOf.Click.PlayOneShotOnCamera();
		Close();
	}

	private void HandleChangeLoadedModClicked()
	{
		SoundDefOf.Click.PlayOneShotOnCamera();
		if (Current.ProgramState == ProgramState.Entry)
		{
			ModsConfig.SetActiveToList(loadedModIdsList);
		}
		ModsConfig.SaveFromList(loadedModIdsList);
		IEnumerable<string> enumerable = from id in Enumerable.Range(0, loadedModIdsList.Count)
			where ModLister.GetModWithIdentifier(loadedModIdsList[id]) == null
			select loadedModNamesList[id];
		if (enumerable.Any())
		{
			Messages.Message(string.Format("{0}: {1}", "MissingMods".Translate(), enumerable.ToCommaList()), MessageTypeDefOf.RejectInput, historical: false);
		}
		ModsConfig.RestartFromChangedMods();
	}

	private void HandleSaveCurrentModList()
	{
		SoundDefOf.Click.PlayOneShotOnCamera();
		ModList modList = new ModList();
		modList.ids = runningModIdsList;
		modList.names = runningModNamesList;
		Find.WindowStack.Add(new Dialog_ModList_Save(modList));
	}

	private void HandleLoadAnywayClicked()
	{
		SoundDefOf.Click.PlayOneShotOnCamera();
		loadAction();
		Close();
	}

	public override void OnAcceptKeyPressed()
	{
		HandleLoadAnywayClicked();
	}
}
