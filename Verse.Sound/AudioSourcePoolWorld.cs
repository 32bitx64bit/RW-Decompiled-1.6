using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound;

public class AudioSourcePoolWorld
{
	private List<AudioSource> sourcesWorld = new List<AudioSource>();

	private const int NumSourcesWorld = 32;

	public AudioSourcePoolWorld()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("OneShotSourcesWorldContainer");
		val.transform.position = Vector3.zero;
		for (int i = 0; i < 32; i++)
		{
			GameObject val2 = new GameObject("OneShotSource_" + i);
			val2.transform.parent = val.transform;
			val2.transform.localPosition = Vector3.zero;
			sourcesWorld.Add(AudioSourceMaker.NewAudioSourceOn(val2));
		}
	}

	public AudioSource GetSourceWorld()
	{
		foreach (AudioSource item in sourcesWorld)
		{
			if (!item.isPlaying)
			{
				SoundFilterUtility.DisableAllFiltersOn(item);
				return item;
			}
		}
		return null;
	}
}
