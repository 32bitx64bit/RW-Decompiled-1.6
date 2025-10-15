using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class MoteAttached : Mote
{
	private static readonly List<Vector3> animalHeadOffsets = new List<Vector3>
	{
		new Vector3(0f, 0f, 0.4f),
		new Vector3(0.4f, 0f, 0.25f),
		new Vector3(0f, 0f, 0.1f),
		new Vector3(-0.4f, 0f, 0.25f)
	};

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.SpawnSetup(map, respawningAfterLoad);
		exactPosition += def.mote.attachedDrawOffset;
	}

	protected override void TimeInterval(float deltaTime)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		base.TimeInterval(deltaTime);
		if (!link1.Linked)
		{
			return;
		}
		bool flag = detachAfterTicks == -1 || Find.TickManager.TicksGame - spawnTick < detachAfterTicks;
		if (!link1.Target.ThingDestroyed && flag)
		{
			link1.UpdateDrawPos();
			if (link1.rotateWithTarget)
			{
				base.Rotation = link1.Target.Thing.Rotation;
			}
		}
		Vector3 val = def.mote.attachedDrawOffset;
		if (def.mote.attachedToHead && link1.Target.Thing is Pawn pawn)
		{
			bool humanlike = pawn.RaceProps.Humanlike;
			List<Vector3> headPosPerRotation = pawn.RaceProps.headPosPerRotation;
			Rot4 rotation = ((pawn.GetPosture() != 0) ? pawn.Drawer.renderer.LayingFacing() : (humanlike ? Rot4.North : pawn.Rotation));
			if (humanlike)
			{
				val = pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			}
			else
			{
				float bodySizeFactor = pawn.ageTracker.CurLifeStage.bodySizeFactor;
				Vector2 val2 = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.drawSize * bodySizeFactor;
				val = ((!headPosPerRotation.NullOrEmpty()) ? headPosPerRotation[rotation.AsInt].ScaledBy(new Vector3(val2.x, 1f, val2.y)) : (animalHeadOffsets[rotation.AsInt] * pawn.BodySize));
			}
		}
		exactPosition = link1.LastDrawPos + val;
		IntVec3 intVec = exactPosition.ToIntVec3();
		if (base.Spawned && !intVec.InBounds(base.Map))
		{
			Destroy();
		}
		else
		{
			base.Position = intVec;
		}
	}
}
