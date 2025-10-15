using UnityEngine;

namespace Verse;

public class CreditRecord_Role : CreditsEntry
{
	public string roleKey;

	public string creditee;

	public string extra;

	public bool displayKey;

	public bool compressed;

	public bool smallFont;

	public CreditRecord_Role()
	{
	}

	public CreditRecord_Role(string roleKey, string creditee, string extra = null)
	{
		this.roleKey = roleKey;
		this.creditee = creditee;
		this.extra = extra;
	}

	public override float DrawHeight(float width)
	{
		if (roleKey.NullOrEmpty())
		{
			width *= 0.5f;
		}
		Text.Font = (smallFont ? GameFont.Small : GameFont.Medium);
		float result = (compressed ? Text.CalcHeight(creditee, width * 0.5f) : 50f);
		Text.Font = GameFont.Medium;
		return result;
	}

	public override void Draw(Rect rect)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = (smallFont ? GameFont.Small : GameFont.Medium);
		Text.Anchor = (TextAnchor)3;
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = 0f;
		if (!roleKey.NullOrEmpty())
		{
			((Rect)(ref rect2)).width = ((Rect)(ref rect)).width / 2f;
			if (displayKey)
			{
				Widgets.Label(rect2, roleKey);
			}
		}
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref rect2)).xMax;
		if (roleKey.NullOrEmpty())
		{
			Text.Anchor = (TextAnchor)4;
		}
		Widgets.Label(val, creditee);
		Text.Anchor = (TextAnchor)3;
		if (!extra.NullOrEmpty())
		{
			Rect rect3 = val;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 28f;
			Text.Font = GameFont.Tiny;
			GUI.color = new Color(0.7f, 0.7f, 0.7f);
			Widgets.Label(rect3, extra);
			GUI.color = Color.white;
		}
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)0;
	}

	public CreditRecord_Role Compress()
	{
		compressed = true;
		return this;
	}

	public CreditRecord_Role WithSmallFont()
	{
		smallFont = true;
		return this;
	}
}
