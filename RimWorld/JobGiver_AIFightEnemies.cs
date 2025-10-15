using Verse;
using Verse.AI;

namespace RimWorld;

public class JobGiver_AIFightEnemies : JobGiver_AIFightEnemy
{
	protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
	{
		Thing enemyTarget = pawn.mindState.enemyTarget;
		bool allowManualCastWeapons = !pawn.IsColonist && !pawn.IsColonySubhuman;
		Verb verb = verbToUse ?? pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons, allowTurrets);
		if (verb == null)
		{
			dest = IntVec3.Invalid;
			return false;
		}
		CastPositionRequest newReq = default(CastPositionRequest);
		newReq.caster = pawn;
		newReq.target = enemyTarget;
		newReq.verb = verb;
		newReq.maxRangeFromTarget = verb.EffectiveRange;
		newReq.wantCoverFromTarget = verb.EffectiveRange > 5f;
		return CastPositionFinder.TryFindCastPosition(newReq, out dest);
	}
}
