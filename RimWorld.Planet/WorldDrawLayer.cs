using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public abstract class WorldDrawLayer : WorldDrawLayerBase
{
	public PlanetLayer planetLayer;

	private List<MeshCollider> meshCollidersInOrder;

	private List<List<int>> triangleIndexToTileID;

	public override bool Visible
	{
		get
		{
			if (base.Visible)
			{
				if (PlanetLayer.Selected != planetLayer)
				{
					return VisibleWhenLayerNotSelected;
				}
				return true;
			}
			return false;
		}
	}

	public override Vector3 Position
	{
		get
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (CameraIsOrigin)
			{
				return Find.WorldCameraDriver.CameraPosition;
			}
			return planetLayer.Origin;
		}
	}

	public bool Raycastable
	{
		get
		{
			if (!meshCollidersInOrder.NullOrEmpty())
			{
				return PlanetLayer.Selected == planetLayer;
			}
			return false;
		}
	}

	public void Initialize(PlanetLayer layer)
	{
		planetLayer = layer;
	}

	public override IEnumerable Regenerate()
	{
		triangleIndexToTileID?.Clear();
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
	}

	protected void AppendRaycastableTriangle(int subMeshIndex, int tileIndex)
	{
		if (triangleIndexToTileID == null)
		{
			triangleIndexToTileID = new List<List<int>>();
		}
		while (triangleIndexToTileID.Count < subMeshes.Count)
		{
			triangleIndexToTileID.Add(new List<int>());
		}
		triangleIndexToTileID[subMeshIndex].Add(tileIndex);
	}

	public bool TryGetTileFromRayHit(RaycastHit hit, out PlanetTile id)
	{
		id = PlanetTile.Invalid;
		for (int i = 0; i < meshCollidersInOrder.Count; i++)
		{
			if ((Object)(object)meshCollidersInOrder[i] == (Object)(object)((RaycastHit)(ref hit)).collider)
			{
				id = new PlanetTile(triangleIndexToTileID[i][((RaycastHit)(ref hit)).triangleIndex], planetLayer);
				return true;
			}
		}
		return false;
	}

	protected IEnumerable RegenerateWorldMeshColliders()
	{
		if (meshCollidersInOrder == null)
		{
			meshCollidersInOrder = new List<MeshCollider>();
		}
		else
		{
			meshCollidersInOrder.Clear();
		}
		GameObject gameObject = WorldTerrainColliderManager.Get(planetLayer, WorldCameraManager.WorldLayer);
		gameObject.transform.position = Position;
		MeshCollider[] components = gameObject.GetComponents<MeshCollider>();
		for (int j = 0; j < components.Length; j++)
		{
			Object.Destroy((Object)(object)components[j]);
		}
		for (int i = 0; i < subMeshes.Count; i++)
		{
			MeshCollider val = gameObject.AddComponent<MeshCollider>();
			val.sharedMesh = subMeshes[i].mesh;
			meshCollidersInOrder.Add(val);
			yield return null;
		}
	}
}
