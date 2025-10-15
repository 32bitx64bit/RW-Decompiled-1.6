using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompRadialPulse : ThingComp
{
	private static readonly Material RingMat = MaterialPool.MatFrom("Other/ForceField", ShaderDatabase.MoteGlow);

	private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();

	private const float TextureActualRingSizeFactor = 1.1601562f;

	private CompProperties_RadialPulse Props => (CompProperties_RadialPulse)props;

	private float RingLerpFactor => (float)(Find.TickManager.TicksGame % Props.ticksBetweenPulses) / (float)Props.ticksPerPulse;

	private float RingScale => Props.radius * Mathf.Lerp(0f, 2f, RingLerpFactor) * 1.1601562f;

	private bool ParentIsActive => parent.GetComp<CompSendSignalOnMotion>()?.Sent ?? false;

	public override void PostDraw()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!ParentIsActive)
		{
			Vector3 val = parent.Position.ToVector3Shifted();
			val.y = AltitudeLayer.MoteOverhead.AltitudeFor();
			Color color = Props.color;
			color.a = Mathf.Lerp(Props.color.a, 0f, RingLerpFactor);
			MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
			Matrix4x4 val2 = default(Matrix4x4);
			((Matrix4x4)(ref val2)).SetTRS(val, Quaternion.identity, new Vector3(RingScale, 1f, RingScale));
			Graphics.DrawMesh(MeshPool.plane10, val2, RingMat, 0, (Camera)null, 0, MatPropertyBlock);
		}
	}
}
