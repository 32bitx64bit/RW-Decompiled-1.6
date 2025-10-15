using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class SectionLayer_Gas : SectionLayer
{
	private MaterialPropertyBlock propertyBlock;

	private static Material GasMat = MaterialPool.MatFrom("Things/Gas/GasCloudThickA", ShaderDatabase.GasRotating, 3000);

	private static bool gasMatSet = false;

	protected virtual FloatRange VertexScaleOffsetRange => new FloatRange(0.4f, 0.6f);

	protected virtual FloatRange VertexPositionOffsetRange => new FloatRange(-0.2f, 0.2f);

	public override bool Visible => DebugViewSettings.drawGas;

	public virtual Material Mat => GasMat;

	public SectionLayer_Gas(Section section)
		: base(section)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		relevantChangeTypes = MapMeshFlagDefOf.Gas;
		propertyBlock = new MaterialPropertyBlock();
		if (!gasMatSet)
		{
			gasMatSet = true;
			GasMat.SetTexture(ShaderPropertyIDs.ToxGasTex, (Texture)(object)ContentFinder<Texture2D>.Get("Things/Gas/GasCloudThickA"));
			GasMat.SetTexture(ShaderPropertyIDs.RotGasTex, (Texture)(object)ContentFinder<Texture2D>.Get("Things/Gas/GasCloudThickA"));
			GasMat.SetTexture(ShaderPropertyIDs.DeadlifeDustTex, (Texture)(object)ContentFinder<Texture2D>.Get("Things/Gas/DeadlifeDust"));
		}
	}

	public override void DrawLayer()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible)
		{
			return;
		}
		propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecsPausable, RealTime.UnpausedRealTime);
		int count = subMeshes.Count;
		for (int i = 0; i < count; i++)
		{
			LayerSubMesh layerSubMesh = subMeshes[i];
			if (layerSubMesh.finalized && !layerSubMesh.disabled)
			{
				Graphics.DrawMesh(layerSubMesh.mesh, Vector3.zero, Quaternion.identity, layerSubMesh.material, 0, (Camera)null, 0, propertyBlock);
			}
		}
	}

	public virtual Color ColorAt(IntVec3 cell)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Color.op_Implicit(DensityAt(cell));
	}

	public virtual Vector4 DensityAt(IntVec3 cell)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return base.Map.gasGrid.DensitiesAt(cell);
	}

	public override void Regenerate()
	{
		ClearSubMeshes(MeshParts.All);
		LayerSubMesh subMesh = GetSubMesh(Mat);
		float altitude = AltitudeLayer.Gas.AltitudeFor();
		int num = section.botLeft.x;
		foreach (IntVec3 item in section.CellRect)
		{
			if (base.Map.gasGrid.AnyGasAt(item))
			{
				int count = subMesh.verts.Count;
				AddCell(item, num, count, subMesh, altitude);
			}
			num++;
		}
		if (subMesh.verts.Count > 0)
		{
			subMesh.FinalizeMesh(MeshParts.All);
		}
	}

	protected void AddCell(IntVec3 c, int index, int startVertIndex, LayerSubMesh sm, float altitude)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		Rand.PushState(index);
		Color val = ColorAt(c);
		float randomInRange = VertexScaleOffsetRange.RandomInRange;
		float randomInRange2 = VertexPositionOffsetRange.RandomInRange;
		float randomInRange3 = VertexPositionOffsetRange.RandomInRange;
		float num = (float)c.x - randomInRange + randomInRange2;
		float num2 = (float)(c.x + 1) + randomInRange + randomInRange2;
		float num3 = (float)c.z - randomInRange + randomInRange3;
		float num4 = (float)(c.z + 1) + randomInRange + randomInRange3;
		float num5 = altitude + Rand.Range(-0.01f, 0.01f);
		sm.verts.Add(new Vector3(num, num5, num3));
		sm.verts.Add(new Vector3(num, num5, num4));
		sm.verts.Add(new Vector3(num2, num5, num4));
		sm.verts.Add(new Vector3(num2, num5, num3));
		sm.uvs.Add(new Vector3(0f, 0f, (float)index));
		sm.uvs.Add(new Vector3(0f, 1f, (float)index));
		sm.uvs.Add(new Vector3(1f, 1f, (float)index));
		sm.uvs.Add(new Vector3(1f, 0f, (float)index));
		sm.colors.Add(Color32.op_Implicit(val));
		sm.colors.Add(Color32.op_Implicit(val));
		sm.colors.Add(Color32.op_Implicit(val));
		sm.colors.Add(Color32.op_Implicit(val));
		sm.tris.Add(startVertIndex);
		sm.tris.Add(startVertIndex + 1);
		sm.tris.Add(startVertIndex + 2);
		sm.tris.Add(startVertIndex);
		sm.tris.Add(startVertIndex + 2);
		sm.tris.Add(startVertIndex + 3);
		Rand.PopState();
	}
}
