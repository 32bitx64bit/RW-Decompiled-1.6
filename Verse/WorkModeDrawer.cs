using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse;

public class WorkModeDrawer
{
	private const float MouseoverLineWidth = 0.1f;

	private const float CircleOutlineRadius = 0.5f;

	private static readonly Vector3 IconScale = Vector3.one * 0.5f;

	public MechWorkModeDef def;

	private Material iconMat;

	protected virtual bool DrawIconAtTarget => true;

	public virtual void DrawControlGroupMouseOverExtra(MechanitorControlGroup group)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		GlobalTargetInfo targetForLine = GetTargetForLine(group);
		List<Pawn> mechsForReading = group.MechsForReading;
		Map currentMap = Find.CurrentMap;
		if (!targetForLine.IsValid || targetForLine.Map != currentMap)
		{
			return;
		}
		Vector3 val = targetForLine.Cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MoteOverhead);
		for (int i = 0; i < mechsForReading.Count; i++)
		{
			if (mechsForReading[i].Map == currentMap)
			{
				GenDraw.DrawLineBetween(val, mechsForReading[i].DrawPos, SimpleColor.White, 0.1f);
				GenDraw.DrawCircleOutline(mechsForReading[i].DrawPos, 0.5f);
			}
		}
		if (DrawIconAtTarget)
		{
			if ((Object)(object)iconMat == (Object)null)
			{
				iconMat = MaterialPool.MatFrom(def.uiIcon);
			}
			Matrix4x4 val2 = Matrix4x4.TRS(val, Quaternion.identity, IconScale);
			Graphics.DrawMesh(MeshPool.plane14, val2, iconMat, 0);
		}
	}

	public virtual GlobalTargetInfo GetTargetForLine(MechanitorControlGroup group)
	{
		return group.Target;
	}
}
