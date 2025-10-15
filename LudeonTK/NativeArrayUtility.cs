using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace LudeonTK;

public static class NativeArrayUtility
{
	private static class ArrayAccessor<T>
	{
		public static readonly Func<List<T>, T[]> Getter;

		static ArrayAccessor()
		{
			DynamicMethod dynamicMethod = new DynamicMethod("get", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T[]), new Type[1] { typeof(List<T>) }, typeof(ArrayAccessor<T>), skipVisibility: true);
			ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldfld, typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic));
			iLGenerator.Emit(OpCodes.Ret);
			Getter = (Func<List<T>, T[]>)dynamicMethod.CreateDelegate(typeof(Func<List<T>, T[]>));
		}
	}

	public static void EnsureDisposed<T>(this ref NativeArray<T> array) where T : unmanaged
	{
		if (array.IsCreated)
		{
			array.Dispose();
		}
	}

	public static void EnsureDisposed<T>(this ref NativeList<T> array) where T : unmanaged
	{
		if (array.IsCreated)
		{
			array.Dispose();
		}
	}

	public static void EnsureDisposed(this ref NativeBitArray array)
	{
		if (((NativeBitArray)(ref array)).IsCreated)
		{
			((NativeBitArray)(ref array)).Dispose();
		}
	}

	public static void EnsureDisposed<TElement, TPriority, TComparer>(this ref NativePriorityQueue<TElement, TPriority, TComparer> array) where TElement : unmanaged where TPriority : unmanaged where TComparer : unmanaged, IComparer<TPriority>
	{
		if (array.IsCreated)
		{
			array.Dispose();
		}
	}

	public static void EnsureDisposed<T>(this ref UnsafeList<T> list) where T : unmanaged
	{
		if (list.IsCreated)
		{
			list.Dispose();
		}
	}

	public static void EnsureDisposed<T, U>(this ref UnsafeHeap<T, U> list) where T : unmanaged where U : unmanaged, IComparer<T>
	{
		if (list.IsCreated)
		{
			list.Dispose();
		}
	}

	public static void EnsureDisposed(this ref UnsafeBitArray bitArray)
	{
		if (((UnsafeBitArray)(ref bitArray)).IsCreated)
		{
			((UnsafeBitArray)(ref bitArray)).Dispose();
		}
	}

	public unsafe static void MemClear<T>(NativeArray<T> array) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		UnsafeUtility.MemClear(NativeArrayUnsafeUtility.GetUnsafePtr<T>(array), (long)array.Length * (long)sizeof(T));
	}

	public unsafe static void MemClear<T>(UnsafeList<T> list) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		UnsafeUtility.MemClear((void*)list.Ptr, (long)list.Length * (long)sizeof(T));
	}

	public unsafe static void MemClear<T>(NativeList<T> list) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		UnsafeUtility.MemClear((void*)NativeListUnsafeUtility.GetUnsafePtr<T>(list), (long)list.Length * (long)sizeof(T));
	}

	public static void Clear<T>(this NativeArray<T> array) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MemClear<T>(array);
	}

	public static void Clear<T>(this NativeList<T> array) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MemClear<T>(array);
	}

	public static void Clear<T>(this UnsafeList<T> array) where T : unmanaged
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		MemClear<T>(array);
	}

	public static T[] GetInternalArray<T>(List<T> list)
	{
		return ArrayAccessor<T>.Getter(list);
	}

	public unsafe static NativeArray<T> GetNativeArrayCopy<T>(List<T> source, Allocator allocator, bool clearMemory = false) where T : unmanaged
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		T[] internalArray = GetInternalArray(source);
		NativeArray<T> val = default(NativeArray<T>);
		val._002Ector(source.Count, allocator, (NativeArrayOptions)(clearMemory ? 1 : 0));
		fixed (T* ptr = internalArray)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(val), ptr2, (long)source.Count * (long)UnsafeUtility.SizeOf<T>());
		}
		return val;
	}

	public unsafe static NativeArray<T> GetNativeArrayCopy<T>(T[] source, Allocator allocator, bool clearMemory = false) where T : unmanaged
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<T> val = new NativeArray<T>(source.Length, allocator, (NativeArrayOptions)(clearMemory ? 1 : 0));
		fixed (T* ptr = source)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(val), ptr2, (long)source.Length * (long)UnsafeUtility.SizeOf<T>());
		}
		return val;
	}

	public unsafe static void CopyNativeToArray<T>(T[] destination, NativeArray<T> source) where T : unmanaged
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (source.Length != destination.Length)
		{
			throw new Exception($"Attempted to copy a native array of length {source.Length} into an array with length {destination.Length}");
		}
		fixed (T* ptr = destination)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(ptr2, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(source), (long)destination.Length * (long)UnsafeUtility.SizeOf<T>());
		}
	}

	public unsafe static void CopyUnsafeListToArray<T>(T[] destination, UnsafeList<T> source) where T : unmanaged
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (source.Length != destination.Length)
		{
			throw new Exception($"Attempted to copy a unsafe array of length {source.Length} into an array with length {destination.Length}");
		}
		fixed (T* ptr = destination)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(ptr2, (void*)source.Ptr, (long)destination.Length * (long)UnsafeUtility.SizeOf<T>());
		}
	}

	public static void CopyNativeListToList<T>(List<T> destination, NativeList<T> source) where T : unmanaged
	{
		destination.Clear();
		destination.Capacity = Mathf.Max(source.Length, destination.Capacity);
		for (int i = 0; i < source.Length; i++)
		{
			destination.Add(source[i]);
		}
	}

	public static void CopyUnsafeListToList<T>(List<T> destination, UnsafeList<T> source) where T : unmanaged
	{
		destination.Clear();
		destination.Capacity = Mathf.Max(destination.Count, destination.Capacity);
		for (int i = 0; i < source.Length; i++)
		{
			destination.Add(source[i]);
		}
	}

	public static void CopyArrayToList<T>(List<T> destination, NativeArray<T> source) where T : unmanaged
	{
		destination.Clear();
		destination.Capacity = Mathf.Max(destination.Count, destination.Capacity);
		for (int i = 0; i < source.Length; i++)
		{
			destination.Add(source[i]);
		}
	}

	public unsafe static void CopyArrayToNative<T>(NativeArray<T> destination, T[] source) where T : unmanaged
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (source.Length != destination.Length)
		{
			throw new Exception($"Attempted to copy an array of length {source.Length} into a native array with length {destination.Length}");
		}
		fixed (T* ptr = source)
		{
			void* ptr2 = ptr;
			UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<T>(destination), ptr2, (long)source.Length * (long)UnsafeUtility.SizeOf<T>());
		}
	}

	public static void CopyFrom(this NativeBitArray destination, NativeBitArray source)
	{
		((NativeBitArray)(ref destination)).Copy(0, ref source, 0, Mathf.Min(((NativeBitArray)(ref destination)).Length, ((NativeBitArray)(ref source)).Length));
	}

	public static void Reverse<T>(this NativeList<T> list) where T : unmanaged
	{
		int length = list.Length;
		int num = 0;
		int num2 = length - 1;
		while (num < num2)
		{
			int num3 = num;
			int num4 = num2;
			T val = list[num2];
			T val2 = list[num];
			T val4 = (list[num3] = val);
			val4 = (list[num4] = val2);
			num++;
			num2--;
		}
	}

	public static NativeArray<T> EmptyArray<T>() where T : unmanaged
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return new NativeArray<T>(0, (Allocator)4, (NativeArrayOptions)1);
	}

	public static NativeList<T> EmptyList<T>() where T : unmanaged
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new NativeList<T>(0, AllocatorHandle.op_Implicit((Allocator)4));
	}

	public static NativeBitArray EmptyBitArray()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new NativeBitArray(0, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
	}
}
