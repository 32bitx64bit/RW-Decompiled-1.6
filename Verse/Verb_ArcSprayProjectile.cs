namespace Verse;

public class Verb_ArcSprayProjectile : Verb_ArcSpray
{
	protected override void HitCell(IntVec3 cell)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		base.HitCell(cell);
		Map map = caster.Map;
		if (GenSight.LineOfSight(caster.Position, cell, map, skipFirstCell: true))
		{
			((Projectile)GenSpawn.Spawn(verbProps.defaultProjectile, caster.Position, map)).Launch(caster, caster.DrawPos, cell, cell, ProjectileHitFlags.IntendedTarget, preventFriendlyFire);
		}
	}
}
