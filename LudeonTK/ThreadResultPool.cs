using Unity.Collections;
using Verse;

namespace LudeonTK;

public static class ThreadResultPool
{
	public static NativeArray<int> closestResults;

	public static NativeArray<float> closestSqrDists;

	private static bool allocated;

	public static void EnsureReady()
	{
		EnsureAllocated();
		Reset();
	}

	private static void EnsureAllocated()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!allocated)
		{
			allocated = true;
			UnityData.DisposeStatic += Dispose;
			closestResults = new NativeArray<int>(128, (Allocator)4, (NativeArrayOptions)0);
			closestSqrDists = new NativeArray<float>(128, (Allocator)4, (NativeArrayOptions)0);
		}
	}

	private static void Reset()
	{
		if (closestResults.IsCreated)
		{
			for (int i = 0; i < closestResults.Length; i++)
			{
				closestResults[i] = -1;
				closestSqrDists[i] = float.MaxValue;
			}
		}
	}

	private static void Dispose()
	{
		if (allocated)
		{
			closestResults.Dispose();
			closestSqrDists.Dispose();
			allocated = false;
		}
	}
}
