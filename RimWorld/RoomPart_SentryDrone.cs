using Verse;

namespace RimWorld;

public class RoomPart_SentryDrone : RoomPartWorker
{
	public RoomPart_SentryDrone(RoomPartDef def)
		: base(def)
	{
	}

	public override void FillRoom(Map map, LayoutRoom room, Faction faction, float threatPoints)
	{
		if (ModsConfig.OdysseyActive)
		{
			if (!room.TryGetRandomCellInRoom(map, out var cell2, 0, 0, Validator))
			{
				Log.Error("Failed to find cell to spawn sentry drone.");
			}
			else
			{
				GenSpawn.Spawn(PawnGenerator.GeneratePawn(PawnKindDefOf.Drone_Sentry, faction), cell2, map);
			}
		}
		bool Validator(IntVec3 cell)
		{
			if (cell.Standable(map))
			{
				return cell.GetThingList(map).Count == 0;
			}
			return false;
		}
	}
}
