using UnityEngine;

namespace RimWorld.Planet;

public class Caravan_Tweener
{
	private Caravan caravan;

	private Vector3 tweenedPos = Vector3.zero;

	private Vector3 lastTickSpringPos;

	private const float SpringTightness = 0.09f;

	public Vector3 TweenedPos => tweenedPos;

	public Vector3 LastTickTweenedVelocity => TweenedPos - lastTickSpringPos;

	public Vector3 TweenedPosRoot => CaravanTweenerUtility.PatherTweenedPosRoot(caravan) + CaravanTweenerUtility.CaravanCollisionPosOffsetFor(caravan);

	public Caravan_Tweener(Caravan caravan)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this.caravan = caravan;
	}

	public void TweenerTickInterval(int delta)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		lastTickSpringPos = tweenedPos;
		Vector3 val = TweenedPosRoot - tweenedPos;
		tweenedPos += val * 0.09f;
	}

	public void ResetTweenedPosToRoot()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		tweenedPos = TweenedPosRoot;
		lastTickSpringPos = tweenedPos;
	}
}
