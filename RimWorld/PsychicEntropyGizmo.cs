using System.Collections.Generic;
using LudeonTK;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class PsychicEntropyGizmo : Gizmo
{
	private Pawn_PsychicEntropyTracker tracker;

	private float selectedPsyfocusTarget = -1f;

	private static bool draggingBar;

	private float lastTargetValue;

	private float targetValue;

	private Texture2D LimitedTex;

	private Texture2D UnlimitedTex;

	private const string LimitedIconPath = "UI/Icons/EntropyLimit/Limited";

	private const string UnlimitedIconPath = "UI/Icons/EntropyLimit/Unlimited";

	public const float CostPreviewFadeIn = 0.1f;

	public const float CostPreviewSolid = 0.15f;

	public const float CostPreviewFadeInSolid = 0.25f;

	public const float CostPreviewFadeOut = 0.6f;

	private static readonly Color PainBoostColor = new Color(0.2f, 0.65f, 0.35f);

	private static readonly Texture2D EntropyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.46f, 0.34f, 0.35f));

	private static readonly Texture2D EntropyBarTexAdd = SolidColorMaterials.NewSolidColorTexture(new Color(0.78f, 0.72f, 0.66f));

	private static readonly Texture2D OverLimitBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.75f, 0.2f, 0.15f));

	private static readonly Texture2D PsyfocusBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.34f, 0.42f, 0.43f));

	private static readonly Texture2D PsyfocusBarTexReduce = SolidColorMaterials.NewSolidColorTexture(new Color(0.65f, 0.83f, 0.83f));

	private static readonly Texture2D PsyfocusBarHighlightTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.43f, 0.54f, 0.55f));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.03f, 0.035f, 0.05f));

	private static readonly Texture2D PsyfocusTargetTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.74f, 0.97f, 0.8f));

	public PsychicEntropyGizmo(Pawn_PsychicEntropyTracker tracker)
	{
		this.tracker = tracker;
		Order = -100f;
		targetValue = tracker.TargetPsyfocus;
		LimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/EntropyLimit/Limited");
		UnlimitedTex = ContentFinder<Texture2D>.Get("UI/Icons/EntropyLimit/Unlimited");
	}

	private void DrawThreshold(Rect rect, float percent, float entropyValue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + 3f + (((Rect)(ref rect)).width - 8f) * percent;
		((Rect)(ref val)).y = ((Rect)(ref rect)).y + ((Rect)(ref rect)).height - 9f;
		((Rect)(ref val)).width = 2f;
		((Rect)(ref val)).height = 6f;
		Rect val2 = val;
		if (entropyValue < percent)
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.GreyTex);
		}
		else
		{
			GUI.DrawTexture(val2, (Texture)(object)BaseContent.BlackTex);
		}
	}

	private void DrawPsyfocusTarget(Rect rect, float percent)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Round((((Rect)(ref rect)).width - 8f) * percent);
		Rect val = default(Rect);
		((Rect)(ref val)).x = ((Rect)(ref rect)).x + 3f + num;
		((Rect)(ref val)).y = ((Rect)(ref rect)).y;
		((Rect)(ref val)).width = 2f;
		((Rect)(ref val)).height = ((Rect)(ref rect)).height;
		GUI.DrawTexture(val, (Texture)(object)PsyfocusTargetTex);
		float num2 = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect)).x + 2f + num);
		float xMax = UIScaling.AdjustCoordToUIScalingCeil(num2 + 4f);
		val = default(Rect);
		((Rect)(ref val)).y = ((Rect)(ref rect)).y - 3f;
		((Rect)(ref val)).height = 5f;
		((Rect)(ref val)).xMin = num2;
		((Rect)(ref val)).xMax = xMax;
		Rect val2 = val;
		GUI.DrawTexture(val2, (Texture)(object)PsyfocusTargetTex);
		Rect val3 = val2;
		((Rect)(ref val3)).y = ((Rect)(ref rect)).yMax - 2f;
		GUI.DrawTexture(val3, (Texture)(object)PsyfocusTargetTex);
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 6f);
		Command_Psycast command_Psycast = MapGizmoUtility.LastMouseOverGizmo as Command_Psycast;
		float num = Mathf.Repeat(Time.time, 0.85f);
		float num2 = 1f;
		if (num < 0.1f)
		{
			num2 = num / 0.1f;
		}
		else if (num >= 0.25f)
		{
			num2 = 1f - (num - 0.25f) / 0.6f;
		}
		Widgets.DrawWindowBackground(rect);
		Text.Font = GameFont.Small;
		Rect rect2 = val;
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 6f;
		((Rect)(ref rect2)).height = Text.LineHeight;
		Widgets.Label(rect2, "PsychicEntropyShort".Translate());
		Rect rect3 = val;
		((Rect)(ref rect3)).y = ((Rect)(ref rect3)).y + 38f;
		((Rect)(ref rect3)).height = Text.LineHeight;
		Widgets.Label(rect3, "PsyfocusLabelGizmo".Translate());
		Rect rect4 = val;
		((Rect)(ref rect4)).x = ((Rect)(ref rect4)).x + 63f;
		((Rect)(ref rect4)).y = ((Rect)(ref rect4)).y + 6f;
		((Rect)(ref rect4)).width = 100f;
		((Rect)(ref rect4)).height = 22f;
		float entropyRelativeValue = tracker.EntropyRelativeValue;
		Widgets.FillableBar(rect4, Mathf.Min(entropyRelativeValue, 1f), EntropyBarTex, EmptyBarTex, doBorder: true);
		if (tracker.EntropyValue > tracker.MaxEntropy)
		{
			Widgets.FillableBar(rect4, Mathf.Min(entropyRelativeValue - 1f, 1f), OverLimitBarTex, EntropyBarTex, doBorder: true);
		}
		if (command_Psycast != null)
		{
			Ability ability = command_Psycast.Ability;
			if (ability.def.EntropyGain > float.Epsilon)
			{
				Rect rect5 = rect4.ContractedBy(3f);
				float width = ((Rect)(ref rect5)).width;
				float num3 = tracker.EntropyToRelativeValue(tracker.EntropyValue + ability.def.EntropyGain);
				float num4 = entropyRelativeValue;
				if (num4 > 1f)
				{
					num4 -= 1f;
					num3 -= 1f;
				}
				((Rect)(ref rect5)).xMin = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect5)).xMin + num4 * width);
				((Rect)(ref rect5)).width = UIScaling.AdjustCoordToUIScalingFloor(Mathf.Max(Mathf.Min(num3, 1f) - num4, 0f) * width);
				GUI.color = new Color(1f, 1f, 1f, num2 * 0.7f);
				GenUI.DrawTextureWithMaterial(rect5, (Texture)(object)EntropyBarTexAdd, null);
				GUI.color = Color.white;
			}
		}
		if (tracker.EntropyValue > tracker.MaxEntropy)
		{
			foreach (KeyValuePair<PsychicEntropySeverity, float> entropyThreshold in Pawn_PsychicEntropyTracker.EntropyThresholds)
			{
				if (entropyThreshold.Value > 1f && entropyThreshold.Value < 2f)
				{
					DrawThreshold(rect4, entropyThreshold.Value - 1f, entropyRelativeValue);
				}
			}
		}
		string label = tracker.EntropyValue.ToString("F0") + " / " + tracker.MaxEntropy.ToString("F0");
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect4, label);
		Text.Anchor = (TextAnchor)0;
		Text.Font = GameFont.Tiny;
		GUI.color = Color.white;
		Rect rect6 = val;
		((Rect)(ref rect6)).width = 175f;
		((Rect)(ref rect6)).height = 38f;
		TooltipHandler.TipRegion(rect6, delegate
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			float num10 = tracker.EntropyValue / tracker.RecoveryRate;
			TaggedString taggedString = ("PsychicEntropy".Translate() + ": ").Colorize(ColoredText.TipSectionTitleColor) + Mathf.Round(tracker.EntropyValue) + " / " + Mathf.Round(tracker.MaxEntropy);
			taggedString += "\n" + "PawnTooltipPsychicEntropyStats".Translate(tracker.RecoveryRate.ToString("0.#"), Mathf.Round(num10));
			return (taggedString + ("\n\n" + "PawnTooltipPsychicEntropyDesc".Translate())).Resolve();
		}, Gen.HashCombineInt(tracker.GetHashCode(), 133858));
		Rect val2 = val;
		((Rect)(ref val2)).x = ((Rect)(ref val2)).x + 63f;
		((Rect)(ref val2)).y = ((Rect)(ref val2)).y + 38f;
		((Rect)(ref val2)).width = 100f;
		((Rect)(ref val2)).height = 22f;
		lastTargetValue = targetValue;
		if (tracker.Pawn.IsColonistPlayerControlled)
		{
			Widgets.DraggableBar(val2, PsyfocusBarTex, PsyfocusBarHighlightTex, EmptyBarTex, PsyfocusTargetTex, ref draggingBar, tracker.CurrentPsyfocus, ref targetValue, Pawn_PsychicEntropyTracker.PsyfocusBandPercentages, 16);
			if (lastTargetValue != targetValue)
			{
				tracker.SetPsyfocusTarget(targetValue);
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.MeditationDesiredPsyfocus, KnowledgeAmount.Total);
			}
		}
		else
		{
			Widgets.FillableBar(val2, tracker.CurrentPsyfocus, PsyfocusBarTex, EmptyBarTex, doBorder: true);
		}
		UIHighlighter.HighlightOpportunity(val2, "PsyfocusBar");
		if (command_Psycast != null)
		{
			float min = command_Psycast.Ability.def.PsyfocusCostRange.min;
			if (min > float.Epsilon)
			{
				Rect rect7 = val2.ContractedBy(3f);
				float num5 = Mathf.Max(tracker.CurrentPsyfocus - min, 0f);
				float width2 = ((Rect)(ref rect7)).width;
				((Rect)(ref rect7)).xMin = UIScaling.AdjustCoordToUIScalingFloor(((Rect)(ref rect7)).xMin + num5 * width2);
				((Rect)(ref rect7)).width = UIScaling.AdjustCoordToUIScalingCeil((tracker.CurrentPsyfocus - num5) * width2);
				GUI.color = new Color(1f, 1f, 1f, num2);
				GenUI.DrawTextureWithMaterial(rect7, (Texture)(object)PsyfocusBarTexReduce, null);
				GUI.color = Color.white;
			}
		}
		Rect rect8 = val;
		((Rect)(ref rect8)).y = ((Rect)(ref rect8)).y + 38f;
		((Rect)(ref rect8)).width = 175f;
		((Rect)(ref rect8)).height = 38f;
		TooltipHandler.TipRegion(rect8, () => tracker.PsyfocusTipString(selectedPsyfocusTarget), Gen.HashCombineInt(tracker.GetHashCode(), 133873));
		if (tracker.Pawn.IsColonistPlayerControlled)
		{
			float num6 = 32f;
			float num7 = 4f;
			float num8 = ((Rect)(ref val)).height / 2f - num6 + num7;
			float num9 = ((Rect)(ref val)).width - num6;
			Rect val3 = new Rect(((Rect)(ref val)).x + num9, ((Rect)(ref val)).y + num8, num6, num6);
			if (Widgets.ButtonImage(val3, tracker.limitEntropyAmount ? LimitedTex : UnlimitedTex))
			{
				tracker.limitEntropyAmount = !tracker.limitEntropyAmount;
				if (tracker.limitEntropyAmount)
				{
					SoundDefOf.Tick_Low.PlayOneShotOnCamera();
				}
				else
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
				}
			}
			TooltipHandler.TipRegionByKey(val3, "PawnTooltipPsychicEntropyLimit");
		}
		if (TryGetPainMultiplier(tracker.Pawn, out var painMultiplier))
		{
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)4;
			string recoveryBonus = (painMultiplier - 1f).ToStringPercent("F0");
			string text = recoveryBonus;
			float widthCached = text.GetWidthCached();
			Rect rect9 = val;
			((Rect)(ref rect9)).x = ((Rect)(ref rect9)).x + (((Rect)(ref val)).width - widthCached / 2f - 16f);
			((Rect)(ref rect9)).y = ((Rect)(ref rect9)).y + 38f;
			((Rect)(ref rect9)).width = widthCached;
			((Rect)(ref rect9)).height = Text.LineHeight;
			GUI.color = PainBoostColor;
			Widgets.Label(rect9, text);
			GUI.color = Color.white;
			Text.Font = GameFont.Tiny;
			Text.Anchor = (TextAnchor)0;
			TooltipHandler.TipRegion(rect9.ContractedBy(-1f), () => "PawnTooltipPsychicEntropyPainFocus".Translate(tracker.Pawn.health.hediffSet.PainTotal.ToStringPercent("F0"), recoveryBonus), Gen.HashCombineInt(tracker.GetHashCode(), 133878));
		}
		return new GizmoResult(GizmoState.Clear);
	}

	private bool TryGetPainMultiplier(Pawn pawn, out float painMultiplier)
	{
		List<StatPart> parts = StatDefOf.PsychicEntropyRecoveryRate.parts;
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i] is StatPart_Pain statPart_Pain)
			{
				painMultiplier = statPart_Pain.PainFactor(tracker.Pawn);
				return true;
			}
		}
		painMultiplier = 0f;
		return false;
	}

	public override float GetWidth(float maxWidth)
	{
		return 212f;
	}
}
