using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class DamagedMatPool
{
	private static Dictionary<Material, Material> damagedMats = new Dictionary<Material, Material>();

	public static readonly Color DamagedMatStartingColor = Color.red;

	public static int MatCount => damagedMats.Count;

	public static Material GetDamageFlashMat(Material baseMat, float damPct)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (damPct < 0.01f)
		{
			return baseMat;
		}
		if (!damagedMats.TryGetValue(baseMat, out var value))
		{
			value = MaterialAllocator.Create(baseMat);
			damagedMats.Add(baseMat, value);
		}
		value.color = Color.Lerp(baseMat.color, DamagedMatStartingColor, damPct);
		return value;
	}
}
