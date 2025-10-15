using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Verse;

public static class ContentFinder<T> where T : class
{
	public static T Get(string itemPath, bool reportFailure = true)
	{
		if (!UnityData.IsInMainThread)
		{
			Log.Error("Tried to get a resource \"" + itemPath + "\" from a different thread. All resources must be loaded in the main thread.");
			return null;
		}
		T val = null;
		if (typeof(T) != typeof(Shader))
		{
			List<ModContentPack> runningModsListForReading = LoadedModManager.RunningModsListForReading;
			for (int num = runningModsListForReading.Count - 1; num >= 0; num--)
			{
				val = runningModsListForReading[num].GetContentHolder<T>().Get(itemPath);
				if (val != null)
				{
					return val;
				}
			}
		}
		if (typeof(T) == typeof(Texture2D))
		{
			val = (T)(object)Resources.Load<Texture2D>(GenFilePaths.ContentPath<Texture2D>() + itemPath);
		}
		if (typeof(T) == typeof(AudioClip))
		{
			val = (T)(object)Resources.Load<AudioClip>(GenFilePaths.ContentPath<AudioClip>() + itemPath);
		}
		if (val != null)
		{
			return val;
		}
		if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(AudioClip) || typeof(T) == typeof(Shader))
		{
			T val2 = TryFindAssetInModBundles(itemPath);
			if (val2 != null)
			{
				return val2;
			}
		}
		if (reportFailure)
		{
			string text = ((ContentFinderRequester.requester != null) ? (" for def '" + ContentFinderRequester.requester.defName + "'") : "");
			Log.Error("Could not load " + typeof(T).Name + " at '" + itemPath + "'" + text + " in any active mod or in base resources.");
		}
		return null;
	}

	public static T TryFindAssetInModBundles(string itemPath)
	{
		string[] array;
		if (typeof(T) == typeof(Texture2D))
		{
			array = ModAssetBundlesHandler.TextureExtensions;
		}
		else if (typeof(T) == typeof(AudioClip))
		{
			array = ModAssetBundlesHandler.AudioClipExtensions;
		}
		else
		{
			if (!(typeof(T) == typeof(Shader)))
			{
				throw new NotSupportedException($"Unsupported type {typeof(T)} for asset bundle loading. Did you forget to add a new type?");
			}
			array = ModAssetBundlesHandler.ShaderExtensions;
		}
		List<ModContentPack> runningModsListForReading = LoadedModManager.RunningModsListForReading;
		string path = Path.Combine("Assets", "Data");
		for (int num = runningModsListForReading.Count - 1; num >= 0; num--)
		{
			string path2 = Path.Combine(path, runningModsListForReading[num].FolderName);
			string path3 = Path.Combine(path, runningModsListForReading[num].PackageIdPlayerFacing);
			bool flag = !runningModsListForReading[num].IsOfficialMod;
			foreach (AssetBundle loadedAssetBundle in runningModsListForReading[num].assetBundles.loadedAssetBundles)
			{
				string text = Path.Combine(Path.Combine(path2, GenFilePaths.ContentPath<T>()), itemPath);
				string text2 = Path.Combine(Path.Combine(path3, GenFilePaths.ContentPath<T>()), itemPath);
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					if (loadedAssetBundle.LoadAsset(text + text3, typeof(T)) is T result)
					{
						return result;
					}
					if (flag && loadedAssetBundle.LoadAsset(text2 + text3, typeof(T)) is T result2)
					{
						return result2;
					}
				}
			}
		}
		return null;
	}

	public static IEnumerable<T> GetAllInFolder(string folderPath)
	{
		if (!UnityData.IsInMainThread)
		{
			Log.Error("Tried to get all resources in a folder \"" + folderPath + "\" from a different thread. All resources must be loaded in the main thread.");
			yield break;
		}
		foreach (ModContentPack item in LoadedModManager.RunningModsListForReading)
		{
			foreach (T item2 in item.GetContentHolder<T>().GetAllUnderPath(folderPath))
			{
				yield return item2;
			}
		}
		T[] array = null;
		if (typeof(T) == typeof(Texture2D))
		{
			array = (T[])(object)Resources.LoadAll<Texture2D>(GenFilePaths.ContentPath<Texture2D>() + folderPath);
		}
		if (typeof(T) == typeof(AudioClip))
		{
			array = (T[])(object)Resources.LoadAll<AudioClip>(GenFilePaths.ContentPath<AudioClip>() + folderPath);
		}
		if (array != null)
		{
			T[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				yield return array2[j];
			}
		}
		List<ModContentPack> mods = LoadedModManager.RunningModsListForReading;
		string modsDir = Path.Combine("Assets", "Data");
		if (!(typeof(T) == typeof(Texture2D)) && !(typeof(T) == typeof(AudioClip)))
		{
			yield break;
		}
		for (int j = mods.Count - 1; j >= 0; j--)
		{
			string dirForBundleWithFolderName = Path.Combine(modsDir, mods[j].FolderName);
			string dirForBundleWithPackageId = Path.Combine(modsDir, mods[j].PackageIdPlayerFacing);
			bool canLoadViaPackageId = !mods[j].IsOfficialMod;
			string[] validExtensions;
			if (typeof(T) == typeof(Texture2D))
			{
				validExtensions = ModAssetBundlesHandler.TextureExtensions;
			}
			else
			{
				if (!(typeof(T) == typeof(AudioClip)))
				{
					throw new NotSupportedException($"Unsupported type {typeof(T)} for asset bundle loading. Did you forget to add a new type?");
				}
				validExtensions = ModAssetBundlesHandler.AudioClipExtensions;
			}
			for (int i = 0; i < mods[j].assetBundles.loadedAssetBundles.Count; i++)
			{
				AssetBundle assetBundle = mods[j].assetBundles.loadedAssetBundles[i];
				string text = Path.Combine(Path.Combine(dirForBundleWithFolderName, GenFilePaths.ContentPath<T>()).Replace('\\', '/'), folderPath).ToLower();
				string pathWithoutExtWithPackageId = Path.Combine(Path.Combine(dirForBundleWithPackageId, GenFilePaths.ContentPath<T>()).Replace('\\', '/'), folderPath).ToLower();
				string text2 = text;
				if (text2[text2.Length - 1] != '/')
				{
					text += "/";
				}
				string text3 = pathWithoutExtWithPackageId;
				if (text3[text3.Length - 1] != '/')
				{
					pathWithoutExtWithPackageId += "/";
				}
				IEnumerable<string> byPrefix = mods[j].AllAssetNamesInBundleTrie(i).GetByPrefix(text);
				foreach (string item3 in byPrefix)
				{
					if (validExtensions.Contains<string>(Path.GetExtension(item3)))
					{
						yield return (T)(object)assetBundle.LoadAsset(item3, typeof(T));
					}
				}
				if (!canLoadViaPackageId)
				{
					continue;
				}
				IEnumerable<string> byPrefix2 = mods[j].AllAssetNamesInBundleTrie(i).GetByPrefix(pathWithoutExtWithPackageId);
				foreach (string item4 in byPrefix2)
				{
					if (validExtensions.Contains<string>(Path.GetExtension(item4)))
					{
						yield return (T)(object)assetBundle.LoadAsset(item4, typeof(T));
					}
				}
			}
		}
	}
}
