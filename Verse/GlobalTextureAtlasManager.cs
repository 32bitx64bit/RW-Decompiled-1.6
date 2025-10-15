using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public static class GlobalTextureAtlasManager
{
	public static bool rebakeAtlas = false;

	private static List<PawnTextureAtlas> pawnTextureAtlases = new List<PawnTextureAtlas>();

	private static List<StaticTextureAtlas> staticTextureAtlases = new List<StaticTextureAtlas>();

	private static Dictionary<TextureAtlasGroupKey, (List<Texture2D>, HashSet<Texture2D>)> buildQueue = new Dictionary<TextureAtlasGroupKey, (List<Texture2D>, HashSet<Texture2D>)>();

	private static Dictionary<Texture2D, Texture2D> buildQueueMasks = new Dictionary<Texture2D, Texture2D>();

	public static void ClearStaticAtlasBuildQueue()
	{
		buildQueue.Clear();
	}

	public static bool TryInsertStatic(TextureAtlasGroup group, Texture2D texture, Texture2D mask = null)
	{
		if (((Texture)texture).width >= 512 || ((Texture)texture).height >= 512)
		{
			return false;
		}
		if ((Object)(object)mask != (Object)null && (((Texture)texture).width != ((Texture)mask).width || ((Texture)texture).height != ((Texture)mask).height))
		{
			Log.Warning("Texture " + ((Object)texture).name + " has dimensions of " + ((Texture)texture).width + " x " + ((Texture)texture).height + ", but its mask has " + ((Texture)mask).width + " x " + ((Texture)mask).height + ". This is not supported, texture will be excluded from atlas");
			return false;
		}
		TextureAtlasGroupKey textureAtlasGroupKey = default(TextureAtlasGroupKey);
		textureAtlasGroupKey.group = group;
		textureAtlasGroupKey.hasMask = (Object)(object)mask != (Object)null;
		TextureAtlasGroupKey key = textureAtlasGroupKey;
		if (!buildQueue.TryGetValue(key, out var value))
		{
			value = (new List<Texture2D>(), new HashSet<Texture2D>());
			buildQueue.Add(key, value);
		}
		if (value.Item2.Add(texture))
		{
			value.Item1.Add(texture);
		}
		if ((Object)(object)mask != (Object)null)
		{
			if (buildQueueMasks.ContainsKey(texture))
			{
				if ((Object)(object)buildQueueMasks[texture] != (Object)(object)mask)
				{
					Log.Error("Same texture with 2 different masks inserted into texture atlas manager (" + ((Object)texture).name + ") - " + ((Object)mask).name + " | " + ((Object)buildQueueMasks[texture]).name + "!");
				}
			}
			else
			{
				buildQueueMasks.Add(texture, mask);
			}
		}
		return true;
	}

	public static void BakeStaticAtlases()
	{
		BuildingsDamageSectionLayerUtility.TryInsertIntoAtlas();
		MinifiedThing.TryInsertIntoAtlas();
		int pixels = 0;
		List<Texture2D> currentBatch = new List<Texture2D>();
		foreach (KeyValuePair<TextureAtlasGroupKey, (List<Texture2D>, HashSet<Texture2D>)> item in buildQueue)
		{
			foreach (Texture2D item2 in item.Value.Item1)
			{
				int num = ((Texture)item2).width * ((Texture)item2).height;
				if (num + pixels > StaticTextureAtlas.MaxPixelsPerAtlas)
				{
					FlushBatch(item.Key);
				}
				pixels += num;
				currentBatch.Add(item2);
			}
			FlushBatch(item.Key);
		}
		void FlushBatch(TextureAtlasGroupKey groupKey)
		{
			StaticTextureAtlas staticTextureAtlas = new StaticTextureAtlas(groupKey);
			foreach (Texture2D item3 in currentBatch)
			{
				if (!groupKey.hasMask || !buildQueueMasks.TryGetValue(item3, out var value))
				{
					value = null;
				}
				staticTextureAtlas.Insert(item3, value);
			}
			staticTextureAtlas.Bake();
			staticTextureAtlases.Add(staticTextureAtlas);
			pixels = 0;
			currentBatch.Clear();
		}
	}

	public static bool TryGetStaticTile(TextureAtlasGroup group, Texture2D texture, out StaticTextureAtlasTile tile, bool ignoreFoundInOtherAtlas = false)
	{
		foreach (StaticTextureAtlas staticTextureAtlase in staticTextureAtlases)
		{
			if (staticTextureAtlase.groupKey.group == group && staticTextureAtlase.TryGetTile((Texture)(object)texture, out tile))
			{
				return true;
			}
		}
		foreach (StaticTextureAtlas staticTextureAtlase2 in staticTextureAtlases)
		{
			if (staticTextureAtlase2.TryGetTile((Texture)(object)texture, out tile))
			{
				if (!ignoreFoundInOtherAtlas)
				{
					Log.Warning("Found texture " + ((Object)texture).name + " in another atlas group than requested (found in " + staticTextureAtlase2.groupKey.ToString() + ", requested in " + group.ToString() + ")!");
				}
				return true;
			}
		}
		tile = null;
		return false;
	}

	public static bool TryGetPawnFrameSet(Pawn pawn, out PawnTextureAtlasFrameSet frameSet, out bool createdNew, bool allowCreatingNew = true)
	{
		foreach (PawnTextureAtlas pawnTextureAtlase in pawnTextureAtlases)
		{
			if (pawnTextureAtlase.TryGetFrameSet(pawn, out frameSet, out createdNew))
			{
				return true;
			}
		}
		if (allowCreatingNew)
		{
			PawnTextureAtlas pawnTextureAtlas = new PawnTextureAtlas();
			pawnTextureAtlases.Add(pawnTextureAtlas);
			return pawnTextureAtlas.TryGetFrameSet(pawn, out frameSet, out createdNew);
		}
		createdNew = false;
		frameSet = null;
		return false;
	}

	public static bool TryMarkPawnFrameSetDirty(Pawn pawn)
	{
		if (!TryGetPawnFrameSet(pawn, out var frameSet, out var _, allowCreatingNew: false))
		{
			return false;
		}
		for (int i = 0; i < frameSet.isDirty.Length; i++)
		{
			frameSet.isDirty[i] = true;
		}
		return true;
	}

	public static void GlobalTextureAtlasManagerUpdate()
	{
		if (rebakeAtlas)
		{
			FreeAllRuntimeAtlases();
			PortraitsCache.Clear();
			rebakeAtlas = false;
		}
		foreach (PawnTextureAtlas pawnTextureAtlase in pawnTextureAtlases)
		{
			pawnTextureAtlase.GC();
		}
	}

	public static void FreeAllRuntimeAtlases()
	{
		foreach (PawnTextureAtlas pawnTextureAtlase in pawnTextureAtlases)
		{
			pawnTextureAtlase.Destroy();
		}
		pawnTextureAtlases.Clear();
	}

	public static void DumpPawnAtlases(string folder)
	{
		foreach (PawnTextureAtlas pawnTextureAtlase in pawnTextureAtlases)
		{
			TextureAtlasHelper.WriteDebugPNG(pawnTextureAtlase.RawTexture, folder + "\\dump_" + ((Object)pawnTextureAtlase.RawTexture).GetInstanceID() + ".png");
		}
		Log.Message("Atlases have been dumped to " + folder);
	}

	public static void DumpStaticAtlases(string folder)
	{
		foreach (StaticTextureAtlas staticTextureAtlase in staticTextureAtlases)
		{
			TextureAtlasHelper.WriteDebugPNG(staticTextureAtlase.ColorTexture, folder + "\\" + ((Object)staticTextureAtlase.ColorTexture).name + ".png");
			if ((Object)(object)staticTextureAtlase.MaskTexture != (Object)null)
			{
				TextureAtlasHelper.WriteDebugPNG(staticTextureAtlase.MaskTexture, folder + "\\" + ((Object)staticTextureAtlase.MaskTexture).name + ".png");
			}
		}
		Log.Message("Atlases have been dumped to " + folder);
	}
}
