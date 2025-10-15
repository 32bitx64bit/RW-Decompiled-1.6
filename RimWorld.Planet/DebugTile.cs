using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class DebugTile
{
	public PlanetTile tile;

	public string displayString;

	public float colorPct;

	public int ticksLeft;

	public Material customMat;

	private Mesh mesh;

	private static List<Vector3> tmpVerts = new List<Vector3>();

	private static List<int> tmpIndices = new List<int>();

	private Vector2 ScreenPos => GenWorldUI.WorldToUIPosition(Find.WorldGrid.GetTileCenter(tile));

	private bool VisibleForCamera
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Rect val = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
			return ((Rect)(ref val)).Contains(ScreenPos);
		}
	}

	public float DistanceToCamera
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tile);
			return Vector3.Distance(((Component)Find.WorldCamera).transform.position, tileCenter);
		}
	}

	public void Draw()
	{
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		if (!VisibleForCamera)
		{
			return;
		}
		if ((Object)(object)mesh == (Object)null)
		{
			Find.WorldGrid.GetTileVertices(tile, tmpVerts);
			for (int i = 0; i < tmpVerts.Count; i++)
			{
				Vector3 val = tmpVerts[i];
				tmpVerts[i] = val + ((Vector3)(ref val)).normalized * 0.02f;
			}
			mesh = new Mesh();
			((Object)mesh).name = "DebugTile";
			mesh.SetVertices(tmpVerts);
			tmpIndices.Clear();
			for (int j = 0; j < tmpVerts.Count - 2; j++)
			{
				tmpIndices.Add(j + 2);
				tmpIndices.Add(j + 1);
				tmpIndices.Add(0);
			}
			mesh.SetTriangles(tmpIndices, 0);
		}
		Material val2 = ((!((Object)(object)customMat != (Object)null)) ? WorldDebugMatsSpectrum.Mat(Mathf.RoundToInt(colorPct * 100f) % 100) : customMat);
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, val2, WorldCameraManager.WorldLayer);
	}

	public void OnGUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (VisibleForCamera)
		{
			Vector2 screenPos = ScreenPos;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(screenPos.x - 20f, screenPos.y - 20f, 40f, 40f);
			if (displayString != null)
			{
				Widgets.Label(rect, displayString);
			}
		}
	}
}
