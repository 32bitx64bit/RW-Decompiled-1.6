using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public abstract class MapDrawLayer
{
	private Map map;

	public ulong relevantChangeTypes;

	public List<LayerSubMesh> subMeshes = new List<LayerSubMesh>();

	protected Map Map => map;

	public virtual bool Visible => true;

	public bool Dirty { get; set; }

	public MapDrawLayer(Map map)
	{
		this.map = map;
	}

	public static LayerSubMesh CreateFreeSubMesh(Material material, Map map = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		if ((Object)(object)material == (Object)null)
		{
			return null;
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "SectionLayerSubMesh_Free" + ((map == null) ? "" : ("_" + map.Tile));
		}
		return new LayerSubMesh(val, material);
	}

	public LayerSubMesh GetSubMesh(Material material)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if ((Object)(object)material == (Object)null)
		{
			return null;
		}
		for (int i = 0; i < subMeshes.Count; i++)
		{
			if ((Object)(object)subMeshes[i].material == (Object)(object)material)
			{
				return subMeshes[i];
			}
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "SectionLayerSubMesh_" + GetType().Name + "_" + Map.Tile;
		}
		LayerSubMesh layerSubMesh = new LayerSubMesh(val, material);
		subMeshes.Add(layerSubMesh);
		return layerSubMesh;
	}

	public virtual CellRect GetBoundaryRect()
	{
		return map.BoundsRect();
	}

	public virtual void RefreshSubMeshBounds()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = GetBoundaryRect().ExpandedBy(2).ToBounds();
		for (int i = 0; i < subMeshes.Count; i++)
		{
			subMeshes[i].mesh.bounds = bounds;
		}
	}

	protected void FinalizeMesh(MeshParts tags)
	{
		for (int i = 0; i < subMeshes.Count; i++)
		{
			if (subMeshes[i].verts.Count > 0)
			{
				subMeshes[i].FinalizeMesh(tags);
			}
		}
	}

	public virtual void DrawLayer()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible)
		{
			return;
		}
		int count = subMeshes.Count;
		for (int i = 0; i < count; i++)
		{
			LayerSubMesh layerSubMesh = subMeshes[i];
			if (layerSubMesh.finalized && !layerSubMesh.disabled)
			{
				Graphics.DrawMesh(layerSubMesh.mesh, Matrix4x4.identity, layerSubMesh.material, layerSubMesh.renderLayer);
			}
		}
	}

	public abstract void Regenerate();

	protected void ClearSubMeshes(MeshParts parts)
	{
		foreach (LayerSubMesh subMesh in subMeshes)
		{
			subMesh.Clear(parts);
		}
	}

	public void Dispose()
	{
		foreach (LayerSubMesh subMesh in subMeshes)
		{
			Object.Destroy((Object)(object)subMesh.mesh);
		}
		subMeshes.Clear();
	}
}
