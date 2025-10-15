using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class DamageGraphicData
{
	public bool enabled = true;

	public Rect rectN;

	public Rect rectE;

	public Rect rectS;

	public Rect rectW;

	public Rect rect;

	[NoTranslate]
	public List<string> scratches;

	[NoTranslate]
	public string cornerTL;

	[NoTranslate]
	public string cornerTR;

	[NoTranslate]
	public string cornerBL;

	[NoTranslate]
	public string cornerBR;

	[NoTranslate]
	public string edgeLeft;

	[NoTranslate]
	public string edgeRight;

	[NoTranslate]
	public string edgeTop;

	[NoTranslate]
	public string edgeBot;

	[Unsaved(false)]
	public List<Material> scratchMats;

	[Unsaved(false)]
	public Material cornerTLMat;

	[Unsaved(false)]
	public Material cornerTRMat;

	[Unsaved(false)]
	public Material cornerBLMat;

	[Unsaved(false)]
	public Material cornerBRMat;

	[Unsaved(false)]
	public Material edgeLeftMat;

	[Unsaved(false)]
	public Material edgeRightMat;

	[Unsaved(false)]
	public Material edgeTopMat;

	[Unsaved(false)]
	public Material edgeBotMat;

	public void ResolveReferencesSpecial()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Expected O, but got Unknown
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Expected O, but got Unknown
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Expected O, but got Unknown
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Expected O, but got Unknown
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Expected O, but got Unknown
			if (scratches != null)
			{
				scratchMats = new List<Material>();
				for (int i = 0; i < scratches.Count; i++)
				{
					Material val = MaterialPool.MatFrom(scratches[i], ShaderDatabase.Transparent, 2905);
					scratchMats.Add(val);
					GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)val.mainTexture);
				}
			}
			if (cornerTL != null)
			{
				cornerTLMat = MaterialPool.MatFrom(cornerTL, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)cornerTLMat.mainTexture);
			}
			if (cornerTR != null)
			{
				cornerTRMat = MaterialPool.MatFrom(cornerTR, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)cornerTRMat.mainTexture);
			}
			if (cornerBL != null)
			{
				cornerBLMat = MaterialPool.MatFrom(cornerBL, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)cornerBLMat.mainTexture);
			}
			if (cornerBR != null)
			{
				cornerBRMat = MaterialPool.MatFrom(cornerBR, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)cornerBRMat.mainTexture);
			}
			if (edgeTop != null)
			{
				edgeTopMat = MaterialPool.MatFrom(edgeTop, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)edgeTopMat.mainTexture);
			}
			if (edgeBot != null)
			{
				edgeBotMat = MaterialPool.MatFrom(edgeBot, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)edgeBotMat.mainTexture);
			}
			if (edgeLeft != null)
			{
				edgeLeftMat = MaterialPool.MatFrom(edgeLeft, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)edgeLeftMat.mainTexture);
			}
			if (edgeRight != null)
			{
				edgeRightMat = MaterialPool.MatFrom(edgeRight, ShaderDatabase.Transparent, 2905);
				GlobalTextureAtlasManager.TryInsertStatic(TextureAtlasGroup.Building, (Texture2D)edgeRightMat.mainTexture);
			}
		});
	}
}
