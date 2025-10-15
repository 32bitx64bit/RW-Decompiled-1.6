using UnityEngine;

namespace Verse.Noise;

public class DistanceFromPlanetViewCenter : ModuleBase
{
	public Vector3 viewCenter;

	public float viewAngle;

	public bool invert;

	public DistanceFromPlanetViewCenter()
		: base(0)
	{
	}

	public DistanceFromPlanetViewCenter(Vector3 viewCenter, float viewAngle, bool invert = false)
		: base(0)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.viewCenter = viewCenter;
		this.viewAngle = viewAngle;
		this.invert = invert;
	}

	public override double GetValue(double x, double y, double z)
	{
		float valueInt = GetValueInt(x, y, z);
		if (invert)
		{
			return 1f - valueInt;
		}
		return valueInt;
	}

	private float GetValueInt(double x, double y, double z)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (viewAngle >= 180f)
		{
			return 0f;
		}
		return Mathf.Min(Vector3.Angle(viewCenter, new Vector3((float)x, (float)y, (float)z)) / viewAngle, 1f);
	}
}
