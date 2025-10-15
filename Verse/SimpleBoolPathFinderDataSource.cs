using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Verse;

public abstract class SimpleBoolPathFinderDataSource : IPathFinderDataSource, IDisposable
{
	protected readonly Map map;

	protected readonly int cellCount;

	protected NativeBitArray data;

	public ReadOnly Data => ((NativeBitArray)(ref data)).AsReadOnly();

	public SimpleBoolPathFinderDataSource(Map map)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		this.map = map;
		cellCount = map.cellIndices.NumGridCells;
		data = new NativeBitArray(cellCount, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
	}

	public virtual void Dispose()
	{
		((NativeBitArray)(ref data)).Dispose();
	}

	public abstract void ComputeAll(IEnumerable<PathRequest> requests);

	public abstract bool UpdateIncrementally(IEnumerable<PathRequest> requests, List<IntVec3> cellDeltas);
}
