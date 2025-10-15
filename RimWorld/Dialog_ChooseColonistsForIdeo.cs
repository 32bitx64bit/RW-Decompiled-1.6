using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_ChooseColonistsForIdeo : Window
{
	private Ideo ideo;

	private List<Pawn> pawns = new List<Pawn>();

	private Func<Pawn, bool> canChangeIdeo;

	private Func<Pawn, Ideo> originalIdeo;

	private Func<Pawn, Ideo> pawnIdeoGetter;

	private Action<Pawn, Ideo> pawnIdeoSetter;

	private const float HeaderLabelHeight = 40f;

	private readonly Vector2 BottomButtonSize = new Vector2(160f, 40f);

	private const float BottomAreaHeight = 55f;

	private const float RowButtonWidth = 150f;

	public override Vector2 InitialSize => new Vector2(500f, 600f);

	public Dialog_ChooseColonistsForIdeo(Ideo ideo, IEnumerable<Pawn> pawns, Func<Pawn, bool> canChangeIdeo, Func<Pawn, Ideo> originalIdeo, Func<Pawn, Ideo> pawnIdeoGetter = null, Action<Pawn, Ideo> pawnIdeoSetter = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		forcePause = true;
		closeOnCancel = false;
		absorbInputAroundWindow = true;
		this.ideo = ideo;
		this.pawns.AddRange(pawns);
		this.canChangeIdeo = canChangeIdeo;
		this.originalIdeo = originalIdeo;
		this.pawnIdeoGetter = pawnIdeoGetter;
		this.pawnIdeoSetter = pawnIdeoSetter;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(new Rect(0f, 0f, ((Rect)(ref inRect)).width, 40f), "ChooseColonistsForIdeoTitle".Translate());
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)0;
		Widgets.Label(new Rect(0f, 40f, ((Rect)(ref inRect)).width, 40f), "ChooseColonistsForIdeoDesc".Translate());
		((Rect)(ref inRect)).yMin = ((Rect)(ref inRect)).yMin + 112f;
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.ColumnWidth = ((Rect)(ref inRect)).width;
		listing_Standard.Begin(inRect);
		for (int i = 0; i < pawns.Count; i++)
		{
			Pawn pawn = pawns[i];
			Rect rect = listing_Standard.GetRect(24f);
			if (i % 2 == 0)
			{
				Widgets.DrawLightHighlight(rect);
			}
			Widgets.BeginGroup(rect);
			WidgetRow widgetRow = new WidgetRow(0f, 0f);
			RenderTexture tex = PortraitsCache.Get(pawn, new Vector2(24f, 24f), Rot4.South);
			widgetRow.Icon((Texture)(object)tex);
			Ideo ideo = ((pawnIdeoGetter != null) ? pawnIdeoGetter(pawn) : pawn.Ideo);
			GUI.color = ideo.Color;
			widgetRow.Icon((Texture)(object)ideo.Icon);
			GUI.color = Color.white;
			widgetRow.Label(pawn.LabelShortCap);
			float width = listing_Standard.ColumnWidth - widgetRow.FinalX - 150f;
			widgetRow.Gap(width);
			if (canChangeIdeo(pawn))
			{
				if (ideo == this.ideo)
				{
					if (widgetRow.ButtonText("RevertToPreviousIdeoligion".Translate()))
					{
						if (pawnIdeoSetter != null)
						{
							pawnIdeoSetter(pawn, originalIdeo(pawn));
						}
						else
						{
							pawn.ideo.SetIdeo(originalIdeo(pawn));
						}
					}
				}
				else if (widgetRow.ButtonText("ConvertToPlayerIdeoligion".Translate()))
				{
					if (pawnIdeoSetter != null)
					{
						pawnIdeoSetter(pawn, this.ideo);
					}
					else
					{
						pawn.ideo.SetIdeo(this.ideo);
					}
				}
			}
			else
			{
				widgetRow.Label("ExistingFollowerOfPlayerIdeoligion".Translate());
			}
			Widgets.EndGroup();
			listing_Standard.Gap(6f);
		}
		listing_Standard.End();
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).width / 2f - BottomButtonSize.x / 2f, ((Rect)(ref inRect)).yMax - 55f, BottomButtonSize.x, BottomButtonSize.y), "Close".Translate()))
		{
			Close();
		}
	}
}
