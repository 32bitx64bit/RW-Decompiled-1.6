using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class TemporaryThingDrawer : IExposable
{
	private List<TemporaryThingDrawable> drawables = new List<TemporaryThingDrawable>();

	public void Tick()
	{
		for (int num = drawables.Count - 1; num >= 0; num--)
		{
			TemporaryThingDrawable temporaryThingDrawable = drawables[num];
			if (temporaryThingDrawable.ticksLeft >= 0 && temporaryThingDrawable.thing != null)
			{
				temporaryThingDrawable.ticksLeft--;
			}
			else
			{
				drawables.RemoveAt(num);
			}
		}
	}

	public void Draw()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (TemporaryThingDrawable drawable in drawables)
		{
			drawable.thing.DrawNowAt(drawable.position);
		}
	}

	public void AddThing(Thing t, Vector3 position, int ticks)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		drawables.Add(new TemporaryThingDrawable
		{
			thing = t,
			position = position,
			ticksLeft = ticks
		});
	}

	public void ExposeData()
	{
		Scribe_Collections.Look(ref drawables, "drawables", LookMode.Undefined);
	}
}
