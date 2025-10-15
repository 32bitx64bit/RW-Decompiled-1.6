using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public abstract class Lesson_Instruction : Lesson
{
	public InstructionDef def;

	private const float RectWidth = 310f;

	private const float BarHeight = 30f;

	protected Map Map => Find.AnyPlayerHomeMap;

	protected virtual float ProgressPercent => -1f;

	protected virtual bool ShowProgressBar => ProgressPercent >= 0f;

	public override string DefaultRejectInputMessage => def.rejectInputMessage;

	public override InstructionDef Instruction => def;

	public override void ExposeData()
	{
		Scribe_Defs.Look(ref def, "def");
		base.ExposeData();
	}

	public override void OnActivated()
	{
		base.OnActivated();
		if (def.giveOnActivateCount > 0)
		{
			Thing thing = ThingMaker.MakeThing(def.giveOnActivateDef);
			thing.stackCount = def.giveOnActivateCount;
			GenSpawn.Spawn(thing, TutorUtility.FindUsableRect(2, 2, Map).CenterCell, Map);
		}
		if (!def.resetBuildDesignatorStuffs)
		{
			return;
		}
		foreach (DesignationCategoryDef allDef in DefDatabase<DesignationCategoryDef>.AllDefs)
		{
			foreach (Designator resolvedAllowedDesignator in allDef.ResolvedAllowedDesignators)
			{
				if (resolvedAllowedDesignator is Designator_Build designator_Build)
				{
					designator_Build.ResetStuffToDefault();
				}
			}
		}
	}

	public override void LessonOnGUI()
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		string textAdj = def.Text.AdjustedForKeys();
		float num = Text.CalcHeight(textAdj, 290f) + 20f;
		if (ShowProgressBar)
		{
			num += 47f;
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)UI.screenWidth - 17f - 155f, 17f + num / 2f);
		if (!Find.TutorialState.introDone)
		{
			float screenOverlayAlpha = 0f;
			if (def.startCentered)
			{
				Vector2 val2 = default(Vector2);
				((Vector2)(ref val2))._002Ector((float)(UI.screenWidth / 2), (float)(UI.screenHeight / 2));
				if (base.AgeSeconds < 4f)
				{
					val = val2;
					screenOverlayAlpha = 0.9f;
				}
				else if (base.AgeSeconds < 5f)
				{
					float num2 = (base.AgeSeconds - 4f) / 1f;
					val = Vector2.Lerp(val2, val, num2);
					screenOverlayAlpha = Mathf.Lerp(0.9f, 0f, num2);
				}
			}
			if (screenOverlayAlpha > 0f)
			{
				Rect fullScreenRect = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
				Find.WindowStack.ImmediateWindow(972651, fullScreenRect, WindowLayer.SubSuper, delegate
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					GUI.color = new Color(1f, 1f, 1f, screenOverlayAlpha);
					GUI.DrawTexture(fullScreenRect, (Texture)(object)BaseContent.BlackTex);
					GUI.color = Color.white;
				}, doBackground: false, absorbInputAroundWindow: true, 0f);
			}
			else
			{
				Find.TutorialState.introDone = true;
			}
		}
		Rect mainRect = new Rect(val.x - 155f, val.y - num / 2f - 10f, 310f, num);
		if (Find.TutorialState.introDone && Find.WindowStack.IsOpen<Page_ConfigureStartingPawns>())
		{
			Rect val3 = mainRect;
			((Rect)(ref val3)).x = 17f;
			if ((((Rect)(ref mainRect)).Contains(Event.current.mousePosition) || (def == InstructionDefOf.RandomizeCharacter && UI.screenHeight <= 768)) && !((Rect)(ref val3)).Contains(Event.current.mousePosition))
			{
				((Rect)(ref mainRect)).x = 17f;
			}
		}
		Find.WindowStack.ImmediateWindow(177706, mainRect, WindowLayer.Super, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			Rect val4 = mainRect.AtZero();
			Widgets.DrawWindowBackgroundTutor(val4);
			Rect val5 = val4.ContractedBy(10f);
			Text.Font = GameFont.Small;
			Rect rect = val5;
			if (ShowProgressBar)
			{
				((Rect)(ref rect)).height = ((Rect)(ref rect)).height - 47f;
			}
			Widgets.Label(rect, textAdj);
			if (ShowProgressBar)
			{
				Widgets.FillableBar(new Rect(((Rect)(ref val5)).x, ((Rect)(ref val5)).yMax - 30f, ((Rect)(ref val5)).width, 30f), ProgressPercent, LearningReadout.ProgressBarFillTex);
			}
			if (base.AgeSeconds < 0.5f)
			{
				GUI.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, base.AgeSeconds / 0.5f));
				GUI.DrawTexture(val4, (Texture)(object)BaseContent.WhiteTex);
				GUI.color = Color.white;
			}
		}, doBackground: false);
		if (def.highlightTags != null)
		{
			for (int i = 0; i < def.highlightTags.Count; i++)
			{
				UIHighlighter.HighlightTag(def.highlightTags[i]);
			}
		}
	}

	public override void Notify_Event(EventPack ep)
	{
		if (def.eventTagsEnd != null && def.eventTagsEnd.Contains(ep.Tag))
		{
			Find.ActiveLesson.Deactivate();
		}
	}

	public override AcceptanceReport AllowAction(EventPack ep)
	{
		return def.actionTagsAllowed != null && def.actionTagsAllowed.Contains(ep.Tag);
	}

	public override void PostDeactivated()
	{
		SoundDefOf.CommsWindow_Close.PlayOneShotOnCamera();
		TutorSystem.Notify_Event("InstructionDeactivated-" + def.defName);
		if (def.endTutorial)
		{
			Find.ActiveLesson.Deactivate();
			Find.TutorialState.Notify_TutorialEnding();
			LessonAutoActivator.Notify_TutorialEnding();
		}
	}
}
