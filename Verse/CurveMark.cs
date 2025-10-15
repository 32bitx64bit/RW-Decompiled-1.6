using UnityEngine;

namespace Verse;

public struct CurveMark
{
	private float x;

	private string message;

	private Color color;

	public float X => x;

	public string Message => message;

	public Color Color => color;

	public CurveMark(float x, string message, Color color)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.x = x;
		this.message = message;
		this.color = color;
	}
}
