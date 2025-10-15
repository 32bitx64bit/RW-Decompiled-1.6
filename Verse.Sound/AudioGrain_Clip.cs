using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound;

public class AudioGrain_Clip : AudioGrain
{
	[NoTranslate]
	public string clipPath = "";

	public override IEnumerable<ResolvedGrain> GetResolvedGrains()
	{
		AudioClip val = ContentFinder<AudioClip>.Get(clipPath);
		if ((Object)(object)val != (Object)null)
		{
			yield return new ResolvedGrain_Clip(val);
		}
		else
		{
			Log.Error("Grain couldn't resolve: Clip not found at " + clipPath);
		}
	}
}
