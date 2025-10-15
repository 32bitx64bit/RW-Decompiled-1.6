using System;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs;

public struct ManagedJob : IJob, IDisposable
{
	private GCHandle _managedJobGcHandle;

	public IJob Job
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (!_managedJobGcHandle.IsAllocated)
			{
				return null;
			}
			return (IJob)_managedJobGcHandle.Target;
		}
	}

	public bool HasJob => Job != null;

	public ManagedJob(IJob managedJob)
	{
		_managedJobGcHandle = ((managedJob != null) ? GCHandle.Alloc(managedJob) : default(GCHandle));
	}

	public void Execute()
	{
		IJob job = Job;
		if (job != null)
		{
			job.Execute();
		}
	}

	public void Dispose()
	{
		if (_managedJobGcHandle.IsAllocated)
		{
			_managedJobGcHandle.Free();
		}
	}

	public JobHandle Schedule(JobHandle dependsOn = default(JobHandle))
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobExtensions.Schedule<ManagedJob>(this, dependsOn);
		IJobExtensions.Schedule<DisposeJob<ManagedJob>>(new DisposeJob<ManagedJob>(this), val);
		return val;
	}

	public void Run()
	{
		try
		{
			IJobExtensions.Run<ManagedJob>(this);
		}
		finally
		{
			Dispose();
		}
	}
}
