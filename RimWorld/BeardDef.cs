using UnityEngine;
using Verse;

namespace RimWorld;

public class BeardDef : StyleItemDef
{
	public Vector3 offsetNarrowEast;

	public Vector3 offsetNarrowSouth;

	public Vector3 GetOffset(HeadTypeDef headType, Rot4 rot)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!headType.narrow || rot == Rot4.North)
		{
			return Vector3.zero;
		}
		if (rot == Rot4.South)
		{
			return offsetNarrowSouth;
		}
		if (rot == Rot4.East)
		{
			return offsetNarrowEast;
		}
		return new Vector3(0f - offsetNarrowEast.x, 0f, offsetNarrowEast.z);
	}

	public override Graphic GraphicFor(Pawn pawn, Color color)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (noGraphic)
		{
			return null;
		}
		return GraphicDatabase.Get<Graphic_Multi>(texPath, overrideShaderTypeDef?.Shader ?? ShaderDatabase.CutoutHair, Vector2.one, color);
	}
}
