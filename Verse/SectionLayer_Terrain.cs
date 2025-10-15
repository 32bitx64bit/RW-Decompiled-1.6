using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class SectionLayer_Terrain : SectionLayer
{
	private static readonly Color32 ColorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private static readonly Color32 ColorClear = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)0);

	public const float MaxWeatherBuildupCoverageForVisualPollution = 0.4f;

	public override bool Visible => DebugViewSettings.drawTerrain;

	public SectionLayer_Terrain(Section section)
		: base(section)
	{
		relevantChangeTypes = MapMeshFlagDefOf.Terrain;
	}

	public virtual Material GetMaterialFor(CellTerrain cellTerrain)
	{
		bool polluted = cellTerrain.polluted && cellTerrain.snowCoverage < 0.4f && cellTerrain.sandCoverage < 0.4f && cellTerrain.def.graphicPolluted != BaseContent.BadGraphic && !WorldComponent_GravshipController.DisableDrawingPollution;
		return base.Map.terrainGrid.GetMaterial(cellTerrain.def, polluted, cellTerrain.color);
	}

	public bool AllowRenderingFor(TerrainDef terrain)
	{
		if (!DebugViewSettings.drawTerrainWater)
		{
			return !terrain.HasTag("Water");
		}
		return true;
	}

	public override void Regenerate()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		ClearSubMeshes(MeshParts.All);
		TerrainGrid terrainGrid = base.Map.terrainGrid;
		CellRect cellRect = section.CellRect;
		CellTerrain[] array = new CellTerrain[8];
		HashSet<CellTerrain> hashSet = new HashSet<CellTerrain>();
		bool[] array2 = new bool[8];
		foreach (IntVec3 item in cellRect)
		{
			hashSet.Clear();
			TerrainDef terrainDef = terrainGrid.TerrainAt(item);
			CellTerrain cellTerrain = new CellTerrain(terrainGrid.TerrainAt(item), item.IsPolluted(base.Map), base.Map.snowGrid.GetDepth(item), item.GetSandDepth(base.Map), terrainGrid.ColorAt(item));
			LayerSubMesh subMesh = GetSubMesh(terrainDef.dontRender ? MatBases.ShadowMask : GetMaterialFor(cellTerrain));
			float num = AltitudeLayer.Terrain.AltitudeFor();
			if (subMesh != null && AllowRenderingFor(cellTerrain.def))
			{
				int count = subMesh.verts.Count;
				subMesh.verts.Add(new Vector3((float)item.x, num, (float)item.z));
				subMesh.verts.Add(new Vector3((float)item.x, num, (float)(item.z + 1)));
				subMesh.verts.Add(new Vector3((float)(item.x + 1), num, (float)(item.z + 1)));
				subMesh.verts.Add(new Vector3((float)(item.x + 1), num, (float)item.z));
				subMesh.colors.Add(ColorWhite);
				subMesh.colors.Add(ColorWhite);
				subMesh.colors.Add(ColorWhite);
				subMesh.colors.Add(ColorWhite);
				subMesh.tris.Add(count);
				subMesh.tris.Add(count + 1);
				subMesh.tris.Add(count + 2);
				subMesh.tris.Add(count);
				subMesh.tris.Add(count + 2);
				subMesh.tris.Add(count + 3);
			}
			for (int i = 0; i < 8; i++)
			{
				IntVec3 c = item + GenAdj.AdjacentCellsAroundBottom[i];
				if (!c.InBounds(base.Map))
				{
					array[i] = cellTerrain;
					continue;
				}
				CellTerrain cellTerrain2 = new CellTerrain(terrainGrid.TerrainAt(c), c.IsPolluted(base.Map), base.Map.snowGrid.GetDepth(c), c.GetSandDepth(base.Map), terrainGrid.ColorAt(c));
				Thing edifice = c.GetEdifice(base.Map);
				if (edifice != null && edifice.def.coversFloor)
				{
					cellTerrain2.def = TerrainDefOf.Underwall;
				}
				array[i] = cellTerrain2;
				if (!cellTerrain2.Equals(cellTerrain) && cellTerrain2.def.edgeType != 0 && terrainGrid.FoundationAt(item) == null && terrainGrid.FoundationAt(c) == null && cellTerrain2.def.renderPrecedence >= cellTerrain.def.renderPrecedence)
				{
					hashSet.Add(cellTerrain2);
				}
			}
			foreach (CellTerrain item2 in hashSet)
			{
				LayerSubMesh subMesh2 = GetSubMesh(GetMaterialFor(item2));
				if (subMesh2 == null || !AllowRenderingFor(item2.def))
				{
					continue;
				}
				int count = subMesh2.verts.Count;
				subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, (float)item.z));
				subMesh2.verts.Add(new Vector3((float)item.x, 0f, (float)item.z));
				subMesh2.verts.Add(new Vector3((float)item.x, 0f, (float)item.z + 0.5f));
				subMesh2.verts.Add(new Vector3((float)item.x, 0f, (float)(item.z + 1)));
				subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, (float)(item.z + 1)));
				subMesh2.verts.Add(new Vector3((float)(item.x + 1), 0f, (float)(item.z + 1)));
				subMesh2.verts.Add(new Vector3((float)(item.x + 1), 0f, (float)item.z + 0.5f));
				subMesh2.verts.Add(new Vector3((float)(item.x + 1), 0f, (float)item.z));
				subMesh2.verts.Add(new Vector3((float)item.x + 0.5f, 0f, (float)item.z + 0.5f));
				for (int j = 0; j < 8; j++)
				{
					array2[j] = false;
				}
				for (int k = 0; k < 8; k++)
				{
					if (k % 2 == 0)
					{
						if (array[k].Equals(item2))
						{
							array2[(k - 1 + 8) % 8] = true;
							array2[k] = true;
							array2[(k + 1) % 8] = true;
						}
					}
					else if (array[k].Equals(item2))
					{
						array2[k] = true;
					}
				}
				for (int l = 0; l < 8; l++)
				{
					subMesh2.colors.Add(array2[l] ? ColorWhite : ColorClear);
				}
				subMesh2.colors.Add(ColorClear);
				for (int m = 0; m < 8; m++)
				{
					subMesh2.tris.Add(count + m);
					subMesh2.tris.Add(count + (m + 1) % 8);
					subMesh2.tris.Add(count + 8);
				}
			}
		}
		FinalizeMesh(MeshParts.All);
	}
}
