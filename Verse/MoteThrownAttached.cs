using UnityEngine;

namespace Verse;

internal class MoteThrownAttached : MoteThrown
{
	private Vector3 attacheeLastPosition = new Vector3(-1000f, -1000f, -1000f);

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		base.SpawnSetup(map, respawningAfterLoad);
		if (link1.Linked)
		{
			attacheeLastPosition = link1.LastDrawPos;
		}
		exactPosition += def.mote.attachedDrawOffset;
	}

	protected override Vector3 NextExactPosition(float deltaTime)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.NextExactPosition(deltaTime);
		if (link1.Linked)
		{
			bool flag = detachAfterTicks == -1 || Find.TickManager.TicksGame - spawnTick < detachAfterTicks;
			if (!link1.Target.ThingDestroyed && flag)
			{
				link1.UpdateDrawPos();
			}
			Vector3 val2 = link1.LastDrawPos - attacheeLastPosition;
			val += val2;
			val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
			attacheeLastPosition = link1.LastDrawPos;
		}
		return val;
	}
}
