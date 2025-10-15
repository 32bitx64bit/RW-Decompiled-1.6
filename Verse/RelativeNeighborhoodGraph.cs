using System.Collections.Generic;
using DelaunatorSharp;
using UnityEngine;

namespace Verse;

public class RelativeNeighborhoodGraph
{
	public readonly Dictionary<Vector2, List<Vector2>> connections = new Dictionary<Vector2, List<Vector2>>();

	public RelativeNeighborhoodGraph(Delaunator delaunator)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] points = delaunator.Points;
		foreach (Vector2 key in points)
		{
			connections[key] = new List<Vector2>();
		}
		points = delaunator.Points;
		foreach (Vector2 val in points)
		{
			Vector2[] points2 = delaunator.Points;
			foreach (Vector2 val2 in points2)
			{
				if (connections[val].Contains(val2) || !TryGetEdge(delaunator, val, val2, out var edge))
				{
					continue;
				}
				float weight = GetWeight(edge);
				bool flag = false;
				Vector2[] points3 = delaunator.Points;
				foreach (Vector2 val3 in points3)
				{
					if (TryGetEdge(delaunator, val, val3, out var edge2) && TryGetEdge(delaunator, val3, val2, out var edge3))
					{
						float weight2 = GetWeight(edge2);
						float weight3 = GetWeight(edge3);
						if (Mathf.Max(weight2, weight3) < weight)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					connections[val].Add(val2);
					connections[val2].Add(val);
				}
			}
		}
	}

	private static float GetWeight(Edge edge)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = edge.Q - edge.P;
		return ((Vector2)(ref val)).magnitude;
	}

	private static bool TryGetEdge(Delaunator delaunator, Vector2 a, Vector2 b, out Edge edge)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		foreach (IEdge edge2 in delaunator.GetEdges())
		{
			if ((edge2.P == a || edge2.Q == a) && (edge2.P == b || edge2.Q == b))
			{
				edge = (Edge)(object)edge2;
				return true;
			}
		}
		edge = default(Edge);
		return false;
	}
}
