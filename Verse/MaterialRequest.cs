using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public struct MaterialRequest : IEquatable<MaterialRequest>
{
	public Shader shader;

	public Texture mainTex;

	public Color color;

	public Color colorTwo;

	public Texture2D maskTex;

	public Texture secondaryTex;

	public int renderQueue;

	public bool needsMainTex;

	public List<ShaderParameter> shaderParameters;

	public string BaseTexPath
	{
		set
		{
			mainTex = (Texture)(object)ContentFinder<Texture2D>.Get(value);
		}
	}

	public MaterialRequest(Texture tex)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		shader = ShaderDatabase.Cutout;
		mainTex = tex;
		color = Color.white;
		colorTwo = Color.white;
		maskTex = null;
		renderQueue = 0;
		shaderParameters = null;
		secondaryTex = null;
		needsMainTex = true;
	}

	public MaterialRequest(Texture tex, Shader shader)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		this.shader = shader;
		mainTex = tex;
		color = Color.white;
		colorTwo = Color.white;
		maskTex = null;
		renderQueue = 0;
		shaderParameters = null;
		secondaryTex = null;
		needsMainTex = true;
	}

	public MaterialRequest(Texture tex, Shader shader, Color color)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		this.shader = shader;
		mainTex = tex;
		this.color = color;
		colorTwo = Color.white;
		maskTex = null;
		renderQueue = 0;
		shaderParameters = null;
		secondaryTex = null;
		needsMainTex = true;
	}

	public MaterialRequest(Shader shader)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		this.shader = shader;
		mainTex = null;
		color = Color.white;
		colorTwo = Color.white;
		maskTex = null;
		renderQueue = 0;
		shaderParameters = null;
		secondaryTex = null;
		needsMainTex = false;
	}

	public override int GetHashCode()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return Gen.HashCombine<Texture>(Gen.HashCombine(Gen.HashCombineInt(Gen.HashCombine<Texture2D>(Gen.HashCombine<Texture>(Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombine<Shader>(0, shader), color), colorTwo), mainTex), maskTex), renderQueue), shaderParameters), secondaryTex);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MaterialRequest other))
		{
			return false;
		}
		return Equals(other);
	}

	public bool Equals(MaterialRequest other)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)other.shader == (Object)(object)shader && (Object)(object)other.mainTex == (Object)(object)mainTex && other.color == color && other.colorTwo == colorTwo && (Object)(object)other.maskTex == (Object)(object)maskTex && other.renderQueue == renderQueue && (Object)(object)other.secondaryTex == (Object)(object)secondaryTex)
		{
			return other.shaderParameters == shaderParameters;
		}
		return false;
	}

	public static bool operator ==(MaterialRequest lhs, MaterialRequest rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MaterialRequest lhs, MaterialRequest rhs)
	{
		return !(lhs == rhs);
	}

	public override string ToString()
	{
		return "MaterialRequest(" + ((Object)shader).name + ", " + ((Object)mainTex).name + ", " + ((object)(Color)(ref color)).ToString() + ", " + ((object)(Color)(ref colorTwo)).ToString() + ", " + ((object)maskTex)?.ToString() + ", " + ((object)secondaryTex)?.ToString() + ", " + renderQueue + ")";
	}
}
