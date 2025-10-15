using UnityEngine;

namespace Verse;

public class Verb_ArcSpray : Verb_Spray
{
	protected override void PreparePath()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		path.Clear();
		Vector3 val = (currentTarget.CenterVector3 - caster.Position.ToVector3Shifted()).Yto0();
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 tan = normalized.RotatedBy(90f);
		for (int i = 0; i < verbProps.sprayNumExtraCells; i++)
		{
			for (int j = 0; j < 15; j++)
			{
				float value = Rand.Value;
				float num = Rand.Value - 0.5f;
				float num2 = value * verbProps.sprayWidth * 2f - verbProps.sprayWidth;
				float num3 = num * (float)verbProps.sprayThicknessCells + num * 2f * verbProps.sprayArching;
				IntVec3 item = (currentTarget.CenterVector3 + num2 * tan - num3 * normalized).ToIntVec3();
				if (!path.Contains(item) || Rand.Value < 0.25f)
				{
					path.Add(item);
					break;
				}
			}
		}
		path.Add(currentTarget.Cell);
		path.SortBy(delegate(IntVec3 c)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val2 = (c.ToVector3Shifted() - caster.DrawPos).Yto0();
			return ((Vector3)(ref val2)).normalized.AngleToFlat(tan);
		});
	}
}
