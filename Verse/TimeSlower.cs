using UnityEngine;

namespace Verse;

public class TimeSlower
{
	private int forceNormalSpeedUntil;

	private const int ForceTicksStandard = 800;

	private const int ForceTicksShort = 240;

	public bool ForcedNormalSpeed
	{
		get
		{
			if (DebugViewSettings.neverForceNormalSpeed)
			{
				return false;
			}
			return Find.TickManager.TicksGame < forceNormalSpeedUntil;
		}
	}

	public void SignalForceNormalSpeed()
	{
		forceNormalSpeedUntil = Mathf.Max(new int[1] { Find.TickManager.TicksGame + 800 });
	}

	public void SignalForceNormalSpeedShort()
	{
		forceNormalSpeedUntil = Mathf.Max(forceNormalSpeedUntil, Find.TickManager.TicksGame + 240);
	}
}
