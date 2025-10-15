using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class FastVector2Comparer : IEqualityComparer<Vector2>
{
	public static readonly FastVector2Comparer Instance = new FastVector2Comparer();

	public bool Equals(Vector2 x, Vector2 y)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return x == y;
	}

	public int GetHashCode(Vector2 obj)
	{
		return ((object)(Vector2)(ref obj)).GetHashCode();
	}
}
