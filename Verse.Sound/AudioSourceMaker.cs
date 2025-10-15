using UnityEngine;

namespace Verse.Sound;

public static class AudioSourceMaker
{
	private const AudioRolloffMode WorldRolloffMode = 1;

	public static AudioSource NewAudioSourceOn(GameObject go)
	{
		if ((Object)(object)go.GetComponent<AudioSource>() != (Object)null)
		{
			Log.Warning("Adding audio source on " + ((object)go)?.ToString() + " that already has one.");
			return go.GetComponent<AudioSource>();
		}
		AudioSource obj = go.AddComponent<AudioSource>();
		obj.rolloffMode = (AudioRolloffMode)1;
		obj.dopplerLevel = 0f;
		obj.playOnAwake = false;
		return obj;
	}
}
