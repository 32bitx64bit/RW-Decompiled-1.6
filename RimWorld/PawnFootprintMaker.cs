using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnFootprintMaker
{
	private Pawn pawn;

	private Vector3 lastFootprintPlacePos;

	private bool lastFootprintRight;

	private const float FootprintIntervalDist = 0.632f;

	private static readonly Vector3 FootprintOffset = new Vector3(0f, 0f, -0.3f);

	private const float LeftRightOffsetDist = 0.17f;

	private const float FootprintSplashSize = 2f;

	public PawnFootprintMaker(Pawn pawn)
	{
		this.pawn = pawn;
	}

	public void ProcessPostTickVisuals(int ticksPassed)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!pawn.RaceProps.makesFootprints || (pawn.IsMutant && !pawn.mutant.Def.makesFootprints))
		{
			TerrainDef terrain = pawn.Position.GetTerrain(pawn.Map);
			if (terrain == null || !terrain.takeSplashes)
			{
				return;
			}
		}
		if ((pawn.Drawer.DrawPos - lastFootprintPlacePos).MagnitudeHorizontalSquared() > 0.39942405f)
		{
			TryPlaceFootprint();
		}
	}

	private void TryPlaceFootprint()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Vector3 drawPos = pawn.Drawer.DrawPos;
		Vector3 val = drawPos - lastFootprintPlacePos;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float rot = normalized.AngleFlat();
		float angle = (lastFootprintRight ? 90 : (-90));
		Vector3 val2 = normalized.RotatedBy(angle) * 0.17f * Mathf.Sqrt(pawn.BodySize);
		Vector3 val3 = drawPos + FootprintOffset + val2;
		IntVec3 c = val3.ToIntVec3();
		if (c.InBounds(pawn.Map))
		{
			TerrainDef terrain = c.GetTerrain(pawn.Map);
			if (terrain != null)
			{
				if (terrain.takeSplashes)
				{
					FleckMaker.WaterSplash(val3, pawn.Map, Mathf.Sqrt(pawn.BodySize) * 2f, 1.5f);
				}
				if (pawn.RaceProps.makesFootprints && terrain.takeFootprints && pawn.Map.snowGrid.GetDepth(pawn.Position) >= 0.4f)
				{
					FleckMaker.PlaceFootprint(val3, pawn.Map, rot);
				}
			}
		}
		lastFootprintPlacePos = drawPos;
		lastFootprintRight = !lastFootprintRight;
	}
}
