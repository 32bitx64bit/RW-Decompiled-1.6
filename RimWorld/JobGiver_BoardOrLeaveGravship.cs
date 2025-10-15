using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld;

public class JobGiver_BoardOrLeaveGravship : ThinkNode_JobGiver
{
	protected override Job TryGiveJob(Pawn pawn)
	{
		if (pawn.Downed)
		{
			return null;
		}
		Building_GravEngine playerGravEngine_NewTemp = GravshipUtility.GetPlayerGravEngine_NewTemp(pawn.Map);
		if (playerGravEngine_NewTemp == null)
		{
			return null;
		}
		if (playerGravEngine_NewTemp.pawnsToBoard != null && playerGravEngine_NewTemp.pawnsToBoard.Contains(pawn))
		{
			IntVec3 spot;
			if (GravshipUtility.IsOnboardGravship_NewTemp(pawn.Position, playerGravEngine_NewTemp, pawn, desperate: false, respectAllowedAreas: false))
			{
				spot = pawn.Position;
			}
			else if (!GravshipUtility.TryFindSpotOnGravship(pawn, playerGravEngine_NewTemp, out spot))
			{
				Messages.Message("FailedToBoardGravship".Translate(pawn.Named("PAWN"), playerGravEngine_NewTemp.RenamableLabel), pawn, MessageTypeDefOf.NegativeEvent, historical: false);
				playerGravEngine_NewTemp.pawnsToBoard.Remove(pawn);
				return null;
			}
			if (pawn.lord?.LordJob is LordJob_Ritual lordJob_Ritual)
			{
				lordJob_Ritual.Cancel();
			}
			Job job = JobMaker.MakeJob(JobDefOf.GotoShip, spot);
			job.locomotionUrgency = LocomotionUrgency.Jog;
			return job;
		}
		if (playerGravEngine_NewTemp.pawnsToLeave != null && playerGravEngine_NewTemp.pawnsToLeave.Contains(pawn))
		{
			IntVec3 spot2;
			if (!GravshipUtility.IsOnboardGravship_NewTemp(pawn.Position, playerGravEngine_NewTemp, pawn, desperate: true, respectAllowedAreas: false))
			{
				spot2 = pawn.Position;
			}
			else if (!GravshipUtility.TryFindSpotOffGravship(pawn, playerGravEngine_NewTemp, out spot2))
			{
				Messages.Message("FailedToLeaveGravship".Translate(pawn.Named("PAWN"), playerGravEngine_NewTemp.RenamableLabel), pawn, MessageTypeDefOf.NegativeEvent, historical: false);
				playerGravEngine_NewTemp.pawnsToLeave.Remove(pawn);
				return null;
			}
			pawn.lord?.LordJob?.Notify_PawnLost(pawn, PawnLostCondition.ForcedByPlayerAction);
			Job job2 = JobMaker.MakeJob(JobDefOf.LeaveShip, spot2);
			job2.locomotionUrgency = LocomotionUrgency.Jog;
			return job2;
		}
		return null;
	}
}
