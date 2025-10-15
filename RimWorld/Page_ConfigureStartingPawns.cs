using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Page_ConfigureStartingPawns : Page
{
	private int curPawnIndex;

	private bool renderClothes;

	private bool renderHeadgear;

	private int reorderableGroupID;

	private Vector2 scroll;

	private const float TabAreaWidth = 140f;

	private const float RightRectLeftPadding = 5f;

	private const float PawnEntryHeight = 60f;

	private const float SkillSummaryHeight = 127f;

	public static readonly Vector2 PawnPortraitSize = new Vector2(92f, 128f);

	private static readonly Vector2 PawnSelectorPortraitSize = new Vector2(70f, 110f);

	public override string PageTitle => "CreateCharacters".Translate();

	private bool StartingPawnsAllBabies
	{
		get
		{
			List<Pawn> startingAndOptionalPawns = Find.GameInitData.startingAndOptionalPawns;
			int num = 0;
			for (int i = 0; i < Find.GameInitData.startingPawnCount; i++)
			{
				if (startingAndOptionalPawns[i].DevelopmentalStage.Baby())
				{
					num++;
				}
			}
			return num >= Find.GameInitData.startingPawnCount;
		}
	}

	private AcceptanceReport ExtraCanDoNextReport
	{
		get
		{
			if (ModsConfig.BiotechActive && StartingPawnsAllBabies)
			{
				return "ChooseChildOrAdult".Translate();
			}
			IEnumerable<Pawn> source = Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount);
			if (source.Any((Pawn p) => (p.DevelopmentalStage & Find.GameInitData.allowedDevelopmentalStages) == 0))
			{
				return "SelectedCharactersMustBeOfAllowedDevelopmentalStages".Translate(Find.GameInitData.allowedDevelopmentalStages.ToCommaListOr());
			}
			if (!Find.GameInitData.startingSkillsRequired.NullOrEmpty())
			{
				foreach (SkillDef skill in Find.GameInitData.startingSkillsRequired)
				{
					if (!source.Any((Pawn p) => !p.skills.GetSkill(skill).TotallyDisabled))
					{
						return "SelectedCharacterMustBeCapableOf".Translate(skill.skillLabel);
					}
				}
			}
			if (!Find.GameInitData.startingPawnsRequired.NullOrEmpty())
			{
				for (int i = 0; i < Find.GameInitData.startingPawnsRequired.Count; i++)
				{
					PawnKindCount required3 = Find.GameInitData.startingPawnsRequired[i];
					int num = source.Count((Pawn p) => p.kindDef == required3.kindDef);
					if (required3.count > num)
					{
						if (required3.count <= 1 || required3.kindDef.labelPlural.NullOrEmpty())
						{
							_ = required3.kindDef.label;
						}
						else
						{
							_ = required3.kindDef.labelPlural;
						}
						return "SelectedCharactersMustInclude".Translate(required3.Summary.Named("SUMMARY"));
					}
				}
			}
			if (!Find.GameInitData.startingXenotypesRequired.NullOrEmpty())
			{
				for (int j = 0; j < Find.GameInitData.startingXenotypesRequired.Count; j++)
				{
					XenotypeCount required2 = Find.GameInitData.startingXenotypesRequired[j];
					if (source.Count((Pawn p) => p.genes.Xenotype == required2.xenotype && required2.allowedDevelopmentalStages.Has(p.DevelopmentalStage)) != required2.count)
					{
						return "SelectedCharactersMustInclude".Translate(required2.Summary.Named("SUMMARY"));
					}
				}
			}
			if (!Find.GameInitData.startingMutantsRequired.NullOrEmpty())
			{
				for (int k = 0; k < Find.GameInitData.startingMutantsRequired.Count; k++)
				{
					MutantCount required = Find.GameInitData.startingMutantsRequired[k];
					if (source.Count((Pawn p) => p.IsMutant && p.mutant.Def == required.mutant && required.allowedDevelopmentalStages.Has(p.DevelopmentalStage)) != required.count)
					{
						return "SelectedCharactersMustInclude".Translate(required.Summary.Named("SUMMARY"));
					}
				}
			}
			return true;
		}
	}

	public override void PreOpen()
	{
		base.PreOpen();
		if (Find.GameInitData.startingAndOptionalPawns.Count > 0)
		{
			curPawnIndex = 0;
		}
		renderClothes = true;
		renderHeadgear = false;
	}

	public override void PostOpen()
	{
		base.PostOpen();
		TutorSystem.Notify_Event("PageStart-ConfigureStartingPawns");
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		DrawPageTitle(rect);
		DrawApparelOptions(rect);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 45f;
		DoBottomButtons(rect, "Start".Translate(), null, null, showNext: true, doNextOnKeypress: false);
		DrawXenotypeEditorButton(rect);
		AcceptanceReport extraCanDoNextReport = ExtraCanDoNextReport;
		if (!extraCanDoNextReport.Accepted && !extraCanDoNextReport.Reason.NullOrEmpty())
		{
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).center.x + Page.BottomButSize.x / 2f + 4f, ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - Page.BottomButSize.y, Page.BottomButSize.x, Page.BottomButSize.y);
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).xMax - Page.BottomButSize.x - 4f;
			string text = ExtraCanDoNextReport.Reason.TruncateHeight(((Rect)(ref rect2)).width, ((Rect)(ref rect2)).height);
			using (new TextBlock(GameFont.Tiny, Color.red))
			{
				Widgets.Label(rect2, text);
			}
			if (ExtraCanDoNextReport.Reason != text && Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
				TooltipHandler.TipRegion(rect2, ExtraCanDoNextReport.Reason);
			}
		}
		((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 48f;
		Rect rect3 = rect;
		((Rect)(ref rect3)).width = 140f;
		DrawPawnList(rect3);
		UIHighlighter.HighlightOpportunity(rect3, "ReorderPawn");
		Rect rect4 = rect;
		((Rect)(ref rect4)).xMin = ((Rect)(ref rect4)).xMin + 140f;
		Rect rect5 = rect4.BottomPartPixels(127f);
		((Rect)(ref rect4)).yMax = ((Rect)(ref rect5)).yMin;
		rect4 = rect4.ContractedBy(4f);
		rect5 = rect5.ContractedBy(4f);
		StartingPawnUtility.DrawPortraitArea(rect4, curPawnIndex, renderClothes, renderHeadgear);
		StartingPawnUtility.DrawSkillSummaries(rect5);
	}

	private void DrawPawnList(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Invalid comparison between Unknown and I4
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - 22f;
		float num = 0f;
		if (Find.GameInitData.startingPawnCount < Find.GameInitData.startingAndOptionalPawns.Count)
		{
			num = 22f;
		}
		float num2 = (float)Find.GameInitData.startingAndOptionalPawns.Count * 60f + 22f + num;
		float num3 = ((num2 > ((Rect)(ref val)).height) ? 16f : 0f);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, 0f, ((Rect)(ref val)).width - num3, num2);
		Rect rect2 = val2;
		((Rect)(ref rect2)).height = 60f;
		rect2 = rect2.ContractedBy(4f);
		scroll = GUI.BeginScrollView(val, scroll, val2);
		if ((int)Event.current.type == 7)
		{
			reorderableGroupID = ReorderableWidget.NewGroup(delegate(int from, int to)
			{
				if (TutorSystem.AllowAction("ReorderPawn"))
				{
					Pawn item = Find.GameInitData.startingAndOptionalPawns[from];
					Find.GameInitData.startingAndOptionalPawns.Insert(to, item);
					Find.GameInitData.startingAndOptionalPawns.RemoveAt((from < to) ? from : (from + 1));
					StartingPawnUtility.ReorderRequests(from, to);
					TutorSystem.Notify_Event("ReorderPawn");
					if (to < Find.GameInitData.startingPawnCount && from >= Find.GameInitData.startingPawnCount)
					{
						TutorSystem.Notify_Event("ReorderPawnOptionalToStarting");
					}
					curPawnIndex = ((from < to) ? (to - 1) : (curPawnIndex = to));
				}
			}, ReorderableDirection.Vertical, rect, -1f, null, playSoundOnStartReorder: false);
		}
		DrawPawnListLabel(ref rect2, "StartingPawnsSelected".Translate());
		Rect rect3 = default(Rect);
		for (int i = 0; i < Find.GameInitData.startingAndOptionalPawns.Count; i++)
		{
			if (i == Find.GameInitData.startingPawnCount)
			{
				DrawPawnListLabel(ref rect2, "StartingPawnsLeftBehind".Translate());
			}
			Pawn pawn = Find.GameInitData.startingAndOptionalPawns[i];
			Widgets.BeginGroup(rect2.ExpandedBy(4f));
			((Rect)(ref rect3))._002Ector(new Vector2(4f, 4f), ((Rect)(ref rect2)).size);
			Widgets.DrawOptionBackground(rect3, curPawnIndex == i);
			MouseoverSounds.DoRegion(rect3);
			Widgets.BeginGroup(rect3);
			GUI.color = new Color(1f, 1f, 1f, 0.2f);
			Rect val3 = new Rect(110f - PawnSelectorPortraitSize.x / 2f, 40f - PawnSelectorPortraitSize.y / 2f, PawnSelectorPortraitSize.x, PawnSelectorPortraitSize.y);
			Vector2 pawnSelectorPortraitSize = PawnSelectorPortraitSize;
			Rot4 south = Rot4.South;
			bool flag = renderClothes;
			bool flag2 = renderHeadgear;
			GUI.DrawTexture(val3, (Texture)(object)PortraitsCache.Get(pawn, pawnSelectorPortraitSize, south, default(Vector3), 1f, supersample: true, compensateForUIScale: true, flag2, flag));
			GUI.color = Color.white;
			Widgets.Label(label: (!(pawn.Name is NameTriple nameTriple)) ? pawn.LabelShort : (string.IsNullOrEmpty(nameTriple.Nick) ? nameTriple.First : nameTriple.Nick), rect: rect3.TopPart(0.5f).Rounded());
			if (Text.CalcSize(pawn.story.TitleCap).x > ((Rect)(ref rect3)).width)
			{
				Widgets.Label(rect3.BottomPart(0.5f).Rounded(), pawn.story.TitleShortCap);
			}
			else
			{
				Widgets.Label(rect3.BottomPart(0.5f).Rounded(), pawn.story.TitleCap);
			}
			if ((int)Event.current.type == 0 && Mouse.IsOver(rect3))
			{
				curPawnIndex = i;
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			}
			Widgets.EndGroup();
			Widgets.EndGroup();
			if (ReorderableWidget.Reorderable(reorderableGroupID, rect2.ExpandedBy(4f)))
			{
				Widgets.DrawRectFast(rect2, Widgets.WindowBGFillColor * new Color(1f, 1f, 1f, 0.5f));
			}
			if (Mouse.IsOver(rect2))
			{
				TooltipHandler.TipRegion(rect2, new TipSignal("DragToReorder".Translate(), pawn.GetHashCode() * 3499));
			}
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 60f;
		}
		GUI.EndScrollView();
		Rect rect4 = rect;
		((Rect)(ref rect4)).yMin = ((Rect)(ref val)).yMax;
		using (new TextBlock(Color.gray))
		{
			Widgets.Label(rect4, "DragToReorder".Translate());
		}
	}

	private void DrawPawnListLabel(ref Rect rect, string label, bool isGray = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin - 4f;
		((Rect)(ref rect2)).height = 22f;
		((Rect)(ref rect)).y = ((Rect)(ref rect)).y + 22f;
		string text = label.Truncate(((Rect)(ref rect2)).width);
		TextBlock textBlock = new TextBlock(isGray ? Color.gray : Color.white);
		try
		{
			Widgets.Label(rect2, text);
		}
		finally
		{
			((IDisposable)textBlock).Dispose();
		}
		if (label != text)
		{
			TooltipHandler.TipRegion(rect2, label);
		}
	}

	private void DrawApparelOptions(Rect rect)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (ModsConfig.IdeologyActive)
		{
			string text = "ShowHeadgear".Translate();
			string text2 = "ShowApparel".Translate();
			float num = Mathf.Max(Text.CalcSize(text).x, Text.CalcSize(text2).x) + 4f + 24f;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref rect)).xMax - num, ((Rect)(ref rect)).y, num, Text.LineHeight * 2f);
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f), text, ref renderHeadgear);
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f), text2, ref renderClothes);
		}
	}

	private void DrawXenotypeEditorButton(Rect rect)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!ModsConfig.BiotechActive)
		{
			return;
		}
		Text.Font = GameFont.Small;
		float num = (((Rect)(ref rect)).width - Page.BottomButSize.x) / 2f;
		float num2 = ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - 38f;
		if (Widgets.ButtonText(new Rect(num, num2, Page.BottomButSize.x, Page.BottomButSize.y), "XenotypeEditor".Translate()))
		{
			Find.WindowStack.Add(new Dialog_CreateXenotype(curPawnIndex, delegate
			{
				CharacterCardUtility.cachedCustomXenotypes = null;
				StartingPawnUtility.RandomizePawn(curPawnIndex);
			}));
		}
	}

	protected override bool CanDoNext()
	{
		if (!base.CanDoNext())
		{
			return false;
		}
		if (TutorSystem.TutorialMode)
		{
			WorkTypeDef workTypeDef = StartingPawnUtility.RequiredWorkTypesDisabledForEveryone().FirstOrDefault();
			if (workTypeDef != null)
			{
				Messages.Message("RequiredWorkTypeDisabledForEveryone".Translate() + ": " + workTypeDef.gerundLabel.CapitalizeFirst() + ".", MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
		}
		foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
		{
			if (!startingAndOptionalPawn.Name.IsValid)
			{
				Messages.Message("EveryoneNeedsValidName".Translate(), MessageTypeDefOf.RejectInput, historical: false);
				return false;
			}
		}
		AcceptanceReport extraCanDoNextReport = ExtraCanDoNextReport;
		if (!extraCanDoNextReport.Reason.NullOrEmpty())
		{
			Messages.Message(extraCanDoNextReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		PortraitsCache.Clear();
		return true;
	}

	protected override void DoNext()
	{
		CheckWarnRequiredWorkTypesDisabledForEveryone(delegate
		{
			foreach (Pawn startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
			{
				if (startingAndOptionalPawn.Name is NameTriple nameTriple && string.IsNullOrEmpty(nameTriple.Nick))
				{
					startingAndOptionalPawn.Name = new NameTriple(nameTriple.First, nameTriple.First, nameTriple.Last);
				}
			}
			base.DoNext();
		});
	}

	private void CheckWarnRequiredWorkTypesDisabledForEveryone(Action nextAction)
	{
		IEnumerable<WorkTypeDef> enumerable = StartingPawnUtility.RequiredWorkTypesDisabledForEveryone();
		if (enumerable.Any())
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (WorkTypeDef item in enumerable)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.AppendLine();
				}
				stringBuilder.Append("  - " + item.gerundLabel.CapitalizeFirst());
			}
			TaggedString text = "ConfirmRequiredWorkTypeDisabledForEveryone".Translate(stringBuilder.ToString());
			Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(text, nextAction));
		}
		else
		{
			nextAction();
		}
	}

	public void SelectPawn(Pawn c)
	{
		int num = StartingPawnUtility.PawnIndex(c);
		if (num != -1)
		{
			curPawnIndex = num;
		}
	}
}
