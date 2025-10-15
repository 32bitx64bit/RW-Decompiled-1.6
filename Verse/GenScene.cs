using UnityEngine.SceneManagement;
using Verse.Profile;

namespace Verse;

public static class GenScene
{
	public const string EntrySceneName = "Entry";

	public const string PlaySceneName = "Play";

	public static bool InEntryScene
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			return ((Scene)(ref activeScene)).name == "Entry";
		}
	}

	public static bool InPlayScene
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			return ((Scene)(ref activeScene)).name == "Play";
		}
	}

	public static void GoToMainMenu()
	{
		LongEventHandler.ClearQueuedEvents();
		Current.Game?.Dispose();
		LongEventHandler.QueueLongEvent(delegate
		{
			MemoryUtility.ClearAllMapsAndWorld();
			Current.Game = null;
		}, "Entry", "LoadingLongEvent", doAsynchronously: true, null, showExtraUIInfo: false);
	}
}
