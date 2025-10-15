using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnTweener
{
	private Pawn pawn;

	private Vector3 tweenedPos = new Vector3(0f, 0f, 0f);

	private int lastDrawFrame = -1;

	private int lastDrawTick = -1;

	private int lastOffsetTick = -1;

	private Vector3 lastOffset = Vector3.zero;

	private Vector3 lastTickSpringPos;

	private const float SpringTightness = 0.09f;

	private const float CrawlsPerTile = 3f;

	private const float CrawlingLurchFactor = 3f;

	public Vector3 TweenedPos => tweenedPos;

	public Vector3 LastTickTweenedVelocity => TweenedPos - lastTickSpringPos;

	public PawnTweener(Pawn pawn)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		this.pawn = pawn;
	}

	public void PreDrawPosCalculation()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (lastDrawFrame == RealTime.frameCount)
		{
			return;
		}
		if (!pawn.Spawned)
		{
			tweenedPos = pawn.Position.ToVector3Shifted();
			return;
		}
		if (lastDrawFrame < RealTime.frameCount - 1 && lastDrawTick < GenTicks.TicksGame - 1)
		{
			ResetTweenedPosToRoot();
		}
		else
		{
			lastTickSpringPos = tweenedPos;
			float tickRateMultiplier = Find.TickManager.TickRateMultiplier;
			if (tickRateMultiplier < 5f)
			{
				Vector3 val = TweenedPosRoot() - tweenedPos;
				float num = 0.09f * (RealTime.deltaTime * 60f * tickRateMultiplier);
				if (RealTime.deltaTime > 0.05f)
				{
					num = Mathf.Min(num, 1f);
				}
				tweenedPos += val * num;
			}
			else
			{
				ResetTweenedPosToRoot();
			}
		}
		lastDrawFrame = RealTime.frameCount;
		lastDrawTick = GenTicks.TicksGame;
	}

	public void ResetTweenedPosToRoot()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		tweenedPos = TweenedPosRoot();
		lastTickSpringPos = tweenedPos;
		lastDrawFrame = RealTime.frameCount;
		lastDrawTick = GenTicks.TicksGame;
	}

	public void Notify_Teleported()
	{
		lastDrawFrame = -1;
		lastDrawTick = -1;
	}

	private Vector3 TweenedPosRoot()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (!pawn.Spawned)
		{
			return pawn.Position.ToVector3Shifted();
		}
		float num = 0f;
		if (pawn.Spawned && pawn.ageTracker.CurLifeStage.sittingOffset.HasValue && !pawn.pather.MovingNow && pawn.GetPosture() == PawnPosture.Standing)
		{
			Building edifice = pawn.Position.GetEdifice(pawn.Map);
			if (edifice != null && edifice.def.building != null && edifice.def.building.isSittable)
			{
				num = pawn.ageTracker.CurLifeStage.sittingOffset.Value;
			}
		}
		float num2 = pawn.pather.MovePercentage;
		if (pawn.Crawling)
		{
			num2 = num2 - num2 % (1f / 3f) + Mathf.Pow(num2 % (1f / 3f) * 3f, 3f) * (1f / 3f);
		}
		Vector3 val;
		if (GenTicks.TicksGame == lastOffsetTick)
		{
			val = lastOffset;
		}
		else
		{
			val = (lastOffset = PawnCollisionTweenerUtility.PawnCollisionPosOffsetFor(pawn));
			lastOffsetTick = GenTicks.TicksGame;
		}
		return pawn.pather.nextCell.ToVector3Shifted() * num2 + pawn.Position.ToVector3Shifted() * (1f - num2) + new Vector3(0f, 0f, num) + val;
	}
}
