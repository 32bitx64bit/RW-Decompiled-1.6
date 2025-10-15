using UnityEngine;
using Verse;

namespace RimWorld;

public class RecipeTooltipLayout
{
	private Vector2 curPos;

	private Vector2 maxSize;

	private float indent;

	public Vector2 Size => maxSize;

	public bool Empty
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (!(Size.x <= 0f))
			{
				return Size.y <= 0f;
			}
			return true;
		}
	}

	public void Label(string text, bool draw, GameFont font = GameFont.Small)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		GameFont font2 = Text.Font;
		Text.Font = font;
		Vector2 val = Text.CalcSize(text);
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(curPos.x, curPos.y, val.x, val.y);
		if (draw)
		{
			Widgets.Label(rect, text);
		}
		curPos.x = ((Rect)(ref rect)).xMax;
		ExpandToFit(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
		Text.Font = font2;
	}

	public void Icon(Texture2D icon, Color color, float iconSize, bool draw)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(curPos.x, curPos.y, iconSize, iconSize);
		if (draw)
		{
			Color color2 = GUI.color;
			GUI.color = color;
			GUI.DrawTexture(val, (Texture)(object)icon);
			GUI.color = color2;
		}
		curPos.x = ((Rect)(ref val)).xMax;
		ExpandToFit(((Rect)(ref val)).xMax, ((Rect)(ref val)).yMax);
	}

	public void Gap(float x, float y)
	{
		curPos.x += x;
		curPos.y += y;
		ExpandToFit(curPos.x, curPos.y);
	}

	public void Reset(float newIndent = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		curPos = Vector2.zero;
		maxSize = Vector2.zero;
		indent = newIndent;
		curPos.x = Mathf.Max(curPos.x, indent);
		curPos.y = Mathf.Max(curPos.y, indent);
	}

	public void Expand(float width, float height)
	{
		maxSize.x += width;
		maxSize.y += height;
	}

	public void Newline(GameFont font = GameFont.Small)
	{
		curPos.x = indent;
		curPos.y += Text.LineHeightOf(font);
		ExpandToFit(curPos.x, curPos.y);
	}

	private void ExpandToFit(float x, float y)
	{
		maxSize.x = Mathf.Max(maxSize.x, x);
		maxSize.y = Mathf.Max(maxSize.y, y);
	}
}
