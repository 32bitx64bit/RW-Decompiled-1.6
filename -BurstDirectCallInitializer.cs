using RimWorld;
using RimWorld.Planet;
using UnityEngine;

internal static class _0024BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
	private static void Initialize()
	{
		MapGenUtility.ComputeLargestRects_0000B6FE_0024BurstDirectCall.Initialize();
		MapGenUtility.RectsComputeSpaces_0000B6FF_0024BurstDirectCall.Initialize();
		FastTileFinder.Initialize_0024ComputeQueryJob_SphericalDistance_00014ED2_0024BurstDirectCall();
		PlanetLayer.CalculateAverageTileSize_00015393_0024BurstDirectCall.Initialize();
		PlanetLayer.IntGetTileSize_00015395_0024BurstDirectCall.Initialize();
		PlanetLayer.IntGetTileCenter_00015398_0024BurstDirectCall.Initialize();
	}
}
