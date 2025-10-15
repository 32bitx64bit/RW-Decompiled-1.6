using System;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Jobs;

namespace Gilzoide.ManagedJobs;

public struct ManagedJobParallelForTransform : IJobParallelForTransform, IDisposable
{
	private GCHandle _managedJobGcHandle;

	public IJobParallelForTransform Job
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (!_managedJobGcHandle.IsAllocated)
			{
				return null;
			}
			return (IJobParallelForTransform)_managedJobGcHandle.Target;
		}
	}

	public bool HasJob => Job != null;

	public ManagedJobParallelForTransform(IJobParallelForTransform managedJob)
	{
		_managedJobGcHandle = ((managedJob != null) ? GCHandle.Alloc(managedJob) : default(GCHandle));
	}

	public void Execute(int index, TransformAccess transform)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		IJobParallelForTransform job = Job;
		if (job != null)
		{
			job.Execute(index, transform);
		}
	}

	public void Dispose()
	{
		if (_managedJobGcHandle.IsAllocated)
		{
			_managedJobGcHandle.Free();
		}
	}

	public JobHandle Schedule(TransformAccessArray transforms, JobHandle dependsOn = default(JobHandle))
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = IJobParallelForTransformExtensions.Schedule<ManagedJobParallelForTransform>(this, transforms, dependsOn);
		IJobExtensions.Schedule<DisposeJob<ManagedJobParallelForTransform>>(new DisposeJob<ManagedJobParallelForTransform>(this), val);
		return val;
	}
}
