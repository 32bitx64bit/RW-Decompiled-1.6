using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse;

public class Listing_Tree : Listing_Lines
{
	public float nestIndentWidth = 11f;

	protected const float OpenCloseWidgetSize = 18f;

	protected virtual float LabelWidth => base.ColumnWidth - 26f;

	protected float EditAreaWidth => base.ColumnWidth - LabelWidth;

	public override void Begin(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.Begin(rect);
		Text.Anchor = (TextAnchor)3;
		Text.WordWrap = false;
	}

	public override void End()
	{
		base.End();
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
	}

	protected float XAtIndentLevel(int indentLevel)
	{
		return (float)indentLevel * nestIndentWidth;
	}

	protected void LabelLeft(string label, string tipText, int indentLevel, float widthOffset = 0f, Color? textColor = null, float leftOffset = 0f)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, base.ColumnWidth, lineHeight);
		((Rect)(ref val)).xMin = XAtIndentLevel(indentLevel) + 18f + leftOffset;
		Widgets.DrawHighlightIfMouseover(val);
		if (!tipText.NullOrEmpty())
		{
			if (Mouse.IsOver(val))
			{
				GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
			}
			TooltipHandler.TipRegion(val, tipText);
		}
		Text.Anchor = (TextAnchor)3;
		GUI.color = (Color)(((_003F?)textColor) ?? Color.white);
		((Rect)(ref val)).width = LabelWidth - ((Rect)(ref val)).xMin + widthOffset;
		((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax + 5f;
		((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin - 5f;
		Widgets.Label(val, label.Truncate(((Rect)(ref val)).width));
		Text.Anchor = (TextAnchor)0;
		GUI.color = Color.white;
	}

	protected bool OpenCloseWidget(TreeNode node, int indentLevel, int openMask)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!node.Openable)
		{
			return false;
		}
		float num = XAtIndentLevel(indentLevel);
		float num2 = curY + lineHeight / 2f - 9f;
		Rect butRect = new Rect(num, num2, 18f, 18f);
		bool flag = IsOpen(node, openMask);
		Texture2D tex = (flag ? TexButton.Collapse : TexButton.Reveal);
		if (Widgets.ButtonImage(butRect, tex))
		{
			if (flag)
			{
				SoundDefOf.TabClose.PlayOneShotOnCamera();
			}
			else
			{
				SoundDefOf.TabOpen.PlayOneShotOnCamera();
			}
			node.SetOpen(openMask, !flag);
			return true;
		}
		return false;
	}

	public virtual bool IsOpen(TreeNode node, int openMask)
	{
		if (node.IsOpen(openMask))
		{
			return true;
		}
		return false;
	}

	public void InfoText(string text, int indentLevel)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Text.WordWrap = true;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, curY, base.ColumnWidth, 50f);
		((Rect)(ref rect)).xMin = LabelWidth;
		((Rect)(ref rect)).height = Text.CalcHeight(text, ((Rect)(ref rect)).width);
		Widgets.Label(rect, text);
		curY += ((Rect)(ref rect)).height;
		Text.WordWrap = false;
	}

	public bool ButtonText(string label)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Text.WordWrap = true;
		float num = Text.CalcHeight(label, base.ColumnWidth);
		bool result = Widgets.ButtonText(new Rect(0f, curY, base.ColumnWidth, num), label);
		curY += num + 0f;
		Text.WordWrap = false;
		return result;
	}

	public WidgetRow StartWidgetsRow(int indentLevel)
	{
		WidgetRow result = new WidgetRow(LabelWidth, curY);
		curY += 24f;
		return result;
	}
}
