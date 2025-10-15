using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldDrawLayer_Terrain : WorldDrawLayer
{
	private readonly List<Vector3> elevationValues = new List<Vector3>();

	private static bool setGlobalMacro;

	private static readonly List<PlanetTile> neighbours = new List<PlanetTile>(8);

	private const string MacroTexturePath = "World/WorldMacro";

	public override IEnumerable Regenerate()
	{
		foreach (object item2 in base.Regenerate())
		{
			yield return item2;
		}
		if (!setGlobalMacro)
		{
			setGlobalMacro = true;
			Shader.SetGlobalTexture(ShaderPropertyIDs.MacroTex, (Texture)(object)ContentFinder<Texture2D>.Get("World/WorldMacro"));
		}
		NativeArray<int> tileIDToVerts_offsets = planetLayer.UnsafeTileIDToVerts_offsets;
		NativeArray<Vector3> verts = planetLayer.UnsafeVerts;
		foreach (object item3 in CalculateInterpolatedVerticesParams())
		{
			yield return item3;
		}
		int num = 0;
		for (int i = 0; i < planetLayer.TilesCount; i++)
		{
			Tile tile = planetLayer[i];
			BiomeDef primaryBiome = tile.PrimaryBiome;
			Landmark landmark = tile.Landmark;
			bool flag = landmark != null && landmark.def.drawType == LandmarkDef.LandmarkDrawType.TerrainMask;
			Material val = primaryBiome.DrawMaterial;
			if (flag)
			{
				MaterialRequest req = new MaterialRequest(val.mainTexture, val.shader);
				req.maskTex = landmark.def.Texture;
				req.secondaryTex = planetLayer.Def.backgroundBiome.DrawMaterial.mainTexture;
				req.renderQueue = val.renderQueue;
				req.shaderParameters = landmark.def.terrainParameters;
				val = MaterialPool.MatFrom(req);
			}
			int subMeshIndex;
			LayerSubMesh subMesh = GetSubMesh(val, out subMeshIndex);
			int count = subMesh.verts.Count;
			int num2 = 0;
			int oneAfterLastVertIndex = GetOneAfterLastVertIndex(i, tileIDToVerts_offsets, verts);
			int num3 = oneAfterLastVertIndex - tileIDToVerts_offsets[i];
			int num4 = tileIDToVerts_offsets[i];
			int num5 = 0;
			int num6 = 0;
			if (flag)
			{
				num5 = Rand.RangeSeeded(0, landmark.def.atlasSize.x, i);
				num6 = Rand.RangeSeeded(0, landmark.def.atlasSize.z, i ^ 0x1BD3E0);
			}
			int num7 = 0;
			if (flag && landmark.def.coastRotateMode != 0)
			{
				num7 = GetLandmarkRotationIncrements(tile, planetLayer, landmark, tileIDToVerts_offsets, verts);
			}
			for (int j = num4; j < oneAfterLastVertIndex; j++)
			{
				Vector3 item = verts[j];
				subMesh.verts.Add(item);
				subMesh.uvs.Add(elevationValues[num]);
				if (flag)
				{
					float angleOffset = 360f / (float)num3 * (float)(-num7);
					Vector2 val2 = (GenGeo.RegularPolygonVertexPosition(num3, num2, angleOffset) + Vector2.one) / 2f;
					float num8 = 1f / (float)landmark.def.atlasSize.x;
					float num9 = 1f / (float)landmark.def.atlasSize.z;
					float num10 = (float)num5 * num8;
					float num11 = (float)num6 * num9;
					val2.x = Mathf.Lerp(num10, num10 + num8, val2.x);
					val2.y = Mathf.Lerp(num11, num11 + num9, val2.y);
					subMesh.uvsChannelTwo.Add(Vector2.op_Implicit(val2));
				}
				num++;
				if (j < oneAfterLastVertIndex - 2)
				{
					subMesh.tris.Add(count + num2 + 2);
					subMesh.tris.Add(count + num2 + 1);
					subMesh.tris.Add(count);
					AppendRaycastableTriangle(subMeshIndex, i);
				}
				num2++;
			}
		}
		FinalizeMesh(MeshParts.All);
		foreach (object item4 in RegenerateWorldMeshColliders())
		{
			yield return item4;
		}
		elevationValues.Clear();
		elevationValues.TrimExcess();
	}

	private static int GetLandmarkRotationIncrements(Tile tile, PlanetLayer layer, Landmark landmark, NativeArray<int> tileIDToVerts_offsets, NativeArray<Vector3> verts)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		int oneAfterLastVertIndex = GetOneAfterLastVertIndex(tile.tile.tileId, tileIDToVerts_offsets, verts);
		int num = tileIDToVerts_offsets[tile.tile.tileId];
		int num2 = 0;
		layer.GetTileNeighbors(tile.tile, neighbours);
		PlanetTile planetTile = PlanetTile.Invalid;
		for (int i = 0; i < neighbours.Count; i++)
		{
			if (LandmarkDef.IsValidRotatableNeighbour(layer[neighbours[i]].PrimaryBiome, landmark.def.coastRotateMode))
			{
				planetTile = neighbours[i];
				break;
			}
		}
		if (planetTile.Valid)
		{
			int num3 = tileIDToVerts_offsets[planetTile.tileId];
			int oneAfterLastVertIndex2 = GetOneAfterLastVertIndex(planetTile.tileId, tileIDToVerts_offsets, verts);
			bool flag = false;
			for (int j = num; j < oneAfterLastVertIndex; j++)
			{
				int num4 = WrapIndex(num, oneAfterLastVertIndex, j + 1);
				int num5 = WrapIndex(num, oneAfterLastVertIndex, j + 2);
				for (int k = num3; k < oneAfterLastVertIndex2; k++)
				{
					int num6 = WrapIndex(num3, oneAfterLastVertIndex2, k);
					int num7 = WrapIndex(num3, oneAfterLastVertIndex2, k + 1);
					if ((!(verts[num4] != verts[num6]) || !(verts[num5] != verts[num6])) && (!(verts[num4] != verts[num7]) || !(verts[num5] != verts[num7])))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				num2++;
			}
			if (!flag)
			{
				Log.Error($"Failed to get increments for rotation of landmark with a valid neighbouring tile. Tile: {tile}");
			}
		}
		else
		{
			Log.Error($"Failed to get non background biome near tile with a landmark which has to rotate away from the background. Tile: {tile}");
		}
		return num2;
		static int WrapIndex(int start, int oneAfterLast, int index)
		{
			if (index >= oneAfterLast)
			{
				return start + index % oneAfterLast;
			}
			return index;
		}
	}

	private static int GetOneAfterLastVertIndex(int index, NativeArray<int> tileIDToVerts_offsets, NativeArray<Vector3> verts)
	{
		if (index + 1 >= tileIDToVerts_offsets.Length)
		{
			return verts.Length;
		}
		return tileIDToVerts_offsets[index + 1];
	}

	private IEnumerable CalculateInterpolatedVerticesParams()
	{
		elevationValues.Clear();
		int tilesCount = planetLayer.TilesCount;
		NativeArray<Vector3> verts = planetLayer.UnsafeVerts;
		NativeArray<int> tileIDToVerts_offsets = planetLayer.UnsafeTileIDToVerts_offsets;
		NativeArray<int> tileIDToNeighbors_offsets = planetLayer.UnsafeTileIDToNeighbors_offsets;
		NativeArray<PlanetTile> tileIDToNeighbors_values = planetLayer.UnsafeTileIDToNeighbors_values;
		for (int i = 0; i < planetLayer.TilesCount; i++)
		{
			Tile tile = planetLayer[i];
			float elevation = tile.elevation;
			int num = ((i + 1 < tileIDToNeighbors_offsets.Length) ? tileIDToNeighbors_offsets[i + 1] : tileIDToNeighbors_values.Length);
			int num2 = ((i + 1 < tilesCount) ? tileIDToVerts_offsets[i + 1] : verts.Length);
			for (int j = tileIDToVerts_offsets[i]; j < num2; j++)
			{
				Vector3 val = default(Vector3);
				val.x = elevation;
				Vector3 val2 = val;
				bool flag = false;
				for (int k = tileIDToNeighbors_offsets[i]; k < num; k++)
				{
					int num3 = ((tileIDToNeighbors_values[k].tileId + 1 < tileIDToVerts_offsets.Length) ? tileIDToVerts_offsets[tileIDToNeighbors_values[k].tileId + 1] : verts.Length);
					for (int l = tileIDToVerts_offsets[tileIDToNeighbors_values[k].tileId]; l < num3; l++)
					{
						if (!(verts[l] == verts[j]))
						{
							continue;
						}
						Tile tile2 = planetLayer[tileIDToNeighbors_values[k]];
						if (!flag)
						{
							if ((tile2.elevation >= 0f && elevation <= 0f) || (tile2.elevation <= 0f && elevation >= 0f))
							{
								flag = true;
							}
							else if (tile2.elevation > val2.x)
							{
								val2.x = tile2.elevation;
							}
						}
						break;
					}
				}
				val2.y = val2.x;
				if (flag)
				{
					val2.x = 0f;
				}
				if ((Object)(object)tile.PrimaryBiome.DrawMaterial.shader != (Object)(object)ShaderDatabase.WorldOcean && val2.x < 0f)
				{
					val2.x = 0f;
				}
				elevationValues.Add(val2);
			}
			if (i % 1000 == 0)
			{
				yield return null;
			}
		}
	}
}
