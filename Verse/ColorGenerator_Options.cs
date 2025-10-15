using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class ColorGenerator_Options : ColorGenerator
{
	public List<ColorOption> options = new List<ColorOption>();

	public override Color ExemplaryColor
	{
		get
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			ColorOption colorOption = null;
			for (int i = 0; i < options.Count; i++)
			{
				if (colorOption == null || options[i].weight > colorOption.weight)
				{
					colorOption = options[i];
				}
			}
			if (colorOption == null)
			{
				return Color.white;
			}
			if (colorOption.only.a >= 0f)
			{
				return colorOption.only;
			}
			return new Color((colorOption.min.r + colorOption.max.r) / 2f, (colorOption.min.g + colorOption.max.g) / 2f, (colorOption.min.b + colorOption.max.b) / 2f, (colorOption.min.a + colorOption.max.a) / 2f);
		}
	}

	public override Color NewRandomizedColor()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return options.RandomElementByWeight((ColorOption pi) => pi.weight).RandomizedColor();
	}
}
