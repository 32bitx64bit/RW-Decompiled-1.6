using System.Collections.Generic;
using System.Xml;
using RimWorld;
using UnityEngine;

namespace Verse;

public class DrawData
{
	public struct RotationalData
	{
		public Rot4? rotation;

		public Vector2? pivot;

		public Vector3? offset;

		public float? rotationOffset;

		public bool? flip;

		public float? layer;

		public RotationalData(Rot4? rotation, float layer)
		{
			this = default(RotationalData);
			this.rotation = rotation;
			this.layer = layer;
			pivot = null;
			offset = null;
			rotationOffset = null;
			flip = null;
		}
	}

	private class BodyTypeDefWithScale
	{
		public BodyTypeDef bodyType;

		public float scale = 1f;

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "bodyType", xmlRoot.Name);
			scale = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
		}
	}

	public static readonly Vector2 PivotCenter = new Vector2(0.5f, 0.5f);

	private RotationalData defaultData;

	private RotationalData? dataNorth;

	private RotationalData? dataEast;

	private RotationalData? dataSouth;

	private RotationalData? dataWest;

	public bool scaleOffsetByBodySize;

	public bool useBodyPartAnchor;

	public float scale = 1f;

	public float childScale = 1f;

	private List<BodyTypeDefWithScale> bodyTypeScales;

	public float ScaleFor(Pawn pawn)
	{
		float num = scale;
		if (pawn.RaceProps.Humanlike)
		{
			if (pawn.DevelopmentalStage.Child())
			{
				num *= childScale;
			}
			if (bodyTypeScales != null)
			{
				foreach (BodyTypeDefWithScale bodyTypeScale in bodyTypeScales)
				{
					if (pawn.story.bodyType == bodyTypeScale.bodyType)
					{
						num *= bodyTypeScale.scale;
						break;
					}
				}
			}
		}
		return num;
	}

	public Vector2 PivotForRot(Rot4 rot)
	{
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		return (Vector2)(rot.AsInt switch
		{
			0 => ((_003F?)dataNorth?.pivot) ?? ((_003F?)defaultData.pivot) ?? PivotCenter, 
			1 => ((_003F?)dataEast?.pivot) ?? ((_003F?)defaultData.pivot) ?? PivotCenter, 
			2 => ((_003F?)dataSouth?.pivot) ?? ((_003F?)defaultData.pivot) ?? PivotCenter, 
			3 => ((_003F?)dataWest?.pivot) ?? ((_003F?)defaultData.pivot) ?? PivotCenter, 
			_ => ((_003F?)defaultData.pivot) ?? PivotCenter, 
		});
	}

	public Vector3 OffsetForRot(Rot4 rot)
	{
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		switch (rot.AsInt)
		{
		case 0:
			return (Vector3)(((_003F?)dataNorth?.offset) ?? ((_003F?)defaultData.offset) ?? Vector3.zero);
		case 1:
		{
			Vector3? val = dataEast?.offset;
			if (val.HasValue)
			{
				return val.Value;
			}
			Vector3? val2 = dataWest?.offset;
			if (val2.HasValue)
			{
				Vector3 value = val2.Value;
				value.x *= -1f;
				return value;
			}
			Vector3? offset = defaultData.offset;
			if (offset.HasValue)
			{
				return offset.Value;
			}
			return Vector3.zero;
		}
		case 2:
			return (Vector3)(((_003F?)dataSouth?.offset) ?? ((_003F?)defaultData.offset) ?? Vector3.zero);
		case 3:
		{
			Vector3? val3 = dataWest?.offset;
			if (val3.HasValue)
			{
				return val3.Value;
			}
			Vector3? val4 = dataEast?.offset;
			if (val4.HasValue)
			{
				Vector3 value2 = val4.Value;
				value2.x *= -1f;
				return value2;
			}
			Vector3? offset2 = defaultData.offset;
			if (offset2.HasValue)
			{
				return offset2.Value;
			}
			return (Vector3)(((_003F?)dataWest?.offset) ?? ((_003F?)defaultData.offset) ?? Vector3.zero);
		}
		default:
			return (Vector3)(((_003F?)defaultData.offset) ?? Vector3.zero);
		}
	}

	public float RotationOffsetForRot(Rot4 rot)
	{
		return rot.AsInt switch
		{
			0 => dataNorth?.rotationOffset ?? defaultData.rotationOffset.GetValueOrDefault(), 
			1 => dataEast?.rotationOffset ?? defaultData.rotationOffset.GetValueOrDefault(), 
			2 => dataSouth?.rotationOffset ?? defaultData.rotationOffset.GetValueOrDefault(), 
			3 => dataWest?.rotationOffset ?? defaultData.rotationOffset.GetValueOrDefault(), 
			_ => defaultData.rotationOffset.GetValueOrDefault(), 
		};
	}

	public bool FlipForRot(Rot4 rot)
	{
		return rot.AsInt switch
		{
			0 => dataNorth?.flip ?? defaultData.flip.GetValueOrDefault(), 
			1 => dataEast?.flip ?? defaultData.flip.GetValueOrDefault(), 
			2 => dataSouth?.flip ?? defaultData.flip.GetValueOrDefault(), 
			3 => dataWest?.flip ?? defaultData.flip.GetValueOrDefault(), 
			_ => defaultData.flip.GetValueOrDefault(), 
		};
	}

	public float LayerForRot(Rot4 rot, float defaultLayer)
	{
		return rot.AsInt switch
		{
			0 => dataNorth?.layer ?? defaultData.layer ?? defaultLayer, 
			1 => dataEast?.layer ?? defaultData.layer ?? defaultLayer, 
			2 => dataSouth?.layer ?? defaultData.layer ?? defaultLayer, 
			3 => dataWest?.layer ?? defaultData.layer ?? defaultLayer, 
			_ => defaultData.layer ?? defaultLayer, 
		};
	}

	public static DrawData NewWithData(params RotationalData[] data)
	{
		DrawData drawData = new DrawData();
		for (int i = 0; i < data.Length; i++)
		{
			RotationalData value = data[i];
			if (!value.rotation.HasValue)
			{
				drawData.defaultData = value;
				continue;
			}
			switch (value.rotation.Value.AsInt)
			{
			case 0:
				drawData.dataNorth = value;
				break;
			case 1:
				drawData.dataEast = value;
				break;
			case 2:
				drawData.dataSouth = value;
				break;
			case 3:
				drawData.dataWest = value;
				break;
			}
		}
		return drawData;
	}
}
