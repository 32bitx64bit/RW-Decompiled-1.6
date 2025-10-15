using System;
using System.Collections.Generic;
using System.IO;
using KTrie;
using UnityEngine;

namespace Verse;

public class ModContentHolder<T> where T : class
{
	private ModContentPack mod;

	public Dictionary<string, T> contentList = new Dictionary<string, T>();

	public List<IDisposable> extraDisposables = new List<IDisposable>();

	private StringTrieSet contentListTrie = new StringTrieSet();

	public ModContentHolder(ModContentPack mod)
	{
		this.mod = mod;
	}

	public void ClearDestroy()
	{
		if (typeof(Object).IsAssignableFrom(typeof(T)))
		{
			foreach (T value in contentList.Values)
			{
				T localObj = value;
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Expected O, but got Unknown
					Object.Destroy((Object)localObj);
				});
			}
		}
		for (int i = 0; i < extraDisposables.Count; i++)
		{
			extraDisposables[i].Dispose();
		}
		extraDisposables.Clear();
		contentList.Clear();
		contentListTrie.Clear();
	}

	public void ReloadAll(bool hotReload = false)
	{
		foreach (Pair<string, LoadedContentItem<T>> item in ModContentLoader<T>.LoadAllForMod(mod))
		{
			string first = item.First;
			first = first.Replace('\\', '/');
			string text = GenFilePaths.ContentPath<T>();
			if (first.StartsWith(text))
			{
				first = first.Substring(text.Length);
			}
			if (first.EndsWith(Path.GetExtension(first)))
			{
				first = first.Substring(0, first.Length - Path.GetExtension(first).Length);
			}
			if (contentList.ContainsKey(first))
			{
				if (!hotReload)
				{
					Log.Warning("Tried to load duplicate " + typeof(T)?.ToString() + " with path: " + item.Second.internalFile?.ToString() + " and internal path: " + first);
				}
			}
			else
			{
				contentList.Add(first, item.Second.contentItem);
				contentListTrie.Add(first);
				if (item.Second.extraDisposable != null)
				{
					extraDisposables.Add(item.Second.extraDisposable);
				}
			}
		}
	}

	public T Get(string path)
	{
		if (contentList.TryGetValue(path, out var value))
		{
			return value;
		}
		return null;
	}

	public IEnumerable<T> GetAllUnderPath(string pathRoot)
	{
		string prefix = ((pathRoot.NullOrEmpty() || pathRoot[pathRoot.Length - 1] == '/') ? pathRoot : (pathRoot + "/"));
		foreach (string item in contentListTrie.GetByPrefix(prefix))
		{
			yield return contentList[item];
		}
	}
}
