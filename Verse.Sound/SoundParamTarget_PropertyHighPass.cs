using System;
using UnityEngine;

namespace Verse.Sound;

public class SoundParamTarget_PropertyHighPass : SoundParamTarget
{
	private HighPassFilterProperty filterProperty;

	public override string Label => "HighPassFilter-" + filterProperty;

	public override Type NeededFilterType => typeof(SoundFilterHighPass);

	public override void SetOn(Sample sample, float value)
	{
		AudioHighPassFilter val = ((Component)sample.source).GetComponent<AudioHighPassFilter>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)sample.source).gameObject.AddComponent<AudioHighPassFilter>();
		}
		if (filterProperty == HighPassFilterProperty.Cutoff)
		{
			val.cutoffFrequency = value;
		}
		if (filterProperty == HighPassFilterProperty.Resonance)
		{
			val.highpassResonanceQ = value;
		}
	}
}
