using System;
using UnityEngine;

namespace Verse;

public class ListableOption_WebLink : ListableOption
{
	public Texture2D image;

	public string url;

	private static readonly Vector2 Imagesize = new Vector2(24f, 18f);

	public ListableOption_WebLink(string label, Texture2D image)
		: base(label, null)
	{
		minHeight = 24f;
		this.image = image;
	}

	public ListableOption_WebLink(string label, string url, Texture2D image)
		: this(label, image)
	{
		this.url = url;
	}

	public ListableOption_WebLink(string label, Action action, Texture2D image)
		: this(label, image)
	{
		base.action = action;
	}

	public override float DrawOption(Vector2 pos, float width)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		float num = width - Imagesize.x - 3f;
		float num2 = Text.CalcHeight(label, num);
		float num3 = Mathf.Max(minHeight, num2);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(pos.x, pos.y, width, num3);
		GUI.color = Color.white;
		if ((Object)(object)image != (Object)null)
		{
			Rect val2 = new Rect(pos.x, pos.y + num3 / 2f - Imagesize.y / 2f, Imagesize.x, Imagesize.y);
			if (Mouse.IsOver(val))
			{
				GUI.color = Widgets.MouseoverOptionColor;
			}
			GUI.DrawTexture(val2, (Texture)(object)image);
		}
		Widgets.Label(new Rect(((Rect)(ref val)).xMax - num, pos.y, num, num2), label);
		GUI.color = Color.white;
		if (Widgets.ButtonInvisible(val))
		{
			if (action != null)
			{
				action();
			}
			else
			{
				Application.OpenURL(url);
			}
		}
		return num3;
	}
}
