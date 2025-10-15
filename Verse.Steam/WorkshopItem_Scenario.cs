using System.IO;
using System.Linq;
using RimWorld;
using Steamworks;

namespace Verse.Steam;

public class WorkshopItem_Scenario : WorkshopItem
{
	private Scenario cachedScenario;

	public override PublishedFileId_t PublishedFileId
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return base.PublishedFileId;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			base.PublishedFileId = value;
			if (cachedScenario != null)
			{
				cachedScenario.SetPublishedFileId(value);
			}
		}
	}

	public Scenario GetScenario()
	{
		if (cachedScenario == null)
		{
			LoadScenario();
		}
		return cachedScenario;
	}

	private void LoadScenario()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (GameDataSaveLoader.TryLoadScenario(base.Directory.GetFiles("*.rsc").First((FileInfo fi) => fi.Extension == ".rsc").FullName, ScenarioCategory.SteamWorkshop, out cachedScenario))
		{
			cachedScenario.SetPublishedFileId(PublishedFileId);
		}
	}
}
