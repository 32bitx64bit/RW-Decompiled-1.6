using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LudeonTK;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Verse;

namespace RimWorld.Planet;

public class FastTileFinder : IDisposable
{
	public enum LandmarkMode
	{
		Any,
		Required,
		Forbidden
	}

	public readonly struct TileQueryParams : IEquatable<TileQueryParams>
	{
		public readonly PlanetTile origin;

		public readonly float minDistTiles;

		public readonly float maxDistTiles;

		public readonly LandmarkMode landmarkMode;

		public readonly bool reachable;

		public readonly bool checkBiome;

		public readonly bool comboLandmarks;

		public readonly bool validSettlement;

		public readonly Hilliness minHilliness;

		public readonly Hilliness maxHilliness;

		public bool Valid { get; }

		public TileQueryParams(PlanetTile origin, float minDistTiles = 0f, float maxDistTiles = float.MaxValue, LandmarkMode landmarkMode = LandmarkMode.Any, bool reachable = true, Hilliness minHilliness = Hilliness.Undefined, Hilliness maxHilliness = Hilliness.Undefined, bool checkBiome = true, bool validSettlement = true, bool comboLandmarks = true)
		{
			this.origin = origin;
			this.minDistTiles = minDistTiles;
			this.maxDistTiles = maxDistTiles;
			this.landmarkMode = landmarkMode;
			this.reachable = reachable;
			this.minHilliness = minHilliness;
			this.maxHilliness = maxHilliness;
			this.checkBiome = checkBiome;
			this.validSettlement = validSettlement;
			this.comboLandmarks = comboLandmarks;
			Valid = true;
		}

		public static bool operator ==(TileQueryParams lhs, TileQueryParams rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(TileQueryParams lhs, TileQueryParams rhs)
		{
			return !lhs.Equals(rhs);
		}

		public bool Equals(TileQueryParams other)
		{
			if (origin.Equals(other.origin) && landmarkMode == other.landmarkMode && reachable == other.reachable && minHilliness == other.minHilliness && maxHilliness == other.maxHilliness && checkBiome == other.checkBiome && validSettlement == other.validSettlement && comboLandmarks == other.comboLandmarks && math.abs(minDistTiles - other.minDistTiles) < 0.01f)
			{
				return math.abs(maxDistTiles - other.maxDistTiles) < 0.01f;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is TileQueryParams other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = HashCode.Combine<PlanetTile, LandmarkMode, bool, Hilliness, Hilliness, bool>(origin, landmarkMode, reachable, minHilliness, maxHilliness, checkBiome);
			int num2 = HashCode.Combine<bool, float, float, bool>(validSettlement, minDistTiles, maxDistTiles, comboLandmarks);
			return HashCode.Combine<int, int>(num, num2);
		}
	}

	private readonly struct CachedTileData : IEquatable<CachedTileData>
	{
		public readonly ushort biomeHash;

		public readonly ushort landmarkHash;

		public readonly int regionId;

		public readonly bool landmark;

		public readonly bool validForSettlement;

		public readonly bool comboLandmark;

		public readonly float3 center;

		public readonly Hilliness hilliness;

		public readonly bool passable;

		public bool Valid { get; }

		public CachedTileData(Tile tile)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			center = float3.op_Implicit(tile.Layer.GetTileCenter(tile.tile));
			regionId = Find.WorldReachability.GetLocalFieldId(tile.tile);
			passable = Find.WorldPathGrid.PassableFast(tile.tile);
			landmark = tile.Landmark != null;
			biomeHash = tile.PrimaryBiome.shortHash;
			landmarkHash = tile.Landmark?.def.shortHash ?? 0;
			hilliness = tile.hilliness;
			validForSettlement = TileFinder.IsValidTileForNewSettlement(tile.tile);
			comboLandmark = tile.Landmark?.isComboLandmark ?? false;
			Valid = true;
		}

		public bool Equals(CachedTileData other)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (biomeHash == other.biomeHash && landmarkHash == other.landmarkHash && regionId == other.regionId && landmark == other.landmark && validForSettlement == other.validForSettlement)
			{
				float3 val = center;
				if (((float3)(ref val)).Equals(other.center) && hilliness == other.hilliness)
				{
					return passable == other.passable;
				}
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is CachedTileData other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return HashCode.Combine<ushort, ushort, int, bool, bool, float3, int, bool>(biomeHash, landmarkHash, regionId, landmark, validForSettlement, center, (int)hilliness, passable);
		}
	}

	[BurstCompile]
	private struct ComputeQueryJob : IJobParallelFor
	{
		public delegate float SphericalDistance_00014ED2_0024PostfixBurstDelegate(in float3 a, in float3 b);

		internal static class SphericalDistance_00014ED2_0024BurstDirectCall
		{
			private static IntPtr Pointer;

			private static IntPtr DeferredCompilation;

			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr P_0)
			{
				if (Pointer == (IntPtr)0)
				{
					Pointer = (nint)BurstCompiler.GetILPPMethodFunctionPointer2(DeferredCompilation, (RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/, typeof(SphericalDistance_00014ED2_0024PostfixBurstDelegate).TypeHandle);
				}
				P_0 = Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				nint result = 0;
				GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Constructor()
			{
				DeferredCompilation = BurstCompiler.CompileILPPMethod2((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);
			}

			public static void Initialize()
			{
			}

			static SphericalDistance_00014ED2_0024BurstDirectCall()
			{
				Constructor();
			}

			public unsafe static float Invoke(in float3 a, in float3 b)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = GetFunctionPointer();
					if (functionPointer != (IntPtr)0)
					{
						return ((delegate* unmanaged[Cdecl]<ref float3, ref float3, float>)functionPointer)(ref a, ref b);
					}
				}
				return SphericalDistance_0024BurstManaged(in a, in b);
			}
		}

		[NativeSetThreadIndex]
		[ReadOnly]
		private int threadIndex;

		[ReadOnly]
		public NativeArray<CachedTileData> tiles;

		[ReadOnly]
		public NativeArray<ushort> biomes;

		[ReadOnly]
		public NativeArray<ushort> landmarks;

		[ReadOnly]
		public TileQueryParams normal;

		[ReadOnly]
		public TileQueryParams desperate;

		[ReadOnly]
		public int layerId;

		[ReadOnly]
		public float averageTileSize;

		[ReadOnly]
		public float layerRadius;

		[ReadOnly]
		public float3 vOrigin;

		[ReadOnly]
		public int region;

		[ReadOnly]
		public bool closest;

		public int countNormal;

		public int countDesperate;

		[ReadOnly]
		public int maximum;

		[NativeDisableParallelForRestriction]
		public NativeArray<PlanetTile> normalResult;

		[NativeDisableParallelForRestriction]
		public NativeArray<PlanetTile> desperateResult;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> closestResults;

		[NativeDisableParallelForRestriction]
		public NativeArray<float> closestSqrDists;

		[BurstCompile]
		public void Execute(int i)
		{
			CachedTileData tile = tiles[i];
			if (countNormal > maximum && !closestResults.IsCreated)
			{
				return;
			}
			if (IsValidQuery(in normal, in tile))
			{
				int num = Interlocked.Increment(ref countNormal);
				if (num <= maximum)
				{
					normalResult[num - 1] = new PlanetTile(i, layerId);
				}
				if (closest)
				{
					CheckClosest(i);
				}
			}
			if (countDesperate <= maximum && desperate.Valid && IsValidQuery(in desperate, in tile))
			{
				int num2 = Interlocked.Increment(ref countDesperate);
				if (num2 <= maximum)
				{
					desperateResult[num2 - 1] = new PlanetTile(i, layerId);
				}
				if (closest)
				{
					CheckClosest(i);
				}
			}
		}

		private void CheckClosest(int index)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			float num = math.distancesq(tiles[index].center, vOrigin);
			if (closestResults[threadIndex] == -1 || num < closestSqrDists[threadIndex])
			{
				closestResults[threadIndex] = index;
				closestSqrDists[threadIndex] = num;
			}
		}

		private bool IsValidQuery(in TileQueryParams query, in CachedTileData tile)
		{
			if (query.reachable && region >= 0 && tile.regionId != region)
			{
				return false;
			}
			if (query.minHilliness != 0 && (int)tile.hilliness < (int)query.minHilliness)
			{
				return false;
			}
			if (query.maxHilliness != 0 && (int)tile.hilliness > (int)query.maxHilliness)
			{
				return false;
			}
			if (query.validSettlement && !tile.validForSettlement)
			{
				return false;
			}
			if (!IsValidLandmark(in query, in tile))
			{
				return false;
			}
			if (query.checkBiome && !IsValidBiome(in tile))
			{
				return false;
			}
			if (!IsValidDistance(in query, in tile))
			{
				return false;
			}
			return true;
		}

		private bool IsValidDistance(in TileQueryParams query, in CachedTileData tile)
		{
			float num = SphericalDistance(in vOrigin, in tile.center);
			num = num * layerRadius / averageTileSize;
			if (num >= query.minDistTiles)
			{
				return num <= query.maxDistTiles;
			}
			return false;
		}

		private bool IsValidLandmark(in TileQueryParams query, in CachedTileData tile)
		{
			if (query.landmarkMode == LandmarkMode.Any)
			{
				return true;
			}
			if (!query.comboLandmarks && tile.comboLandmark)
			{
				return false;
			}
			if (landmarks.Length > 0)
			{
				bool flag = false;
				for (int i = 0; i < landmarks.Length; i++)
				{
					if (landmarks[i] == tile.landmarkHash)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return tile.landmark == (query.landmarkMode == LandmarkMode.Required);
		}

		private bool IsValidBiome(in CachedTileData tile)
		{
			if (biomes.Length <= 0)
			{
				return true;
			}
			bool result = false;
			for (int i = 0; i < biomes.Length; i++)
			{
				if (biomes[i] == tile.biomeHash)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		[BurstCompile]
		private static float SphericalDistance(in float3 a, in float3 b)
		{
			return SphericalDistance_00014ED2_0024BurstDirectCall.Invoke(in a, in b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[BurstCompile]
		public static float SphericalDistance_0024BurstManaged(in float3 a, in float3 b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (math.lengthsq(a - b) < 0.0001f)
			{
				return 0f;
			}
			return math.acos(math.dot(math.normalize(a), math.normalize(b)));
		}
	}

	private NativeArray<PlanetTile> tmpNormalResults;

	private NativeArray<PlanetTile> tmpDesperateResults;

	private NativeArray<CachedTileData> cachedTileData;

	private ParallelLoopResult rebuildingTask;

	private bool cacheDirty = true;

	private bool tileChangedSinceQuery;

	private TileQueryParams lastNormalQuery;

	private TileQueryParams lastDesperateQuery;

	private readonly List<PlanetTile> tmpOutputResults = new List<PlanetTile>();

	private TileQueryParams lastClosestNormalQuery;

	private TileQueryParams lastClosestDesperateQuery;

	private PlanetTile tmpOutputClosest = PlanetTile.Invalid;

	public PlanetLayer Layer { get; }

	public int MaxResults => math.min(Layer.TilesCount, 50);

	public FastTileFinder(PlanetLayer layer)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Layer = layer;
		tmpNormalResults = new NativeArray<PlanetTile>(MaxResults, (Allocator)4, (NativeArrayOptions)1);
		tmpDesperateResults = new NativeArray<PlanetTile>(MaxResults, (Allocator)4, (NativeArrayOptions)1);
		cachedTileData = new NativeArray<CachedTileData>(layer.TilesCount, (Allocator)4, (NativeArrayOptions)1);
	}

	public void Dispose()
	{
		NativeArrayUtility.EnsureDisposed(ref tmpNormalResults);
		NativeArrayUtility.EnsureDisposed(ref tmpDesperateResults);
		NativeArrayUtility.EnsureDisposed(ref cachedTileData);
	}

	public void DirtyCache()
	{
		cacheDirty = true;
	}

	public void DirtyTile(PlanetTile tile)
	{
		if (!cacheDirty)
		{
			tileChangedSinceQuery = true;
			cachedTileData[tile.tileId] = new CachedTileData(tile.Tile);
		}
	}

	public List<PlanetTile> Query(TileQueryParams query, List<BiomeDef> biomes = null, List<LandmarkDef> landmarks = null, TileQueryParams desperate = default(TileQueryParams))
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (cacheDirty)
		{
			RegenerateCache();
		}
		else if (!tileChangedSinceQuery && lastNormalQuery == query && lastDesperateQuery == desperate)
		{
			return tmpOutputResults;
		}
		PrepareBuffers(biomes, landmarks, out var nBiomes, out var nLandmarks);
		using (ProfilerBlock.Scope("Compute Tile Query"))
		{
			ComputeQueryJob computeQueryJob = ParameterizeJob(query, desperate, nBiomes, nLandmarks);
			int length = cachedTileData.Length;
			int idealBatchCount = UnityData.GetIdealBatchCount(cachedTileData.Length);
			JobHandle val = default(JobHandle);
			val = IJobParallelForExtensions.Schedule<ComputeQueryJob>(computeQueryJob, length, idealBatchCount, val);
			((JobHandle)(ref val)).Complete();
		}
		nBiomes.Dispose();
		nLandmarks.Dispose();
		lastNormalQuery = query;
		lastDesperateQuery = desperate;
		tileChangedSinceQuery = false;
		if (tmpNormalResults.Length > 0)
		{
			NativeArrayUtility.CopyArrayToList(tmpOutputResults, tmpNormalResults);
			RemoveInvalidTiles(tmpOutputResults);
		}
		if (tmpOutputResults.Count == 0 && tmpDesperateResults.Length > 0)
		{
			NativeArrayUtility.CopyArrayToList(tmpOutputResults, tmpDesperateResults);
			RemoveInvalidTiles(tmpOutputResults);
		}
		return tmpOutputResults;
	}

	public bool Closest(TileQueryParams query, out PlanetTile tile, List<BiomeDef> biomes = null, List<LandmarkDef> landmarks = null, TileQueryParams desperate = default(TileQueryParams))
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (cacheDirty)
		{
			RegenerateCache();
		}
		else if (!tileChangedSinceQuery && lastClosestNormalQuery == query && lastClosestDesperateQuery == desperate)
		{
			tile = tmpOutputClosest;
			return tile.Valid;
		}
		PrepareBuffers(biomes, landmarks, out var nBiomes, out var nLandmarks);
		using (ProfilerBlock.Scope("Compute Tile Query"))
		{
			ComputeQueryJob computeQueryJob = ParameterizeJob(query, desperate, nBiomes, nLandmarks);
			computeQueryJob.closest = true;
			ComputeQueryJob computeQueryJob2 = computeQueryJob;
			int length = cachedTileData.Length;
			int idealBatchCount = UnityData.GetIdealBatchCount(cachedTileData.Length);
			JobHandle val = default(JobHandle);
			val = IJobParallelForExtensions.Schedule<ComputeQueryJob>(computeQueryJob2, length, idealBatchCount, val);
			((JobHandle)(ref val)).Complete();
		}
		nBiomes.Dispose();
		nLandmarks.Dispose();
		lastClosestNormalQuery = query;
		lastClosestDesperateQuery = desperate;
		tileChangedSinceQuery = false;
		if (TryGetClosest(out tile))
		{
			tmpOutputClosest = tile;
			return true;
		}
		tmpOutputClosest = (tile = PlanetTile.Invalid);
		return false;
	}

	private bool TryGetClosest(out PlanetTile closest)
	{
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < ThreadResultPool.closestSqrDists.Length; i++)
		{
			if (ThreadResultPool.closestSqrDists[i] < num2)
			{
				num = ThreadResultPool.closestResults[i];
				num2 = ThreadResultPool.closestSqrDists[i];
			}
		}
		if (num == -1)
		{
			closest = PlanetTile.Invalid;
			return false;
		}
		closest = new PlanetTile(num, Layer);
		return true;
	}

	private ComputeQueryJob ParameterizeJob(TileQueryParams query, TileQueryParams desperate, NativeArray<ushort> nBiomes, NativeArray<ushort> nLandmarks)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		PlanetTile origin = query.origin;
		int region = ((origin.Valid && origin.Layer == Layer) ? cachedTileData[origin.tileId].regionId : (-1));
		ComputeQueryJob result = default(ComputeQueryJob);
		result.tiles = cachedTileData;
		result.normalResult = tmpNormalResults;
		result.desperateResult = tmpDesperateResults;
		result.closestResults = ThreadResultPool.closestResults;
		result.closestSqrDists = ThreadResultPool.closestSqrDists;
		result.biomes = nBiomes;
		result.landmarks = nLandmarks;
		result.normal = query;
		result.desperate = desperate;
		result.layerId = Layer.LayerID;
		result.layerRadius = Layer.Radius;
		result.averageTileSize = Layer.AverageTileSize;
		result.maximum = MaxResults;
		result.countNormal = 0;
		result.countDesperate = 0;
		result.vOrigin = float3.op_Implicit(origin.Layer.GetTileCenter(origin));
		result.region = region;
		return result;
	}

	private void PrepareBuffers(List<BiomeDef> biomes, List<LandmarkDef> landmarks, out NativeArray<ushort> nBiomes, out NativeArray<ushort> nLandmarks)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ResetNativeCache();
		ThreadResultPool.EnsureReady();
		nBiomes = new NativeArray<ushort>((!biomes.NullOrEmpty()) ? biomes.Count : 0, (Allocator)3, (NativeArrayOptions)1);
		nLandmarks = new NativeArray<ushort>((!landmarks.NullOrEmpty()) ? landmarks.Count : 0, (Allocator)3, (NativeArrayOptions)1);
		if (!biomes.NullOrEmpty())
		{
			for (int i = 0; i < biomes.Count; i++)
			{
				nBiomes[i] = biomes[i].shortHash;
			}
		}
		if (!landmarks.NullOrEmpty())
		{
			for (int j = 0; j < landmarks.Count; j++)
			{
				nLandmarks[j] = landmarks[j].shortHash;
			}
		}
	}

	private static void RemoveInvalidTiles(List<PlanetTile> tiles)
	{
		for (int num = tiles.Count - 1; num >= 0; num--)
		{
			if (!tiles[num].Valid)
			{
				tiles.RemoveAt(num);
			}
		}
	}

	private void ResetNativeCache()
	{
		for (int i = 0; i < MaxResults; i++)
		{
			tmpNormalResults[i] = PlanetTile.Invalid;
			tmpDesperateResults[i] = PlanetTile.Invalid;
		}
	}

	public void RegenerateCache()
	{
		if (Find.WorldGrid != null)
		{
			cacheDirty = false;
			tileChangedSinceQuery = true;
			Parallel.For(0, Layer.TilesCount, delegate(int i)
			{
				cachedTileData[i] = new CachedTileData(Layer.Tiles[i]);
			});
		}
	}

	public static void Initialize_0024ComputeQueryJob_SphericalDistance_00014ED2_0024BurstDirectCall()
	{
		ComputeQueryJob.SphericalDistance_00014ED2_0024BurstDirectCall.Initialize();
	}
}
