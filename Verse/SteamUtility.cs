using Steamworks;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public static class SteamUtility
{
	private static string cachedPersonaName;

	public static string SteamPersonaName
	{
		get
		{
			if (SteamManager.Initialized && cachedPersonaName == null)
			{
				cachedPersonaName = SteamFriends.GetPersonaName();
			}
			if (cachedPersonaName == null)
			{
				return "???";
			}
			return cachedPersonaName;
		}
	}

	public static void OpenUrl(string url)
	{
		if (SteamManager.Initialized && SteamUtils.IsOverlayEnabled())
		{
			SteamFriends.ActivateGameOverlayToWebPage(url, (EActivateGameOverlayToWebPageMode)0);
		}
		else
		{
			Application.OpenURL(url);
		}
	}

	public static void OpenWorkshopPage(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		OpenUrl(SteamWorkshopPageUrl(pfid));
	}

	public static void OpenSteamWorkshopPage()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		AppId_t appID = SteamUtils.GetAppID();
		OpenUrl("http://steamcommunity.com/workshop/browse/?appid=" + ((object)(AppId_t)(ref appID)).ToString());
	}

	public static string SteamWorkshopPageUrl(PublishedFileId_t pfid)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		PublishedFileId_t val = pfid;
		return "steam://url/CommunityFilePage/" + ((object)(PublishedFileId_t)(ref val)).ToString();
	}
}
