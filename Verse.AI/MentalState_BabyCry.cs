using RimWorld;
using UnityEngine;

namespace Verse.AI;

public class MentalState_BabyCry : MentalState_BabyFit
{
	private int ticksUntilLeftTear;

	private int ticksUntilRightTear;

	private const int TicksBetweenTearDots = 35;

	private static readonly IntRange TicksBetweenTears = new IntRange(25, 40);

	private const float speed = 0.66f;

	private static readonly FloatRange randAngle = new FloatRange(10f, 30f);

	private static readonly FloatRange randScale = new FloatRange(0.6f, 1f);

	public override void MentalStateTick(int delta)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		base.MentalStateTick(delta);
		float num = base.pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None);
		if (base.pawn.SpawnedParentOrMe is Pawn pawn && !pawn.Position.Fogged(pawn.Map))
		{
			if (--ticksUntilLeftTear <= 0)
			{
				pawn.Map.flecks.CreateFleck(new FleckCreationData
				{
					spawnPosition = base.pawn.DrawPosHeld.Value + Vector3Utility.RotatedBy(new Vector3(-0.15f, 0f, 0.066f), num),
					velocitySpeed = -0.66f,
					velocityAngle = 90f + num - randAngle.RandomInRange,
					def = FleckDefOf.FleckBabyCrying,
					scale = randScale.RandomInRange
				});
				ticksUntilLeftTear = TicksBetweenTears.RandomInRange;
			}
			if (--ticksUntilRightTear <= 0)
			{
				pawn.Map.flecks.CreateFleck(new FleckCreationData
				{
					spawnPosition = base.pawn.DrawPosHeld.Value + Vector3Utility.RotatedBy(new Vector3(0.15f, 0f, 0.066f), num),
					velocitySpeed = 0.66f,
					velocityAngle = 90f + num + randAngle.RandomInRange,
					def = FleckDefOf.FleckBabyCrying,
					scale = randScale.RandomInRange,
					exactScale = new Vector3(-1f, 1f, 1f)
				});
				ticksUntilRightTear = TicksBetweenTears.RandomInRange;
			}
			if (base.pawn.IsHashIntervalTick(35, delta))
			{
				MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_BabyCryingDots, Vector3Utility.RotatedBy(new Vector3(0.27f, 0f, 0.066f), num)).exactRotation = Rand.Value * 180f;
				MoteMaker.MakeAttachedOverlay(pawn, ThingDefOf.Mote_BabyCryingDots, Vector3Utility.RotatedBy(new Vector3(-0.27f, 0f, 0.066f), num)).exactRotation = Rand.Value * 180f;
			}
		}
	}

	protected override void AuraEffect(Thing source, Pawn hearer)
	{
		hearer.HearClamor(source, ClamorDefOf.BabyCry);
		if (source is Pawn otherPawn && hearer.needs.mood != null)
		{
			if (hearer == otherPawn.GetMother() || hearer == otherPawn.GetFather())
			{
				hearer.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.MyCryingBaby, otherPawn);
			}
			else
			{
				hearer.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CryingBaby, otherPawn);
			}
			hearer.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.BabyCriedSocial, otherPawn);
		}
	}
}
