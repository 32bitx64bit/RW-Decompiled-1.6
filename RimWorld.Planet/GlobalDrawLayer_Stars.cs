using System.Collections;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class GlobalDrawLayer_Stars : WorldDrawLayerBase
{
	private bool calculatedForStaticRotation;

	private PlanetTile calculatedForStartingTile = PlanetTile.Invalid;

	public const float DistanceToStars = 20f;

	private static readonly FloatRange StarsDrawSize = new FloatRange(1f, 3.8f);

	private const int StarsCount = 1500;

	private const float DistToSunToReduceStarSize = 0.8f;

	protected override int RenderLayer => WorldCameraManager.WorldSkyboxLayer;

	public override bool ShouldRegenerate
	{
		get
		{
			if (!base.ShouldRegenerate && (Find.GameInitData == null || !(Find.GameInitData.startingTile != calculatedForStartingTile)))
			{
				return UseStaticRotation != calculatedForStaticRotation;
			}
			return true;
		}
	}

	private bool UseStaticRotation => Current.ProgramState == ProgramState.Entry;

	protected override Quaternion Rotation
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (UseStaticRotation)
			{
				return Quaternion.identity;
			}
			return Quaternion.LookRotation(GenCelestial.CurSunPositionInWorldSpace());
		}
	}

	public override IEnumerable Regenerate()
	{
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
		Rand.PushState();
		Rand.Seed = Find.World.info.Seed;
		for (int i = 0; i < 1500; i++)
		{
			Vector3 unitVector = Rand.UnitVector3;
			Vector3 pos = unitVector * 20f;
			LayerSubMesh subMesh = GetSubMesh(WorldMaterials.Stars);
			float num = StarsDrawSize.RandomInRange;
			Vector3 val;
			if (!UseStaticRotation)
			{
				val = Vector3.forward;
			}
			else
			{
				Vector3 val2 = GenCelestial.CurSunPositionInWorldSpace();
				val = ((Vector3)(ref val2)).normalized;
			}
			Vector3 val3 = val;
			float num2 = Vector3.Dot(unitVector, val3);
			if (num2 > 0.8f)
			{
				num *= GenMath.LerpDouble(0.8f, 1f, 1f, 0.35f, num2);
			}
			WorldRendererUtility.PrintQuadTangentialToPlanet(pos, num, 0f, subMesh, counterClockwise: true, Rand.Range(0f, 360f));
		}
		calculatedForStartingTile = ((Find.GameInitData != null) ? Find.GameInitData.startingTile : PlanetTile.Invalid);
		calculatedForStaticRotation = UseStaticRotation;
		Rand.PopState();
		FinalizeMesh(MeshParts.All);
	}
}
