using System.Collections.Generic;
using UnityEngine;

namespace RimWorld.Planet;

public static class WorldTerrainColliderManager
{
	private static readonly Dictionary<PlanetLayer, Dictionary<int, GameObject>> layerWorldTerrainColliders = new Dictionary<PlanetLayer, Dictionary<int, GameObject>>();

	public static void ClearCache()
	{
		foreach (KeyValuePair<PlanetLayer, Dictionary<int, GameObject>> layerWorldTerrainCollider in layerWorldTerrainColliders)
		{
			layerWorldTerrainCollider.Deconstruct(out var _, out var value);
			foreach (KeyValuePair<int, GameObject> item in value)
			{
				item.Deconstruct(out var _, out var value2);
				Object.Destroy((Object)(object)value2);
			}
		}
		layerWorldTerrainColliders.Clear();
	}

	public static void EnsureRaycastCollidersUpdated()
	{
		foreach (var (planetLayer2, dictionary2) in layerWorldTerrainColliders)
		{
			foreach (KeyValuePair<int, GameObject> item in dictionary2)
			{
				item.Deconstruct(out var _, out var value);
				value.SetActive(planetLayer2.Raycastable);
			}
		}
	}

	private static GameObject CreateGameObject(PlanetLayer planetLayer, int layer)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		GameObject val = new GameObject($"{planetLayer} WorldTerrainCollider layer {layer}");
		Object.DontDestroyOnLoad((Object)val);
		val.layer = layer;
		return val;
	}

	public static GameObject Get(PlanetLayer planetLayer, int layer)
	{
		if (!layerWorldTerrainColliders.TryGetValue(planetLayer, out var value))
		{
			value = (layerWorldTerrainColliders[planetLayer] = new Dictionary<int, GameObject>());
		}
		if (!value.TryGetValue(layer, out var value2))
		{
			value2 = (value[layer] = CreateGameObject(planetLayer, layer));
		}
		value2.SetActive(false);
		return value2;
	}
}
