using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class MapDrawLayer_OrbitalDebris : MapDrawLayer
{
	private readonly Dictionary<LayerSubMesh, float> parallaxPer10CellLookup;

	private readonly Dictionary<OrbitalDebrisDef.DebrisGraphic, float> offsets;

	private static readonly Vector2[] UVs = (Vector2[])(object)new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f),
		new Vector2(1f, 0f)
	};

	private static readonly FloatRange GroupsPer100SqrTiles = new FloatRange(2f, 2.5f);

	private static readonly FloatRange SizeFactorRange = new FloatRange(0.75f, 1.25f);

	private static readonly IntRange PiecesPerCluster = new IntRange(5, 20);

	private static readonly IntRange ClusterSize = new IntRange(20, 40);

	private static readonly SimpleCurve DistanceFromCenterWeightingCurve = new SimpleCurve
	{
		new CurvePoint(0f, 0f),
		new CurvePoint(0.1f, 0.2f),
		new CurvePoint(0.4f, 0.6f),
		new CurvePoint(0.6f, 0.4f),
		new CurvePoint(0.9f, 0.2f),
		new CurvePoint(1f, 0f)
	};

	private static readonly List<Vector3> usedPoints = new List<Vector3>();

	public override bool Visible
	{
		get
		{
			if (ModsConfig.OdysseyActive)
			{
				return base.Map.OrbitalDebris != null;
			}
			return false;
		}
	}

	public MapDrawLayer_OrbitalDebris(Map map)
		: base(map)
	{
		if (base.Map.OrbitalDebris != null)
		{
			parallaxPer10CellLookup = new Dictionary<LayerSubMesh, float>();
			offsets = new Dictionary<OrbitalDebrisDef.DebrisGraphic, float>();
			CacheData();
		}
	}

	private void CacheData()
	{
		parallaxPer10CellLookup.Clear();
		offsets.Clear();
		List<OrbitalDebrisDef.DebrisGraphic> list = base.Map.OrbitalDebris.graphics.OrderBy((OrbitalDebrisDef.DebrisGraphic debris) => debris.order).ToList();
		float num = 0.03658537f;
		for (int i = 0; i < list.Count; i++)
		{
			offsets[list[i]] = num;
			num += 0.03658537f;
		}
	}

	public override void DrawLayer()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible)
		{
			return;
		}
		Vector3 val = ((Component)Find.Camera).transform.position.Yto0() - base.Map.Center.ToVector3().Yto0();
		for (int i = 0; i < subMeshes.Count; i++)
		{
			LayerSubMesh layerSubMesh = subMeshes[i];
			if (layerSubMesh.finalized && !layerSubMesh.disabled)
			{
				Matrix4x4 val2 = Matrix4x4.Translate(val / 10f * parallaxPer10CellLookup[layerSubMesh]);
				Graphics.DrawMesh(layerSubMesh.mesh, val2, layerSubMesh.material, layerSubMesh.renderLayer);
			}
		}
	}

	public override void Regenerate()
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckOdyssey("Orbital Debris"))
		{
			return;
		}
		ClearSubMeshes(MeshParts.All);
		parallaxPer10CellLookup.Clear();
		OrbitalDebrisDef orbitalDebris = base.Map.OrbitalDebris;
		Rand.PushState(base.Map.uniqueID);
		CellRect cellRect = base.Map.BoundsRect().ExpandedBy(30);
		int num = Mathf.RoundToInt((float)(cellRect.Size.x * cellRect.Size.z) / 10000f * GroupsPer100SqrTiles.RandomInRange);
		for (int i = 0; i < num; i++)
		{
			if (!TryGetGroupCenter(out var position))
			{
				continue;
			}
			int randomInRange = PiecesPerCluster.RandomInRange;
			for (int j = 0; j < randomInRange; j++)
			{
				OrbitalDebrisDef.DebrisGraphic graphic = orbitalDebris.graphics.RandomElementByWeight((OrbitalDebrisDef.DebrisGraphic w) => w.weighting);
				Vector3 vect = position + Vector3Utility.RotatedBy(new Vector3((float)ClusterSize.RandomInRange, 0f, (float)ClusterSize.RandomInRange), Rand.Range(0f, 360f));
				AddQuad(graphic, vect.ToIntVec3());
			}
			usedPoints.Add(position);
		}
		Rand.PopState();
		FinalizeMesh(MeshParts.All);
		usedPoints.Clear();
	}

	private bool TryGetGroupCenter(out Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		position = Vector3.zero;
		Vector3 val = base.Map.Center.ToVector3();
		for (int i = 0; i < 20; i++)
		{
			float num = Rand.ByCurve(DistanceFromCenterWeightingCurve) * (float)base.Map.Size.x / 2f;
			int num2 = Rand.Range(0, 360);
			bool flag = true;
			for (int j = 0; j < 360; j += 20)
			{
				position = val + Vector3Utility.RotatedBy(new Vector3(num, 0f, 0f), (j + num2) % 360);
				foreach (Vector3 usedPoint in usedPoints)
				{
					if (Vector3.Distance(usedPoint, position) <= (float)(ClusterSize.max * 2))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AddQuad(OrbitalDebrisDef.DebrisGraphic graphic, IntVec3 c)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		if (!offsets.ContainsKey(graphic))
		{
			CacheData();
		}
		Material matSingle = graphic.graphicData.Graphic.MatSingle;
		LayerSubMesh subMesh = GetSubMesh(matSingle);
		subMesh.Clear(MeshParts.Verts | MeshParts.Tris | MeshParts.UVs1 | MeshParts.Normals);
		parallaxPer10CellLookup[subMesh] = graphic.parallaxPer10Cells;
		int count = subMesh.verts.Count;
		float num = Rand.RangeSeeded(0f, 0.03658537f, base.Map.cellIndices[c]);
		int num2 = Rand.Range(0, 360);
		Vector3 val = default(Vector3);
		for (int i = 0; i < 4; i++)
		{
			float randomInRange = SizeFactorRange.RandomInRange;
			((Vector3)(ref val))._002Ector((UVs[i].x - 0.5f) * graphic.graphicData.drawSize.x * randomInRange, 0f, (UVs[i].y - 0.5f) * graphic.graphicData.drawSize.y * randomInRange);
			val = val.RotatedBy(num2);
			val += new Vector3((float)c.x, AltitudeLayer.BelowTerrain.AltitudeFor() - offsets[graphic] + num, (float)c.z);
			subMesh.verts.Add(val);
			subMesh.normals.Add(Vector3.up);
			subMesh.uvs.Add(Vector2.op_Implicit(UVs[i]));
		}
		subMesh.tris.Add(count);
		subMesh.tris.Add(count + 1);
		subMesh.tris.Add(count + 2);
		subMesh.tris.Add(count);
		subMesh.tris.Add(count + 2);
		subMesh.tris.Add(count + 3);
	}
}
