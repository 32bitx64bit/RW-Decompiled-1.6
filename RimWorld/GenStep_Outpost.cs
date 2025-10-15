using System.Collections.Generic;
using System.Linq;
using RimWorld.BaseGen;
using Verse;

namespace RimWorld;

public class GenStep_Outpost : GenStep
{
	public int size = 16;

	public int requiredWorshippedTerminalRooms;

	public int requiredGravcoreRooms;

	public bool allowGeneratingThronerooms = true;

	public bool settlementDontGeneratePawns;

	public bool allowGeneratingFarms = true;

	public bool generateLoot = true;

	public MapGenUtility.PostProcessSettlementParams postProcessSettlementParams;

	public bool unfogged;

	public bool attackWhenPlayerBecameEnemy;

	public FloatRange defaultPawnGroupPointsRange = SymbolResolver_Settlement.DefaultPawnsPoints;

	public PawnGroupKindDef pawnGroupKindDef;

	public CellRect? forcedRect;

	public Faction overrideFaction;

	private const float MaxWaterOverlap = 0.1f;

	private static readonly List<CellRect> possibleRects = new List<CellRect>();

	private bool WillPostProcess => postProcessSettlementParams != null;

	public override int SeedPart => 398638181;

	public override void Generate(Map map, GenStepParams parms)
	{
		List<CellRect> usedRects = MapGenerator.GetOrGenerateVar<List<CellRect>>("UsedRects");
		if (!MapGenerator.TryGetVar<CellRect>("RectOfInterest", out var var) && !MapGenUtility.TryGetClosestClearRectTo(out var, new IntVec2(size, size), map.Center, Validator) && !MapGenUtility.TryGetRandomClearRect(size, size, out var, -1, -1, Validator))
		{
			Log.Error("Failed to find location for outpost");
			return;
		}
		Faction faction = ((overrideFaction != null) ? overrideFaction : ((map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction()));
		ResolveParams resolveParams = default(ResolveParams);
		resolveParams.rect = forcedRect ?? GetOutpostRect(var, usedRects, map);
		resolveParams.faction = faction;
		resolveParams.edgeDefenseWidth = 2;
		resolveParams.edgeDefenseTurretsCount = Rand.RangeInclusive(0, 1);
		resolveParams.edgeDefenseMortarsCount = 0;
		resolveParams.settlementDontGeneratePawns = settlementDontGeneratePawns;
		resolveParams.attackWhenPlayerBecameEnemy = attackWhenPlayerBecameEnemy;
		resolveParams.pawnGroupKindDef = pawnGroupKindDef;
		ResolveParams resolveParams2 = resolveParams;
		if (parms.sitePart != null)
		{
			resolveParams2.bedCount = ((parms.sitePart.expectedEnemyCount == -1) ? null : new int?(parms.sitePart.expectedEnemyCount));
			resolveParams2.sitePart = parms.sitePart;
			resolveParams2.settlementPawnGroupPoints = parms.sitePart.parms.threatPoints;
			resolveParams2.settlementPawnGroupSeed = OutpostSitePartUtility.GetPawnGroupMakerSeed(parms.sitePart.parms);
		}
		else
		{
			resolveParams2.settlementPawnGroupPoints = defaultPawnGroupPointsRange.RandomInRange;
		}
		resolveParams2.allowGeneratingThronerooms = allowGeneratingThronerooms;
		if (generateLoot)
		{
			if (parms.sitePart != null)
			{
				resolveParams2.lootMarketValue = parms.sitePart.parms.lootMarketValue;
			}
			else
			{
				resolveParams2.lootMarketValue = null;
			}
		}
		else
		{
			resolveParams2.lootMarketValue = 0f;
		}
		RimWorld.BaseGen.BaseGen.globalSettings.map = map;
		RimWorld.BaseGen.BaseGen.globalSettings.minBuildings = requiredWorshippedTerminalRooms + requiredGravcoreRooms + 1;
		RimWorld.BaseGen.BaseGen.globalSettings.minBarracks = 1;
		RimWorld.BaseGen.BaseGen.globalSettings.requiredWorshippedTerminalRooms = requiredWorshippedTerminalRooms;
		RimWorld.BaseGen.BaseGen.globalSettings.requiredGravcoreRooms = requiredGravcoreRooms;
		RimWorld.BaseGen.BaseGen.globalSettings.maxFarms = (allowGeneratingFarms ? (-1) : 0);
		RimWorld.BaseGen.BaseGen.symbolStack.Push("settlement", resolveParams2);
		if (faction != null && faction == Faction.OfEmpire)
		{
			RimWorld.BaseGen.BaseGen.globalSettings.minThroneRooms = (allowGeneratingThronerooms ? 1 : 0);
			RimWorld.BaseGen.BaseGen.globalSettings.minLandingPads = 1;
		}
		List<Building> previous = null;
		if (WillPostProcess)
		{
			previous = new List<Building>(map.listerThings.GetThingsOfType<Building>());
		}
		RimWorld.BaseGen.BaseGen.Generate();
		if (faction != null && faction == Faction.OfEmpire && RimWorld.BaseGen.BaseGen.globalSettings.landingPadsGenerated == 0)
		{
			GenStep_Settlement.GenerateLandingPadNearby(resolveParams2.rect, map, faction, out var usedRect);
			usedRects.Add(usedRect);
		}
		if (WillPostProcess)
		{
			List<Building> placed = (from b in map.listerThings.GetThingsOfType<Building>()
				where !previous.Contains(b)
				select b).ToList();
			previous.Clear();
			MapGenUtility.PostProcessSettlement(map, placed, postProcessSettlementParams);
		}
		if (unfogged)
		{
			foreach (IntVec3 item in resolveParams2.rect)
			{
				MapGenerator.rootsToUnfog.Add(item);
			}
		}
		usedRects.Add(resolveParams2.rect);
		bool Validator(CellRect r)
		{
			if ((float)r.Cells.Count((IntVec3 c) => c.GetTerrain(map).IsWater) > (float)(size * size) * 0.1f)
			{
				return false;
			}
			if (usedRects.Any((CellRect ur) => ur.Overlaps(r)))
			{
				return false;
			}
			if (!r.CenterCell.InHorDistOf(map.Center, (float)map.Size.x * 0.75f))
			{
				return false;
			}
			return true;
		}
	}

	private CellRect GetOutpostRect(CellRect rectToDefend, List<CellRect> usedRects, Map map)
	{
		possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size, rectToDefend.CenterCell.z - size / 2, size, size));
		possibleRects.Add(new CellRect(rectToDefend.maxX + 1, rectToDefend.CenterCell.z - size / 2, size, size));
		possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size, size, size));
		possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1, size, size));
		CellRect mapRect = new CellRect(0, 0, map.Size.x, map.Size.z);
		possibleRects.RemoveAll((CellRect x) => !x.FullyContainedWithin(mapRect));
		if (possibleRects.Any())
		{
			IEnumerable<CellRect> source = possibleRects.Where((CellRect x) => !usedRects.Any((CellRect y) => x.Overlaps(y)));
			if (!source.Any())
			{
				possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size * 2, rectToDefend.CenterCell.z - size / 2, size, size));
				possibleRects.Add(new CellRect(rectToDefend.maxX + 1 + size, rectToDefend.CenterCell.z - size / 2, size, size));
				possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size * 2, size, size));
				possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1 + size, size, size));
			}
			if (source.Any())
			{
				return source.RandomElement();
			}
			return possibleRects.RandomElement();
		}
		return rectToDefend;
	}
}
