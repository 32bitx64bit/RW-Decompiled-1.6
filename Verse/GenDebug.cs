using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse;

public static class GenDebug
{
	public static void DebugPlaceSphere(Vector3 Loc, float Scale)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = GameObject.CreatePrimitive((PrimitiveType)0);
		obj.transform.position = Loc;
		obj.transform.localScale = new Vector3(Scale, Scale, Scale);
	}

	public static void LogList<T>(IEnumerable<T> list)
	{
		foreach (T item in list)
		{
			Log.Message("    " + item.ToString());
		}
	}

	public static void ClearArea(CellRect r, Map map)
	{
		r.ClipInsideMap(map);
		foreach (IntVec3 item in r)
		{
			map.roofGrid.SetRoof(item, null);
		}
		foreach (IntVec3 item2 in r)
		{
			foreach (Thing item3 in item2.GetThingList(map).ToList())
			{
				if (item3.def.destroyable)
				{
					item3.Destroy();
				}
			}
		}
	}

	public static void SpawnArea(CellRect r, Map map, ThingDef def)
	{
		foreach (IntVec3 item in r)
		{
			GenSpawn.Spawn(def, item, map);
		}
	}
}
