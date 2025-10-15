using Steamworks;

namespace Verse.Steam;

internal static class WorkshopUtility
{
	public static string GetLabel(this WorkshopInteractStage stage)
	{
		if (stage == WorkshopInteractStage.None)
		{
			return "None".Translate();
		}
		return ("WorkshopInteractStage_" + stage).Translate();
	}

	public static string GetLabel(this EItemUpdateStatus status)
	{
		return ("EItemUpdateStatus_" + ((object)(EItemUpdateStatus)(ref status)).ToString()).Translate();
	}

	public static string GetLabel(this EResult result)
	{
		return ((object)(EResult)(ref result)).ToString().Substring(9);
	}
}
