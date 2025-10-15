using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WITab_Orbit : WITab
{
	private Vector2 scrollPosition;

	private float lastDrawnHeight;

	private static readonly Vector2 WinSize = new Vector2(440f, 540f);

	public override bool IsVisible
	{
		get
		{
			if (ModsConfig.OdysseyActive && base.SelPlanetTile.Valid)
			{
				return base.SelPlanetTile.LayerDef == PlanetLayerDefOf.Orbit;
			}
			return false;
		}
	}

	public WITab_Orbit()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabOrbit";
		tutorTag = "Orbit";
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Tile selTile = base.SelTile;
		listing.Label(selTile.PrimaryBiome.description);
		listing.Gap(8f);
		listing.GapLine();
		if (!selTile.PrimaryBiome.implemented)
		{
			listing.Label(string.Format("{0} {1}", selTile.PrimaryBiome.LabelCap, "BiomeNotImplemented".Translate()));
		}
		ListOrbitalDetails(listing, selTile, base.SelPlanetTile);
		listing.GapLine();
		ListMiscDetails(listing, selTile, base.SelPlanetTile);
		lastDrawnHeight = ((Rect)(ref infoRect)).y + listing.CurHeight;
	}

	private static void ListOrbitalDetails(Listing_Standard listing, Tile ws, PlanetTile tile)
	{
		listing.LabelDouble("Elevation".Translate(), ws.Layer.Def.elevationString.Formatted(ws.elevation.ToString("F0")));
		listing.LabelDouble("AvgTemp".Translate(), GenTemperature.GetAverageTemperatureLabel(tile));
	}

	private static void ListMiscDetails(Listing_Standard listing, Tile ws, PlanetTile tile)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		listing.LabelDouble("TimeZone".Translate(), GenDate.TimeZoneAt(Find.WorldGrid.LongLatOf(tile).x).ToStringWithSign());
		if (Prefs.DevMode)
		{
			listing.LabelDouble("Debug world tile ID", tile.ToString());
		}
	}
}
