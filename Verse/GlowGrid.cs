using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Verse.Glow;

namespace Verse;

[BurstCompile(CompileSynchronously = true)]
public sealed class GlowGrid : IDisposable
{
	private static class FloodFillerPool
	{
		public static NativeArray<GlowUniqueState> pool;

		private static bool allocated;

		public static void EnsureAllocated()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (!allocated)
			{
				allocated = true;
				UnityData.DisposeStatic += Dispose;
				pool = new NativeArray<GlowUniqueState>(UnityData.MaxJobWorkerThreadCount, (Allocator)4, (NativeArrayOptions)1);
				for (int i = 0; i < pool.Length; i++)
				{
					pool[i] = GlowUniqueState.AllocateNew();
				}
			}
		}

		private static void Dispose()
		{
			if (allocated)
			{
				for (int i = 0; i < pool.Length; i++)
				{
					pool[i].Dispose();
				}
				pool.Dispose();
				allocated = false;
			}
		}
	}

	private class GlowPool : IDisposable
	{
		public NativeList<LocalGlowArea> pool = new NativeList<LocalGlowArea>(4096, AllocatorHandle.op_Implicit((Allocator)4));

		public int Take()
		{
			for (int i = 0; i < pool.Length; i++)
			{
				LocalGlowArea localGlowArea = pool[i];
				if (!localGlowArea.inUse)
				{
					localGlowArea.Take();
					pool[i] = localGlowArea;
					return i;
				}
			}
			LocalGlowArea localGlowArea2 = LocalGlowArea.AllocateNew();
			localGlowArea2.Take();
			pool.Add(ref localGlowArea2);
			return pool.Length - 1;
		}

		public void ReturnSet(int index)
		{
			LocalGlowArea localGlowArea = pool[index];
			localGlowArea.Return();
			pool[index] = localGlowArea;
		}

		public void Dispose()
		{
			for (int i = 0; i < pool.Length; i++)
			{
				pool[i].Dispose();
			}
			pool.Dispose();
		}
	}

	[BurstCompile]
	private struct CombineColorsJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeList<GlowLight> lights;

		public UnsafeBitArray dirtyCells;

		public CellIndices indices;

		public bool allDirty;

		[NativeDisableParallelForRestriction]
		public NativeArray<Color32> glow;

		[NativeDisableParallelForRestriction]
		public NativeArray<Color32> glowNoCavePlants;

		[NativeDisableParallelForRestriction]
		public NativeList<LocalGlowArea> glowPool;

		[BurstCompile]
		public void Execute(int i)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			if (!((UnsafeBitArray)(ref dirtyCells)).IsSet(i) && !allDirty)
			{
				return;
			}
			((UnsafeBitArray)(ref dirtyCells)).Set(i, false);
			IntVec3 world = indices.IndexToCell(i);
			Color32 existingSum = default(Color32);
			Color32 existingSum2 = default(Color32);
			for (int j = 0; j < lights.Length; j++)
			{
				GlowLight glowLight = lights[j];
				if (glowLight.AffectedRect.Contains(world))
				{
					int num = glowLight.WorldToLocalIndex(in world);
					Color32 toAdd = glowPool[glowLight.localGlowPoolIndex].colors[num];
					AddColors(ref existingSum, in toAdd, glowLight.overlightRadius);
					if (!glowLight.isCavePlant)
					{
						AddColors(ref existingSum2, in toAdd, glowLight.overlightRadius);
					}
				}
			}
			glow[i] = existingSum;
			glowNoCavePlants[i] = existingSum2;
		}

		[BurstCompile]
		private void AddColors(ref Color32 existingSum, in Color32 toAdd, float overlightRadius)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			float num = (int)toAdd.a;
			ColorInt colorInt = toAdd.AsColorInt();
			colorInt.ClampToNonNegative();
			colorInt.a = 0;
			if (colorInt.r > 0 || colorInt.g > 0 || colorInt.b > 0)
			{
				ColorInt colorInt2 = existingSum.AsColorInt();
				colorInt2 += colorInt;
				if (num < overlightRadius)
				{
					colorInt2.a = 1;
				}
				colorInt2.ProjectToColor32Fast(out var outColor);
				existingSum = outColor;
			}
		}
	}

	private readonly Map map;

	private readonly CellIndices indices;

	private readonly GlowPool glowPool;

	private NativeList<GlowLight> lights;

	private NativeBitArray lightBlockers;

	private UnsafeBitArray dirtyCells;

	private NativeArray<Color32> accumulatedGlow;

	private NativeArray<Color32> accumulatedGlowNoCavePlants;

	private readonly HashSet<CompGlower> litGlowers = new HashSet<CompGlower>();

	private readonly HashSet<IntVec3> litTerrain = new HashSet<IntVec3>();

	private bool anyDirtyCell = true;

	private bool anyDirtyLight;

	private bool hasRunInitially;

	private const int AlphaOfOverlit = 1;

	private const float GameGlowLitThreshold = 0.3f;

	private const float GameGlowOverlitThreshold = 0.9f;

	private const float GroundGameGlowFactor = 3.6f;

	private const float MaxGameGlowFromNonOverlitGroundLights = 0.5f;

	public const int MaxLightRadius = 40;

	public const int MaxLightDiameter = 81;

	public const int MaxLightCells = 6561;

	public GlowGrid(Map map)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		indices = map.cellIndices;
		lights = new NativeList<GlowLight>(4096, AllocatorHandle.op_Implicit((Allocator)4));
		lightBlockers = new NativeBitArray(indices.NumGridCells, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		dirtyCells = new UnsafeBitArray(indices.NumGridCells, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		accumulatedGlow = new NativeArray<Color32>(indices.NumGridCells, (Allocator)4, (NativeArrayOptions)1);
		accumulatedGlowNoCavePlants = new NativeArray<Color32>(indices.NumGridCells, (Allocator)4, (NativeArrayOptions)1);
		glowPool = new GlowPool();
		FloodFillerPool.EnsureAllocated();
	}

	private Color32 GetAccumulatedGlowAt(IntVec3 c, bool ignoreCavePlants = false)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return GetAccumulatedGlowAt(map.cellIndices.CellToIndex(c), ignoreCavePlants);
	}

	private Color32 GetAccumulatedGlowAt(int index, bool ignoreCavePlants = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!accumulatedGlow.IsCreated)
		{
			return default(Color32);
		}
		if (map.terrainGrid.TerrainAt(index).exposesToVacuum)
		{
			return default(Color32);
		}
		return (ignoreCavePlants ? accumulatedGlowNoCavePlants : accumulatedGlow)[index];
	}

	public Color32 VisualGlowAt(int index)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetAccumulatedGlowAt(index);
	}

	public Color32 VisualGlowAt(IntVec3 c)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GetAccumulatedGlowAt(c);
	}

	public float GroundGlowAt(IntVec3 c, bool ignoreCavePlants = false, bool ignoreSky = false)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if (!ignoreSky && !map.roofGrid.Roofed(c))
		{
			num = map.skyManager.CurSkyGlow;
			if (num >= 1f)
			{
				return num;
			}
		}
		Color32 accumulatedGlowAt = GetAccumulatedGlowAt(c, ignoreCavePlants);
		if (accumulatedGlowAt.a == 1)
		{
			return 1f;
		}
		float num2 = (float)Mathf.Max(Mathf.Max((int)accumulatedGlowAt.r, (int)accumulatedGlowAt.g), (int)accumulatedGlowAt.b) / 255f * 3.6f;
		num2 = Mathf.Min(0.5f, num2);
		return Mathf.Max(num, num2);
	}

	public PsychGlow PsychGlowAt(IntVec3 c)
	{
		return PsychGlowAtGlow(GroundGlowAt(c));
	}

	public static PsychGlow PsychGlowAtGlow(float glow)
	{
		if (glow > 0.9f)
		{
			return PsychGlow.Overlit;
		}
		if (glow > 0.3f)
		{
			return PsychGlow.Lit;
		}
		return PsychGlow.Dark;
	}

	public void RegisterGlower(CompGlower newGlow)
	{
		if (!litGlowers.Add(newGlow))
		{
			return;
		}
		ref NativeList<GlowLight> reference = ref lights;
		GlowLight glowLight = new GlowLight(newGlow, glowPool.Take());
		reference.Add(ref glowLight);
		anyDirtyLight = true;
		ref NativeList<GlowLight> reference2 = ref lights;
		foreach (IntVec3 item in reference2[reference2.Length - 1].AffectedRect.ClipInsideMap(map))
		{
			DirtyCell(item);
		}
	}

	public void DeRegisterGlower(CompGlower oldGlow)
	{
		if (!litGlowers.Remove(oldGlow) || !lights.IsCreated)
		{
			return;
		}
		int glowerIndex = GetGlowerIndex(oldGlow);
		GlowLight glowLight = lights[glowerIndex];
		foreach (IntVec3 item in glowLight.AffectedRect.ClipInsideMap(map))
		{
			DirtyCell(item);
		}
		glowPool.ReturnSet(glowLight.localGlowPoolIndex);
		lights.RemoveAt(glowerIndex);
	}

	[BurstCompile]
	private int GetGlowerIndex(CompGlower glower)
	{
		for (int i = 0; i < lights.Length; i++)
		{
			GlowLight glowLight = lights[i];
			if (!glowLight.isTerrain && glowLight.id == glower.parent.thingIDNumber)
			{
				return i;
			}
		}
		return -1;
	}

	public void RegisterTerrain(IntVec3 cell)
	{
		if (!litTerrain.Add(cell))
		{
			return;
		}
		ref NativeList<GlowLight> reference = ref lights;
		GlowLight glowLight = new GlowLight(cell, map, glowPool.Take());
		reference.Add(ref glowLight);
		anyDirtyLight = true;
		ref NativeList<GlowLight> reference2 = ref lights;
		foreach (IntVec3 item in reference2[reference2.Length - 1].AffectedRect.ClipInsideMap(map))
		{
			DirtyCell(item);
		}
	}

	public void DeregisterTerrain(IntVec3 cell)
	{
		if (!litTerrain.Remove(cell) || !lights.IsCreated)
		{
			return;
		}
		for (int i = 0; i < lights.Length; i++)
		{
			GlowLight glowLight = lights[i];
			if (!glowLight.isTerrain || glowLight.position != cell)
			{
				continue;
			}
			foreach (IntVec3 item in glowLight.AffectedRect.ClipInsideMap(map))
			{
				DirtyCell(item);
			}
			glowPool.ReturnSet(glowLight.localGlowPoolIndex);
			lights.RemoveAt(i);
			break;
		}
	}

	public void GlowGridUpdate_First()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (!accumulatedGlow.IsCreated || !accumulatedGlowNoCavePlants.IsCreated)
		{
			return;
		}
		try
		{
			JobHandle val;
			if (anyDirtyLight)
			{
				using (new ProfilerBlock("ComputeGlowGridsJob()"))
				{
					ComputeGlowGridsJob computeGlowGridsJob = default(ComputeGlowGridsJob);
					computeGlowGridsJob.lights = lights;
					computeGlowGridsJob.indices = indices;
					computeGlowGridsJob.lightBlockers = lightBlockers;
					computeGlowGridsJob.statePool = FloodFillerPool.pool;
					computeGlowGridsJob.glowPool = glowPool.pool;
					computeGlowGridsJob.mapSize = map.Size;
					ComputeGlowGridsJob computeGlowGridsJob2 = computeGlowGridsJob;
					int length = lights.Length;
					int idealBatchCount = UnityData.GetIdealBatchCount(lights.Length);
					val = default(JobHandle);
					val = IJobParallelForExtensions.Schedule<ComputeGlowGridsJob>(computeGlowGridsJob2, length, idealBatchCount, val);
					((JobHandle)(ref val)).Complete();
					anyDirtyLight = false;
				}
			}
			if (anyDirtyCell)
			{
				using (new ProfilerBlock("CombineColorsJob()"))
				{
					CombineColorsJob combineColorsJob = default(CombineColorsJob);
					combineColorsJob.lights = lights;
					combineColorsJob.dirtyCells = dirtyCells;
					combineColorsJob.indices = indices;
					combineColorsJob.glow = accumulatedGlow;
					combineColorsJob.glowNoCavePlants = accumulatedGlowNoCavePlants;
					combineColorsJob.glowPool = glowPool.pool;
					combineColorsJob.allDirty = !hasRunInitially;
					CombineColorsJob combineColorsJob2 = combineColorsJob;
					int numGridCells = indices.NumGridCells;
					int idealBatchCount2 = UnityData.GetIdealBatchCount(indices.NumGridCells);
					val = default(JobHandle);
					val = IJobParallelForExtensions.Schedule<CombineColorsJob>(combineColorsJob2, numGridCells, idealBatchCount2, val);
					((JobHandle)(ref val)).Complete();
					anyDirtyCell = false;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex.ToString());
			throw;
		}
		if (!hasRunInitially)
		{
			map.mapDrawer.WholeMapChanged(MapMeshFlagDefOf.GroundGlow);
		}
		hasRunInitially = true;
	}

	public void DirtyCell(IntVec3 cell)
	{
		if (((UnsafeBitArray)(ref dirtyCells)).IsCreated && hasRunInitially)
		{
			((UnsafeBitArray)(ref dirtyCells)).Set(indices.CellToIndex(cell), true);
			map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.Roofs);
			map.mapDrawer.MapMeshDirty(cell, MapMeshFlagDefOf.GroundGlow);
			anyDirtyCell = true;
			map.events.Notify_GlowChanged(cell);
		}
	}

	public void LightBlockerAdded(IntVec3 cell)
	{
		((NativeBitArray)(ref lightBlockers)).Set(indices.CellToIndex(cell), true);
		DirtyCell(cell);
		DirtyLightsAround(cell);
	}

	public void LightBlockerRemoved(IntVec3 cell)
	{
		if (((NativeBitArray)(ref lightBlockers)).IsCreated)
		{
			((NativeBitArray)(ref lightBlockers)).Set(indices.CellToIndex(cell), false);
			DirtyCell(cell);
			DirtyLightsAround(cell);
		}
	}

	[BurstCompile]
	private void DirtyLightsAround(IntVec3 cell)
	{
		if (!hasRunInitially)
		{
			return;
		}
		for (int i = 0; i < lights.Length; i++)
		{
			GlowLight glowLight = lights[i];
			if (glowLight.dirty || !glowLight.AffectedRect.Contains(cell))
			{
				continue;
			}
			glowLight.dirty = true;
			lights[i] = glowLight;
			foreach (IntVec3 item in glowLight.AffectedRect)
			{
				if (item.InBounds(map) && indices.Contains(item))
				{
					DirtyCell(item);
				}
			}
			anyDirtyLight = true;
		}
	}

	public void DevPrintLightIdsAffectingCell(IntVec3 cell)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Lights affecting {cell}\n");
		for (int i = 0; i < lights.Length; i++)
		{
			if (lights[i].AffectedRect.Contains(cell))
			{
				stringBuilder.AppendLine($"   [{i}] - {lights[i].ToString()}");
			}
		}
		Log.Message(stringBuilder.ToString());
	}

	public void DevDirtyRect(CellRect rect)
	{
		foreach (IntVec3 cell in rect.Cells)
		{
			((UnsafeBitArray)(ref dirtyCells)).Set(indices.CellToIndex(cell), true);
		}
		anyDirtyCell = true;
	}

	public void Dispose()
	{
		glowPool.Dispose();
		lights.Dispose();
		((UnsafeBitArray)(ref dirtyCells)).Dispose();
		((NativeBitArray)(ref lightBlockers)).Dispose();
		accumulatedGlow.Dispose();
		accumulatedGlowNoCavePlants.Dispose();
	}
}
