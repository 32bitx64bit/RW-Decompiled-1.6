using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class DrawBatchPropertyBlock
{
	private enum PropertyType
	{
		Float,
		Color,
		Vector
	}

	private struct Property
	{
		public int propertyId;

		public PropertyType type;

		public float floatVal;

		public Vector4 vectorVal;

		public void Write(MaterialPropertyBlock propertyBlock)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			switch (type)
			{
			case PropertyType.Float:
				propertyBlock.SetFloat(propertyId, floatVal);
				break;
			case PropertyType.Color:
				propertyBlock.SetColor(propertyId, Color.op_Implicit(vectorVal));
				break;
			case PropertyType.Vector:
				propertyBlock.SetVector(propertyId, vectorVal);
				break;
			}
		}
	}

	private List<Property> properties = new List<Property>();

	public string leakDebugString;

	public void Clear()
	{
		properties.Clear();
	}

	public void SetFloat(string name, float val)
	{
		SetFloat(Shader.PropertyToID(name), val);
	}

	public void SetFloat(int propertyId, float val)
	{
		properties.Add(new Property
		{
			propertyId = propertyId,
			type = PropertyType.Float,
			floatVal = val
		});
	}

	public void SetColor(string name, Color val)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SetColor(Shader.PropertyToID(name), val);
	}

	public void SetColor(int propertyId, Color val)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		properties.Add(new Property
		{
			propertyId = propertyId,
			type = PropertyType.Color,
			vectorVal = Color.op_Implicit(val)
		});
	}

	public void SetVector(string name, Vector4 val)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SetVector(Shader.PropertyToID(name), val);
	}

	public void SetVector(int propertyId, Vector4 val)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		properties.Add(new Property
		{
			propertyId = propertyId,
			type = PropertyType.Vector,
			vectorVal = val
		});
	}

	public void Write(MaterialPropertyBlock propertyBlock)
	{
		foreach (Property property in properties)
		{
			property.Write(propertyBlock);
		}
	}
}
