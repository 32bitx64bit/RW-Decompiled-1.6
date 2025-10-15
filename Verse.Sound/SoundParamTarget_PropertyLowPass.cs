using System;
using UnityEngine;

namespace Verse.Sound;

public class SoundParamTarget_PropertyLowPass : SoundParamTarget
{
	private LowPassFilterProperty filterProperty;

	public override string Label => "LowPassFilter-" + filterProperty;

	public override Type NeededFilterType => typeof(SoundFilterLowPass);

	public override void SetOn(Sample sample, float value)
	{
		AudioLowPassFilter val = ((Component)sample.source).GetComponent<AudioLowPassFilter>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)sample.source).gameObject.AddComponent<AudioLowPassFilter>();
		}
		if (filterProperty == LowPassFilterProperty.Cutoff)
		{
			val.cutoffFrequency = value;
		}
		if (filterProperty == LowPassFilterProperty.Resonance)
		{
			val.lowpassResonanceQ = value;
		}
	}
}
