using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompFleshmassBase : ThingComp
{
	[Unsaved(false)]
	private static List<Graphic> cachedBaseGFX;

	[Unsaved(false)]
	private static List<Graphic> cachedOutlineGFX;

	private CompProperties_FleshmassBase Props => (CompProperties_FleshmassBase)props;

	public List<Graphic> CachedBaseGFX
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (cachedBaseGFX == null)
			{
				cachedBaseGFX = new List<Graphic>
				{
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_1x1A", ShaderDatabase.Cutout, new Vector2(2.3f, 2.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_1x1B", ShaderDatabase.Cutout, new Vector2(2.3f, 2.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_2x2A", ShaderDatabase.Cutout, new Vector2(3.3f, 3.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_2x2B", ShaderDatabase.Cutout, new Vector2(3.3f, 3.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_3x3A", ShaderDatabase.Cutout, new Vector2(4.3f, 4.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_3x3B", ShaderDatabase.Cutout, new Vector2(4.3f, 4.3f), Color.white)
				};
			}
			return cachedBaseGFX;
		}
	}

	public List<Graphic> CachedOutlineGFX
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (cachedOutlineGFX == null)
			{
				cachedOutlineGFX = new List<Graphic>
				{
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_1x1A_Outline", ShaderDatabase.Cutout, new Vector2(2.3f, 2.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_1x1B_Outline", ShaderDatabase.Cutout, new Vector2(2.3f, 2.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_2x2A_Outline", ShaderDatabase.Cutout, new Vector2(3.3f, 3.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_2x2B_Outline", ShaderDatabase.Cutout, new Vector2(3.3f, 3.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_3x3A_Outline", ShaderDatabase.Cutout, new Vector2(4.3f, 4.3f), Color.white),
					GraphicDatabase.Get<Graphic_Single>("Things/Building/Fleshmass/Bases/FleshmassBase_3x3B_Outline", ShaderDatabase.Cutout, new Vector2(4.3f, 4.3f), Color.white)
				};
			}
			return cachedOutlineGFX;
		}
	}

	public override void PostDraw()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		int index = (Props.size - 1) * 2 + parent.thingIDNumber % 2;
		Rot4 rot = new Rot4(parent.thingIDNumber % 8 / 2);
		CachedBaseGFX[index].Draw(parent.DrawPos.WithYOffset(0.01f), rot, parent);
		CachedOutlineGFX[index].Draw(parent.DrawPos.WithYOffset(-0.01f), rot, parent);
	}
}
