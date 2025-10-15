using System;
using LudeonTK;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Verse.Glow;

public struct LocalGlowArea : IDisposable
{
	public bool inUse;

	public UnsafeList<Color32> colors;

	public static LocalGlowArea AllocateNew()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		LocalGlowArea localGlowArea = default(LocalGlowArea);
		localGlowArea.inUse = false;
		localGlowArea.colors = new UnsafeList<Color32>(6561, AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		LocalGlowArea result = localGlowArea;
		result.colors.Resize(6561, (NativeArrayOptions)1);
		return result;
	}

	public void Take()
	{
		inUse = true;
	}

	public void Return()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NativeArrayUtility.MemClear<Color32>(colors);
		inUse = false;
	}

	public void Dispose()
	{
		colors.EnsureDisposed<Color32>();
	}
}
