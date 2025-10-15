using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_SelectXenotypeIcon : Window
{
	private Vector2 scrollPosition;

	private float scrollHeight;

	private XenotypeIconDef selected;

	private Action<XenotypeIconDef> iconSelector;

	private const float HeaderHeight = 35f;

	private const float IconSize = 35f;

	private const float IconGap = 6f;

	private const int IconsPerRow = 8;

	private static readonly Color OutlineColorSelected = new Color(1f, 1f, 0.7f, 1f);

	private static readonly Color OutlineColorUnselected = new Color(1f, 1f, 1f, 0.1f);

	public override Vector2 InitialSize => new Vector2(334f + Margin * 2f + 16f, 400f);

	public Dialog_SelectXenotypeIcon(XenotypeIconDef selected, Action<XenotypeIconDef> iconSelector)
	{
		this.selected = selected;
		this.iconSelector = iconSelector;
		closeOnClickedOutside = true;
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckBiotech("xenotype icon"))
		{
			Close(doCloseSound: false);
		}
		else
		{
			base.PostOpen();
		}
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		Text.Font = GameFont.Medium;
		Widgets.Label(val, "SelectIcon".Translate());
		Text.Font = GameFont.Small;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + 39f;
		((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - (Window.CloseButSize.y + 4f);
		Rect outRect = val;
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - 4f;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref outRect)).x, ((Rect)(ref outRect)).y, ((Rect)(ref outRect)).width - 16f, scrollHeight);
		Widgets.DrawLightHighlight(val2);
		Widgets.BeginScrollView(outRect, ref scrollPosition, val2);
		float num = ((Rect)(ref outRect)).x + 6f;
		float num2 = ((Rect)(ref outRect)).y + 6f;
		Rect val3 = default(Rect);
		foreach (XenotypeIconDef allDef in DefDatabase<XenotypeIconDef>.AllDefs)
		{
			if (num + 35f + 6f > ((Rect)(ref val2)).width)
			{
				num = ((Rect)(ref outRect)).x + 6f;
				num2 += 41f;
				scrollHeight = Mathf.Max(scrollHeight, num2);
			}
			((Rect)(ref val3))._002Ector(num, num2, 35f, 35f);
			Widgets.DrawHighlight(val3);
			if (selected == allDef)
			{
				GUI.color = OutlineColorSelected;
				Widgets.DrawHighlight(val3);
				Widgets.DrawBox(val3.ExpandedBy(2f), 2);
			}
			else
			{
				GUI.color = OutlineColorUnselected;
				Widgets.DrawBox(val3);
			}
			GUI.color = Color.white;
			if (Widgets.ButtonImage(val3, allDef.Icon, XenotypeDef.IconColor))
			{
				selected = allDef;
			}
			num += 41f;
		}
		Widgets.EndScrollView();
		if (Widgets.ButtonText(new Rect((((Rect)(ref rect)).width - Window.CloseButSize.x) / 2f, ((Rect)(ref val)).yMax, Window.CloseButSize.x, Window.CloseButSize.y), "Accept".Translate()))
		{
			Close();
		}
	}

	public override void PreClose()
	{
		iconSelector(selected);
	}
}
