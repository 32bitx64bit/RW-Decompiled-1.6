using System;
using UnityEngine;

namespace Verse;

public readonly struct TextBlock : IDisposable
{
	private readonly GameFont oldFont;

	private readonly TextAnchor oldAnchor;

	private readonly bool oldWordWrap;

	private readonly Color oldColor;

	public TextBlock(GameFont newFont)
		: this(newFont, null, null, null)
	{
	}

	public TextBlock(TextAnchor newAnchor)
		: this(null, newAnchor, null, null)
	{
	}//IL_000a: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(bool newWordWrap)
		: this(null, null, newWordWrap, null)
	{
	}

	public TextBlock(Color newColor, TextAnchor newAnchor, bool newWordWrap)
		: this(null, newAnchor, newWordWrap, newColor)
	{
	}//IL_000a: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(Color newColor)
		: this(null, null, null, newColor)
	{
	}//IL_001c: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(TextAnchor newAnchor, Color newColor)
		: this(null, newAnchor, null, newColor)
	{
	}//IL_000a: Unknown result type (might be due to invalid IL or missing references)
	//IL_0019: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(GameFont newFont, TextAnchor newAnchor)
		: this(newFont, newAnchor, null, null)
	{
	}//IL_0007: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(GameFont newFont, Color newColor)
		: this(newFont, null, null, newColor)
	{
	}//IL_0019: Unknown result type (might be due to invalid IL or missing references)


	public TextBlock(GameFont? newFont, TextAnchor? newAnchor, Color? newColor)
		: this(newFont, newAnchor, null, newColor)
	{
	}

	public TextBlock(GameFont? newFont, TextAnchor? newAnchor, bool? newWordWrap)
		: this(newFont, newAnchor, newWordWrap, null)
	{
	}

	public TextBlock(GameFont? newFont, TextAnchor? newAnchor, bool? newWordWrap, Color? newColor)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		oldFont = Text.Font;
		oldAnchor = Text.Anchor;
		oldWordWrap = Text.WordWrap;
		oldColor = GUI.color;
		if (newFont.HasValue)
		{
			Text.Font = newFont.Value;
		}
		if (newAnchor.HasValue)
		{
			Text.Anchor = newAnchor.Value;
		}
		if (newWordWrap.HasValue)
		{
			Text.WordWrap = newWordWrap.Value;
		}
		if (newColor.HasValue)
		{
			GUI.color = newColor.Value;
		}
	}

	public static TextBlock Default()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return new TextBlock(GameFont.Small, (TextAnchor)0, true, Color.white);
	}

	public void Dispose()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = oldFont;
		Text.Anchor = oldAnchor;
		Text.WordWrap = oldWordWrap;
		GUI.color = oldColor;
	}
}
