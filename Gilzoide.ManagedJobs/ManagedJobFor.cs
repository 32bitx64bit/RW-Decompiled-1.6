using System;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs;

public struct ManagedJobFor : IJobFor, IDisposable
{
	private GCHandle _managedJobGcHandle;

	public IJobFor Job
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (!_managedJobGcHandle.IsAllocated)
			{
				return null;
			}
			return (IJobFor)_managedJobGcHandle.Target;
		}
	}

	public bool HasJob => Job != null;

	public ManagedJobFor(IJobFor managedJob)
	{
		_managedJobGcHandle = ((managedJob != null) ? GCHandle.Alloc(managedJob) : default(GCHandle));
	}

	public void Execute(int index)
	{
		IJobFor job = Job;
		if (job != null)
		{
			job.Execute(index);
		}
	}

	public void Dispose()
	{
		if (_managedJobGcHandle.IsAllocated)
		{
			_managedJobGcHandle.Free();
		}
	}

	public JobHandle Schedule(int arrayLength, JobHandle dependsOn = default(JobHandle))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobForExtensions.Schedule<ManagedJobFor>(this, arrayLength, dependsOn);
		IJobExtensions.Schedule<DisposeJob<ManagedJobFor>>(new DisposeJob<ManagedJobFor>(this), val);
		return val;
	}

	public JobHandle ScheduleParallel(int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle))
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobForExtensions.ScheduleParallel<ManagedJobFor>(this, arrayLength, innerloopBatchCount, dependsOn);
		IJobExtensions.Schedule<DisposeJob<ManagedJobFor>>(new DisposeJob<ManagedJobFor>(this), val);
		return val;
	}

	public void Run(int arrayLength)
	{
		try
		{
			IJobForExtensions.Run<ManagedJobFor>(this, arrayLength);
		}
		finally
		{
			Dispose();
		}
	}
}
