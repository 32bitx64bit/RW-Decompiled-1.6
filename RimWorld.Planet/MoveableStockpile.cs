using Verse;

namespace RimWorld.Planet;

public class MoveableStockpile : MoveableArea
{
	private bool hidden;

	private StorageSettings settings;

	public MoveableStockpile()
	{
	}

	public MoveableStockpile(Gravship gravship, Zone_Stockpile stockpile)
		: base(gravship, stockpile.label, stockpile.RenamableLabel, stockpile.color, stockpile.ID)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		hidden = stockpile.Hidden;
		settings = new StorageSettings();
		settings.CopyFrom(stockpile.settings);
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref hidden, "hidden", defaultValue: false);
		Scribe_Deep.Look(ref settings, "settings");
	}

	public void TryCreateStockpile(ZoneManager zoneManager, IntVec3 newOrigin)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Zone_Stockpile zone_Stockpile = new Zone_Stockpile(StorageSettingsPreset.DefaultStockpile, zoneManager)
		{
			label = label,
			Hidden = hidden,
			color = color,
			ID = id
		};
		zone_Stockpile.settings = new StorageSettings(zone_Stockpile);
		zone_Stockpile.settings.CopyFrom(settings);
		zoneManager.RegisterZone(zone_Stockpile);
		foreach (IntVec3 relativeCell in base.RelativeCells)
		{
			zone_Stockpile.AddCell(newOrigin + relativeCell);
		}
	}
}
