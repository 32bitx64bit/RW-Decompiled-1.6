using System.Collections.Generic;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class MapDrawLayer_ExteriorLightingOverlay : MapDrawLayer
{
	private static readonly int RenderLayer = LayerMask.NameToLayer("GravshipExclude");

	private const int Size = 200;

	public override bool Visible
	{
		get
		{
			if (DebugViewSettings.drawLightingOverlay)
			{
				return !base.Map.DrawMapClippers;
			}
			return false;
		}
	}

	public MapDrawLayer_ExteriorLightingOverlay(Map map)
		: base(map)
	{
	}

	public override void Regenerate()
	{
		LayerSubMesh subMesh = GetSubMesh(MatBases.LightOverlay);
		LayerSubMesh subMesh2 = GetSubMesh(MatBases.ShadowMask);
		subMesh.renderLayer = RenderLayer;
		subMesh2.renderLayer = RenderLayer;
		MakeBaseGeometry(base.Map, subMesh);
		MakeBaseGeometry(base.Map, subMesh2);
	}

	private static IEnumerable<Rect> GetClipperRects(Map map)
	{
		yield return new Rect(-200f, 0f, 200f, (float)map.Size.z);
		yield return new Rect((float)map.Size.x, 0f, 200f, (float)map.Size.z);
		yield return new Rect(-200f, (float)map.Size.z, (float)(map.Size.x + 400), 200f);
		yield return new Rect(-200f, -200f, (float)(map.Size.x + 400), 200f);
	}

	private static void MakeBaseGeometry(Map map, LayerSubMesh sm)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		float num = AltitudeLayer.LightingOverlay.AltitudeFor();
		sm.Clear(MeshParts.Verts | MeshParts.Tris | MeshParts.Colors);
		foreach (Rect clipperRect in GetClipperRects(map))
		{
			Rect current = clipperRect;
			sm.verts.Add(new Vector3(((Rect)(ref current)).x, num, ((Rect)(ref current)).y));
			sm.verts.Add(new Vector3(((Rect)(ref current)).x, num, ((Rect)(ref current)).yMax));
			sm.verts.Add(new Vector3(((Rect)(ref current)).xMax, num, ((Rect)(ref current)).yMax));
			sm.verts.Add(new Vector3(((Rect)(ref current)).xMax, num, ((Rect)(ref current)).y));
			sm.colors.Add(new Color32((byte)0, (byte)0, (byte)0, (byte)0));
			sm.colors.Add(new Color32((byte)0, (byte)0, (byte)0, (byte)0));
			sm.colors.Add(new Color32((byte)0, (byte)0, (byte)0, (byte)0));
			sm.colors.Add(new Color32((byte)0, (byte)0, (byte)0, (byte)0));
			sm.tris.Add(sm.verts.Count - 4);
			sm.tris.Add(sm.verts.Count - 3);
			sm.tris.Add(sm.verts.Count - 2);
			sm.tris.Add(sm.verts.Count - 4);
			sm.tris.Add(sm.verts.Count - 2);
			sm.tris.Add(sm.verts.Count - 1);
		}
		sm.FinalizeMesh(MeshParts.Verts | MeshParts.Tris | MeshParts.Colors);
	}
}
