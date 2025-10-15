using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using RimWorld.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Verse;

public static class ModContentLoader<T> where T : class
{
	private static string[] AcceptableExtensionsAudio = new string[7] { ".wav", ".mp3", ".ogg", ".xm", ".it", ".mod", ".s3m" };

	private static string[] AcceptableExtensionsTexture = new string[5] { ".png", ".jpg", ".jpeg", ".psd", ".dds" };

	private static string[] AcceptableExtensionsString = new string[1] { ".txt" };

	public static bool IsAcceptableExtension(string extension)
	{
		string[] array;
		if (typeof(T) == typeof(AudioClip))
		{
			array = AcceptableExtensionsAudio;
		}
		else if (typeof(T) == typeof(Texture2D))
		{
			array = AcceptableExtensionsTexture;
		}
		else
		{
			if (!(typeof(T) == typeof(string)))
			{
				Log.Error("Unknown content type " + typeof(T));
				return false;
			}
			array = AcceptableExtensionsString;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (extension.ToLower() == text)
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<Pair<string, LoadedContentItem<T>>> LoadAllForMod(ModContentPack mod)
	{
		DeepProfiler.Start("Loading assets of type " + typeof(T)?.ToString() + " for mod " + mod);
		Dictionary<string, FileInfo> allFilesForMod = ModContentPack.GetAllFilesForMod(mod, GenFilePaths.ContentPath<T>(), IsAcceptableExtension);
		HashSet<string> ddsFiles = (from k in allFilesForMod.Keys
			select k.ToLowerInvariant() into k
			where k.EndsWith(".dds")
			select k).ToHashSet();
		foreach (KeyValuePair<string, FileInfo> item in allFilesForMod)
		{
			item.Deconstruct(out var key, out var value);
			string text = key;
			FileInfo fileInfo = value;
			if (typeof(T) == typeof(Texture2D) && text.Length > 4)
			{
				int length = text.Length;
				int num = length - 4;
				if (!text.Substring(num, length - num).Equals(".dds", StringComparison.OrdinalIgnoreCase))
				{
					key = text.ToLowerInvariant();
					if (ddsFiles.Contains(key.Substring(0, key.Length - 4) + ".dds"))
					{
						continue;
					}
				}
			}
			LoadedContentItem<T> loadedContentItem = LoadItem((FilesystemFile)fileInfo);
			if (loadedContentItem != null)
			{
				yield return new Pair<string, LoadedContentItem<T>>(text, loadedContentItem);
			}
		}
		DeepProfiler.End();
	}

	public static LoadedContentItem<T> LoadItem(VirtualFile file)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		try
		{
			if (typeof(T) == typeof(string))
			{
				return new LoadedContentItem<T>(file, (T)(object)file.ReadAllText());
			}
			if (typeof(T) == typeof(Texture2D))
			{
				return new LoadedContentItem<T>(file, (T)(object)LoadTexture(file));
			}
			if (typeof(T) == typeof(AudioClip))
			{
				IDisposable extraDisposable = null;
				string text = GenFilePaths.SafeURIForUnityWWWFromPath(file.FullPath);
				new Uri(text);
				DownloadHandlerAudioClip val = new DownloadHandlerAudioClip(text, GetAudioTypeFromURI(text));
				val.streamAudio = ShouldStreamAudioClipFromFile(file);
				UnityWebRequest val2 = new UnityWebRequest(text, "GET", (DownloadHandler)(object)val, (UploadHandler)null);
				T val3;
				try
				{
					val2.SendWebRequest();
					while (!val2.isDone)
					{
						Thread.Sleep(1);
					}
					if (val2.error != null)
					{
						throw new InvalidOperationException("Unity reported error: " + val2.error + " while loading clip from " + text);
					}
					val3 = (T)(object)val.audioClip;
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				Object val4 = (Object)(object)((val3 is Object) ? val3 : null);
				if (val4 != (Object)null)
				{
					val4.name = Path.GetFileNameWithoutExtension(file.Name);
				}
				return new LoadedContentItem<T>(file, val3, extraDisposable);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"Exception loading {typeof(T)} from file.\nabsFilePath: {file.FullPath}\nException: {arg}");
		}
		if (typeof(T) == typeof(Texture2D))
		{
			return (LoadedContentItem<T>)(object)new LoadedContentItem<Texture2D>(file, BaseContent.BadTex);
		}
		return null;
	}

	private static AudioType GetAudioTypeFromURI(string uri)
	{
		if (!uri.EndsWith(".ogg"))
		{
			return (AudioType)20;
		}
		return (AudioType)14;
	}

	private static bool ShouldStreamAudioClipFromFile(VirtualFile file)
	{
		if (!(file is FilesystemFile) || !file.Exists)
		{
			return false;
		}
		return file.Length > 307200;
	}

	private static Texture2D LoadTexture(VirtualFile file)
	{
		if (!file.Exists)
		{
			return null;
		}
		if (file.Name.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
		{
			return ModDdsLoader.TryLoadDds(file);
		}
		Texture2D obj = LoadTextureViaImageConversion(file);
		((Object)obj).name = Path.GetFileNameWithoutExtension(file.Name);
		return obj;
	}

	private static Texture2D LoadTextureViaImageConversion(VirtualFile file)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		byte[] array = file.ReadAllBytes();
		Texture2D val = new Texture2D(2, 2, (TextureFormat)1, true);
		ImageConversion.LoadImage(val, array);
		if ((((Texture)val).width < 4 || ((Texture)val).height < 4 || !Mathf.IsPowerOfTwo(((Texture)val).width) || !Mathf.IsPowerOfTwo(((Texture)val).height)) && Prefs.TextureCompression)
		{
			int num = StaticTextureAtlas.CalculateMaxMipmapsForDxtSupport(val);
			if (Prefs.LogVerbose)
			{
				Log.Warning($"Texture {file.Name} is being reloaded with reduced mipmap count (clamped to {num}) due to non-power-of-two dimensions: ({((Texture)val).width}x{((Texture)val).height}). This will be slower to load, and will look worse when zoomed out. Consider using a power-of-two texture size instead.");
			}
			if (!UnityData.ComputeShadersSupported)
			{
				Texture2D val2 = new Texture2D(((Texture)val).width, ((Texture)val).height, (TextureFormat)1, num, false);
				Object.DestroyImmediate((Object)(object)val);
				val = val2;
				ImageConversion.LoadImage(val, array);
			}
		}
		bool flag = ((Texture)val).width % 4 == 0 && ((Texture)val).height % 4 == 0;
		if (Prefs.TextureCompression && flag)
		{
			if (!UnityData.ComputeShadersSupported)
			{
				val.Compress(true);
				((Texture)val).filterMode = (FilterMode)2;
				((Texture)val).anisoLevel = 2;
				val.Apply(true, true);
			}
			else
			{
				((Texture)val).filterMode = (FilterMode)2;
				((Texture)val).anisoLevel = 2;
				val.Apply(true, true);
				val = StaticTextureAtlas.FastCompressDXT(val, deleteOriginal: true);
			}
		}
		else
		{
			((Texture)val).filterMode = (FilterMode)2;
			((Texture)val).anisoLevel = 2;
			val.Apply(true, true);
		}
		return val;
	}
}
