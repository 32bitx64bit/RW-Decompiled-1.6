using System;
using UnityEngine;

namespace Verse.Sound;

public class SoundParamTarget_PropertyEcho : SoundParamTarget
{
	private EchoFilterProperty filterProperty;

	public override string Label => "EchoFilter-" + filterProperty;

	public override Type NeededFilterType => typeof(SoundFilterEcho);

	public override void SetOn(Sample sample, float value)
	{
		AudioEchoFilter val = ((Component)sample.source).GetComponent<AudioEchoFilter>();
		if ((Object)(object)val == (Object)null)
		{
			val = ((Component)sample.source).gameObject.AddComponent<AudioEchoFilter>();
		}
		if (filterProperty == EchoFilterProperty.Delay)
		{
			val.delay = value;
		}
		if (filterProperty == EchoFilterProperty.DecayRatio)
		{
			val.decayRatio = value;
		}
		if (filterProperty == EchoFilterProperty.WetMix)
		{
			val.wetMix = value;
		}
		if (filterProperty == EchoFilterProperty.DryMix)
		{
			val.dryMix = value;
		}
	}
}
