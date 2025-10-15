using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_AttachmentBody : PawnRenderNodeWorker_Body
{
	public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.ScaleFor(node, parms);
		Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
		return val * ((bodyGraphicScale.x + bodyGraphicScale.y) / 2f);
	}
}
