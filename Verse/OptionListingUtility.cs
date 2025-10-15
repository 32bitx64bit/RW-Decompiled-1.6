using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class OptionListingUtility
{
	private const float OptionSpacing = 7f;

	public static float DrawOptionListing(Rect rect, List<ListableOption> optList)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Small;
		foreach (ListableOption opt in optList)
		{
			num += opt.DrawOption(new Vector2(0f, num), ((Rect)(ref rect)).width) + 7f;
		}
		Widgets.EndGroup();
		return num;
	}
}
