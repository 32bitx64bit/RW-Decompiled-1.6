using UnityEngine;

namespace Verse;

public struct FleckDrawPosition
{
	public Vector3 worldPosition;

	public float height;

	public Vector3 anchorOffset;

	public Vector3 unattachedDrawOffset;

	public Vector3 attachedOffset;

	public Vector3 ExactPosition => worldPosition + unattachedDrawOffset + attachedOffset + Vector3.forward * height + anchorOffset;

	public FleckDrawPosition(Vector3 worldPos, float height, Vector3 anchorOffset, Vector3 unattachedOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		worldPosition = worldPos;
		this.height = height;
		this.anchorOffset = anchorOffset;
		unattachedDrawOffset = unattachedOffset;
		attachedOffset = Vector3.zero;
	}

	public FleckDrawPosition(Vector3 worldPos, float height, Vector3 anchorOffset, Vector3 unattachedOffset, Vector3 attachedOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		worldPosition = worldPos;
		this.height = height;
		this.anchorOffset = anchorOffset;
		unattachedDrawOffset = unattachedOffset;
		this.attachedOffset = attachedOffset;
	}
}
