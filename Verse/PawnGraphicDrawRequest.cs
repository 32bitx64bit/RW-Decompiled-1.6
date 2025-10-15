using UnityEngine;

namespace Verse;

public struct PawnGraphicDrawRequest
{
	public readonly PawnRenderNode node;

	public readonly Mesh mesh;

	public Material material;

	public Matrix4x4 preDrawnComputedMatrix;

	public PawnGraphicDrawRequest(PawnRenderNode node, Mesh mesh = null, Material material = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		this.node = node;
		this.mesh = mesh;
		this.material = material;
		preDrawnComputedMatrix = default(Matrix4x4);
	}
}
