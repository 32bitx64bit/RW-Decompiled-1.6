using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Verse.Glow;

public struct GlowCellComparer : IComparer<IntVec3>
{
	private UnsafeList<GlowCell> area;

	private GlowLight light;

	public GlowCellComparer(UnsafeList<GlowCell> area, in GlowLight light)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.area = area;
		this.light = light;
	}

	public int Compare(IntVec3 a, IntVec3 b)
	{
		int num = light.DeltaToLocalIndex(in a);
		int num2 = light.DeltaToLocalIndex(in b);
		return area[num].intDist.CompareTo(area[num2].intDist);
	}
}
