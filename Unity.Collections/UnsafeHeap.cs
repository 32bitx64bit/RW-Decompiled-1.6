using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(NativeHeapDebugView<, >))]
public struct UnsafeHeap<T, U> : IDisposable where T : unmanaged where U : unmanaged, IComparer<T>
{
	public const int DEFAULT_CAPACITY = 128;

	internal const int VALIDATION_ERROR_WRONG_INSTANCE = 1;

	internal const int VALIDATION_ERROR_INVALID = 2;

	internal const int VALIDATION_ERROR_REMOVED = 3;

	[NativeDisableUnsafePtrRestriction]
	internal unsafe HeapData<T, U>* Data;

	internal Allocator Allocator;

	public unsafe int Capacity
	{
		get
		{
			return Data->Capacity;
		}
		set
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			TableValue* ptr = (TableValue*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<TableValue>() * value), UnsafeUtility.AlignOf<TableValue>(), Allocator);
			HeapNode<T>* ptr2 = (HeapNode<T>*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<HeapNode<T>>() * value), UnsafeUtility.AlignOf<HeapNode<T>>(), Allocator);
			int num = ((Data->Capacity < value) ? Data->Capacity : value);
			UnsafeUtility.MemCpy((void*)ptr, (void*)Data->Table, (long)(num * UnsafeUtility.SizeOf<TableValue>()));
			UnsafeUtility.MemCpy((void*)ptr2, (void*)Data->Heap, (long)(num * UnsafeUtility.SizeOf<HeapNode<T>>()));
			for (int i = 0; i < value - Data->Capacity; i++)
			{
				ptr2[i + Data->Capacity] = new HeapNode<T>
				{
					TableIndex = i + Data->Capacity
				};
			}
			UnsafeUtility.Free((void*)Data->Table, Allocator);
			UnsafeUtility.Free((void*)Data->Heap, Allocator);
			Data->Table = ptr;
			Data->Heap = ptr2;
			Data->Capacity = value;
		}
	}

	public unsafe int Count => Data->Count;

	public unsafe U Comparator
	{
		get
		{
			return Data->Comparator;
		}
		set
		{
			Data->Comparator = value;
		}
	}

	public unsafe bool IsCreated => Data != null;

	public UnsafeHeap(Allocator allocator, int initialCapacity = 128, U comparator = default(U))
		: this(initialCapacity, comparator, allocator)
	{
	}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


	public unsafe void Dispose()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Data->Count = 0;
		Data->Capacity = 0;
		UnsafeUtility.Free((void*)Data->Heap, Allocator);
		UnsafeUtility.Free((void*)Data->Table, Allocator);
		UnsafeUtility.Free((void*)Data, Allocator);
		Data = null;
	}

	public unsafe void Clear()
	{
		Data->Count = 0;
	}

	public T Peek()
	{
		if (!TryPeek(out var t))
		{
			throw new InvalidOperationException("Cannot Peek NativeHeap when the count is zero.");
		}
		return t;
	}

	public unsafe bool TryPeek(out T t)
	{
		if (Data->Count == 0)
		{
			t = default(T);
			return false;
		}
		t = Data->Heap->Item;
		return true;
	}

	public T Pop()
	{
		if (!TryPop(out var t))
		{
			throw new InvalidOperationException("Cannot Pop NativeHeap when the count is zero.");
		}
		return t;
	}

	public unsafe bool TryPop(out T t)
	{
		if (Data->Count == 0)
		{
			t = default(T);
			return false;
		}
		HeapNode<T> heap = *Data->Heap;
		int num = --Data->Count;
		HeapNode<T> node = Data->Heap[num];
		Data->Heap[num] = heap;
		InsertAndBubbleDown(node, 0);
		t = heap.Item;
		return true;
	}

	public unsafe NativeHeapIndex Insert(in T t)
	{
		if (Data->Count == Data->Capacity)
		{
			Capacity *= 2;
		}
		HeapNode<T> node = Data->Heap[Data->Count];
		node.Item = t;
		int insertIndex = Data->Count++;
		InsertAndBubbleUp(node, insertIndex);
		NativeHeapIndex result = default(NativeHeapIndex);
		result.TableIndex = node.TableIndex;
		return result;
	}

	public unsafe T Remove(NativeHeapIndex index)
	{
		int heapIndex = Data->Table[index.TableIndex].HeapIndex;
		HeapNode<T> heapNode = Data->Heap[heapIndex];
		HeapNode<T> node = Data->Heap[--Data->Count];
		UnsafeUtility.WriteArrayElement<HeapNode<T>>((void*)Data->Heap, Data->Count, heapNode);
		if (heapIndex != 0)
		{
			int num = (heapIndex - 1) / 2;
			HeapNode<T> heapNode2 = Data->Heap[num];
			if (Data->Comparator.Compare(node.Item, heapNode2.Item) < 0)
			{
				InsertAndBubbleUp(node, heapIndex);
				return heapNode.Item;
			}
		}
		InsertAndBubbleDown(node, heapIndex);
		return heapNode.Item;
	}

	internal unsafe UnsafeHeap(int initialCapacity, U comparator, Allocator allocator)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Data = (HeapData<T, U>*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<HeapData<T, U>>(), UnsafeUtility.AlignOf<HeapData<T, U>>(), allocator);
		Data->Heap = (HeapNode<T>*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<HeapNode<T>>() * initialCapacity), UnsafeUtility.AlignOf<HeapNode<T>>(), allocator);
		Data->Table = (TableValue*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<TableValue>() * initialCapacity), UnsafeUtility.AlignOf<TableValue>(), allocator);
		Allocator = allocator;
		for (int i = 0; i < initialCapacity; i++)
		{
			Data->Heap[i] = new HeapNode<T>
			{
				TableIndex = i
			};
		}
		Data->Count = 0;
		Data->Capacity = initialCapacity;
		Data->Comparator = comparator;
	}

	internal unsafe void InsertAndBubbleDown(HeapNode<T> node, int insertIndex)
	{
		while (true)
		{
			int num = insertIndex * 2 + 1;
			int num2 = insertIndex * 2 + 2;
			if (num >= Data->Count)
			{
				break;
			}
			if (num2 >= Data->Count || Data->Comparator.Compare(Data->Heap[num].Item, Data->Heap[num2].Item) <= 0)
			{
				HeapNode<T> heapNode = Data->Heap[num];
				if (Data->Comparator.Compare(node.Item, heapNode.Item) <= 0)
				{
					break;
				}
				Data->Heap[insertIndex] = heapNode;
				Data->Table[heapNode.TableIndex].HeapIndex = insertIndex;
				insertIndex = num;
			}
			else
			{
				HeapNode<T> heapNode2 = Data->Heap[num2];
				if (Data->Comparator.Compare(node.Item, heapNode2.Item) <= 0)
				{
					break;
				}
				Data->Heap[insertIndex] = heapNode2;
				Data->Table[heapNode2.TableIndex].HeapIndex = insertIndex;
				insertIndex = num2;
			}
		}
		Data->Heap[insertIndex] = node;
		Data->Table[node.TableIndex].HeapIndex = insertIndex;
	}

	internal unsafe void InsertAndBubbleUp(HeapNode<T> node, int insertIndex)
	{
		while (insertIndex != 0)
		{
			int num = (insertIndex - 1) / 2;
			HeapNode<T> heapNode = Data->Heap[num];
			if (Data->Comparator.Compare(heapNode.Item, node.Item) <= 0)
			{
				break;
			}
			Data->Heap[insertIndex] = heapNode;
			Data->Table[heapNode.TableIndex].HeapIndex = insertIndex;
			insertIndex = num;
		}
		Data->Heap[insertIndex] = node;
		Data->Table[node.TableIndex].HeapIndex = insertIndex;
	}
}
