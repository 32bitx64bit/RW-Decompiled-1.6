using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class ZoneColorUtility
{
	private static List<Color> growingZoneColors;

	private static List<Color> storageZoneColors;

	private static List<Color> fishingZoneColors;

	private static int nextGrowingZoneColorIndex;

	private static int nextStorageZoneColorIndex;

	private static int nextFishingZoneColorIndex;

	private const float ZoneOpacity = 0.09f;

	static ZoneColorUtility()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		growingZoneColors = new List<Color>();
		storageZoneColors = new List<Color>();
		fishingZoneColors = new List<Color>();
		nextGrowingZoneColorIndex = 0;
		nextStorageZoneColorIndex = 0;
		nextFishingZoneColorIndex = 0;
		Color item = default(Color);
		foreach (Color item4 in GrowingZoneColors())
		{
			((Color)(ref item))._002Ector(item4.r, item4.g, item4.b, 0.09f);
			growingZoneColors.Add(item);
		}
		Color item2 = default(Color);
		foreach (Color item5 in StorageZoneColors())
		{
			((Color)(ref item2))._002Ector(item5.r, item5.g, item5.b, 0.09f);
			storageZoneColors.Add(item2);
		}
		Color item3 = default(Color);
		foreach (Color item6 in FishingZoneColors())
		{
			((Color)(ref item3))._002Ector(item6.r, item6.g, item6.b, 0.09f);
			fishingZoneColors.Add(item3);
		}
	}

	public static Color NextGrowingZoneColor()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Color result = growingZoneColors[nextGrowingZoneColorIndex];
		nextGrowingZoneColorIndex++;
		if (nextGrowingZoneColorIndex >= growingZoneColors.Count)
		{
			nextGrowingZoneColorIndex = 0;
		}
		return result;
	}

	public static Color NextStorageZoneColor()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Color result = storageZoneColors[nextStorageZoneColorIndex];
		nextStorageZoneColorIndex++;
		if (nextStorageZoneColorIndex >= storageZoneColors.Count)
		{
			nextStorageZoneColorIndex = 0;
		}
		return result;
	}

	public static Color NextFishingZoneColor()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Color result = fishingZoneColors[nextFishingZoneColorIndex];
		nextFishingZoneColorIndex++;
		if (nextFishingZoneColorIndex >= fishingZoneColors.Count)
		{
			nextFishingZoneColorIndex = 0;
		}
		return result;
	}

	public static IEnumerable<Color> GrowingZoneColors()
	{
		yield return Color.Lerp(new Color(0f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 1f, 0.5f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 1f, 0.5f), Color.gray, 0.5f);
	}

	public static IEnumerable<Color> StorageZoneColors()
	{
		yield return Color.Lerp(new Color(1f, 0f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 0f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0f, 0f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 0f, 0.5f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0f, 0.5f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 0f, 1f), Color.gray, 0.5f);
	}

	public static IEnumerable<Color> FishingZoneColors()
	{
		yield return Color.Lerp(new Color(1f, 0.8f, 0f), Color.gray, 0.25f);
		yield return Color.Lerp(new Color(1f, 41f / 85f, 0f), Color.gray, 0.25f);
		yield return Color.Lerp(new Color(1f, 0.36862746f, 0f), Color.gray, 0.25f);
		yield return Color.Lerp(new Color(1f, 0.96862745f, 0f), Color.gray, 0.25f);
		yield return Color.Lerp(new Color(1f, 0.14901961f, 0f), Color.gray, 0.25f);
	}
}
