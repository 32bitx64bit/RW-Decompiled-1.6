using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class SectionLayer_GravshipMask : SectionLayer
{
	public enum MaskOverrideMode
	{
		None,
		Shadow,
		Gravship
	}

	public static Building_GravEngine Engine { get; set; }

	public static MaskOverrideMode OverrideMode { get; set; }

	public SectionLayer_GravshipMask(Section section)
		: base(section)
	{
		relevantChangeTypes = MapMeshFlagDefOf.None;
	}

	public static void ResetStaticData()
	{
		Engine = null;
		OverrideMode = MaskOverrideMode.None;
	}

	public static LayerSubMesh BakeGravshipShadowMask(Map map, Material mat, IEnumerable<IntVec3> cells)
	{
		LayerSubMesh layerSubMesh = MapDrawLayer.CreateFreeSubMesh(mat);
		RegenerateGravshipShadowMask(map, layerSubMesh, cells);
		if (layerSubMesh.verts.Count > 0)
		{
			layerSubMesh.FinalizeMesh(MeshParts.Verts | MeshParts.Tris);
		}
		return layerSubMesh;
	}

	public static bool IsValidSubstructure(IntVec3 c)
	{
		if (Engine != null && Engine.Spawned && Engine.ValidSubstructureNoRegen != null)
		{
			return Engine.ValidSubstructureNoRegen.Contains(c);
		}
		return false;
	}

	public static LayerSubMesh BakeDummyShadowMask(Material mat, Vector3 origin, float width, float height)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		LayerSubMesh layerSubMesh = MapDrawLayer.CreateFreeSubMesh(mat);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(width * 0.5f, 0f, height * 0.5f);
		layerSubMesh.verts.Add(origin - val);
		layerSubMesh.verts.Add(origin + new Vector3(0f - val.x, 0f, val.z));
		layerSubMesh.verts.Add(origin + val);
		layerSubMesh.verts.Add(origin + new Vector3(val.x, 0f, 0f - val.z));
		int count = layerSubMesh.verts.Count;
		layerSubMesh.tris.Add(count - 4);
		layerSubMesh.tris.Add(count - 3);
		layerSubMesh.tris.Add(count - 2);
		layerSubMesh.tris.Add(count - 4);
		layerSubMesh.tris.Add(count - 2);
		layerSubMesh.tris.Add(count - 1);
		layerSubMesh.FinalizeMesh(MeshParts.Verts | MeshParts.Tris);
		return layerSubMesh;
	}

	private void RegenerateGravshipMask(LayerSubMesh subMesh)
	{
		Building_GravEngine engine = Engine;
		if (engine == null || !engine.Spawned)
		{
			return;
		}
		HashSet<IntVec3> validSubstructure = Engine.ValidSubstructure;
		foreach (IntVec3 item in section.CellRect)
		{
			if (validSubstructure.Contains(item))
			{
				SectionLayer_IndoorMask.AppendQuadToMesh(subMesh, item.x, item.z, 0f);
			}
		}
	}

	private static void RegenerateGravshipShadowMask(Map map, LayerSubMesh subMesh, IEnumerable<IntVec3> cells)
	{
		string defName = TerrainDefOf.Space.defName;
		foreach (IntVec3 cell in cells)
		{
			if (cell.InBounds(map) && map.terrainGrid.TerrainAt(cell).defName != defName)
			{
				SectionLayer_IndoorMask.AppendQuadToMesh(subMesh, cell.x, cell.z, 0f);
			}
		}
	}

	public override void Regenerate()
	{
		LayerSubMesh subMesh = GetSubMesh(MatBases.GravshipMask);
		subMesh.Clear(MeshParts.All);
		switch (OverrideMode)
		{
		case MaskOverrideMode.Shadow:
			RegenerateGravshipShadowMask(base.Map, subMesh, section.CellRect);
			break;
		case MaskOverrideMode.Gravship:
			RegenerateGravshipMask(subMesh);
			break;
		case MaskOverrideMode.None:
			if (WorldComponent_GravshipController.CutsceneInProgress || Find.GravshipController.IsGravshipTravelling)
			{
				RegenerateGravshipShadowMask(base.Map, subMesh, section.CellRect);
			}
			else
			{
				RegenerateGravshipMask(subMesh);
			}
			break;
		}
		if (subMesh.verts.Count > 0)
		{
			subMesh.FinalizeMesh(MeshParts.Verts | MeshParts.Tris);
		}
	}
}
