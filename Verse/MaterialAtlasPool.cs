using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MaterialAtlasPool
{
	private class MaterialAtlas
	{
		protected Material[] subMats = (Material[])(object)new Material[16];

		private const float TexPadding = 1f / 32f;

		public MaterialAtlas(Material newRootMat)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			Vector2 mainTextureScale = default(Vector2);
			((Vector2)(ref mainTextureScale))._002Ector(0.1875f, 0.1875f);
			Vector2 mainTextureOffset = default(Vector2);
			for (int i = 0; i < 16; i++)
			{
				float num = (float)(i % 4) * 0.25f + 1f / 32f;
				float num2 = (float)(i / 4) * 0.25f + 1f / 32f;
				((Vector2)(ref mainTextureOffset))._002Ector(num, num2);
				Material val = MaterialAllocator.Create(newRootMat);
				((Object)val).name = ((Object)newRootMat).name + "_ASM" + i;
				val.mainTextureScale = mainTextureScale;
				val.mainTextureOffset = mainTextureOffset;
				subMats[i] = val;
			}
		}

		public Material SubMat(LinkDirections linkSet)
		{
			int num = (int)linkSet;
			if (num >= subMats.Length)
			{
				Log.Warning("Cannot get submat of index " + num + ": out of range.");
				return BaseContent.BadMat;
			}
			return subMats[num];
		}
	}

	private static Dictionary<Material, MaterialAtlas> atlasDict = new Dictionary<Material, MaterialAtlas>();

	public static Material SubMaterialFromAtlas(Material mat, LinkDirections LinkSet)
	{
		if (!atlasDict.ContainsKey(mat))
		{
			atlasDict.Add(mat, new MaterialAtlas(mat));
		}
		return atlasDict[mat].SubMat(LinkSet);
	}
}
