using UnityEngine;

namespace Verse;

public static class OriginalEventUtility
{
	private static EventType? originalType;

	public static EventType EventType => (EventType)(((_003F?)originalType) ?? Event.current.rawType);

	public static void RecordOriginalEvent(Event e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		originalType = e.type;
	}

	public static void Reset()
	{
		originalType = null;
	}
}
