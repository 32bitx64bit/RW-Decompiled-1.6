using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class DebugWorldLine
{
	public Vector3 a;

	public Vector3 b;

	public int ticksLeft;

	private bool onPlanetSurface;

	public int TicksLeft
	{
		get
		{
			return ticksLeft;
		}
		set
		{
			ticksLeft = value;
		}
	}

	public DebugWorldLine(Vector3 a, Vector3 b, bool onPlanetSurface)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		this.a = a;
		this.b = b;
		this.onPlanetSurface = onPlanetSurface;
		ticksLeft = 100;
	}

	public DebugWorldLine(Vector3 a, Vector3 b, bool onPlanetSurface, int ticksLeft)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		this.a = a;
		this.b = b;
		this.onPlanetSurface = onPlanetSurface;
		this.ticksLeft = ticksLeft;
	}

	public void Draw()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(a, b);
		if (num < 0.001f)
		{
			return;
		}
		if (onPlanetSurface)
		{
			float averageTileSize = Find.WorldGrid.AverageTileSize;
			int num2 = Mathf.Max(Mathf.RoundToInt(num / averageTileSize), 0);
			float num3 = 0.08f;
			for (int i = 0; i < num2; i++)
			{
				Vector3 val = Vector3.Lerp(a, b, (float)i / (float)num2);
				Vector3 val2 = Vector3.Lerp(a, b, (float)(i + 1) / (float)num2);
				val = ((Vector3)(ref val)).normalized * (100f + num3);
				val2 = ((Vector3)(ref val2)).normalized * (100f + num3);
				GenDraw.DrawWorldLineBetween(val, val2);
			}
		}
		else
		{
			GenDraw.DrawWorldLineBetween(a, b);
		}
	}
}
