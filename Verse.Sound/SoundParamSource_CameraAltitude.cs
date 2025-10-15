using UnityEngine;

namespace Verse.Sound;

public class SoundParamSource_CameraAltitude : SoundParamSource
{
	public override string Label => "Camera altitude";

	public override float ValueFor(Sample samp)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)Find.Camera).transform.position.y;
	}
}
