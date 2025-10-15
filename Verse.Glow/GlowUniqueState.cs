using System;
using LudeonTK;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Verse.Glow;

public struct GlowUniqueState : IDisposable
{
	[NativeDisableParallelForRestriction]
	public UnsafeHeap<IntVec3, GlowCellComparer> queue;

	[NativeDisableParallelForRestriction]
	public UnsafeList<GlowCell> area;

	[NativeDisableParallelForRestriction]
	public UnsafeBitArray blockers;

	public static GlowUniqueState AllocateNew()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		GlowUniqueState glowUniqueState = default(GlowUniqueState);
		glowUniqueState.queue = new UnsafeHeap<IntVec3, GlowCellComparer>((Allocator)4, 324);
		glowUniqueState.area = new UnsafeList<GlowCell>(6561, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)0);
		glowUniqueState.blockers = new UnsafeBitArray(8, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		GlowUniqueState result = glowUniqueState;
		result.area.Resize(6561, (NativeArrayOptions)1);
		return result;
	}

	public void PrepareComparer(ref GlowLight light)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		queue.Comparator = new GlowCellComparer(area, in light);
	}

	public void Clear()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NativeArrayUtility.MemClear<GlowCell>(area);
		queue.Clear();
		((UnsafeBitArray)(ref blockers)).Clear();
	}

	public void Dispose()
	{
		NativeArrayUtility.EnsureDisposed(ref area);
		NativeArrayUtility.EnsureDisposed(ref queue);
		blockers.EnsureDisposed();
	}
}
