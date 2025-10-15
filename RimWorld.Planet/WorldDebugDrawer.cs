using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldDebugDrawer
{
	private List<DebugTile> debugTiles = new List<DebugTile>();

	private List<DebugWorldLine> debugLines = new List<DebugWorldLine>();

	private const int DefaultLifespanTicks = 50;

	private static float MaxDistToCameraToDisplayLabel => WorldCameraDriver.MinAltitude - 100f + 14f;

	public void FlashTile(PlanetTile tile, float colorPct = 0f, string text = null, int duration = 50)
	{
		DebugTile debugTile = new DebugTile();
		debugTile.tile = tile;
		debugTile.displayString = text;
		debugTile.colorPct = colorPct;
		debugTile.ticksLeft = duration;
		debugTiles.Add(debugTile);
	}

	public void FlashTile(PlanetTile tile, Material mat, string text = null, int duration = 50)
	{
		DebugTile debugTile = new DebugTile();
		debugTile.tile = tile;
		debugTile.displayString = text;
		debugTile.customMat = mat;
		debugTile.ticksLeft = duration;
		debugTiles.Add(debugTile);
	}

	public void FlashLine(Vector3 a, Vector3 b, bool onPlanetSurface = false, int duration = 50)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DebugWorldLine debugWorldLine = new DebugWorldLine(a, b, onPlanetSurface);
		debugWorldLine.TicksLeft = duration;
		debugLines.Add(debugWorldLine);
	}

	public void FlashLine(PlanetTile tileA, PlanetTile tileB, int duration = 50)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		WorldGrid worldGrid = Find.WorldGrid;
		Vector3 tileCenter = worldGrid.GetTileCenter(tileA);
		Vector3 tileCenter2 = worldGrid.GetTileCenter(tileB);
		DebugWorldLine debugWorldLine = new DebugWorldLine(tileCenter, tileCenter2, onPlanetSurface: true);
		debugWorldLine.TicksLeft = duration;
		debugLines.Add(debugWorldLine);
	}

	public void WorldDebugDrawerUpdate()
	{
		for (int i = 0; i < debugTiles.Count; i++)
		{
			debugTiles[i].Draw();
		}
		for (int j = 0; j < debugLines.Count; j++)
		{
			debugLines[j].Draw();
		}
	}

	public void WorldDebugDrawerTick()
	{
		for (int num = debugTiles.Count - 1; num >= 0; num--)
		{
			DebugTile debugTile = debugTiles[num];
			debugTile.ticksLeft--;
			if (debugTile.ticksLeft <= 0)
			{
				debugTiles.RemoveAt(num);
			}
		}
		for (int num2 = debugLines.Count - 1; num2 >= 0; num2--)
		{
			DebugWorldLine debugWorldLine = debugLines[num2];
			debugWorldLine.ticksLeft--;
			if (debugWorldLine.ticksLeft <= 0)
			{
				debugLines.RemoveAt(num2);
			}
		}
	}

	public void WorldDebugDrawerOnGUI()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)4;
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		for (int i = 0; i < debugTiles.Count; i++)
		{
			if (!(debugTiles[i].DistanceToCamera > MaxDistToCameraToDisplayLabel))
			{
				debugTiles[i].OnGUI();
			}
		}
		GUI.color = Color.white;
		Text.Anchor = (TextAnchor)0;
	}
}
