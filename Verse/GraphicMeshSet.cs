using UnityEngine;

namespace Verse;

public class GraphicMeshSet
{
	private Mesh[] meshes = (Mesh[])(object)new Mesh[4];

	public GraphicMeshSet(Mesh normalMesh, Mesh leftMesh)
	{
		meshes[0] = (meshes[1] = (meshes[2] = normalMesh));
		meshes[3] = leftMesh;
	}

	public GraphicMeshSet(Mesh mesh)
	{
		meshes[0] = (meshes[1] = (meshes[2] = (meshes[3] = mesh)));
	}

	public GraphicMeshSet(float size)
	{
		meshes[0] = (meshes[1] = (meshes[2] = MeshMakerPlanes.NewPlaneMesh(size, flipped: false, backLift: true)));
		meshes[3] = MeshMakerPlanes.NewPlaneMesh(size, flipped: true, backLift: true);
	}

	public GraphicMeshSet(float width, float height)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector2 size = default(Vector2);
		((Vector2)(ref size))._002Ector(width, height);
		meshes[0] = (meshes[1] = (meshes[2] = MeshMakerPlanes.NewPlaneMesh(size, flipped: false, backLift: true, twist: false)));
		meshes[3] = MeshMakerPlanes.NewPlaneMesh(size, flipped: true, backLift: true, twist: false);
	}

	public GraphicMeshSet(Mesh northMesh, Mesh eastMesh, Mesh southMesh, Mesh westMesh)
	{
		meshes[0] = northMesh;
		meshes[1] = eastMesh;
		meshes[2] = southMesh;
		meshes[3] = westMesh;
	}

	public Mesh MeshAt(Rot4 rot)
	{
		return meshes[rot.AsInt];
	}
}
