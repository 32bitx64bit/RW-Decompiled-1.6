using UnityEngine;

namespace Verse;

public struct CurvePoint
{
	private Vector2 loc;

	public Vector2 Loc => loc;

	public float x => loc.x;

	public float y => loc.y;

	public CurvePoint(float x, float y)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		loc = new Vector2(x, y);
	}

	public CurvePoint(Vector2 loc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.loc = loc;
	}

	public static CurvePoint FromString(string str)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new CurvePoint(ParseHelper.FromString<Vector2>(str));
	}

	public override string ToString()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return loc.ToStringTwoDigits();
	}

	public static implicit operator Vector2(CurvePoint pt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pt.loc;
	}
}
