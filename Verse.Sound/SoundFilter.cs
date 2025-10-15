using UnityEngine;

namespace Verse.Sound;

public abstract class SoundFilter
{
	public abstract void SetupOn(AudioSource source);

	protected static T GetOrMakeFilterOn<T>(AudioSource source) where T : Behaviour
	{
		T val = ((Component)source).gameObject.GetComponent<T>();
		if ((Object)(object)val != (Object)null)
		{
			((Behaviour)val).enabled = true;
		}
		else
		{
			val = ((Component)source).gameObject.AddComponent<T>();
		}
		return val;
	}
}
