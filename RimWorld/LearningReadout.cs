using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class LearningReadout : IExposable
{
	private List<ConceptDef> activeConcepts = new List<ConceptDef>();

	private ConceptDef selectedConcept;

	private bool showAllMode;

	private float contentHeight;

	private Vector2 scrollPosition = Vector2.zero;

	private string searchString = "";

	private float lastConceptActivateRealTime = -999f;

	private ConceptDef mouseoverConcept;

	private Rect windowRect;

	private Action windowOnGUICached;

	private const float OuterMargin = 8f;

	private const float InnerMargin = 7f;

	private const float ReadoutWidth = 200f;

	private const float InfoPaneWidth = 310f;

	private const float OpenButtonSize = 24f;

	public static readonly Texture2D ProgressBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(38f / 51f, 0.6039216f, 0.2f));

	public static readonly Texture2D ProgressBarBGTex = SolidColorMaterials.NewSolidColorTexture(new Color(26f / 51f, 0.40784314f, 2f / 15f));

	private static List<ConceptDef> tmpConceptsToShow = new List<ConceptDef>();

	public int ActiveConceptsCount => activeConcepts.Count;

	public bool ShowAllMode => showAllMode;

	public LearningReadout()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		windowOnGUICached = WindowOnGUI;
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref activeConcepts, "activeConcepts", LookMode.Undefined);
		Scribe_Defs.Look(ref selectedConcept, "selectedConcept");
		if (Scribe.mode == LoadSaveMode.PostLoadInit)
		{
			activeConcepts.RemoveAll((ConceptDef c) => PlayerKnowledgeDatabase.IsComplete(c));
		}
	}

	public bool TryActivateConcept(ConceptDef conc)
	{
		if (activeConcepts.Contains(conc))
		{
			return false;
		}
		activeConcepts.Add(conc);
		SoundDefOf.Lesson_Activated.PlayOneShotOnCamera();
		lastConceptActivateRealTime = RealTime.LastRealTime;
		return true;
	}

	public bool IsActive(ConceptDef conc)
	{
		return activeConcepts.Contains(conc);
	}

	public void LearningReadoutUpdate()
	{
	}

	public void Notify_ConceptNewlyLearned(ConceptDef conc)
	{
		if (activeConcepts.Contains(conc) || selectedConcept == conc)
		{
			SoundDefOf.Lesson_Deactivated.PlayOneShotOnCamera();
			SoundDefOf.CommsWindow_Close.PlayOneShotOnCamera();
		}
		if (activeConcepts.Contains(conc))
		{
			activeConcepts.Remove(conc);
		}
		if (selectedConcept == conc)
		{
			selectedConcept = null;
		}
	}

	private string FilterSearchStringInput(string input)
	{
		if (input == searchString)
		{
			return input;
		}
		if (input.Length > 20)
		{
			input = input.Substring(0, 20);
		}
		return input;
	}

	public void LearningReadoutOnGUI()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!TutorSystem.TutorialMode && TutorSystem.AdaptiveTrainingEnabled && (Find.PlaySettings.showLearningHelper || activeConcepts.Count != 0) && !Find.WindowStack.IsOpen<Screen_Credits>())
		{
			float num = (float)UI.screenHeight / 2f;
			float num2 = contentHeight + 14f;
			windowRect = new Rect((float)UI.screenWidth - 8f - 200f, 8f, 200f, Mathf.Min(num2, num));
			Rect val = windowRect;
			Find.WindowStack.ImmediateWindow(76136312, windowRect, WindowLayer.Super, windowOnGUICached, doBackground: false);
			float num3 = Time.realtimeSinceStartup - lastConceptActivateRealTime;
			if (num3 < 1f && num3 > 0f)
			{
				GenUI.DrawFlash(((Rect)(ref val)).x, ((Rect)(ref val)).center.y, (float)UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, num3) * 0.85f, new Color(0.8f, 0.77f, 0.53f));
			}
			ConceptDef conceptDef = ((selectedConcept != null) ? selectedConcept : mouseoverConcept);
			if (conceptDef != null)
			{
				DrawInfoPane(conceptDef);
				conceptDef.HighlightAllTags();
			}
			mouseoverConcept = null;
		}
	}

	private void WindowOnGUI()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		Rect val = windowRect.AtZero().ContractedBy(7f);
		bool flag = contentHeight > ((Rect)(ref val)).height;
		Widgets.DrawWindowBackgroundTutor(windowRect.AtZero());
		float y = ((Rect)(ref val)).y;
		Text.Font = GameFont.Small;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(((Rect)(ref val)).x, y, ((Rect)(ref val)).width - 24f, 24f);
		Widgets.Label(rect, "LearningHelper".Translate());
		y += ((Rect)(ref rect)).height;
		if (Widgets.ButtonImage(new Rect(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).y, 24f, 24f), (!showAllMode) ? TexButton.Plus : TexButton.Minus))
		{
			showAllMode = !showAllMode;
			if (showAllMode)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.Tick_Low.PlayOneShotOnCamera();
			}
		}
		if (showAllMode)
		{
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(((Rect)(ref val)).x, y, ((Rect)(ref val)).width - 20f - 2f, 28f);
			searchString = FilterSearchStringInput(Widgets.TextField(val2, searchString));
			if (searchString == "")
			{
				GUI.color = new Color(0.6f, 0.6f, 0.6f, 1f);
				Text.Anchor = (TextAnchor)3;
				Rect rect2 = val2;
				((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + 7f;
				Widgets.Label(rect2, "Filter".Translate() + "...");
				Text.Anchor = (TextAnchor)0;
				GUI.color = Color.white;
			}
			if (Widgets.ButtonImage(new Rect(((Rect)(ref val2)).xMax + 4f, y + 14f - 10f, 20f, 20f), TexButton.CloseXSmall))
			{
				searchString = "";
				SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
			}
			y += 32f;
		}
		tmpConceptsToShow.Clear();
		if (showAllMode)
		{
			tmpConceptsToShow.AddRange(DefDatabase<ConceptDef>.AllDefsListForReading);
		}
		else
		{
			tmpConceptsToShow.AddRange(activeConcepts);
		}
		if (tmpConceptsToShow.Any())
		{
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			Widgets.DrawLineHorizontal(((Rect)(ref val)).x, y, ((Rect)(ref val)).width);
			GUI.color = Color.white;
			y += 4f;
		}
		float num = y - ((Rect)(ref val)).y;
		((Rect)(ref val)).yMin = y;
		Rect viewRect = val.AtZero();
		if (flag)
		{
			((Rect)(ref viewRect)).height = contentHeight - num;
			((Rect)(ref viewRect)).width = ((Rect)(ref viewRect)).width - 20f;
			Widgets.BeginScrollView(val, ref scrollPosition, viewRect);
		}
		else
		{
			Widgets.BeginGroup(val);
		}
		y = 0f;
		if (showAllMode)
		{
			tmpConceptsToShow.SortBy((ConceptDef x) => -DisplayPriority(x), (ConceptDef x) => x.label);
		}
		for (int i = 0; i < tmpConceptsToShow.Count; i++)
		{
			if (!tmpConceptsToShow[i].TriggeredDirect)
			{
				Rect val3 = DrawConceptListRow(0f, y, ((Rect)(ref viewRect)).width, tmpConceptsToShow[i]);
				y = ((Rect)(ref val3)).yMax;
			}
		}
		tmpConceptsToShow.Clear();
		contentHeight = num + y;
		if (flag)
		{
			Widgets.EndScrollView();
		}
		else
		{
			Widgets.EndGroup();
		}
	}

	private int DisplayPriority(ConceptDef conc)
	{
		int num = 1;
		if (MatchesSearchString(conc))
		{
			num += 10000;
		}
		return num;
	}

	private bool MatchesSearchString(ConceptDef conc)
	{
		if (searchString != "")
		{
			return conc.label.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return false;
	}

	private Rect DrawConceptListRow(float x, float y, float width, ConceptDef conc)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		float knowledge = PlayerKnowledgeDatabase.GetKnowledge(conc);
		bool num = PlayerKnowledgeDatabase.IsComplete(conc);
		bool num2 = !num && knowledge > 0f;
		float num3 = Text.CalcHeight(conc.LabelCap, width);
		if (num2)
		{
			num3 += 0f;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(x, y, width, num3);
		if (num2)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(val);
			((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 1f;
			((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - 1f;
			Widgets.FillableBar(rect, PlayerKnowledgeDatabase.GetKnowledge(conc), ProgressBarFillTex, ProgressBarBGTex, doBorder: false);
		}
		if (num)
		{
			GUI.DrawTexture(val, (Texture)(object)BaseContent.GreyTex);
		}
		if (selectedConcept == conc)
		{
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightSelectedTex);
		}
		Widgets.DrawHighlightIfMouseover(val);
		if (MatchesSearchString(conc))
		{
			Widgets.DrawHighlight(val);
		}
		Widgets.Label(val, conc.LabelCap);
		if (Mouse.IsOver(val) && selectedConcept == null)
		{
			mouseoverConcept = conc;
		}
		if (Widgets.ButtonInvisible(val))
		{
			if (selectedConcept == conc)
			{
				selectedConcept = null;
			}
			else
			{
				selectedConcept = conc;
			}
			SoundDefOf.PageChange.PlayOneShotOnCamera();
		}
		return val;
	}

	private Rect DrawInfoPane(ConceptDef conc)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		float knowledge = PlayerKnowledgeDatabase.GetKnowledge(conc);
		bool complete = PlayerKnowledgeDatabase.IsComplete(conc);
		bool drawProgressBar = !complete && knowledge > 0f;
		Text.Font = GameFont.Medium;
		float titleHeight = Text.CalcHeight(conc.LabelCap, 276f);
		Text.Font = GameFont.Small;
		float textHeight = Text.CalcHeight(conc.HelpTextAdjusted, 296f);
		float num = titleHeight + textHeight + 14f + 5f;
		if (selectedConcept == conc)
		{
			num += 40f;
		}
		if (drawProgressBar)
		{
			num += 30f;
		}
		Rect outRect = new Rect((float)UI.screenWidth - 8f - 200f - 8f - 310f, 8f, 310f, num);
		Rect result = outRect;
		Find.WindowStack.ImmediateWindow(987612111, outRect, WindowLayer.Super, delegate
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			outRect = outRect.AtZero();
			Rect val = outRect.ContractedBy(7f);
			Widgets.DrawShadowAround(outRect);
			Widgets.DrawWindowBackgroundTutor(outRect);
			Rect rect = val;
			((Rect)(ref rect)).width = ((Rect)(ref rect)).width - 20f;
			((Rect)(ref rect)).height = titleHeight + 5f;
			Text.Font = GameFont.Medium;
			Widgets.Label(rect, conc.LabelCap);
			Text.Font = GameFont.Small;
			Rect rect2 = val;
			((Rect)(ref rect2)).yMin = ((Rect)(ref rect)).yMax;
			((Rect)(ref rect2)).height = textHeight;
			Widgets.Label(rect2, conc.HelpTextAdjusted);
			if (drawProgressBar)
			{
				Rect rect3 = val;
				((Rect)(ref rect3)).yMin = ((Rect)(ref rect2)).yMax;
				((Rect)(ref rect3)).height = 30f;
				Widgets.FillableBar(rect3, PlayerKnowledgeDatabase.GetKnowledge(conc), ProgressBarFillTex);
			}
			if (selectedConcept == conc)
			{
				if (Widgets.CloseButtonFor(outRect))
				{
					selectedConcept = null;
					SoundDefOf.PageChange.PlayOneShotOnCamera();
				}
				Rect rect4 = default(Rect);
				((Rect)(ref rect4))._002Ector(((Rect)(ref val)).center.x - 70f, ((Rect)(ref val)).yMax - 30f, 140f, 30f);
				if (!complete)
				{
					if (Widgets.ButtonText(rect4, "MarkLearned".Translate()))
					{
						selectedConcept = null;
						SoundDefOf.PageChange.PlayOneShotOnCamera();
						PlayerKnowledgeDatabase.SetKnowledge(conc, 1f);
					}
				}
				else
				{
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
					Text.Anchor = (TextAnchor)4;
					Widgets.Label(rect4, "AlreadyLearned".Translate());
					Text.Anchor = (TextAnchor)0;
					GUI.color = Color.white;
				}
			}
		}, doBackground: false);
		return result;
	}
}
