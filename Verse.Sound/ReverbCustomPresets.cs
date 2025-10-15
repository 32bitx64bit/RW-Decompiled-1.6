using UnityEngine;

namespace Verse.Sound;

public static class ReverbCustomPresets
{
	public static void SetupAs(this ReverbSetup setup, AudioReverbPreset preset)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("OneFrameTemp");
		val.AddComponent<AudioSource>();
		AudioReverbFilter val2 = val.AddComponent<AudioReverbFilter>();
		val2.reverbPreset = preset;
		setup.dryLevel = val2.dryLevel;
		setup.room = val2.room;
		setup.roomHF = val2.roomHF;
		setup.roomLF = val2.roomLF;
		setup.decayTime = val2.decayTime;
		setup.decayHFRatio = val2.decayHFRatio;
		setup.reflectionsLevel = val2.reflectionsLevel;
		setup.reflectionsDelay = val2.reflectionsDelay;
		setup.reverbLevel = val2.reverbLevel;
		setup.reverbDelay = val2.reverbDelay;
		setup.hfReference = val2.hfReference;
		setup.lfReference = val2.lfReference;
		setup.diffusion = val2.diffusion;
		setup.density = val2.density;
		Object.Destroy((Object)(object)val2);
	}
}
