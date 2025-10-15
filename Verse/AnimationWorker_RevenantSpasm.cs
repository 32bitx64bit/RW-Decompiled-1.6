using UnityEngine;

namespace Verse;

public class AnimationWorker_RevenantSpasm : BaseAnimationWorker
{
	private class RevenantSpasmData
	{
		public float nextSpasm = -99999f;

		public float spasmStart = -99999f;

		public float spasmLength;

		public float startHeadRot = 90f;

		public Vector3 startHeadOffset = Vector3.zero;

		public float targetHeadRot = 90f;

		public Vector3 targetHeadOffset = Vector3.zero;
	}

	private static readonly IntRange SpasmIntervalShort = new IntRange(6, 18);

	private static readonly IntRange SpasmIntervalLong = new IntRange(120, 180);

	private static readonly IntRange SpasmLength = new IntRange(30, 60);

	private const int ShortSpasmLength = 6;

	private static float AnimationProgress(RevenantSpasmData data)
	{
		return ((float)Find.TickManager.TicksGame - data.spasmStart) / data.spasmLength;
	}

	private static float Rotation(RevenantSpasmData data)
	{
		return Mathf.Lerp(data.startHeadRot, data.targetHeadRot, AnimationProgress(data));
	}

	private static Vector3 Offset(RevenantSpasmData data)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.Lerp(data.startHeadOffset.x, data.targetHeadOffset.x, AnimationProgress(data)), 0f, Mathf.Lerp(data.startHeadOffset.z, data.targetHeadOffset.z, AnimationProgress(data)));
	}

	public override bool Enabled(AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
	{
		if (!def.playWhenDowned)
		{
			return !parms.pawn.Downed;
		}
		return true;
	}

	public override void PostDraw(AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms, Matrix4x4 matrix)
	{
	}

	public override Vector3 OffsetAtTick(int tick, AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		RevenantSpasmData animationWorkerData = node.GetAnimationWorkerData<RevenantSpasmData>();
		CheckAndSpasm(animationWorkerData);
		if (parms.facing == Rot4.East || parms.facing == Rot4.West)
		{
			return Offset(animationWorkerData) / 2f;
		}
		return Offset(animationWorkerData);
	}

	public override float AngleAtTick(int tick, AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
	{
		RevenantSpasmData animationWorkerData = node.GetAnimationWorkerData<RevenantSpasmData>();
		CheckAndSpasm(animationWorkerData);
		if (parms.facing == Rot4.East || parms.facing == Rot4.West)
		{
			return Rotation(animationWorkerData) / 2f;
		}
		return Rotation(animationWorkerData);
	}

	public override Vector3 ScaleAtTick(int tick, AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.one;
	}

	public override GraphicStateDef GraphicStateAtTick(int tick, AnimationDef def, PawnRenderNode node, AnimationPart part, PawnDrawParms parms)
	{
		return null;
	}

	private static void CheckAndSpasm(RevenantSpasmData data)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((float)Find.TickManager.TicksGame > data.nextSpasm)
		{
			data.startHeadRot = Rotation(data);
			data.startHeadOffset = Offset(data);
			data.targetHeadRot = Rand.Range(-20, 20);
			data.targetHeadOffset = new Vector3(Rand.Range(-0.1f, 0.1f), 0f, Rand.Range(-0.05f, 0.05f));
			data.spasmStart = Find.TickManager.TicksGame;
			data.spasmLength = (Rand.Bool ? SpasmLength.RandomInRange : 6);
			data.nextSpasm = Find.TickManager.TicksGame + (Rand.Bool ? SpasmIntervalShort.RandomInRange : SpasmIntervalLong.RandomInRange);
		}
	}
}
