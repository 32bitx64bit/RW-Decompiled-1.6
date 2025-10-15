using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound;

public class AudioSourcePoolCamera
{
	public GameObject cameraSourcesContainer;

	private List<AudioSource> sourcesCamera = new List<AudioSource>();

	private const int NumSourcesCamera = 16;

	public AudioSourcePoolCamera()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		cameraSourcesContainer = new GameObject("OneShotSourcesCameraContainer");
		cameraSourcesContainer.transform.parent = ((Component)Find.Camera).transform;
		cameraSourcesContainer.transform.localPosition = Vector3.zero;
		for (int i = 0; i < 16; i++)
		{
			GameObject val = new GameObject("OneShotSourceCamera_" + i);
			val.transform.parent = cameraSourcesContainer.transform;
			val.transform.localPosition = Vector3.zero;
			AudioSource val2 = AudioSourceMaker.NewAudioSourceOn(val);
			val2.bypassReverbZones = true;
			sourcesCamera.Add(val2);
		}
	}

	public AudioSource GetSourceCamera()
	{
		for (int i = 0; i < sourcesCamera.Count; i++)
		{
			AudioSource val = sourcesCamera[i];
			if (!val.isPlaying)
			{
				val.clip = null;
				SoundFilterUtility.DisableAllFiltersOn(val);
				return val;
			}
		}
		return null;
	}
}
