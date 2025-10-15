using UnityEngine;

namespace LudeonTK;

public static class KeyFrameExtensions
{
	public static Vector2 ToV2(this Keyframe kf)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(((Keyframe)(ref kf)).time, ((Keyframe)(ref kf)).value);
	}

	public static Vector4 ToVector4(this Keyframe kf)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector4(((Keyframe)(ref kf)).time, ((Keyframe)(ref kf)).value, ((Keyframe)(ref kf)).inTangent, ((Keyframe)(ref kf)).outTangent);
	}

	public static Keyframe ToKeyframe(this Vector4 kf)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Keyframe(kf.x, kf.y, kf.z, kf.w);
	}

	public static Keyframe ToKeyframe(this Vector2 v2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Keyframe(v2.x, v2.y);
	}
}
