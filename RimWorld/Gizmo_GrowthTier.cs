using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_GrowthTier : Gizmo
{
	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(GenUI.FillableBar_Empty);

	private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1254902f, 46f / 85f, 0f));

	private const float Spacing = 8f;

	private const float LabelWidthPercent = 0.55f;

	private const float BarMarginY = 2f;

	private const int GrowthTierTooltipId = 837825001;

	private Pawn child;

	private float Width => 190f;

	private int GrowthTier => child.ageTracker.GrowthTier;

	public override bool Visible
	{
		get
		{
			if (!child.IsColonistPlayerControlled && !child.IsPrisonerOfColony)
			{
				return child.IsSlaveOfColony;
			}
			return true;
		}
	}

	public override float GetWidth(float maxWidth)
	{
		return Width;
	}

	public Gizmo_GrowthTier(Pawn child)
	{
		this.child = child;
		Order = -100f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 8f);
		Widgets.DrawWindowBackground(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref val)).x, ((Rect)(ref rect2)).yMax, ((Rect)(ref val)).width, ((Rect)(ref rect2)).height);
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax - 2f;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
		DrawGrowthTier(rect2);
		DrawLearning(rect3);
		return new GizmoResult(GizmoState.Clear);
	}

	private string GrowthTierTooltip(int tier)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		TaggedString taggedString = ("StatsReport_GrowthTier".Translate() + ": ").AsTipTitle() + tier + "\n" + "StatsReport_GrowthTierDesc".Translate().Colorize(ColoredText.SubtleGrayColor) + "\n\n";
		if (child.ageTracker.AtMaxGrowthTier)
		{
			taggedString += ("MaxTier".Translate() + ": ").AsTipTitle() + "MaxTierDesc".Translate(child.Named("PAWN"));
		}
		else
		{
			TaggedString taggedString2 = taggedString;
			string text = ("ProgressToNextGrowthTier".Translate() + ": ").AsTipTitle();
			string text2 = Mathf.FloorToInt(child.ageTracker.growthPoints).ToString();
			float pointsRequirement = GrowthUtility.GrowthTiers[tier + 1].pointsRequirement;
			taggedString = taggedString2 + (text + text2 + " / " + pointsRequirement);
			if (child.ageTracker.canGainGrowthPoints)
			{
				taggedString += string.Format(" (+{0})", "PerDay".Translate(child.ageTracker.GrowthPointsPerDay.ToStringByStyle(ToStringStyle.FloatMaxTwo)));
			}
		}
		if (child.ageTracker.AgeBiologicalYears < 13)
		{
			int num = 0;
			for (int i = child.ageTracker.AgeBiologicalYears + 1; i <= 13; i++)
			{
				if (GrowthUtility.IsGrowthBirthday(i))
				{
					num = i;
					break;
				}
			}
			taggedString += "\n\n" + ("NextGrowthMomentAt".Translate() + ": ").AsTipTitle() + num;
		}
		GrowthUtility.GrowthTier growthTier = GrowthUtility.GrowthTiers[tier];
		taggedString += "\n\n" + ("ThisGrowthTier".Translate(tier) + ":").AsTipTitle();
		if (growthTier.passionGainsRange.TrueMax > 0)
		{
			taggedString += "\n  - " + "NumPassionsFromOptions".Translate(growthTier.passionGainsRange.ToString(), growthTier.passionChoices);
		}
		taggedString += "\n  - " + "NumTraitsFromOptions".Translate(growthTier.traitGains, growthTier.traitChoices);
		if (!child.ageTracker.AtMaxGrowthTier)
		{
			GrowthUtility.GrowthTier growthTier2 = GrowthUtility.GrowthTiers[tier + 1];
			taggedString += "\n\n" + ("NextGrowthTier".Translate(tier + 1) + ":").AsTipTitle();
			if (growthTier2.passionGainsRange.TrueMax > 0)
			{
				taggedString += "\n  - " + "NumPassionsFromOptions".Translate(growthTier2.passionGainsRange.ToString(), growthTier2.passionChoices);
			}
			taggedString += "\n  - " + "NumTraitsFromOptions".Translate(growthTier2.traitGains, growthTier2.traitChoices);
		}
		return taggedString.Resolve();
	}

	private void DrawGrowthTier(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		int growthTier = GrowthTier;
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width * 0.55f;
		string label = string.Concat("StatsReport_GrowthTier".Translate() + ": ", growthTier.ToString());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, label);
		Text.Anchor = (TextAnchor)0;
		float percentToNextGrowthTier = child.ageTracker.PercentToNextGrowthTier;
		Rect rect3 = rect;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - 2f;
		Widgets.FillableBar(rect3, percentToNextGrowthTier, BarTex, EmptyBarTex, doBorder: true);
		Text.Anchor = (TextAnchor)4;
		float pointsRequirement = GrowthUtility.GrowthTiers[GrowthUtility.GrowthTiers.Length - 1].pointsRequirement;
		string text2;
		if (!child.ageTracker.AtMaxGrowthTier)
		{
			string text = Mathf.FloorToInt(child.ageTracker.growthPoints).ToString();
			float pointsRequirement2 = GrowthUtility.GrowthTiers[growthTier + 1].pointsRequirement;
			text2 = text + " / " + pointsRequirement2;
		}
		else
		{
			text2 = pointsRequirement + " / " + pointsRequirement;
		}
		string label2 = text2;
		Widgets.Label(rect3, label2);
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			TooltipHandler.TipRegion(rect, new TipSignal(GrowthTierTooltip(growthTier), child.thingIDNumber ^ 0x31F031E9));
		}
	}

	private void DrawLearning(Rect rect)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (child.needs.learning != null)
		{
			Rect rect2 = rect;
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width * 0.55f;
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, NeedDefOf.Learning.LabelCap);
			Text.Anchor = (TextAnchor)0;
			Rect rect3 = rect;
			((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
			((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - 2f;
			Widgets.FillableBar(rect3, child.needs.learning.CurLevelPercentage, Widgets.BarFullTexHor, EmptyBarTex, doBorder: true);
			Text.Anchor = (TextAnchor)4;
			string label = child.needs.learning.CurLevelPercentage.ToStringPercent();
			Widgets.Label(rect3, label);
			Text.Anchor = (TextAnchor)0;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, child.needs.learning.GetTipString());
			}
		}
	}
}
