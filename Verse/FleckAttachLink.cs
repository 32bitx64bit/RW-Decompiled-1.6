using UnityEngine;

namespace Verse;

public struct FleckAttachLink
{
	private TargetInfo targetInt;

	private Vector3 lastDrawPosInt;

	public int detachAfterTicks;

	public static readonly FleckAttachLink Invalid = new FleckAttachLink(TargetInfo.Invalid);

	public bool Linked => targetInt.IsValid;

	public TargetInfo Target => targetInt;

	public Vector3 LastDrawPos => lastDrawPosInt;

	public FleckAttachLink(TargetInfo target)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		targetInt = target;
		detachAfterTicks = -1;
		lastDrawPosInt = Vector3.zero;
		if (target.IsValid)
		{
			UpdateDrawPos();
		}
	}

	public void UpdateDrawPos()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (targetInt.HasThing)
		{
			lastDrawPosInt = (Vector3)(((_003F?)targetInt.Thing.DrawPosHeld) ?? lastDrawPosInt);
		}
		else
		{
			lastDrawPosInt = targetInt.Cell.ToVector3Shifted();
		}
	}
}
