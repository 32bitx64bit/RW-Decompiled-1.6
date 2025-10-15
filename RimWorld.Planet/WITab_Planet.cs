using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WITab_Planet : WITab
{
	private static readonly Vector2 WinSize = new Vector2(400f, 150f);

	public override bool IsVisible => base.SelPlanetTile.Valid;

	private string Desc
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PlanetSeed".Translate());
			stringBuilder.Append(": ");
			stringBuilder.AppendLine(Find.World.info.seedString);
			stringBuilder.Append("PlanetCoverageShort".Translate());
			stringBuilder.Append(": ");
			stringBuilder.AppendLine(Find.World.info.planetCoverage.ToStringPercent());
			return stringBuilder.ToString();
		}
	}

	public WITab_Planet()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		size = WinSize;
		labelKey = "TabPlanet";
	}

	protected override void FillTab()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Rect val = GenUI.ContractedBy(new Rect(0f, 0f, WinSize.x, WinSize.y), 10f);
		Text.Font = GameFont.Medium;
		Widgets.Label(val, Find.World.info.name);
		Rect rect = val;
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 35f;
		Text.Font = GameFont.Small;
		Widgets.Label(rect, Desc);
	}
}
