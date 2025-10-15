using UnityEngine;
using Verse;

namespace RimWorld;

public class ITab_Book : ITab
{
	private Vector2 infoScroll;

	protected const float TopPadding = 20f;

	protected const float InitialHeight = 350f;

	public const float TitleHeight = 30f;

	protected const float InitialWidth = 610f;

	public override bool IsVisible => base.SelThing is Book;

	public ITab_Book()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(Mathf.Min(610f, (float)UI.screenWidth), 350f);
		labelKey = "TabBookContents";
	}

	protected override void FillTab()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (base.SelThing is Book book)
		{
			Rect val = GenUI.ContractedBy(new Rect(0f, 20f, size.x, size.y - 20f), 10f);
			Rect rect = val;
			((Rect)(ref rect)).y = 10f;
			((Rect)(ref rect)).height = 30f;
			Rect rect2 = val;
			((Rect)(ref rect2)).xMax = ((Rect)(ref val)).center.x - 17f;
			((Rect)(ref rect2)).y = ((Rect)(ref rect)).yMax + 17f;
			((Rect)(ref rect2)).yMax = ((Rect)(ref val)).yMax;
			Rect rect3 = val;
			((Rect)(ref rect3)).x = ((Rect)(ref rect2)).xMax + 20f;
			((Rect)(ref rect3)).xMax = ((Rect)(ref val)).xMax + 10f;
			((Rect)(ref rect3)).y = ((Rect)(ref rect)).yMax + 26f;
			((Rect)(ref rect3)).yMax = ((Rect)(ref val)).yMax;
			BookUIUtility.DrawTitle(rect, book);
			BookUIUtility.DrawBookInfoPanel(rect2, book);
			BookUIUtility.DrawBookDescPanel(rect3, book, ref infoScroll);
		}
	}
}
