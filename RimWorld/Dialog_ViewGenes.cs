using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_ViewGenes : Window
{
	private Pawn target;

	private Vector2 scrollPosition;

	private const float HeaderHeight = 30f;

	public override Vector2 InitialSize => new Vector2(736f, 700f);

	public Dialog_ViewGenes(Pawn target)
	{
		this.target = target;
		closeOnClickedOutside = true;
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckBiotech("genes viewing"))
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		((Rect)(ref inRect)).yMax = ((Rect)(ref inRect)).yMax - Window.CloseButSize.y;
		Rect rect = inRect;
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 34f;
		Text.Font = GameFont.Medium;
		Widgets.Label(rect, "ViewGenes".Translate() + ": " + target.genes.XenotypeLabelCap);
		Text.Font = GameFont.Small;
		GUI.color = XenotypeDef.IconColor;
		GUI.DrawTexture(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, 30f, 30f), (Texture)(object)target.genes.XenotypeIcon);
		GUI.color = Color.white;
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 34f;
		Vector2 size = Vector2.zero;
		GeneUIUtility.DrawGenesInfo(inRect, target, InitialSize.y, ref size, ref scrollPosition);
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).xMax - Window.CloseButSize.x, ((Rect)(ref inRect)).yMax, Window.CloseButSize.x, Window.CloseButSize.y), "Close".Translate()))
		{
			Close();
		}
	}
}
