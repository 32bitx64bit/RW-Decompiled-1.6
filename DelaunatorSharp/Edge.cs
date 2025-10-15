using UnityEngine;

namespace DelaunatorSharp;

public struct Edge : IEdge
{
	public Vector2 P { get; set; }

	public Vector2 Q { get; set; }

	public int Index { get; set; }

	public Edge(int e, Vector2 p, Vector2 q)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Index = e;
		P = p;
		Q = q;
	}
}
