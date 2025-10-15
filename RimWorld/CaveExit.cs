using UnityEngine;
using Verse;

namespace RimWorld;

public class CaveExit : PocketMapExit
{
	private static readonly Vector3 RopeDrawOffset = new Vector3(0f, 1f, 1f);

	[Unsaved(false)]
	private Graphic cachedRopeGraphic;

	private Graphic RopeGraphic
	{
		get
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (cachedRopeGraphic == null)
			{
				cachedRopeGraphic = GraphicDatabase.Get<Graphic_Single_AgeSecs>("Things/Building/Misc/CaveExit/CaveExit_Rope", ShaderDatabase.CaveExitRope, def.graphicData.drawSize, Color.white);
			}
			return cachedRopeGraphic;
		}
	}

	protected override void DrawAt(Vector3 drawLoc, bool flip = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		RopeGraphic.Draw(DrawPos + RopeDrawOffset, Rot4.North, this);
	}
}
