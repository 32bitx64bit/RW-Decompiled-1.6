using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class WorldRendererUtility
{
	private const float WorldIntersectionFactor = 1f;

	public static WorldRenderMode CurrentWorldRenderMode
	{
		get
		{
			if (Find.World == null)
			{
				return WorldRenderMode.None;
			}
			if (WorldComponent_GravshipController.CutsceneInProgress)
			{
				if (Find.CurrentMap != null && Find.CurrentMap.generatorDef.renderWorld)
				{
					return WorldRenderMode.Background;
				}
				return WorldRenderMode.None;
			}
			if (Current.ProgramState == ProgramState.Playing && Find.CurrentMap == null)
			{
				return WorldRenderMode.Planet;
			}
			if (Find.World.renderer.wantedMode == WorldRenderMode.Planet)
			{
				return WorldRenderMode.Planet;
			}
			if (Find.CurrentMap != null && Find.CurrentMap.generatorDef.renderWorld)
			{
				return WorldRenderMode.Background;
			}
			return Find.World.renderer.wantedMode;
		}
	}

	public static bool WorldRendered => CurrentWorldRenderMode != WorldRenderMode.None;

	public static bool WorldBackgroundNow => CurrentWorldRenderMode == WorldRenderMode.Background;

	public static bool WorldSelected => CurrentWorldRenderMode == WorldRenderMode.Planet;

	public static bool DrawingMap => CurrentWorldRenderMode != WorldRenderMode.Planet;

	public static void UpdateGlobalShadersParams()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = -GenCelestial.CurSunPositionInWorldSpace();
		float num = ((WorldBackgroundNow || !PlanetLayer.Selected.IsRootSurface || Find.PlaySettings.usePlanetDayNightSystem) ? 1f : 0f);
		Shader.SetGlobalVector(ShaderPropertyIDs.PlanetSunLightDirection, Vector4.op_Implicit(val));
		Shader.SetGlobalFloat(ShaderPropertyIDs.PlanetSunLightEnabled, num);
		Shader.SetGlobalFloat(ShaderPropertyIDs.BackgroundModeEnabled, WorldBackgroundNow ? 1f : 0f);
	}

	public static void PrintQuadTangentialToPlanet(Vector3 pos, float size, float altOffset, LayerSubMesh subMesh, bool counterClockwise = false, float rotation = 0f, bool printUVs = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PrintQuadTangentialToPlanet(pos, pos, size, altOffset, subMesh, counterClockwise, rotation, printUVs);
	}

	public static void PrintQuadTangentialToPlanet(Vector3 pos, Vector3 posForTangents, float size, float altOffset, LayerSubMesh subMesh, bool counterClockwise = false, float rotation = 0f, bool printUVs = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		GetTangentsToPlanet(posForTangents, out var first, out var second, rotation);
		Vector3 normalized = ((Vector3)(ref posForTangents)).normalized;
		float num = size * 0.5f;
		Vector3 item = pos - first * num - second * num + normalized * altOffset;
		Vector3 item2 = pos - first * num + second * num + normalized * altOffset;
		Vector3 item3 = pos + first * num + second * num + normalized * altOffset;
		Vector3 item4 = pos + first * num - second * num + normalized * altOffset;
		int count = subMesh.verts.Count;
		subMesh.verts.Add(item);
		subMesh.verts.Add(item2);
		subMesh.verts.Add(item3);
		subMesh.verts.Add(item4);
		if (printUVs)
		{
			subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 0f)));
			subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 1f)));
			subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 1f)));
			subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 0f)));
		}
		if (counterClockwise)
		{
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count + 1);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 3);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count);
		}
		else
		{
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 1);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count);
			subMesh.tris.Add(count + 2);
			subMesh.tris.Add(count + 3);
		}
	}

	public static void DrawQuadTangentialToPlanet(Vector3 pos, float size, float altOffset, Material material, float rotationAngle = 0f, bool counterClockwise = false, bool useSkyboxLayer = false, MaterialPropertyBlock propertyBlock = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)material == (Object)null)
		{
			Log.Warning("Tried to draw quad with null material.");
			return;
		}
		Vector3 normalized = ((Vector3)(ref pos)).normalized;
		Vector3 val = ((!counterClockwise) ? normalized : (-normalized));
		Quaternion val2 = Quaternion.LookRotation(Vector3.Cross(val, Vector3.up), val);
		Quaternion val3 = Quaternion.AngleAxis(rotationAngle, normalized) * val2;
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector(size, 1f, size);
		Matrix4x4 val5 = Matrix4x4.TRS(pos + normalized * altOffset, val3, val4);
		int num = (useSkyboxLayer ? WorldCameraManager.WorldSkyboxLayer : WorldCameraManager.WorldLayer);
		if (propertyBlock != null)
		{
			Graphics.DrawMesh(MeshPool.plane10, val5, material, num, (Camera)null, 0, propertyBlock);
		}
		else
		{
			Graphics.DrawMesh(MeshPool.plane10, val5, material, num);
		}
	}

	public static void GetTangentsToPlanet(Vector3 pos, out Vector3 first, out Vector3 second, float rotation = 0f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = ((Vector3)(ref pos)).normalized;
		Vector3 val = Vector3.up;
		if (Mathf.Abs(Vector3.Dot(normalized, Vector3.up)) > 0.999f)
		{
			val = Vector3.right;
		}
		Quaternion val2 = Quaternion.LookRotation(normalized, val);
		Quaternion val3 = Quaternion.AngleAxis(rotation, normalized) * val2;
		first = val3 * Vector3.up;
		second = val3 * Vector3.right;
	}

	public static Vector3 ProjectOnQuadTangentialToPlanet(Vector3 center, Vector2 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		GetTangentsToPlanet(center, out var first, out var second);
		return point.x * first + point.y * second;
	}

	public static void GetTangentialVectorFacing(Vector3 root, Vector3 pointToFace, out Vector3 forward, out Vector3 right)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(root, pointToFace);
		forward = val * Vector3.up;
		right = val * Vector3.left;
	}

	public static void PrintTextureAtlasUVs(int indexX, int indexY, int numX, int numY, LayerSubMesh subMesh)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / (float)numX;
		float num2 = 1f / (float)numY;
		float num3 = (float)indexX * num;
		float num4 = (float)indexY * num2;
		subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(num3, num4)));
		subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(num3, num4 + num2)));
		subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(num3 + num, num4 + num2)));
		subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(num3 + num, num4)));
	}

	public static bool HiddenBehindTerrainNow(Vector3 pos)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<int, PlanetLayer> dictionary = (Dictionary<int, PlanetLayer>)Find.WorldGrid.PlanetLayers;
		foreach (int key in dictionary.Keys)
		{
			PlanetLayer planetLayer = dictionary[key];
			if (planetLayer.Visible && planetLayer.Def.obstructsExpandingIcons && planetLayer.LineIntersects(Find.WorldCameraDriver.CameraPosition, pos))
			{
				return true;
			}
		}
		return false;
	}
}
