using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MaterialPool
{
	private static Dictionary<MaterialRequest, Material> matDictionary = new Dictionary<MaterialRequest, Material>();

	private static Dictionary<Material, MaterialRequest> matDictionaryReverse = new Dictionary<Material, MaterialRequest>();

	public static Material MatFrom(string texPath, bool reportFailure)
	{
		if (texPath == null || texPath == "null")
		{
			return null;
		}
		return MatFrom(new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath, reportFailure)));
	}

	public static Material MatFrom(string texPath)
	{
		if (texPath == null || texPath == "null")
		{
			return null;
		}
		return MatFrom(new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath)));
	}

	public static Material MatFrom(Texture2D srcTex)
	{
		return MatFrom(new MaterialRequest((Texture)(object)srcTex));
	}

	public static Material MatFrom(Texture2D srcTex, Shader shader, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return MatFrom(new MaterialRequest((Texture)(object)srcTex, shader, color));
	}

	public static Material MatFrom(Texture2D srcTex, Shader shader, Color color, int renderQueue)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		MaterialRequest req = new MaterialRequest((Texture)(object)srcTex, shader, color);
		req.renderQueue = renderQueue;
		return MatFrom(req);
	}

	public static Material MatFrom(string texPath, Shader shader)
	{
		return MatFrom(new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath), shader));
	}

	public static Material MatFrom(string texPath, Shader shader, int renderQueue)
	{
		MaterialRequest req = new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath), shader);
		req.renderQueue = renderQueue;
		return MatFrom(req);
	}

	public static Material MatFrom(string texPath, Shader shader, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return MatFrom(new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath), shader, color));
	}

	public static Material MatFrom(string texPath, Shader shader, Color color, int renderQueue)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		MaterialRequest req = new MaterialRequest((Texture)(object)ContentFinder<Texture2D>.Get(texPath), shader, color);
		req.renderQueue = renderQueue;
		return MatFrom(req);
	}

	public static Material MatFrom(Shader shader)
	{
		return MatFrom(new MaterialRequest(shader));
	}

	public static Material MatFrom(MaterialRequest req)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		if (!UnityData.IsInMainThread)
		{
			Log.Error("Tried to get a material from a different thread.");
			return null;
		}
		if ((Object)(object)req.mainTex == (Object)null && req.needsMainTex)
		{
			Log.Error("MatFrom with null sourceTex.");
			return BaseContent.BadMat;
		}
		if ((Object)(object)req.shader == (Object)null)
		{
			Log.Warning("Matfrom with null shader.");
			return BaseContent.BadMat;
		}
		if ((Object)(object)req.maskTex != (Object)null && !req.shader.SupportsMaskTex())
		{
			Log.Error("MaterialRequest has maskTex but shader does not support it. req=" + req.ToString());
			req.maskTex = null;
		}
		req.color = Color32.op_Implicit(Color32.op_Implicit(req.color));
		req.colorTwo = Color32.op_Implicit(Color32.op_Implicit(req.colorTwo));
		if (!matDictionary.TryGetValue(req, out var value))
		{
			value = MaterialAllocator.Create(req.shader);
			((Object)value).name = ((Object)req.shader).name;
			if ((Object)(object)req.mainTex != (Object)null)
			{
				Material obj = value;
				((Object)obj).name = ((Object)obj).name + "_" + ((Object)req.mainTex).name;
				value.mainTexture = req.mainTex;
			}
			value.color = req.color;
			if ((Object)(object)req.maskTex != (Object)null)
			{
				value.SetTexture(ShaderPropertyIDs.MaskTex, (Texture)(object)req.maskTex);
				value.SetColor(ShaderPropertyIDs.ColorTwo, req.colorTwo);
			}
			else if (req.colorTwo != default(Color))
			{
				value.SetColor(ShaderPropertyIDs.ColorTwo, req.colorTwo);
			}
			if ((Object)(object)req.secondaryTex != (Object)null)
			{
				value.SetTexture(ShaderPropertyIDs.SecondaryTex, req.secondaryTex);
			}
			if (req.renderQueue != 0)
			{
				value.renderQueue = req.renderQueue;
			}
			if (!req.shaderParameters.NullOrEmpty())
			{
				for (int i = 0; i < req.shaderParameters.Count; i++)
				{
					req.shaderParameters[i].Apply(value);
				}
			}
			matDictionary.Add(req, value);
			matDictionaryReverse.Add(value, req);
			if ((Object)(object)req.shader == (Object)(object)ShaderDatabase.CutoutPlant || (Object)(object)req.shader == (Object)(object)ShaderDatabase.TransparentPlant)
			{
				WindManager.Notify_PlantMaterialCreated(value);
			}
		}
		return value;
	}

	public static bool TryGetRequestForMat(Material material, out MaterialRequest request)
	{
		return matDictionaryReverse.TryGetValue(material, out request);
	}
}
