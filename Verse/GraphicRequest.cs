using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public struct GraphicRequest : IEquatable<GraphicRequest>
{
	public Type graphicClass;

	public Texture2D texture;

	public string path;

	public string maskPath;

	public Shader shader;

	public Vector2 drawSize;

	public Color color;

	public Color colorTwo;

	public GraphicData graphicData;

	public int renderQueue;

	public List<ShaderParameter> shaderParameters;

	public GraphicRequest(Type graphicClass, string path, Shader shader, Vector2 drawSize, Color color, Color colorTwo, GraphicData graphicData, int renderQueue, List<ShaderParameter> shaderParameters, string maskPath)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		this.graphicClass = graphicClass;
		this.path = path;
		this.maskPath = maskPath;
		this.shader = shader;
		this.drawSize = drawSize;
		this.color = color;
		this.colorTwo = colorTwo;
		this.graphicData = graphicData;
		this.renderQueue = renderQueue;
		this.shaderParameters = (shaderParameters.NullOrEmpty() ? null : shaderParameters);
		texture = null;
	}

	public GraphicRequest(Type graphicClass, Texture2D texture, Shader shader, Vector2 drawSize, Color color, Color colorTwo, GraphicData graphicData, int renderQueue, List<ShaderParameter> shaderParameters, string maskPath)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		this.graphicClass = graphicClass;
		this.texture = texture;
		this.maskPath = maskPath;
		this.shader = shader;
		this.drawSize = drawSize;
		this.color = color;
		this.colorTwo = colorTwo;
		this.graphicData = graphicData;
		this.renderQueue = renderQueue;
		this.shaderParameters = (shaderParameters.NullOrEmpty() ? null : shaderParameters);
		path = null;
	}

	public override int GetHashCode()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (path == null)
		{
			path = BaseContent.BadTexPath;
		}
		return Gen.HashCombine(Gen.HashCombine(Gen.HashCombine(Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Vector2>(Gen.HashCombine<Shader>(Gen.HashCombine(Gen.HashCombine<Texture2D>(Gen.HashCombine(Gen.HashCombine(0, graphicClass), path), texture), maskPath), shader), drawSize), color), colorTwo), graphicData), renderQueue), shaderParameters);
	}

	public override string ToString()
	{
		return $"{graphicClass}: {path}";
	}

	public override bool Equals(object obj)
	{
		if (!(obj is GraphicRequest))
		{
			return false;
		}
		return Equals((GraphicRequest)obj);
	}

	public bool Equals(GraphicRequest other)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (graphicClass == other.graphicClass && path == other.path && (Object)(object)texture == (Object)(object)other.texture && maskPath == other.maskPath && (Object)(object)shader == (Object)(object)other.shader && drawSize == other.drawSize && color == other.color && colorTwo == other.colorTwo && graphicData == other.graphicData && renderQueue == other.renderQueue)
		{
			return shaderParameters == other.shaderParameters;
		}
		return false;
	}

	public static bool operator ==(GraphicRequest lhs, GraphicRequest rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(GraphicRequest lhs, GraphicRequest rhs)
	{
		return !(lhs == rhs);
	}
}
