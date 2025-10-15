using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Verse;

public abstract class SimplePathFinderDataSource<T> : IPathFinderDataSource, IDisposable where T : struct
{
	protected readonly Map map;

	protected readonly int cellCount;

	protected NativeArray<T> data;

	public ReadOnly<T> Data => data.AsReadOnly();

	public SimplePathFinderDataSource(Map map)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		cellCount = map.cellIndices.NumGridCells;
		data = new NativeArray<T>(cellCount, (Allocator)4, (NativeArrayOptions)1);
	}

	public virtual void Dispose()
	{
		data.Dispose();
	}

	public abstract void ComputeAll(IEnumerable<PathRequest> requests);

	public abstract bool UpdateIncrementally(IEnumerable<PathRequest> requests, List<IntVec3> cellDeltas);
}
