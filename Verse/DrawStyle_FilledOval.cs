using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class DrawStyle_FilledOval : DrawStyle
{
	private const float RadiusOffset = 0.4f;

	public override bool CanHaveDuplicates => false;

	public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		CellRect cellRect = CellRect.FromLimits(origin, target);
		float radius = (float)cellRect.Width / 2f;
		float ratio = (float)cellRect.Width / (float)cellRect.Height;
		Vector3 val = cellRect.CenterCell.ToVector3();
		if (cellRect.Width % 2 == 0)
		{
			val.x -= 0.5f;
		}
		if (cellRect.Height % 2 == 0)
		{
			val.z -= 0.5f;
		}
		foreach (IntVec3 cell in cellRect.Cells)
		{
			Vector3 val2 = cell.ToVector3() - val;
			if (Filled(val2.x, val2.z, radius, ratio))
			{
				buffer.Add(cell);
			}
		}
	}

	protected float DistanceSqr(float x, float z, float ratio)
	{
		return Mathf.Pow(z * ratio, 2f) + Mathf.Pow(x, 2f);
	}

	protected bool Inside(float x, float z, float radius, float ratio)
	{
		return DistanceSqr(x, z, ratio) <= (radius + 0.4f) * (radius + 0.4f);
	}

	protected virtual bool Filled(float x, float z, float radius, float ratio)
	{
		return Inside(x, z, radius, ratio);
	}
}
