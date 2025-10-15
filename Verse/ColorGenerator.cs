using UnityEngine;

namespace Verse;

public abstract class ColorGenerator
{
	public virtual Color ExemplaryColor
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Rand.PushState(764543439);
			Color result = NewRandomizedColor();
			Rand.PopState();
			return result;
		}
	}

	public abstract Color NewRandomizedColor();
}
