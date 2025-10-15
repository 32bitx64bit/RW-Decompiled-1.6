using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Gas : Graphic_Single
{
	private const float PositionVariance = 0.45f;

	private const float SizeVariance = 0.2f;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState();
		Rand.Seed = thing.thingIDNumber.GetHashCode();
		float num = (float)Rand.Range(0, 360) + ((thing as Gas)?.graphicRotation ?? 0f);
		Vector3 val = thing.TrueCenter() + new Vector3(Rand.Range(-0.45f, 0.45f), 0f, Rand.Range(-0.45f, 0.45f));
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(Rand.Range(0.8f, 1.2f) * drawSize.x, 0f, Rand.Range(0.8f, 1.2f) * drawSize.y);
		Matrix4x4 val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(val, Quaternion.AngleAxis(num, Vector3.up), val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, MatSingle, 0);
		Rand.PopState();
	}
}
