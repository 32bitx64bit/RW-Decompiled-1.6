using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class CellDrawer
{
	private bool markedForDraw;

	private Material material;

	private bool dirty = true;

	private List<Mesh> meshes = new List<Mesh>();

	private int mapSizeX;

	private int mapSizeZ;

	private Func<int, Color?> colorSource;

	private static List<Vector3> verts = new List<Vector3>();

	private static List<int> tris = new List<int>();

	private static List<Color> colors = new List<Color>();

	private const int MaxCellsPerMesh = 16383;

	public CellDrawer(Func<int, Color?> cellIndexToColor, Material material, int mapSizeX, int mapSizeZ)
	{
		colorSource = cellIndexToColor;
		this.material = material;
		this.mapSizeX = mapSizeX;
		this.mapSizeZ = mapSizeZ;
	}

	public void MarkForDraw()
	{
		markedForDraw = true;
	}

	public void Update()
	{
		if (markedForDraw)
		{
			Draw();
			markedForDraw = false;
		}
	}

	private void Draw()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			RegenerateMesh();
		}
		for (int i = 0; i < meshes.Count; i++)
		{
			Graphics.DrawMesh(meshes[i], Vector3.zero, Quaternion.identity, material, 0);
		}
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public void RegenerateMesh()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		for (int i = 0; i < meshes.Count; i++)
		{
			meshes[i].Clear();
		}
		int num = 0;
		int num2 = 0;
		if (meshes.Count < 1)
		{
			Mesh item = new Mesh
			{
				name = "CellDrawer"
			};
			meshes.Add(item);
		}
		Mesh mesh = meshes[num];
		CellRect cellRect = new CellRect(0, 0, mapSizeX, mapSizeZ);
		float num3 = AltitudeLayer.MapDataOverlay.AltitudeFor();
		for (int j = cellRect.minX; j <= cellRect.maxX; j++)
		{
			for (int k = cellRect.minZ; k <= cellRect.maxZ; k++)
			{
				int arg = CellIndicesUtility.CellToIndex(j, k, mapSizeX);
				Color? val = colorSource(arg);
				if (!val.HasValue)
				{
					continue;
				}
				verts.Add(new Vector3((float)j, num3, (float)k));
				verts.Add(new Vector3((float)j, num3, (float)(k + 1)));
				verts.Add(new Vector3((float)(j + 1), num3, (float)(k + 1)));
				verts.Add(new Vector3((float)(j + 1), num3, (float)k));
				Color value = val.Value;
				colors.Add(value);
				colors.Add(value);
				colors.Add(value);
				colors.Add(value);
				int count = verts.Count;
				tris.Add(count - 4);
				tris.Add(count - 3);
				tris.Add(count - 2);
				tris.Add(count - 4);
				tris.Add(count - 2);
				tris.Add(count - 1);
				num2++;
				if (num2 >= 16383)
				{
					FinalizeWorkingDataIntoMesh(mesh);
					num++;
					if (meshes.Count < num + 1)
					{
						Mesh item2 = new Mesh
						{
							name = "CellDrawer"
						};
						meshes.Add(item2);
					}
					mesh = meshes[num];
					num2 = 0;
				}
			}
		}
		FinalizeWorkingDataIntoMesh(mesh);
		dirty = false;
	}

	private void FinalizeWorkingDataIntoMesh(Mesh mesh)
	{
		if (verts.Count > 0)
		{
			mesh.SetVertices(verts);
			verts.Clear();
			mesh.SetTriangles(tris, 0);
			tris.Clear();
			mesh.SetColors(colors);
			colors.Clear();
		}
	}
}
