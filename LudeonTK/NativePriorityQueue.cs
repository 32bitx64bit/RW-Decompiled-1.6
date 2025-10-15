using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace LudeonTK;

[NativeContainer]
[DebuggerDisplay("Count = {Count}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
public struct NativePriorityQueue<TElement, TPriority, TComparer> : IDisposable where TElement : unmanaged where TPriority : unmanaged where TComparer : unmanaged, IComparer<TPriority>
{
	internal struct Data
	{
		public int m_Count;
	}

	[NativeDisableUnsafePtrRestriction]
	internal unsafe void* m_ElementBuffer;

	[NativeDisableUnsafePtrRestriction]
	internal unsafe void* m_PriorityBuffer;

	[NativeDisableUnsafePtrRestriction]
	internal unsafe Data* m_Data;

	internal int m_Capacity;

	internal TComparer m_Comparer;

	internal Allocator m_AllocatorLabel;

	private const int Arity = 4;

	private const int Log2Arity = 2;

	public unsafe bool IsCreated => m_ElementBuffer != null;

	public int Capacity => m_Capacity;

	public unsafe int Count
	{
		get
		{
			if (m_Data == null)
			{
				return 0;
			}
			return m_Data->m_Count;
		}
	}

	public unsafe NativePriorityQueue(int capacity, TComparer comparer, Allocator allocator)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		long num = (long)UnsafeUtility.SizeOf<TElement>() * (long)capacity;
		m_ElementBuffer = UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<TElement>(), allocator);
		long num2 = (long)UnsafeUtility.SizeOf<TPriority>() * (long)capacity;
		m_PriorityBuffer = UnsafeUtility.Malloc(num2, UnsafeUtility.AlignOf<TPriority>(), allocator);
		m_Data = (Data*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<Data>(), UnsafeUtility.AlignOf<Data>(), allocator);
		m_Data->m_Count = 0;
		m_Capacity = capacity;
		m_Comparer = comparer;
		m_AllocatorLabel = allocator;
	}

	[WriteAccessRequired]
	public unsafe void Clear()
	{
		m_Data->m_Count = 0;
	}

	[WriteAccessRequired]
	public unsafe void Enqueue(TElement element, TPriority priority)
	{
		int num = m_Data->m_Count;
		m_Data->m_Count = num + 1;
		TComparer comparer = m_Comparer;
		while (num > 0)
		{
			int num2 = num - 1 >> 2;
			TPriority val = UnsafeUtility.ReadArrayElement<TPriority>(m_PriorityBuffer, num2);
			if (comparer.Compare(priority, val) >= 0)
			{
				break;
			}
			TElement val2 = UnsafeUtility.ReadArrayElement<TElement>(m_ElementBuffer, num2);
			UnsafeUtility.WriteArrayElement<TElement>(m_ElementBuffer, num, val2);
			UnsafeUtility.WriteArrayElement<TPriority>(m_PriorityBuffer, num, val);
			num = num2;
		}
		UnsafeUtility.WriteArrayElement<TElement>(m_ElementBuffer, num, element);
		UnsafeUtility.WriteArrayElement<TPriority>(m_PriorityBuffer, num, priority);
	}

	[WriteAccessRequired]
	public unsafe void Dequeue(out TElement element, out TPriority priority)
	{
		element = UnsafeUtility.ReadArrayElement<TElement>(m_ElementBuffer, 0);
		priority = UnsafeUtility.ReadArrayElement<TPriority>(m_PriorityBuffer, 0);
		RemoveRootNode();
	}

	private unsafe void RemoveRootNode()
	{
		int num = m_Data->m_Count - 1;
		m_Data->m_Count = num;
		if (num > 0)
		{
			TElement element = UnsafeUtility.ReadArrayElement<TElement>(m_ElementBuffer, num);
			TPriority priority = UnsafeUtility.ReadArrayElement<TPriority>(m_PriorityBuffer, num);
			MoveDown(element, priority, 0);
		}
	}

	private unsafe void MoveDown(TElement element, TPriority priority, int nodeIndex)
	{
		TComparer comparer = m_Comparer;
		int count = Count;
		int num;
		while ((num = (nodeIndex << 2) + 1) < count)
		{
			TElement val = UnsafeUtility.ReadArrayElement<TElement>(m_ElementBuffer, num);
			TPriority val2 = UnsafeUtility.ReadArrayElement<TPriority>(m_PriorityBuffer, num);
			int num2 = num;
			int num3 = math.min(num + 4, count);
			while (++num < num3)
			{
				TPriority val3 = UnsafeUtility.ReadArrayElement<TPriority>(m_PriorityBuffer, num);
				if (comparer.Compare(val3, val2) < 0)
				{
					val = UnsafeUtility.ReadArrayElement<TElement>(m_ElementBuffer, num);
					val2 = val3;
					num2 = num;
				}
			}
			if (comparer.Compare(priority, val2) <= 0)
			{
				break;
			}
			UnsafeUtility.WriteArrayElement<TElement>(m_ElementBuffer, nodeIndex, val);
			UnsafeUtility.WriteArrayElement<TPriority>(m_PriorityBuffer, nodeIndex, val2);
			nodeIndex = num2;
		}
		UnsafeUtility.WriteArrayElement<TElement>(m_ElementBuffer, nodeIndex, element);
		UnsafeUtility.WriteArrayElement<TPriority>(m_PriorityBuffer, nodeIndex, priority);
	}

	[WriteAccessRequired]
	public unsafe void Dispose()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (m_ElementBuffer == null)
		{
			throw new ObjectDisposedException("The collection is already disposed.");
		}
		if ((int)m_AllocatorLabel == 0)
		{
			throw new InvalidOperationException("The collection can not be Disposed because it was not allocated with a valid allocator.");
		}
		if ((int)m_AllocatorLabel > 1)
		{
			UnsafeUtility.Free(m_ElementBuffer, m_AllocatorLabel);
			UnsafeUtility.Free(m_PriorityBuffer, m_AllocatorLabel);
			UnsafeUtility.Free((void*)m_Data, m_AllocatorLabel);
			m_AllocatorLabel = (Allocator)0;
		}
		m_ElementBuffer = null;
		m_PriorityBuffer = null;
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void CheckAllocateArguments(int capacity, Allocator allocator)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity", "Capacity must be >= 0");
		}
		if ((int)allocator <= 1 || (int)allocator > 4)
		{
			throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", "allocator");
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void CheckTotalSize(long totalSize)
	{
		if (totalSize > int.MaxValue)
		{
			throw new ArgumentOutOfRangeException("totalSize", $"Allocation total size cannot exceed {int.MaxValue} bytes");
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void CheckNotEmpty(int count)
	{
		if (count == 0)
		{
			throw new InvalidOperationException("The container is empty");
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void CheckSufficientCapacity(int capacity, int index)
	{
		if (index >= capacity)
		{
			throw new InvalidOperationException("The container has a fixed capacity and is already full.");
		}
	}
}
