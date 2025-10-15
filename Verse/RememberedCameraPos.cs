using UnityEngine;

namespace Verse;

public class RememberedCameraPos : IExposable
{
	public Vector3 rootPos;

	public float rootSize;

	public RememberedCameraPos(Map map)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		rootPos = map.Center.ToVector3Shifted();
		rootSize = 24f;
	}

	public void ExposeData()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Scribe_Values.Look(ref rootPos, "rootPos");
		Scribe_Values.Look(ref rootSize, "rootSize", 0f);
	}
}
