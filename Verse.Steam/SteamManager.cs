using System;
using System.Text;
using Steamworks;
using UnityEngine;

namespace Verse.Steam;

public static class SteamManager
{
	private static SteamAPIWarningMessageHook_t steamAPIWarningMessageHook;

	private static bool initializedInt;

	public static bool Initialized => initializedInt;

	public static bool Active => true;

	public static void InitIfNeeded()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		if (initializedInt)
		{
			return;
		}
		if (!Packsize.Test())
		{
			Log.Error("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}
		if (!DllCheck.Test())
		{
			Log.Error("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			Log.Error("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + ex);
			Application.Quit();
			return;
		}
		initializedInt = SteamAPI.Init();
		if (!initializedInt)
		{
			Log.Warning("[Steamworks.NET] SteamAPI.Init() failed. Possible causes: Steam client not running, launched from outside Steam without steam_appid.txt in place, running with different privileges than Steam client (e.g. \"as administrator\")");
			return;
		}
		if (steamAPIWarningMessageHook == null)
		{
			steamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(steamAPIWarningMessageHook);
		}
		Workshop.Init();
		SteamDeck.Init();
	}

	public static void Update()
	{
		if (initializedInt)
		{
			SteamAPI.RunCallbacks();
			SteamDeck.Update();
		}
	}

	public static void ShutdownSteam()
	{
		if (initializedInt)
		{
			SteamDeck.Shutdown();
			SteamAPI.Shutdown();
			initializedInt = false;
		}
	}

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Log.Error(pchDebugText.ToString());
	}
}
