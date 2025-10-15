using RimWorld;
using UnityEngine;

namespace Verse;

internal class SectionLayer_SunShadows : SectionLayer_Dynamic
{
	private static readonly Color32 LowVertexColor = new Color32((byte)0, (byte)0, (byte)0, (byte)0);

	private static int frame;

	private static CellRect cachedRect;

	public override bool Visible
	{
		get
		{
			if (DebugViewSettings.drawShadows)
			{
				return !(base.Map?.Biome?.disableShadows).GetValueOrDefault();
			}
			return false;
		}
	}

	public SectionLayer_SunShadows(Section section)
		: base(section)
	{
		relevantChangeTypes = MapMeshFlagDefOf.Buildings;
	}

	public override bool ShouldDrawDynamic(CellRect view)
	{
		return section.CellRect.Overlaps(GetSunShadowsViewRect(section.map, view));
	}

	public override CellRect GetBoundaryRect()
	{
		return new CellRect(0, 0, section.map.Size.x, section.map.Size.z);
	}

	public override void DrawLayer()
	{
		RefreshSubMeshBounds();
		base.DrawLayer();
	}

	public override void Regenerate()
	{
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		if (!MatBases.SunShadow.shader.isSupported)
		{
			return;
		}
		Building[] innerArray = base.Map.edificeGrid.InnerArray;
		float num = AltitudeLayer.Shadows.AltitudeFor();
		CellRect cellRect = new CellRect(section.botLeft.x, section.botLeft.z, 17, 17);
		cellRect.ClipInsideMap(base.Map);
		LayerSubMesh subMesh = GetSubMesh(MatBases.SunShadow);
		subMesh.Clear(MeshParts.All);
		subMesh.verts.Capacity = cellRect.Area * 2;
		subMesh.tris.Capacity = cellRect.Area * 4;
		subMesh.colors.Capacity = cellRect.Area * 2;
		CellIndices cellIndices = base.Map.cellIndices;
		Color32 item = default(Color32);
		for (int i = cellRect.minX; i <= cellRect.maxX; i++)
		{
			for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
			{
				Building building = innerArray[cellIndices.CellToIndex(i, j)];
				if (building == null || !(building.def.staticSunShadowHeight > 0f))
				{
					continue;
				}
				float staticSunShadowHeight = building.def.staticSunShadowHeight;
				((Color32)(ref item))._002Ector((byte)0, (byte)0, (byte)0, (byte)(255f * staticSunShadowHeight));
				int count = subMesh.verts.Count;
				subMesh.verts.Add(new Vector3((float)i, num, (float)j));
				subMesh.verts.Add(new Vector3((float)i, num, (float)(j + 1)));
				subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)(j + 1)));
				subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)j));
				subMesh.colors.Add(LowVertexColor);
				subMesh.colors.Add(LowVertexColor);
				subMesh.colors.Add(LowVertexColor);
				subMesh.colors.Add(LowVertexColor);
				int count2 = subMesh.verts.Count;
				subMesh.tris.Add(count2 - 4);
				subMesh.tris.Add(count2 - 3);
				subMesh.tris.Add(count2 - 2);
				subMesh.tris.Add(count2 - 4);
				subMesh.tris.Add(count2 - 2);
				subMesh.tris.Add(count2 - 1);
				if (i > 0)
				{
					building = innerArray[cellIndices.CellToIndex(i - 1, j)];
					if (building == null || building.def.staticSunShadowHeight < staticSunShadowHeight)
					{
						int count3 = subMesh.verts.Count;
						subMesh.verts.Add(new Vector3((float)i, num, (float)j));
						subMesh.verts.Add(new Vector3((float)i, num, (float)(j + 1)));
						subMesh.colors.Add(item);
						subMesh.colors.Add(item);
						subMesh.tris.Add(count + 1);
						subMesh.tris.Add(count);
						subMesh.tris.Add(count3);
						subMesh.tris.Add(count3);
						subMesh.tris.Add(count3 + 1);
						subMesh.tris.Add(count + 1);
					}
				}
				if (i < base.Map.Size.x - 1)
				{
					building = innerArray[cellIndices.CellToIndex(i + 1, j)];
					if (building == null || building.def.staticSunShadowHeight < staticSunShadowHeight)
					{
						int count4 = subMesh.verts.Count;
						subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)(j + 1)));
						subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)j));
						subMesh.colors.Add(item);
						subMesh.colors.Add(item);
						subMesh.tris.Add(count + 2);
						subMesh.tris.Add(count4);
						subMesh.tris.Add(count4 + 1);
						subMesh.tris.Add(count4 + 1);
						subMesh.tris.Add(count + 3);
						subMesh.tris.Add(count + 2);
					}
				}
				if (j > 0)
				{
					building = innerArray[cellIndices.CellToIndex(i, j - 1)];
					if (building == null || building.def.staticSunShadowHeight < staticSunShadowHeight)
					{
						int count5 = subMesh.verts.Count;
						subMesh.verts.Add(new Vector3((float)i, num, (float)j));
						subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)j));
						subMesh.colors.Add(item);
						subMesh.colors.Add(item);
						subMesh.tris.Add(count);
						subMesh.tris.Add(count + 3);
						subMesh.tris.Add(count5);
						subMesh.tris.Add(count + 3);
						subMesh.tris.Add(count5 + 1);
						subMesh.tris.Add(count5);
					}
				}
			}
		}
		if (subMesh.verts.Count > 0)
		{
			subMesh.FinalizeMesh(MeshParts.Verts | MeshParts.Tris | MeshParts.Colors);
			float num2 = Mathf.Max(15f, 15f);
			Bounds bounds = subMesh.mesh.bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			size.x += 2f * num2 + 2f;
			size.z += 2f * num2 + 2f;
			subMesh.mesh.bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 1000f));
		}
	}

	public static CellRect GetSunShadowsViewRect(Map map, CellRect rect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (frame == RealTime.frameCount)
		{
			return cachedRect;
		}
		GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(map, GenCelestial.LightType.Shadow);
		if (lightSourceInfo.vector.x < 0f)
		{
			rect.maxX -= Mathf.FloorToInt(lightSourceInfo.vector.x);
		}
		else
		{
			rect.minX -= Mathf.CeilToInt(lightSourceInfo.vector.x);
		}
		if (lightSourceInfo.vector.y < 0f)
		{
			rect.maxZ -= Mathf.FloorToInt(lightSourceInfo.vector.y);
		}
		else
		{
			rect.minZ -= Mathf.CeilToInt(lightSourceInfo.vector.y);
		}
		frame = RealTime.frameCount;
		return cachedRect = rect.ClipInsideMap(map);
	}
}
