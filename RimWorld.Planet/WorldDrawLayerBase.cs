using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public abstract class WorldDrawLayerBase
{
	protected bool dirty = true;

	private MaterialPropertyBlock propertyBlock;

	protected readonly List<LayerSubMesh> subMeshes = new List<LayerSubMesh>();

	private const int MaxVerticesPerMesh = 40000;

	public virtual bool ShouldRegenerate
	{
		get
		{
			if (dirty)
			{
				return Visible;
			}
			return false;
		}
	}

	protected virtual int RenderLayer => WorldCameraManager.WorldLayer;

	protected virtual Quaternion Rotation => Quaternion.identity;

	protected virtual float Alpha => 1f;

	public bool Dirty => dirty;

	public virtual bool Visible
	{
		get
		{
			if (WorldRendererUtility.WorldBackgroundNow)
			{
				return VisibleInBackground;
			}
			return true;
		}
	}

	public virtual bool VisibleWhenLayerNotSelected => true;

	public virtual bool VisibleInBackground => true;

	public virtual Vector3 Position
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (CameraIsOrigin)
			{
				return Find.WorldCameraDriver.CameraPosition;
			}
			return Vector3.zero;
		}
	}

	public virtual bool CameraIsOrigin => false;

	public MaterialPropertyBlock MatPropBlock
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			//IL_0017: Expected O, but got Unknown
			MaterialPropertyBlock obj = propertyBlock;
			if (obj == null)
			{
				MaterialPropertyBlock val = new MaterialPropertyBlock();
				MaterialPropertyBlock val2 = val;
				propertyBlock = val;
				obj = val2;
			}
			return obj;
		}
	}

	protected LayerSubMesh GetSubMesh(Material material)
	{
		int subMeshIndex;
		return GetSubMesh(material, out subMeshIndex);
	}

	protected LayerSubMesh GetSubMesh(Material material, out int subMeshIndex)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		for (int i = 0; i < subMeshes.Count; i++)
		{
			LayerSubMesh layerSubMesh = subMeshes[i];
			if ((Object)(object)layerSubMesh.material == (Object)(object)material && layerSubMesh.verts.Count < 40000)
			{
				subMeshIndex = i;
				return layerSubMesh;
			}
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
		}
		LayerSubMesh layerSubMesh2 = new LayerSubMesh(val, material);
		subMeshIndex = subMeshes.Count;
		subMeshes.Add(layerSubMesh2);
		return layerSubMesh2;
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

	public void RegenerateNow()
	{
		dirty = false;
		Regenerate().ExecuteEnumerable();
	}

	public virtual void Render()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible)
		{
			return;
		}
		if (ShouldRegenerate)
		{
			RegenerateNow();
		}
		int renderLayer = RenderLayer;
		Vector3 position = Position;
		Quaternion rotation = Rotation;
		float alpha = Alpha;
		for (int i = 0; i < subMeshes.Count; i++)
		{
			if (subMeshes[i].finalized)
			{
				if (!Mathf.Approximately(alpha, 1f))
				{
					Color color = subMeshes[i].material.color;
					MatPropBlock.SetColor(ShaderPropertyIDs.Color, new Color(color.r, color.g, color.b, color.a * alpha));
					Graphics.DrawMesh(subMeshes[i].mesh, position, rotation, subMeshes[i].material, renderLayer, (Camera)null, 0, MatPropBlock);
				}
				else
				{
					Graphics.DrawMesh(subMeshes[i].mesh, position, rotation, subMeshes[i].material, renderLayer);
				}
			}
		}
	}

	public virtual IEnumerable Regenerate()
	{
		dirty = false;
		ClearSubMeshes(MeshParts.All);
		yield break;
	}

	public void SetDirty()
	{
		dirty = true;
	}

	private void ClearSubMeshes(MeshParts parts)
	{
		for (int i = 0; i < subMeshes.Count; i++)
		{
			subMeshes[i].Clear(parts);
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
