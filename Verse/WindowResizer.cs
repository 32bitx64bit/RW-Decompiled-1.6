using UnityEngine;

namespace Verse;

public class WindowResizer
{
	public Vector2 minWindowSize = new Vector2(150f, 150f);

	private bool isResizing;

	private Rect resizeStart;

	private const float ResizeButtonSize = 24f;

	public Rect DoResizeControl(Rect winRect)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Invalid comparison between Unknown and I4
		Vector2 mousePosition = Event.current.mousePosition;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref winRect)).width - 24f, ((Rect)(ref winRect)).height - 24f, 24f, 24f);
		if ((int)Event.current.type == 0 && Mouse.IsOver(val))
		{
			isResizing = true;
			resizeStart = new Rect(mousePosition.x, mousePosition.y, ((Rect)(ref winRect)).width, ((Rect)(ref winRect)).height);
		}
		if (isResizing)
		{
			((Rect)(ref winRect)).width = ((Rect)(ref resizeStart)).width + (mousePosition.x - ((Rect)(ref resizeStart)).x);
			((Rect)(ref winRect)).height = ((Rect)(ref resizeStart)).height + (mousePosition.y - ((Rect)(ref resizeStart)).y);
			if (((Rect)(ref winRect)).width < minWindowSize.x)
			{
				((Rect)(ref winRect)).width = minWindowSize.x;
			}
			if (((Rect)(ref winRect)).height < minWindowSize.y)
			{
				((Rect)(ref winRect)).height = minWindowSize.y;
			}
			((Rect)(ref winRect)).xMax = Mathf.Min((float)UI.screenWidth, ((Rect)(ref winRect)).xMax);
			((Rect)(ref winRect)).yMax = Mathf.Min((float)UI.screenHeight, ((Rect)(ref winRect)).yMax);
			if ((int)Event.current.type == 1)
			{
				isResizing = false;
			}
		}
		Widgets.ButtonImage(val, TexUI.WinExpandWidget);
		return new Rect(((Rect)(ref winRect)).x, ((Rect)(ref winRect)).y, (float)(int)((Rect)(ref winRect)).width, (float)(int)((Rect)(ref winRect)).height);
	}
}
