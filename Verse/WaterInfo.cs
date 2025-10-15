using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RimWorld;
using UnityEngine;

namespace Verse;

public class WaterInfo : MapComponent
{
	public Texture2D riverFlowTexture;

	public IntVec3 lakeCenter = IntVec3.Invalid;

	public List<RiverNode> riverGraph = new List<RiverNode>();

	public List<float> riverFlowMap;

	[Obsolete]
	public CellRect riverFlowMapBounds;

	private Color[] flowMapPixels;

	private Texture2D skyTex;

	private const string ReflectionTexture = "Other/WaterReflection";

	public WaterInfo(Map map)
		: base(map)
	{
	}

	public override void MapRemoved()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			Object.Destroy((Object)(object)riverFlowTexture);
		});
	}

	public void SetTextures()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		Camera subcamera = Current.SubcameraDriver.GetSubcamera(SubcameraDefOf.WaterDepth);
		if ((Object)(object)subcamera != (Object)null)
		{
			Shader.SetGlobalTexture(ShaderPropertyIDs.WaterOutputTex, (Texture)(object)subcamera.targetTexture);
		}
		if ((Object)(object)skyTex == (Object)null)
		{
			skyTex = ContentFinder<Texture2D>.Get("Other/WaterReflection");
		}
		Shader.SetGlobalTexture(ShaderPropertyIDs.WaterReflectionTex, (Texture)(object)skyTex);
		Camera camera = Find.Camera;
		Vector4 val = (Object.op_Implicit((Object)(object)camera.targetTexture) ? new Vector4((float)((Texture)camera.targetTexture).width, (float)((Texture)camera.targetTexture).height, 1f / (float)((Texture)camera.targetTexture).width, 1f / (float)((Texture)camera.targetTexture).height) : new Vector4((float)Screen.width, (float)Screen.height, 1f / (float)Screen.width, 1f / (float)Screen.height));
		Shader.SetGlobalVector(ShaderPropertyIDs.MainCameraScreenParams, val);
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		Matrix4x4 val2 = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * worldToCameraMatrix;
		Shader.SetGlobalMatrix(ShaderPropertyIDs.MainCameraVP, val2);
		if ((Object)(object)riverFlowTexture == (Object)null && riverFlowMap != null && riverFlowMap.Count > 0)
		{
			flowMapPixels = (Color[])(object)new Color[riverFlowMap.Count / 2];
			Vector2 val3 = default(Vector2);
			for (int i = 0; i < map.Size.x; i++)
			{
				for (int j = 0; j < map.Size.z; j++)
				{
					IntVec3 intVec = new IntVec3(i, 0, j);
					if (!map.terrainGrid.BaseTerrainAt(intVec).IsRiver)
					{
						continue;
					}
					int num = intVec.x * map.Size.z + intVec.z;
					((Vector2)(ref val3))._002Ector(riverFlowMap[num * 2], riverFlowMap[num * 2 + 1]);
					int num2 = 1;
					foreach (IntVec3 item in GenAdjFast.AdjacentCells8Way(intVec))
					{
						if (item.InBounds(map) && map.terrainGrid.BaseTerrainAt(intVec).IsRiver)
						{
							int num3 = item.x * map.Size.z + item.z;
							val3 += new Vector2(riverFlowMap[num3 * 2], riverFlowMap[num3 * 2 + 1]);
							num2++;
						}
					}
					flowMapPixels[j * map.Size.x + i] = new Color(val3.x / (float)num2, val3.y / (float)num2, 0f);
				}
			}
			riverFlowTexture = new Texture2D(map.Size.x, map.Size.z, (TextureFormat)19, false);
			riverFlowTexture.SetPixels(flowMapPixels);
			((Texture)riverFlowTexture).wrapMode = (TextureWrapMode)1;
			riverFlowTexture.Apply();
		}
		Shader.SetGlobalTexture(ShaderPropertyIDs.WaterOffsetTex, (Texture)(object)riverFlowTexture);
		Shader.SetGlobalVector(ShaderPropertyIDs.MapSize, new Vector4((float)map.Size.x, (float)map.Size.z));
	}

	public Vector3 GetWaterMovement(Vector3 position)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (riverFlowMap == null)
		{
			return Vector3.zero;
		}
		IntVec3 intVec = position.ToIntVec3();
		int num = intVec.x * map.Size.z + intVec.z;
		return new Vector3(riverFlowMap[num * 2], 0f, riverFlowMap[num * 2 + 1]);
	}

	public override void ExposeData()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.ExposeData();
		if (Scribe.mode == LoadSaveMode.Saving)
		{
			string value = null;
			if (riverFlowMap != null)
			{
				value = Convert.ToBase64String(CompressUtility.Compress(Span<byte>.op_Implicit(MemoryMarshal.AsBytes<float>(riverFlowMap.AsSpan())))).AddLineBreaksToLongString();
			}
			Scribe_Values.Look(ref value, "riverFlowMapDeflate");
		}
		else if (Scribe.EnterNode("riverFlowMapDeflate"))
		{
			Scribe.ExitNode();
			string value2 = null;
			Scribe_Values.Look(ref value2, "riverFlowMapDeflate");
			if (value2 != null)
			{
				Span<float> val = MemoryMarshal.Cast<byte, float>(Span<byte>.op_Implicit(CompressUtility.Decompress(Convert.FromBase64String(value2))));
				riverFlowMap = new List<float>(val.Length);
				Span<float> val2 = riverFlowMap.AsSpan(val.Length);
				val.CopyTo(val2);
				riverFlowMap.UnsafeSetCount(val.Length);
			}
		}
	}

	public void DebugDrawRiver()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		if (riverGraph.NullOrEmpty())
		{
			return;
		}
		if (DebugViewSettings.drawRiverDebug)
		{
			foreach (RiverNode item in riverGraph)
			{
				GenDraw.DrawLineBetween(item.start, item.end, SimpleColor.Green, 5f);
				GenDraw.DrawArrowRotated(Vector3.Lerp(item.start, item.end, 0.5f), item.start.AngleToFlat(item.end) - 270f, ghost: false);
			}
		}
		if (!DebugViewSettings.drawRiverFlowDebug)
		{
			return;
		}
		Vector3 val = default(Vector3);
		foreach (IntVec3 allCell in map.AllCells)
		{
			int num = allCell.x * map.Size.z + allCell.z;
			((Vector3)(ref val))._002Ector(riverFlowMap[num * 2], 0f, riverFlowMap[num * 2 + 1]);
			if (!(val == Vector3.zero))
			{
				GenDraw.DrawArrowRotated(allCell.ToVector3Shifted(), val.AngleFlat(), ghost: false);
			}
		}
	}
}
