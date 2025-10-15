using UnityEngine;

namespace Verse;

public static class SectionLayerGeometryMaker_Solid
{
	public static void MakeBaseGeometry(Section section, LayerSubMesh sm, AltitudeLayer altitudeLayer)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		sm.Clear(MeshParts.Verts | MeshParts.Tris);
		CellRect cellRect = new CellRect(section.botLeft.x, section.botLeft.z, 17, 17);
		cellRect.ClipInsideMap(section.map);
		float num = altitudeLayer.AltitudeFor();
		sm.verts.Capacity = cellRect.Area * 9;
		for (int i = cellRect.minX; i <= cellRect.maxX; i++)
		{
			for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
			{
				sm.verts.Add(new Vector3((float)i, num, (float)j));
				sm.verts.Add(new Vector3((float)i, num, (float)j + 0.5f));
				sm.verts.Add(new Vector3((float)i, num, (float)(j + 1)));
				sm.verts.Add(new Vector3((float)i + 0.5f, num, (float)(j + 1)));
				sm.verts.Add(new Vector3((float)(i + 1), num, (float)(j + 1)));
				sm.verts.Add(new Vector3((float)(i + 1), num, (float)j + 0.5f));
				sm.verts.Add(new Vector3((float)(i + 1), num, (float)j));
				sm.verts.Add(new Vector3((float)i + 0.5f, num, (float)j));
				sm.verts.Add(new Vector3((float)i + 0.5f, num, (float)j + 0.5f));
			}
		}
		int num2 = cellRect.Area * 8 * 3;
		sm.tris.Capacity = num2;
		int num3 = 0;
		while (sm.tris.Count < num2)
		{
			sm.tris.Add(num3 + 7);
			sm.tris.Add(num3);
			sm.tris.Add(num3 + 1);
			sm.tris.Add(num3 + 1);
			sm.tris.Add(num3 + 2);
			sm.tris.Add(num3 + 3);
			sm.tris.Add(num3 + 3);
			sm.tris.Add(num3 + 4);
			sm.tris.Add(num3 + 5);
			sm.tris.Add(num3 + 5);
			sm.tris.Add(num3 + 6);
			sm.tris.Add(num3 + 7);
			sm.tris.Add(num3 + 7);
			sm.tris.Add(num3 + 1);
			sm.tris.Add(num3 + 8);
			sm.tris.Add(num3 + 1);
			sm.tris.Add(num3 + 3);
			sm.tris.Add(num3 + 8);
			sm.tris.Add(num3 + 3);
			sm.tris.Add(num3 + 5);
			sm.tris.Add(num3 + 8);
			sm.tris.Add(num3 + 5);
			sm.tris.Add(num3 + 7);
			sm.tris.Add(num3 + 8);
			num3 += 9;
		}
		sm.FinalizeMesh(MeshParts.Verts | MeshParts.Tris);
	}
}
