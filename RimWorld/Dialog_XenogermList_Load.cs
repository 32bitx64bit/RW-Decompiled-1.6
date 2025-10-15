using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_XenogermList_Load : Window
{
	protected float bottomAreaHeight;

	protected Vector2 scrollPosition = Vector2.zero;

	private Action<CustomXenogerm> loadAction;

	protected const float EntryHeight = 40f;

	protected const float NameLeftMargin = 8f;

	protected const float NameRightMargin = 4f;

	protected const float InfoWidth = 94f;

	protected const float InteractButWidth = 100f;

	protected const float InteractButHeight = 36f;

	protected const float DeleteButSize = 36f;

	public Dialog_XenogermList_Load(Action<CustomXenogerm> loadAction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		doCloseButton = true;
		doCloseX = true;
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		this.loadAction = loadAction;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		List<CustomXenogerm> customXenogermsForReading = Find.CustomXenogermDatabase.CustomXenogermsForReading;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref inRect)).width - 16f, 40f);
		float y = val.y;
		float num = (float)customXenogermsForReading.Count * y;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, num);
		float height = ((Rect)(ref inRect)).height - Window.CloseButSize.y - bottomAreaHeight - 18f;
		Rect outRect = inRect.TopPartPixels(height);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num2 = 0f;
		Rect rect = default(Rect);
		Rect val2 = default(Rect);
		Rect rect2 = default(Rect);
		for (int num3 = customXenogermsForReading.Count - 1; num3 >= 0; num3--)
		{
			CustomXenogerm xenogerm = customXenogermsForReading[num3];
			if (num2 + val.y >= scrollPosition.y && num2 <= scrollPosition.y + ((Rect)(ref outRect)).height)
			{
				((Rect)(ref rect))._002Ector(0f, num2, val.x, val.y);
				if (num3 % 2 == 0)
				{
					Widgets.DrawAltRect(rect);
				}
				Widgets.BeginGroup(rect);
				((Rect)(ref val2))._002Ector(((Rect)(ref rect)).width - 36f, (((Rect)(ref rect)).height - 36f) / 2f, 36f, 36f);
				if (Widgets.ButtonImage(val2, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(xenogerm.name), delegate
					{
						Find.CustomXenogermDatabase.Remove(xenogerm);
					}, destructive: true));
				}
				TooltipHandler.TipRegionByKey(val2, "DeleteThisXenogerm");
				Text.Font = GameFont.Small;
				if (Widgets.ButtonText(new Rect(((Rect)(ref val2)).x - 100f, (((Rect)(ref rect)).height - 36f) / 2f, 100f, 36f), "LoadGameButton".Translate()))
				{
					loadAction(xenogerm);
					Close();
				}
				GUI.color = Color.white;
				((Rect)(ref rect2))._002Ector(8f, 0f, ((Rect)(ref rect)).width - 4f, ((Rect)(ref rect)).height);
				Text.Anchor = (TextAnchor)3;
				Text.Font = GameFont.Small;
				Widgets.Label(rect2, xenogerm.name.Truncate(((Rect)(ref rect2)).width * 1.8f));
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				Widgets.EndGroup();
			}
			num2 += val.y;
		}
		Widgets.EndScrollView();
	}
}
