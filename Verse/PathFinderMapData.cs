using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LudeonTK;
using RimWorld;
using Unity.Collections;
using Verse.AI;
using Verse.AI.Group;

namespace Verse;

public class PathFinderMapData : IDisposable
{
	private readonly Map map;

	private readonly CostSource normalCost;

	private readonly CostSource fenceBlockedCost;

	private readonly CostSource flyingCost;

	private readonly AreaSource areas;

	private readonly PerceptualSource perceptual;

	private readonly ConnectivitySource connectivity;

	private readonly WaterSource water;

	private readonly FenceSource fences;

	private readonly BuildingSource buildings;

	private readonly FactionSource factions;

	private readonly FogSource fogged;

	private readonly PersistentDangerSource persistentDanger;

	private readonly DarknessSource darknessDanger;

	private readonly List<IPathFinderDataSource> sources;

	private int lastGatherTick = -1;

	private readonly List<IntVec3> cellDeltas = new List<IntVec3>();

	private readonly HashSet<IntVec3> cellDeltaSet = new HashSet<IntVec3>();

	private readonly List<CellRect> cellRectDeltas = new List<CellRect>();

	private NativeArray<byte> emptyByteGrid = NativeArrayUtility.EmptyArray<byte>();

	private NativeArray<bool> emptyBoolGrid = NativeArrayUtility.EmptyArray<bool>();

	private NativeArray<ushort> emptyUShortGrid = NativeArrayUtility.EmptyArray<ushort>();

	private NativeBitArray emptyBitGrid = NativeArrayUtility.EmptyBitArray();

	public PathFinderMapData(Map map)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		cellDeltas.Capacity = map.cellIndices.NumGridCells;
		cellDeltaSet.EnsureCapacity(map.cellIndices.NumGridCells);
		normalCost = new CostSource(map, map.pathing.Normal);
		fenceBlockedCost = new CostSource(map, map.pathing.FenceBlocked);
		flyingCost = new CostSource(map, map.pathing.Flying);
		areas = new AreaSource(map);
		perceptual = new PerceptualSource(map);
		connectivity = new ConnectivitySource(map);
		water = new WaterSource(map);
		fences = new FenceSource(map);
		buildings = new BuildingSource(map);
		factions = new FactionSource(map);
		fogged = new FogSource(map);
		persistentDanger = new PersistentDangerSource(map);
		darknessDanger = new DarknessSource(map);
		sources = new List<IPathFinderDataSource>
		{
			normalCost, fenceBlockedCost, flyingCost, areas, perceptual, connectivity, water, fences, buildings, factions,
			fogged, persistentDanger, darknessDanger
		};
		map.events.BuildingSpawned += Notify_BuildingChanged;
		map.events.BuildingDespawned += Notify_BuildingChanged;
		map.events.BuildingHitPointsChanged += Notify_BuildingChanged;
		map.events.ThingSpawned += Notify_ThingSpawnedDespawned;
		map.events.ThingDespawned += Notify_ThingSpawnedDespawned;
		map.events.ReservationAdded += Notify_Reservation;
		map.events.ReservationRemoved += Notify_Reservation;
		map.events.HaulEnrouteAdded += Notify_HaulEnroute;
		map.events.HaulEnrouteReleased += Notify_HaulReleased;
		map.events.FactionRemoved += Notify_FactionRemoved;
		map.events.TerrainChanged += Notify_CellDelta;
		map.events.PathCostRecalculate += Notify_CellDelta;
		map.events.CellFogChanged += Event_FogChanged;
		map.events.MapFogged += Notify_MapDirtied;
	}

	private void Notify_HaulEnroute(Thing enroute, Pawn pawn, ThingDef stuff, int count)
	{
		Notify_CellDelta(enroute.OccupiedRect());
	}

	private void Notify_HaulReleased(Thing enroute, Pawn pawn)
	{
		Notify_CellDelta(enroute.OccupiedRect());
	}

	private void Notify_ThingSpawnedDespawned(Thing thing)
	{
		if (thing.def.pathfinderDangerous && (!(thing is AttachableThing attachableThing) || !(attachableThing.parent is Pawn)))
		{
			if (thing.def.size.Area == 1)
			{
				Notify_CellDelta(thing.Position);
			}
			else
			{
				Notify_CellDelta(thing.OccupiedRect());
			}
		}
	}

	public void Dispose()
	{
		foreach (IPathFinderDataSource source in sources)
		{
			source.Dispose();
		}
		emptyByteGrid.Dispose();
		emptyBoolGrid.Dispose();
		emptyUShortGrid.Dispose();
		((NativeBitArray)(ref emptyBitGrid)).Dispose();
	}

	public CellConnection CellConnectionsAt(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return connectivity.Data[index];
	}

	private void Notify_Reservation(ReservationManager.Reservation reservation)
	{
		if (reservation.Target.HasThing)
		{
			Notify_CellDelta(reservation.Target.Thing.OccupiedRect());
		}
	}

	public void Notify_CellDelta(IntVec3 cell)
	{
		if (cell.InBounds(map))
		{
			if (cellDeltaSet.Add(cell))
			{
				cellDeltas.Add(cell);
			}
		}
		else
		{
			Log.Warning($"{this} was notified using an out-of-bounds cell: {cell}");
		}
	}

	private void Notify_MapDirtied()
	{
		lastGatherTick = -1;
	}

	private void Notify_BuildingChanged(Building building)
	{
		Notify_CellDelta(building.OccupiedRect());
	}

	private void Event_FogChanged(IntVec3 cell, bool _)
	{
		Notify_CellDelta(cell);
	}

	public void Notify_CellDelta(CellRect rect)
	{
		cellRectDeltas.Add(rect);
	}

	public void Notify_AreaDelta(Area area, IntVec3 cell)
	{
		areas.Notify_AreaDelta(area, cell);
	}

	public void Notify_FactionRemoved(Faction faction)
	{
		factions.Notify_Removed(faction);
	}

	public bool GatherData(IEnumerable<PathRequest> requests)
	{
		using (ProfilerBlock.Scope("PathFinderMapData.GatherData"))
		{
			if (lastGatherTick == GenTicks.TicksGame)
			{
				return false;
			}
			foreach (CellRect cellRectDelta in cellRectDeltas)
			{
				foreach (IntVec3 cell in cellRectDelta.Cells)
				{
					if (cell.InBounds(map) && cellDeltaSet.Add(cell))
					{
						cellDeltas.Add(cell);
					}
				}
			}
			bool anyChanged = cellDeltas.Any();
			cellRectDeltas.Clear();
			if (lastGatherTick >= 0)
			{
				using (ProfilerBlock.Scope("incremental"))
				{
					Parallel.ForEach(sources, delegate(IPathFinderDataSource source)
					{
						if (source.UpdateIncrementally(requests, cellDeltas))
						{
							anyChanged = true;
						}
					});
				}
			}
			else
			{
				anyChanged = true;
				using (ProfilerBlock.Scope("recompute"))
				{
					Parallel.ForEach(sources, delegate(IPathFinderDataSource source)
					{
						source.ComputeAll(requests);
					});
				}
			}
			cellDeltas.Clear();
			cellDeltaSet.Clear();
			lastGatherTick = GenTicks.TicksGame;
			return anyChanged;
		}
	}

	public void ParameterizePathJob(ref PathFinderJob job)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		job.connectivity = connectivity.Data;
		job.fences = fences.Data;
		job.buildings = buildings.Buildings;
	}

	public void ParameterizeGridJob(PathRequest request, ref PathFinder.MapGridRequest query, ref PathGridJob job, ref PathFinder.GridJobOutput output)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		Faction requesterFaction = request.RequesterFaction;
		Lord requesterLord = request.RequesterLord;
		job.traverseParams = query.traverseParams;
		job.tuning = query.tuning;
		job.indicies = map.cellIndices;
		job.grid = output.grid;
		job.pathGridDirect = (request.TraverseParms.fenceBlocked ? fenceBlockedCost.Data : normalCost.Data);
		job.building = buildings.Buildings;
		job.buildingDestroyable = buildings.Destroyable;
		job.player = buildings.Player;
		job.water = water.Data;
		job.fence = fences.Data;
		job.factionCosts = factions[requesterFaction];
		job.buildingHitPoints = buildings.Hitpoints;
		job.fogged = fogged.Data;
		job.darknessDanger = darknessDanger.Data;
		job.persistentDanger = persistentDanger.Data;
		Pawn pawn = request.pawn;
		if (pawn != null && pawn.Drafted)
		{
			job.perceptualCost = perceptual.CostDrafted;
		}
		else
		{
			job.perceptualCost = perceptual.CostUndrafted;
		}
		if (requesterLord != null)
		{
			NativeBitArray walkGrid = requesterLord.LordJob.GetWalkGrid(request.pawn);
			job.lordGrid = (((NativeBitArray)(ref walkGrid)).IsCreated ? ((NativeBitArray)(ref walkGrid)).AsReadOnly() : ((NativeBitArray)(ref emptyBitGrid)).AsReadOnly());
		}
		else
		{
			job.lordGrid = ((NativeBitArray)(ref emptyBitGrid)).AsReadOnly();
		}
		ReadOnly allowedGrid;
		if (request.area == null)
		{
			allowedGrid = ((NativeBitArray)(ref emptyBitGrid)).AsReadOnly();
		}
		else
		{
			NativeBitArray val = areas.DataForArea(request.area);
			allowedGrid = ((NativeBitArray)(ref val)).AsReadOnly();
		}
		job.allowedGrid = allowedGrid;
		job.custom = ((request.customizer != null) ? request.customizer.GetOffsetGrid().AsReadOnly() : emptyUShortGrid.AsReadOnly());
		job.avoidGrid = query.avoidGrid?.Grid ?? emptyByteGrid.AsReadOnly();
	}

	public void LogCell(IntVec3 cell)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		int num = map.cellIndices.CellToIndex(cell);
		string[] obj = new string[6]
		{
			$"Cell: {cell}\n",
			$"Cost: {normalCost.Data[num]} (fenced: {fenceBlockedCost.Data[num]}, flying: {flyingCost.Data[num]})\n",
			$"Connections: {CellConnectionsAt(num)}\n",
			null,
			null,
			null
		};
		ReadOnly val = buildings.Buildings;
		obj[3] = $"Building: {((ReadOnly)(ref val)).IsSet(num)}\n";
		val = buildings.Player;
		obj[4] = $"Player: {((ReadOnly)(ref val)).IsSet(num)}\n";
		val = fences.Data;
		obj[5] = $"Fence: {((ReadOnly)(ref val)).IsSet(num)}";
		string text = string.Concat(obj);
		foreach (Faction allFaction in Find.FactionManager.AllFactions)
		{
			if (factions.HasFactionData(allFaction))
			{
				text += $"\n    [{allFaction.def.defName}]: {factions[allFaction][num]}";
			}
		}
		Log.Message(text);
	}

	public void RegisterSource(IPathFinderDataSource source)
	{
		if (sources.Contains(source))
		{
			throw new InvalidOperationException($"Source {source} is already registered in {this}");
		}
		sources.Add(source);
	}
}
