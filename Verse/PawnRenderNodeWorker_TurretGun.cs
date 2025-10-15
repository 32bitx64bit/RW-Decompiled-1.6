using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_TurretGun : PawnRenderNodeWorker
{
	public override Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = base.RotationFor(node, parms);
		if (node is PawnRenderNode_TurretGun pawnRenderNode_TurretGun)
		{
			val *= pawnRenderNode_TurretGun.turretComp.curRotation.ToQuat();
		}
		return val;
	}
}
