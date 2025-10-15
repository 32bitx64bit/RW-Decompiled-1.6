using Unity.Profiling;
using UnityEngine;

namespace Verse;

public static class MemoryUsageUtility
{
	private static ProfilerRecorder systemMemoryRecorder;

	private static ProfilerRecorder totalMemoryRecorder;

	private static ProfilerRecorder gfxMemoryRecorder;

	private static ProfilerRecorder gcMemoryRecorder;

	private static ProfilerRecorder gcReservedMemoryRecorder;

	private static ProfilerRecorder audioMemoryRecorder;

	public static long TrackedMemoryUsageBytes => ((ProfilerRecorder)(ref totalMemoryRecorder)).CurrentValue;

	public static long OsMemoryUsageBytes => ((ProfilerRecorder)(ref systemMemoryRecorder)).CurrentValue;

	public static long GraphicsMemoryUsageBytes => ((ProfilerRecorder)(ref gfxMemoryRecorder)).CurrentValue;

	public static long ManagedMemoryUsageBytes => ((ProfilerRecorder)(ref gcMemoryRecorder)).CurrentValue;

	public static long ManagedMemoryReservedBytes => ((ProfilerRecorder)(ref gcReservedMemoryRecorder)).CurrentValue;

	public static long AudioMemoryUsageBytes => ((ProfilerRecorder)(ref audioMemoryRecorder)).CurrentValue;

	public static long TextureMemoryUsageBytes => (long)Texture.currentTextureMemory;

	static MemoryUsageUtility()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory", 1, (ProfilerRecorderOptions)24);
		totalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 1, (ProfilerRecorderOptions)24);
		gfxMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx Used Memory", 1, (ProfilerRecorderOptions)24);
		gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 1, (ProfilerRecorderOptions)24);
		gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory", 1, (ProfilerRecorderOptions)24);
		audioMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Audio Used Memory", 1, (ProfilerRecorderOptions)24);
		((ProfilerRecorder)(ref systemMemoryRecorder)).Stop();
		((ProfilerRecorder)(ref totalMemoryRecorder)).Stop();
		((ProfilerRecorder)(ref gfxMemoryRecorder)).Stop();
		((ProfilerRecorder)(ref gcMemoryRecorder)).Stop();
		((ProfilerRecorder)(ref gcReservedMemoryRecorder)).Stop();
		((ProfilerRecorder)(ref audioMemoryRecorder)).Stop();
	}

	public static void SetShouldRecord(bool shouldRecord)
	{
		if (shouldRecord)
		{
			((ProfilerRecorder)(ref systemMemoryRecorder)).Start();
			((ProfilerRecorder)(ref totalMemoryRecorder)).Start();
			((ProfilerRecorder)(ref gfxMemoryRecorder)).Start();
			((ProfilerRecorder)(ref gcMemoryRecorder)).Start();
			((ProfilerRecorder)(ref gcReservedMemoryRecorder)).Start();
			((ProfilerRecorder)(ref audioMemoryRecorder)).Start();
		}
		else
		{
			((ProfilerRecorder)(ref systemMemoryRecorder)).Stop();
			((ProfilerRecorder)(ref totalMemoryRecorder)).Stop();
			((ProfilerRecorder)(ref gfxMemoryRecorder)).Stop();
			((ProfilerRecorder)(ref gcMemoryRecorder)).Stop();
			((ProfilerRecorder)(ref gcReservedMemoryRecorder)).Stop();
			((ProfilerRecorder)(ref audioMemoryRecorder)).Stop();
		}
	}
}
