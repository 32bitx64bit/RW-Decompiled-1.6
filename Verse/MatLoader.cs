using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MatLoader
{
	private struct Request
	{
		public string path;

		public int renderQueue;

		public override int GetHashCode()
		{
			return Gen.HashCombineInt(Gen.HashCombine(0, path), renderQueue);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Request))
			{
				return false;
			}
			return Equals((Request)obj);
		}

		public bool Equals(Request other)
		{
			if (other.path == path)
			{
				return other.renderQueue == renderQueue;
			}
			return false;
		}

		public static bool operator ==(Request lhs, Request rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Request lhs, Request rhs)
		{
			return !(lhs == rhs);
		}

		public override string ToString()
		{
			return "MatLoader.Request(" + path + ", " + renderQueue + ")";
		}
	}

	private static Dictionary<Request, Material> dict = new Dictionary<Request, Material>();

	private static Dictionary<Material, string> loadedDirect = new Dictionary<Material, string>();

	public static void ClearCache()
	{
		dict.Clear();
	}

	public static Material LoadMat(string matPath, int renderQueue = -1)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		Material val = (Material)Resources.Load("Materials/" + matPath, typeof(Material));
		if ((Object)(object)val == (Object)null)
		{
			Log.Warning("Could not load material " + matPath);
		}
		Request request = default(Request);
		request.path = matPath;
		request.renderQueue = renderQueue;
		Request key = request;
		if (!dict.TryGetValue(key, out var value))
		{
			value = MaterialAllocator.Create(val);
			if (renderQueue != -1)
			{
				value.renderQueue = renderQueue;
			}
			dict.Add(key, value);
		}
		return value;
	}

	public static Material LoadMatDirect(string matPath)
	{
		Material val = Resources.Load<Material>("Materials/" + matPath);
		if ((Object)(object)val == (Object)null)
		{
			throw new Exception("Material not found: " + matPath);
		}
		if (loadedDirect.ContainsKey(val))
		{
			Debug.LogWarning((object)("Material '" + matPath + "' has been loaded multiple times. Mutations to this material might cause bugs"));
		}
		return val;
	}
}
