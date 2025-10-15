using UnityEngine;

namespace RimWorld;

public struct WornGraphicBodyTypeData
{
	public Vector2 offset;

	public Vector2? scale;

	public Vector2 Scale => (Vector2)(((_003F?)scale) ?? Vector2.one);
}
