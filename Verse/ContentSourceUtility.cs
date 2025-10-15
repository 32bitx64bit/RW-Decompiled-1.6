using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class ContentSourceUtility
{
	public const float IconSize = 24f;

	private static readonly Texture2D ContentSourceIcon_OfficialModsFolder = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/OfficialModsFolder");

	private static readonly Texture2D ContentSourceIcon_ModsFolder = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/ModsFolder");

	private static readonly Texture2D ContentSourceIcon_SteamWorkshop = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/SteamWorkshop");

	public static Texture2D GetIcon(this ContentSource s)
	{
		return (Texture2D)(s switch
		{
			ContentSource.Undefined => BaseContent.BadTex, 
			ContentSource.OfficialModsFolder => ContentSourceIcon_OfficialModsFolder, 
			ContentSource.ModsFolder => ContentSourceIcon_ModsFolder, 
			ContentSource.SteamWorkshop => ContentSourceIcon_SteamWorkshop, 
			_ => throw new NotImplementedException(), 
		});
	}

	public static void DrawContentSource(Rect r, ContentSource source, Action clickAction = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y + ((Rect)(ref r)).height / 2f - 12f, 24f, 24f);
		GUI.DrawTexture(val, (Texture)(object)source.GetIcon());
		if (Mouse.IsOver(val))
		{
			TooltipHandler.TipRegion(val, () => "Source".Translate() + ": " + source.HumanLabel(), (int)(((Rect)(ref r)).x + ((Rect)(ref r)).y * 56161f));
			Widgets.DrawHighlight(val);
		}
		if (clickAction != null && Widgets.ButtonInvisible(val))
		{
			clickAction();
		}
	}

	public static string HumanLabel(this ContentSource s)
	{
		return ("ContentSource_" + s).Translate();
	}
}
