using UnityEngine;

namespace Verse;

public class PawnRenderNode_Swaddle : PawnRenderNode
{
	public new PawnRenderNodeProperties_Swaddle Props => (PawnRenderNodeProperties_Swaddle)props;

	public PawnRenderNode_Swaddle(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
		: base(pawn, props, tree)
	{
	}

	public override Color ColorFor(Pawn pawn)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState(pawn.thingIDNumber);
		PawnRenderNodeProperties_Swaddle pawnRenderNodeProperties_Swaddle = Props;
		float randomInRange = pawnRenderNodeProperties_Swaddle.brightnessRange.RandomInRange;
		float num = Rand.Range(0f - pawnRenderNodeProperties_Swaddle.swaddleColorOffset, pawnRenderNodeProperties_Swaddle.swaddleColorOffset);
		float num2 = Rand.Range(0f - pawnRenderNodeProperties_Swaddle.swaddleColorOffset, pawnRenderNodeProperties_Swaddle.swaddleColorOffset);
		float num3 = Rand.Range(0f - pawnRenderNodeProperties_Swaddle.swaddleColorOffset, pawnRenderNodeProperties_Swaddle.swaddleColorOffset);
		Rand.PopState();
		return new Color(randomInRange + num, randomInRange + num2, randomInRange + num3);
	}
}
