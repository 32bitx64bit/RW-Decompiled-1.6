using UnityEngine;

namespace Verse;

internal sealed class DebugCell
{
	public IntVec3 c;

	public string displayString;

	public float colorPct;

	public int ticksLeft;

	public Material customMat;

	public void Draw()
	{
		if ((Object)(object)customMat != (Object)null)
		{
			CellRenderer.RenderCell(c, customMat);
		}
		else
		{
			CellRenderer.RenderCell(c, colorPct);
		}
	}

	public void OnGUI()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (displayString != null)
		{
			Vector2 val = c.ToUIPosition();
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector(val.x - 20f, val.y - 20f, 40f, 40f);
			Rect val3 = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
			if (((Rect)(ref val3)).Overlaps(val2))
			{
				Widgets.Label(val2, displayString);
			}
		}
	}
}
