using RimWorld;
using UnityEngine;

namespace Verse;

public class SectionLayer_Plans : SectionLayer
{
	public override bool Visible => DebugViewSettings.drawWorldOverlays;

	public SectionLayer_Plans(Section section)
		: base(section)
	{
		relevantChangeTypes = MapMeshFlagDefOf.Plan;
	}

	public override void DrawLayer()
	{
		if (!Find.ScreenshotModeHandler.Active)
		{
			base.DrawLayer();
		}
	}

	public override void Regenerate()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		float num = AltitudeLayer.MetaOverlays.AltitudeFor();
		PlanManager planManager = base.Map.planManager;
		CellRect cellRect = new CellRect(section.botLeft.x, section.botLeft.z, 17, 17);
		cellRect.ClipInsideMap(base.Map);
		ClearSubMeshes(MeshParts.All);
		for (int i = cellRect.minX; i <= cellRect.maxX; i++)
		{
			for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
			{
				Plan plan = planManager.PlanAt(new IntVec3(i, 0, j));
				if (plan != null && !plan.Hidden)
				{
					LayerSubMesh subMesh = GetSubMesh(plan.Material);
					int count = subMesh.verts.Count;
					subMesh.verts.Add(new Vector3((float)i, num, (float)j));
					subMesh.verts.Add(new Vector3((float)i, num, (float)(j + 1)));
					subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)(j + 1)));
					subMesh.verts.Add(new Vector3((float)(i + 1), num, (float)j));
					subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 0f)));
					subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 1f)));
					subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 1f)));
					subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 0f)));
					subMesh.tris.Add(count);
					subMesh.tris.Add(count + 1);
					subMesh.tris.Add(count + 2);
					subMesh.tris.Add(count);
					subMesh.tris.Add(count + 2);
					subMesh.tris.Add(count + 3);
				}
			}
		}
		FinalizeMesh(MeshParts.Verts | MeshParts.Tris | MeshParts.UVs1);
	}
}
