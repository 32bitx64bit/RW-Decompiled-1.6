using UnityEngine;
using Verse;

namespace RimWorld;

public class Beam : Bullet
{
	public override Vector3 ExactPosition => destination + Vector3.up * def.Altitude;

	public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
		Vector3 val = (ExactPosition - launcher.Position.ToVector3Shifted()).Yto0();
		Vector3 offsetA = ((Vector3)(ref val)).normalized * def.projectile.beamStartOffset;
		if (def.projectile.beamMoteDef != null)
		{
			MoteMaker.MakeInteractionOverlay(def.projectile.beamMoteDef, launcher, usedTarget.ToTargetInfo(base.Map), offsetA, Vector3.zero);
		}
		ImpactSomething();
	}
}
