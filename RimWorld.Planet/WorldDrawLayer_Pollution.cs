using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldDrawLayer_Pollution : WorldDrawLayer
{
	private const int TilesPerSubMesh = 500;

	private const float ScaleUVFactor = 0.1f;

	private static readonly Color DefaultTileColor = Color.white;

	private static readonly Color BordersUnpollutedTileColor = new Color(1f, 1f, 1f, 0.4f);

	private readonly List<Vector3> verts = new List<Vector3>();

	private readonly Dictionary<int, List<LayerSubMesh>> subMeshesByRegion = new Dictionary<int, List<LayerSubMesh>>();

	private readonly Queue<int> regionsToRegenerate = new Queue<int>();

	private Material lightPollution;

	private Material moderatePollution;

	private Material extemePollution;

	private readonly List<PlanetTile> tmpNeighbors = new List<PlanetTile>();

	private readonly HashSet<Vector3> tmpBordersUnpollutedVerts = new HashSet<Vector3>();

	private readonly List<Vector3> tmpVerts = new List<Vector3>();

	private static readonly List<PlanetTile> TmpChangedNeighbours = new List<PlanetTile>();

	private Material LightPollution
	{
		get
		{
			if ((Object)(object)lightPollution == (Object)null)
			{
				lightPollution = MaterialPool.MatFrom("World/Pollution/Light", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return lightPollution;
		}
	}

	private Material ModeratePollution
	{
		get
		{
			if ((Object)(object)moderatePollution == (Object)null)
			{
				moderatePollution = MaterialPool.MatFrom("World/Pollution/Moderate", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return moderatePollution;
		}
	}

	private Material ExtremePollution
	{
		get
		{
			if ((Object)(object)extemePollution == (Object)null)
			{
				extemePollution = MaterialPool.MatFrom("World/Pollution/Extreme", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return extemePollution;
		}
	}

	private int GetRegionIdForTile(PlanetTile tile)
	{
		return Mathf.FloorToInt((float)tile.tileId / 500f);
	}

	public List<LayerSubMesh> GetSubMeshesForRegion(int regionId)
	{
		if (!subMeshesByRegion.ContainsKey(regionId))
		{
			subMeshesByRegion[regionId] = new List<LayerSubMesh>();
		}
		return subMeshesByRegion[regionId];
	}

	public LayerSubMesh GetSubMeshForMaterialAndRegion(Material material, int regionId)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
		for (int i = 0; i < subMeshesForRegion.Count; i++)
		{
			if ((Object)(object)subMeshesForRegion[i].material == (Object)(object)material)
			{
				return subMeshesForRegion[i];
			}
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
		}
		LayerSubMesh layerSubMesh = new LayerSubMesh(val, material);
		subMeshesForRegion.Add(layerSubMesh);
		subMeshes.Add(layerSubMesh);
		return layerSubMesh;
	}

	private void RegenerateRegion(int regionId)
	{
		List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
		for (int i = 0; i < subMeshesForRegion.Count; i++)
		{
			subMeshesForRegion[i].Clear(MeshParts.All);
		}
		int num = regionId * 500;
		int num2 = num + 500;
		for (int j = num; j < num2; j++)
		{
			PlanetTile tile = new PlanetTile(j, planetLayer);
			if (!Find.World.grid.InBounds(tile))
			{
				break;
			}
			TryAddMeshForTile(tile);
		}
		for (int k = 0; k < subMeshesForRegion.Count; k++)
		{
			if (subMeshesForRegion[k].verts.Count > 0)
			{
				subMeshesForRegion[k].FinalizeMesh(MeshParts.All);
			}
		}
	}

	public override IEnumerable Regenerate()
	{
		if (!ModsConfig.BiotechActive)
		{
			yield break;
		}
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
		int pollutedMeshesPrinted = 0;
		verts.Clear();
		subMeshesByRegion.Clear();
		regionsToRegenerate.Clear();
		for (int i = 0; i < planetLayer.TilesCount; i++)
		{
			if (TryAddMeshForTile(planetLayer.PlanetTileForID(i)))
			{
				pollutedMeshesPrinted++;
				if (pollutedMeshesPrinted % 1000 == 0)
				{
					yield return null;
				}
			}
		}
		FinalizeMesh(MeshParts.All);
	}

	private bool TryAddMeshForTile(PlanetTile tile)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		PollutionLevel pollution = tile.Tile.PollutionLevel();
		Material materialForTilePollution = GetMaterialForTilePollution(pollution);
		if ((Object)(object)materialForTilePollution == (Object)null)
		{
			return false;
		}
		int regionIdForTile = GetRegionIdForTile(tile);
		LayerSubMesh subMeshForMaterialAndRegion = GetSubMeshForMaterialAndRegion(materialForTilePollution, regionIdForTile);
		Find.WorldGrid.GetTileVertices(tile, verts);
		Find.WorldGrid.GetTileNeighbors(tile, tmpNeighbors);
		int count = subMeshForMaterialAndRegion.verts.Count;
		tmpBordersUnpollutedVerts.Clear();
		tmpVerts.Clear();
		for (int i = 0; i < tmpNeighbors.Count; i++)
		{
			if (planetLayer[tmpNeighbors[i]].PollutionLevel() < PollutionLevel.Moderate)
			{
				Vector3 center = Find.WorldGrid.GetTileCenter(tmpNeighbors[i]);
				tmpVerts.AddRange(verts);
				tmpVerts.SortBy((Vector3 v) => Vector2.Distance(Vector2.op_Implicit(center), Vector2.op_Implicit(v)));
				for (int j = 0; j < 2; j++)
				{
					tmpBordersUnpollutedVerts.Add(tmpVerts[j]);
				}
			}
		}
		int k = 0;
		for (int count2 = verts.Count; k < count2; k++)
		{
			Vector3 val = verts[k];
			Vector3 val2 = verts[k];
			Vector3 val3 = val + ((Vector3)(ref val2)).normalized * 0.02f;
			subMeshForMaterialAndRegion.verts.Add(val3);
			subMeshForMaterialAndRegion.uvs.Add(val3 * 0.1f);
			Color val4 = (tmpBordersUnpollutedVerts.Contains(verts[k]) ? BordersUnpollutedTileColor : DefaultTileColor);
			subMeshForMaterialAndRegion.colors.Add(Color32.op_Implicit(val4));
			if (k < count2 - 2)
			{
				subMeshForMaterialAndRegion.tris.Add(count + k + 2);
				subMeshForMaterialAndRegion.tris.Add(count + k + 1);
				subMeshForMaterialAndRegion.tris.Add(count);
			}
		}
		tmpBordersUnpollutedVerts.Clear();
		tmpVerts.Clear();
		return true;
	}

	private Material GetMaterialForTilePollution(PollutionLevel pollution)
	{
		return (Material)(pollution switch
		{
			PollutionLevel.Light => LightPollution, 
			PollutionLevel.Moderate => ModeratePollution, 
			PollutionLevel.Extreme => ExtremePollution, 
			_ => null, 
		});
	}

	public void Notify_TilePollutionChanged(PlanetTile tileId)
	{
		int regionIdForTile = GetRegionIdForTile(tileId);
		if (!regionsToRegenerate.Contains(regionIdForTile))
		{
			regionsToRegenerate.Enqueue(regionIdForTile);
		}
		Find.WorldGrid.GetTileNeighbors(tileId, TmpChangedNeighbours);
		for (int i = 0; i < TmpChangedNeighbours.Count; i++)
		{
			int regionIdForTile2 = GetRegionIdForTile(TmpChangedNeighbours[i]);
			if (!regionsToRegenerate.Contains(regionIdForTile2))
			{
				regionsToRegenerate.Enqueue(regionIdForTile2);
			}
		}
		TmpChangedNeighbours.Clear();
	}

	public override void Render()
	{
		if (regionsToRegenerate.Count > 0)
		{
			int regionId = regionsToRegenerate.Dequeue();
			RegenerateRegion(regionId);
		}
		base.Render();
	}
}
