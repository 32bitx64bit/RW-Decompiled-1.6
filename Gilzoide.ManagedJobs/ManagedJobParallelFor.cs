using System;
using System.Runtime.InteropServices;
using Unity.Jobs;

namespace Gilzoide.ManagedJobs;

public struct ManagedJobParallelFor : IJobParallelFor, IDisposable
{
	private GCHandle _managedJobGcHandle;

	public IJobParallelFor Job
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (!_managedJobGcHandle.IsAllocated)
			{
				return null;
			}
			return (IJobParallelFor)_managedJobGcHandle.Target;
		}
	}

	public bool HasJob => Job != null;

	public ManagedJobParallelFor(IJobParallelFor managedJob)
	{
		_managedJobGcHandle = ((managedJob != null) ? GCHandle.Alloc(managedJob) : default(GCHandle));
	}

	public void Execute(int index)
	{
		IJobParallelFor job = Job;
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

	public JobHandle Schedule(int arrayLength, int innerloopBatchCount, JobHandle dependsOn = default(JobHandle))
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobParallelForExtensions.Schedule<ManagedJobParallelFor>(this, arrayLength, innerloopBatchCount, dependsOn);
		IJobExtensions.Schedule<DisposeJob<ManagedJobParallelFor>>(new DisposeJob<ManagedJobParallelFor>(this), val);
		return val;
	}

	public void Run(int arrayLength)
	{
		try
		{
			IJobParallelForExtensions.Run<ManagedJobParallelFor>(this, arrayLength);
		}
		finally
		{
			Dispose();
		}
	}
}
