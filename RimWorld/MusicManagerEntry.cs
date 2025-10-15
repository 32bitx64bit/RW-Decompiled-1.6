using UnityEngine;
using Verse;

namespace RimWorld;

public class MusicManagerEntry
{
	private AudioSource audioSource;

	private int silentTillFrame = -1;

	private float silenceMultiplier = 1f;

	private const string SourceGameObjectName = "MusicAudioSourceDummy";

	private const float SilenceMultiplierChangePerSecond = 1.75f;

	private float CurVolume => Prefs.VolumeMusic * SongDefOf.EntrySong.volume;

	public float CurSanitizedVolume => AudioSourceUtility.GetSanitizedVolume(CurVolume, "MusicManagerEntry");

	public void MusicManagerEntryUpdate()
	{
		if ((Object)(object)audioSource == (Object)null || !audioSource.isPlaying)
		{
			StartPlaying();
		}
		float curSanitizedVolume = CurSanitizedVolume;
		if (Time.frameCount > silentTillFrame)
		{
			silenceMultiplier = Mathf.Clamp01(silenceMultiplier + 1.75f * Time.deltaTime);
		}
		else if (Time.frameCount <= silentTillFrame)
		{
			silenceMultiplier = Mathf.Clamp01(silenceMultiplier - 1.75f * Time.deltaTime);
		}
		audioSource.volume = curSanitizedVolume * silenceMultiplier;
	}

	private void StartPlaying()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		if ((Object)(object)audioSource != (Object)null && !audioSource.isPlaying)
		{
			audioSource.Play();
			return;
		}
		if ((Object)(object)GameObject.Find("MusicAudioSourceDummy") != (Object)null)
		{
			Log.Error("MusicManagerEntry did StartPlaying but there is already a music source GameObject.");
			return;
		}
		GameObject val = new GameObject("MusicAudioSourceDummy");
		val.transform.parent = ((Component)Camera.main).transform;
		audioSource = val.AddComponent<AudioSource>();
		audioSource.bypassEffects = true;
		audioSource.bypassListenerEffects = true;
		audioSource.bypassReverbZones = true;
		audioSource.priority = 0;
		audioSource.clip = SongDefOf.EntrySong.clip;
		audioSource.volume = CurSanitizedVolume;
		audioSource.loop = true;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
	}

	public void MaintainSilence()
	{
		silentTillFrame = Time.frameCount + 1;
	}
}
