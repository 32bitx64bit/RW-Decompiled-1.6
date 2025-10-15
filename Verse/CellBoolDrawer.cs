using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class CellBoolDrawer
{
	private bool wantDraw;

	private Material material;

	private bool materialCaresAboutVertexColors;

	private bool dirty = true;

	private List<Mesh> meshes = new List<Mesh>();

	private int mapSizeX;

	private int mapSizeZ;

	private float opacity = 0.33f;

	private int renderQueue = 3600;

	private Func<Color> colorGetter;

	private Func<int, Color> extraColorGetter;

	private Func<int, bool> cellBoolGetter;

	private static List<Vector3> verts = new List<Vector3>();

	private static List<int> tris = new List<int>();

	private static List<Color> colors = new List<Color>();

	private const float DefaultOpacity = 0.33f;

	private const int MaxCellsPerMesh = 16383;

	private CellBoolDrawer(int mapSizeX, int mapSizeZ, float opacity = 0.33f)
	{
		this.mapSizeX = mapSizeX;
		this.mapSizeZ = mapSizeZ;
		this.opacity = opacity;
	}

	public CellBoolDrawer(ICellBoolGiver giver, int mapSizeX, int mapSizeZ, float opacity = 0.33f)
		: this(mapSizeX, mapSizeZ, opacity)
	{
		colorGetter = () => giver.Color;
		extraColorGetter = giver.GetCellExtraColor;
		cellBoolGetter = giver.GetCellBool;
	}

	public CellBoolDrawer(ICellBoolGiver giver, int mapSizeX, int mapSizeZ, int renderQueue, float opacity = 0.33f)
		: this(giver, mapSizeX, mapSizeZ, opacity)
	{
		this.renderQueue = renderQueue;
	}

	public CellBoolDrawer(Func<int, bool> cellBoolGetter, Func<Color> colorGetter, Func<int, Color> extraColorGetter, int mapSizeX, int mapSizeZ, float opacity = 0.33f)
		: this(mapSizeX, mapSizeZ, opacity)
	{
		this.colorGetter = colorGetter;
		this.extraColorGetter = extraColorGetter;
		this.cellBoolGetter = cellBoolGetter;
	}

	public CellBoolDrawer(Func<int, bool> cellBoolGetter, Func<Color> colorGetter, Func<int, Color> extraColorGetter, int mapSizeX, int mapSizeZ, int renderQueue, float opacity = 0.33f)
		: this(cellBoolGetter, colorGetter, extraColorGetter, mapSizeX, mapSizeZ, opacity)
	{
		this.renderQueue = renderQueue;
	}

	public void MarkForDraw()
	{
		wantDraw = true;
	}

	public void CellBoolDrawerUpdate()
	{
		if (wantDraw)
		{
			ActuallyDraw();
			wantDraw = false;
		}
	}

	private void ActuallyDraw()
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
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
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
				name = "CellBoolDrawer"
			};
			meshes.Add(item);
		}
		Mesh mesh = meshes[num];
		CellRect cellRect = new CellRect(0, 0, mapSizeX, mapSizeZ);
		float num3 = AltitudeLayer.MapDataOverlay.AltitudeFor();
		bool careAboutVertexColors = false;
		for (int j = cellRect.minX; j <= cellRect.maxX; j++)
		{
			for (int k = cellRect.minZ; k <= cellRect.maxZ; k++)
			{
				int arg = CellIndicesUtility.CellToIndex(j, k, mapSizeX);
				if (!cellBoolGetter(arg))
				{
					continue;
				}
				verts.Add(new Vector3((float)j, num3, (float)k));
				verts.Add(new Vector3((float)j, num3, (float)(k + 1)));
				verts.Add(new Vector3((float)(j + 1), num3, (float)(k + 1)));
				verts.Add(new Vector3((float)(j + 1), num3, (float)k));
				Color val = extraColorGetter(arg);
				colors.Add(val);
				colors.Add(val);
				colors.Add(val);
				colors.Add(val);
				if (val != Color.white)
				{
					careAboutVertexColors = true;
				}
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
							name = "CellBoolDrawer"
						};
						meshes.Add(item2);
					}
					mesh = meshes[num];
					num2 = 0;
				}
			}
		}
		FinalizeWorkingDataIntoMesh(mesh);
		CreateMaterialIfNeeded(careAboutVertexColors);
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

	private void CreateMaterialIfNeeded(bool careAboutVertexColors)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)material == (Object)null || materialCaresAboutVertexColors != careAboutVertexColors)
		{
			Color val = colorGetter();
			material = SolidColorMaterials.SimpleSolidColorMaterial(new Color(val.r, val.g, val.b, opacity * val.a), careAboutVertexColors);
			materialCaresAboutVertexColors = careAboutVertexColors;
			material.renderQueue = renderQueue;
		}
	}

	public void Notify_ColorChanged()
	{
		material = null;
		SetDirty();
	}
}
