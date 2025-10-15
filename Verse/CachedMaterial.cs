using UnityEngine;

namespace Verse;

public class CachedMaterial
{
	private string texPath;

	private Material cachedMaterial;

	private Shader shader;

	private Color color;

	private int validationIndex;

	private static int curValidationIndex;

	public Material Material
	{
		get
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)cachedMaterial == (Object)null || validationIndex != curValidationIndex)
			{
				if (texPath.NullOrEmpty())
				{
					cachedMaterial = BaseContent.BadMat;
				}
				else
				{
					cachedMaterial = MaterialPool.MatFrom(texPath, shader, color);
				}
				validationIndex = curValidationIndex;
			}
			return cachedMaterial;
		}
	}

	public string TexturePath => texPath;

	public Color Color => color;

	public CachedMaterial(string texPath, Shader shader, Color color)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.texPath = texPath;
		this.shader = shader;
		this.color = color;
		cachedMaterial = null;
		validationIndex = -1;
	}

	public CachedMaterial(string texPath, Shader shader)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		this.texPath = texPath;
		this.shader = shader;
		color = Color.white;
		cachedMaterial = null;
		validationIndex = -1;
	}

	public static void ResetStaticData()
	{
		curValidationIndex++;
	}
}
