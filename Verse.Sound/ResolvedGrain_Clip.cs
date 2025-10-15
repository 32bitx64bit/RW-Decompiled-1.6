using UnityEngine;

namespace Verse.Sound;

public class ResolvedGrain_Clip : ResolvedGrain
{
	public AudioClip clip;

	public ResolvedGrain_Clip(AudioClip clip)
	{
		this.clip = clip;
		duration = clip.length;
	}

	public override string ToString()
	{
		return "Clip:" + ((Object)clip).name;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is ResolvedGrain_Clip resolvedGrain_Clip))
		{
			return false;
		}
		return (Object)(object)resolvedGrain_Clip.clip == (Object)(object)clip;
	}

	public override int GetHashCode()
	{
		if ((Object)(object)clip == (Object)null)
		{
			return 0;
		}
		return ((object)clip).GetHashCode();
	}
}
