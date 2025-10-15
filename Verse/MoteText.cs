using UnityEngine;

namespace Verse;

public class MoteText : MoteThrown
{
	public string text;

	public Color textColor = Color.white;

	public float overrideTimeBeforeStartFadeout = -1f;

	protected float TimeBeforeStartFadeout
	{
		get
		{
			if (!(overrideTimeBeforeStartFadeout >= 0f))
			{
				return base.SolidTime;
			}
			return overrideTimeBeforeStartFadeout;
		}
	}

	protected override bool EndOfLife => base.AgeSecs >= TimeBeforeStartFadeout + def.mote.fadeOutTime;

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
	}

	public override void DrawGUIOverlay()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - (base.AgeSecs - TimeBeforeStartFadeout) / def.mote.fadeOutTime;
		Color val = default(Color);
		((Color)(ref val))._002Ector(textColor.r, textColor.g, textColor.b, num);
		GenMapUI.DrawText(new Vector2(exactPosition.x, exactPosition.z), text, val);
	}
}
