using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld.Planet;

public class WorldDrawLayer_Rivers : WorldDrawLayer_Paths
{
	private readonly Color32 riverColor = new Color32((byte)73, (byte)82, (byte)100, byte.MaxValue);

	private const float PerlinFrequency = 0.6f;

	private const float PerlinMagnitude = 0.1f;

	private readonly ModuleBase riverDisplacementX = new Perlin(0.6000000238418579, 2.0, 0.5, 3, 84905524, QualityMode.Medium);

	private readonly ModuleBase riverDisplacementY = new Perlin(0.6000000238418579, 2.0, 0.5, 3, 37971116, QualityMode.Medium);

	private readonly ModuleBase riverDisplacementZ = new Perlin(0.6000000238418579, 2.0, 0.5, 3, 91572032, QualityMode.Medium);

	public override bool VisibleWhenLayerNotSelected => false;

	public override bool VisibleInBackground => false;

	public WorldDrawLayer_Rivers()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		pointyEnds = true;
	}

	public override IEnumerable Regenerate()
	{
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
		LayerSubMesh subMesh = GetSubMesh(WorldMaterials.Rivers);
		LayerSubMesh subMeshBorder = GetSubMesh(WorldMaterials.RiversBorder);
		List<OutputDirection> outputs = new List<OutputDirection>();
		List<OutputDirection> outputsBorder = new List<OutputDirection>();
		int i = 0;
		while (i < planetLayer.TilesCount)
		{
			if (i % 1000 == 0)
			{
				yield return null;
			}
			if (subMesh.verts.Count > 60000)
			{
				subMesh = GetSubMesh(WorldMaterials.Rivers);
				subMeshBorder = GetSubMesh(WorldMaterials.RiversBorder);
			}
			SurfaceTile surfaceTile = (SurfaceTile)planetLayer[i];
			if (surfaceTile.potentialRivers != null)
			{
				outputs.Clear();
				outputsBorder.Clear();
				for (int j = 0; j < surfaceTile.potentialRivers.Count; j++)
				{
					outputs.Add(new OutputDirection
					{
						neighbor = surfaceTile.potentialRivers[j].neighbor,
						width = surfaceTile.potentialRivers[j].river.widthOnWorld - 0.2f
					});
					outputsBorder.Add(new OutputDirection
					{
						neighbor = surfaceTile.potentialRivers[j].neighbor,
						width = surfaceTile.potentialRivers[j].river.widthOnWorld
					});
				}
				GeneratePaths(subMesh, surfaceTile.tile, outputs, riverColor, allowSmoothTransition: true);
				GeneratePaths(subMeshBorder, surfaceTile.tile, outputsBorder, riverColor, allowSmoothTransition: true);
			}
			int num = i + 1;
			i = num;
		}
		FinalizeMesh(MeshParts.All);
	}

	public override Vector3 FinalizePoint(Vector3 inp, float distortionFrequency, float distortionIntensity)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float magnitude = ((Vector3)(ref inp)).magnitude;
		Vector3 val = inp + new Vector3(riverDisplacementX.GetValue(inp), riverDisplacementY.GetValue(inp), riverDisplacementZ.GetValue(inp)) * 0.1f;
		inp = ((Vector3)(ref val)).normalized * magnitude;
		return inp + ((Vector3)(ref inp)).normalized * 0.01f;
	}
}
