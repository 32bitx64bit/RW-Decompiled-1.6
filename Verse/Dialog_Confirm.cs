using System;
using UnityEngine;

namespace Verse;

public class Dialog_Confirm : Window
{
	private string title;

	private string confirm;

	private Action onConfirm;

	private static readonly Vector2 ButtonSize = new Vector2(120f, 32f);

	public override Vector2 InitialSize => new Vector2(280f, 150f);

	public Dialog_Confirm(string title, Action onConfirm)
		: this(title, "Confirm".Translate(), onConfirm)
	{
	}

	public Dialog_Confirm(string title, string confirm, Action onConfirm)
	{
		this.title = title;
		this.confirm = confirm;
		this.onConfirm = onConfirm;
		forcePause = true;
		closeOnAccept = false;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		bool flag = false;
		if ((int)Event.current.type == 4 && ((int)Event.current.keyCode == 13 || (int)Event.current.keyCode == 271))
		{
			flag = true;
			Event.current.Use();
		}
		Rect rect = inRect;
		((Rect)(ref rect)).width = ((Rect)(ref inRect)).width / 2f - 5f;
		((Rect)(ref rect)).yMin = ((Rect)(ref inRect)).yMax - ButtonSize.y - 10f;
		Rect rect2 = inRect;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect)).xMax + 10f;
		((Rect)(ref rect2)).yMin = ((Rect)(ref inRect)).yMax - ButtonSize.y - 10f;
		Rect rect3 = inRect;
		((Rect)(ref rect3)).y = ((Rect)(ref rect3)).y + 4f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect2)).y - 10f;
		using (new TextBlock((TextAnchor)1))
		{
			Widgets.Label(rect3, title);
		}
		if (Widgets.ButtonText(rect, "Cancel".Translate()))
		{
			Find.WindowStack.TryRemove(this);
		}
		if (Widgets.ButtonText(rect2, confirm) || flag)
		{
			onConfirm?.Invoke();
			Find.WindowStack.TryRemove(this);
		}
	}
}
