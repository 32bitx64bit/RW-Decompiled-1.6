using UnityEngine;

namespace Verse;

public class MoteDualAttached : Mote
{
	protected MoteAttachLink link2 = MoteAttachLink.Invalid;

	public void Attach(TargetInfo a, TargetInfo b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		link1 = new MoteAttachLink(a, Vector3.zero);
		link2 = new MoteAttachLink(b, Vector3.zero);
	}

	public void Attach(TargetInfo a, TargetInfo b, Vector3 offsetA, Vector3 offsetB)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		link1 = new MoteAttachLink(a, offsetA);
		link2 = new MoteAttachLink(b, offsetB);
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdatePositionAndRotation();
		base.DrawAt(drawLoc, flip);
	}

	public void UpdateTargets(TargetInfo a, TargetInfo b, Vector3 offsetA, Vector3 offsetB)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		link1.UpdateTarget(a, offsetA);
		link2.UpdateTarget(b, offsetB);
	}

	protected void UpdatePositionAndRotation()
	{
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (link1.Linked)
		{
			if (link2.Linked)
			{
				if (!link1.Target.ThingDestroyed)
				{
					link1.UpdateDrawPos();
				}
				if (!link2.Target.ThingDestroyed)
				{
					link2.UpdateDrawPos();
				}
				exactPosition = (link1.LastDrawPos + link2.LastDrawPos) * 0.5f;
				if (def.mote.rotateTowardsTarget)
				{
					exactRotation = link1.LastDrawPos.AngleToFlat(link2.LastDrawPos) + 90f;
				}
				if (def.mote.scaleToConnectTargets)
				{
					linearScale = new Vector3(def.graphicData.drawSize.y, 1f, (link2.LastDrawPos - link1.LastDrawPos).MagnitudeHorizontal());
				}
			}
			else
			{
				if (!link1.Target.ThingDestroyed)
				{
					link1.UpdateDrawPos();
				}
				exactPosition = link1.LastDrawPos + def.mote.attachedDrawOffset;
			}
		}
		exactPosition.y = def.altitudeLayer.AltitudeFor();
	}
}
