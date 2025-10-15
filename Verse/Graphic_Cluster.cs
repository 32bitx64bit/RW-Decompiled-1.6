using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Cluster : Graphic_Collection
{
	public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

	protected virtual float PositionVariance => 0.45f;

	protected virtual float SizeVariance => 0.2f;

	private float SizeFactorMin => 1f - SizeVariance;

	private float SizeFactorMax => 1f + SizeVariance;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		Log.ErrorOnce("Graphic_Cluster cannot draw realtime.", 9432243);
	}

	protected virtual Vector3 GetCenter(Thing thing, int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return thing.TrueCenter();
	}

	protected virtual int ScatterCount(Thing thing)
	{
		if (!(thing is Filth { thickness: var thickness }))
		{
			return 3;
		}
		return thickness;
	}

	public Graphic SubGraphicFor(Thing thing)
	{
		if (thing == null)
		{
			return subGraphics[0];
		}
		int num = thing.OverrideGraphicIndex ?? thing.thingIDNumber;
		return subGraphics[num % subGraphics.Length];
	}

	public override void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState(thing.Position.GetHashCode());
		int num = ScatterCount(thing);
		Filth filth = thing as Filth;
		Vector2 size = default(Vector2);
		for (int i = 0; i < num; i++)
		{
			Material material = MatSingle;
			Vector3 center = GetCenter(thing, i) + new Vector3(Rand.Range(0f - PositionVariance, PositionVariance), 0f, Rand.Range(0f - PositionVariance, PositionVariance));
			((Vector2)(ref size))._002Ector(Rand.Range(data.drawSize.x * SizeFactorMin, data.drawSize.x * SizeFactorMax), Rand.Range(data.drawSize.y * SizeFactorMin, data.drawSize.y * SizeFactorMax));
			float rot = (float)Rand.RangeInclusive(0, 360) + extraRotation;
			if (filth?.drawInstances != null && filth.drawInstances.Count == num)
			{
				rot = filth.drawInstances[i].rotation;
			}
			bool flipUv = Rand.Value < 0.5f;
			Graphic.TryGetTextureAtlasReplacementInfo(material, thing.def.category.ToAtlasGroup(), flipUv, vertexColors: true, out material, out var uvs, out var vertexColor);
			Printer_Plane.PrintPlane(layer, center, size, material, rot, flipUv, uvs, (Color32[])(object)new Color32[4] { vertexColor, vertexColor, vertexColor, vertexColor });
		}
		Rand.PopState();
	}

	public override string ToString()
	{
		return "Scatter(subGraphic[0]=" + subGraphics[0]?.ToString() + ", count=" + subGraphics.Length + ")";
	}
}
