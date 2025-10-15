using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Flicker : Graphic_Collection
{
	private const int BaseTicksPerFrameChange = 15;

	private const float MaxOffset = 0.05f;

	public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if (thingDef == null)
		{
			Vector3 val = loc;
			Log.ErrorOnce("Fire DrawWorker with null thingDef: " + ((object)(Vector3)(ref val)).ToString(), 3427324);
			return;
		}
		if (subGraphics == null)
		{
			Log.ErrorOnce("Graphic_Flicker has no subgraphics " + thingDef, 358773632);
			return;
		}
		int num = Find.TickManager.TicksGame;
		if (thing != null)
		{
			num += Mathf.Abs(thing.thingIDNumber ^ 0x80FD52);
		}
		int num2 = num / 15;
		int num3 = Mathf.Abs(num2 ^ ((thing?.thingIDNumber ?? 0) * 391)) % subGraphics.Length;
		float num4 = 1f;
		CompFireOverlayBase compFireOverlayBase = null;
		Fire fire = thing as Fire;
		CompProperties_FireOverlay compProperties = thingDef.GetCompProperties<CompProperties_FireOverlay>();
		if (fire != null)
		{
			num4 = fire.fireSize;
		}
		else if (thing != null)
		{
			compFireOverlayBase = thing.TryGetComp<CompFireOverlayBase>();
			if (compFireOverlayBase != null)
			{
				num4 = compFireOverlayBase.FireSize;
			}
			else
			{
				compFireOverlayBase = thing.TryGetComp<CompDarklightOverlay>();
				if (compFireOverlayBase != null)
				{
					num4 = compFireOverlayBase.FireSize;
				}
			}
		}
		else if (compProperties != null)
		{
			num4 = compProperties.fireSize;
		}
		if (num3 < 0 || num3 >= subGraphics.Length)
		{
			Log.ErrorOnce("Fire drawing out of range: " + num3, 7453435);
			num3 = 0;
		}
		Graphic graphic = subGraphics[num3];
		float num5 = ((compFireOverlayBase == null) ? Mathf.Min(num4 / 1.2f, 1.2f) : num4);
		Vector3 val2 = GenRadial.RadialPattern[num2 % GenRadial.RadialPattern.Length].ToVector3() / GenRadial.MaxRadialPatternRadius;
		val2 *= 0.05f;
		Vector3 val3 = loc + val2 * num4;
		if (thing?.Graphic?.data != null)
		{
			val3 += thing.Graphic.data.DrawOffsetForRot(rot);
		}
		if (compFireOverlayBase != null)
		{
			val3 += compFireOverlayBase.Props.DrawOffsetForRot(rot);
		}
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(num5, 1f, num5);
		Matrix4x4 val5 = default(Matrix4x4);
		((Matrix4x4)(ref val5)).SetTRS(val3, Quaternion.identity, val4);
		Graphics.DrawMesh(MeshPool.plane10, val5, graphic.MatSingle, 0);
	}

	public override string ToString()
	{
		return "Flicker(subGraphic[0]=" + subGraphics[0]?.ToString() + ", count=" + subGraphics.Length + ")";
	}
}
