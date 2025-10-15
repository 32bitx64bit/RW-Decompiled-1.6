using UnityEngine;

namespace Verse;

public class CreditRecord_RoleTwoCols : CreditsEntry
{
	public string creditee1;

	public string creditee2;

	public string extra;

	public bool compressed;

	public CreditRecord_RoleTwoCols()
	{
	}

	public CreditRecord_RoleTwoCols(string creditee1, string creditee2, string extra = null)
	{
		this.creditee1 = creditee1;
		this.creditee2 = creditee2;
		this.extra = extra;
	}

	public override float DrawHeight(float width)
	{
		float num = Text.CalcHeight(creditee1, width * 0.5f);
		float num2 = Text.CalcHeight(creditee2, width * 0.5f);
		if (!compressed)
		{
			return 50f;
		}
		return Mathf.Max(num, num2);
	}

	public override void Draw(Rect rect)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)3;
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = 0f;
		((Rect)(ref rect2)).width = ((Rect)(ref rect)).width / 2f;
		Widgets.Label(rect2, creditee1);
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref rect2)).xMax;
		Widgets.Label(val, creditee2);
		if (!extra.NullOrEmpty())
		{
			Rect rect3 = val;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 28f;
			Text.Font = GameFont.Tiny;
			GUI.color = new Color(0.7f, 0.7f, 0.7f);
			Widgets.Label(rect3, extra);
			GUI.color = Color.white;
		}
	}

	public CreditRecord_RoleTwoCols Compress()
	{
		compressed = true;
		return this;
	}
}
