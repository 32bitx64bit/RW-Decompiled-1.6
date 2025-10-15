using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class SolidColorMaterials
{
	private static Dictionary<Color, Material> simpleColorMats = new Dictionary<Color, Material>();

	private static Dictionary<Color, Material> simpleColorAndVertexColorMats = new Dictionary<Color, Material>();

	public static int SimpleColorMatCount => simpleColorMats.Count + simpleColorAndVertexColorMats.Count;

	public static Material SimpleSolidColorMaterial(Color col, bool careAboutVertexColors = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		col = Color32.op_Implicit(Color32.op_Implicit(col));
		Material value;
		if (careAboutVertexColors)
		{
			if (!simpleColorAndVertexColorMats.TryGetValue(col, out value))
			{
				value = NewSolidColorMaterial(col, ShaderDatabase.VertexColor);
				simpleColorAndVertexColorMats.Add(col, value);
			}
		}
		else if (!simpleColorMats.TryGetValue(col, out value))
		{
			value = NewSolidColorMaterial(col, ShaderDatabase.SolidColor);
			simpleColorMats.Add(col, value);
		}
		return value;
	}

	public static Material NewSolidColorMaterial(Color col, Shader shader)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!UnityData.IsInMainThread)
		{
			Log.Error("Tried to create a material from a different thread.");
			return null;
		}
		Material obj = MaterialAllocator.Create(shader);
		obj.color = col;
		string name = ((Object)shader).name;
		Color val = col;
		((Object)obj).name = "SolidColorMat-" + name + "-" + ((object)(Color)(ref val)).ToString();
		return obj;
	}

	public static Texture2D NewSolidColorTexture(float r, float g, float b, float a)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return NewSolidColorTexture(new Color(r, g, b, a));
	}

	public static Texture2D NewSolidColorTexture(Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		if (!UnityData.IsInMainThread)
		{
			Log.Error("Tried to create a texture from a different thread.");
			return null;
		}
		Texture2D val = new Texture2D(2, 2);
		Color val2 = color;
		((Object)val).name = "SolidColorTex-" + ((object)(Color)(ref val2)).ToString();
		val.SetPixel(0, 0, color);
		val.SetPixel(1, 0, color);
		val.SetPixel(0, 1, color);
		val.SetPixel(1, 1, color);
		val.Apply();
		return val;
	}
}
