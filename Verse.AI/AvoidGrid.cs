using System;
using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using Unity.Collections;
using UnityEngine;

namespace Verse.AI;

public class AvoidGrid : IDisposable
{
	public readonly Map map;

	private NativeArray<byte> grid;

	private bool gridDirty = true;

	public ReadOnly<byte> Grid
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (gridDirty)
			{
				Regenerate();
			}
			return grid.AsReadOnly();
		}
	}

	public byte this[IntVec3 c]
	{
		get
		{
			return grid[map.cellIndices.CellToIndex(c)];
		}
		private set
		{
			grid[map.cellIndices.CellToIndex(c)] = value;
		}
	}

	public AvoidGrid(Map map)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		grid = new NativeArray<byte>(map.cellIndices.NumGridCells, (Allocator)4, (NativeArrayOptions)1);
		map.events.BuildingSpawned += Notify_BuildingSpawned;
		map.events.BuildingDespawned += Notify_BuildingDespawned;
	}

	public void Dispose()
	{
		grid.Dispose();
	}

	public void Regenerate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		gridDirty = false;
		grid.Clear<byte>();
		List<Building> allBuildingsColonist = map.listerBuildings.allBuildingsColonist;
		for (int i = 0; i < allBuildingsColonist.Count; i++)
		{
			if (allBuildingsColonist[i].def.building.ai_combatDangerous && allBuildingsColonist[i] is Building_TurretGun tur)
			{
				PrintAvoidGridAroundTurret(tur);
			}
		}
		ExpandAvoidGridIntoEdifices();
	}

	private void Notify_BuildingSpawned(Building building)
	{
		if (building.def.building.ai_combatDangerous || !building.CanBeSeenOver())
		{
			gridDirty = true;
		}
	}

	private void Notify_BuildingDespawned(Building building)
	{
		if (building.def.building.ai_combatDangerous || !building.CanBeSeenOver())
		{
			gridDirty = true;
		}
	}

	public void DebugDrawOnMap()
	{
		if (DebugViewSettings.drawAvoidGrid && Find.CurrentMap == map)
		{
			DebugDraw();
		}
	}

	private void PrintAvoidGridAroundTurret(Building_TurretGun tur)
	{
		float range = tur.GunCompEq.PrimaryVerb.verbProps.range;
		float num = tur.GunCompEq.PrimaryVerb.verbProps.EffectiveMinRange(allowAdjacentShot: true);
		int num2 = GenRadial.NumCellsInRadius(range + 4f);
		for (int i = ((!(num < 1f)) ? GenRadial.NumCellsInRadius(num) : 0); i < num2; i++)
		{
			IntVec3 intVec = tur.Position + GenRadial.RadialPattern[i];
			if (intVec.InBounds(tur.Map) && intVec.WalkableByNormal(tur.Map) && GenSight.LineOfSight(intVec, tur.Position, tur.Map, skipFirstCell: true))
			{
				IncrementAvoidGrid(intVec, 45);
			}
		}
	}

	private void IncrementAvoidGrid(IntVec3 c, int num)
	{
		byte b = this[c];
		b = (byte)Mathf.Min(255, b + num);
		this[c] = b;
	}

	private void ExpandAvoidGridIntoEdifices()
	{
		int numGridCells = map.cellIndices.NumGridCells;
		for (int i = 0; i < numGridCells; i++)
		{
			if (grid[i] == 0 || map.edificeGrid[i] != null)
			{
				continue;
			}
			for (int j = 0; j < 8; j++)
			{
				IntVec3 c = map.cellIndices.IndexToCell(i) + GenAdj.AdjacentCells[j];
				if (c.InBounds(map) && c.GetEdifice(map) != null)
				{
					this[c] = (byte)Mathf.Min(255, Mathf.Max((int)this[c], (int)grid[i]));
				}
			}
		}
	}

	private void DebugDraw()
	{
		for (int i = 0; i < grid.Length; i++)
		{
			byte b = grid[i];
			if (b > 0)
			{
				CellRenderer.RenderCell(map.cellIndices.IndexToCell(i), (float)(int)b / 255f * 0.5f);
			}
		}
	}
}
