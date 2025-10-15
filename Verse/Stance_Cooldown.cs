using UnityEngine;

namespace Verse;

public class Stance_Cooldown : Stance_Busy
{
	private const float RadiusPerTick = 0.002f;

	private const float MaxRadius = 0.5f;

	public Stance_Cooldown()
	{
	}

	public Stance_Cooldown(int ticks, LocalTargetInfo focusTarg, Verb verb)
		: base(ticks, focusTarg, verb)
	{
	}

	public override void StanceDraw()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (Find.Selector.IsSelected(stanceTracker.pawn))
		{
			float radius = Mathf.Min(0.5f, (float)ticksLeft * 0.002f);
			GenDraw.DrawCooldownCircle(stanceTracker.pawn.Drawer.DrawPos + new Vector3(0f, 0.2f, 0f), radius);
		}
	}
}
