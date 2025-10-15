using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace LudeonTK;

public abstract class EditWindow : Window_Dev
{
	private const float SuperimposeAvoidThreshold = 8f;

	private const float SuperimposeAvoidOffset = 16f;

	private const float SuperimposeAvoidOffsetMinEdge = 200f;

	public override Vector2 InitialSize => new Vector2(500f, 500f);

	protected override float Margin => 8f;

	public EditWindow()
	{
		resizeable = true;
		draggable = true;
		preventCameraMotion = false;
		doCloseX = true;
		((Rect)(ref windowRect)).x = 5f;
		((Rect)(ref windowRect)).y = 5f;
	}

	protected void DoRowButton(ref float x, float y, string text, string tooltip, Action action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Text.CalcSize(text);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, val.x + 10f, 24f);
		if (DevGUI.ButtonText(rect, text))
		{
			action();
		}
		if (!tooltip.NullOrEmpty())
		{
			TooltipHandler.TipRegion(rect, tooltip);
		}
		x += ((Rect)(ref rect)).width + 4f;
	}

	protected void DoImageToggle(ref float x, float y, Texture2D texture, string tooltip, ref bool toggle)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(x, y, 24f, 24f);
		DevGUI.CheckboxImage(rect, texture, ref toggle);
		TooltipHandler.TipRegion(rect, tooltip);
		x += 28f;
	}

	public override void PostOpen()
	{
		while (!(((Rect)(ref windowRect)).x > (float)UI.screenWidth - 200f) && !(((Rect)(ref windowRect)).y > (float)UI.screenHeight - 200f))
		{
			bool flag = false;
			foreach (EditWindow item in Find.WindowStack.Windows.Where((Window di) => di is EditWindow).Cast<EditWindow>())
			{
				if (item != this && Mathf.Abs(((Rect)(ref item.windowRect)).x - ((Rect)(ref windowRect)).x) < 8f && Mathf.Abs(((Rect)(ref item.windowRect)).y - ((Rect)(ref windowRect)).y) < 8f)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				ref Rect reference = ref windowRect;
				((Rect)(ref reference)).x = ((Rect)(ref reference)).x + 16f;
				ref Rect reference2 = ref windowRect;
				((Rect)(ref reference2)).y = ((Rect)(ref reference2)).y + 16f;
				continue;
			}
			break;
		}
	}
}
