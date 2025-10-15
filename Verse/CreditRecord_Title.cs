using UnityEngine;

namespace Verse;

public class CreditRecord_Title : CreditsEntry
{
	public string title;

	public CreditRecord_Title()
	{
	}

	public CreditRecord_Title(string title)
	{
		this.title = title;
	}

	public override float DrawHeight(float width)
	{
		return 100f;
	}

	public override void Draw(Rect rect)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 31f;
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)4;
		Widgets.Label(rect, title);
		Text.Anchor = (TextAnchor)0;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		Widgets.DrawLineHorizontal(((Rect)(ref rect)).x + 10f, Mathf.Round(((Rect)(ref rect)).yMax) - 14f, ((Rect)(ref rect)).width - 20f);
		GUI.color = Color.white;
	}
}
