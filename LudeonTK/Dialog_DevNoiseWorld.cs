using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace LudeonTK;

public class Dialog_DevNoiseWorld : Dialog_DevNoiseBase
{
	protected override string Title => "World Noise Visualizer";

	public override void PreOpen()
	{
		base.PreOpen();
		OnNoiseChanged();
	}

	public override void PostClose()
	{
		base.PostClose();
		OnNoiseChanged();
	}

	protected override void OnNoiseChanged()
	{
		Find.World.renderer.SetDirty<WorldDrawLayer_DebugNoise>(PlanetLayer.Selected);
	}

	protected override int GetSeed()
	{
		return Find.World.info.Seed;
	}

	protected override void DoWindowListing()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Find.World.renderer.TryGetLayer<WorldDrawLayer_DebugNoise>(PlanetLayer.Selected, out var _))
		{
			base.DoWindowListing();
			if (Find.WorldSelector.SelectedTile == PlanetTile.Invalid)
			{
				PrintLabel("No tile selected");
				return;
			}
			PlanetTile selectedTile = Find.WorldSelector.SelectedTile;
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(selectedTile);
			float num = (float)noise.GetValue(tileCenter.x, tileCenter.y, tileCenter.z);
			PrintLabel($"Selected tile ({selectedTile}):");
			PrintLabel("Coordinates: " + ((Vector3)(ref tileCenter)).ToString("0.#"));
			PrintLabel($"Noise value: {num:0.###}");
		}
	}
}
