using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Lesson_Note : Lesson
{
	public ConceptDef def;

	public bool doFadeIn = true;

	private float expiryTime = float.MaxValue;

	private const float RectWidth = 500f;

	private const float TextWidth = 432f;

	private const float FadeInDuration = 0.4f;

	private const float DoneButPad = 8f;

	private const float DoneButSize = 32f;

	private const float ExpiryDuration = 2.1f;

	private const float ExpiryFadeTime = 1.1f;

	public bool Expiring => expiryTime < float.MaxValue;

	public Rect MainRect
	{
		get
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			float num = Text.CalcHeight(def.HelpTextAdjusted, 432f) + 20f;
			return new Rect(Messages.MessagesTopLeftStandard.x, 0f, 500f, num);
		}
	}

	public override float MessagesYOffset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Rect mainRect = MainRect;
			return ((Rect)(ref mainRect)).height;
		}
	}

	public Lesson_Note()
	{
	}

	public Lesson_Note(ConceptDef concept)
	{
		def = concept;
	}

	public override void ExposeData()
	{
		Scribe_Defs.Look(ref def, "def");
	}

	public override void OnActivated()
	{
		base.OnActivated();
		SoundDefOf.TutorMessageAppear.PlayOneShotOnCamera();
	}

	public override void LessonOnGUI()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Rect mainRect = MainRect;
		float alpha = 1f;
		if (doFadeIn)
		{
			alpha = Mathf.Clamp01(base.AgeSeconds / 0.4f);
		}
		if (Expiring)
		{
			float num = expiryTime - Time.timeSinceLevelLoad;
			if (num < 1.1f)
			{
				alpha = num / 1.1f;
			}
		}
		Find.WindowStack.ImmediateWindow(134706, mainRect, WindowLayer.Super, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = mainRect.AtZero();
			Text.Font = GameFont.Small;
			if (!Expiring)
			{
				def.HighlightAllTags();
			}
			if (doFadeIn || Expiring)
			{
				GUI.color = new Color(1f, 1f, 1f, alpha);
			}
			Widgets.DrawWindowBackgroundTutor(rect);
			Rect rect2 = rect.ContractedBy(10f);
			((Rect)(ref rect2)).width = 432f;
			Widgets.Label(rect2, def.HelpTextAdjusted);
			Rect butRect = new Rect(((Rect)(ref rect)).xMax - 32f - 8f, ((Rect)(ref rect)).y + 8f, 32f, 32f);
			Texture2D tex = ((!Expiring) ? TexButton.CloseXBig : Widgets.CheckboxOnTex);
			if (Widgets.ButtonImage(butRect, tex, new Color(0.95f, 0.95f, 0.95f), new Color(71f / 85f, 2f / 3f, 14f / 51f)))
			{
				SoundDefOf.Click.PlayOneShotOnCamera();
				CloseButtonClicked();
			}
			if (Time.timeSinceLevelLoad > expiryTime)
			{
				CloseButtonClicked();
			}
			GUI.color = Color.white;
		}, doBackground: false, absorbInputAroundWindow: false, alpha);
	}

	private void CloseButtonClicked()
	{
		KnowledgeAmount know = (def.noteTeaches ? KnowledgeAmount.NoteTaught : KnowledgeAmount.NoteClosed);
		PlayerKnowledgeDatabase.KnowledgeDemonstrated(def, know);
		Find.ActiveLesson.Deactivate();
	}

	public override void Notify_KnowledgeDemonstrated(ConceptDef conc)
	{
		if (def == conc && PlayerKnowledgeDatabase.GetKnowledge(conc) > 0.2f && !Expiring)
		{
			expiryTime = Time.timeSinceLevelLoad + 2.1f;
		}
	}
}
