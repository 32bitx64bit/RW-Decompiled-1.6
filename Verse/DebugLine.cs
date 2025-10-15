using UnityEngine;

namespace Verse;

internal struct DebugLine
{
	public Vector3 a;

	public Vector3 b;

	private int deathTick;

	private SimpleColor color;

	public bool Done => deathTick <= Find.TickManager.TicksGame;

	public DebugLine(Vector3 a, Vector3 b, int ticksLeft = 100, SimpleColor color = SimpleColor.White)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		this.a = a;
		this.b = b;
		deathTick = Find.TickManager.TicksGame + ticksLeft;
		this.color = color;
	}

	public void Draw()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GenDraw.DrawLineBetween(a, b, color);
	}
}
