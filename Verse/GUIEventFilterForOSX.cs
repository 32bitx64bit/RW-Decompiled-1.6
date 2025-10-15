using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class GUIEventFilterForOSX
{
	private static List<Event> eventsThisFrame = new List<Event>();

	private static int lastRecordedFrame = -1;

	public static void CheckRejectGUIEvent()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		if ((int)UnityData.platform != 1 || ((int)Event.current.type != 0 && (int)Event.current.type != 1))
		{
			return;
		}
		if (Time.frameCount != lastRecordedFrame)
		{
			eventsThisFrame.Clear();
			lastRecordedFrame = Time.frameCount;
		}
		for (int i = 0; i < eventsThisFrame.Count; i++)
		{
			if (EventsAreEquivalent(eventsThisFrame[i], Event.current))
			{
				RejectEvent();
			}
		}
		eventsThisFrame.Add(Event.current);
	}

	private static bool EventsAreEquivalent(Event A, Event B)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (A.button == B.button && A.keyCode == B.keyCode)
		{
			return A.type == B.type;
		}
		return false;
	}

	private static void RejectEvent()
	{
		if (DebugViewSettings.logInput)
		{
			Log.Message("Frame " + Time.frameCount + ": REJECTED " + Event.current.ToStringFull());
		}
		Event.current.Use();
	}
}
