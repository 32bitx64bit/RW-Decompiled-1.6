using UnityEngine;

namespace Verse;

public class CreditRecord_TitleLocalization : CreditsEntry
{
	public string title;

	public CreditRecord_TitleLocalization()
	{
	}

	public CreditRecord_TitleLocalization(string title)
	{
		this.title = title;
	}

	public override float DrawHeight(float width)
	{
		return 40f;
	}

	public override void Draw(Rect rect)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, title);
		Text.Anchor = (TextAnchor)0;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		Widgets.DrawLineHorizontal(((Rect)(ref rect)).x + 10f, Mathf.Round(((Rect)(ref rect)).yMax) - 14f, ((Rect)(ref rect)).width - 20f);
		GUI.color = Color.white;
	}
}
