using UnityEngine;

namespace Verse;

public class Graphic_LinkedCornerFiller : Graphic_Linked
{
	private const float ShiftUp = 0.09f;

	private const float CoverSize = 0.5f;

	private static readonly float CoverSizeCornerCorner;

	private static readonly float DistCenterCorner;

	private static readonly float CoverOffsetDist;

	private static readonly Vector2[] CornerFillUVs;

	public override LinkDrawerType LinkerType => LinkDrawerType.CornerFiller;

	public Graphic_LinkedCornerFiller(Graphic subGraphic)
		: base(subGraphic)
	{
	}

	public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new Graphic_LinkedCornerFiller(subGraphic.GetColoredVersion(newShader, newColor, newColorTwo))
		{
			data = data
		};
	}

	public override void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		base.Print(layer, thing, extraRotation);
		IntVec3 position = thing.Position;
		Vector2 size = default(Vector2);
		for (int i = 0; i < 4; i++)
		{
			IntVec3 c = thing.Position + GenAdj.DiagonalDirectionsAround[i];
			if (!ShouldLinkWith(c, thing) || (i == 0 && (!ShouldLinkWith(position + IntVec3.West, thing) || !ShouldLinkWith(position + IntVec3.South, thing))) || (i == 1 && (!ShouldLinkWith(position + IntVec3.West, thing) || !ShouldLinkWith(position + IntVec3.North, thing))) || (i == 2 && (!ShouldLinkWith(position + IntVec3.East, thing) || !ShouldLinkWith(position + IntVec3.North, thing))) || (i == 3 && (!ShouldLinkWith(position + IntVec3.East, thing) || !ShouldLinkWith(position + IntVec3.South, thing))))
			{
				continue;
			}
			Vector3 drawPos = thing.DrawPos;
			Vector3 val = GenAdj.DiagonalDirectionsAround[i].ToVector3();
			Vector3 center = drawPos + ((Vector3)(ref val)).normalized * CoverOffsetDist + Altitudes.AltIncVect + new Vector3(0f, 0f, 0.09f);
			((Vector2)(ref size))._002Ector(0.5f, 0.5f);
			if (!c.InBounds(thing.Map))
			{
				if (c.x == -1)
				{
					center.x -= 1f;
					size.x *= 5f;
				}
				if (c.z == -1)
				{
					center.z -= 1f;
					size.y *= 5f;
				}
				if (c.x == thing.Map.Size.x)
				{
					center.x += 1f;
					size.x *= 5f;
				}
				if (c.z == thing.Map.Size.z)
				{
					center.z += 1f;
					size.y *= 5f;
				}
			}
			Printer_Plane.PrintPlane(layer, center, size, LinkedDrawMatFrom(thing, thing.Position), extraRotation, flipUv: false, CornerFillUVs);
		}
	}

	static Graphic_LinkedCornerFiller()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2(0.5f, 0.5f);
		CoverSizeCornerCorner = ((Vector2)(ref val)).magnitude;
		val = new Vector2(0.5f, 0.5f);
		DistCenterCorner = ((Vector2)(ref val)).magnitude;
		CoverOffsetDist = DistCenterCorner - CoverSizeCornerCorner * 0.5f;
		CornerFillUVs = (Vector2[])(object)new Vector2[4]
		{
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f),
			new Vector2(0.5f, 0.6f)
		};
	}
}
