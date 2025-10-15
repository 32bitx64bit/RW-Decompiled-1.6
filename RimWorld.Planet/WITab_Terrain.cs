using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WITab_Terrain : WITab
{
	private Vector2 scrollPosition;

	private float lastDrawnHeight;

	private static string cachedGrowingQuadrumsDescription;

	private static PlanetTile cachedGrowingQuadrumsTile;

	private static readonly Vector2 WinSize = new Vector2(440f, 540f);

	public override bool IsVisible
	{
		get
		{
			if (base.SelPlanetTile.Valid)
			{
				return base.SelPlanetTile.Tile.OnSurface;
			}
			return false;
		}
	}

	public WITab_Terrain()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabTerrain";
		tutorTag = "Terrain";
	}

	protected override void FillTab()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		Rect outRect = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, Mathf.Max(lastDrawnHeight, ((Rect)(ref outRect)).height));
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		Rect val2 = val;
		Text.Font = GameFont.Medium;
		Widgets.Label(val2, base.SelTile.PrimaryBiome.LabelCap);
		Rect val3 = val2;
		((Rect)(ref val3)).yMin = ((Rect)(ref val3)).yMin + 35f;
		((Rect)(ref val3)).height = 99999f;
		Text.Font = GameFont.Small;
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.verticalSpacing = 0f;
		listing_Standard.Begin(val3);
		DrawScrollContents(listing_Standard, val3);
		listing_Standard.End();
		Widgets.EndScrollView();
	}

	private void DrawScrollContents(Listing_Standard listing, Rect infoRect)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Tile selTile = base.SelTile;
		PlanetTile selPlanetTile = base.SelPlanetTile;
		listing.Label(selTile.PrimaryBiome.description);
		listing.Gap(8f);
		listing.GapLine();
		if (!selTile.PrimaryBiome.implemented)
		{
			listing.Label(string.Format("{0} {1}", selTile.PrimaryBiome.LabelCap, "BiomeNotImplemented".Translate()));
		}
		ListGeometricDetails(listing, selTile, selPlanetTile);
		listing.GapLine();
		ListTemperatureDetails(listing, selPlanetTile, selTile);
		ListPollutionDetails(listing, selPlanetTile);
		listing.GapLine();
		ListMiscDetails(listing, selTile, selPlanetTile);
		lastDrawnHeight = ((Rect)(ref infoRect)).y + listing.CurHeight;
	}

	private static void ListMiscDetails(Listing_Standard listing, Tile ws, PlanetTile tile)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		listing.LabelDouble("AverageDiseaseFrequency".Translate(), string.Format("{0:F1} {1}", 60f / ws.PrimaryBiome.diseaseMtbDays, "PerYear".Translate()));
		listing.LabelDouble("TimeZone".Translate(), GenDate.TimeZoneAt(Find.WorldGrid.LongLatOf(tile).x).ToStringWithSign());
		if (Prefs.DevMode)
		{
			listing.LabelDouble("Debug world tile ID", tile.ToString());
		}
	}

	private static void ListPollutionDetails(Listing_Standard listing, PlanetTile tileID)
	{
		if (!ModsConfig.BiotechActive)
		{
			return;
		}
		listing.GapLine();
		listing.LabelDouble("TilePollution".Translate(), Find.WorldGrid[tileID].pollution.ToStringPercent(), "TerrainPollutionTip".Translate());
		string text = "";
		foreach (IGrouping<float, CurvePoint> item in from p in WorldPollutionUtility.NearbyPollutionOverDistanceCurve
			group p by p.y)
		{
			if (!text.NullOrEmpty())
			{
				text += "\n";
			}
			if (item.Count() > 1)
			{
				CurvePoint curvePoint = GenCollection.MinBy(item, (CurvePoint p) => p.x);
				CurvePoint curvePoint2 = GenCollection.MaxBy(item, (CurvePoint p) => p.x);
				text += string.Format(" - {0}-{1} {2}, {3}x {4}", curvePoint.x, curvePoint2.x, "NearbyPollutionTilesAway".Translate(), item.Key, "PollutionLower".Translate());
			}
			else
			{
				text += string.Format(" - {0} {1}, {2}x {3}", item.First().x, "NearbyPollutionTilesAway".Translate(), item.Key, "PollutionLower".Translate());
			}
		}
		TaggedString taggedString = "NearbyPollutionTip".Translate(4, text);
		float num = WorldPollutionUtility.CalculateNearbyPollutionScore(tileID);
		if (num >= GameConditionDefOf.NoxiousHaze.minNearbyPollution)
		{
			float num2 = GameConditionDefOf.NoxiousHaze.mtbOverNearbyPollutionCurve.Evaluate(num);
			taggedString += "\n\n" + "NoxiousHazeInterval".Translate(num2);
		}
		else
		{
			taggedString += "\n\n" + "NoxiousHazeNeverOccurring".Translate();
		}
		listing.LabelDouble("TilePollutionNearby".Translate(), WorldPollutionUtility.CalculateNearbyPollutionScore(tileID).ToStringByStyle(ToStringStyle.FloatTwo), taggedString);
	}

	private static void ListTemperatureDetails(Listing_Standard listing, PlanetTile tile, Tile ws)
	{
		listing.LabelDouble("AvgTemp".Translate(), GenTemperature.GetAverageTemperatureLabel(tile));
		string rightLabel = cachedGrowingQuadrumsDescription;
		if (cachedGrowingQuadrumsTile != tile)
		{
			rightLabel = (cachedGrowingQuadrumsDescription = Zone_Growing.GrowingQuadrumsDescription(tile));
			cachedGrowingQuadrumsTile = tile;
		}
		listing.LabelDouble("OutdoorGrowingPeriod".Translate(), rightLabel);
		listing.LabelDouble("Rainfall".Translate(), ws.rainfall.ToString("F0") + "mm");
		if (ws.PrimaryBiome.foragedFood != null && ws.PrimaryBiome.forageability > 0f)
		{
			listing.LabelDouble("Forageability".Translate(), ws.PrimaryBiome.forageability.ToStringPercent() + " (" + ws.PrimaryBiome.foragedFood.label + ")");
		}
		else
		{
			listing.LabelDouble("Forageability".Translate(), "0%");
		}
		listing.LabelDouble("AnimalsCanGrazeNow".Translate(), VirtualPlantsUtility.EnvironmentAllowsEatingVirtualPlantsNowAt(tile) ? "Yes".Translate() : "No".Translate());
	}

	private static void ListGeometricDetails(Listing_Standard listing, Tile ws, PlanetTile tile)
	{
		if (ws.HillinessLabel != 0)
		{
			listing.LabelDouble("Terrain".Translate(), ws.HillinessLabel.GetLabelCap());
		}
		if (ws is SurfaceTile surfaceTile)
		{
			if (surfaceTile.Roads != null)
			{
				listing.LabelDouble("Road".Translate(), surfaceTile.Roads.Select((SurfaceTile.RoadLink roadlink) => roadlink.road.label).Distinct().ToCommaList(useAnd: true)
					.CapitalizeFirst());
			}
			if (surfaceTile.Rivers != null)
			{
				listing.LabelDouble("River".Translate(), GenCollection.MaxBy(surfaceTile.Rivers, (SurfaceTile.RiverLink riverlink) => riverlink.river.degradeThreshold).river.LabelCap);
			}
		}
		if (!Find.World.Impassable(tile))
		{
			StringBuilder stringBuilder = new StringBuilder();
			PlanetTile tile2 = tile;
			StringBuilder explanation = stringBuilder;
			string rightLabel = (WorldPathGrid.CalculatedMovementDifficultyAt(tile2, perceivedStatic: false, null, explanation) * Find.WorldGrid.GetRoadMovementDifficultyMultiplier(tile, PlanetTile.Invalid, stringBuilder)).ToString("0.#");
			if (WorldPathGrid.WillWinterEverAffectMovementDifficulty(tile) && WorldPathGrid.GetCurrentWinterMovementDifficultyOffset(tile) < 2f)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.Append(" (");
				stringBuilder.Append("MovementDifficultyOffsetInWinter".Translate($"+{2f:0.#}"));
				stringBuilder.Append(")");
			}
			listing.LabelDouble("MovementDifficulty".Translate(), rightLabel, stringBuilder.ToString());
		}
		if (ws.PrimaryBiome.canBuildBase)
		{
			listing.LabelDouble("StoneTypesHere".Translate(), (from rt in Find.World.NaturalRockTypesIn(tile)
				select rt.label).ToCommaList(useAnd: true).CapitalizeFirst());
		}
		listing.LabelDouble("Elevation".Translate(), ws.Layer.Def.elevationString.Formatted(ws.elevation.ToString("F0")));
		if (ModsConfig.OdysseyActive && ws.Landmark != null)
		{
			listing.LabelDouble("Landmark".Translate(), ws.Landmark.name, ws.Landmark.def.description);
		}
		if (ws.Mutators.Any())
		{
			IOrderedEnumerable<TileMutatorDef> source = ws.Mutators.OrderBy((TileMutatorDef m) => -m.displayPriority);
			listing.LabelDouble("TileMutators".Translate(), source.Select((TileMutatorDef m) => m.Label(tile)).ToCommaList().CapitalizeFirst(), source.Select((TileMutatorDef m) => m.Label(tile).Colorize(ColoredText.TipSectionTitleColor).CapitalizeFirst() + "\n" + m.Description(tile)).ToStringList("\n\n"));
		}
	}
}
