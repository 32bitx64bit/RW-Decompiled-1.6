using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_ChooseColor : Window
{
	private string header;

	private Color selectedColor;

	private Action<Color> onApply;

	private List<Color> colors;

	private Vector2 scrollPosition;

	private const float HeaderHeight = 35f;

	private const int ColorSize = 22;

	private const int ColorPadding = 2;

	public override Vector2 InitialSize => new Vector2(500f, 410f);

	public Dialog_ChooseColor(string header, Color selectedColor, List<Color> colors, Action<Color> onApply)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.header = header;
		this.selectedColor = selectedColor;
		this.onApply = onApply;
		this.colors = colors;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Widgets.Label(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y, ((Rect)(ref inRect)).width, 35f), header);
		Text.Font = GameFont.Small;
		Widgets.ColorSelectorIcon(GenUI.ContractedBy(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).y + 35f + 10f, 88f, 88f), 2f), null, selectedColor, drawColor: true);
		Rect val = inRect;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + 105f;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 45f;
		((Rect)(ref val)).height = ((Rect)(ref val)).height - (Window.CloseButSize.y + 10f);
		float num = (float)colors.Count / (((Rect)(ref val)).width / 24f);
		Rect viewRect = val;
		((Rect)(ref viewRect)).width = ((Rect)(ref viewRect)).width - 16f;
		((Rect)(ref viewRect)).height = num * 24f;
		Widgets.BeginScrollView(val, ref scrollPosition, viewRect);
		Widgets.ColorSelector(val, ref selectedColor, colors, out var _);
		Widgets.EndScrollView();
		Text.Font = GameFont.Small;
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).height - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "CloseButton".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).width - Window.CloseButSize.x, ((Rect)(ref inRect)).height - Window.CloseButSize.y, Window.CloseButSize.x, Window.CloseButSize.y), "OK".Translate()))
		{
			onApply?.Invoke(selectedColor);
			Close();
		}
	}
}
