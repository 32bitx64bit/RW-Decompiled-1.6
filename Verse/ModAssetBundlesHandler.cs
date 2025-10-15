using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Verse;

public class ModAssetBundlesHandler
{
	private ModContentPack mod;

	public List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

	public static readonly string[] TextureExtensions = new string[4] { ".png", ".psd", ".jpg", ".jpeg" };

	public static readonly string[] AudioClipExtensions = new string[3] { ".wav", ".mp3", ".ogg" };

	public static readonly string[] ShaderExtensions = new string[1] { ".shader" };

	public const string LinuxBundleSuffix = "_linux";

	public const string MacBundleSuffix = "_mac";

	public const string WindowsBundleSuffix = "_win";

	private string BundleSuffixForCurrentOs
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Invalid comparison between Unknown and I4
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			RuntimePlatform platform = Application.platform;
			if ((int)platform <= 2)
			{
				if ((int)platform <= 1)
				{
					return "_mac";
				}
				if ((int)platform == 2)
				{
					goto IL_0032;
				}
			}
			else
			{
				if ((int)platform == 7)
				{
					goto IL_0032;
				}
				if ((int)platform == 13 || (int)platform == 16)
				{
					return "_linux";
				}
			}
			throw new NotSupportedException($"Unsupported platform for asset bundle loading: {Application.platform}");
			IL_0032:
			return "_win";
		}
	}

	public ModAssetBundlesHandler(ModContentPack mod)
	{
		this.mod = mod;
	}

	public void ReloadAll(bool hotReload = false)
	{
		foreach (KeyValuePair<string, FileInfo> item in ModContentPack.GetAllFilesForMod(mod, GenFilePaths.ContentPath<AssetBundle>(), IsAcceptableExtension))
		{
			item.Deconstruct(out var _, out var value);
			FileInfo fileInfo = value;
			string bundleNameWithoutOsSpecifier = GetBundleNameWithoutOsSpecifier(fileInfo);
			if (bundleNameWithoutOsSpecifier != null && !(fileInfo.Name.Replace(bundleNameWithoutOsSpecifier, "") == BundleSuffixForCurrentOs))
			{
				continue;
			}
			AssetBundle val = AssetBundle.LoadFromFile(fileInfo.FullName);
			if ((Object)(object)val != (Object)null)
			{
				if (!loadedAssetBundles.Contains(val))
				{
					loadedAssetBundles.Add(val);
				}
			}
			else
			{
				Log.Error("Could not load asset bundle at " + fileInfo.FullName);
			}
		}
	}

	private bool IsAcceptableExtension(string extension)
	{
		if (!extension.NullOrEmpty())
		{
			return false;
		}
		return true;
	}

	private string GetBundleNameWithoutOsSpecifier(FileInfo file)
	{
		string name = file.Name;
		if (name.EndsWith("_linux"))
		{
			string text = name;
			int length = "_linux".Length;
			return text.Substring(0, text.Length - length);
		}
		if (name.EndsWith("_mac"))
		{
			string text = name;
			int length = "_mac".Length;
			return text.Substring(0, text.Length - length);
		}
		if (name.EndsWith("_win"))
		{
			string text = name;
			int length = "_win".Length;
			return text.Substring(0, text.Length - length);
		}
		return null;
	}

	public void ClearDestroy()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			for (int i = 0; i < loadedAssetBundles.Count; i++)
			{
				loadedAssetBundles[i].Unload(true);
			}
			loadedAssetBundles.Clear();
		});
	}
}
