using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public class WornGraphicData
{
	public bool renderUtilityAsPack;

	public WornGraphicDirectionData north;

	public WornGraphicDirectionData south;

	public WornGraphicDirectionData east;

	public WornGraphicDirectionData west;

	public WornGraphicBodyTypeData male;

	public WornGraphicBodyTypeData female;

	public WornGraphicBodyTypeData thin;

	public WornGraphicBodyTypeData hulk;

	public WornGraphicBodyTypeData fat;

	public WornGraphicDirectionData GetDirectionalData(Rot4 facing)
	{
		return facing.AsInt switch
		{
			0 => north, 
			1 => east, 
			2 => south, 
			3 => west, 
			_ => throw new ArgumentException($"Unhandled Rot4: {facing}"), 
		};
	}

	public Vector2 BeltOffsetAt(Rot4 facing, BodyTypeDef bodyType)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		WornGraphicDirectionData directionalData = GetDirectionalData(facing);
		Vector2 val = directionalData.offset;
		if (bodyType == BodyTypeDefOf.Male)
		{
			val += directionalData.male.offset;
		}
		else if (bodyType == BodyTypeDefOf.Female)
		{
			val += directionalData.female.offset;
		}
		else if (bodyType == BodyTypeDefOf.Thin)
		{
			val += directionalData.thin.offset;
		}
		else if (bodyType == BodyTypeDefOf.Hulk)
		{
			val += directionalData.hulk.offset;
		}
		else if (bodyType == BodyTypeDefOf.Fat)
		{
			val += directionalData.fat.offset;
		}
		return val;
	}

	public Vector2 BeltScaleAt(Rot4 facing, BodyTypeDef bodyType)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = GetDirectionalData(facing).Scale;
		if (bodyType == BodyTypeDefOf.Male)
		{
			val *= male.Scale;
		}
		else if (bodyType == BodyTypeDefOf.Female)
		{
			val *= female.Scale;
		}
		else if (bodyType == BodyTypeDefOf.Thin)
		{
			val *= thin.Scale;
		}
		else if (bodyType == BodyTypeDefOf.Hulk)
		{
			val *= hulk.Scale;
		}
		else if (bodyType == BodyTypeDefOf.Fat)
		{
			val *= fat.Scale;
		}
		return val;
	}
}
